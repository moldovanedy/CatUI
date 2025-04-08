using System;

namespace CatUIUtility
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            //show help on how to use the tool
            if (args.Length == 0 || (args.Length == 1 && (args[0] == "--help" || args[0] == "-h")))
            {
                Console.WriteLine(
                    "Usage: catui-utility <command> [arguments]\n" +
                    "\n" +
                    "<command> is one of the following:\n" +
                    "- theme: Outputs code that can be used to setup the theme with the CatTheme; " +
                    "you can use a JSON file from Material Theme Builder to set the colors accordingly " +
                    "or to use Material 3 typography options for beautiful and consistent text in your app.\n" +
                    "\n" +
                    "[arguments]: Use \"-h\" or \"-help\" after an option to get more information about the arguments " +
                    "for each options.\n"
                );
                return;
            }

            if (args[0] == "theme")
            {
                ThemeCommand.Start(args);
            }
            else
            {
                Console.WriteLine($"Unknown command \"{args[0]}\". Valid commands are: \"theme\".");
            }
        }
    }
}
