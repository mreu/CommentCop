// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7004CodeFixProvider.cs" author="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlCommenter.Regions
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;

    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    /// <summary>
    /// The MR7004 Code fix provider class.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MR7004CodeFixProvider))]
    [Shared]
    public class MR7004CodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Apply text from #region. (MR7004)";

        /// <summary>
        /// Gets the fixable diagnostic ids.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MR7004EndregionMustHaveTheSameTextAsTheRegion.DiagnosticId7004);

        /// <summary>
        /// Get fix all provider.
        /// </summary>
        /// <returns>The fix all provider.</returns>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <summary>
        /// Register code fixes async.
        /// </summary>
        /// <param name="context">The code fix content.</param>
        /// <returns>The task.</returns>
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var texts = await Helper.RegionText.GetTextFromRegion(root, diagnosticSpan.Start);

            if (string.IsNullOrEmpty(texts?.Item1))
            {
                return;
            }

            // Find the type declaration identified by the diagnostic.
            ////var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            var identifierToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
            var trivia = identifierToken.LeadingTrivia.FirstOrDefault(x => x.SpanStart <= diagnosticSpan.Start && x.Span.End >= diagnosticSpan.Start);

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    Title,
                    cancellationToken => GetTransformedDocumentAsync(context.Document, root, trivia, texts.Item1, cancellationToken),
                    Title),
                diagnostic);
        }

        /// <summary>
        /// Get transformed document async.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="root">The syntax root.</param>
        /// <param name="identifierTrivia">The syntax token.</param>
        /// <param name="text">The text to insert/replace at #endregion.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task{Document}</returns>
        private static async Task<Document> GetTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            SyntaxTrivia identifierTrivia,
            string text,
            CancellationToken cancellationToken)
        {
            try
            {
                // ReSharper disable once UnusedVariable
                var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                var newTrivia = TriviaList(Trivia(EndRegionDirectiveTrivia(true)
                            .WithEndRegionKeyword(Token(TriviaList(), SyntaxKind.EndRegionKeyword, TriviaList(Space)))
                            .WithEndOfDirectiveToken(Token(TriviaList(PreprocessingMessage(text)), SyntaxKind.EndOfDirectiveToken, TriviaList(CarriageReturnLineFeed)))));

                return document.WithSyntaxRoot(root.ReplaceTrivia(identifierTrivia, newTrivia));
            }
            catch (Exception exp)
            {
                Debug.WriteLine($"{nameof(MR7004CodeFixProvider)} - Exception on {identifierTrivia} = {exp.Message}");

                return document;
            }
        }
    }
}
