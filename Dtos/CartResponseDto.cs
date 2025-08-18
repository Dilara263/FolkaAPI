using System.Collections.Generic;
using FolkaAPI.Models;

namespace FolkaAPI.Dtos
{
    public class CartResponseDto
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalPrice { get; set; }
    }
}
