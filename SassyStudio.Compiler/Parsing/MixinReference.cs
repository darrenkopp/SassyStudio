using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Editor;

namespace SassyStudio.Compiler.Parsing
{
    public class MixinReference : ComplexItem, IResolvableToken
    {
        readonly List<FunctionArgument> _Arguments = new List<FunctionArgument>(0);

        public AtRule Rule { get; protected set; }
        public MixinName Name { get; protected set; }
        public TokenItem OpenBrace { get; protected set; }
        public TokenItem CloseBrace { get; protected set; }
        public MixinContentBlock Content { get; protected set; }
        public TokenItem Semicolon { get; protected set; }
        public IReadOnlyCollection<FunctionArgument> Arguments { get { return _Arguments; } }

        public override bool IsUnclosed { get { return Semicolon == null || (OpenBrace != null && CloseBrace != null); } }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (AtRule.IsRule(text, stream, "include"))
            {
                Rule = AtRule.CreateParsed(itemFactory, text, stream);
                if (Rule != null)
                    Children.Add(Rule);

                Name = MixinName.CreateParsed(itemFactory, text, stream, SassClassifierType.MixinReference);
                if (Name != null)
                    Children.Add(Name);

                if (stream.Current.Type == TokenType.OpenFunctionBrace)
                {
                    OpenBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.FunctionBrace);

                    while (!IsTerminator(stream.Current.Type))
                    {
                        var argument = itemFactory.CreateSpecific<FunctionArgument>(this, text, stream);
                        if (argument != null && argument.Parse(itemFactory, text, stream))
                        {
                            _Arguments.Add(argument);
                            Children.Add(argument);
                        }
                        else
                        {
                            Children.AddCurrentAndAdvance(stream);
                        }
                    }
                }

                if (stream.Current.Type == TokenType.CloseFunctionBrace)
                    CloseBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.FunctionBrace);

                if (stream.Current.Type == TokenType.Semicolon)
                {
                    Semicolon = Children.AddCurrentAndAdvance(stream);
                }
                else if (stream.Current.Type == TokenType.OpenCurlyBrace)
                {

                    var content = new MixinContentBlock();
                    if (content.Parse(itemFactory, text, stream))
                    {
                        Content = content;
                        Children.Add(content);
                    }
                }
            }

            return Children.Count > 0;
        }

        public override void Freeze()
        {
            base.Freeze();
            _Arguments.TrimExcess();
        }

        static bool IsTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.CloseFunctionBrace:
                case TokenType.Semicolon:
                    return true;
                default:
                    return false;
            }
        }

        public ParseItem GetSourceToken()
        {
            return ReverseSearch.Find<MixinDefinition>(this, x => x.Name.Equals(Name));
        }
    }
}
