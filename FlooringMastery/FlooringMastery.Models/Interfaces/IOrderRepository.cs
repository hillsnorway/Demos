using FlooringMastery.Models.Responses;
using System;
using System.Collections.Generic;


namespace FlooringMastery.Models.Interfaces
{
    public interface IOrderRepository
    {
        DateTime OrdersDate { get; }

        OrdersResponse GetOrders();
        OrderResponse GetOrder(int orderNumber);
        OrderResponse AddOrder(Order newOrder);
        OrderResponse EditOrder(Order editedOrder);
        OrderResponse DeleteOrder(int orderNumber);
        List<string> GetAllFileNames(out string dataPath);
    }
}
