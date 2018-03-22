using FlooringMastery.Models;
using FlooringMastery.Models.Interfaces;
using FlooringMastery.Models.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;


namespace FlooringMastery.Data
{
    public class OrderTestRepository : IOrderRepository
    {
        public DateTime OrdersDate { get; private set; }
        private List<Order> _orders;

        public OrderTestRepository(DateTime orderDate)
        {
            OrdersDate = orderDate;
            _orders = new List<Order>();
            _orders.Add(new Order
            {
                OrderDate = orderDate,
                OrderNumber = 1,
                CustomerName = "Wise",
                OrderStateTax = new StateTax
                {
                    StateCode = "OH",
                    StateName = "Ohio",
                    TaxRate = 6.25M
                },
                OrderProduct = new Product
                {
                    ProductType = "Wood",
                    CostPerSquareFoot = 5.15M,
                    LaborCostPerSquareFoot = 4.75M
                },
                Area = 100M,
                FileMaterialCost = 515M,
                FileLaborCost = 475M,
                FileTax = 61.88M,
                FileTotal = 1051.88M
            });
            _orders.Add(new Order
            {
                OrderDate = orderDate,
                OrderNumber = 2,
                CustomerName = "Hill",
                OrderStateTax = new StateTax
                {
                    StateCode = "MI",
                    StateName = "Michigan",
                    TaxRate = 5.75M
                },
                OrderProduct = new Product
                {
                    ProductType = "Tile",
                    CostPerSquareFoot = 3.5M,
                    LaborCostPerSquareFoot = 4.15M
                },
                Area = 100M,
                FileMaterialCost = 350M,
                FileLaborCost = 415M,
                FileTax = 43.99M,
                FileTotal = 808.99M
            });
            //5,Charles Babbage, TN,9.46, Carpet,475,12.5,6.3,5937.5,2992.5,844.78,9774.78
            _orders.Add(new Order
            {
                OrderDate = orderDate,
                OrderNumber =  5,
                CustomerName = "Charles Babbage",
                OrderStateTax = new StateTax
                {
                    StateCode = "TN",
                    StateName = "Tenessee",
                    TaxRate = 9.46M
                },
                OrderProduct = new Product
                {
                    ProductType = "Carpet",
                    CostPerSquareFoot = 12.5M,
                    LaborCostPerSquareFoot = 6.3M
                },
                Area = 475M,
                FileMaterialCost = 5937.5M,
                FileLaborCost = 2992.5M,
                FileTax = 844.78M,
                FileTotal = 9774.78M
            });
        }

        public OrdersResponse GetOrders()
        {
            OrdersResponse response = new OrdersResponse();

            response.Success = true;
            response.Orders = _orders;
            return response;
        }

        public OrderResponse GetOrder(int orderNumber)
        {
            OrderResponse response = new OrderResponse();

            //Find matching record, or return null
            Order orderToFind = _orders.Find(o => o.OrderNumber == orderNumber);
            if (orderToFind.OrderNumber == orderNumber)
            {
                response.Success = true;
                response.Order = orderToFind;
                return response;
            }
            else
            {
                response.Message = $"Could not find an order matching the order number: {orderNumber}!";
                response.Success = false;
                return response;
            }
        }

        public OrderResponse AddOrder(Order newOrder)
        {
            OrderResponse response = new OrderResponse();

            if (OrdersDate == newOrder.OrderDate)
            {
                newOrder.OrderNumber = _orders.Max(o => o.OrderNumber) + 1;
                _orders.Add(newOrder);
                response.Success = true;
                response.Order = newOrder;
                return response;
            }
            else
            {
                //Need to add orders to a new file -- how implement this for testing?
                response.Message = $"Only able to add orders in Test Repo when new order date is same as Test Repo order date...";
                response.Success = false;
                return response;
            }
        }

        public OrderResponse EditOrder(Order editedOrder)
        {
            OrderResponse response = new OrderResponse();

            _orders.RemoveAll(o => o.OrderNumber == editedOrder.OrderNumber);
            _orders.Add(editedOrder);
            response.Success = true;
            response.Order = editedOrder;
            return response;
        }

        public OrderResponse DeleteOrder(int orderNumber)
        {
            OrderResponse response = new OrderResponse();

            //Find the order record to delete.
            Order orderToDelete = _orders.Find(o => o.OrderNumber == orderNumber);
            if (orderToDelete != null)
            {
                _orders.Remove(orderToDelete);
                response.Order = orderToDelete;
                response.Success = true;
                return response;
            }
            else
            {
                response.Message = $"Unable to locate record to delete for order number {orderNumber}.  The record was not deleted!";
                response.Success = false;
                return response;
            }
        }

        public List<string> GetAllFileNames(out string dataPath)
        {
            //Mocking a file name here...
            dataPath = ConfigurationManager.AppSettings["DataPath"].ToString();
            string filePrefix = ConfigurationManager.AppSettings["DataFilePrefix"].ToString();
            string fileSuffix = ConfigurationManager.AppSettings["DataFileSuffix"].ToString();
            string dataFileDateParseFormat = ConfigurationManager.AppSettings["DataFileDateParseFormat"].ToString();
            string fileName = DateTime.Now.Date.ToString(dataFileDateParseFormat);
            List<string> fileNames = new List<string>();
            fileNames.Add(dataPath+filePrefix +fileName+fileSuffix);
            return fileNames;
        }
    }
}
