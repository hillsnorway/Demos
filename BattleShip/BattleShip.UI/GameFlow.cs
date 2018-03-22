using System;
using BattleShip.BLL.GameLogic;
using BattleShip.BLL.Requests;
using BattleShip.BLL.Responses;
using BattleShip.BLL.Ships;
using BattleShip.UI.Enums;

namespace BattleShip.UI
{

    public class GameFlow
    {
        private static Random _rnd;
        private int _gridSize;
        private DisplayGrid _grid;
        public int CurrentPlayerNr { get; private set; }
        public Player Opponent { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public Player P1 { get; private set; }
        public Player P2 { get; private set; }

        public GameFlow()
        {
            _gridSize = 10; //Default size is 10x10
            _grid = new DisplayGrid(_gridSize);
            //Board Sizes between 5-10 should be playable.
            //Board Sizes between 10-26 probably require extensive retesting...
            _rnd = new Random();
            P1 = new Player();
            P2 = new Player();
        }

        public void MainMenu()
        {
            string strMenu, strBattleShip;
            string strError = "";
            ConsoleKeyInfo menuInput;
            bool blnErrorMessage = false;

            strBattleShip = (
            @"                                  |__" + "\n" +
            @"                                  |\/" + "\n" +
            @"                                  ---" + "\n" +
            @"                                  / | [" + "\n" +
            @"                              !   | |||" + "\n" +
            @"                            _/| _ /| -++'" + "\n" +
            @"                        +  +--|    |--|--|_ |-" + "\n" +
            @"                     { /|__|  |/\__|  |--- |||__/" + "\n" +
            @"                    +---------------___[}-_===_.'____                  /\" + "\n" +
            @"                ____`-' ||___-{]_| _[}-  |     |_[___\\==--            \/   _" + "\n" +
            @" __..._____-- ==/ ___]_ | __ | _____________________________[___\\== --____, ------' .7" + "\n" +
            @"   |                                                                         BB - 35 /" + "\n" +
            @"   \________________________________________________________________________________| " + "\n" +
            @"============================================================================================" + "\n");
            Console.WriteLine(strBattleShip);

            do
            {
                strMenu =(
                "Welcome to: \n\n" +
                " B A T T L E S H I P !!!\n\n" +
                ":-) have a lot of fun :-)\n\n" +
                "Menu: \n" +
                //$"0: Set Board Size -- CurrentGridSize: ({_gridSize}x{_gridSize}) !!!ONLY SQUARE GRIDSIZES (5x5)-(10x10) WORK!!!\n" +
                $"1: Setup Player A -- PlayerName: {P1.Name}  PlayerStatus: {P1.PlayerStatus}   Wins: {P1.Wins}\n" +
                $"2: Setup Player B -- PlayerName: {P2.Name}  PlayerStatus: {P2.PlayerStatus}   Wins: {P2.Wins}\n" +
                $"3: Play Game ");
                if (P1.PlayerStatus != PlayerStatus.Initialized || P2.PlayerStatus != PlayerStatus.Initialized) strMenu += "------- Both players must be (re)initialized to play a game!";
                else strMenu += "------- Both players are initialized - PRESS 3 TO PLAY A GAME!!!";
                strMenu += (
                $"\nPressing (Q/q) quits the game at any time...\n" +
                $"\n" +
                $"Enter the number for the menu item you want:");

                Console.Clear();
                Console.WriteLine(strBattleShip);
                if (blnErrorMessage)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(strError);
                    Console.ResetColor(); 
                    blnErrorMessage = false;
                }
                Console.WriteLine(strMenu);
                menuInput = Console.ReadKey();
                switch (menuInput.Key)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.NumPad0:
                        //TODO: Implement setting the gridsize.
                        break;
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        SetupPlayer(P1, "");
                        //Stub Setup P1 -- used for testing UI...  
                        //SetupPlayer(p1, "P1 Joe Cool"); //Remember to change SetupPlayer() to use StubSetupPlayerBoard()
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        SetupPlayer(P2, "");
                        //Stub Setup P2 -- used for testing UI...
                        //SetupPlayer(p2, "P2 Dude Awewome"); //Remember to change SetupPlayer() to use StubSetupPlayerBoard()
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        RunGame();
                        break;
                    default:
                        blnErrorMessage = true;
                        strError = "You can only enter the listed numbers to pick menu items! \n";
                        break;
                }
            } while (menuInput.Key != ConsoleKey.Q);

        }

