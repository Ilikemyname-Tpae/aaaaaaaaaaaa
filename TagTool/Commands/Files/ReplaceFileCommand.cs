using System;
using System.Collections.Generic;
using System.IO;
using TagTool.Cache;
using TagTool.Tags.Definitions;

namespace TagTool.Commands.Files
{
    class ReplaceFileCommand : Command
    {
        private GameCache Cache { get; }
        private CachedTag Tag { get; }
        private VFilesList Definition { get; }

        public ReplaceFileCommand(GameCache cache, CachedTag tag, VFilesList definition)
            : base(false,

                  "ReplaceFile",
                  "Replace a file stored in the tag",

                  "ReplaceFile <virtual path> [filename]",

                  "Replaces a file stored in the tag. The tag will be resized as necessary.")
        {
            Cache = cache;
            Tag = tag;
            Definition = definition;
        }

        public override object Execute(List<string> args)
        {
            if (args.Count != 1 && args.Count != 2)
                return false;

            var virtualPath = args[0];
            var inputPath = (args.Count == 2) ? args[1] : virtualPath;
            var file = Definition.Find(virtualPath);

            if (file == null)
            {
                Console.WriteLine("Unable to find file {0}.", virtualPath);
                return true;
            }

            byte[] data;

            try
            {
                data = File.ReadAllBytes(inputPath);
            }
            catch (IOException)
            {
                Console.WriteLine("Unable to read from {0}.", inputPath);
                return true;
            }

            Definition.Replace(file, data);

            using (var stream = Cache.OpenCacheReadWrite())
                Cache.Serialize(stream, Tag, Definition);

            Console.WriteLine("Imported 0x{0:X} bytes.", data.Length);

            return true;
        }
    }
}
