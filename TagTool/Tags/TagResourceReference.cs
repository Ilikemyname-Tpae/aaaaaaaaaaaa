using TagTool.Cache;
using TagTool.Common;

namespace TagTool.Tags
{
    [TagStructure(Size = 0x8)]
    public class TagResourceReference : TagStructure
    {
        /// <summary>
        /// ID is an index in ResourceGestalt.TagResources
        /// </summary>
        [TagField(Gen = CacheGeneration.Third)]
        public DatumHandle Gen3ResourceID;

        /// <summary>
        /// PageableResource structure (as a pointer)
        /// </summary>
        [TagField(Gen = CacheGeneration.HaloOnline, Flags = TagFieldFlags.Pointer)]
        public PageableResource HaloOnlinePageableResource;

        public int Unused;
    }
}
