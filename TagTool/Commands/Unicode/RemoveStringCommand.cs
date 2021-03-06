using TagTool.Cache;
using TagTool.Common;
using TagTool.Tags.Definitions;
using System.Collections.Generic;

namespace TagTool.Commands.Unicode
{
    class RemoveStringCommand : Command
    {
        private GameCache Cache { get; }
        private CachedTag Tag { get; }
        private MultilingualUnicodeStringList Definition { get; set; }

        public RemoveStringCommand(GameCache cache, CachedTag tag, MultilingualUnicodeStringList definition) :
            base(false,
                
                "RemoveString",
                "Removes a string entry from the multilingual_unicode_string_list definition.",
                
                "RemoveString <StringID>",

                "Removes a string entry from the multilingual_unicode_string_list definition.")
        {
            Cache = cache;
            Tag = tag;
            Definition = definition;
        }

        public override object Execute(List<string> args)
        {
            if (args.Count != 1)
                return false;

            var stringID = Cache.StringTable.GetStringId(args[0]);

            var newDefinition = new MultilingualUnicodeStringList
            {
                Data = new byte[0],
                Strings = new List<LocalizedString>()
            };

            foreach (var oldString in Definition.Strings)
            {
                if (oldString.StringID == stringID)
                    continue;

                var newString = new LocalizedString
                {
                    Offsets = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
                    StringID = oldString.StringID,
                    StringIDStr = oldString.StringIDStr
                };

                for (var i = 0; i < 12; i++)
                {
                    if (oldString.Offsets[i] == -1)
                        continue;

                    newDefinition.SetString(newString, (GameLanguage)i, Definition.GetString(oldString, (GameLanguage)i));
                }

                newDefinition.Strings.Add(newString);
            }

            Definition.Data = newDefinition.Data;
            Definition.Strings = newDefinition.Strings;

            return true;
        }
    }
}