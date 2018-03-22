using System;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using BattleShip.BLL.Ships;
using BattleShip.UI.Enums;

namespace BattleShip.UI
{
    public class DisplayGrid
    {
        private int _gridSize; //Created with a fixed Board Size of 10x10 - other sizes may require retesting...

        public DisplayGrid(int gridSize)
        {
            if (gridSize >= 5 && gridSize <= 26) _gridSize = gridSize;
            else
            {
                //TODO:unusable gridSize passed, this should return an error somehow.
            }
        }

        public void Grid(Player myPlayer, BoardViewType myBoardType)
        {
            //Create a grid that can be filled with values from a string array (string[gridSize,gridSize])
            //Example 10x10:
            //   01234567890123456789012
            // 0   1 2 3 4 5 6 7 8 9 0 1
            // 1 A · · · · · · · · · · |
            // 2 B · · · · · · · · · · |
            // 3 C · · · · · · · · · · |
            // 4 E · · · · · · · · · · |
            // 5 F · · · · · · · · · · |
            // 6 G · · · · · · · · · · |
            // 7 H · · · · · · · · · · |
            // 8 I · · · · · · · · · · |
            // 9 J · · · · · · · · · · |
            // 0 K · · · · · · · · · · |
            // 1  ˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉˉ

            const string constBottomLine = "ˉ"; //"\u02C9"
            const string constRightLine = "|"; //"\u007c"
            const string constEmptyCell = "·"; //"\u00B7"
            const ConsoleColor constRowHeaderColor = ConsoleColor.Black;
            const ConsoleColor constColHeaderColor = ConsoleColor.Black;
            const ConsoleColor constBottomLineColor = ConsoleColor.Black;
            const ConsoleColor constRightLineColor = ConsoleColor.Black;
            const ConsoleColor constRowHeaderBackColor = ConsoleColor.DarkGray;
            const ConsoleColor constColHeaderBackColor = ConsoleColor.DarkGray;
            const ConsoleColor constBottomLineBackColor = ConsoleColor.DarkGray;
            const ConsoleColor constRightLineBackColor = ConsoleColor.DarkGray;
            const ConsoleColor constEmptyCellColor = ConsoleColor.Cyan;
            const ConsoleColor constShipCell = ConsoleColor.DarkBlue;
            const ConsoleColor constHitCell = ConsoleColor.Red;
            const ConsoleColor constMissedCell = ConsoleColor.Yellow;

            int maxX = _gridSize + 1, maxY = _gridSize * 2 + 2;

            //Instantiate myMatrix depending on the BoardViewType
            char[,] myMatrix = new char[_gridSize,_gridSize];
            if (myBoardType == BoardViewType.ShipPositions) myMatrix = ShipLocationMatrix(myPlayer);
            if (myBoardType == BoardViewType.ShotHistory) myMatrix = CurrentShotStatusMatrix(myPlayer);
            if (myBoardType == BoardViewType.ShipShotStatus) myMatrix = ShipShotStatusMatrix(myPlayer);

            for (int rowX = 0; rowX <= maxX; rowX++)
            {
                for(int colY =0; colY <= maxY; colY++)
                {
                    string strCell = "";
                    if (rowX == 0 || colY == 0)
                    {
                        // Cell 0,0
                        if (rowX == 0 && colY == 0)
                        {
                            strCell = " ";

                            Console.BackgroundColor = constColHeaderBackColor;
                            Console.Write(strCell);
                            Console.ResetColor();
                        } 
                        // Col Numbering
                        else if (rowX == 0)
                        {
                            if (colY / 2 >= 10)
                            {
                                if (colY % 2 == 0 && colY > 0 && colY < maxY) strCell = ((colY / 2) % 10).ToString();
                                else strCell = " ";
                            }
                            else if (colY % 2 == 0 && colY > 0 && colY < maxY) strCell = (colY / 2).ToString();
                            else strCell = " ";
                            Console.ForegroundColor = constColHeaderColor;
                            Console.BackgroundColor = constColHeaderBackColor;
                            Console.Write(strCell);
                            Console.ResetColor();
                        }
                        // Row Numbering
                        else if (rowX == maxX)
                        {
                            strCell = " ";
                            Console.BackgroundColor = constColHeaderBackColor;
                            Console.Write(strCell);
                            Console.ResetColor();
                        } 
                        else if (rowX < maxX)
                        {
                            switch (rowX)
                            {
                                case 1:
                                    strCell = "A";
                                    break;
                                case 2:
                                    strCell = "B";
                                    break;
                                case 3:
                                    strCell = "C";
                                    break;
                                case 4:
                                    strCell = "D";
                                    break;
                                case 5:
                                    strCell = "E";
                                    break;
                                case 6:
                                    strCell = "F";
                                    break;
                                case 7:
                                    strCell = "G";
                                    break;
                                case 8:
                                    strCell = "H";
                                    break;
                                case 9:
                                    strCell = "I";
                                    break;
                                case 10:
                                    strCell = "J";
                                    break;
                                case 11:
                                    strCell = "K";
                                    break;
                                case 12:
                                    strCell = "L";
                                    break;
                                case 13:
                                    strCell = "M";
                                    break;
                                case 14:
                                    strCell = "N";
                                    break;
                                case 15:
                                    strCell = "O";
                                    break;
                                case 16:
                                    strCell = "P";
                                    break;
                                case 17:
                                    strCell = "Q";
                                    break;
                                case 18:
                                    strCell = "R";
                                    break;
                                case 19:
                                    strCell = "S";
                                    break;
                                case 20:
                                    strCell = "T";
                                    break;
                                case 21:
                                    strCell = "U";
                                    break;
                                case 22:
                                    strCell = "V";
                                    break;
                                case 23:
                                    strCell = "W";
                                    break;
                                case 24:
                                    strCell = "X";
                                    break;
                                case 25:
                                    strCell = "Y";
                                    break;
                                case 26:
                                    strCell = "Z";
                                    break;
                                default:
                                    strCell = " ";
                                    break;
                            }
                            Console.ForegroundColor = constRowHeaderColor;
                            Console.BackgroundColor = constRowHeaderBackColor;
                            Console.Write(strCell);
                            Console.ResetColor();
                        }
                    }
                    else if (rowX > 0 && rowX < maxX && colY % 2 != 0 && colY != maxY)
                    {
                        //Padding
                        strCell = " ";
                        Console.Write(strCell);
                        Console.ResetColor();
                    }
                    else if (rowX == maxX)
                    {
                        //BottomLine
                        if (colY == maxY) strCell = " ";
                        else strCell = constBottomLine;
                        Console.ForegroundColor = constBottomLineColor;
                        Console.BackgroundColor = constBottomLineBackColor;
                        Console.Write(strCell);
                        Console.ResetColor();
                    }
                    else if (colY == maxY)
                    {
                        //RightLine
                        strCell = constRightLine;
                        Console.ForegroundColor = constRightLineColor;
                        Console.BackgroundColor = constRightLineBackColor;
                        Console.Write(strCell);
                        Console.ResetColor();
                    }
                    else
                    {
                        //Matrix Element: '\0' if empty
                        if (myMatrix[rowX - 1, colY / 2 - 1] != '\0') strCell = myMatrix[rowX - 1, colY / 2 - 1].ToString();
                        else strCell = constEmptyCell;
                        Console.ForegroundColor = constEmptyCellColor;
                        if (myBoardType == BoardViewType.ShipPositions|| myBoardType == BoardViewType.ShipShotStatus)
                        {
                            switch (strCell)
                            {
                                case constEmptyCell:
                                    //Console.ForegroundColor = constEmptyCellColor;
                                    break;
                                default:
                                    Console.ForegroundColor = constShipCell;
                                    break;
                            }
                        } 
                        if (myBoardType == BoardViewType.ShotHistory || myBoardType == BoardViewType.ShipShotStatus)
                        {
                            switch (strCell)
                            {
                                case "H":
                                    Console.ForegroundColor = constHitCell;
                                    break;
                                case "M":
                                    Console.ForegroundColor = constMissedCell;
                                    break;
                                default:
                                    //Console.ForegroundColor = constEmptyCellColor;
                                    break;
                            }
                        }
                        Console.Write(strCell);
                        Console.ResetColor();
                    }
                }
                Console.ResetColor();
                Console.Write("\n");
            }
            return;
        }

