using System.Reflection;
using Engine;

class Program
{
    static string engine_version = "0.1.0";
    static int Main()
    {
        PrintStartMessage();

        try {
            int width = Config.GetInt("window.cfg", "WINDOW_WIDTH");
            int height = Config.GetInt("window.cfg", "WINDOW_HEIGHT");
            string title = Config.GetBool("window.cfg", "GAME_NAME_AS_WINDOW_TITLE")
                ? Config.GetString("meta.cfg", "GAME_NAME")
                : Config.GetString("window.cfg", "CUSTOM_WINDOW_TITLE");


            Logger.Info("Runtime", "Starting engine...");
            var engine = new EngineRuntime(width, height, title);
            ScriptLoader.LoadScripts(Assembly.Load("Game"), engine.Scripts);
            engine.Run();
            return 0;
        } catch (Exception ex) {
            Logger.Exception(ex);
            return 1;
        }

        void PrintStartMessage()
        {
            Console.WriteLine();
            Logger.Quote($"Shard Engine v{engine_version}");
            Logger.Quote("(c) 2026 MiraDiv. All rights reserved.");
            Console.WriteLine();

            string gamename = Config.GetString("meta.cfg", "GAME_NAME");
            string gamecopy = Config.GetString("meta.cfg", "GAME_COPYRIGHT");
            Logger.Quote($"Game: {gamename}\n{gamecopy}");
            Console.WriteLine();
        }
    }
}