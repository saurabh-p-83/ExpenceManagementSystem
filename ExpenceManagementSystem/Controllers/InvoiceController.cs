using Application.Interface;
using Domain.Entities.Invoice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenceManagementSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }
        [Authorize]
        [HttpPost("GetAllInvoices")]
        public async Task<IActionResult> GetAllInvoices([FromBody] GetInvoiceDtoReq dto)
        {
            var invoices = await _invoiceService.GetInvoiceAsync(dto);
            return Ok(invoices);
        }

        [HttpPost]
        public async Task<IActionResult> PostInvoice([FromForm] PostInvoiceDto dto)
        {
            var id = await _invoiceService.SaveInvoiceAsync(dto);
            return Ok(new { id });
        }
    }
}
