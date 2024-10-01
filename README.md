# What is this

This repository contains three projects.

## RetendoCopilotApi

This is an REST API that offers 2 endpoints. One for getting a chat responce from a support chatbot and the other to upload tickets to the database that the chatbot is based on.

### Usage

There is two API endpoint in this project. The first is "/GetChatResponse" which takes in a support question and returns the response and the context of the conversation, which is then used in the next request if you want to continue on the same conversation. You will also get returned some information about the cost of the call and how long each call to the LLM took.

When using the API, you can use the following JSON object as an example request body:

```json
{
  "userMessage": "String",
  "context": "String",
  "numberOfResultsManuals": "int",  "// Optional (default: 3)"
  "numberOfResultsTickets": "int"   "// Optional (default: 5)"
}
```

Here is an return object for the API:

```json
{
  "responseMessage": "string",
  "costInformation": {
    "// Represents an object of type CostInformation"
  },
  "timings": [
    {
      "// Represents an object of type TimingEntry"
    }
  ],
  "context": "string"
}
```

The second API endpoint is "/UploadSupportTickets" and is used to upload tickets to the AWS S3 bucket with tickets. The input is a list of JSON objects that are the raw support tickets from Retendo's support ticket provider. The return is a JSON object with a bool that is true or false if tickets got uploaded and a message regarding how many tiuckets got uploaded of them you sent.

Here is how a JSON object looks like for this API endpoint:

```json
{
  "supportTickets": ["rawJSONForSupportTicketFromProvider"]
}
```

Here is a return object of this API endpoint:

```json
{
  "success": "boolean",
  "message": "string"
}
```

## RetendoCopilotChatbot

This project is a library that helps to interact with AWS. It is used to create a chatbot based on the RAG model. 

### Usage

To use this library you create an object of the Copilot class. To do that you first need an object for the AwsHelper class. Here is how you initialize them:

```csharp
AwsHelper awsHelper = new AwsHelper(
    modId: "your-module-id",
    region: "your-region",
    kbIdManual: "your-kb-id-manual",
    kbIdTicket: "your-kb-id-ticket",
    awsAccessKeyId: "your-access-key-id",
    awsSecretAccessKey: "your-secret-access-key",
    guardrailIdentifier: "your-guardrail-identifier",
    guardrailVersion: "your-guardrail-version"
);
Copilot copilot = new Copilot(awsHelper);
```
You then call "GetChatResponseAsync" in the copilot class to generate a chat response based on the user query and chat history. "chatMessages" is the chat history, it will be returned to the user and doesn't need any modifying at all (it will be empty when doing the first request).

```csharp
public async Task<ChatResponse> GetChatResponseAsync(string query, List<ChatMessage> chatMessages, int numberOfResultsManuals = 3, int numberOfResultsTickets = 5)
```

## RetendoDataHandler

This is a library to upload data in bulk, more precisly tickets, to a AWS S3 bucket. It also removes some personal data, however, not at all enough to be a reliable personal data remover.

### Usage



# Why this repository

This repository will be used by Retendo to create a support chatbot which will be deployed to their users. 