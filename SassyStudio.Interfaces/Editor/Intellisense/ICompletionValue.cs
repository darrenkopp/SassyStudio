namespace SassyStudio.Editor.Intellisense
{
    public interface ICompletionValue
    {
        string DisplayText { get; }
        string CompletionText { get; }
        string Description { get; }
        SassCompletionValueType Type { get; }
    }
}