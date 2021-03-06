using System;
using System.IO;
using TagTool.BlamFile;
using TagTool.Cache.Gen2;
using TagTool.Cache.Resources;
using TagTool.IO;
using TagTool.Serialization;
using TagTool.Tags;

namespace TagTool.Cache
{
    public class GameCacheGen2 : GameCache
    {
        public MapFile BaseMapFile;
        public FileInfo CacheFile;

        public TagCacheGen2 TagCacheGen2;
        public StringTableGen2 StringTableGen2;

        public override TagCache TagCache => TagCacheGen2;
        public override StringTable StringTable => StringTableGen2;

        public override ResourceCache ResourceCache => throw new NotImplementedException();

        public GameCacheGen2(MapFile mapFile, FileInfo file)
        {
            BaseMapFile = mapFile;
            CacheFile = file;
            Version = BaseMapFile.Version;
            CacheFile = file;
            Deserializer = new TagDeserializer(Version);
            Serializer = new TagSerializer(Version);
            Endianness = BaseMapFile.EndianFormat;
            DisplayName = mapFile.Header.Name + ".map";
            Directory = file.Directory;

            using (var cacheStream = OpenCacheRead())
            using (var reader = new EndianReader(cacheStream, Endianness))
            {
                TagCacheGen2 = new TagCacheGen2(reader, mapFile);
                StringTableGen2 = new StringTableGen2(reader, mapFile);
            }
        }

        #region Serialization

        public override T Deserialize<T>(Stream stream, CachedTag instance) =>
            Deserialize<T>(new Gen2SerializationContext(stream, this, (CachedTagGen2)instance));

        public override object Deserialize(Stream stream, CachedTag instance) =>
            Deserialize(new Gen2SerializationContext(stream, this, (CachedTagGen2)instance), TagDefinition.Find(instance.Group.Tag));

        public override void Serialize(Stream stream, CachedTag instance, object definition)
        {
            if (typeof(CachedTagGen2) == instance.GetType())
                Serialize(stream, (CachedTagGen2)instance, definition);
            else
                throw new Exception($"Try to serialize a {instance.GetType()} into a Gen 3 Game Cache");
        }

        public void Serialize(Stream stream, CachedTagGen2 instance, object definition)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(Stream stream, CachedTagGen2 instance) =>
            Deserialize<T>(new Gen2SerializationContext(stream, this, instance));

        public object Deserialize(Stream stream, CachedTagGen2 instance) =>
            Deserialize(new Gen2SerializationContext(stream, this, instance), TagDefinition.Find(instance.Group.Tag));

        //
        // private methods for internal use
        //

        private T Deserialize<T>(ISerializationContext context) =>
            Deserializer.Deserialize<T>(context);

        private object Deserialize(ISerializationContext context, Type type) =>
            Deserializer.Deserialize(context, type);

        #endregion


        public override Stream OpenCacheRead() => CacheFile.OpenRead();

        public override Stream OpenCacheReadWrite()
        {
            throw new NotImplementedException();
        }

        public override Stream OpenCacheWrite()
        {
            throw new NotImplementedException();
        }

        public override void SaveStrings()
        {
            throw new NotImplementedException();
        }
    }
}
