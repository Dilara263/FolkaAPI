namespace FolkaAPI.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public required string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public Order? Order { get; set; }
    }
}