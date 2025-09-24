# Build single file framework dependent

```
dotnet publish -r win-x64 -c Release -o publish --no-self-contained RamTool.Console\RamTool.Console.csproj -- /p:PublishSingleFile=true
```

# Build Native AOT single file

```
dotnet publish -r win-x64 -c Release -o publish --self-contained RamTool.Console\RamTool.Console.csproj -- /p:PublishAot=true /p:InvariantGlobalization=true /p:OptimizationPreference=Size /p:IlcDisableReflection=true /p:StackTraceSupport=false
```

Require [Desktop development with C++](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot)