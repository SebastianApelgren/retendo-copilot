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


## AwsHelper Class

The `AwsHelper` class is designed to interact with the AWS Bedrock API. It provides methods to retrieve relevant contexts from AWS based on a query and to generate responses using a large language model (LLM). Below is a detailed description of its functionality:

### Constructor

```csharp
public AwsHelper(string modId, string region, string kbIdManual, string kbIdTicket, string awsAccessKeyId, string awsSecretAccessKey, string guardrailIdentifier, string guardrailVersion)
```

- Initializes the `AwsHelper` class with necessary AWS credentials and configuration parameters.

### Methods

#### RetrieveAsync

```csharp
public async Task<List<string>> RetrieveAsync(string query, int numberOfResultsManuals, int numberOfResultsTickets)
```

- Retrieves relevant contexts (manuals and tickets) from AWS based on the provided query.

#### GenerateResponseAsync

```csharp
public async Task<InvokeModelResult> GenerateResponseAsync(string query)
```

- Sends a single prompt to the LLM and generates a response for the given query.

#### GenerateConversationResponseAsync

```csharp
public async Task<ConversationResponse> GenerateConversationResponseAsync(List<ChatMessage> chatMessages, string prompt)
```

- Sends a complete conversation to the LLM and gets a response.

### Private Methods

#### CreateConverseRequest

```csharp
private ConverseRequest CreateConverseRequest(List<ChatMessage> chatMessages, string prompt)
```

- Creates a `ConverseRequest` object from a list of chat messages and a prompt to be sent to the conversation API.

#### CreateInvokeModelRequest

```csharp
private InvokeModelRequest CreateInvokeModelRequest(string query, int maxTokens = 4096, double temp = 0.5)
```

- Creates an `InvokeModelRequest` object from a query to be sent to the invoke API.

### Dependencies

- `Amazon.BedrockRuntime`
- `Amazon.BedrockAgentRuntime`
- `System.Text.Json`
- `RetendoCopilotChatbot.Models`

### Usage

To use the `AwsHelper` class, instantiate it with the required parameters and call the provided methods to interact with the AWS Bedrock API.

```csharp
var awsHelper = new AwsHelper(modId, region, kbIdManual, kbIdTicket, awsAccessKeyId, awsSecretAccessKey, guardrailIdentifier, guardrailVersion);
var contexts = await awsHelper.RetrieveAsync(query, numberOfResultsManuals, numberOfResultsTickets);
var response = await awsHelper.GenerateResponseAsync(query);
var conversationResponse = await awsHelper.GenerateConversationResponseAsync(chatMessages, prompt);
```

# RetendoDataHandler

## AwsS3Helper Class

The `AwsS3Helper` class is a utility class designed to facilitate interactions with Amazon Web Services (AWS) Simple Storage Service (S3). It provides methods to upload support tickets to a specified S3 bucket, create folders within the bucket, and check for the existence of folders. 

### Key Features

- **Upload Support Tickets**: The `UploadToS3` method uploads a list of support tickets to a specified bucket and path within the bucket.
- **Create Folders**: The `CreateS3Folder` method creates a new folder in a specified bucket and path.
- **Check Folder Existence**: The `FolderExists` method checks if a folder exists in a specified bucket and path.

### Constructor

```csharp
public AwsS3Helper(string region, string awsAccessKey, string awsSecretAccessKey)
```

- **region**: The AWS region where the S3 bucket is located.
- **awsAccessKey**: The AWS access key for authentication.
- **awsSecretAccessKey**: The AWS secret access key for authentication.

### Methods

- **UploadToS3**
    ```csharp
    public async Task<bool> UploadToS3(string bucketName, string pathInBucket, List<SupportTicket> tickets)
    ```
    Uploads support tickets to the specified S3 bucket and path.

- **CreateS3Folder**
    ```csharp
    public async Task CreateS3Folder(string bucketName, string path, string name)
    ```
    Creates a folder in the specified S3 bucket and path.

- **FolderExists**
    ```csharp
    public async Task<bool> FolderExists(string bucketName, string path, string name)
    ```
    Checks if a folder exists in the specified S3 bucket and path.

### Dependencies

- **AmazonS3Client**: Used to interact with AWS S3.
- **JsonSerializer**: Used to serialize support tickets to JSON format.

### Example Usage

```csharp
var awsS3Helper = new AwsS3Helper("us-west-2", "yourAccessKey", "yourSecretAccessKey");
await awsS3Helper.UploadToS3("yourBucketName", "yourPath", supportTickets);
await awsS3Helper.CreateS3Folder("yourBucketName", "yourPath", "newFolder");
bool exists = await awsS3Helper.FolderExists("yourBucketName", "yourPath", "folderName");
```
