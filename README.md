# Blackbird.io Apertera

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Apertera (powered by Alexa Translations AI) is a neural machine translation service built for regulated industries. This app integrates Apertera's translation capabilities into Blackbird workflows, enabling both text and file translation with full Blackbird interoperability support.

## Before setting up

Sign in to your [Alexa Translations](https://ai.alexatranslations.com) account and obtain your API key and username from your account settings or contact your Apertera administrator.

## Connecting

1. Navigate to Apps and search for **Apertera**.
2. Click **Add connection**.
3. Name your connection and fill in the following fields:
   - **API key** — your Alexa Translations API key.
   - **Username** — your Alexa Translations username.
4. Click **Connect**.

## Actions

### Translate text

Translates a plain-text string using Alexa Translations AI.

| Input | Description |
|-------|-------------|
| **Text** | The text to translate. |
| **Source language** | ISO 639-2/B code of the source language (e.g. `eng`). |
| **Target language** | ISO 639-2/B code of the target language (e.g. `fra`). |

| Output | Description |
|--------|-------------|
| **Translated text** | The translated text. |

### Translate

Translates a file using Alexa Translations AI. Supports two strategies selectable via **File translation strategy**:

**Blackbird interoperability mode** (default) — Blackbird parses the file into an internal XLIFF representation, extracts translatable segments, sends them to Alexa Translations, and returns the result. Supported input formats: HTML, DOCX, PPTX, XLIFF, XML, TXT. The output format is controlled by **Output file handling**.

**Apertera native mode** — The file is sent directly to Alexa Translations' document translation endpoint. Supports HTML, DOCX, PPTX, TXT, and PDF.

| Input | Description |
|-------|-------------|
| **File** | The file to translate. |
| **Source language** | ISO 639-2/B code of the source language (e.g. `eng`). Required. |
| **Target language** | ISO 639-2/B code of the target language (e.g. `fra`). |
| **File translation strategy** | `Blackbird interoperability` (default) or `Apertera native`. |
| **Output file handling** | Blackbird mode only. `Interoperable XLIFF` (default, returns XLIFF 2.0), `XLIFF 1.2`, or `Original format` (returns the source file type with translations applied). |
| **Project ID** | Optional. Native mode only. Associate the translation job with a specific Apertera project. |

| Output | Description |
|--------|-------------|
| **File** | The translated file. |

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
