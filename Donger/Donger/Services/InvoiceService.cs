using Donger.Data;
using Donger.Models;
using Donger.Models.ViewModels; // Make sure to include your ViewModels namespace
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Donger.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDetailsViewModel> GetInvoiceDetailsAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Debtors) // Include the Debtors (Persons)
                .Include(i => i.SubInvoices) // Include the SubInvoices
                    .ThenInclude(sub => sub.Creditor) // Then include the Creditor for each SubInvoice
                .Include(i => i.SubInvoices) // Include SubInvoices again to ThenInclude Products (EF Core handles this efficiently)
                     .ThenInclude(sub => sub.Products) // Then include the Products for each SubInvoice
                .Where(i => i.Id == id) // Filter for the specific invoice ID
                .Select(i => new InvoiceDetailsViewModel // Project the data into the ViewModel
                {
                    Id = i.Id,
                    Date = i.Date,
                    TotalPrice = i.TotalPrice, // Assuming TotalPrice is calculated and stored

                    Debtors = i.Debtors.Select(d => new PersonViewModel
                    {
                        Id = d.Id,
                        Name = d.Name
                    }).ToList(),

                    SubInvoices = i.SubInvoices.Select(sub => new SubInvoiceViewModel
                    {
                        Id = sub.Id,
                        TotalPrice = sub.TotalPrice, // Assuming TotalPrice is calculated and stored

                        Creditor = new PersonViewModel
                        {
                            Id = sub.Creditor.Id,
                            Name = sub.Creditor.Name
                        },

                        Products = sub.Products.Select(p => new ProductViewModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Price = p.Price
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync(); // Get the single invoice or null

            return invoice;
        }
        
        public async Task CreateInvoiceAsync(CreateInvoiceViewModel model)
        {
            // Fetch related entities from the database
            var debtors = await _context.People
                .Where(p => model.SelectedDebtorIds.Contains(p.Id))
                .ToListAsync();

            var newInvoice = new Invoice
            {
                Date = model.Date,
                Debtors = debtors, // Link debtors to the invoice
                SubInvoices = new List<SubInvoice>(),
                TotalPrice = 0 // Will calculate this after adding sub-invoices
            };

            decimal totalInvoicePrice = 0;

            // Create SubInvoices and Products
            foreach (var subInvoiceModel in model.SubInvoices)
            {
                var creditor = await _context.People.FindAsync(subInvoiceModel.CreditorId);
                if (creditor == null)
                {
                    // Handle case where creditor is not found (should ideally be validated before this)
                    throw new KeyNotFoundException($"Creditor with id {subInvoiceModel.CreditorId} not found.");
                }

                var products = await _context.Products
                    .Where(p => subInvoiceModel.SelectedProductIds.Contains(p.Id))
                    .ToListAsync();

                decimal totalSubInvoicePrice = products.Sum(p => p.Price);

                var newSubInvoice = new SubInvoice
                {
                    CreditorId = subInvoiceModel.CreditorId,
                    Creditor = creditor, // Link the creditor
                    Products = products, // Link the products
                    TotalPrice = totalSubInvoicePrice
                };

                newInvoice.SubInvoices.Add(newSubInvoice);
                totalInvoicePrice += totalSubInvoicePrice;
            }

            newInvoice.TotalPrice = totalInvoicePrice;

            _context.Invoices.Add(newInvoice);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateInvoiceAsync(EditInvoiceInputModel model)
        {
            // 1. Fetch the existing Invoice with all related entities eagerly loaded
            var existingInvoice = await _context.Invoices
                .Include(i => i.Debtors)
                .Include(i => i.SubInvoices)
                    .ThenInclude(sub => sub.Products)
                .FirstOrDefaultAsync(i => i.Id == model.Id);

            if (existingInvoice == null)
            {
                throw new KeyNotFoundException($"Invoice with id {model.Id} not found.");
            }

            // 2. Update scalar properties of the Invoice
            existingInvoice.Date = model.Date;

            // 3. Update Debtors (Many-to-Many) - Replace the collection
            var selectedDebtors = await _context.People
                .Where(p => model.SelectedDebtorIds.Contains(p.Id))
                .ToListAsync();
            existingInvoice.Debtors = selectedDebtors; // EF Core handles adds/removes in the join table


            // 4. Update SubInvoices and their Products (One-to-Many relationships)
            var existingSubInvoiceIds = existingInvoice.SubInvoices.Select(s => s.Id).ToList();
            var inputSubInvoiceIds = model.SubInvoices.Where(s => s.Id.HasValue).Select(s => s.Id.Value).ToList();

            // Identify SubInvoices to Remove
            var subInvoicesToRemove = existingInvoice.SubInvoices
                .Where(s => !inputSubInvoiceIds.Contains(s.Id))
                .ToList();
            _context.SubInvoices.RemoveRange(subInvoicesToRemove);

            // Process Input SubInvoices (Add or Update)
            decimal totalInvoicePrice = 0;
            var updatedSubInvoices = new List<SubInvoice>();

            foreach (var subInvoiceInput in model.SubInvoices)
            {
                SubInvoice subInvoice;

                if (subInvoiceInput.Id.HasValue && subInvoiceInput.Id.Value > 0)
                {
                    // Attempt to find the existing SubInvoice
                    subInvoice = existingInvoice.SubInvoices.FirstOrDefault(s => s.Id == subInvoiceInput.Id.Value);

                    if (subInvoice != null)
                    {
                        // Update existing SubInvoice properties
                        subInvoice.CreditorId = subInvoiceInput.CreditorId;

                        // Update Products within this SubInvoice
                        var existingProductIds = subInvoice.Products.Select(p => p.Id).ToList();
                        var inputProductIds = subInvoiceInput.SelectedProductIds.ToList();

                        // Identify Products to Remove from this SubInvoice
                        var productsToRemove = subInvoice.Products
                            .Where(p => !inputProductIds.Contains(p.Id))
                            .ToList();
                        foreach (var productToRemove in productsToRemove)
                        {
                             subInvoice.Products.Remove(productToRemove);
                            // If Product entity itself needed deletion (unlikely if linking existing), would mark entity state
                            // _context.Entry(productToRemove).State = EntityState.Deleted;
                        }


                        // Identify Products to Add to this SubInvoice
                        var productIdsToAdd = inputProductIds.Except(existingProductIds).ToList();
                        if (productIdsToAdd.Any())
                        {
                            var productsToAdd = await _context.Products
                                .Where(p => productIdsToAdd.Contains(p.Id))
                                .ToListAsync();
                             foreach(var productToAdd in productsToAdd)
                             {
                                  subInvoice.Products.Add(productToAdd);
                             }

                        }

                        // Recalculate TotalPrice for the updated SubInvoice
                        subInvoice.TotalPrice = subInvoice.Products.Sum(p => p.Price);
                         _context.Entry(subInvoice).State = EntityState.Modified; // Mark sub-invoice as modified
                    }
                    else
                    {
                        // Handle case where input ID was provided but no matching SubInvoice found
                        // This might indicate a data inconsistency or an attempt to edit a deleted sub-invoice.
                        // Depending on requirements, you might skip, log, or throw an exception.
                        // For now, let's throw as it indicates an unexpected state.
                         throw new KeyNotFoundException($"SubInvoice with id {subInvoiceInput.Id.Value} not found for Invoice id {model.Id}.");
                    }
                }
                else
                {
                    // This is a New SubInvoice
                     var creditor = await _context.People.FindAsync(subInvoiceInput.CreditorId);
                    if (creditor == null)
                    {
                         throw new KeyNotFoundException($"Creditor with id {subInvoiceInput.CreditorId} not found for new sub-invoice.");
                    }

                    var products = await _context.Products
                        .Where(p => subInvoiceInput.SelectedProductIds.Contains(p.Id))
                        .ToListAsync();

                    decimal totalSubInvoicePrice = products.Sum(p => p.Price);

                    subInvoice = new SubInvoice
                    {
                        CreditorId = subInvoiceInput.CreditorId,
                        Creditor = creditor, // EF Core will handle setting this relationship
                        Products = products, // EF Core will handle linking these products
                        TotalPrice = totalSubInvoicePrice
                    };

                    existingInvoice.SubInvoices.Add(subInvoice); // Add the new sub-invoice to the collection
                     _context.Entry(subInvoice).State = EntityState.Added; // Mark as added
                }
                 updatedSubInvoices.Add(subInvoice);
                 totalInvoicePrice += subInvoice.TotalPrice;
            }

            // Update the Invoice's SubInvoices collection reference if necessary (EF Core tracks changes in the collection directly)
            // existingInvoice.SubInvoices = updatedSubInvoices; // This might not be strictly necessary if EF Core tracks additions/removals on the original collection

            existingInvoice.TotalPrice = totalInvoicePrice;


            // 5. Save Changes
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency conflicts if necessary
                 if (!await InvoiceExistsAsync(model.Id))
                {
                    throw new KeyNotFoundException($"Invoice with id {model.Id} not found during save.");
                }
                else
                {
                    throw; // Re-throw the exception if it's a different concurrency issue
                }
            }
        }

        // Helper method to check if Invoice exists (useful for concurrency handling)
        private async Task<bool> InvoiceExistsAsync(int id)
        {
            return await _context.Invoices.AnyAsync(e => e.Id == id);
        }

        // Implement other IInvoiceService methods here later (Add, Update, Delete, GetAll)
    }
}