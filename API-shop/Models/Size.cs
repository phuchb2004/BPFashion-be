namespace API_shop.Models
{
    public class Size
    {
        public int sizeId { get; set; }
        public string sizeName { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
    }
}
