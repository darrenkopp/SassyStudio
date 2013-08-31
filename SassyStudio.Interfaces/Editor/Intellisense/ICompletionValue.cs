namespace SassyStudio.Editor.Intellisense
{
    public interface ICompletionValue : IRange
    {
        string DisplayText { get; }
        string CompletionText { get; }
        SassCompletionValueType Type { get; }
    }
}