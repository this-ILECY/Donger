using System.Collections.Generic;

namespace Donger.Models
{
    public class SubInvoice
    {
        public int Id { get; set; } // Primary Key

        // Foreign Key for the creditor (Person)
        public int CreditorId { get; set; }
        // Navigation property for the creditor
        public Person Creditor { get; set; }

        // Foreign Key for the parent Invoice
        public int InvoiceId { get; set; }
        // Navigation property for the parent Invoice
        public Invoice Invoice { get; set; }

        // Navigation property for the products in this sub-invoice (one-to-many)
        public ICollection<Product> Products { get; set; }

        // Total price for this sub-invoice (can be calculated or stored)
        public decimal TotalPrice { get; set; }
    }
}