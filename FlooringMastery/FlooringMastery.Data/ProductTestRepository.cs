using FlooringMastery.Models;
using FlooringMastery.Models.Interfaces;
using FlooringMastery.Models.Responses;
using System.Collections.Generic;


namespace FlooringMastery.Data
{
    public class ProductTestRepository : IProductRepository
    {
        private List<Product> _products;

        public ProductTestRepository()
        {
            _products = new List<Product>();
            _products.Add(new Product
            {
                ProductType = "Carpet",
                CostPerSquareFoot = 2.25M,
                LaborCostPerSquareFoot = 2.1M
            });
            _products.Add(new Product
            {
                ProductType = "Laminate",
                CostPerSquareFoot = 1.75M,
                LaborCostPerSquareFoot = 2.1M
            });
            _products.Add(new Product
            {
                ProductType = "Tile",
                CostPerSquareFoot = 3.5M,
                LaborCostPerSquareFoot = 4.15M
            });
            _products.Add(new Product
            {
                ProductType = "Wood",
                CostPerSquareFoot = 5.15M,
                LaborCostPerSquareFoot = 4.75M
            });
        }

        public ProductsResponse GetProducts()
        {
            ProductsResponse response = new ProductsResponse();

            response.Success = true;
            response.Products = _products;
            return response;
        }

        public ProductResponse GetProductByType(string productType)
        {
            ProductResponse response = new ProductResponse();

            //Find matching record, or return null
            //return 
            Product lookedupProduct = _products.Find(p => p.ProductType.ToUpper() == productType.ToUpper());
            if (lookedupProduct != null && lookedupProduct.ProductType.ToUpper() == productType.ToUpper())
            {
                response.Success = true;
                response.Product = lookedupProduct;
                return response;
            }
            else
            {
                response.Message = $"Could not find a product of type: {productType}!";
                response.Success = false;
                return response;
            }



        }
    }
}
