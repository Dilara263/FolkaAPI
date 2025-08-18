using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FolkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private static Dictionary<string, List<string>> _userFavoriteProductIds = new Dictionary<string, List<string>>();

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetFavorites()
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            if (!_userFavoriteProductIds.TryGetValue(userId, out List<string> userFavorites))
            {
                userFavorites = new List<string>();
                _userFavoriteProductIds.Add(userId, userFavorites);
            }

            if (userFavorites.Count == 0)
            {
                return NoContent();
            }

            return Ok(userFavorites);
        }

        [HttpPost("{productId}")]
        public IActionResult AddFavorite(string productId)
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!_userFavoriteProductIds.TryGetValue(userId, out List<string> userFavorites))
            {
                userFavorites = new List<string>();
                _userFavoriteProductIds.Add(userId, userFavorites);
            }

            if (!userFavorites.Contains(productId))
            {
                userFavorites.Add(productId);
            }
            return NoContent();
        }

        [HttpDelete("{productId}")]
        public IActionResult RemoveFavorite(string productId)
        {
            string userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (_userFavoriteProductIds.TryGetValue(userId, out List<string> userFavorites))
            {
                userFavorites.Remove(productId);
            }
            return NoContent();
        }
    }
}
