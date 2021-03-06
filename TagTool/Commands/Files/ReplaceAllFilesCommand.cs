using System;
using System.Collections.Generic;
using System.IO;
using TagTool.Cache;
using TagTool.Tags.Definitions;

namespace TagTool.Commands.Files
{
    class ReplaceAllFilesCommand : Command
    {
        private GameCache Cache { get; }
        private CachedTag Tag { get; }
        private VFilesList Definition { get; }

        public ReplaceAllFilesCommand(GameCache cache, CachedTag tag, VFilesList definition)
            : base(false,

                  "ReplaceAllFiles",
                  "Replace all files stored in the tag.",

                  "ReplaceAllFiles <directory>",
                  "Replaces all file stored in the tag. The tag will be resized as necessary.")
        {
            Cache = cache;
            Tag = tag;
            Definition = definition;
        }

        public override object Execute(List<string> args)
        {
            if (args.Count != 1)
                return false;

            var rootDirectory = new DirectoryInfo(args[0]).FullName.Replace('/', '\\');

            if (!rootDirectory.EndsWith("\\"))
                rootDirectory += "\\";

            var directories = new List<DirectoryInfo> { new DirectoryInfo(rootDirectory) };
            var imported = 0;

            Definition.Files.Clear();
            Definition.Data = new byte[0];

            while (directories.Count != 0)
            {
                var directory = directories[0];

                foreach (var file in directory.GetFiles())
                {
                    Definition.Insert(file.Name, file.DirectoryName.Replace(rootDirectory, "").Replace("/", "\\") + "\\", File.ReadAllBytes(file.FullName));
                    imported++;
                }

                foreach (var subdirectory in directory.GetDirectories())
                    directories.Add(subdirectory);

                directories.RemoveAt(0);
            }
            
            using (var stream = Cache.OpenCacheReadWrite())
                Cache.Serialize(stream, Tag, Definition);

            Console.WriteLine("Imported {0} files.", imported);

            return true;
        }
    }
}