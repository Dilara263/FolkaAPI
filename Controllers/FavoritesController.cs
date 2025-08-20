using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using FolkaAPI.Data;
using FolkaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FolkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetFavorites()
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userFavoriteProductIds = await _context.UserFavorites
                                                    .Where(uf => uf.UserId == userId)
                                                    .Select(uf => uf.ProductId)
                                                    .ToListAsync();

            if (userFavoriteProductIds.Count == 0)
            {
                return NoContent();
            }

            return Ok(userFavoriteProductIds);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddFavorite(string productId)
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var existingFavorite = await _context.UserFavorites
                                                .AnyAsync(uf => uf.UserId == userId && uf.ProductId == productId);

            if (existingFavorite)
            {
                return BadRequest(new { message = "Ürün zaten favorilerde." });
            }

            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
            {
                return NotFound(new { message = "Favorilere eklenecek ürün bulunamadı." });
            }

            _context.UserFavorites.Add(new UserFavorite { UserId = userId, ProductId = productId });
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFavorite(string productId)
        {
            string? userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var favoriteToRemove = await _context.UserFavorites
                                                .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.ProductId == productId);

            if (favoriteToRemove == null)
            {
                return NoContent();
            }

            _context.UserFavorites.Remove(favoriteToRemove);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}