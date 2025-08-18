namespace FolkaAPI.Dtos
{
    public class AddToCartDto
    {
        public required string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
