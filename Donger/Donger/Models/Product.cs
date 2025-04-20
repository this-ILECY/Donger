namespace Donger.Models
{
    public class Product
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool Consumed { get; set; }
        public bool IsLongTerm { get; set; } // To distinguish between long-term and short-term products

        // Foreign Key for the parent SubInvoice
        public int SubInvoiceId { get; set; }
        // Navigation property for the parent SubInvoice
        public SubInvoice SubInvoice { get; set; }
    }
}