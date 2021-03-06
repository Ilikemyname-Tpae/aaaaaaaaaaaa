using System;
using System.Collections.Generic;
using TagTool.Cache;
using TagTool.Common;

namespace TagTool.Commands.Tags
{
    class ListTagsCommand : Command
    {
        private GameCache Cache { get; }

        public ListTagsCommand(GameCache cache)
            : base(true,

                  "ListTags",
                  "Lists tag instances that are of the specified tag group.",

                  "ListTags <Tag Group> {Options}",

                  "Lists tag instances that are of the specified tag group." +
                  "Multiple group tags to list tags from can be specified.\n" +
                  "Tags of a group which inherit from the given group tags will also\n" +
                  "be printed. If no group tag is specified, all tags in the current\n" +
                  "tag cache file will be listed.")
        {
            Cache = cache;
        }

        public override object Execute(List<string> args)
        {
            var groupTag = (args.Count == 0 || args[0].EndsWith(":")) ? Tag.Null : Tag.Parse(Cache, args[0]);

            if (args.Count > 0 && !args[0].EndsWith(":"))
                args.RemoveAt(0);

            var named = false;
            var unnamed = false;
            
            var startFilter = "";
            var endFilter = "";
            var filter = "";

            while (args.Count >= 1)
            {
                switch (args[0].ToLower())
                {
                    case "starts:":
                    case "startswith:":
                    case "starts_with:":
                    case "starting:":
                    case "startingwith:":
                    case "starting_with:":
                    case "start_filter:":
                    case "starting_filter:":
                        startFilter = args[1];
                        args.RemoveAt(1);
                        goto case "named";

                    case "ends:":
                    case "ending:":
                    case "endingwith:":
                    case "ending_with:":
                    case "endswith:":
                    case "ends_with:":
                    case "end_filter:":
                    case "ending_filter:":
                        endFilter = args[1];
                        args.RemoveAt(1);
                        goto case "named";

                    case "named:":
                    case "filter:":
                    case "contains:":
                    case "containing:":
                        filter = args[1];
                        args.RemoveAt(1);
                        goto case "named";

                    case "named":
                        named = true;
                        break;

                    case "unnamed":
                        unnamed = true;
						break;

                    default:
                        throw new FormatException(args[0]);
                }

                args.RemoveAt(0);
            }

            foreach (var tag in Cache.TagCache.TagTable)
            {
                if (tag == null || (groupTag != Tag.Null && !tag.IsInGroup(groupTag)))
                    continue;
                string groupName;
                if (Cache.Version > CacheVersion.Halo3Beta) // gen 1 and gen 2 tag groups don't have a name stringid
                    groupName = Cache.StringTable.GetString(tag.Group.Name);
                else
                    groupName = tag.Group.Tag.ToString();
                
                if (named)
                {
                    if (tag.Name == null)
                        continue;

                    if (!tag.Name.StartsWith(startFilter) || !tag.Name.Contains(filter) || !tag.Name.EndsWith(endFilter))
                        continue;
                }
                
                if (unnamed && tag.Name == null)
                    Console.WriteLine($"[Index: 0x{tag.Index:X4}, Offset: 0x{tag.DefinitionOffset:X8}] {groupName} ({tag.Group.Tag})");
                else if (named && tag.Name != null)
                    Console.WriteLine($"[Index: 0x{tag.Index:X4}, Offset: 0x{tag.DefinitionOffset:X8}] {tag.Name}.{groupName}");
                else if (!named && !unnamed)
                    Console.WriteLine($"[Index: 0x{tag.Index:X4}, Offset: 0x{tag.DefinitionOffset:X8}] {tag.Name}.{groupName}");
            }

            return true;
        }
    }
}