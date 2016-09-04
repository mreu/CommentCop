// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7007CodeFixProvider.cs" author="Michael Reukauff">
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
    /// The MR7007 Code fix provider class.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MR7007CodeFixProvider))]
    [Shared]
    public class MR7007CodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Remove empty line(s). (MR7007)";

        /// <summary>
        /// Gets the fixable diagnostic ids.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MR7007NoEmptyLineFollowingRegionKeyword.DiagnosticId7007);

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
            var identifierToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
            context.RegisterCodeFix(
                CodeAction.Create(
                    Title,
                    cancellationToken => GetTransformedDocumentAsync(context.Document, root, identifierToken, cancellationToken),
                    Title),
                diagnostic);
        }

        /// <summary>
        /// Get transformed document async.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="root">The syntax root.</param>
        /// <param name="identifierToken">The syntax token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task{Document}</returns>
        private static async Task<Document> GetTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            SyntaxToken identifierToken,
            CancellationToken cancellationToken)
        {
            try
            {
                // ReSharper disable once UnusedVariable
                var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                if (!identifierToken.LeadingTrivia.Any(x => x.IsKind(SyntaxKind.RegionDirectiveTrivia)))
                {
                    return document;
                }

                var triviaList = SyntaxFactory.TriviaList();

                var remove = false;
                foreach (var trivia in identifierToken.LeadingTrivia)
                {
                    if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        if (remove)
                        {
                            continue;
                        }
                    }

                    remove = trivia.IsKind(SyntaxKind.RegionDirectiveTrivia);

                    triviaList = triviaList.Add(trivia);
                }

                var newtoken = identifierToken.WithLeadingTrivia(triviaList);

                return document.WithSyntaxRoot(root.ReplaceToken(identifierToken, newtoken));
            }
            catch (Exception exp)
            {
                Debug.WriteLine($"{nameof(MR7007CodeFixProvider)} - Exception on {identifierToken} = {exp.Message}");

                return document;
            }
        }
    }
}
