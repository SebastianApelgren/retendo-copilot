namespace RetendoCopilotApi.Helpers
{
    public class EnvironmentVariableHelper
    {
        private const string AwsAccessKeyId = "AWS_ACCESS_KEY_ID";
        private const string AwsSecretAccessKey = "AWS_SECRET_ACCESS_KEY";
        private const string ModelId = "MODEL_ID";
        private const string Region = "REGION";
        private const string KnowledgeBaseIdManual = "KNOWLEDGE_BASE_ID_MANUAL";
        private const string KnowledgeBaseIdTickets = "KNOWLEDGE_BASE_ID_TICKETS";
        private const string GuardrailIdentifier = "GUARDRAIL_IDENTIFIER";
        private const string GuardrailVersion = "GUARDRAIL_VERSION";

        public static string GetAwsAccessKey()
        {
            return GetVariable(AwsAccessKeyId);
        }

        public static string GetAwsSecretAccessKey()
        {
            return GetVariable(AwsSecretAccessKey);
        }

        public static string GetGuardrailIdentifier()
        {
            return GetVariable(GuardrailIdentifier);
        }

        public static string GetGuardrailVersion()
        {
            return GetVariable(GuardrailVersion);
        }

        public static string GetKnowledgeBaseIdManual()
        {
            return GetVariable(KnowledgeBaseIdManual);
        }

        public static string GetKnowledgeBaseIdTickets()
        {
            return GetVariable(KnowledgeBaseIdTickets);
        }

        public static string GetModelId()
        {
            return GetVariable(ModelId);
        }

        public static string GetRegion()
        {
            return GetVariable(Region);
        }

        public static string GetVariable(string name)
        {
            string? variable = Environment.GetEnvironmentVariable(name);

            if (string.IsNullOrEmpty(variable))
                throw new InvalidOperationException($"Missing {name} in environment variables");

            return variable;
        }
    }
}
