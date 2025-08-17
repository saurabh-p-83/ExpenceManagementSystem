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
        Task<IEnumerable<Invoice>> GetInvoicesAsync(Guid userId);
        Task<Invoice> GetByIdAsync(Guid id);
        Task<Guid> AddInvoiceAsync(Invoice invoice);
        Task SaveChangesAsync();
    }
}
