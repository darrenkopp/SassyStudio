using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    class SassCompletionSource : ICompletionSource
    {
        readonly ISassEditor Editor;
        readonly IIntellisenseCache Cache;
        readonly IIntellisenseManager IntellisenseManager;
        readonly ICompletionCompiler Compiler;
        readonly ITextStructureNavigatorSelectorService TextNavigator;

        public SassCompletionSource(ISassEditor editor, IIntellisenseManager intellisenseManager, ICompletionCompiler compiler, ITextStructureNavigatorSelectorService textNavigator)
        {
            Editor = editor;
            Cache = intellisenseManager.Get(editor.Document);
            IntellisenseManager = intellisenseManager;
            Compiler = compiler;
            TextNavigator = textNavigator;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            // if we don't have a stylesheet for the document yet, there isn't much we can do
            var stylesheet = Editor.Document.Stylesheet;
            if (stylesheet == null)
                return;

            // signal to cache that we are in an intellisense session and thus we do not want
            // to update the cache until we are done

            var span = FindTokenSpanAtPosition(session); 
            var position = span.GetSpan(session.TextView.TextSnapshot).Start.Position;
            var context = CreateCompletionContext(stylesheet, position);
            if (IsInvalidCompletionContext(context))
                return;

            var types = CalculateApplicableContexts(context);

            var values = (
                from type in types
                from provider in IntellisenseManager.ValueProvidersFor(type)
                from value in provider.GetCompletions(type, context)
                select value
            );

            var set = Compiler.Compile(span, values);
            if (set.Completions.Count > 0)
                completionSets.Add(set);
        }

        private bool IsInvalidCompletionContext(ICompletionContext context)
        {
            if (context.Current == null || context.Current is Comment)
                return true;

            var token = context.Current as TokenItem;
            if (token != null && (token.SourceType == TokenType.String || token.SourceType == TokenType.BadString))
                return true;

            return false;
        }

        private IEnumerable<SassCompletionContextType> CalculateApplicableContexts(ICompletionContext context)
        {
            // it's likely that multiple value providers will return the same context type
            // so wrap in a set so we only have to process that type once
            return new HashSet<SassCompletionContextType>(
                IntellisenseManager.ContextProviders.SelectMany(x => x.GetContext(context.Current, context.Position))
            );
        }

        private ICompletionContext CreateCompletionContext(ISassStylesheet stylesheet, int position)
        {
            var current = stylesheet.Children.FindItemContainingPosition(Math.Max(0, position-1));
            if (current != null && !current.IsUnclosed)
                current = stylesheet.Children.FindItemContainingPosition(position);

            current = current ?? (stylesheet as Stylesheet);

            return new CompletionContext
            {
                Current = current,
                Position = position,
                Cache = Cache
            };
        }

        private ITrackingSpan FindTokenSpanAtPosition(ICompletionSession session)
        {
            var navigator = TextNavigator.GetTextStructureNavigator(session.TextView.TextBuffer);
            var position = (session.TextView.Caret.Position.BufferPosition) - 1;
            var extent = navigator.GetExtentOfWord(position);

            return position.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
        }

        public void Dispose()
        {
        }

        class CompletionContext : ICompletionContext
        {
            public IIntellisenseCache Cache { get; internal set; }

            public ParseItem Current { get; internal set; }

            public int Position { get; internal set; }
        }
    }

    [Export(typeof(ICompletionSourceProvider))]
    [Name("SCSS Completion Source Provider"), Order(Before = "Default")]
    [ContentType(ScssContentTypeDefinition.ScssContentType)]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    class SassCompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        ITextStructureNavigatorSelectorService TextNavigator { get; set; }

        [Import]
        ISassEditorManager EditorManager { get; set; }

        [Import]
        ICompletionCompiler CompletionCompiler { get; set; }

        [Import]
        IIntellisenseManager IntellisenseManager { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => CreateCompletionSource(textBuffer));
        }

        private SassCompletionSource CreateCompletionSource(ITextBuffer buffer)
        {
            var editor = EditorManager.Get(buffer);

            return new SassCompletionSource(editor, IntellisenseManager, CompletionCompiler, TextNavigator);
        }
    }
}
