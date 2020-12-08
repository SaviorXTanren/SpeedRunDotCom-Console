using CommandLine;
using System.Collections.Generic;
using System.Linq;

namespace SpeedRunDotCom_Console
{
    public class CommandLineArguments
    {
        [Option('g', "Game", Required = true, MetaValue = "GAME NAME", HelpText = "The name of the game.")]
        public IEnumerable<string> GameNameList { get; set; }

        [Option('c', "Category", Default = null, MetaValue = "ANY%/100%/etc..", HelpText = "The specific category for the game.")]
        public IEnumerable<string> CategoryList { get; set; }

        [Option('p', "Platform", Required = false, Default = null, MetaValue = "PC/PS4/etc..", HelpText = "The specific platform for the game.")]
        public IEnumerable<string> PlatformList { get; set; }

        [Option('w', "WorldRecord", HelpText = "Whether to show the world record time or not. The -w or -u arguments is required.")]
        public bool WorldRecord { get; set; }

        [Option('u', "Username", MetaValue = "USERNAME", HelpText = "Username to look up record for. The -w or -u arguments is required.")]
        public string Username { get; set; }

        [Option("f1", Default = null, HelpText = "The 1st name of a variable to filter on.")]
        public IEnumerable<string> Filter1NamesList { get; set; }

        [Option("v1", Default = null, HelpText = "The 1st value of a variable to filter on.")]
        public IEnumerable<string> Filter1ValuesList { get; set; }

        [Option("f2", Default = null, HelpText = "The 2nd name of a variable to filter on.")]
        public IEnumerable<string> Filter2NamesList { get; set; }

        [Option("v2", Default = null, HelpText = "The 2nd value of a variable to filter on.")]
        public IEnumerable<string> Filter2ValuesList { get; set; }

        [Option("f3", Default = null, HelpText = "The 3rd name of a variable to filter on.")]
        public IEnumerable<string> Filter3NamesList { get; set; }

        [Option("v3", Default = null, HelpText = "The 3rd value of a variable to filter on.")]
        public IEnumerable<string> Filter3ValuesList { get; set; }

        [Option('o', "OutputToFile", HelpText = "Output results to \"output.txt\" file in executing location.")]
        public bool OutputToFile { get; set; }

        public string GameName { get { return string.Join(" ", this.GameNameList); } }

        public string Category { get { return string.Join(" ", this.CategoryList); } }

        public string Platform { get { return string.Join(" ", this.PlatformList); } }

        public bool CategorySet { get { return !string.IsNullOrEmpty(this.Category); } }

        public bool PlatformSet { get { return !string.IsNullOrEmpty(this.Platform); } }

        public bool UsernameSet { get { return !string.IsNullOrEmpty(this.Username); } }

        public Dictionary<string, string> Filters
        {
            get
            {
                Dictionary<string, string> filters = new Dictionary<string, string>();
                if (this.Filter1NamesList.Count() > 0 && this.Filter1ValuesList.Count() > 0)
                {
                    filters[string.Join(" ", this.Filter1NamesList)] = string.Join(" ", this.Filter1ValuesList);
                }
                if (this.Filter2NamesList.Count() > 0 && this.Filter2ValuesList.Count() > 0)
                {
                    filters[string.Join(" ", this.Filter2NamesList)] = string.Join(" ", this.Filter2ValuesList);
                }
                if (this.Filter3NamesList.Count() > 0 && this.Filter3ValuesList.Count() > 0)
                {
                    filters[string.Join(" ", this.Filter3NamesList)] = string.Join(" ", this.Filter3ValuesList);
                }
                return filters;
            }
        }
    }
}
