namespace AE_CommerceApi.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        //Relation entre classe
        public ProductCategory? ProductCategory { get; set; } /*= new ProductCategory();*/

        //Relation entre classe <<offre>>
        public Offer? Offer { get; set; } /*= new Offer();*/

        public int Quantity { get; set; }
        public double Price { get; set; }

        public string ImageName { get; set; } = string.Empty;


    }
}
