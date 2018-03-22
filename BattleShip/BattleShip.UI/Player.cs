using BattleShip.BLL.GameLogic;
using BattleShip.UI.Enums;

namespace BattleShip.UI
{
    public class Player
    {
        public string Name { get; private set; }
        public PlayerStatus PlayerStatus { get; private set; }
        public int Wins { get; private set; }
        public Board board { get; private set; }

        public Player()
        {
            Name = "";
            PlayerStatus = PlayerStatus.Uninitialized;
            Wins = 0;
            board = new Board();
        }

        public void SetName(string myName)
        {
            Name = myName;
        }

        public void SetPlayerStatus(PlayerStatus myPlayerStatus)
        {
            PlayerStatus = myPlayerStatus;
        }

        public void SetWins(int myWins)
        {
            Wins = myWins;
        }

        public void SetBoard (Board myBoard)
        {
            board = myBoard;
        }
    }
}
