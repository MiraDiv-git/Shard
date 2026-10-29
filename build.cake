var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var runtime = Argument("runtime", "linux-x64");
var outputDir = MakeAbsolute(Directory($"./bin/{configuration}/{runtime}"));

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
    DotNetPublish("./Game/Game.csproj", new DotNetPublishSettings {
        Configuration = configuration,
        Runtime = runtime,
        SelfContained = true,
        PublishSingleFile = true,
        ArgumentCustomization = args => args.Append("/p:DebugType=none /p:DebugSymbols=false"),
        OutputDirectory = outputDir
    });
});

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);