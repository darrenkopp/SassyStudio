using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class ImportDirective : ComplexItem
    {
        readonly List<ImportFile> _Files = new List<ImportFile>(0);

        public AtRule Rule { get; protected set; }
        public TokenItem Semicolon { get; protected set; }
        public IList<ImportFile> Files { get { return _Files; } }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "import"))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                if (Rule != null)
                    Children.Add(Rule);

                while (!IsTerminator(stream.Current.Type))
                {
                    var file = itemFactory.CreateSpecific<ImportFile>(this, text, stream) ?? new ImportFile();
                    if (file.Parse(itemFactory, text, stream))
                    {
                        Children.Add(file);
                        Files.Add(file);
                    }
                    else
                    {
                        // bad news bears
                        Children.AddCurrentAndAdvance(stream);
                    }
                }

                if (stream.Current.Type == TokenType.Semicolon)
                    Semicolon = Children.AddCurrentAndAdvance(stream, SassClassifierType.Punctuation);
            }

            return Children.Count > 0;
        }

        static bool IsTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.Semicolon:
                    return true;
            }

            return false;
        }

        public override void Freeze()
        {
            base.Freeze();

            if (_Files.Count > 0)
                _Files.TrimExcess();
        }
    }
}
