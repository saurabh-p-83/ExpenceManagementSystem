using Domain.Entities.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Invoice;
namespace Application.Interface
{
    public interface IOcrInvoiceService
    {
        Task<Invoices> ExtractInvoiceDataAsync(Stream fileStream, string fileName);
        Task<Invoices> ExtractInvoiceDataFromBlobAsync(string fileUrl);
    }
}
