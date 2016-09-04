// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7010CodeFixProvider.cs" author="Michael Reukauff">
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

    /// <summary>
    /// The MR7010 Code fix provider class.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MR7010CodeFixProvider))]
    [Shared]
    public class MR7010CodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Insert empty line. (MR7010)";

        /// <summary>
        /// Gets the fixable diagnostic ids.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MR7010EmptyLineMustPreceedEndRegionKeyword.DiagnosticId7010);

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
            ////var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            ////var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    Title,
                    cancellationToken => GetTransformedDocumentAsync(context.Document, root, diagnostic, cancellationToken),
                    Title),
                diagnostic);
        }

        /// <summary>
        /// Get transformed document async.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="root">The syntax root.</param>
        /// <param name="diagnostic">The syntax token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task{Document}</returns>
        private static async Task<Document> GetTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            Diagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            try
            {
                // ReSharper disable once UnusedVariable
                var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                var token = root.FindToken(diagnostic.Location.SourceSpan.Start);

                if (!token.LeadingTrivia.Any(x => x.IsKind(SyntaxKind.EndRegionDirectiveTrivia)))
                {
                    return document;
                }

                var triviaList = SyntaxFactory.TriviaList(token.LeadingTrivia);

                var idx = token.LeadingTrivia.IndexOfTrivia(diagnostic.Location.SourceSpan) - 1;

                if (idx < 0)
                {
                    triviaList = triviaList.Insert(0, SyntaxFactory.CarriageReturnLineFeed);
                }
                else
                {
                    triviaList = triviaList.Insert(idx, SyntaxFactory.CarriageReturnLineFeed);
                }

                var newtoken = token.WithLeadingTrivia(triviaList);

                return document.WithSyntaxRoot(root.ReplaceToken(token, newtoken));
            }
            catch (Exception exp)
            {
                Debug.WriteLine($"{nameof(MR7010CodeFixProvider)} - Exception on {diagnostic} = {exp.Message}");

                return document;
            }
        }
    }
}
