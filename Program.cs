using CommandLine;
using SpeedrunComSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SpeedRunDotCom_Console
{
    public class Program
    {
        private static SpeedrunComClient client = new SpeedrunComClient();
        private static List<string> messages = new List<string>();

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
                    messages.Clear();
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
                if (!options.UsernameSet)
                {
                    options.WorldRecord = true;
                }

                Game game = client.Games.SearchGame(name: options.GameName);
                if (game != null && game.Categories.Count > 0)
                {
                    Dictionary<Category, List<Run>> categories = new Dictionary<Category, List<Run>>();
                    if (options.CategorySet)
                    {
                        Category category = game.Categories.FirstOrDefault(c => string.Equals(c.Name, options.Category, StringComparison.CurrentCultureIgnoreCase));
                        if (category != null)
                        {
                            categories[category] = new List<Run>(category.Runs);
                        }
                        else
                        {
                            messages.Add("Category not found");
                        }
                    }
                    else
                    {
                        if (options.UsernameSet)
                        {
                            categories = game.Categories.ToDictionary(c => c, c => c.Runs.ToList());
                        }
                        else if (game.FullGameCategories.Count() > 0)
                        {
                            categories[game.FullGameCategories.First()] = new List<Run>(game.FullGameCategories.First().Runs);
                        }
                        else
                        {
                            categories[game.Categories.First()] = new List<Run>(game.Categories.First().Runs);
                        }
                    }

                    foreach (var kvp in categories)
                    {
                        List<Run> runs = new List<Run>(kvp.Value);
                        Run worldRecord = null;
                        Run personalBest = null;

                        if (options.WorldRecord)
                        {
                            worldRecord = kvp.Key.WorldRecord;
                        }

                        if (options.PlatformSet || options.Filters.Count > 0)
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

                            if (options.WorldRecord)
                            {
                                worldRecord = runs.OrderBy(r => r.Times.Primary).FirstOrDefault();
                            }
                        }

                        if (runs.Count == 0)
                        {
                            messages.Add("No runs found meeting the specified criteria");
                            return;
                        }

                        if (!string.IsNullOrEmpty(options.Username))
                        {
                            personalBest = runs.Where(r => string.Equals(r.Player.Name, options.Username, StringComparison.CurrentCultureIgnoreCase)).OrderBy(r => r.Times.Primary).FirstOrDefault();
                        }

                        AddRun(options, kvp.Key, worldRecord, personalBest);
                    }

                    if (messages.Count == 0)
                    {
                        messages.Add("Could not find any times for the search criteria");
                    }
                }
                else
                {
                    messages.Add("Game not found");
                }

                Console.WriteLine(string.Join(" ** ", messages));
                if (options.OutputToFile)
                {
                    File.WriteAllText("output.txt", string.Join("; ", messages));
                }
            });
        }

        private static void AddRun(CommandLineArguments options, Category category, Run worldRecord, Run personalBest)
        {
            if (worldRecord != null || personalBest != null)
            {
                string run = string.Empty;
                if (!options.CategorySet)
                {
                    run += $"{category.Name} ";
                }

                if (worldRecord == personalBest)
                {
                    run += GetRunTime(options, "WR & PB", worldRecord);
                }
                else
                {
                    if (worldRecord != null)
                    {
                        run += GetRunTime(options, "WR", worldRecord, includePlayerNames: true);
                    }

                    if (worldRecord != null && personalBest != null)
                    {
                        run += ", ";
                    }

                    if (personalBest != null)
                    {
                        run += GetRunTime(options, "PB", personalBest);
                    }
                }
                messages.Add(run);
            }
        }

        private static string GetRunTime(CommandLineArguments options, string header, Run run, bool includePlayerNames = true)
        {
            string text = $"{header}: {run.Times.Primary}";
            if (includePlayerNames)
            {
                text += $" - {run.Player.Name}";
            }
            return text;
        }
    }
}
