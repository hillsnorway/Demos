

namespace FlooringMastery.Models.Responses
{
    public class OrderResponse : Response
    {
        public Order Order { get; set; }

        public OrderResponse()
        {
            this.Order = new Order();
        }
    }
}
