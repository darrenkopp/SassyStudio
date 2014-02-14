using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Compiler
{
    static class ReverseSearch
    {
        public static T Find<T>(ParseItem start, Func<T, bool> predicate) where T : ParseItem
        {
            if (start == null)
                return null;

            var current = start;
            while (current != null)
            {
                var candidate = current as T;
                if (candidate != null && predicate(candidate))
                {
                    return candidate;
                }
                else if (current is ISassStylesheet)
                {
                    var sheet = current as ISassStylesheet;
                    var match = Find(sheet.Children.OfType<T>(), start, predicate);
                    if (match != null)
                        return match;
                    
                    foreach (var import in sheet.Children.OfType<SassImportDirective>().Reverse())
                    {
                        match = FindInDocument<T>(import, predicate);
                        if (match != null)
                            return match;
                    }
                }
                else if (current is UserFunctionDefinition)
                {
                    // search argument definitions
                    var function = current as UserFunctionDefinition;

                    var match = Find(function.Arguments.Select(x => x.Variable).OfType<T>(), start, predicate);
                    if (match != null)
                        return match;
                }
                else if (current is MixinDefinition)
                {
                    // search argument definitions
                    var mixin = current as MixinDefinition;

                    var match = Find(mixin.Arguments.Select(x => x.Variable).OfType<T>(), start, predicate);
                    if (match != null)
                        return match;
                }
                
                // search for items defined in block scope
                if (current is BlockItem)
                {
                    var block = current as BlockItem;
                    var match = Find<T>(block.Children.OfType<T>(), start, predicate);
                    if (match != null)
                        return match;
                }

                current = current.Parent;
            }

            return null;
        }

        private static T Find<T>(IEnumerable<T> items, ParseItem start, Func<T, bool> predicate)
            where T : ParseItem
        {
            return items.Where(x => x.End < start.Start).FirstOrDefault(predicate);
        }

        private static T FindInDocument<T>(SassImportDirective import, Func<T, bool> predicate) 
            where T : ParseItem
        {
            if (import.Files.Count == 0) return null;
            var candidates = import.Files.Reverse()
                .Where(x => x.Document != null && x.Document.Stylesheet != null)
                .SelectMany(x => x.Document.Stylesheet.Children.Reverse());

            foreach (var candidate in candidates)
            {
                if (candidate is T)
                {
                    var element = candidate as T;
                    if (predicate(element))
                        return element;
                }
                else if (candidate is SassImportDirective)
                {
                    var nestedImport = candidate as SassImportDirective;
                    var match = FindInDocument<T>(nestedImport, predicate);
                    if (match != null)
                        return match;
                }
            }

            return null;
        }
    }
}
