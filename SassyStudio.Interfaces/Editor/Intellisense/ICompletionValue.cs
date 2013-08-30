namespace SassyStudio.Editor.Intellisense
{
    public interface ICompletionValue
    {
        string DisplayText { get; }
        string CompletionText { get; }
    }
}