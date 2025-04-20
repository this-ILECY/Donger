using System;
using System.Collections.Generic;

namespace Donger.Models
{
    public class Invoice
    {
        public int Id { get; set; } // Primary Key
        public DateTime Date { get; set; } // Date of the meal

        // Navigation property for the sub-invoices that make up this invoice (one-to-many)
        public ICollection<SubInvoice> SubInvoices { get; set; }

        // Navigation property for the debtors who participated in this meal (many-to-many)
        public ICollection<Person> Debtors { get; set; }

        // Total price for this invoice (sum of all sub-invoice total prices)
        public decimal TotalPrice { get; set; }
    }
}