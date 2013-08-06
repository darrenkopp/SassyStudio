using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class VariableDefinition : ComplexItem
    {
        private readonly ExpresionMode Mode;
        public VariableDefinition(ExpresionMode mode = ExpresionMode.None)
        {
            Mode = mode;
            Values = new ParseItemList();
        }

        public VariableName Name { get; protected set; }
        public TokenItem Colon { get; protected set; }
        public ParseItemList Values { get; protected set; }
        public ImportanceModifier Modifier { get; protected set; }
        public TokenItem Semicolon { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            var name = new VariableName(SassClassifierType.VariableDefinition);
            if (name.Parse(itemFactory, text, stream))
            {
                Name = name;
                Children.Add(name);
            }

            if (stream.Current.Type == TokenType.Colon)
                Colon = Children.AddCurrentAndAdvance(stream);

            while (!IsValueTerminator(Mode, stream))
            {
                ParseItem item;
                if (itemFactory.TryCreateParsedOrDefault(this, text, stream, out item))
                {
                    Values.Add(item);
                    Children.Add(item);
                }
            }

            if (ImportanceModifier.IsImportanceModifier(text, stream))
            {
                Modifier = new ImportanceModifier();
                Modifier.Parse(itemFactory, text, stream);
                Children.Add(Modifier);
            }

            if (stream.Current.Type == TokenType.Semicolon)
                Semicolon = Children.AddCurrentAndAdvance(stream);

            return Children.Count > 0;
        }

        public override void Freeze()
        {
            base.Freeze();
            Values.TrimExcess();
        }

        static bool IsValueTerminator(ExpresionMode mode, ITokenStream stream)
        {
            switch (stream.Current.Type)
            {
                case TokenType.EndOfFile:
                case TokenType.Bang:
                case TokenType.Semicolon:
                case TokenType.CloseCurlyBrace:
                case TokenType.OpenCurlyBrace:
                    return true;
            }

            if (mode == ExpresionMode.Argument)
                return stream.Current.Type == TokenType.Comma || stream.Current.Type == TokenType.CloseFunctionBrace;

            return false;
        }
    }
}
