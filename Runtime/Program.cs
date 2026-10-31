using System.Reflection;
using Engine;
using Runtime;

var engine = new EngineRuntime(new DebugRenderer());
ScriptLoader.LoadScripts(Assembly.Load("Game"), engine.Scripts);
engine.Run();