namespace ASM.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Book? Book { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public int? OrderId { get; set; }
        public virtual Order? Order { get; set; }
    }
}
