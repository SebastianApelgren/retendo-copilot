# What is this

This repository contains three projects.

## RetendoCopilotApi

This is an REST API that offers 2 endpoints. One for getting a chat responce from a support chatbot and the other to upload tickets to the database that the chatbot is based on.

### How to use it

There is two API endpoint in this project. The first is "/GetChatResponse" which takes in a support question and returns the response and the context of the conversation, which is then used in the next request if you want to continue on the same conversation. You will also get returned some information about the cost of the call and how long each call to the LLM took.

When using the API, you can use the following JSON object as an example request body:

```json
{
  "userMessage": "String",
  "context": "String",
  "numberOfResultsManuals": **int**,  // Optional (default: 3)
  "numberOfResultsTickets": int   // Optional (default: 5)
}
```

The second API endpoint is "/UploadSupportTickets" and is used to upload tickets to the AWS S3 bucket with tickets. The input is a list of JSON objects that are the raw support tickets from Retendo's support ticket provider. The return is a JSON object with a bool that is true or false if tickets got uploaded and a message regarding how many tiuckets got uploaded of them you sent.

Here is how a JSON object looks like for this API endpoint:

```json
{
  "supportTickets": [**rawSupportTicketFromProvider**]
}
```

## RetendoCopilotChatbot

This project is a library that helps to nteract with AWS. It is used to create a chatbot based on the RAG model.

## RetendoDataHandler

This is a library to upload data in bulk, more precisly tickets, to a AWS S3 bucket. It also removes some personal data, however, it is very primitive.

# Why this repository

This repository will be used by Retendo to create a support chatbot which will be deployed to their users. 