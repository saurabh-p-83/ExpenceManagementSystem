using Application.Interface;
using Domain.Entities.Invoice;
using Domain.Enums;


namespace Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IFileStorageService _fileStorageService;

        public InvoiceService(IInvoiceRepository invoiceRepository, IFileStorageService fileStorageService)
        {
            _invoiceRepository = invoiceRepository;
            _fileStorageService = fileStorageService;
        }

        public async Task<IEnumerable<GetInvoiceDtoRes>> GetInvoiceAsync(GetInvoiceDtoReq input)
        {
            var userIds = input.userId
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => Guid.Parse(id.Trim()))
                .ToList();

            var allInvoices = new List<Invoice>();

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
        public async Task<Guid> SaveInvoiceAsync(PostInvoiceDto dto)
        {
            var fileUrl = await _fileStorageService.UploadFileOnBlob(dto.BillFile);

            var category = CategorizeInvoice(dto);

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                Vendor = dto.Vendor,
                Amount = dto.Amount,
                Date = dto.Date,
                Category = category,
                Description = dto.Description,
                UserId = dto.UserId,  
                FileUrl = fileUrl
            };

            await _invoiceRepository.AddInvoiceAsync(invoice);

            return invoice.Id;
        }
        private InvoiceCategory CategorizeInvoice(PostInvoiceDto dto)
        {
            //Basic AI logic placeholder — replace with ML later
            if (dto.Vendor.Contains("Amazon", StringComparison.OrdinalIgnoreCase))
                return InvoiceCategory.Shopping;

            if (dto.Vendor.Contains("Uber", StringComparison.OrdinalIgnoreCase))
                return InvoiceCategory.Travel;

            return InvoiceCategory.Other;
        }
    }
}
