using Application.Interface;
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

        public OcrInvoiceService(IOptions<AzureDocumentIntelligenceSettings> settings)
        {
            var config = settings.Value ?? throw new ArgumentNullException(nameof(settings));

            var credential = new AzureKeyCredential(config.ApiKey);
            _client = new DocumentIntelligenceClient(new Uri(config.Endpoint), credential);
        }

        /// <summary>
        /// Extracts invoice data from a file stream
        /// </summary>
        public async Task<Invoices> ExtractInvoiceDataAsync(Stream fileStream, string fileName)
        {
            var operation = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                "prebuilt-invoice",
                BinaryData.FromStream(fileStream));
            return MapToInvoice(operation.Value, fileName);
        }

        /// <summary>
        /// Extracts invoice data from a file already uploaded to Blob Storage.
        /// </summary>
        public async Task<Invoices> ExtractInvoiceDataFromBlobAsync(string fileUrl)
        {
            var operation = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed,
                "prebuilt-invoice",
                new Uri(fileUrl));

            return MapToInvoice(operation.Value, fileUrl);
        }

        private Invoices MapToInvoice(AnalyzeResult result, string fileReference)
        {
            var doc = result.Documents.FirstOrDefault();
            if (doc == null)
                throw new Exception("Invoice data could not be extracted.");

            var fields = doc.Fields;

            string vendor = fields.TryGetValue("VendorName", out var vendorField)
                ? vendorField.Content
                : "Unknown";

            decimal amount = 0;
            if (fields.TryGetValue("InvoiceTotal", out var totalField))
            {
                if (totalField.ValueCurrency is not null)
                    amount = Convert.ToDecimal(totalField.ValueCurrency.Amount);
                else if (decimal.TryParse(totalField.Content, out var parsed))
                    amount = parsed;
            }

            DateTime date = DateTime.UtcNow;
            if (fields.TryGetValue("InvoiceDate", out var dateField))
            {
                if (dateField.ValueDate is not null)
                    date = dateField.ValueDate.Value.DateTime;
                else if (DateTime.TryParse(dateField.Content, out var parsed))
                    date = parsed;
            }

            return new Invoices
            {
                Id = Guid.NewGuid(),
                Vendor = vendor,
                Amount = amount,
                Date = date,
                Category = CategorizeVendor(vendor),
                Description = fields.TryGetValue("CustomerName", out var custField) ? custField.Content : "",
                FileUrl = fileReference
            };
        }

        private InvoiceCategory CategorizeVendor(string vendor)
        {
            vendor = vendor.ToLowerInvariant();

            if (vendor.Contains("airlines") || vendor.Contains("hotel") || vendor.Contains("travel"))
                return InvoiceCategory.Travel;

            if (vendor.Contains("food") || vendor.Contains("restaurant") || vendor.Contains("cafe") || vendor.Contains("pizza"))
                return InvoiceCategory.Food;

            return InvoiceCategory.Other;
        }
    }
}