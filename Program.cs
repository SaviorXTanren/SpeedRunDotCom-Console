using CommandLine;
using SpeedrunComSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SpeedRunDotCom_Console
{
    public class Program
    {
        private static SpeedrunComClient client = new SpeedrunComClient();
        private static StringBuilder output = new StringBuilder();

        public static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                while (true)
                {
                    Console.Write("Command: ");
                    string input = Console.ReadLine();
                    string[] splits = input.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    Run(splits);

                    Console.WriteLine();
                    output = new StringBuilder();
                }
            }
            else
            {
                Run(args);
            }
        }

        private static void Run(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineArguments>(args).WithParsed<CommandLineArguments>(options =>
            {
                Game game = client.Games.SearchGame(name: options.GameName);
                if (game != null)
                {
                    Category category = game.Categories.FirstOrDefault(c => string.Equals(c.Name, options.Category, StringComparison.CurrentCultureIgnoreCase));
                    if (category != null)
                    {
                        PrintCategory(options, game, category);
                    }
                    else
                    {
                        output.Append("Category not found");
                    }
                }
                else
                {
                    output.Append("Game not found");
                }

                Console.WriteLine(output.ToString());
                if (options.OutputToFile)
                {
                    File.WriteAllText("output.txt", output.ToString());
                }
            });
        }

        private static void PrintCategory(CommandLineArguments options, Game game, Category category)
        {
            List<Run> runs = new List<Run>(category.Runs);
            Run worldRecord = null;
            Run personalBest = null;
            if (string.IsNullOrEmpty(options.Platform) && options.Filters.Count == 0)
            {
                if (options.WorldRecord)
                {
                    worldRecord = category.WorldRecord;
                    if (worldRecord == null)
                    {
                        output.Append("World Record not found");
                        return;
                    }
                }
            }
            else
            {
                runs.RemoveAll(r => r.Status.Type != RunStatusType.Verified);

                if (!string.IsNullOrEmpty(options.Platform))
                {
                    runs.RemoveAll(r => !string.Equals(r.Platform.Name, options.Platform, StringComparison.CurrentCultureIgnoreCase));
                }

                if (options.Filters.Count != 0)
                {
                    foreach (var filter in options.Filters)
                    {
                        runs.RemoveAll(r => !r.VariableValues.Any(vv => string.Equals(vv.Name, filter.Key, StringComparison.CurrentCultureIgnoreCase) && string.Equals(vv.Value, filter.Value, StringComparison.CurrentCultureIgnoreCase)));
                    }
                }
            }

            if (runs.Count == 0)
            {
                output.Append("No runs found meeting the specified criteria");
                return;
            }

            if (worldRecord == null && options.WorldRecord)
            {
                worldRecord = runs.OrderBy(r => r.Times.Primary).FirstOrDefault();
                if (worldRecord == null)
                {
                    output.Append("World Record not found");
                    return;
                }
            }

            if (!string.IsNullOrEmpty(options.Username))
            {
                personalBest = runs.Where(r => string.Equals(r.Player.Name, options.Username, StringComparison.CurrentCultureIgnoreCase)).OrderBy(r => r.Times.Primary).FirstOrDefault();
                if (personalBest == null)
                {
                    output.Append("Username not found");
                    return;
                }
            }

            PrintRuns(options, worldRecord, personalBest);
        }

        private static void PrintRuns(CommandLineArguments options, Run worldRecord, Run personalBest)
        {
            if (worldRecord == personalBest)
            {
                PrintRunTime(options, "WR & PB", worldRecord);
            }
            else
            {
                if (worldRecord != null)
                {
                    PrintRunTime(options, "WR", worldRecord, includePlayerNames: true);
                }

                if (worldRecord != null && personalBest != null)
                {
                    output.Append(", ");
                }

                if (personalBest != null)
                {
                    PrintRunTime(options, "PB", personalBest);
                }
            }
        }

        private static void PrintRunTime(CommandLineArguments options, string header, Run run, bool includePlayerNames = true)
        {
            output.Append($"{header}: {run.Times.Primary}");
            if (includePlayerNames)
            {
                output.Append($" - {run.Player.Name}");
            }
        }
    }
}
