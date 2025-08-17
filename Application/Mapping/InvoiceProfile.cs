using Application.DTOs.Invoices;
using AutoMapper;
using Domain.Entities.Invoice;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Mapping
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            // Entity -> DTO
            CreateMap<Invoices, GetInvoiceDtoRes>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()));

            // DTO -> Entity
            CreateMap<PostInvoiceDto, Invoices>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.FileUrl, opt => opt.Ignore());
        }
    }
}
