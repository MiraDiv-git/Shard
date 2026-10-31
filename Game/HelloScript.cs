using Engine;

namespace Game;

public class HelloScript : Script
{
    public override void Ready()
    {
        PrintGameAuthor();
    }

    public void PrintGameAuthor()
    {
        string gamename = Config.Get("./Game/config/meta.cfg", "GAME_NAME");
        string gamecopy = Config.Get("./Game/config/meta.cfg", "GAME_COPYRIGHT");
        
        Console.WriteLine($"Game: {gamename}\n{gamecopy}");
    }
}