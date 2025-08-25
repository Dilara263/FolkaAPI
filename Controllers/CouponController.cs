using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FolkaAPI.Data;
using FolkaAPI.Models;
using FolkaAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using System;

namespace FolkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CouponController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CouponController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet("MyCoupons")]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetMyCoupons()
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userCoupons = await _context.UserCoupons
                                            .Where(uc => uc.UserId == userId && !uc.IsUsed)
                                            .Include(uc => uc.Coupon)
                                            .ToListAsync();

            var activeCoupons = userCoupons
                                .Where(uc => uc.Coupon != null && uc.Coupon.IsActive && uc.Coupon.ExpiryDate > DateTime.UtcNow)
                                .Select(uc => new CouponDto
                                {
                                    Id = uc.Coupon!.Id,
                                    Description = uc.Coupon.Description,
                                    DiscountValue = uc.Coupon.DiscountValue,
                                    DiscountType = uc.Coupon.DiscountType,
                                    ExpiryDate = uc.Coupon.ExpiryDate,
                                    MinOrderAmount = uc.Coupon.MinOrderAmount
                                })
                                .ToList();

            if (!activeCoupons.Any())
            {
                return NotFound(new { message = "Henüz aktif kuponunuz bulunmamaktadır." });
            }

            return Ok(activeCoupons);
        }

        [HttpGet("{couponId}")]
        public async Task<ActionResult<CouponDto>> GetCouponDetails(string couponId)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == couponId);

            if (coupon == null || !coupon.IsActive || coupon.ExpiryDate <= DateTime.UtcNow)
            {
                return NotFound(new { message = "Kupon bulunamadı veya geçersiz." });
            }

            return Ok(new CouponDto
            {
                Id = coupon.Id,
                Description = coupon.Description,
                DiscountValue = coupon.DiscountValue,
                DiscountType = coupon.DiscountType,
                ExpiryDate = coupon.ExpiryDate,
                MinOrderAmount = coupon.MinOrderAmount
            });
        }
    }
}
