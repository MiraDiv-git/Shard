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
        string gamename = Config.Get("meta.cfg", "GAME_NAME");
        string gamecopy = Config.Get("meta.cfg", "GAME_COPYRIGHT");
        
        Logger.Quote($"Game: {gamename}\n{gamecopy}\n");
    }
}