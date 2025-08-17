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
        Task<IEnumerable<Invoices>> GetInvoicesAsync(Guid userId);
        Task<Invoices> GetByIdAsync(Guid id);
        Task<Guid> AddInvoiceAsync(Invoices invoice);
        Task SaveChangesAsync();
    }
}
