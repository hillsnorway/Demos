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
    class TaxFileRepoTests
    {
        private static string _dataSuffix = ConfigurationManager.AppSettings["DataFileSuffix"].ToString();
        private static string _dataFileDateParseFormat = ConfigurationManager.AppSettings["DataFileDateParseFormat"].ToString();
        private static string _dataPath = ConfigurationManager.AppSettings["DataPath"].ToString();
        private static string _taxFilePrefix = ConfigurationManager.AppSettings["TaxFileName"].ToString().Replace(_dataSuffix, "");
        private static string _taxTestFile = _taxFilePrefix + "Test" + _dataSuffix;

        [SetUp]
        public void TestSetup()
        {
            string _seedPath = _dataPath + @"TestSeeds\";

            if (File.Exists(_dataPath + _taxTestFile))
            {
                File.Delete(_dataPath + _taxTestFile);
            }
            File.Copy(_seedPath + _taxTestFile, _dataPath + _taxTestFile);
        }

        [Test]
        public void CanReadTaxes()
        {
            //TaxManager myTM = TaxManagerFactory.Create();
            TaxFileRepository myTR = new TaxFileRepository(_dataPath + _taxTestFile);
            TaxesResponse taxesResponse = new TaxesResponse();
            taxesResponse = myTR.GetTaxes();

            //Did it retrieve 7 taxes regions?
            Assert.AreEqual(7, taxesResponse.Taxes.Count());

            //HI,Hawaii,4.35
            StateTax stateToValidate = taxesResponse.Taxes.Find(t => t.StateCode == "HI");
            Assert.AreEqual("HI", stateToValidate.StateCode);
            Assert.AreEqual("Hawaii", stateToValidate.StateName);
            Assert.AreEqual(4.35, stateToValidate.TaxRate);

            //RI,Rhode Island,7
            stateToValidate = taxesResponse.Taxes.Find(t => t.StateCode == "RI");
            Assert.AreEqual("RI", stateToValidate.StateCode);
            Assert.AreEqual("Rhode Island", stateToValidate.StateName);
            Assert.AreEqual(7, stateToValidate.TaxRate);
        }

        [Test]
        public void CanGetStateTaxByStateCode()
        {
            //TaxManager myTM = TaxManagerFactory.Create();
            TaxFileRepository myTR = new TaxFileRepository(_dataPath + _taxTestFile);
            TaxResponse taxResponse = new TaxResponse();
            taxResponse = myTR.GetTaxByState("OR");
            Assert.IsFalse(taxResponse.Success);

            taxResponse = myTR.GetTaxByState("TN");
            Assert.IsTrue(taxResponse.Success);

            //TN,Tennessee,9.46
            Assert.AreEqual("TN", taxResponse.StateTax.StateCode);
            Assert.AreEqual("Tennessee", taxResponse.StateTax.StateName);
            Assert.AreEqual(9.46, taxResponse.StateTax.TaxRate);
        }
    }
}
