namespace SassyStudio.Editor.Intellisense
{
    interface IBrowserReference
    {
        string Abbreviation { get; }
        string Name { get; }
    }

    class BrowserReference : IBrowserReference
    {
        public BrowserReference(string abbreviation, string name)
        {
            Abbreviation = abbreviation;
            Name = name;
        }
        
        public string Abbreviation { get; private set; }
        public string Name { get; private set; }
    }
}