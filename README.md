# RetendoCopilotChatbot

## Copilot Class

The `Copilot` class is the core component of the RetendoCopilotChatbot. It is responsible for generating responses to user queries by interacting with AWS services to retrieve relevant contexts and generate appropriate responses. Below is a detailed description of its functionality:

### Fields

- **awsHelper**: An instance of `AwsHelper` used to interact with AWS services.
- **cantHelpMessage**: A default message returned when the chatbot cannot answer a query.

### Constructor

- **Copilot(AwsHelper awsHelper)**: Initializes the `Copilot` class with an instance of `AwsHelper`.

### Methods

- **GetChatResponseAsync(string query, List<ChatMessage> chatMessages, int numberOfResultsManuals = 3, int numberOfResultsTickets = 5)**: 
    - Generates a response to a user query.
    - Determines if the query should be answered by the chatbot or searched in the documentation.
    - Retrieves relevant contexts from AWS and generates a response based on those contexts.
    - Returns a `ChatResponse` object containing the response, timing information, and cost information.

- **GetShouldSearchInDocumentationAsync(string query, CostInformation costInformation)**: 
    - Determines if the query should be answered by the chatbot or searched in the documentation.
    - Returns a string indicating the result.

- **GetShouldSearchInDocumentationOrShouldAnswerAsync(string query, CostInformation costInformation)**: 
    - Similar to `GetShouldSearchInDocumentationAsync` but also checks if the query is appropriate to answer.
    - Returns a string indicating the result.

### Usage

To use the `Copilot` class, create an instance by passing an `AwsHelper` object to the constructor. Then, call the `GetChatResponseAsync` method with the user query and other parameters to get a response.

```csharp
AwsHelper awsHelper = new AwsHelper();
Copilot copilot = new Copilot(awsHelper);
ChatResponse response = await copilot.GetChatResponseAsync("Your query here", chatMessages);
```

This class ensures that user queries are handled efficiently and appropriately, leveraging AWS services to provide accurate and relevant responses.











































