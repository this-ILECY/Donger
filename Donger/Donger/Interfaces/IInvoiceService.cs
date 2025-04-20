using Donger.Models;
using Donger.Models.ViewModels; // Make sure to include your ViewModels namespace
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Donger.Services
{
    public interface IInvoiceService
    {
        // Method to get detailed information for a single invoice
        Task<InvoiceDetailsViewModel> GetInvoiceDetailsAsync(int id);
        Task CreateInvoiceAsync(CreateInvoiceViewModel model);
        Task UpdateInvoiceAsync(EditInvoiceInputModel model);

        // We'll add other Invoice CRUD methods later (Add, Update, Delete, GetAll)
        // Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        // Task AddInvoiceAsync(Invoice invoice);
        // Task UpdateInvoiceAsync(Invoice invoice);
        // Task DeleteInvoiceAsync(int id);
        // Task<bool> InvoiceExistsAsync(int id);
    }
}