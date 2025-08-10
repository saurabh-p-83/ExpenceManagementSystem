using Domain.Entities.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Invoice
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<InvoiceDto>> GetInvoicesAsync(Guid userId);
        Task<InvoiceDto> GetByIdAsync(Guid id);
        Task<Guid> AddInvoiceAsync(InvoiceDto invoice);
        Task SaveChangesAsync();
    }
}
