using FlooringMastery.Models.Responses;


namespace FlooringMastery.Models.Interfaces
{
    public interface IProductRepository
    {
        ProductsResponse GetProducts();
        ProductResponse GetProductByType(string productType);
    }
}
