using Application.DTOs.Invoices;
using Application.Interface.Invoice;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseManagementSystemAPI.Controllers
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var id = await _invoiceService.SaveInvoiceAsync(dto, userId);
            return Ok(new { id });
        }
    }
}
