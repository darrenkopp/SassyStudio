using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public class ForLoopDirective : ControlDirective
    {
        public VariableName Variable { get; protected set; }
        public TokenItem FromKeyword { get; protected set; }
        public ParseItem FromValue { get; protected set; }
        public TokenItem ToKeyword { get; protected set; }
        public TokenItem ThroughKeyword { get; protected set; }
        public ParseItem RangeValue { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "for"))
            {
                ParseRule(itemFactory, text, stream);

                var variable = new VariableName(SassClassifierType.VariableDefinition);
                if (variable.Parse(itemFactory, text, stream))
                {
                    Variable = variable;
                    Children.Add(Variable);
                }

                if (IsKeyword(text, stream, "from"))
                {
                    FromKeyword = Children.AddCurrentAndAdvance(stream, SassClassifierType.Keyword);

                    if (stream.Current.Type == TokenType.Number)
                        FromValue = Children.AddCurrentAndAdvance(stream, SassClassifierType.Number);
                }

                bool isToKeyword = IsKeyword(text, stream, "to");
                bool isThroughKeyword = !isToKeyword && IsKeyword(text, stream, "through");

                if (isToKeyword || isThroughKeyword)
                {
                    if (isToKeyword)
                        ToKeyword = Children.AddCurrentAndAdvance(stream, SassClassifierType.Keyword);

                    if (isThroughKeyword)
                        ThroughKeyword = Children.AddCurrentAndAdvance(stream, SassClassifierType.Keyword);

                    if (stream.Current.Type == TokenType.Number)
                        RangeValue = Children.AddCurrentAndAdvance(stream, SassClassifierType.Number);
                }

                ParseBody(itemFactory, text, stream);
            }

            return Children.Count > 0;
        }

        private bool IsKeyword(ITextProvider text, ITokenStream stream, string keyword)
        {
            return text.CompareOrdinal(stream.Current.Start, keyword);
        }

        public override IEnumerable<VariableName> GetDefinedVariables(int position)
        {
            var variables = base.GetDefinedVariables(position);
            if (Body != null && position > Body.Start)
                variables = variables.Concat(new[] { Variable });

            return variables;
        }

        static bool IsForStatementTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.OpenCurlyBrace:
                    return true;
                default:
                    return false;
            }
        }
    }
}
