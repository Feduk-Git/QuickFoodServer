namespace QuickFoodServer.Models.DTO
{
    public class CartItemDto
    {
        public ProductDto ProductDto { get; set; }
        public int ItemsCount { get; set; }
        public decimal Price { get; set; }
    }
}
