using NUnit.Framework;
using BattleShip.BLL.Requests;
using BattleShip.UI;
using BattleShip.UI.Enums;

namespace Battleship.Tests
{
    [TestFixture]
    public class EvaluateCoordinatesTests
    {
        [Test]
        public void CanEvaluateSingelXCoordinates()
        {
            GameFlow testGF = new GameFlow();
            Coordinate testXY;

            //Valid X Coordinate values: A-J & a-j (single character only)
            testXY = testGF.EvaluateCoordinates("A", "", out CoordinateInput response1);
            //Unless two valid coordinates are given, the function returns a Coordinate = null
            Assert.IsNull(testXY);

            //When a valid X Coordinate is given w/o a Y Coordinate, CoordinateInput returned in the out parameter has the value CoordinateInput.ValidCoordinate
            //Valid X Coordinate values can be single upper case characters [A-J]
            Assert.AreEqual(response1, CoordinateInput.ValidCoordinate);

            testXY = testGF.EvaluateCoordinates("Z", "", out CoordinateInput response2);

            //Invalid X Coordinate return the value CoordinateInput.ValidCoordinate in the out parameter
            Assert.AreEqual(response2, CoordinateInput.InvalidXCoordinate);

            testXY = testGF.EvaluateCoordinates("j", "", out CoordinateInput response3);

            //Valid X Coordinate values can be single characters [a-j]
            Assert.AreEqual(response3, CoordinateInput.ValidCoordinate);

            testXY = testGF.EvaluateCoordinates("abc", "", out CoordinateInput response4);

            //Invalid X Coordinates also include any multi character values
            Assert.AreEqual(response4, CoordinateInput.InvalidXCoordinate);
        }

        [Test]
        public void CanEvaluateSingelYCoordinates()
        {
            GameFlow testGF = new GameFlow();
            Coordinate testXY;

            //Valid Y Coordinate values: 0-9 (single digit only - 0 used for 10)
            testXY = testGF.EvaluateCoordinates("", "0", out CoordinateInput response1);
            //Unless two valid coordinates are given, the function returns a Coordinate = null
            Assert.IsNull(testXY);

            //When a valid Y Coordinate is given w/o an X Coordinate, CoordinateInput returned in the out parameter has the value CoordinateInput.ValidCoordinate
            //Valid Y Coordinate values can be single digits [0-9]
            Assert.AreEqual(response1, CoordinateInput.ValidCoordinate);

            testXY = testGF.EvaluateCoordinates("", "f", out CoordinateInput response2);

            //Invalid Y Coordinate return the value CoordinateInput.ValidCoordinate in the out parameter
            Assert.AreEqual(response2, CoordinateInput.InvalidYCoordinate);

            testXY = testGF.EvaluateCoordinates("", "1f2", out CoordinateInput response4);

            //Invalid Y Coordinates also include any multi character values
            Assert.AreEqual(response4, CoordinateInput.InvalidYCoordinate);

        }

        [Test]
        public void CoordinateReturnedWhenValidXYCoordinatesGiven()
        {
            GameFlow testGF = new GameFlow();
            Coordinate testXY;

            testXY = testGF.EvaluateCoordinates("A", "0", out CoordinateInput response1);
            //If valid X & Y coordinates are given, a Coordinate corresponding to them (A-J = 1-10) is returned
            Assert.IsNotNull(testXY);
            Assert.AreEqual("1", testXY.XCoordinate.ToString());
            Assert.AreEqual("10", testXY.YCoordinate.ToString());
            Assert.AreEqual(response1, CoordinateInput.ValidCoordinate);

            testXY = testGF.EvaluateCoordinates("i", "6", out CoordinateInput response2);
            //If valid X & Y coordinates are given, a Coordinate corresponding to them (A-J = 1-10) is returned
            Assert.IsNotNull(testXY);
            Assert.AreEqual("9", testXY.XCoordinate.ToString());
            Assert.AreEqual("6", testXY.YCoordinate.ToString());
            Assert.AreEqual(response2, CoordinateInput.ValidCoordinate);
        }

        [Test]
        public void CoordinateIsNullWhenInvalidXYCoordinatesGiven()
        {
            GameFlow testGF = new GameFlow();
            Coordinate testXY;

            testXY = testGF.EvaluateCoordinates("Z", "6", out CoordinateInput response1);
            //If an invalid X & a valid Y coordinate are given, a Coordinate = null is returned, and the value CoordinateInput.InvalidXCoordinate in the out parameter
            Assert.IsNull(testXY);
            Assert.AreEqual(response1, CoordinateInput.InvalidXCoordinate);

            testXY = testGF.EvaluateCoordinates("b", "-4", out CoordinateInput response2);
            //If a valid X & an invalid Y coordinate are given, a Coordinate = null is returned, and the value CoordinateInput.InvalidYCoordinate in the out parameter
            Assert.IsNull(testXY);
            Assert.AreEqual(response2, CoordinateInput.InvalidYCoordinate);
        }
    }
}
