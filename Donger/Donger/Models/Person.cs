using System.Collections.Generic;

namespace Donger.Models
{
    public class Person
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }

        // Navigation property for Invoices where this person is a debtor (many-to-many)
        public ICollection<Invoice> InvoicesAttended { get; set; }

        // Navigation property for SubInvoices where this person is a creditor (one-to-many)
        public ICollection<SubInvoice> SubInvoicesCreated { get; set; }
    }
}