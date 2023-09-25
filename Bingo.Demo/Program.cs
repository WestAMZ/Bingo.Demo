using Bingo.Demo;

public class Program
{
    private static void Main(string[] args)
    {
        var gameInstance = new BingoGame(6);
        gameInstance.PrintBoard();
        gameInstance.StartGame();
        Console.ReadKey();
    }

}