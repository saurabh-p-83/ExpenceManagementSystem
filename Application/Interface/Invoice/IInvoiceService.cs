using Application.DTOs.Invoices;

namespace Application.Interface.Invoice;

public interface IInvoiceService
{
    Task<IEnumerable<GetInvoiceDtoRes>> GetInvoiceAsync(GetInvoiceDtoReq input);
    Task<Guid> SaveInvoiceAsync(PostInvoiceDto dto, Guid userId);
}