namespace TagTool.Scripting.Compiler
{
    public class EndOfFile : IScriptSyntax
    {
        public int Line { get; set; }

        public override string ToString() =>
            $"EndOfFile {{ Line: {Line} }}";
    }
}