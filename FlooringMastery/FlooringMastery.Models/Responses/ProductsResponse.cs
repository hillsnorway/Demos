using System.Collections.Generic;


namespace FlooringMastery.Models.Responses
{
    public class ProductsResponse : Response
    {
        public List<Product> Products { get; set; }

        public ProductsResponse()
        {
            this.Products = new List<Product>();
        }
    }
}
