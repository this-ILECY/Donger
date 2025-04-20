using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Donger.Models.ViewModels
{
    public class EditInvoiceInputModel
    {
        [Required]
        public int Id { get; set; } // The ID of the Invoice being edited

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please select at least one debtor.")]
        public List<int> SelectedDebtorIds { get; set; } = new List<int>();

        [Required(ErrorMessage = "Please add at least one sub-invoice.")]
        public List<EditSubInvoiceInputModel> SubInvoices { get; set; } = new List<EditSubInvoiceInputModel>();

        // Properties to hold data for dropdowns/selection lists in the UI (not for input)
        public List<PersonViewModel> AvailablePersons { get; set; }
        public List<ProductViewModel> AvailableProducts { get; set; }
    }

    public class EditSubInvoiceInputModel
    {
        // We might need an ID here if we want to update existing sub-invoices
        // and identify which ones are new, edited, or deleted.
        // For simplicity initially, let's assume we might replace sub-invoices
        // or handle updates carefully. Let's add an optional ID.
        public int? Id { get; set; } // Null for new sub-invoices added during edit

        [Required(ErrorMessage = "Please select a creditor for this sub-invoice.")]
        public int CreditorId { get; set; }

        [Required(ErrorMessage = "Please select at least one product for this sub-invoice.")]
        public List<int> SelectedProductIds { get; set; } = new List<int>();

        // TotalPrice will be calculated in the service
    }

    // Re-using PersonViewModel and ProductViewModel
}