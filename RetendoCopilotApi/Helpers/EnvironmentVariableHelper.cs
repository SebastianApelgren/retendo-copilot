namespace RetendoCopilotApi.Helpers
{
    public class EnvironmentVariableHelper
    {
        private const string AwsAccessKeyId = "AWS_ACCESS_KEY_ID";
        private const string AwsSecretAccessKey = "AWS_SECRET_ACCESS_KEY";
        private const string ModelId = "MODEL_ID";
        private const string Region = "REGION";
        private const string KnowledgeBaseId = "KNOWLEDGE_BASE_ID";

        public static string GetAwsAccessKey()
        {
            return GetVariable(AwsAccessKeyId);
        }

        public static string GetAwsSecretAccessKey()
        {
            return GetVariable(AwsSecretAccessKey);
        }

        public static string GetKnowledgeBaseId()
        {
            return GetVariable(KnowledgeBaseId);
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
