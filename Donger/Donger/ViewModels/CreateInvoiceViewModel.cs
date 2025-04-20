using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Donger.Models.ViewModels
{
    public class CreateInvoiceViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please select at least one debtor.")]
        public List<int> SelectedDebtorIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "Please add at least one sub-invoice.")]
        public List<CreateSubInvoiceViewModel> SubInvoices { get; set; } = new List<CreateSubInvoiceViewModel>();

        // Properties to hold data for dropdowns/selection lists in the UI (not for input)
        public List<PersonViewModel> AvailablePersons { get; set; }
        public List<ProductViewModel> AvailableProducts { get; set; }
    }

    public class CreateSubInvoiceViewModel
    {
        [Required(ErrorMessage = "Please select a creditor for this sub-invoice.")]
        public int CreditorId { get; set; }

        [Required(ErrorMessage = "Please select at least one product for this sub-invoice.")]
        public List<int> SelectedProductIds { get; set; } = new List<int>();

        // We won't ask the user for the total price here, we will calculate it in the service
        // public decimal TotalPrice { get; set; }
    }

    // Re-using PersonViewModel and ProductViewModel from the previous step
    // public class PersonViewModel { public int Id { get; set; } public string Name { get; set; } }
    // public class ProductViewModel { public int Id { get; set; } public string Name { get; set; } public decimal Price { get; set; } }
}