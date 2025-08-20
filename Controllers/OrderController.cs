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

namespace FolkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
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

            decimal totalPrice = 0;
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
                    totalPrice += productPrice * item.Quantity;
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

            return new CartResponseDto
            {
                CartItems = enrichedCartItems,
                TotalPrice = totalPrice
            };
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userCartItems = await _context.CartItems
                                            .Where(ci => ci.UserId == userId)
                                            .ToListAsync();

            if (!userCartItems.Any())
            {
                return BadRequest(new { message = "Sepetiniz boş, sipariş oluşturulamaz." });
            }

            var orderItems = new List<OrderItem>();
            decimal orderTotalPrice = 0;

            foreach (var cartItem in userCartItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);

                if (product == null)
                {
                    return BadRequest(new { message = $"Ürün bulunamadı: {cartItem.ProductId}" });
                }

                if (cartItem.Quantity > product.Stock)
                {
                    return BadRequest(new { message = $"'{product.Name}' ürünü için yeterli stok yok. Mevcut stok: {product.Stock}" });
                }

                decimal unitPrice = 0;
                if (!string.IsNullOrEmpty(product.Price))
                {
                    string priceNumericString = new string(product.Price.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
                    decimal.TryParse(priceNumericString.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out unitPrice);
                }

                orderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = unitPrice,
                    ProductName = product.Name,
                    ProductImage = product.Image
                });
                orderTotalPrice += unitPrice * cartItem.Quantity;

                // Stoktan düşme (gerçek uygulamada transaction içinde olmalı)
                product.Stock -= cartItem.Quantity;
                _context.Products.Update(product);
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalPrice = orderTotalPrice,
                Status = "Pending",
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(userCartItems);

            await _context.SaveChangesAsync();

            return Ok(new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ProductName = oi.ProductName,
                    ProductImage = oi.ProductImage
                }).ToList()
            });
        }

        [HttpGet("MyOrders")]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetMyOrders()
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var orders = await _context.Orders
                                    .Where(o => o.UserId == userId)
                                    .Include(o => o.OrderItems)
                                    .OrderByDescending(o => o.OrderDate)
                                    .ToListAsync();

            if (!orders.Any())
            {
                return NotFound(new { message = "Henüz bir siparişiniz bulunmamaktadır." });
            }

            var orderDtos = orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ProductName = oi.ProductName,
                    ProductImage = oi.ProductImage
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrderDetails(int orderId)
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var order = await _context.Orders
                                    .Include(o => o.OrderItems)
                                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound(new { message = "Sipariş bulunamadı veya bu siparişi görüntüleme yetkiniz yok." });
            }

            var orderDto = new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ProductName = oi.ProductName,
                    ProductImage = oi.ProductImage
                }).ToList()
            };

            return Ok(orderDto);
        }
    }
}