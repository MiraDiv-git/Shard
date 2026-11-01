using System.Reflection;
using Engine;

try 
{
    var engine = new EngineRuntime();
    ScriptLoader.LoadScripts(Assembly.Load("Game"), engine.Scripts);
    engine.Run();
    return 0;
}
catch (Exception ex)
{
    Logger.Exception(ex);
        return 1;
}