using System.Reflection;

namespace Engine;

public static class ScriptLoader
{
    public static int LoadScripts(Assembly assembly, ScriptHost host)
    {
        var count = 0;

        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || !typeof(Script).IsAssignableFrom(type))
                continue;

            host.Add((Script)Activator.CreateInstance(type)!);
            count++;
        }

        return count;
    }
}