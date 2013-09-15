namespace SassyStudio.Editor.Intellisense
{
    abstract class CssSchemaItem
    {
        public CssSchemaItem(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; protected set; }
        public string Description { get; protected set; }
    }
}