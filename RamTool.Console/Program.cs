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

        manager.OnEmptyWorkingSet += (sender, eventArgs) => Console.WriteLine("* empty WorkingSet...");
        manager.OnClearFileSystemCache += (sender, eventArgs) => Console.WriteLine("* clear SystemCache...");
        manager.OnClearStandbyCache += (sender, eventArgs) => Console.WriteLine("* clear StandbyCache...");

        if (emptyWorkingSet)
        {
            manager.EmptyWorkingSet();
        }

        manager.ClearFileSystemCache(clearStandbyCache);

        Console.WriteLine();
        Console.WriteLine("complete");
    }

    private static void PrintUsage()
    {
        Console.Write($@"
Usage: {AppDomain.CurrentDomain.FriendlyName} [-e]

arguments:
    -?, -h              help
    -e                  empty WorkingSet for current user process

");
    }
}