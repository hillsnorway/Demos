using FlooringMastery.Models.Interfaces;
using FlooringMastery.Models.Responses;


namespace FlooringMastery.BLL
{
    public class ProductManager
    {
        private IProductRepository _productRepo;

        public ProductManager(IProductRepository concrete)
        {
            _productRepo = concrete;
        }
        
        public ProductsResponse GetProducts()
        {
            return _productRepo.GetProducts();
        }

        public ProductResponse GetProductByType(string productName)
        {
            return _productRepo.GetProductByType(productName);
        }
    }
}