        public ReturnType RunGame()
        {
            ConsoleKeyInfo varInput;

            //Check If Players are Initialized so Game can start
            //if(!((P1.PlayerStatus == PlayerStatus.Initialized  && P2.PlayerStatus == PlayerStatus.Initialized) || (P1.PlayerStatus == PlayerStatus.ActiveGame && P2.PlayerStatus == PlayerStatus.ActiveGame)))
            //Can not allow players with ActiveGames to restart until method to persit CurrentPlayer is implemented!!
            if (!(P1.PlayerStatus == PlayerStatus.Initialized && P2.PlayerStatus == PlayerStatus.Initialized))
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You can not play until both players have been initialized!!!");
                Console.ResetColor();
                Console.ReadKey();
                return ReturnType.Error;
            }

            //initialize currentPlayer property at random
            CurrentPlayerNr = _rnd.Next(1, 3);
            if (CurrentPlayerNr == 1)
            {
                CurrentPlayer = P1;
                Opponent = P2;
            }
            else
            {
                {
                    CurrentPlayer = P2;
                    Opponent = P1;
                }
            }

            P1.SetPlayerStatus (PlayerStatus.ActiveGame);
            P2.SetPlayerStatus (PlayerStatus.ActiveGame);

            Console.Clear();
            Console.WriteLine($"Player {CurrentPlayer.Name} will go first! \n\nPress any key to continue, (Q/q) to quit...");
            varInput = Console.ReadKey();
            if (varInput.Key == ConsoleKey.Q) return ReturnType.Quit; //User quit

