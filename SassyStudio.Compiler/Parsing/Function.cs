using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SassyStudio.Compiler.Lexing;

namespace SassyStudio.Compiler.Parsing
{
    public class Function : ComplexItem
    {
        static readonly ICollection<string> WellKnownFunctions = new HashSet<string>(BuiltInFunctionNames);

        readonly SassClassifierType FunctionClassifierType;
        readonly List<FunctionArgument> _Arguments = new List<FunctionArgument>(0);
        public Function(SassClassifierType classifierType = SassClassifierType.SystemFunction)
        {
            FunctionClassifierType = classifierType;
        }

        public TokenItem Name { get; protected set; }
        public TokenItem OpenBrace { get; protected set; }
        public IList<FunctionArgument> Arguments { get { return _Arguments; } }
        public TokenItem CloseBrace { get; protected set; }

        public override bool Parse(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            if (IsFunctionCall(stream) && IsFunctionNameValid(text, stream))
            {
                Name = Children.AddCurrentAndAdvance(stream, FunctionClassifierType);

                if (stream.Current.Type == TokenType.OpenFunctionBrace)
                    OpenBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.FunctionBrace);

                ParseArguments(itemFactory, text, stream);

                if (OpenBrace != null && stream.Current.Type == TokenType.CloseFunctionBrace)
                    CloseBrace = Children.AddCurrentAndAdvance(stream, SassClassifierType.FunctionBrace);
            }

            return Children.Count > 0;
        }

        protected virtual void ParseArguments(IItemFactory itemFactory, ITextProvider text, ITokenStream stream)
        {
            while (!IsTerminator(stream.Current.Type))
            {
                var argument = itemFactory.CreateSpecific<FunctionArgument>(this, text, stream);
                if (argument == null || !argument.Parse(itemFactory, text, stream))
                    break;

                Arguments.Add(argument);
                Children.Add(argument);
            }
        }

        public override void Freeze()
        {
            _Arguments.TrimExcess();
            base.Freeze();
        }

        protected virtual bool IsFunctionNameValid(ITextProvider text, ITokenStream stream)
        {
            return true;
        }

        static bool IsFunctionCall(ITokenStream stream)
        {
            return stream.Current.Type == TokenType.Function || (stream.Current.Type == TokenType.Identifier && stream.Peek(1).Type == TokenType.OpenFunctionBrace);
        }

        static bool IsTerminator(TokenType type)
        {
            switch (type)
            {
                case TokenType.EndOfFile:
                case TokenType.CloseFunctionBrace:
                case TokenType.Comma:
                case TokenType.OpenCurlyBrace:
                case TokenType.Semicolon:
                    return true;
            }

            return false;
        }

        public static bool IsWellKnownFunction(ITextProvider text, ITokenStream stream)
        {
            if (stream.Current.Type == TokenType.Function || stream.Current.Type == TokenType.Identifier)
            {
                var functionName = text.GetText(stream.Current.Start, stream.Current.Length);
                return WellKnownFunctions.Contains(functionName);
            }

            return false;
        }

        public static IEnumerable<string> BuiltInFunctionNames
        {
            get
            {
                // rgb
                yield return "rgb";
                yield return "rgba";
                yield return "red";
                yield return "green";
                yield return "blue";
                yield return "mix";

                // hsl
                yield return "hsl";
                yield return "hsla";
                yield return "hue";
                yield return "saturation";
                yield return "lightness";
                yield return "adjust-hue";
                yield return "lighten";
                yield return "darken";
                yield return "saturate";
                yield return "desaturate";
                yield return "grayscale";
                yield return "complement";
                yield return "invert";

                // opacity
                yield return "alpha";
                yield return "opacify";
                yield return "transparentize";

                // other color functions
                yield return "adjust-color";
                yield return "scale-color";
                yield return "change-color";
                yield return "ie-hex-str";

                // string functions
                yield return "unquote";
                yield return "quote";

                // number functions
                yield return "percentage";
                yield return "round";
                yield return "ceil";
                yield return "floor";
                yield return "abs";
                yield return "min";
                yield return "max";

                // list functions
                yield return "length";
                yield return "nth";
                yield return "join";
                yield return "append";
                yield return "zip";
                yield return "index";

                // introspection functions
                yield return "type-of";
                yield return "unit";
                yield return "unitless";
                yield return "comparable";
            }
        }
    }
}
