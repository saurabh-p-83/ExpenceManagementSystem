using Application.Interface.Invoice;
using Domain.Entities.Invoice;
using Microsoft.EntityFrameworkCore;
using Presistence.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<Invoices>> GetInvoicesAsync(Guid userId)
        {
            return await _context.Invoices
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<Invoices?> GetByIdAsync(Guid id)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddInvoiceAsync(Invoices invoice)
        {
            await _context.Invoices.AddAsync(invoice);
        }

        public void UpdateInvoice(Invoices invoice)
        {
            _context.Invoices.Update(invoice);
        }

        public void DeleteInvoice(Invoices invoice)
        {
            _context.Invoices.Remove(invoice);
        }
    }
}
