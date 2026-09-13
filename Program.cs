using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Sharp_Script
{
    public static class Program
    {
        public const string keyPath = @"Software\Classes\SystemFileAssociations\.cs\shell\RunWithDotnet";
        private static int Main(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Make sure you have at least version 10 of .Net installed first.");
                    Console.WriteLine("Press:");
                    Console.WriteLine("[R] To register.");
                    Console.WriteLine("[U] To unregister.");
                    Console.WriteLine("Any other key to exit.");
                    var inputKey = Console.ReadKey();
                    var exitCode = 0;
                    try
                    {
                        if (inputKey.Key == ConsoleKey.R)
                        {

                            var exePath = Environment.ProcessPath;
                            using var key = Registry.CurrentUser.CreateSubKey(keyPath);
                            Console.WriteLine("Enter the option name that you perfer.");
                            var name = Console.ReadLine();
                            if (name == null)
                            {
                                Console.WriteLine("This name isn't valid, will use default name \"Run\"");
                                name = "Run";
                            }
                            key.SetValue("", name);
                            using var cmdKey = key.CreateSubKey("command");
                            cmdKey.SetValue("", $"\"{exePath}\" \"%1\"");
                            Console.WriteLine("Register succeed.");
                        }
                        else if (inputKey.Key == ConsoleKey.U)
                        {
                            Registry.CurrentUser.DeleteSubKeyTree(keyPath, throwOnMissingSubKey: false);
                            Console.WriteLine("Unregister succeed.");
                        }
                        else
                        {
                            return exitCode;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine("An unexpected exception was thrown: " + ex.ToString());
                        exitCode = -1;
                    }
                    Console.WriteLine("Any key to exit.");
                    Console.ReadKey();
                    return exitCode;
                }
                else
                {
                    var filePath = args[0];
                    var psi = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"run --file \"{filePath}\"",
                        UseShellExecute = false,
                        WorkingDirectory = Path.GetDirectoryName(filePath)!
                    };
                    Process.Start(psi);
                    return 0;
                }
            }
            else
            {
                Console.WriteLine("OSPlatform not supported.");
                Console.WriteLine("Any key to exit.");
                Console.ReadKey();
                return -1;
            }
        }
    }
}