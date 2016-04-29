﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7002CodeFixProvider.cs" author="Michael Reukauff">
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
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    /// <summary>
    /// The xml doc code fix provider.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MR7002CodeFixProvider))]
    [Shared]
    public class MR7002CodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Apply text from #region. (MR7002)";

        /// <summary>
        /// Gets the fixable diagnostic ids.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MR7002EndregionsMustHaveDescription.DiagnosticId7002);

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

            // Register a code action that will invoke the fix.
            var identifierToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
            context.RegisterCodeFix(
                CodeAction.Create(
                    Title,
                    cancellationToken => GetTransformedDocumentAsync(context.Document, root, identifierToken, texts.Item1, cancellationToken),
                    Title),
                diagnostic);
        }

        /// <summary>
        /// Get transformed document async.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="root">The syntax root.</param>
        /// <param name="identifierToken">The syntax token.</param>
        /// <param name="text">The text to insert/replace at #endregion.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task{Document}</returns>
        private static async Task<Document> GetTransformedDocumentAsync(
            Document document,
            SyntaxNode root,
            SyntaxToken identifierToken,
            string text,
            CancellationToken cancellationToken)
        {
            try
            {
                ////var texts = Helper.RegionText.GetTextFromRegion(root, spanStart);

                // ReSharper disable once UnusedVariable
                var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                var region = identifierToken.LeadingTrivia.FirstOrDefault(x => x.IsKind(SyntaxKind.EndRegionDirectiveTrivia));

                var token = region.GetStructure() as EndRegionDirectiveTriviaSyntax;

                var oldToken = token?.EndOfDirectiveToken;

                if (oldToken == null)
                {
                    return document;
                }

                var newToken = Token(TriviaList(PreprocessingMessage(' ' + text)), SyntaxKind.EndOfDirectiveToken, TriviaList(LineFeed));

                return document.WithSyntaxRoot(root.ReplaceToken(oldToken.Value, newToken));
            }
            catch (Exception exp)
            {
                Debug.WriteLine($"{nameof(MR7002CodeFixProvider)} - Exception on {identifierToken} = {exp.Message}");

                return document;
            }
        }
    }
}
