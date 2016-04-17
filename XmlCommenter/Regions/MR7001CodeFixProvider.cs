﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7001CodeFixProvider.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Methods
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

    using XmlDocAnalyzer.Experimental;

    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    using Convert = XmlDocAnalyzer.Convert;

    /// <summary>
    /// The xml doc code fix provider.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MR1006CodeFixProvider))]
    [Shared]
    public class MR7001CodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Make description of region beginning with uppercase character (MR7001)";

        /// <summary>
        /// Gets the fixable diagnostic ids.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MR7001DescriptionInRegionsMustBeginWithUppercaseCharacter.DiagnosticId7001);

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
            //// var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

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

                var region = identifierToken.LeadingTrivia.FirstOrDefault(x => x.IsKind(SyntaxKind.RegionDirectiveTrivia));

                var token = region.GetStructure() as RegionDirectiveTriviaSyntax;

                var trivias = token?.EndOfDirectiveToken.GetAllTrivia();

                var trivia = trivias?.FirstOrDefault(x => x.IsKind(SyntaxKind.PreprocessingMessageTrivia));

                if (trivia != null)
                {
                    var text = trivia.ToString();
                    var words = text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    for (var ix = 0; ix < words.Length; ix++)
                    {
                        if (!char.IsLetter(words[ix][0]))
                        {
                            continue;
                        }

                        if (!char.IsUpper(words[ix][0]))
                        {
                            words[ix] = words[ix].Substring(0, 1).ToUpper() + words[ix].Substring(1);
                        }
                    }

                    var newText = string.Join(" ", words);

                    var newTrivia = PreprocessingMessage(newText);

                    return document.WithSyntaxRoot(root.ReplaceTrivia(trivia.Value, newTrivia));
                }

                return document;
            }
            catch (Exception exp)
            {
                Debug.WriteLine($"{nameof(MR7001CodeFixProvider)} - Exception on {identifierToken} = {exp.Message}");

                return document;
            }
        }
    }
}