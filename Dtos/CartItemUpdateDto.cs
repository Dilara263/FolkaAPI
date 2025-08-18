namespace FolkaAPI.Dtos
{
    public class CartItemUpdateDto
    {
        public required string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
