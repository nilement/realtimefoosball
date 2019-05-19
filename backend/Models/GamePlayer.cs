namespace ToughBattle.Models
{
    public class GamePlayer{
        public int PlayerId{get;set;}
        public int GameId{get;set;}
        public TeamColour Colour{get;set;}
        }

    public enum TeamColour{
        Red = 0,
        Blue = 1
    }
}