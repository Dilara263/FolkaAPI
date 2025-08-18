namespace FolkaAPI.Models
{
    public class Product
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Price { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Category { get; set; }
        public int Stock { get; set; } = 10;
    }
}
