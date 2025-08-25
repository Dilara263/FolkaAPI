namespace FolkaAPI.Models
{
    public class UserCoupon
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string CouponId { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime AcquiredDate { get; set; } = DateTime.UtcNow;
        public User? User { get; set; }
        public Coupon? Coupon { get; set; }
    }
}
