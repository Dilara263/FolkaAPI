using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FolkaAPI.Models;
using FolkaAPI.Dtos;
using System.Globalization;

namespace FolkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private static Dictionary<string, List<CartItem>> _userCarts = new Dictionary<string, List<CartItem>>();
        private static readonly List<Product> _products = ProductsController.GetStaticProducts();

        private string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private CartResponseDto GetEnrichedCartResponse(string userId)
        {
            if (!_userCarts.TryGetValue(userId, out List<CartItem>? userCart))
            {
                userCart = new List<CartItem>();
                _userCarts.Add(userId, userCart);
            }

            decimal totalPrice = 0;
            var enrichedCartItems = new List<CartItem>();

            foreach (var item in userCart!)
            {
                var product = _products.FirstOrDefault(p => p.Id == item.ProductId);
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

        [HttpGet]
        public ActionResult<CartResponseDto> GetCart()
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            return Ok(GetEnrichedCartResponse(userId));
        }

        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] AddToCartDto request)
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

            var product = _products.FirstOrDefault(p => p.Id == request.ProductId);
            if (product == null)
            {
                return NotFound(new { message = "Ürün bulunamadı." });
            }

            if (!_userCarts.TryGetValue(userId, out List<CartItem>? userCart))
            {
                userCart = new List<CartItem>();
                _userCarts.Add(userId, userCart);
            }

            var existingItem = userCart!.FirstOrDefault(item => item.ProductId == request.ProductId);

            int quantityToAdd = request.Quantity;

            if (existingItem != null)
            {
                if (existingItem.Quantity + quantityToAdd > product.Stock)
                {
                    return BadRequest(new { message = $"Ürün stokta yeterli değil. Mevcut stok: {product.Stock - existingItem.Quantity}" });
                }
                existingItem.Quantity += quantityToAdd;
            }
            else
            {
                if (quantityToAdd > product.Stock)
                {
                    return BadRequest(new { message = $"Ürün stokta yeterli değil. Mevcut stok: {product.Stock}" });
                }
                userCart.Add(new CartItem { ProductId = request.ProductId, Quantity = quantityToAdd });
            }

            return Ok(GetEnrichedCartResponse(userId));
        }

        [HttpPut("update-quantity")]
        public IActionResult UpdateCartItemQuantity([FromBody] CartItemUpdateDto updateDto)
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

            if (!_userCarts.TryGetValue(userId, out List<CartItem>? userCart))
            {
                return NotFound(new { message = "Sepet bulunamadı." });
            }

            var existingItem = userCart!.FirstOrDefault(item => item.ProductId == updateDto.ProductId);

            if (existingItem == null)
            {
                return NotFound(new { message = "Ürün sepette bulunamadı." });
            }

            if (updateDto.Quantity < 1)
            {
                userCart.Remove(existingItem);
            }
            else
            {
                var product = _products.FirstOrDefault(p => p.Id == updateDto.ProductId);
                if (product == null)
                {
                    return NotFound(new { message = "Ürün bilgisi bulunamadı." });
                }
                if (updateDto.Quantity > product.Stock)
                {
                    return BadRequest(new { message = $"Ürün stokta yeterli değil. Mevcut stok: {product.Stock}" });
                }
                existingItem.Quantity = updateDto.Quantity;
            }

            return Ok(GetEnrichedCartResponse(userId));
        }

        [HttpDelete("{productId}")]
        public IActionResult RemoveFromCart(string productId)
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!_userCarts.TryGetValue(userId, out List<CartItem>? userCart))
            {
                return NoContent();
            }

            userCart!.RemoveAll(item => item.ProductId == productId);

            return Ok(GetEnrichedCartResponse(userId));
        }

        [HttpDelete("clear")]
        public IActionResult ClearCart()
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (_userCarts.ContainsKey(userId))
            {
                _userCarts[userId] = new List<CartItem>();
            }
            return Ok(GetEnrichedCartResponse(userId));
        }
    }
}
