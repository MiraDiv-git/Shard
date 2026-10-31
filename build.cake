var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var runtimes = HasArgument("runtime")
    ? new[] { Argument<string>("runtime") }
    : new[] { "linux-x64", "win-x64" };


string ReadCfg(string path, string key)
{
    foreach (var raw in System.IO.File.ReadLines(path))
    {
        var line = raw.Trim();
        if (line.Length == 0 || line[0] is ';' or '#' or '[') continue;

        var eq = line.IndexOf('=');
        if (eq < 0) continue;

        if (line[..eq].Trim() == key)
            return line[(eq + 1)..].Trim().Trim('"');
    }
    throw new Exception($"Key '{key}' not found in {path}");
}

var gameName = ReadCfg("./Game/config/meta.cfg", "GAME_NAME");


Task("Clean")
    .Does(() =>
{
    CleanDirectory("./bin");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetRestore("./Shard.slnx");
});

Task("Publish")
    .IsDependentOn("Restore")
    .Does(() =>
{
    foreach (var rid in runtimes)
    {
        var outputDir = MakeAbsolute(Directory($"./bin/{configuration}/{rid}"));

        DotNetPublish("./Runtime/Runtime.csproj", new DotNetPublishSettings {
            Configuration = configuration,
            Runtime = rid,
            SelfContained = true,
            PublishSingleFile = true,
            ArgumentCustomization = args => args
                .Append("/p:DebugType=none")
                .Append("/p:DebugSymbols=false")
                .Append("/p:IncludeNativeLibrariesForSelfExtract=true")
                .AppendSwitchQuoted("/p:GameName", "=", gameName),
            OutputDirectory = outputDir
        });
    }
});

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);
