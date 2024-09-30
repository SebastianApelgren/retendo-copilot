using Microsoft.AspNetCore.Mvc;
using RetendoCopilotApi.Helpers;
using RetendoCopilotApi.Models;
using RetendoCopilotChatbot;
using RetendoCopilotChatbot.Models;
using RetendoDataHandler;
using RetendoDataHandler.Helper;

namespace RetendoCopilotApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpPost("GetChatResponse")]
        public async Task<OutputBody> GetChatResponse([FromBody] UserInputBodyChat body)
        {
            //API endpoint to get chat response from input body.

            List<ChatMessage> chatMessages = ContextHelper.DeserializeAndConvertToString(body.Context);

            AwsHelper awsHelper = new AwsHelper(
                EnvironmentVariableHelper.GetModelId(),
                EnvironmentVariableHelper.GetRegion(),
                EnvironmentVariableHelper.GetKnowledgeBaseIdManual(),
                EnvironmentVariableHelper.GetKnowledgeBaseIdTickets(),
                EnvironmentVariableHelper.GetAwsAccessKey(),
                EnvironmentVariableHelper.GetAwsSecretAccessKey(),
                EnvironmentVariableHelper.GetGuardrailIdentifier(),
                EnvironmentVariableHelper.GetGuardrailVersion()
            );

            Copilot copilot = new Copilot(awsHelper);

            ChatResponse response = await copilot.GetChatResponseAsync(body.UserMessage, chatMessages, body.NumberOfResultsManuals, body.NumberOfResultsTickets);

            string serializedContext = ContextHelper.SerializeAndConvertToBase64(chatMessages);

            OutputBody outputBody = new OutputBody(response, serializedContext);

            return outputBody;
        }

        [HttpPost("UploadSupportTickets")]
        public async Task<bool> UploadSupportTickets([FromBody] UserInputBodyTickets body)
        {
            //API endpoint to upload support tickets to S3.

            AwsS3Helper awsS3Helper = new AwsS3Helper(
                EnvironmentVariableHelper.GetRegion(),
                EnvironmentVariableHelper.GetAwsAccessKey(),
                EnvironmentVariableHelper.GetAwsSecretAccessKey()
            );

            DataHandler dataHandler = new DataHandler(awsS3Helper);

            bool success = await dataHandler.UploadTickets(ticketsRaw:body.SupportTickets);

            return success;
        }
    }
}
