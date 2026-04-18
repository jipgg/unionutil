#!/bin/env dotnet
using System.Runtime.CompilerServices;
using static System.Environment;
using static System.IO.Path;
using static System.Console;
using System.Diagnostics;

static string getFilePath([CallerFilePath] string path = null!) => path;
const string apiKeyVar = "UNIONUTIL_NUGET_APIKEY";

try {
   var apiKey = GetEnvironmentVariable(apiKeyVar) ?? throw new($"missing '{apiKeyVar}' in env");
   var rootDir = Join(GetDirectoryName(GetDirectoryName(getFilePath())));
   var outputDir = Join(rootDir, "out");
   var slnPath = Join(rootDir, "unionutil.slnx");

   WriteLine($"packing to '{outputDir}'");
   using var pack = Process.Start("dotnet", $"pack {slnPath} -o {outputDir}");
   pack.WaitForExit();
   if (pack.ExitCode is not 0) throw new("failed to pack");
   var files = Directory.GetFiles(outputDir);
   var command = $"""
      nuget push "{Path.Join(outputDir, "*.nupkg")}" --skip-duplicate -k {apiKey} -s https://api.nuget.org/v3/index.json
      """;
   WriteLine($"starting: dotnet {command}");
   using var p = Process.Start("dotnet", command);
   p.WaitForExit();
} catch (Exception ex) {
   Error.WriteLine($"Problem occurred. {ex.Message}");
   Exit(1);
}

