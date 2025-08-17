namespace FolkaAPI.Models
{
    public class Product
    {
        // Her özelliğin türünün sonuna bir soru işareti (?) ekleyerek
        // bu değerlerin null olabileceğini belirtiyoruz.
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Price { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string? Category { get; set; }
    }
}
