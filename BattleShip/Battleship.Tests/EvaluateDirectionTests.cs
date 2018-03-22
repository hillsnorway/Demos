using NUnit.Framework;
using BattleShip.BLL.Requests;
using BattleShip.UI;
using BattleShip.UI.Enums;
using System;

namespace Battleship.Tests
{
    [TestFixture]
    public class EvaluateDirectionTests
    {
        [Test]
        public void CanEvaluateAllFourDirections()
        {
            GameFlow testGF = new GameFlow();
            Nullable<ShipDirection> testSD;
            string myShipDirection;


            myShipDirection = "Up";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"Up" is evaluated correctly
            Assert.AreEqual(testSD, ShipDirection.Up);

            myShipDirection = "Right";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"Right" is evaluated correctly
            Assert.AreEqual(testSD, ShipDirection.Right);

            myShipDirection = "Down";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"Down" is evaluated correctly
            Assert.AreEqual(testSD, ShipDirection.Down);

            myShipDirection = "Left";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"Left" is evaluated correctly
            Assert.AreEqual(testSD, ShipDirection.Left);
        }

        [Test]
        public void CanEvaluateMixedCaseDirections()
        {
            GameFlow testGF = new GameFlow();
            Nullable<ShipDirection> testSD;
            string myShipDirection;


            myShipDirection = "UP";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"UP" is evaluated correctly
            Assert.AreEqual(testSD, ShipDirection.Up);

            myShipDirection = "right";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"right" is evaluated correctly
            Assert.AreEqual(testSD, ShipDirection.Right);

            myShipDirection = "dOwN";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"dOwN" is evaluated correctly
            Assert.AreEqual(testSD, ShipDirection.Down);

            myShipDirection = "lefT";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"lefT" is evaluated correctly
            Assert.AreEqual(testSD, ShipDirection.Left);
        }

        [Test]
        public void CanEvaluateMistypedDirections()
        {
            GameFlow testGF = new GameFlow();
            Nullable<ShipDirection> testSD;
            string myShipDirection;


            myShipDirection = "uup";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"uup" is invalid
            Assert.IsNull(testSD);

            myShipDirection = "";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"" is invalid
            Assert.IsNull(testSD);

            myShipDirection = "DOne";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"DOne" is invalid
            Assert.IsNull(testSD);

            myShipDirection = "";
            testSD = testGF.EvaluateDirection(myShipDirection);
            //"1234567890" is invalid
            Assert.IsNull(testSD);
        }
    }
}
