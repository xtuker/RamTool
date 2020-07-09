namespace RamTool.Console
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    using RamTool.Manager;

    class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Program.MyResolveEventHandler;
        }

        static void Main(string[] args)
        {
            var clearStandbyCache = true;
            var emptyWorkingSet = args.Any(x => x == "-e");
            if (args.Any(x => x == "-h" || x == "-?"))
            {
                Program.PrintUsage();

                return;
            }

            var manager = new RamManager();

            manager.OnEmptyWorkingSet += (sender, eventArgs) => Console.WriteLine($"* {Resources.Usage.emptyWorkingSet}...");
            manager.OnClearFileSystemCache += (sender, eventArgs) => Console.WriteLine($"* {Resources.Usage.clearSystemCache}...");
            manager.OnClearStandbyCache += (sender, eventArgs) => Console.WriteLine($"* {Resources.Usage.clearStandbyCache}...");

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

        private static Assembly MyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            var assemblyDll = $"{args.Name.Split(',').First()}.dll";

            if(assemblyDll == "RamTool.Manager.dll")
            {
                var currentAssembly = Assembly.GetExecutingAssembly();
                var resourceName = $"{currentAssembly.FullName.Split(',').First()}.Resources.{assemblyDll}";
                using (var resourceStream = currentAssembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream?.Length > 0)
                    {
                        var streamLength = (int) resourceStream.Length;
                        var buffer = new byte[streamLength];
                        resourceStream.Read(buffer, 0, streamLength);
                        return Assembly.Load(buffer);
                    }
                }
            }

            return null;
        }
    }
}
