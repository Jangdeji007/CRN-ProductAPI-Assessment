namespace CRN.ProductAPI.Domain.Entities
{
    public class Item
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public Product Product { get; set; } = null!;
    }
}
