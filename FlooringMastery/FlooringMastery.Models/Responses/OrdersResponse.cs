using System.Collections.Generic;


namespace FlooringMastery.Models.Responses
{
    public class OrdersResponse : Response
    {
        public List<Order> Orders { get; set; }

        public OrdersResponse()
        {
            this.Orders = new List<Order>();
        }
    }
}
