using System.Collections.Generic;
using TagTool.Tags;

namespace TagTool.Shaders
{
    [TagStructure(Size = 0x50)]
    public class VertexShaderBlock : TagStructure
	{
        public byte[] Unknown;
        public byte[] PCShaderBytecode;
        public List<ShaderParameter> XboxParameters;
        public uint Unknown6;
        public List<ShaderParameter> PCParameters;
        public uint Unknown8;
        public uint Unknown9;
        public VertexShaderReference XboxShaderReference;
    }
}
