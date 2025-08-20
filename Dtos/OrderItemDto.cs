﻿namespace FolkaAPI.Dtos
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
    }
}