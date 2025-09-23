namespace RamTool.Console;

using System;
using System.Linq;

using RamTool.Manager;

class Program
{
    static void Main(string[] args)
    {
        var clearStandbyCache = true;
        var emptyWorkingSet = args.Any(x => x == "-e");
        if (args.Any(x => x == "-h" || x == "-?"))
        {
            PrintUsage();

            return;
        }

        var manager = new RamManager();

        manager.OnEmptyWorkingSet += (sender, eventArgs) => Console.WriteLine($@"* {Resources.Usage.emptyWorkingSet}...");
        manager.OnClearFileSystemCache += (sender, eventArgs) => Console.WriteLine($@"* {Resources.Usage.clearSystemCache}...");
        manager.OnClearStandbyCache += (sender, eventArgs) => Console.WriteLine($@"* {Resources.Usage.clearStandbyCache}...");

        if (emptyWorkingSet)
        {
            manager.EmptyWorkingSet();
        }

        manager.ClearFileSystemCache(clearStandbyCache);

        Console.WriteLine();
        Console.WriteLine(Resources.Usage.complete);
    }

    private static void PrintUsage()
    {
        Console.Write($@"
{Resources.Usage.usage}: {AppDomain.CurrentDomain.FriendlyName} [-e]

{Resources.Usage.param}:
    -?, -h              {Resources.Usage.help}
    -e                  {Resources.Usage.emptyWorkingSet}

");
    }
}