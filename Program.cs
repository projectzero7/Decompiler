using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;

if (args.Length < 1)
{
    Console.WriteLine($"Usage: decompiler.exe <path-to-assembly> [output-directory]");
    Console.WriteLine("Example: decompiler.exe MyLibrary.dll ./decompiled");

    return;
}

var assemblyPath = args[0];
var outputDir = args.Length > 1 ? args[1] : "./decompiled";

try
{
    DecompileAssembly(assemblyPath, outputDir);
    Console.WriteLine($"Decompilation complete. Output saved to: {Path.GetFullPath(outputDir)}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}"); ;
}

static void DecompileAssembly(string assemblyPath, string outputDir)
{
    if (!File.Exists(assemblyPath))
    {
        throw new FileNotFoundException($"Assembly not found: {assemblyPath}");
    }

    Directory.CreateDirectory(outputDir);

    var peFile = new PEFile(assemblyPath);

    var settings = new DecompilerSettings();
    var decompiler = new CSharpDecompiler(peFile, new UniversalAssemblyResolver(assemblyPath, false, peFile.Metadata.DetectTargetFrameworkId()), settings);

    Console.WriteLine($"Decompiling assembly: {Path.GetFileName(assemblyPath)}");
    Console.WriteLine($"Target Framework: {peFile.Metadata.DetectTargetFrameworkId()}");

    try
    {
        var source = decompiler.DecompileWholeModuleAsString();
        var filePath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(assemblyPath) + ".cs");

        File.WriteAllText(filePath, source);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to decompile assembly: {ex.Message}");
    }
}
