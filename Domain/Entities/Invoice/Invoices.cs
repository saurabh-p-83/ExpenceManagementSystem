using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Invoice
{
    public class Invoices
    {
        public Guid Id { get; set; }
        public string Vendor { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public InvoiceCategory Category { get; set; }
        public string? Description { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
    }





}
