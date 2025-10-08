namespace API_shop.Models
{
    public class Color
    {
        public int colorId { get; set; }
        public string colorName { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
    }
}
