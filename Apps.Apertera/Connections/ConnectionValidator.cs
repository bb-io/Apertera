using System.Net;
using Apps.Apertera.Api;
using Apps.Apertera.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Apertera.Connections;

public class ConnectionValidator(InvocationContext invocationContext) : BaseInvocable(invocationContext), IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        var creds = authenticationCredentialsProviders.ToArray();
        var client = new Client(creds);
        var request = new RestRequest("/v2/translate/", Method.Post)
            .AddParameter("credential", creds.Get(CredsNames.ApiKey).Value)
            .AddParameter("from", "eng")
            .AddParameter("to", "fra")
            .AddParameter("input", "test");

        var response = await client.ExecuteAsync(request, cancellationToken);
        if (response.StatusCode is HttpStatusCode.Unauthorized
                                or HttpStatusCode.Forbidden
                                or HttpStatusCode.InternalServerError)
        {
            var msg = Client.ExtractErrorMessage(response);
            return new() { IsValid = false, Message = string.IsNullOrWhiteSpace(msg) ? "Invalid API key or username." : msg };
        }

        var body = response.Content ?? string.Empty;
        if (body.Contains("invalid api key", StringComparison.OrdinalIgnoreCase)
            || body.Contains("invalid credential", StringComparison.OrdinalIgnoreCase)
            || body.Contains("authentication failed", StringComparison.OrdinalIgnoreCase))
        {
            return new() { IsValid = false, Message = Client.ExtractErrorMessage(response) };
        }

        return new() { IsValid = true, Message = "Success" };
    }
}
