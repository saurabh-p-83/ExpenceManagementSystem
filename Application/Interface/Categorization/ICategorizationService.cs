using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.NewFolder
{
    public interface ICategorizationService
    {
        Task<InvoiceCategory> CategorizeWithOpenAPIAsync(string vendor, string description);
    }
}
