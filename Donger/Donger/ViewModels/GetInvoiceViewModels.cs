using System;
using System.Collections.Generic;

namespace Donger.Models.ViewModels
{
    public class InvoiceDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }

        public List<PersonViewModel> Debtors { get; set; }
        public List<SubInvoiceViewModel> SubInvoices { get; set; }
    }

    public class PersonViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SubInvoiceViewModel
    {
        public int Id { get; set; }
        public PersonViewModel Creditor { get; set; } // Creditor (name and id)
        public decimal TotalPrice { get; set; } // Sub invoice total price
        public List<ProductViewModel> Products { get; set; } // Products in this sub invoice
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        // We might not need IsLongTerm for just display, but could add if needed later
    }
}