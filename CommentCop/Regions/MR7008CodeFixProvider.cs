﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7008CodeFixProvider.cs" author="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Regions
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
    /// The code fix provider.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MR7008CodeFixProvider))]
    [Shared]
    public class MR7008CodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Remove empty line(s). (MR7008)";

        /// <summary>
        /// Gets the fixable diagnostic ids.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MR7008NoEmptyLinePreceedingEndRegionKeyword.DiagnosticId7008);

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

                var idx = token.LeadingTrivia.IndexOfTrivia(diagnostic.Location.SourceSpan) - 1;

                if (idx < 0)
                {
                    return document;
                }

                var triviaList = SyntaxFactory.TriviaList(token.LeadingTrivia);

                if (triviaList[idx].IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    if (--idx < 0)
                    {
                        return document;
                    }
                }

                for (var ix = idx; ix >= 0; ix--)
                {
                    if (triviaList[ix].IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        triviaList = triviaList.RemoveAt(ix);
                        continue;
                    }

                    break;
                }

                var newtoken = token.WithLeadingTrivia(triviaList);

                return document.WithSyntaxRoot(root.ReplaceToken(token, newtoken));
            }
            catch (Exception exp)
            {
                Debug.WriteLine($"{nameof(MR7008CodeFixProvider)} - Exception on {diagnostic} = {exp.Message}");

                return document;
            }
        }
    }
}
