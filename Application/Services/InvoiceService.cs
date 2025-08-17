using Application.DTOs;
using Application.DTOs.Invoices;
using Application.Interface;
using Application.Interface.Invoice;
using AutoMapper;
using Domain.Entities.Invoice;
using Domain.Enums;


namespace Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;


        public InvoiceService(IInvoiceRepository invoiceRepository, IFileStorageService fileStorageService, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetInvoiceDtoRes>> GetInvoiceAsync(GetInvoiceDtoReq input)
        {
            var allInvoices = new List<Invoices>();

            foreach (var userId in input.UserIds)
            {
                var invoices = await _invoiceRepository.GetInvoicesAsync(userId);
                allInvoices.AddRange(invoices);
            }

            return _mapper.Map<IEnumerable<GetInvoiceDtoRes>>(allInvoices);
        }
        public async Task<Guid> SaveInvoiceAsync(PostInvoiceDto dto)
        {
            var fileUrl = await _fileStorageService.UploadFileOnBlob(dto.BillFile);

            var category = CategorizeInvoice(dto);

            var invoice = _mapper.Map<Invoices>(dto);

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
