using Domain.Entities.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IOcrInvoiceService
    {
        Task<Invoice> ExtractInvoiceDataAsync(Stream fileStream, string fileName);
        Task<Invoice> ExtractInvoiceDataFromBlobAsync(string fileUrl);
    }
}
