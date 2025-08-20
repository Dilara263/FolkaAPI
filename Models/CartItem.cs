namespace FolkaAPI.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string ProductId { get; set; }
        public int Quantity { get; set; }

        public string? ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string? ProductImage { get; set; }
    }
}