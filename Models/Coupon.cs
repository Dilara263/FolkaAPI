namespace FolkaAPI.Models
{
    public class Coupon
    {
        public required string Id { get; set; }
        public required string Description { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = "Percentage";
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddMonths(1);
        public int MinOrderAmount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
