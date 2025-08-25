using System;
namespace FolkaAPI.Models
{
    public class AppliedCoupon
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string CouponId { get; set; }
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = default!;
        public Coupon Coupon { get; set; } = default!;
    }
}