        private char[,] ShipLocationMatrix(Player myPlayer)
        {
            //Loop through grid locations and plot ship locations
            Char[,] retVal = new Char[_gridSize, _gridSize];
            Coordinate xy;

            for (int x = 1; x <= _gridSize; x++)
            {
                for (int y = 1; y <= _gridSize; y++)
                {
                    xy = new Coordinate(x, y);
                    retVal[xy.XCoordinate - 1, xy.YCoordinate - 1] = ParseShipType(BoardCoordinateHasShip(xy, myPlayer));
                }
            }
            return retVal;
        }

        private Char[,] CurrentShotStatusMatrix(Player myPlayer)
        {
            //Looks up shot history using the CheckCoordinate method from the BLL
            Char[,] retVal = new Char[_gridSize, _gridSize];
            retVal = CurrentShotStatusMatrix(myPlayer, retVal);
            return retVal;
        }

        private char[,] CurrentShotStatusMatrix(Player myPlayer, char[,] myMatrix)
        {
            //Looks up shot history using the CheckCoordinate method from the BLL using a pre-existing char[,] matrix
            Coordinate xy;

            for (int x = 1; x <= _gridSize; x++)
            {
                for (int y = 1; y <= _gridSize; y++)
                {
                    xy = new Coordinate(x, y);
                    var resultShotHistory = myPlayer.board.CheckCoordinate(xy);
                    switch (resultShotHistory)
                    {
                        case ShotHistory.Hit:
                            myMatrix[xy.XCoordinate - 1, xy.YCoordinate - 1] = 'H';
                            break;
                        case ShotHistory.Miss:
                            myMatrix[xy.XCoordinate - 1, xy.YCoordinate - 1] = 'M';
                            break;
                        default:
                            //TODO: TEST THIS!!!
                            //myMatrix[xy.XCoordinate - 1, xy.YCoordinate - 1] = '\0';
                            break;
                    }
                }
            }
            return myMatrix;
        }

