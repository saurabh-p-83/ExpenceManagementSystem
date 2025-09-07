using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Invoices
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string Vendor { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public InvoiceCategory Category { get; set; }
        public string? Description { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }

    public class PostInvoiceDto
    {
        public IFormFile BillFile { get; set; } = default!;
    }
    public class GetInvoiceDtoReq
    {
        public List<Guid> UserIds { get; set; }
    }
    public class GetInvoiceDtoRes
    {
        public Guid Id { get; set; }
        public string Vendor { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string? Description { get; set; }
        public Guid UserId { get; set; }
        public string? FileUrl { get; set; }
    }
}