using Domain.Entities.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Invoice
{
    public interface IInvoiceService
    {
        Task<IEnumerable<GetInvoiceDtoRes>> GetInvoiceAsync(GetInvoiceDtoReq input);
        Task<Guid> SaveInvoiceAsync(PostInvoiceDto dto, Guid userId);
    }
}
