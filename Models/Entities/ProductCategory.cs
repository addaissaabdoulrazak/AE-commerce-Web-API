namespace AE_CommerceApi.Models.Entities
{
    public class ProductCategory
    {
        public int Id { get; set; }

        //this just designates the names (Category Name, and Subcategory Name)
        public string Category { get; set; } = "";
        public string SubCategory { get; set; } = "";
    }
}
