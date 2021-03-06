using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TagTool.Cache;
using TagTool.Commands.Editing;
using TagTool.Common;
using System.IO;

namespace TagTool.Commands.Tags
{
    class ForEachCommand : Command
    {
        private CommandContextStack ContextStack { get; }
        private GameCache Cache { get; }

        public ForEachCommand(CommandContextStack contextStack, GameCache cache) :
            base(false,
                
                "ForEach",
                "Executes a command on every instance of the specified tag group.",
                
                "ForEach [Const] <Tag Group> [Named: <Regex>] <Command...>",
                
                "Executes a command on every instance of the specified tag group.")
        {
            ContextStack = contextStack;
            Cache = cache;
        }

        public override object Execute(List<string> args)
        {
            if (args.Count < 1)
                return false;

            var isConst = false;

            if (args[0].ToLower() == "const")
            {
                args.RemoveAt(0);
                isConst = true;
            }

            if (args.Count < 1)
                return false;

            if (!Cache.TryParseGroupTag(args[0], out var groupTag))
            {
                Console.WriteLine($"Invalid tag group: {args[0]}");
                return true;
            }

            args.RemoveAt(0);

            var startFilter = "";
            var endFilter = "";
            var filter = "";
            var filename = "";

            string pattern = null;

            while (args.Count > 0 && args[0].EndsWith(":"))
            {
                switch (args[0].ToLower())
                {
                    case "in_file:":
                        filename = args[1];
                        args.RemoveRange(0, 2);
                        break;
                    case "regex:":
                        if (args.Count < 3)
                            return false;
                        pattern = args[1];
                        args.RemoveRange(0, 2);
                        break;

                    case "starts:":
                    case "startswith:":
                    case "starts_with:":
                    case "starting:":
                    case "startingwith:":
                    case "starting_with:":
                    case "start_filter:":
                    case "starting_filter:":
                        startFilter = args[1];
                        args.RemoveRange(0, 2);
                        break;

                    case "ends:":
                    case "ending:":
                    case "endingwith:":
                    case "ending_with:":
                    case "endswith:":
                    case "ends_with:":
                    case "end_filter:":
                    case "ending_filter:":
                        endFilter = args[1];
                        args.RemoveRange(0, 2);
                        break;

                    case "named:":
                    case "filter:":
                    case "contains:":
                    case "containing:":
                        filter = args[1];
                        args.RemoveRange(0, 2);
                        break;
                }
            }

            var commandsToExecute = new List<List<string>>();

            // if no command is given, keep reading commands from stdin until an empty line encountered
            if (args.Count < 1)
            {
                string line;
                while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
                {
                    var commandsArgs = ArgumentParser.ParseCommand(line, out string redirectFile);
                    commandsToExecute.Add(commandsArgs);
                }
            }
            else
            {
                commandsToExecute.Add(args);
            }

            List<CachedTag> tags = null;

            // if a file is given use that as the source for tags
            if (!string.IsNullOrWhiteSpace(filename))
            {
                var tagsList = new List<CachedTag>();
                foreach (var line in File.ReadAllLines(filename))
                    tags.Add(Cache.GetTag(line));

                tags = tagsList;
            }
            else
            {
                tags = Cache.TagCache.NonNull().ToList();
            }

            var rootContext = ContextStack.Context;


            foreach (var instance in tags)
            {
                if (instance == null || (groupTag != Tag.Null && !instance.IsInGroup(groupTag)))
                    continue;

                var tagName = instance.Name ?? $"0x{instance.Index:X4}";

                try
                {
                    if (pattern != null && !Regex.IsMatch(tagName, pattern, RegexOptions.IgnoreCase))
                        continue;
                }
                catch
                {
                    continue;
                }

                if (!tagName.StartsWith(startFilter) || !tagName.Contains(filter) || !tagName.EndsWith(endFilter))
                    continue;

                object definition = null;
                using (var stream = Cache.OpenCacheRead())
                    definition = Cache.Deserialize(stream, instance);


                ContextStack.Push(EditTagContextFactory.Create(ContextStack, Cache, instance, definition));

                Console.WriteLine();
                Console.WriteLine($"{tagName}.{Cache.StringTable.GetString(instance.Group.Name)}:");
                ContextStack.Context.GetCommand(args[0]).Execute(args.Skip(1).ToList());

                while (ContextStack.Context != rootContext) ContextStack.Pop();

                if (!isConst)
                {
                    using (var stream = Cache.OpenCacheReadWrite())
                        Cache.Serialize(stream, instance, definition);
                }

            }

            Console.WriteLine();
            return true;
        }
    }
}
