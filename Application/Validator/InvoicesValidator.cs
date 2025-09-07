using Application.DTOs.Invoices;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Validators.Invoices
{
    public class PostInvoiceDtoValidator : AbstractValidator<PostInvoiceDto>
    {
        private readonly string[] _allowedContentTypes = new[] { "application/pdf", "image/png", "image/jpeg" };

        public PostInvoiceDtoValidator()
        {
            RuleFor(x => x.BillFile)
                .NotNull().WithMessage("Receipt file is required.")
                .Must(f => f != null && f.Length > 0).WithMessage("File is empty.")
                .Must(f => f != null && f.Length <= 10 * 1024 * 1024).WithMessage("Maximum file size is 10 MB.")
                .Must(f => f != null && _allowedContentTypes.Contains(f.ContentType))
                .WithMessage("Only PDF, PNG and JPEG files are supported.");
        }
    }
    public class GetInvoiceDtoReqValidator : AbstractValidator<GetInvoiceDtoReq>
    {
        public GetInvoiceDtoReqValidator()
        {
            RuleFor(x => x.UserIds)
                .NotEmpty().WithMessage("At least one UserId is required");
        }
    }
}
