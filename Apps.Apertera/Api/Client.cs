using System.Net;
using Apps.Apertera.Constants;
using Apps.Apertera.Models.Dtos;
using Apps.Apertera.Models.Entities;
using Apps.Apertera.Models.Requests;
using Apps.Apertera.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Apertera.Api;

public class Client : BlackBirdRestClient
{
    private readonly string _apiKey;
    private readonly string _username;

    public Client(IEnumerable<AuthenticationCredentialsProvider> creds) : base(new RestClientOptions
    {
        BaseUrl = new Uri("https://ai.alexatranslations.com"),
        Timeout = TimeSpan.FromSeconds(300),
    })
    {
        _apiKey = creds.Get(CredsNames.ApiKey).Value;
        _username = creds.Get(CredsNames.Username).Value;
    }

    public async Task<TextTranslationResultDto> TranslateTextAsync(
        IEnumerable<string> inputs, string from, string to, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(ApiConstants.TextTranslationResource, Method.Post);
        request.AddParameter("credential", _apiKey);
        request.AddParameter("from", from);
        request.AddParameter("to", to);
        foreach (var input in inputs)
            request.AddParameter("input", input);

        return await ExecuteWithErrorHandling<TextTranslationResultDto>(request);
    }

    public async Task<Stream> TranslateDocumentAsync(
        DocumentTranslationDto req, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(ApiConstants.DocumentTranslationResource, Method.Post)
            .AddFile("file", req.FileContent, req.FileName)
            .AddParameter("api_key", _apiKey)
            .AddParameter("username", _username)
            .AddParameter("source_lang", req.SourceLanguage)
            .AddParameter("target_lang", req.TargetLanguage);
        
        if (!string.IsNullOrWhiteSpace(req.ProjectId))
            request.AddParameter("project_id", req.ProjectId);
        foreach (var (key, value) in req.FormatFlags)
            request.AddParameter(key, value);

        var response = await ExecuteAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw ConfigureErrorException(response);

        return new MemoryStream(response.RawBytes ?? []);
    }

    public static string ExtractErrorMessage(RestResponse response)
    {
        if (string.IsNullOrWhiteSpace(response.Content))
            return response.ErrorMessage ?? response.StatusDescription ?? response.StatusCode.ToString();

        try
        {
            var token = JToken.Parse(response.Content);
            return token["message"]?.ToString()
                ?? token["error"]?.ToString()
                ?? token["detail"]?.ToString()
                ?? response.Content;
        }
        catch (JsonReaderException)
        {
            return response.Content;
        }
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var message = ExtractErrorMessage(response);
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            return new PluginApplicationException($"Authentication failed: {message}");

        return new PluginApplicationException(message);
    }
}
