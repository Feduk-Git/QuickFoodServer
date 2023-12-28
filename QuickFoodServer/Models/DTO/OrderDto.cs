namespace QuickFoodServer.Models.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public OrderStatus Status { get; set; }
        public string DateTime {get; set; }
        public List<CartItemDto> CartItems { get; set; }
        public City City { get; set; }
        public decimal Price { get; set; }
    }
}