            do
            {
                Coordinate shotXY;

                Console.Clear();
                //Get Valid Coordinate (includes checking if Coordinate fired at already)
                shotXY = GetShotCoordinate();
                if (shotXY == null)
                {
                    //Need to implement a method for saving the current player state (and side-stepping the random player start for active games is allowed)
                    //Until this is implemented, set the Player Status to PlayerStatus.FinishedGame;
                    P1.SetPlayerStatus(PlayerStatus.FinishedGame);
                    P2.SetPlayerStatus(PlayerStatus.FinishedGame);
                    return ReturnType.Quit; //User quit
                } 
                //Fire Shot
                var shotResult = Opponent.board.FireShot(shotXY);
                Console.Clear();
                //Display Shot Response
                switch (shotResult.ShotStatus)
                {
                    case ShotStatus.Victory:
                        P1.SetPlayerStatus (PlayerStatus.FinishedGame);
                        P2.SetPlayerStatus (PlayerStatus.FinishedGame);
                        CurrentPlayer.SetWins (CurrentPlayer.Wins + 1);
                        Console.WriteLine($"HIT!!! Congratulations, you WIN!  \n\nPress any key return to the main menu...");
                        varInput = Console.ReadKey();
                        break;
                    case ShotStatus.HitAndSunk:
                        Console.WriteLine($"HIT!!! Congratulations, you sank a {shotResult.ShipImpacted}! \n\nPress any key to continue, (Q/q) to quit...");
                        varInput = Console.ReadKey();
                        break;
                    case ShotStatus.Hit:
                        Console.WriteLine("HIT!!! \n\nPress any key to continue, (Q/q) to quit...");
                        varInput = Console.ReadKey();
                        break;
                    case ShotStatus.Miss:
                        Console.WriteLine("Miss... \n\nPress any key to continue,  (Q/q) to quit...");
                        varInput = Console.ReadKey();
                        break;
                    case ShotStatus.Invalid:
                    case ShotStatus.Duplicate:
                    default:
                        break;
                }

                if (shotResult.ShotStatus != ShotStatus.Victory) 
                {
                    SwapCurrentPlayer();
                }
                else if (P1.PlayerStatus == PlayerStatus.FinishedGame && P2.PlayerStatus == PlayerStatus.FinishedGame && varInput.Key.Equals(ConsoleKey.Y))
                {
                    //Players want to play again -- Back to menu so they can set up their boards again.
                    return ReturnType.Success;
                }
            } while (P1.PlayerStatus == PlayerStatus.ActiveGame && P2.PlayerStatus == PlayerStatus.ActiveGame && !varInput.Key.Equals (ConsoleKey.Q));
            if ((P1.PlayerStatus == PlayerStatus.ActiveGame && P2.PlayerStatus == PlayerStatus.ActiveGame && varInput.Key.Equals(ConsoleKey.Q)))
            {
                //Need to implement a method for saving the current player state (and side-stepping the random player start for active games is allowed)
                //Until this is implemented, set the Player Status to PlayerStatus.FinishedGame;
                P1.SetPlayerStatus (PlayerStatus.FinishedGame);
                P2.SetPlayerStatus(PlayerStatus.FinishedGame);
                return ReturnType.Quit;
            }
            return ReturnType.Success;
        }

        private ReturnType SetupPlayer(Player myPlayer, string myPlayerName)
        {
            //Setup a SPECIFIC PLAYER - Do not use currentPlayer object here!!!

            //Does the player have a name from before?
            if (myPlayer.Name.Length == 0)
            {
                bool skipError = true;
                while (myPlayerName.Length == 0)
                {
                    //Prompt user for name
                    Console.Clear();
                    if (skipError != true)
                    {
                        Console.WriteLine("Invalid Entry, try again... \n\n");
                    }
                    skipError = false;
                    Console.WriteLine("Enter a name for this player: \n\nPress ENTER to continue, (Q/q) to quit...");
                    myPlayerName = Console.ReadLine();
                    if (myPlayerName.ToUpper() == "Q") return ReturnType.Quit;
                }
                myPlayer.SetName(myPlayerName);
            }

            //ReInit
            if (myPlayer.PlayerStatus == PlayerStatus.FinishedGame || myPlayer.PlayerStatus == PlayerStatus.Initialized)
            {
                //No current need to do any other kind of other cleanup (SetupPlayerBoard method initializes a new board each time)
            }
            myPlayer.SetPlayerStatus (PlayerStatus.Uninitialized);  //Going to reset the player's board, uninitialized until SetupPlayerBoard returns true
            //StubSetupPlayerBoard(myPlayer) can alternately be used for UI testing -- non-interactive board setup
            if (SetupPlayerBoard(myPlayer) == ReturnType.Success) myPlayer.SetPlayerStatus (PlayerStatus.Initialized);
            else return ReturnType.Error;

            Console.Clear();
            Console.WriteLine($"Great!  Your board is now completely setup, {myPlayer.Name}! \nIt is setup as shown below:");
            _grid.Grid(myPlayer, BoardViewType.ShipPositions);
            if (P1.PlayerStatus == PlayerStatus.Uninitialized || P2.PlayerStatus == PlayerStatus.Uninitialized)
            {
                Console.WriteLine("\nSwitch Seats with the other player, and let them set up their board. :-)");
            }
            else if (P1.PlayerStatus == PlayerStatus.Initialized && P2.PlayerStatus == PlayerStatus.Initialized)
            {
                Console.WriteLine("\nThe game is now ready to play. :-)");
            }
            else
            {
                //Something unexpected is happening to PlayerStatus - you need to debug and refactor...
            }
            Console.WriteLine("Press any key to continue...");
            if (Console.ReadKey().Key == ConsoleKey.Q) return ReturnType.Quit;

            return ReturnType.Success;
        }

        private ReturnType SetupPlayerBoard(Player myPlayer)
        {
            myPlayer.SetBoard(new Board());
            foreach (ShipType myShipType in Enum.GetValues(typeof(ShipType)))
            {
                Nullable<ShipPlacement> myShipPlacement = null;
                do
                {
                    Console.Clear();
                    //Show Error message if ship placement not successful
                    if (myShipPlacement != null)
                    {
                        switch (myShipPlacement.Value)
                        {
                            case ShipPlacement.Overlap:
                                Console.WriteLine($"Could not place a ship with parameters given. \nIt overlapped another ship. \n\nPress any key to continue, (Q/q) to quit...");
                                if (Console.ReadKey().Key == ConsoleKey.Q) return ReturnType.Quit; //User pressed Q.
                                break;
                            case ShipPlacement.NotEnoughSpace:
                                Console.WriteLine($"Could not place a ship with parameters given. \nThere was not enough space on the board. \n\nPress any key to continue, (Q/q) to quit...");
                                if (Console.ReadKey().Key == ConsoleKey.Q) return ReturnType.Quit; //User pressed Q.
                                break;
                            default:
                                break;
                        }
                    }
                    PlaceShipRequest myRequest = null;
                    //Console.Clear();
                    myRequest = GetShipRequest(myShipType, myPlayer);
                    if (myRequest == null) return ReturnType.Error;
                    myShipPlacement = myPlayer.board.PlaceShip(myRequest);
                } while (myShipPlacement != ShipPlacement.Ok);
            }
            return ReturnType.Success;
        }

        private PlaceShipRequest GetShipRequest(ShipType myShipType, Player myPlayer)
        {

            Coordinate shipXY = null;
            Nullable<ShipDirection> shipDirection = null;
            Nullable<CoordinateInput> evalCoordinate = null;
            ConsoleKeyInfo consoleInput;
            string strX = "", strY = "", strDirection = "";
            DisplayGrid myGrid = new DisplayGrid(_gridSize); //Used to display Ship locations.

            Ship myShip = ShipCreator.CreateShip(myShipType); //Used to display ShipName & BoardPositions.Length properties.

            PlaceShipRequest retVal = new PlaceShipRequest();
            //Set the ShipType for the return
            retVal.ShipType = myShipType;

            //Get X Coordinate
            do
            {
                Console.Clear();
                if ((evalCoordinate == CoordinateInput.InvalidXCoordinate || evalCoordinate == CoordinateInput.NoValidCoordinates) && evalCoordinate != null)
                {
                    Console.WriteLine("Invalid X Coordinate, try again!!!");
                    Console.WriteLine();
                }
                Console.WriteLine($"{myPlayer.Name}, your current board setup:");
                myGrid.Grid(myPlayer, BoardViewType.ShipPositions);
                
                Console.WriteLine($"Please enter the X coordinate for your {myShip.ShipName}. \nA {myShip.ShipName} takes up {myShip.BoardPositions.Length} spots on the board. \n(Examples:A <A>, b <B>) \n\nQuit anytime by pressing (Q/q)...");
                consoleInput = Console.ReadKey();
                if (consoleInput.Key.Equals(ConsoleKey.Q)) return null; //User entered Q
                else
                {
                    strX = consoleInput.KeyChar.ToString().ToUpper();
                    EvaluateCoordinates(strX, null, out CoordinateInput response);
                    evalCoordinate = response;
                }
            } while (evalCoordinate != CoordinateInput.ValidCoordinate && !consoleInput.Key.Equals(ConsoleKey.Q));

            //Get Y Coordinate
            do
            {
                Console.Clear();
                if ((evalCoordinate == CoordinateInput.InvalidYCoordinate || evalCoordinate == CoordinateInput.NoValidCoordinates) && evalCoordinate != null)
                {
                    Console.WriteLine("Invalid Y Coordinate, try again!!!");
                    Console.WriteLine();
                }
                Console.WriteLine($"{myPlayer.Name}, your current board setup:");
                myGrid.Grid(myPlayer, BoardViewType.ShipPositions);
                Console.WriteLine($"Please enter the Y coordinate for your {myShip.ShipName}. \nA {myShip.ShipName} takes up {myShip.BoardPositions.Length} spots on the board. \n(Examples:1 <1>, 0 <10>) \n\nQuit anytime by pressing (Q/q)...");
                consoleInput = Console.ReadKey();
                if (consoleInput.Key.Equals(ConsoleKey.Q)) return null; //User entered Q
                else
                {
                    strY = consoleInput.KeyChar.ToString().ToUpper();
                    shipXY = EvaluateCoordinates(strX, strY, out CoordinateInput response);
                    evalCoordinate = response;
                }
            } while (evalCoordinate != CoordinateInput.ValidCoordinate && !consoleInput.Key.Equals(ConsoleKey.Q));

            //Now have a valid coordinate: shipXY
            retVal.Coordinate = shipXY;

            //Get the direction:
            do
            {
                Console.Clear();
                if (strDirection.Length > 0 && shipDirection == null)
                {
                    Console.WriteLine("Invalid direction, try again!!!");
                    Console.WriteLine();
                }
                Console.WriteLine($"{myPlayer.Name}, your current board setup:");
                myGrid.Grid(myPlayer, BoardViewType.ShipPositions);
                Console.WriteLine($"Please enter a direction to use in placing your {myShip.ShipName}. \nDirection is the direction from ({strX},{strY}) the ship will extend. \nA {myShip.ShipName} takes up {myShip.BoardPositions.Length} spots on the board. \nValid directions are: {ShipDirection.Up}, {ShipDirection.Right}, {ShipDirection.Down} & {ShipDirection.Left} (type and press ENTER) \n\nQuit anytime by pressing (Q/q)...");
                strDirection = Console.ReadLine();
                shipDirection = EvaluateDirection(strDirection);
                if (shipDirection != null)
                {
                    //Valid shipDirection
                    retVal.Direction = shipDirection.Value;
                }

            } while (shipXY == null || shipDirection == null);

            return retVal;
        }

        private Coordinate GetShotCoordinate()
        {
            //Designed to use the CurrentPlayer/Opponent - no need to pass in a Player object!!!

            Coordinate shotXY = null;  //Returns Null Coordinate if the users Quits before valid Coordinate is created.
            Nullable<CoordinateInput> evalCoordinate = null;
            ConsoleKeyInfo consoleInput;
            string strX = "", strY = "";
            DisplayGrid myGrid = new DisplayGrid(_gridSize);

            //Get X Coordinate
            do
            {
                Console.Clear();
                if (evalCoordinate == CoordinateInput.InvalidXCoordinate && evalCoordinate != null)
                {
                    Console.WriteLine("Invalid X Coordinate, try again!!!");
                    Console.WriteLine();
                }
                Console.WriteLine($"{CurrentPlayer.Name}, your current shot history:");
                myGrid.Grid(Opponent, BoardViewType.ShotHistory);
                Console.WriteLine($"{CurrentPlayer.Name}, your opponent's shot history:");
                myGrid.Grid(CurrentPlayer, BoardViewType.ShipShotStatus);
                Console.WriteLine("Please enter the X coordinate for your next shot. \n(Examples:A <A>, b <B>) \n\nQuit anytime by pressing (Q/q)...");
                consoleInput = Console.ReadKey();
                if (consoleInput.Key.Equals(ConsoleKey.Q)) break;
                else
                {
                    strX = consoleInput.KeyChar.ToString().ToUpper();
                    EvaluateCoordinates(strX, "", out CoordinateInput response);
                    evalCoordinate = response;
                }
            } while (evalCoordinate != CoordinateInput.ValidCoordinate && !consoleInput.Key.Equals(ConsoleKey.Q));

            if (consoleInput.Key.Equals(ConsoleKey.Q))
            {
                return null;  //User pressed Q
            }

            //Get Y Coordinate
            do
            {
                Console.Clear();
                if (evalCoordinate == CoordinateInput.InvalidYCoordinate && evalCoordinate != null)
                {
                    Console.WriteLine("Invalid Y Coordinate, try again!!!");
                    Console.WriteLine();
                }
                Console.WriteLine($"{CurrentPlayer.Name}, your current shot history:");
                myGrid.Grid(Opponent, BoardViewType.ShotHistory);
                Console.WriteLine($"{CurrentPlayer.Name}, your opponent's shot history:");
                myGrid.Grid(CurrentPlayer, BoardViewType.ShipShotStatus);
                Console.WriteLine("Please enter the Y coordinate for your next shot. \n(Examples:1 <1>, 0 <10>) \n\nQuit anytime by pressing (Q/q)...");
                consoleInput = Console.ReadKey();
                if (consoleInput.Key.Equals(ConsoleKey.Q)) break;
                else
                {
                    strY = consoleInput.KeyChar.ToString().ToUpper();
                    shotXY = EvaluateCoordinates(strX, strY, out CoordinateInput response);
                    evalCoordinate = response;
                }
            } while (evalCoordinate != CoordinateInput.ValidCoordinate && !consoleInput.Key.Equals(ConsoleKey.Q));

            if (consoleInput.Key.Equals(ConsoleKey.Q))
            {
                return null;  //User pressed Q
            }

            //shotXY now has valid X & Y coordinates!
            //Ask user if they want to fire the shot to these coordinates
            Console.Clear();
            Console.WriteLine($"{CurrentPlayer.Name}, your current shot history:");
            myGrid.Grid(Opponent, BoardViewType.ShotHistory);
            Console.WriteLine($"{CurrentPlayer.Name}, your opponent's shot history:");
            myGrid.Grid(CurrentPlayer, BoardViewType.ShipShotStatus);
            Console.WriteLine($"Great! Press any key to fire a shot at coordinate ({strX},{strY}). \nIf you want to pick a different coordinate, press (N/n) \n\nQuit anytime by pressing (Q/q)...");
            consoleInput = Console.ReadKey();
            //Check if Coordinate used before (and that neither Q or N are pressed)
            if (Opponent.board.CheckCoordinate(shotXY) != ShotHistory.Unknown && !consoleInput.Key.Equals(ConsoleKey.Q) && !consoleInput.Key.Equals(ConsoleKey.N))
            {
                //Invalid shot coordinate, recurse to get valid shot!
                Console.Clear();
                Console.WriteLine($"{CurrentPlayer.Name}, your current shot history:");
                myGrid.Grid(Opponent, BoardViewType.ShotHistory);
                Console.WriteLine($"{CurrentPlayer.Name}, your opponent's shot history:");
                myGrid.Grid(CurrentPlayer, BoardViewType.ShipShotStatus);
                Console.WriteLine($"You have previously fired a shot at coordinate ({strX},{strY}). \nPress any key to pick a different coordinate. \n\nQuit anytime by pressing (Q/q)...");
                consoleInput = Console.ReadKey();
                if (!consoleInput.Key.Equals(ConsoleKey.Q))
                {
                    //Recursive call -- handle correctly...
                    shotXY = GetShotCoordinate();
                    if (shotXY != null)
                    {
                        return shotXY;
                    }
                }
            }
            else if (consoleInput.Key.Equals(ConsoleKey.N))
            {
                //User wants to enter a different coordinate, recurse to get a valid shot!
                Console.Clear();
                Console.WriteLine($"{CurrentPlayer.Name}, your current shot history:");
                myGrid.Grid(Opponent, BoardViewType.ShotHistory);
                Console.WriteLine($"{CurrentPlayer.Name}, your opponent's shot history:");
                myGrid.Grid(CurrentPlayer, BoardViewType.ShipShotStatus);
                Console.WriteLine($"You choose not to fire at coordinate ({strX},{strY}). \nPress any key to pick a different coordinate. \n\nQuit anytime by pressing (Q/q)...");
                consoleInput = Console.ReadKey();
                if (!consoleInput.Key.Equals(ConsoleKey.Q))
                {
                    //Recursive call -- handle correctly...
                    shotXY = GetShotCoordinate();
                    if (shotXY != null)
                    {
                        return shotXY;
                    }
                }
                else return null; //User pressed Q
            }
            return shotXY;
        }

        public Coordinate EvaluateCoordinates(string myX, string myY, out CoordinateInput response) //public for test cases in Battleship.Tests
        {
            int intX = 0, intY = 0;

            Coordinate retVal = null;
            response = CoordinateInput.NoValidCoordinates;

            if ((myX == "" || myX == null) && (myY == "" || myY == null)) return retVal;
            if (myX != "" && myX != null)
            {
                myX = myX.ToUpper();
                switch (myX)
                {//TODO: This switch needs additional cases if the function  is to be used for gridSizes other than 10
                    case "A":
                        response = CoordinateInput.ValidCoordinate;
                        intX = 1;
                        break;
                    case "B":
                        response = CoordinateInput.ValidCoordinate;
                        intX = 2;
                        break;
                    case "C":
                        response = CoordinateInput.ValidCoordinate;
                        intX = 3;
                        break;
                    case "D":
                        response = CoordinateInput.ValidCoordinate;
                        intX = 4;
                        break;
                    case "E":
                        if (_gridSize >= 5)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intX = 5;
                        }
                        else response = CoordinateInput.InvalidXCoordinate;
                        break;
                    case "F":
                        if (_gridSize >= 6)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intX = 6;
                        }
                        else response = CoordinateInput.InvalidXCoordinate;
                        break;
                    case "G":
                        if (_gridSize >= 7)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intX = 7;
                        }
                        else response = CoordinateInput.InvalidXCoordinate;
                        break;
                    case "H":
                        if (_gridSize >= 8)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intX = 8;
                        }
                        else response = CoordinateInput.InvalidXCoordinate;
                        break;
                    case "I":
                        if (_gridSize >= 9)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intX = 9;
                        }
                        else response = CoordinateInput.InvalidXCoordinate;
                        break;
                    case "J":
                        if (_gridSize >= 10)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intX = 10;
                        }
                        else response = CoordinateInput.InvalidXCoordinate;
                        break;
                    default:
                        response = CoordinateInput.InvalidXCoordinate;
                        break;
                }
            }
            if (myY != "" && myY != null && response != CoordinateInput.InvalidXCoordinate)
            {
                myY = myY.ToUpper();
                switch (myY)
                {//TODO: This switch needs additional cases if the function  is to be used for gridSizes other than 10
                    case "1":
                        response = CoordinateInput.ValidCoordinate;
                        intY = 1;
                        break;
                    case "2":
                        response = CoordinateInput.ValidCoordinate;
                        intY = 2;
                        break;
                    case "3":
                        response = CoordinateInput.ValidCoordinate;
                        intY = 3;
                        break;
                    case "4":
                        response = CoordinateInput.ValidCoordinate;
                        intY = 4;
                        break;
                    case "5":
                        if (_gridSize >= 5)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intY = 5;
                        }
                        else response = CoordinateInput.InvalidYCoordinate;
                        break;
                    case "6":
                        if (_gridSize >= 6)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intY = 6;
                        }
                        else response = CoordinateInput.InvalidYCoordinate;
                        break;
                    case "7":
                        if (_gridSize >= 7)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intY = 7;
                        }
                        else response = CoordinateInput.InvalidYCoordinate;
                        break;
                    case "8":
                        if (_gridSize >= 8)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intY = 8;
                        }
                        else response = CoordinateInput.InvalidYCoordinate;
                        break;
                    case "9":
                        if (_gridSize >= 9)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intY = 9;
                        }
                        else response = CoordinateInput.InvalidYCoordinate;
                        break;
                    case "0":
                        if (_gridSize >= 10)
                        {
                            response = CoordinateInput.ValidCoordinate;
                            intY = 10;
                        }
                        else response = CoordinateInput.InvalidYCoordinate;
                        break;
                    default:
                        response = CoordinateInput.InvalidYCoordinate;
                        break;
                }
            }
            if (intX > 0 && intY > 0) retVal = new Coordinate(intX, intY);
            return retVal;
        }

        public Nullable<ShipDirection> EvaluateDirection(string myShipDirection) //public for test cases in Battleship.Tests
        {
            Nullable<ShipDirection> retVal = null;
            myShipDirection = myShipDirection.ToUpper();
            if (myShipDirection.Length > 1)
            {
                //To get the Enum Parse to work, the evaluated text needs to be IDENTICAL to the enum label text (f.ex. Up, Right, Down, Left)
                myShipDirection = myShipDirection.Substring(0, 1) + myShipDirection.Substring(1, myShipDirection.Length - 1).ToLower();
            }
            if (myShipDirection == "Q") return null; //User entered Q
            if (Enum.TryParse(myShipDirection, out ShipDirection responseShipDirection))
            {
                switch (responseShipDirection)
                {
                    case ShipDirection.Up:
                        retVal = ShipDirection.Up;
                        break;
                    case ShipDirection.Right:
                        retVal = ShipDirection.Right;
                        break;
                    case ShipDirection.Down:
                        retVal = ShipDirection.Down;
                        break;
                    case ShipDirection.Left:
                        retVal = ShipDirection.Left;
                        break;
                    default:
                        break;
                }
            }
            return retVal;
        }

        private void SwapCurrentPlayer()
        {
            //Swap Current Player
            if (CurrentPlayerNr == 1)
            {
                CurrentPlayerNr = 2;
                CurrentPlayer = P2;
                Opponent = P1;
            }
            else
            {
                CurrentPlayerNr = 1;
                CurrentPlayer = P1;
                Opponent = P2;
            }
        }

        //private ReturnType StubSetupPlayerBoard(Player myPlayer)
        //{
        //    myPlayer.SetBoard(new Board());
        //    StubPlaceDestroyer(myPlayer);
        //    StubPlaceCruiser(myPlayer);
        //    StubPlaceSubmarine(myPlayer);
        //    StubPlaceBattleship(myPlayer);
        //    StubPlaceCarrier(myPlayer);
        //    return ReturnType.Success;
        //}

        ///// <summary>
        ///// StubPlaceXXXXX methods set up a board as follows:
        ///// Destroyer: (1,8) (2,8)
        ///// Cruiser: (3,1) (3,2) (3,3)
        ///// Sub: (1,5) (2,5) (3,5)
        ///// Battleship: (10,6) (10,7) (10,8) (10, 9)
        ///// Carrier: (4,4) (5,4) (6,4) (7,4) (8,4)
        ///// 
        /////    1 2 3 4 5 6 7 8 9 10
        /////  A C C C C C 
        /////  B B B B B
        /////  C S S S 
        /////  D R R R 
        /////  E D D 
        /////  F
        /////  G
        /////  H
        /////  I
        /////  J
        /////  </summary>
        ///// <returns>A board that is ready to play</returns>
        ///// 
        //private void StubPlaceCarrier(Player myPlayer)
        //{
        //    var request = new PlaceShipRequest()
        //    {
        //        Coordinate = new Coordinate(1, 1),
        //        Direction = ShipDirection.Right,
        //        ShipType = ShipType.Carrier
        //    };

        //    myPlayer.board.PlaceShip(request);
        //}

        //private void StubPlaceBattleship(Player myPlayer)
        //{
        //    var request = new PlaceShipRequest()
        //    {
        //        Coordinate = new Coordinate(2, 1),
        //        Direction = ShipDirection.Right,
        //        ShipType = ShipType.Battleship
        //    };

        //    myPlayer.board.PlaceShip(request);
        //}

        //private void StubPlaceSubmarine(Player myPlayer)
        //{
        //    var request = new PlaceShipRequest()
        //    {
        //        Coordinate = new Coordinate(3, 1),
        //        Direction = ShipDirection.Right,
        //        ShipType = ShipType.Submarine
        //    };

        //    myPlayer.board.PlaceShip(request);
        //}

        //private void StubPlaceCruiser(Player myPlayer)
        //{
        //    var request = new PlaceShipRequest()
        //    {
        //        Coordinate = new Coordinate(4, 1),
        //        Direction = ShipDirection.Right,
        //        ShipType = ShipType.Cruiser
        //    };

        //    myPlayer.board.PlaceShip(request);
        //}

        //private void StubPlaceDestroyer(Player myPlayer)
        //{
        //    var request = new PlaceShipRequest()
        //    {
        //        Coordinate = new Coordinate(5, 1),
        //        Direction = ShipDirection.Right,
        //        ShipType = ShipType.Destroyer
        //    };

        //    myPlayer.board.PlaceShip(request);
        //}
    }
}
