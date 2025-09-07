using Application.Interface.Azure;
using Application.Interface.NewFolder;
using Azure;
using Azure.AI.DocumentIntelligence;
using Domain.Entities.Invoice;
using Domain.Enums;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class OcrInvoiceService : IOcrInvoiceService
    {
        private readonly DocumentIntelligenceClient _client;
        private readonly ICategorizationService _categorizationService;

        public OcrInvoiceService(
            IOptions<AzureDocumentIntelligenceSettings> settings,
            ICategorizationService categorizationService)
        {
            var config = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _categorizationService = categorizationService ?? throw new ArgumentNullException(nameof(categorizationService));

            var credential = new AzureKeyCredential(config.ApiKey);
            _client = new DocumentIntelligenceClient(new Uri(config.Endpoint), credential);
        }

        public async Task<Invoices> ExtractInvoiceDataAsync(Stream fileStream, string fileName)
        {
            var operation = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                "prebuilt-invoice",
                BinaryData.FromStream(fileStream));

            return await MapToInvoiceAsync(operation.Value, fileName);
        }

        public async Task<Invoices> ExtractInvoiceDataFromBlobAsync(string fileUrl)
        {
            var operation = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                "prebuilt-invoice",
                new Uri(fileUrl));

            return await MapToInvoiceAsync(operation.Value, fileUrl);
        }

        private async Task<Invoices> MapToInvoiceAsync(AnalyzeResult result, string fileReference)
        {
            var doc = result.Documents.FirstOrDefault();
            if (doc == null)
                throw new Exception("Invoice data could not be extracted.");

            var fields = doc.Fields;

            string vendor = ExtractVendor(fields);
            decimal amount = ExtractAmount(fields);
            DateTime date = ExtractDate(fields);
            string customerName = ExtractCustomerName(fields);

            var category = await _categorizationService.CategorizeWithOpenAPIAsync(vendor, customerName);

            return new Invoices
            {
                Id = Guid.NewGuid(),
                Vendor = vendor,
                Amount = amount,
                Date = date,
                Category = category,
                Description = customerName,
                FileUrl = fileReference
            };
        }

        private string ExtractVendor(IReadOnlyDictionary<string, DocumentField> fields)
        {
            return fields.TryGetValue("VendorName", out var vendorField)
                ? vendorField.Content ?? "Unknown"
                : "Unknown";
        }

        private decimal ExtractAmount(IReadOnlyDictionary<string, DocumentField> fields)
        {
            if (fields.TryGetValue("InvoiceTotal", out var totalField))
            {
                if (totalField.ValueCurrency is not null)
                    return Convert.ToDecimal(totalField.ValueCurrency.Amount);

                if (decimal.TryParse(totalField.Content, out var parsed))
                    return parsed;
            }
            return 0;
        }

        private DateTime ExtractDate(IReadOnlyDictionary<string, DocumentField> fields)
        {
            if (fields.TryGetValue("InvoiceDate", out var dateField))
            {
                if (dateField.ValueDate is not null)
                    return dateField.ValueDate.Value.DateTime;

                if (DateTime.TryParse(dateField.Content, out var parsed))
                    return parsed;
            }
            return DateTime.UtcNow;
        }

        private string ExtractCustomerName(IReadOnlyDictionary<string, DocumentField> fields)
        {
            return fields.TryGetValue("CustomerName", out var custField)
                ? custField.Content ?? string.Empty
                : string.Empty;
        }
    }
}