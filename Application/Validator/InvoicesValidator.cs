using Application.DTOs.Invoices;
using FluentValidation;

namespace Application.Validators.Invoices
{
    public class PostInvoiceDtoValidator : AbstractValidator<PostInvoiceDto>
    {
        public PostInvoiceDtoValidator()
        {
            RuleFor(x => x.Vendor)
                .NotEmpty().WithMessage("Vendor is required")
                .MaximumLength(100);

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0");

            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date cannot be in the future");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");
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
