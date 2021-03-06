using System;
using System.Collections.Generic;
using System.Linq;
using TagTool.Cache;
using TagTool.Tags.Definitions;

namespace TagTool.Commands.Models
{
    class ListVariantsCommand : Command
    {
        private GameCache Cache { get; }
        private Model Definition { get; }

        public ListVariantsCommand(GameCache cache, Model model)
            : base(true,
                  
                  "ListVariants",
                  "List available variants of the current model definition.",
                  
                  "ListVariants",
                  "Lists available variants of the current model definition which can be used with \"extract-model\".")
        {
            Cache = cache;
            Definition = model;
        }

        public override object Execute(List<string> args)
        {
            if (args.Count != 0)
                return false;

            var variantNames = Definition.Variants.Select(v => Cache.StringTable.GetString(v.Name) ?? v.Name.ToString()).OrderBy(n => n).ToList();

            if (variantNames.Count == 0)
            {
                Console.WriteLine("Model has no variants");
                return true;
            }

            foreach (var name in variantNames)
                Console.WriteLine(name);

            return true;
        }
    }
}
