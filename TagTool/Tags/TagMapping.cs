using System;
using static TagTool.Tags.TagFieldFlags;

namespace TagTool.Tags
{
    [TagStructure(Size = 0x20)]
    public class TagMapping : TagStructure
	{
        public ParticleStates InputVariable;
        public ParticleStates RangeVariable;
        public OutputModifierValue OutputModifier;
        public ParticleStates OutputModifierInput;

        public TagFunction Function = new TagFunction { Data = new byte[0] };

        public float RuntimeMConstantValue;
        public EditablePropertiesFlags RuntimeMFlags;

        [TagField(Flags = Padding, Length = 3)]
        public byte[] Unused = new byte[3];

        public enum ParticleStates : byte
        {
            Age,
            SystemAge,
            RandomSeed,
            SystemRandomSeed,
            Random1,
            Random2,
            Random3,
            Random4,
            SystemRandom1,
            SystemRandom2,
            SystemTime,
            SystemLod,
            GameTime,
            EffectAScale,
            EffectBScale,
            PhysicsRotation,
            LocationRandom,
            DistanceFromEmitter,
            SimulationA,
            SimulationB,
            Velocity,
            Random5,
            Random6,
            Random7,
            Random8,
            SystemRandom3,
            SystemRandom4,
            Invalid = 0xFF
        }

        [Flags]
        public enum ParticleStatesFlags : uint
        {
            None = 0,
            Age = 1 << 0,
            SystemAge = 1 << 1,
            RandomSeed = 1 << 2,
            SystemRandomSeed = 1 << 3,
            Random1 = 1 << 4,
            Random2 = 1 << 5,
            Random3 = 1 << 6,
            Random4 = 1 << 7,
            SystemRandom1 = 1 << 8,
            SystemRandom2 = 1 << 9,
            SystemTime = 1 << 10,
            SystemLod = 1 << 11,
            GameTime = 1 << 12,
            EffectAScale = 1 << 13,
            EffectBScale = 1 << 14,
            PhysicsRotation = 1 << 15,
            LocationRandom = 1 << 16,
            DistanceFromEmitter = 1 << 17,
            SimulationA = 1 << 18,
            SimulationB = 1 << 19,
            Velocity = 1 << 20,
            Random5 = 1 << 21,
            Random6 = 1 << 22,
            Random7 = 1 << 23,
            Random8 = 1 << 24,
            SystemRandom3 = 1 << 25,
            SystemRandom4 = 1 << 26,
        }

        public enum OutputModifierValue : sbyte
        {
            None,
            Plus,
            Times
        }

        [Flags]
        public enum EditablePropertiesFlags : byte
        {
            None = 0,
            Bit0 = 1 << 0,
            Bit1 = 1 << 1,
            Bit2 = 1 << 2,
            Bit3 = 1 << 3,
            Bit4 = 1 << 4,
            IsConstant = 1 << 5,
            Bit6 = 1 << 6,
            ConstantOverTime = 1 << 7
        }

        [Flags]
        public enum ForceFlagsValue : byte
        {
            None = 0,
            ForceConstant = 1 << 0
        }
    }
}
