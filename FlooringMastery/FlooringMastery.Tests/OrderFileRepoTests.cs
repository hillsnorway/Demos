using FlooringMastery.BLL;
using FlooringMastery.Data;
using FlooringMastery.Models;
using FlooringMastery.Models.Responses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlooringMastery.Tests
{
    [TestFixture]
    class OrderFileRepoTests
    {
        private static DateTime _orderDateEasy = default(DateTime).Date; //System default date is used for testing Easy Test Cases...
        private static DateTime _orderDateHard = default(DateTime).Date.AddDays(1); //System default date + 1 day is used for testing Hard Test Cases...
        private static string _dataFilePrefix = ConfigurationManager.AppSettings["DataFilePrefix"].ToString();
        private static string _dataSuffix = ConfigurationManager.AppSettings["DataFileSuffix"].ToString();
        private static string _dataFileDateParseFormat = ConfigurationManager.AppSettings["DataFileDateParseFormat"].ToString();
        //If the DataFileDateParseFormat is changed, the physical name of the Orders_01010001.txt seed file (stored in the TestSeeds folder) must be changed to match the new format!!!
        private static string _dataTestFileEasy = _dataFilePrefix + _orderDateEasy.ToString(_dataFileDateParseFormat) + _dataSuffix;
        private static string _dataTestFileHard = _dataFilePrefix + _orderDateHard.ToString(_dataFileDateParseFormat) + _dataSuffix;
        private static string _dataPath = ConfigurationManager.AppSettings["DataPath"].ToString();

        [SetUp]
        public void TestSetup()
        {
            //bool _isFileRepoMode = ConfigurationManager.AppSettings["Mode"].ToString().Replace(_dataSuffix, "").ToUpper()=="LIVE";
            string _seedPath = _dataPath + @"TestSeeds\";

            string _taxFilePrefix = ConfigurationManager.AppSettings["TaxFileName"].ToString().Replace(_dataSuffix, "");
            string _taxTestFile = _taxFilePrefix + "Test" + _dataSuffix;
            string _productFilePrefix = ConfigurationManager.AppSettings["ProductFileName"].ToString().Replace(_dataSuffix, "");
            string _productTestFile = _productFilePrefix + "Test" + _dataSuffix;

            if (File.Exists(_dataPath + _dataTestFileEasy))
            {
                File.Delete(_dataPath + _dataTestFileEasy);
            }
            File.Copy(_seedPath + _dataTestFileEasy, _dataPath + _dataTestFileEasy);

            if (File.Exists(_dataPath + _dataTestFileHard))
            {
                File.Delete(_dataPath + _dataTestFileHard);
            }
            File.Copy(_seedPath + _dataTestFileHard, _dataPath + _dataTestFileHard);

            if (File.Exists(_dataPath + _taxTestFile))
            {
                File.Delete(_dataPath + _taxTestFile);
            }
            File.Copy(_seedPath + _taxTestFile, _dataPath + _taxTestFile);

            if (File.Exists(_dataPath + _productTestFile))
            {
                File.Delete(_dataPath + _productTestFile);
            }
            File.Copy(_seedPath + _productTestFile, _dataPath + _productTestFile);
        }

        [Test]
        public void CanReadOrdersFromEasyFile()
        {
            //OrderManager myOM = OrderManagerFactory.Create(_orderDateEasy);
            OrderFileRepository myOFR = new OrderFileRepository(_orderDateEasy);
            OrdersResponse ordersResponse = new OrdersResponse();
            ordersResponse = myOFR.GetOrders();

            //Did it retrieve 10 orders?
            Assert.AreEqual(10, ordersResponse.Orders.Count());

            //1,Ada Lovelace,TX,8.19,Wood,2203,12.5,9.25,27537.5,20377.75,3924.26,51839.51
            Order orderToValidate = ordersResponse.Orders.Find(o => o.OrderNumber == 1);
            Assert.AreEqual(_orderDateEasy, orderToValidate.OrderDate);
            Assert.AreEqual(1, orderToValidate.OrderNumber);
            Assert.AreEqual("Ada Lovelace", orderToValidate.CustomerName);
            Assert.AreEqual("TX", orderToValidate.OrderStateTax.StateCode);
            Assert.AreEqual(8.19, orderToValidate.OrderStateTax.TaxRate);
            Assert.AreEqual("Wood", orderToValidate.OrderProduct.ProductType);
            Assert.AreEqual(2203, orderToValidate.Area);
            Assert.AreEqual(12.5, orderToValidate.OrderProduct.CostPerSquareFoot);
            Assert.AreEqual(9.25, orderToValidate.OrderProduct.LaborCostPerSquareFoot);
            Assert.AreEqual(27537.5, orderToValidate.FileMaterialCost);
            Assert.AreEqual(20377.75, orderToValidate.FileLaborCost);
            Assert.AreEqual(3924.26, orderToValidate.FileTax);
            Assert.AreEqual(51839.51, orderToValidate.FileTotal);

            //10,Elon Musk,TX,8.19,Carpet,1532,12.5,6.3,19150.0,9651.6,2358.85104,31160.45104
            orderToValidate = ordersResponse.Orders.Find(o => o.OrderNumber == 10);
            Assert.AreEqual(_orderDateEasy, orderToValidate.OrderDate);
            Assert.AreEqual(10, orderToValidate.OrderNumber);
            Assert.AreEqual("Elon Musk", orderToValidate.CustomerName);
            Assert.AreEqual("TX", orderToValidate.OrderStateTax.StateCode);
            Assert.AreEqual(8.19, orderToValidate.OrderStateTax.TaxRate);
            Assert.AreEqual("Carpet", orderToValidate.OrderProduct.ProductType);
            Assert.AreEqual(1532, orderToValidate.Area);
            Assert.AreEqual(12.5, orderToValidate.OrderProduct.CostPerSquareFoot);
            Assert.AreEqual(6.3, orderToValidate.OrderProduct.LaborCostPerSquareFoot);
            Assert.AreEqual(19150.0, orderToValidate.FileMaterialCost);
            Assert.AreEqual(9651.6, orderToValidate.FileLaborCost);
            Assert.AreEqual(2358.85104, orderToValidate.FileTax);
            Assert.AreEqual(31160.45104, orderToValidate.FileTotal);
        }

        [Test]
        public void CanReadOrdersFromHardFile()
        {
            //OrderManager myOM = OrderManagerFactory.Create(_orderDateHard);
            OrderFileRepository myOFR = new OrderFileRepository(_orderDateHard);
            OrdersResponse ordersResponse = new OrdersResponse();
            ordersResponse = myOFR.GetOrders();
            //Did it retrieve 26 orders?
            Assert.AreEqual(26, ordersResponse.Orders.Count());

            //72751,"Alan M. Galloway, Sr.",XX,9.46,Wood,183934,12.5,9.25,2299175,1701389.5,378453.4,4379017.9
            Order orderToValidate = ordersResponse.Orders.Find(o => o.OrderNumber == 72751);
            Assert.AreEqual(_orderDateHard, orderToValidate.OrderDate);
            Assert.AreEqual("Alan M. Galloway, Sr.", orderToValidate.CustomerName);
            Assert.AreEqual("XX", orderToValidate.OrderStateTax.StateCode);
            Assert.AreEqual(9.46M, orderToValidate.OrderStateTax.TaxRate);
            Assert.AreEqual("Wood", orderToValidate.OrderProduct.ProductType);
            Assert.AreEqual(183934M, orderToValidate.Area);
            Assert.AreEqual(12.5M, orderToValidate.OrderProduct.CostPerSquareFoot);
            Assert.AreEqual(9.25M, orderToValidate.OrderProduct.LaborCostPerSquareFoot);
            Assert.AreEqual(2299175M, orderToValidate.FileMaterialCost);
            Assert.AreEqual(1701389.5M, orderToValidate.FileLaborCost);
            Assert.AreEqual(378453.4M, orderToValidate.FileTax);
            Assert.AreEqual(4379017.9M, orderToValidate.FileTotal);

            //73430,"Monsters Inc.",LO,9.46,Carpet,21601,12.5,6.3,270012.5,136086.3,38416.95,444515.75
            orderToValidate = ordersResponse.Orders.Find(o => o.OrderNumber == 73430);
            Assert.AreEqual(_orderDateHard, orderToValidate.OrderDate);
            Assert.AreEqual(73430, orderToValidate.OrderNumber);
            Assert.AreEqual("Monsters Inc.", orderToValidate.CustomerName);
            Assert.AreEqual("LO", orderToValidate.OrderStateTax.StateCode);
            Assert.AreEqual(9.46M, orderToValidate.OrderStateTax.TaxRate);
            Assert.AreEqual("Carpet", orderToValidate.OrderProduct.ProductType);
            Assert.AreEqual(21601M, orderToValidate.Area);
            Assert.AreEqual(12.5M, orderToValidate.OrderProduct.CostPerSquareFoot);
            Assert.AreEqual(6.3M, orderToValidate.OrderProduct.LaborCostPerSquareFoot);
            Assert.AreEqual(270012.5M, orderToValidate.FileMaterialCost);
            Assert.AreEqual(136086.3M, orderToValidate.FileLaborCost);
            Assert.AreEqual(38416.95M, orderToValidate.FileTax);
            Assert.AreEqual(444515.75M, orderToValidate.FileTotal);
        }

        [Test]
        public void CanReadSpecificOrderFromEasyFile()
        {
            //OrderManager myOM = OrderManagerFactory.Create(_orderDateEasy);
            OrderFileRepository myOFR = new OrderFileRepository(_orderDateEasy);
            OrderResponse orderResponse = new OrderResponse();

            //Verify we can get OrderNr 2
            //2,Albert Einstein,TX,8.19,Tile,2336,1.5,14.3,3504,33404.8,3022.83,39931.63
            orderResponse = myOFR.GetOrder(2);
            Assert.IsTrue(orderResponse.Success);
            Assert.AreEqual("Albert Einstein", orderResponse.Order.CustomerName);
        }

        [Test]
        public void CanReadQuoteDelimitedCommaOrderFromHardFile()
        {
            //OrderManager myOM = OrderManagerFactory.Create(_orderDateHard);
            OrderFileRepository myOFR = new OrderFileRepository(_orderDateHard);
            OrdersResponse ordersResponse = new OrdersResponse();
            ordersResponse = myOFR.GetOrders();

            Assert.AreEqual(26, ordersResponse.Orders.Count());

            //11,"Hill, Nathan",KY,6,Marble,250000,88.73,34.25,22182500.00,8562500.00,1844700.0000,32589700.0000
            Order orderToValidate = ordersResponse.Orders.Find(o => o.OrderNumber == 11);
            Assert.AreEqual(_orderDateHard, orderToValidate.OrderDate);
            Assert.AreEqual(11, orderToValidate.OrderNumber);
            Assert.AreEqual("Hill, Nathan", orderToValidate.CustomerName);
            Assert.AreEqual("KY", orderToValidate.OrderStateTax.StateCode);
            Assert.AreEqual(6, orderToValidate.OrderStateTax.TaxRate);
            Assert.AreEqual("Marble", orderToValidate.OrderProduct.ProductType);
            Assert.AreEqual(250000M, orderToValidate.Area);
            Assert.AreEqual(88.73M, orderToValidate.OrderProduct.CostPerSquareFoot);
            Assert.AreEqual(34.25M, orderToValidate.OrderProduct.LaborCostPerSquareFoot);
            Assert.AreEqual(22182500.00M, orderToValidate.FileMaterialCost);
            Assert.AreEqual(8562500.00M, orderToValidate.FileLaborCost);
            Assert.AreEqual(1844700.00M, orderToValidate.FileTax);
            Assert.AreEqual(32589700.00M, orderToValidate.FileTotal);

            //73144,"Thurston Howell, III",KY,6,Marble,65298,88.73,34.25,5793891.54,2236456.5,481820.88,8512168.92
            orderToValidate = ordersResponse.Orders.Find(o => o.OrderNumber == 73144);
            Assert.AreEqual(_orderDateHard, orderToValidate.OrderDate);
            Assert.AreEqual(73144, orderToValidate.OrderNumber);
            Assert.AreEqual("Thurston Howell, III", orderToValidate.CustomerName);
            Assert.AreEqual("KY", orderToValidate.OrderStateTax.StateCode);
            Assert.AreEqual(6, orderToValidate.OrderStateTax.TaxRate);
            Assert.AreEqual("Marble", orderToValidate.OrderProduct.ProductType);
            Assert.AreEqual(65298M, orderToValidate.Area);
            Assert.AreEqual(88.73M, orderToValidate.OrderProduct.CostPerSquareFoot);
            Assert.AreEqual(34.25M, orderToValidate.OrderProduct.LaborCostPerSquareFoot);
            Assert.AreEqual(5793891.54M, orderToValidate.FileMaterialCost);
            Assert.AreEqual(2236456.5M, orderToValidate.FileLaborCost);
            Assert.AreEqual(481820.88M, orderToValidate.FileTax);
            Assert.AreEqual(8512168.92M, orderToValidate.FileTotal);
        }

        [Test]
        public void CanDeleteOrderFromEasyFile()
        {
            //OrderManager myOM = OrderManagerFactory.Create(_orderDateEasy);
            OrderFileRepository myOFR = new OrderFileRepository(_orderDateEasy);
            OrderResponse orderResponse = new OrderResponse();

            //Verify we can get OrderNr 1
            //1,Ada Lovelace,TX,8.19,Wood,2203,12.5,9.25,27537.5,20377.75,3924.26,51839.51
            orderResponse = myOFR.GetOrder(1);
            Assert.IsTrue(orderResponse.Success);
            Assert.AreEqual("Ada Lovelace", orderResponse.Order.CustomerName);

            //Delete OrderNr 1
            orderResponse = myOFR.DeleteOrder(1);
            //Verify which order 1 was the one deleted
            Assert.AreEqual("Ada Lovelace", orderResponse.Order.CustomerName);
            orderResponse = myOFR.GetOrder(1);
            //Should not be able to find OrderNr1 now...
            Assert.IsFalse(orderResponse.Success);

            //Verify we can get OrderNr 10
            //10,Elon Musk,TX,8.19,Carpet,1532,12.5,6.3,19150.0,9651.6,2358.85104,31160.45104
            orderResponse = myOFR.GetOrder(10);
            Assert.IsTrue(orderResponse.Success);
            Assert.AreEqual("Elon Musk", orderResponse.Order.CustomerName);

            //Delete OrderNr 10
            orderResponse = myOFR.DeleteOrder(10);
            //Verify which order 10 was the one deleted
            Assert.AreEqual("Elon Musk", orderResponse.Order.CustomerName);
            orderResponse = myOFR.GetOrder(10);
            //Should not be able to find OrderNr10 now...
            Assert.IsFalse(orderResponse.Success);

            //Verify we can get OrderNr 2
            //2,Albert Einstein,TX,8.19,Tile,2336,1.5,14.3,3504,33404.8,3022.83,39931.63
            orderResponse = myOFR.GetOrder(2);
            Assert.IsTrue(orderResponse.Success);
            Assert.AreEqual("Albert Einstein", orderResponse.Order.CustomerName);
        }

        [Test]
        public void CanAddOrderInExistingFile()
        {
            //Make sure the file does not exist from before
            DateTime orderDate = _orderDateEasy.AddYears(9998); //01.01.9999
            string testFile = _dataPath + _dataFilePrefix + orderDate.ToString(_dataFileDateParseFormat) + _dataSuffix;
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
            }
            Assert.IsFalse(File.Exists(testFile));

            CanCreateNewOrderToNewFile(false);//Create a file with only 1 order, and don't delete it.
            Assert.IsTrue(File.Exists(testFile));

            Order addedOrder = new Order();
            addedOrder.OrderDate = orderDate; ////01.01.9999
            addedOrder.OrderNumber = 0;
            addedOrder.CustomerName = "Dude Awesome";
            addedOrder.OrderStateTax.StateCode = "TX";
            addedOrder.OrderStateTax.TaxRate = 8.19M;
            addedOrder.Area = 100;
            addedOrder.OrderProduct.ProductType = "Carpet";
            addedOrder.OrderProduct.CostPerSquareFoot = 12.5M;
            addedOrder.OrderProduct.LaborCostPerSquareFoot = 6.3M;
            addedOrder.FileMaterialCost = 1250M;
            addedOrder.FileLaborCost = 630M;
            addedOrder.FileTax = 153.97M;
            addedOrder.FileTotal = 2033.97M;

            //OrderManager myOM = OrderManagerFactory.Create(orderDate);
            OrderFileRepository myOFR = new OrderFileRepository(orderDate);
            OrderResponse orderResponse = new OrderResponse();
            orderResponse = myOFR.AddOrder(addedOrder);
            Assert.IsTrue(orderResponse.Success);
            //Newly added order should now have OrderNr. 2
            Assert.AreEqual(2, orderResponse.Order.OrderNumber);
            //Open up this specific file, and check the CustomerName matches.
            orderResponse = myOFR.GetOrder(2);
            Assert.IsTrue(orderResponse.Success);
            Assert.AreEqual("Dude Awesome", orderResponse.Order.CustomerName);
        }

        private void CanCreateNewOrderToNewFile(bool cleanUp)
        {
            Order newOrder = new Order();
            newOrder.OrderDate = _orderDateEasy.AddYears(9998); //01.01.9999
            newOrder.OrderNumber = 0;
            newOrder.CustomerName = "Cool, Joe jr.";
            newOrder.OrderStateTax.StateCode = "TX";
            newOrder.OrderStateTax.TaxRate = 8.19M;
            newOrder.Area = 100;
            newOrder.OrderProduct.ProductType = "Carpet";
            newOrder.OrderProduct.CostPerSquareFoot = 12.5M;
            newOrder.OrderProduct.LaborCostPerSquareFoot = 6.3M;
            newOrder.FileMaterialCost = 1250M;
            newOrder.FileLaborCost = 630M;
            newOrder.FileTax = 153.97M;
            newOrder.FileTotal = 2033.97M;

            //Make sure the file does not exist from before
            string testFile = _dataPath + _dataFilePrefix + newOrder.OrderDate.ToString(_dataFileDateParseFormat) + _dataSuffix;
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
            }

            //OrderManager myOM = OrderManagerFactory.Create(newOrder.OrderDate);
            OrderFileRepository myOFR = new OrderFileRepository(newOrder.OrderDate);
            OrderResponse orderResponse = new OrderResponse();
            orderResponse = myOFR.AddOrder(newOrder);
            Assert.IsTrue(orderResponse.Success);
            Assert.IsTrue(File.Exists(testFile));

            //Clean up...
            if (cleanUp)
            {
                if (File.Exists(testFile))
                {
                    File.Delete(testFile);
                }
            }

        }

        [Test]
        public void CanCreateNewOrderToNewFile()
        {
            CanCreateNewOrderToNewFile(true);
        }

        [Test]
        public void CanDeleteLastOrderAndRemoveFile()
        {
            //Make sure the file does not exist from before
            DateTime orderDate = _orderDateEasy.AddYears(9998); //01.01.9999
            string testFile = _dataPath + _dataFilePrefix + orderDate.ToString(_dataFileDateParseFormat) + _dataSuffix;
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
            }
            Assert.IsFalse(File.Exists(testFile));

            //Call previous test to make sure file with single order exists...
            CanCreateNewOrderToNewFile(false); ///Adds a new file Orders_01019999.txt with 1 order, and leaves the file when done...
            Assert.IsTrue(File.Exists(testFile));

            //OrderManager myOM = OrderManagerFactory.Create(orderDate);
            OrderFileRepository myOFR = new OrderFileRepository(orderDate);
            OrderResponse orderResponse = new OrderResponse();
            orderResponse = myOFR.DeleteOrder(1);
            Assert.IsTrue(orderResponse.Success); //Delete Successful
            Assert.IsFalse(File.Exists(testFile)); //File Now Deleted!

            //Clean up...
            if (File.Exists(testFile))
            {
                File.Delete(testFile);
            }
        }
    }
}
