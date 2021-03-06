using System;
using System.Collections.Generic;
using System.IO;
using TagTool.Cache;
using TagTool.Cache.HaloOnline;

namespace TagTool.Commands.Tags
{
    class ImportTagCommand : Command
    {
        private GameCacheHaloOnlineBase Cache { get; }

        public ImportTagCommand(GameCacheHaloOnlineBase cache)
            : base(true,

                  "ImportTag",
                  "Overwrites a tag with data from a file.",

                  "ImportTag <Tag> <Path>",

                  "Overwrites a tag with data from a file.")
        {
            Cache = cache;
        }

        public override object Execute(List<string> args)
        {
            if (args.Count != 2)
                return false;

            if (!Cache.TryGetCachedTag(args[0], out var instance))
                return false;

            var path = args[1];

            if (!File.Exists(path))
                return false;

            byte[] data;

            using (var inStream = File.OpenRead(path))
            {
                data = new byte[inStream.Length];
                inStream.Read(data, 0, data.Length);
            }

            using (var stream = Cache.OpenCacheReadWrite())
                Cache.TagCacheGenHO.SetTagDataRaw(stream, (CachedTagHaloOnline)instance, data);

            Console.WriteLine($"Imported 0x{data.Length:X} bytes.");

            return true;
        }
    }
}