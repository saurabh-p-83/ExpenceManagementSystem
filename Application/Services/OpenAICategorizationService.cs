using Application.Interface.NewFolder;
using Azure;
using Azure.AI.OpenAI;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using System.ClientModel;

namespace Infrastructure.Services
{
    public class OpenAICategorizationService : ICategorizationService
    {
        private readonly AzureOpenAIClient _client;
        private readonly string _deploymentName;
        private readonly ILogger<OpenAICategorizationService> _logger;

        public OpenAICategorizationService(IConfiguration config,ILogger<OpenAICategorizationService> logger)
        {
            _logger = logger;

            var endpoint = config["AzureOpenAI:Endpoint"];
            var key = config["AzureOpenAI:ApiKey"];
            _deploymentName = config["AzureOpenAI:DeploymentName"] ?? "gpt-4o-mini";

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
            { 
                _client = null;
            }
            else
            {
                // Use AzureOpenAIClient 
                _client = new AzureOpenAIClient(new Uri(endpoint),new ApiKeyCredential(key));
            }
        }

        public async Task<InvoiceCategory> CategorizeWithOpenAPIAsync(string vendor, string description)
        {
            // Fallback to rule-based categorization if OpenAI client is not configured
            if (_client == null)
            {
                return CategorizeWithBasicRules(vendor, description);
            }

            try
            {
                // Get the chat client for the specific deployment
                var chatClient = _client.GetChatClient(_deploymentName);

                // Create messages using the new message types
                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage("You are a helpful invoice categorization AI."),
                    new UserChatMessage(CreateCategorizationPrompt(vendor, description))
                };

                // Use CompleteChatAsync instead of GetChatCompletionsAsync
                var response = await chatClient.CompleteChatAsync(messages);
                var categoryText = response.Value.Content[0].Text.Trim();

                return ParseCategory(categoryText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OpenAI categorization failed. Falling back to rule-based categorization.");
                return CategorizeWithBasicRules(vendor, description);
            }
        }

        private string CreateCategorizationPrompt(string vendor, string description)
        {
            return $@"
            You are an invoice categorization assistant.
            Given the following data:
            Vendor: {vendor}
            Description: {description}

            Pick one category from:
            - Food
            - Travel
            - Shopping
            - Utilities
            - Other

            Respond with only the category word.
            ";
        }

        private InvoiceCategory ParseCategory(string categoryText)
        {
            return categoryText.ToLower() switch
            {
                "food" => InvoiceCategory.Food,
                "travel" => InvoiceCategory.Travel,
                "shopping" => InvoiceCategory.Shopping,
                "utilities" => InvoiceCategory.Utilities,
                _ => InvoiceCategory.Other
            };
        }

        private InvoiceCategory CategorizeWithBasicRules(string vendor, string description)
        {
            vendor = vendor?.ToLowerInvariant() ?? string.Empty;
            description = description?.ToLowerInvariant() ?? string.Empty;

            if (vendor.Contains("amazon") || description.Contains("amazon"))
                return InvoiceCategory.Shopping;

            if (vendor.Contains("uber") || vendor.Contains("ola") || vendor.Contains("cab") ||
                vendor.Contains("taxi") || description.Contains("transport"))
                return InvoiceCategory.Travel;

            if (vendor.Contains("restaurant") || vendor.Contains("cafe") || vendor.Contains("hotel") ||
                vendor.Contains("food") || description.Contains("dining"))
                return InvoiceCategory.Food;

            if (vendor.Contains("electric") || vendor.Contains("water") || vendor.Contains("gas") ||
                vendor.Contains("internet") || vendor.Contains("phone"))
                return InvoiceCategory.Utilities;

            return InvoiceCategory.Other;
        }
    }
}