// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR3001-3005CodeFixProvider.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Property
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

    using Convert = XmlDocAnalyzer.Convert;

    /// <summary>
    /// The xml doc code fix provider.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MR3001_3005CodeFixProvider))]
    [Shared]
    public class MR3001_3005CodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Insert XML documentation header (MR3001 - MR3005)";

        /// <summary>
        /// Gets the fixable diagnostic ids.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            =>
                ImmutableArray.Create(
                    MR3001_3005PropertiesMustHaveXMLComment.DiagnosticId3001,
                    MR3001_3005PropertiesMustHaveXMLComment.DiagnosticId3002,
                    MR3001_3005PropertiesMustHaveXMLComment.DiagnosticId3003,
                    MR3001_3005PropertiesMustHaveXMLComment.DiagnosticId3004,
                    MR3001_3005PropertiesMustHaveXMLComment.DiagnosticId3005);

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
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var declaration = (PropertyDeclarationSyntax)identifierToken.Parent;
            if (declaration.ExplicitInterfaceSpecifier == null && !declaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(declaration, cancellationToken);
                if (declaredSymbol == null)
                {
                    return document;
                }
            }

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
                Debug.WriteLine($"{nameof(MR3001_3005CodeFixProvider)} - Exception on {identifierToken} = {exp.Message}");

                return document;
            }
        }

        /// <summary>
        /// Get summary.
        /// </summary>
        /// <param name="theSyntaxNode">The syntax node to add the summary.</param>
        /// <returns>The syntax list.</returns>
        private static DocumentationCommentTriviaSyntax GetSummary(PropertyDeclarationSyntax theSyntaxNode)
        {
            var summaryStart = XmlElementStartTag(XmlName(Identifier(Constants.Summary)))
                .WithLessThanToken(Token(SyntaxKind.LessThanToken))
                .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken)).NormalizeWhitespace();

            var summaryEnd = XmlElementEndTag(XmlName(Identifier(Constants.Summary))).NormalizeWhitespace()
                .WithLessThanSlashToken(Token(SyntaxKind.LessThanSlashToken))
                .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken));

            var accessors = theSyntaxNode.AccessorList.ChildNodes().OfType<AccessorDeclarationSyntax>();

            var hasGetter = false;
            var hasSetter = false;
            foreach (var accessor in accessors)
            {
                if (accessor.Kind() == SyntaxKind.GetAccessorDeclaration)
                {
                    if (!accessor.Modifiers.Any(SyntaxKind.PrivateKeyword))
                    {
                        hasGetter = true;
                    }

                    continue;
                }

                if (accessor.Kind() == SyntaxKind.SetAccessorDeclaration)
                {
                    if (!accessor.Modifiers.Any(SyntaxKind.PrivateKeyword))
                    {
                        hasSetter = true;
                    }
                }
            }

            var isBool = (theSyntaxNode.Type as PredefinedTypeSyntax)?.Keyword.Kind() == SyntaxKind.BoolKeyword;

            var summaryComment = " " + Convert.Property(theSyntaxNode.Identifier.ValueText, hasGetter, hasSetter, isBool);

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

            // Add exceptions comments
            var throws = theSyntaxNode.DescendantNodes().OfType<ThrowStatementSyntax>();
            foreach (var syntax in throws)
            {
                if (syntax.ChildNodes().OfType<ObjectCreationExpressionSyntax>().Any())
                {
                    var identifier = syntax.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
                    var argumentList = syntax.DescendantNodes().OfType<ArgumentListSyntax>().FirstOrDefault();
                    var parms = argumentList.DescendantTokens().Where(x => x.IsKind(SyntaxKind.StringLiteralToken)).ToList();
                    var parmText = string.Empty;

                    if (parms.Any())
                    {
                        parmText = parms.Last().ValueText;
                    }

                    list = list.AddRange(
                        List(
                            new XmlNodeSyntax[]
                            {
                                xmlComment,

                                XmlElement(
                                    XmlElementStartTag(XmlName(Identifier("exception")))
                                    .WithAttributes(
                                        SingletonList<XmlAttributeSyntax>(
                                            XmlNameAttribute(
                                                XmlName(Identifier(TriviaList(Space), "cref", TriviaList())),
                                                Token(SyntaxKind.DoubleQuoteToken),
                                                IdentifierName(identifier.Identifier.ValueText),
                                                Token(SyntaxKind.DoubleQuoteToken)))),
                                    XmlElementEndTag(XmlName(Identifier("exception"))))
                                    .WithContent(
                                        SingletonList<XmlNodeSyntax>(
                                            XmlText()
                                    .WithTextTokens(
                                        TokenList(
                                            XmlTextLiteral(
                                                TriviaList(),
                                                parmText,
                                                "comment",
                                                TriviaList()))))),

                                newLine
                            }));
                }
            }

            return DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);
        }
    }
}