using Application.Interface;
using Domain.Entities.Invoice;
using Domain.Enums;


namespace Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<IEnumerable<GetInvoiceDtoRes>> GetInvoiceAsync(GetInvoiceDtoReq input)
        {
            var userIds = input.userId
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => Guid.Parse(id.Trim()))
                .ToList();

            var allInvoices = new List<InvoiceDto>();

            foreach (var userId in userIds)
            {
                var invoices = await _invoiceRepository.GetInvoicesAsync(userId);
                allInvoices.AddRange(invoices);
            }

            return allInvoices.Select(i => new GetInvoiceDtoRes
            {
                Id = i.Id,
                Vendor = i.Vendor,
                Amount = i.Amount,
                Date = i.Date,
                Category = i.Category.ToString(),
                Description = i.Description,
                UserId = i.UserId
            });
        }

        public async Task<Guid> SaveInvoiceAsync(PostInvoiceDto dto, Guid userId)
        {
            var invoice = new PostInvoiceDto
            {
                Id = Guid.NewGuid(),
                Vendor = dto.Vendor,
                Amount = dto.Amount,
                Date = dto.Date,
                Category = dto.Category,
                UserId = userId
            };

            return await _invoiceRepository.AddInvoiceAsync(invoice);
        }
    }
}
