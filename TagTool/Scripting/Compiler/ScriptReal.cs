namespace TagTool.Scripting.Compiler
{
    public class ScriptReal : IScriptSyntax
    {
        public double Value;

        public int Line { get; set; }

        public override string ToString() =>
            $"ScriptReal {{ Line: {Line}, Value: {Value} }}";
    }
}