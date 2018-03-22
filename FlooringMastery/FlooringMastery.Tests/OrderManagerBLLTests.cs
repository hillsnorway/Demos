using FlooringMastery.BLL;
using FlooringMastery.Models;
using FlooringMastery.Models.Responses;
using NUnit.Framework;
using System;
using System.Configuration;
using System.IO;
using System.Linq;


namespace FlooringMastery.Tests
{
    [TestFixture]
    class OrderManagerBLLTests
    {
        [TestCase("01/01/0001", 0, "Joe* Cool", "OR", 8.19, 99, "Carpet", 12.5, 6.3, 1237.5, 623.7, 152.43, 2013.63, false)] //AddOrder Should fail - Date not in Future!
        [TestCase("01/01/9999", 0, "Joe* Cool", "OR", 8.19, 99, "Carpet", 12.5, 6.3, 1237.5, 623.7, 152.43, 2013.63, false)] //AddOrder Should fail - CustomerName ="Joe* Cool" has invalid character!
        [TestCase("01/01/9999", 0, "Joe Cool", "OR", 8.19, 99, "Carpet", 12.5, 6.3, 1237.5, 623.7, 152.43, 2013.63, false)]  //AddOrder Should fail - Area=99 is less than 100!
        [TestCase("01/01/9999", 0, "Joe Cool", "OR", 8.19, 100, "Carpet", 12.5, 6.3, 1250, 630, 152.43, 2033.97, false)]     //AddOrder Should fail - OR not a valid State in StateTax file!
        [TestCase("01/01/9999", 0, "Joe Cool", "TX", 8.19, 100, "Carpet", 12.5, 6.3, 1250, 630, 153.97, 2032.97, false)]     //AddOrder Should fail - FileTotal field is incorrect...
        [TestCase("01/01/9999", 0, "Joe Cool", "TX", 8.19, 100, "Carpet", 12.5, 6.3, 1250, 630, 153.97, 2033.97, true)]      //AddOrder Should succeed!!!
        [TestCase("01/01/9999", 0, "Cool, Joe jr.", "TX", 8.19, 100, "Carpet", 12.5, 6.3, 1250, 630, 153.97, 2033.97, true)] //AddOrder Should succeed!!!
        public void CanCreateValidOrder(DateTime oOrderDate, int oOrderNumber, string oCustomerName, string oState, decimal oTaxRate, decimal oArea, string oProductType, decimal oCostPerSquareFoot, decimal oLaborCostPerSquareFoot, decimal oFileMaterialCost, decimal oFileLaborCost, decimal oFileTax, decimal oFileTotal, bool expected)
        {
            Order addedOrder = new Order();
            addedOrder.OrderDate = oOrderDate;
            addedOrder.OrderNumber = oOrderNumber;
            addedOrder.CustomerName = oCustomerName;
            addedOrder.OrderStateTax.StateCode = oState;
            addedOrder.OrderStateTax.TaxRate = oTaxRate;
            addedOrder.Area = oArea;
            addedOrder.OrderProduct.ProductType = oProductType;
            addedOrder.OrderProduct.CostPerSquareFoot = oCostPerSquareFoot;
            addedOrder.OrderProduct.LaborCostPerSquareFoot = oLaborCostPerSquareFoot;
            addedOrder.FileMaterialCost = oFileMaterialCost;
            addedOrder.FileLaborCost = oFileLaborCost;
            addedOrder.FileTax = oFileTax;
            addedOrder.FileTotal = oFileTotal;

            OrderManager myOM = OrderManagerFactory.Create(addedOrder.OrderDate);
            OrderResponse actual = new OrderResponse();
            actual = myOM.AddOrder(addedOrder);
            Assert.AreEqual(expected, actual.Success);
        }

        //5,Charles Babbage,TX,8.19,Carpet,475,12.5,6.3,5937.5,2992.5,731.37,9661.37
        [TestCase("01/01/0001", 5, "Dude* Awesome", "OR", 9.46, 99, "Carpet", 12.5, 6.3, 1237.5, 623.7, 152.43, 2013.63, false)] //AddOrder Should fail - CustomerName ="Joe* Cool" has invalid character!
        [TestCase("01/01/0001", 5, "Dude Awesome", "OR", 9.46, 99, "Carpet", 12.5, 6.3, 1237.5, 623.7, 152.43, 2013.63, false)]  //AddOrder Should fail - Area=99 is less than 100!
        [TestCase("01/01/0001", 5, "Dude Awesome", "OR", 9.46, 475, "Carpet", 12.5, 6.3, 5937.5, 2992.5, 731.37, 9661.37, false)]       //AddOrder Should fail - OR not a valid State in StateTax file!
        [TestCase("01/01/0001", 5, "Dude Awesome", "TX", 8.19, 475, "Carpet", 12.5, 6.3, 5937.5, 2992.5, 731.37, 9660.37, false)]         //AddOrder Should fail - FileTotal field is incorrect...
        [TestCase("01/01/0001", 5, "Dude Awesome", "TX", 8.19, 475, "Carpet", 12.5, 6.3, 5937.5, 2992.5, 731.37, 9661.37, true)]          //AddOrder Should succeed!!!
        [TestCase("01/01/0001", 5, "Awesome, Dude...", "TX", 8.19, 475, "Carpet", 12.5, 6.3, 5937.5, 2992.5, 731.37, 9661.37, true)]     //AddOrder Should succeed!!!
        public void CanEditValidOrder(DateTime oOrderDate, int oOrderNumber, string oCustomerName, string oState, decimal oTaxRate, decimal oArea, string oProductType, decimal oCostPerSquareFoot, decimal oLaborCostPerSquareFoot, decimal oFileMaterialCost, decimal oFileLaborCost, decimal oFileTax, decimal oFileTotal, bool expected)
        {
            Order editedOrder = new Order();
            editedOrder.OrderDate = oOrderDate;
            editedOrder.OrderNumber = oOrderNumber;
            editedOrder.CustomerName = oCustomerName;
            editedOrder.OrderStateTax.StateCode = oState;
            editedOrder.OrderStateTax.TaxRate = oTaxRate;
            editedOrder.Area = oArea;
            editedOrder.OrderProduct.ProductType = oProductType;
            editedOrder.OrderProduct.CostPerSquareFoot = oCostPerSquareFoot;
            editedOrder.OrderProduct.LaborCostPerSquareFoot = oLaborCostPerSquareFoot;
            editedOrder.FileMaterialCost = oFileMaterialCost;
            editedOrder.FileLaborCost = oFileLaborCost;
            editedOrder.FileTax = oFileTax;
            editedOrder.FileTotal = oFileTotal;

            OrderManager myOM = OrderManagerFactory.Create(editedOrder.OrderDate);
            OrderResponse actual = new OrderResponse();
            actual = myOM.EditOrder(editedOrder);
            Assert.AreEqual(expected, actual.Success);
        }
    }
}
