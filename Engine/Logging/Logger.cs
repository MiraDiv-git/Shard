namespace Engine;

public class Logger
{
    public static void Info(string classname, string message)
        => Write(ConsoleColor.Cyan, "Engine Info", classname, message);

    public static void Error(string classname, string message)
        => Write(ConsoleColor.Red, "Engine Error", classname, message);

    public static void Warn(string classname, string message)
        => Write(ConsoleColor.Yellow, "Engine Warning", classname, message);

    public static void Quote(string message)
        => Write(ConsoleColor.DarkGray, null, null, message);

    public static void Exception(Exception ex)
    {
        var prev = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ Engine Error ] {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
        finally
        {
            Console.ForegroundColor = prev;
        }
    }

    private static void Write(ConsoleColor color, string? level, string? classname, string message)
    {
        var prev = Console.ForegroundColor;
        try
        {
            Console.ForegroundColor = color;
            if (level is null)
                Console.WriteLine(message);
            else
                Console.WriteLine($"[ {level} ] {classname}: {message}");
        }
        finally
        {
            Console.ForegroundColor = prev;
        }
    }
}