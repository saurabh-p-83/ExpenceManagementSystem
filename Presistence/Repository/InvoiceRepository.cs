using Application.Interface;
using Domain.Entities.Invoice;
using Microsoft.EntityFrameworkCore;
using Presistence.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;
        public InvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync(Guid userId)
        {
            return await _context.Invoices
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<Invoice> GetByIdAsync(Guid id)
        {
            return await _context.Invoices.FindAsync(id);
        }

        public async Task<Guid> AddInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice.Id;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
