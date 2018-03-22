using FlooringMastery.BLL;
using FlooringMastery.Data;
using FlooringMastery.Models;
using FlooringMastery.Models.Responses;
using NUnit.Framework;
using System.Configuration;
using System.IO;
using System.Linq;


namespace FlooringMastery.Tests
{
    [TestFixture]
    class ProductFileRepoTests
    {
        private static string _dataSuffix = ConfigurationManager.AppSettings["DataFileSuffix"].ToString();
        private static string _dataFileDateParseFormat = ConfigurationManager.AppSettings["DataFileDateParseFormat"].ToString();
        private static string _dataPath = ConfigurationManager.AppSettings["DataPath"].ToString();
        private static string _productFilePrefix = ConfigurationManager.AppSettings["ProductFileName"].ToString().Replace(_dataSuffix, "");
        private static string _productTestFile = _productFilePrefix + "Test" + _dataSuffix;

        [SetUp]
        public void TestSetup()
        {
            string _seedPath = _dataPath + @"TestSeeds\";

            if (File.Exists(_dataPath + _productTestFile))
            {
                File.Delete(_dataPath + _productTestFile);
            }
            File.Copy(_seedPath + _productTestFile, _dataPath + _productTestFile);
        }

        [Test]
        public void CanReadProducts()
        {
            //ProductManager myPM = ProductManagerFactory.Create();
            ProductFileRepository myPR = new ProductFileRepository(_dataPath + _productTestFile);
            ProductsResponse productsResponse = new ProductsResponse();
            productsResponse = myPR.GetProducts();

            //Did it retrieve 10 products?
            Assert.AreEqual(10, productsResponse.Products.Count());

            //Laminate,4.5,9.8
            Product productToValidate = productsResponse.Products.Find(p => p.ProductType == "Laminate");
            Assert.AreEqual("Laminate", productToValidate.ProductType);
            Assert.AreEqual(4.5, productToValidate.CostPerSquareFoot);
            Assert.AreEqual(9.8, productToValidate.LaborCostPerSquareFoot);

            //Bamboo,18.5,9.25
            productToValidate = productsResponse.Products.Find(p => p.ProductType == "Bamboo");
            Assert.AreEqual("Bamboo", productToValidate.ProductType);
            Assert.AreEqual(18.5, productToValidate.CostPerSquareFoot);
            Assert.AreEqual(9.25, productToValidate.LaborCostPerSquareFoot);
        }

        [Test]
        public void CanGetSpecificProduct()
        {
            //ProductManager myPM = ProductManagerFactory.Create();
            ProductFileRepository myPR = new ProductFileRepository(_dataPath + _productTestFile);
            ProductResponse productResponse = new ProductResponse();
            productResponse = myPR.GetProductByType("Graphite");
            Assert.IsFalse(productResponse.Success);

            productResponse = myPR.GetProductByType("Granite");
            Assert.IsTrue(productResponse.Success);

            //Granite,72.5,29.25
            Assert.AreEqual("Granite", productResponse.Product.ProductType);
            Assert.AreEqual(72.5, productResponse.Product.CostPerSquareFoot);
            Assert.AreEqual(29.25, productResponse.Product.LaborCostPerSquareFoot);
        }
    }
}
