using Application.DTOs;
using Application.DTOs.Invoices;
using Application.Interface.Azure;
using Application.Interface.Invoice;
using Application.Interface.NewFolder;
using AutoMapper;
using Domain.Entities.Invoice;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IOcrInvoiceService _ocrInvoiceService;
        private readonly IMapper _mapper;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IFileStorageService fileStorageService,
            IMapper mapper,
            IOcrInvoiceService ocrInvoiceService)
        {
            _invoiceRepository = invoiceRepository;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
            _ocrInvoiceService = ocrInvoiceService;
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

        public async Task<Guid> SaveInvoiceAsync(PostInvoiceDto dto, Guid userId)
        {
            ValidateFile(dto?.BillFile);

            // Extract structured invoice data using OCR
            var invoiceFromOcr = await ExtractInvoiceData(dto.BillFile);

            // Save uploaded file to blob
            var fileUrl = await _fileStorageService.UploadFileOnBlob(dto.BillFile);
            invoiceFromOcr.FileUrl = fileUrl;
            invoiceFromOcr.UserId = userId;

            // Persist the invoice
            await _invoiceRepository.AddInvoiceAsync(invoiceFromOcr);

            return invoiceFromOcr.Id;
        }

        private async Task<Invoices> ExtractInvoiceData(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var invoice = await _ocrInvoiceService.ExtractInvoiceDataAsync(stream, file.FileName);

            if (invoice == null)
            {
                throw new InvalidOperationException("OCR failed to extract invoice data from the uploaded file.");
            }

            return invoice;
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided", nameof(file));
        }
    }
}