namespace FolkaAPI.Models
{
    public class User
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; } //hash'lenerek saklanır
        public string? PhoneNumber { get; set; } // Yeni eklendi
        public string? Address { get; set; } // Yeni eklendi
    }
}
