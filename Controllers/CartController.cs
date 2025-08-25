using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FolkaAPI.Data;
using FolkaAPI.Models;
using FolkaAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System;

namespace FolkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<CartResponseDto> GetEnrichedCartResponse(string userId)
        {
            var userCartItems = await _context.CartItems
                                            .Where(ci => ci.UserId == userId)
                                            .ToListAsync();

            decimal subtotalPrice = 0;
            var enrichedCartItems = new List<CartItem>();

            foreach (var item in userCartItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                if (product != null)
                {
                    decimal productPrice = 0;
                    if (!string.IsNullOrEmpty(product.Price))
                    {
                        string priceNumericString = new string(product.Price.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
                        decimal.TryParse(priceNumericString.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out productPrice);
                    }

                    enrichedCartItems.Add(new CartItem
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductName = product.Name,
                        ProductPrice = productPrice,
                        ProductImage = product.Image
                    });
                    subtotalPrice += productPrice * item.Quantity;
                }
                else
                {
                    enrichedCartItems.Add(new CartItem
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductName = "Bilinmeyen Ürün",
                        ProductPrice = 0,
                        ProductImage = null
                    });
                }
            }

            decimal discountAmount = 0;
            string? appliedCouponCode = null;

            var appliedCouponEntry = await _context.AppliedCoupons.FirstOrDefaultAsync(ac => ac.UserId == userId);

            if (appliedCouponEntry != null)
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == appliedCouponEntry.CouponId);
                var userOwnsCoupon = await _context.UserCoupons.AnyAsync(uc => uc.UserId == userId && uc.CouponId == appliedCouponEntry.CouponId && !uc.IsUsed);

                if (coupon != null && coupon.IsActive && coupon.ExpiryDate > DateTime.UtcNow && userOwnsCoupon)
                {
                    if (subtotalPrice >= coupon.MinOrderAmount)
                    {
                        if (coupon.DiscountType == "Percentage")
                        {
                            discountAmount = subtotalPrice * coupon.DiscountValue;
                        }
                        else if (coupon.DiscountType == "Amount")
                        {
                            discountAmount = coupon.DiscountValue;
                        }
                        appliedCouponCode = coupon.Id;
                    }
                    else
                    {
                        _context.AppliedCoupons.Remove(appliedCouponEntry);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    _context.AppliedCoupons.Remove(appliedCouponEntry);
                    await _context.SaveChangesAsync();
                }
            }

            return new CartResponseDto
            {
                CartItems = enrichedCartItems,
                TotalPrice = subtotalPrice - discountAmount,
                DiscountAmount = discountAmount,
                AppliedCouponCode = appliedCouponCode
            };
        }

        [HttpGet]
        public async Task<ActionResult<CartResponseDto>> GetCart()
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            return Ok(await GetEnrichedCartResponse(userId));
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto request)
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (request == null || string.IsNullOrEmpty(request.ProductId) || request.Quantity <= 0)
            {
                return BadRequest(new { message = "Geçersiz sepet öğesi." });
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId);
            if (product == null)
            {
                return NotFound(new { message = "Ürün bulunamadı." });
            }

            var existingCartItem = await _context.CartItems
                                                    .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == request.ProductId);

            int quantityToAdd = request.Quantity;

            if (existingCartItem != null)
            {
                if (existingCartItem.Quantity + quantityToAdd > product.Stock)
                {
                    return BadRequest(new { message = $"Ürün stokta yeterli değil. Mevcut stok: {product.Stock - existingCartItem.Quantity}" });
                }
                existingCartItem.Quantity += quantityToAdd;
                _context.CartItems.Update(existingCartItem);
            }
            else
            {
                if (quantityToAdd > product.Stock)
                {
                    return BadRequest(new { message = $"Ürün stokta yeterli değil. Mevcut stok: {product.Stock}" });
                }
                _context.CartItems.Add(new CartItem { UserId = userId, ProductId = request.ProductId, Quantity = quantityToAdd });
            }

            await _context.SaveChangesAsync();
            return Ok(await GetEnrichedCartResponse(userId));
        }

        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateCartItemQuantity([FromBody] CartItemUpdateDto updateDto)
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (updateDto == null || string.IsNullOrEmpty(updateDto.ProductId))
            {
                return BadRequest(new { message = "Geçersiz güncelleme isteği." });
            }

            var existingCartItem = await _context.CartItems
                                                    .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == updateDto.ProductId);

            if (existingCartItem == null)
            {
                return NotFound(new { message = "Ürün sepette bulunamadı." });
            }

            if (updateDto.Quantity < 1)
            {
                _context.CartItems.Remove(existingCartItem);
            }
            else
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == updateDto.ProductId);
                if (product == null)
                {
                    return NotFound(new { message = "Ürün bilgisi bulunamadı." });
                }
                if (updateDto.Quantity > product.Stock)
                {
                    return BadRequest(new { message = $"Ürün stokta yeterli değil. Mevcut stok: {product.Stock}" });
                }
                existingCartItem.Quantity = updateDto.Quantity;
                _context.CartItems.Update(existingCartItem);
            }

            await _context.SaveChangesAsync();
            return Ok(await GetEnrichedCartResponse(userId));
        }

        [HttpPost("apply-coupon")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto request)
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (request == null || string.IsNullOrEmpty(request.CouponCode))
            {
                return BadRequest(new { message = "Geçersiz kupon kodu." });
            }

            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Id == request.CouponCode);
            if (coupon == null || !coupon.IsActive || coupon.ExpiryDate <= DateTime.UtcNow)
            {
                var existingAppliedCoupon = await _context.AppliedCoupons.FirstOrDefaultAsync(ac => ac.UserId == userId);
                if (existingAppliedCoupon != null)
                {
                    _context.AppliedCoupons.Remove(existingAppliedCoupon);
                    await _context.SaveChangesAsync();
                }
                return BadRequest(new { message = "Geçersiz veya süresi dolmuş kupon kodu." });
            }

            var userOwnsCoupon = await _context.UserCoupons
                                                .AnyAsync(uc => uc.UserId == userId && uc.CouponId == request.CouponCode && !uc.IsUsed);

            if (!userOwnsCoupon)
            {
                var existingAppliedCoupon = await _context.AppliedCoupons.FirstOrDefaultAsync(ac => ac.UserId == userId);
                if (existingAppliedCoupon != null)
                {
                    _context.AppliedCoupons.Remove(existingAppliedCoupon);
                    await _context.SaveChangesAsync();
                }
                return BadRequest(new { message = "Bu kupona sahip değilsiniz veya zaten kullanılmış." });
            }

            var appliedCouponEntry = await _context.AppliedCoupons.FirstOrDefaultAsync(ac => ac.UserId == userId);

            if (appliedCouponEntry == null)
            {
                _context.AppliedCoupons.Add(new AppliedCoupon { UserId = userId, CouponId = request.CouponCode, AppliedDate = DateTime.UtcNow });
            }
            else
            {
                appliedCouponEntry.CouponId = request.CouponCode;
                appliedCouponEntry.AppliedDate = DateTime.UtcNow;
                _context.AppliedCoupons.Update(appliedCouponEntry);
            }
            await _context.SaveChangesAsync();

            return Ok(await GetEnrichedCartResponse(userId));

        }
    }
}
