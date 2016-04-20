﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR2001-2002CodeFixProvider.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlCommenter.Interfaces
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

    using Convert = XmlCommenter.Convert;

    /// <summary>
    /// The xml doc code fix provider.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MR2001_2002CodeFixProvider))]
    [Shared]
    public class MR2001_2002CodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Insert XML documentation header (MR2001 - MR2002)";

        /// <summary>
        /// Gets the fixable diagnostic ids.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MR2001PublicInterfacesMustHaveXMLComment.DiagnosticId, MR2002InternalInterfacesMustHaveXMLComment.DiagnosticId);

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

            var declaration = (InterfaceDeclarationSyntax)identifierToken.Parent;
            var leadingTrivia = declaration.GetLeadingTrivia();
            var insertionIndex = leadingTrivia.Count;
            while (insertionIndex > 0 && !leadingTrivia[insertionIndex - 1].HasBuiltinEndLine())
            {
                insertionIndex--;
            }

            var xmldoc = await Task.Run(() => GetSummary(declaration), cancellationToken);

            var newLeadingTrivia = leadingTrivia.Insert(insertionIndex, Trivia(xmldoc));
            var newElement = declaration.WithLeadingTrivia(newLeadingTrivia);

            return document.WithSyntaxRoot(root.ReplaceNode(declaration, newElement));
            }
            catch (Exception exp)
            {
                Debug.WriteLine($"{nameof(MR2001_2002CodeFixProvider)} - Exception on {identifierToken} = {exp.Message}");

                return document;
            }
        }

        /// <summary>
        /// Get summary.
        /// </summary>
        /// <param name="theSyntaxNode">The syntax node to add the summary.</param>
        /// <returns>The syntax list.</returns>
        private static DocumentationCommentTriviaSyntax GetSummary(InterfaceDeclarationSyntax theSyntaxNode)
        {
            var summaryStart = XmlElementStartTag(XmlName(Identifier(Constants.Summary)))
                .WithLessThanToken(Token(SyntaxKind.LessThanToken))
                .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken)).NormalizeWhitespace();

            var summaryEnd = XmlElementEndTag(XmlName(Identifier(Constants.Summary))).NormalizeWhitespace()
                .WithLessThanSlashToken(Token(SyntaxKind.LessThanSlashToken))
                .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken));

            var summaryComment = " " + Convert.Interface(theSyntaxNode.Identifier.ValueText);

            var summaryText = SingletonList<XmlNodeSyntax>(
                XmlText().NormalizeWhitespace()
                    .WithTextTokens(
                        TokenList(
                            XmlTextNewLine(TriviaList(), Environment.NewLine, Environment.NewLine, TriviaList()).NormalizeWhitespace(),
                            XmlTextLiteral(
                                TriviaList(DocumentationCommentExterior("///")),
                                summaryComment,
                                summaryComment,
                                TriviaList()).NormalizeWhitespace(),
                            XmlTextNewLine(TriviaList(), Environment.NewLine, Environment.NewLine, TriviaList()).NormalizeWhitespace(),
                            XmlTextLiteral(
                                TriviaList(DocumentationCommentExterior("///")),
                                " ",
                                " ",
                                TriviaList()))).NormalizeWhitespace());

            var xmlComment = XmlText()
                .WithTextTokens(
                    TokenList(
                        XmlTextLiteral(
                            TriviaList(DocumentationCommentExterior("///")),
                            " ",
                            " ",
                            TriviaList()))).NormalizeWhitespace();

            var newLine = XmlText().WithTextTokens(TokenList(XmlTextNewLine(TriviaList(), Environment.NewLine, Environment.NewLine, TriviaList()))).NormalizeWhitespace();

            var summaryElement = XmlElement(summaryStart, summaryEnd).WithContent(summaryText);

            var list = List(new XmlNodeSyntax[] { xmlComment, summaryElement, newLine });

            // Add typeparams comments
            if (theSyntaxNode.TypeParameterList != null)
            {
                if (theSyntaxNode.TypeParameterList.Parameters.Any())
                {
                    foreach (var parameter in theSyntaxNode.TypeParameterList.Parameters)
                    {
                        list = list.AddRange(
                            List(
                                new XmlNodeSyntax[]
                                {
                                xmlComment,

                                XmlElement(
                                    XmlElementStartTag(XmlName(Identifier("typeparam")))
                                    .WithAttributes(
                                        SingletonList<XmlAttributeSyntax>(
                                            XmlNameAttribute(
                                                XmlName(Identifier(TriviaList(Space), "name", TriviaList())),
                                                Token(SyntaxKind.DoubleQuoteToken),
                                                IdentifierName(parameter.Identifier.ValueText),
                                                Token(SyntaxKind.DoubleQuoteToken)))),
                                    XmlElementEndTag(XmlName(Identifier("typeparam"))))
                                    .WithContent(
                                        SingletonList<XmlNodeSyntax>(
                                            XmlText()
                                    .WithTextTokens(
                                        TokenList(
                                            XmlTextLiteral(
                                                TriviaList(),
                                                Convert.Parameter(parameter.Identifier.ValueText, string.Empty),
                                                "comment",
                                                TriviaList()))))),

                                newLine
                                }));
                    }
                }
            }

            return DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);
        }
    }
}
