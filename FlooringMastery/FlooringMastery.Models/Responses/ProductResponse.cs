

namespace FlooringMastery.Models.Responses
{
    public class ProductResponse : Response
    {
        public Product Product { get; set; }

        public ProductResponse()
        {
            this.Product = new Product();
        }
    }
}