        private char[,] ShipShotStatusMatrix(Player myPlayer)
        {
            //TODO: TEST THIS!!!
            //Combines the Matrixes from ShipLocationMatrix & CurrentShotStatusMatrix
            Char[,] retVal = new Char[_gridSize, _gridSize];

            retVal = ShipLocationMatrix(myPlayer);
            retVal = CurrentShotStatusMatrix(myPlayer, retVal);

            return retVal;
        }

            private Nullable<ShipType> BoardCoordinateHasShip(Coordinate myCoordinateXY, Player myPlayer)
        {
            //Search Ships.Ship.BoardPositions for a match to myCoordinateXY
            foreach (Ship ship in myPlayer.board.Ships)
            {
                if (ship != null)
                {
                    foreach (Coordinate shipXY in ship.BoardPositions)
                    {
                        if (myCoordinateXY.Equals(shipXY)) return ship.ShipType;
                    }
                }
            }
            return null;
        }

        private Char ParseShipType(Nullable<ShipType> myShipType)
        {
            Char retVal;

            switch (myShipType)
            {
                case ShipType.Battleship:
                    retVal = 'B';
                    break;
                case ShipType.Carrier:
                    retVal = 'C';
                    break;
                case ShipType.Cruiser:
                    retVal = 'R';
                    break;
                case ShipType.Destroyer:
                    retVal = 'D';
                    break;
                case ShipType.Submarine:
                    retVal = 'S';
                    break;
                default:
                    retVal = '\0'; //Char null value
                    break;
            }
            return retVal;
        }
    }
}
