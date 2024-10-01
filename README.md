# Retendo Copilot

## What is this

This repository contains three projects.

### RetendoCopilotApi

This is an REST API that offers 2 endpoints. One for getting a chat responce from a support chatbot and the other to upload tickets to the database that the chatbot is based on.

#### Usage

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

### RetendoCopilotChatbot

This project is a library that helps to interact with AWS. It is used to create a chatbot based on the RAG model. 

#### Usage

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
You then call "GetChatResponseAsync" in the Copilot class to generate a chat response based on the user query and chat history. "chatMessages" is the chat history, it will be returned to the user and doesn't need any modifying at all (it will be empty when doing the first request).

```csharp
public async Task<ChatResponse> GetChatResponseAsync(string query, List<ChatMessage> chatMessages, int numberOfResultsManuals = 3, int numberOfResultsTickets = 5)
```

If the chatbot doesn't think it can answer the query because it is too personal or inappropriate it will return the class variable "cantHelpMessage" in the Copilot class. It is in swedish right now but can be changed to a code to then be implemented in the frontend if the user want's it.

### RetendoDataHandler

This is a library to upload data in bulk, more precisly tickets, to a AWS S3 bucket. It also removes some personal data, however, not at all enough to be a reliable personal data remover.

#### Usage

To use this library you need two objects, first an object in the AwsS3Helper class and then a DataHandler object. Here is how you create these:

```csharp
AwsS3Helper awsS3Helper = new AwsS3Helper(
    region: "your-region",
    awsAccessKey: "your-aws-access-key",
    awsSecretAccessKey: "your-aws-secret-access-key",
    bucketName: "your-bucket-name"
);
DataHandler dataHandler = new DataHandler(awsS3Helper);
```

You then call "UploadTickets" in the DataHandler class to upload tickets to the S3 bucket. You can either send in a path to files you want to upload. These files have to be row-wise filled with JSON object that are support tickets (that's how a support ticket export is done by Retendo's support provider). Otherwise, you can send in a list of rawSupportTickets which are just deserialized JSON support tickets. Here is the methods header:

```csharp
public async Task<UploadTicketResult> UploadTickets(string dataPath = null, List<SupportTicketRaw> ticketsRaw = null)
```

## Why this repository

This repository will be used by Retendo to create a support chatbot which will be deployed to their users. It will help them with most support tickets and facilitate for their team and save them time.
