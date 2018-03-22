using FlooringMastery.Models;
using FlooringMastery.Models.Interfaces;
using FlooringMastery.Models.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace FlooringMastery.BLL
{
    public class OrderManager
    {
        private IOrderRepository _orderRepo;

        private static int _minArea = int.Parse(ConfigurationManager.AppSettings["MinArea"]);
        private static int _maxArea = int.Parse(ConfigurationManager.AppSettings["MaxArea"]);
        private static int _maxCustNameLength = int.Parse(ConfigurationManager.AppSettings["MaxCustNameLength"]);

        public OrderManager(IOrderRepository concrete)
        {
            _orderRepo = concrete;
        }

        public OrdersResponse GetOrders()
        {
            return _orderRepo.GetOrders();
        }

        public OrderResponse GetOrder(int orderNumber)
        {
            return _orderRepo.GetOrder(orderNumber);
        }

        public OrderResponse AddOrder(Order newOrder)
        {
            Response response = new Response();
            response = ValidOrder(newOrder, true);
            if (response.Success) return _orderRepo.AddOrder(newOrder);
            else
            {
                OrderResponse orderResponse = new OrderResponse();
                orderResponse.Message = response.Message;
                orderResponse.Success = false;
                return orderResponse;
            }
        }

        public OrderResponse EditOrder(Order editedOrder)
        {
            Response response = new Response();
            response = ValidOrder(editedOrder, false);
            if (response.Success) return _orderRepo.EditOrder(editedOrder);
            else
            {
                OrderResponse orderResponse = new OrderResponse();
                orderResponse.Message = response.Message;
                orderResponse.Success = false;
                return orderResponse;
            }
        }

        public OrderResponse DeleteOrder(int orderNumber)
        {
            return _orderRepo.DeleteOrder(orderNumber);
        }

        public List<string> GetAllFileNames(out string dataPath)
        {
            return _orderRepo.GetAllFileNames(out dataPath);
        }

        private static Response ValidOrder (Order order, bool isNewOrder)
        {
            Response response = new Response();

            //Setting Success true by default, if errors are found it will be set to false then...
            response.Success = true;
            //Validate that each element in the order object (except calculated fields, and OrderStateTax.StateName) contains a value other than the object's default value.
            response.Message = "The order could not be processed due to the following problems: \n";

            foreach (PropertyInfo propertyInfo in order.GetType().GetProperties())
            {
                switch (propertyInfo.Name)
                {
                    case "OrderDate":
                        if (isNewOrder && order.OrderDate < DateTime.Now.Date)
                        {
                            response.Message += "* OrderDate invalid! \n";
                            response.Success = false;
                        }
                        break;
                    case "OrderNumber":
                        if (!isNewOrder && order.OrderNumber == 0)
                        {
                            response.Message += "* OrderNumber invalid! \n";
                            response.Success = false;
                        }
                        break;
                    case "CustomerName":
                        if (order.CustomerName == "" || order.CustomerName.Length > _maxCustNameLength || !(order.CustomerName.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '.' || c == ',')))
                        {
                            response.Message += "* CustomerName invalid! \n";
                            response.Success = false;
                        };
                        break;
                    case "OrderStateTax":
                        TaxManager myTM = TaxManagerFactory.Create();
                        TaxResponse myTR = new TaxResponse();
                        myTR = myTM.GetTaxByState(order.OrderStateTax.StateCode);
                        //if ((!myTR.Success) && (!(order.OrderStateTax.StateCode == myTR.StateTax.StateCode && order.OrderStateTax.TaxRate == myTR.StateTax.TaxRate)) )
                        //{
                            
                        //    response.Message += "* One or more of the StateTax values was invalid! \n";
                        //    response.Success = false;
                        //}
                        if (!myTR.Success)
                        {
                            response.Message += "* " + myTR.Message + "  \nContact IT! \n";
                            response.Success = false;
                        }
                        else if (!(order.OrderStateTax.StateCode == myTR.StateTax.StateCode && order.OrderStateTax.TaxRate == myTR.StateTax.TaxRate)) {
                            response.Message += "* One or more of the StateTax values was invalid! \n";
                            response.Success = false;
                        }
                        break;
                    case "OrderProduct":
                        ProductManager myPM = ProductManagerFactory.Create();
                        ProductResponse myPR = new ProductResponse();
                        myPR = myPM.GetProductByType(order.OrderProduct.ProductType);
                        if (!myPR.Success && !(order.OrderProduct.ProductType == myPR.Product.ProductType && order.OrderProduct.CostPerSquareFoot == myPR.Product.CostPerSquareFoot && order.OrderProduct.LaborCostPerSquareFoot == myPR.Product.LaborCostPerSquareFoot))
                        {
                            response.Message += "* One or more of the ProductType values was invalid! \n";
                            response.Success = false;
                        }
                        break;
                    case "Area":
                        if (order.Area < _minArea || order.Area > _maxArea)
                        {
                            response.Message += "* The Area is invalid! \n";
                            response.Success = false;
                        }
                        break;
                    case "FileLaborCost":
                        if (order.FileLaborCost != order.CalcLaborCost)//Math.Round(order.Area * order.OrderProduct.LaborCostPerSquareFoot, MidpointRounding.AwayFromZero))
                        {
                            response.Message += "* The LaborCost is invalid! \n";
                            response.Success = false;
                        }
                        break;
                    case "FileMaterialCost":
                        if (order.FileMaterialCost != order.CalcMaterialCost)// Math.Round(order.Area * order.OrderProduct.CostPerSquareFoot, MidpointRounding.AwayFromZero))
                        {
                            response.Message += "* The MaterialCost is invalid! \n";
                            response.Success = false;
                        }
                        break;
                    case "FileTax":
                        if (order.FileTax != order.CalcTax)//Math.Round((((order.Area * order.OrderProduct.CostPerSquareFoot) + (order.Area * order.OrderProduct.LaborCostPerSquareFoot)) * order.OrderStateTax.TaxRate/100),2, MidpointRounding.AwayFromZero))
                        {
                            response.Message += "* The TaxCost is invalid! \n";
                            response.Success = false;
                        }
                        break;
                    case "FileTotal":
                        if (order.FileTotal != order.CalcTotal)//Math.Round(order.FileMaterialCost +  order.FileLaborCost + order.FileTax, 2, MidpointRounding.AwayFromZero))
                        {
                            response.Message += "* The TotalCost is invalid! \n";
                            response.Success = false;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (response.Success) response.Message = "";
            else response.Message = response.Message.Substring(0, response.Message.Length - 1); //Remove final '\n' - counts as 1 char...
            return response;
        }
    }
}
