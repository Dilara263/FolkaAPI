using System;
using System.Collections.Generic;

namespace FolkaAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; } = "Pending";
        public string? AppliedCouponCode { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}