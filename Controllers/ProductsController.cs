using FolkaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FolkaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private static readonly List<Product> _products = new List<Product>
        {
            new Product { Id = "1", Name = "Hand-Knitted Tote Bag", Price = "250 TL", Description = "Yüksek kaliteli iplikle elde örülmüş, güzel ve dayanıklı bir bez çanta. Günlük kullanım için mükemmeldir.", Image = "http://10.0.2.2:5227/images/totteBag.webp", Category = "Çanta" },
            new Product { Id = "2", Name = "Bohemian Tassel Necklace", Price = "120 TL", Description = "Bu eşsiz, el yapımı püsküllü kolye ile tarzınıza bohem-şık bir dokunuş katın.", Image = "http://10.0.2.2:5227/images/tassel.jpg", Category = "Takı" },
            new Product { Id = "3", Name = "El Örgüsü Omuz Çantası", Price = "350 TL", Description = "Zarif, el yapımı omuz çantası. Her kıyafetle uyum sağlar.", Image = "https://picsum.photos/seed/bag2/400/400", Category = "Çanta" },
            new Product { Id = "4", Name = "Silk Floral Scarf (Fular)", Price = "150 TL", Description = "Güzel bir çiçek baskısına sahip, hafif ve yumuşak bir ipek fular. Çok yönlü ve şık.", Image = "http://10.0.2.2:5227/images/fular.webp", Category = "Fular" },
            new Product { Id = "5", Name = "Gümüş Hayat Ağacı Kolye", Price = "450 TL", Description = "925 ayar gümüş, el işçiliği hayat ağacı kolye.", Image = "https://picsum.photos/seed/necklace2/400/400", Category = "Takı" }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return Ok(_products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(string id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }
  
            return Ok(product);
        }
    }
}
