using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Windows.Forms;

namespace WMPlayerStart
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Prevent exception messages from being translated into the users language
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            string dir = "", subfStr = "";
            bool autoExit = false, subf = false;
            if (args.Length != 0)
            {
                if (args.Length.EqualsToAtLeastOne(1, 2, 3))
                {
                    Console.WriteLine("wmplayerstart [/s true|false /d dir] [/x]\n");
                    Console.WriteLine("  /s true|false    Indicates whether this program will search only specified folder or its subfolders too.");
                    Console.WriteLine("  /d dir           Specifies directory where files will be searching.");
                    Console.WriteLine("  /x               Closes application only after successfull execution.");
                    Console.WriteLine("\nIf program has been run without any flags, it will ask for required values.");
                    return;
                }
                else if (args.Length >= 4)
                {
                    try
                    {
                        subf = bool.Parse(args[1]);
                        subfStr = subf ? "s" : "f";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);

                        Console.Write("\nPress any key to quit...");
                        Console.ReadKey(true);
                        return;
                    }

                    if (args.Length == 4 || (args.Length == 5 && args[4] == "/x"))
                    {
                        if (args[3].StartsWith("\"") && args[(args[args.Length - 1] == "/x" ? args.Length - 2 : args.Length - 1)].EndsWith("\""))
                        {
                            dir = args.Connect(' ', false, 5, args.Length - 2);
                            dir = dir.Remove(0, 1);
                            dir = dir.Remove(dir.Length - 1, 1);
                        }
                        else
                            dir = args[3];
                    }
                    else
                    {
                        if (args[3].StartsWith("\"") && args[args.Length - 2].EndsWith("\""))
                        {
                            dir = args.Connect(' ', false, 4, args.Length - 2);
                            dir = dir.Remove(0, 1);
                            dir = dir.Remove(dir.Length - 1, 1);
                        }
                        else
                        {
                            Console.WriteLine("You must put path into quotation marks if it contains spaces.");
                            Console.Write("\nPress any key to quit...");
                            Console.ReadKey(true);
                            return;
                        }
                    }
                    autoExit = args[args.Length - 1] == "/x";
                }
            }

            string command = "";
            try
            {
                Console.Write("Path to search for the media: ");
                if (args.Length == 0)
                    dir = Console.ReadLine();
                else
                    Console.WriteLine(args.Length.EqualsToAtLeastOne(4, 5) && args[args.Length - 1] == "/x" ? "\"" + dir + "\"" : dir);
                DirectoryInfo dirI = new DirectoryInfo(dir);

                if (dirI.Exists)
                {
                    Console.Write("Search only specified folder or its subfolders too? [f | s]: ");
                    if (args.Length == 0)
                    {
                        subfStr = Console.ReadLine();
                        subf = subfStr == "f" ? false : true;
                    }
                    else
                        Console.WriteLine(subfStr);

                    if (subfStr.EqualsToAtLeastOne("f", "s"))
                    {
                        Console.Write("\nPlease wait...");
                        string[] files = Directory.GetFiles(dir, "*.mp3", subf ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                        Console.WriteLine();
                        if (files.Length == 0)
                        {
                            Console.WriteLine("\nNo files are found.");
                            Console.Write("\nPress any key to quit...");
                            Console.ReadKey(true);
                            return;
                        }
                        else
                        {
                            Array.Sort(files, (f1, f2) => f1.CompareTo(f2));

                            Console.WriteLine("\nFound " + files.Length + " files:");
                            for (int i = 0; i < files.Length; i++)
                            {
                                string[] s = files[i].Split('\\');
                                Console.WriteLine("  " + s[s.Length - 1]);
                            }

                            Process p = new Process();
                            p.StartInfo.FileName = Environment.GetEnvironmentVariable("ProgramFiles(x86)",
                                EnvironmentVariableTarget.Process) + "\\Windows Media Player\\wmplayer.exe";
                            p.StartInfo.Arguments = files.Connect(' ', true);
                            p.Start();

                            command = "\"" + p.StartInfo.FileName + "\" " + p.StartInfo.Arguments;

                            Console.WriteLine("\nSuccessfully started 'wmplayer.exe'.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nUnknown option '" + subfStr + "'.");
                        Console.Write("\nPress any key to quit...");
                        Console.ReadKey(true);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException)
                {
                    Console.WriteLine(e.Message);
                }
                else if (e is System.Security.SecurityException)
                {
                    Console.WriteLine(e.Message);
                }
                else if (e is ArgumentException)
                {
                    Console.WriteLine(e.Message);
                }
                else if (e is PathTooLongException)
                {
                    Console.WriteLine(e.Message);
                }
                else if (e.StackTrace.Contains("System.IO.__Error.WinIOError"))
                {
                    Console.WriteLine(e.Message);
                }
                else
                {
                    Console.WriteLine("Unknown exception:");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

                Console.Write("\nPress any key to quit...");
                Console.ReadKey(true);
                return;
            }
            if (!autoExit)
            {
                Console.WriteLine("\nPress 'C' to copy command used to run wmplayer.exe to clipboard and then quit.");
                Console.WriteLine("Press any other key to quit...");
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.C)
                {
                    Clipboard.SetText(command);
                }
            }
        }
    }
}
