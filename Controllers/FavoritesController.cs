using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FolkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private static List<string> _favoriteProductIds = new List<string>();

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetFavorites()
        {
            return Ok(_favoriteProductIds);
        }

        [HttpPost("{productId}")]
        public IActionResult AddFavorite(string productId)
        {
            if (!_favoriteProductIds.Contains(productId))
            {
                _favoriteProductIds.Add(productId);
            }
            return NoContent();
        }

        [HttpDelete("{productId}")]
        public IActionResult RemoveFavorite(string productId)
        {
            _favoriteProductIds.Remove(productId);
            return NoContent();
        }
    }
}
