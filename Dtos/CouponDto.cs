using System;

namespace FolkaAPI.Dtos
{
    public class CouponDto
    {
        public required string Id { get; set; }
        public required string Description { get; set; }
        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = "Percentage";
        public DateTime ExpiryDate { get; set; }
        public int MinOrderAmount { get; set; }
    }
}