using Engine;

namespace Game;

public class HelloScript : Script
{
    public override void Ready()
    {
        Logger.Info("HelloScript", "Hello from Game.");
    }
}