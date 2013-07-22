using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Scss
{
    static class ScssWellKnownFunctionNames
    {
        public static IEnumerable<string> Names
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
                yield return @"adjust-color";
                yield return @"scale-color";
                yield return @"change-color";
                yield return @"ie-hex-str";

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
                yield return @"type-of";
                yield return "unit";
                yield return "unitless";
                yield return "comparable";
            }
        }
    }
}
