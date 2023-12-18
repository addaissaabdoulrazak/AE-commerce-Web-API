using AE_CommerceApi.Models.Entities;

namespace AE_CommerceApi.Models.DAL
{
    public interface IDataAccess
    {
        List<ProductCategory> GetProductCategories();
        ProductCategory GetProductCategory(int id);

        Offer GetOffer(int id);

        List <Product> GetProduits(string categorie, string subcategory, int count);

        Product GetProduct(int id);

        bool InsertUser(User user);

        string IsUserPresent(string email, string password);
    }
}
