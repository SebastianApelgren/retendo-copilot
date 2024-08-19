using Microsoft.AspNetCore.Mvc;
using RetendoCopilotApi.Helpers;
using RetendoCopilotApi.Models;
using RetendoCopilotChatbot;
using RetendoCopilotChatbot.Models;


namespace RetendoCopilotApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        [HttpPost("GetChatResponse")]
        public async Task<OutputBody> GetChatResponse([FromBody] UserInputBody body)
        {
            List<ChatMessage> chatMessages = ContextHelper.DeserializeAndConvertToString(body.Context);

            AwsHelper awsHelper = new AwsHelper(
                EnvironmentVariableHelper.GetModelId(),
                EnvironmentVariableHelper.GetRegion(),
                EnvironmentVariableHelper.GetKnowledgeBaseIdManual(),
                EnvironmentVariableHelper.GetKnowledgeBaseIdTickets(),
                EnvironmentVariableHelper.GetAwsAccessKey(),
                EnvironmentVariableHelper.GetAwsSecretAccessKey()
            );

            Copilot copilot = new Copilot(awsHelper);

            string response = await copilot.GetChatResponseAsync(body.UserMessage, chatMessages);

            string serializedContext = ContextHelper.SerializeAndConvertToBase64(chatMessages);

            OutputBody outputBody = new OutputBody(response, serializedContext);

            return outputBody;
        }
    }
}
