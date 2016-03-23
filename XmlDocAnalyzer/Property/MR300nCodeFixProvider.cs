// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR300nCodeFixProvider.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Property
{
    using System.Collections.Immutable;
    using System.Composition;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MR300nCodeFixProvider))]
    [Shared]
    public class MR300nCodeFixProvider : CodeFixProvider
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
                    MR3001PublicPropertiesMustHaveXMLComment.DiagnosticId,
                    MR3002InternalPropertiesMustHaveXMLComment.DiagnosticId,
                    MR3003InternalProtectedPropertiesMustHaveXMLComment.DiagnosticId,
                    MR3004ProtectedPropertiesMustHaveXMLComment.DiagnosticId,
                    MR3005PrivatePropertiesMustHaveXMLComment.DiagnosticId);

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
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            return GetTransformedDocumentForMethodDeclaration(
                document,
                root,
                semanticModel,
                (PropertyDeclarationSyntax)identifierToken.Parent,
                cancellationToken);
        }

        /// <summary>
        /// Get transformed document for method declaration.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="root">The syntax root.</param>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="declaration">The method declaration syntax.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The modified or unmodified document.</returns>
        private static Document GetTransformedDocumentForMethodDeclaration(
            Document document,
            SyntaxNode root,
            SemanticModel semanticModel,
            PropertyDeclarationSyntax declaration,
            CancellationToken cancellationToken)
        {
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

            var xmldoc = GetSummary(declaration);
            var xmldoc1 = Trivia(xmldoc);

            var newLeadingTrivia = leadingTrivia.Insert(insertionIndex, xmldoc1);
            var newElement = declaration.WithLeadingTrivia(newLeadingTrivia);

            return document.WithSyntaxRoot(root.ReplaceNode(declaration, newElement));
        }

        /// <summary>
        /// Get summary.
        /// </summary>
        /// <param name="theSyntax">The property to add to the summary.</param>
        /// <returns>The syntax list.</returns>
        private static DocumentationCommentTriviaSyntax GetSummary(PropertyDeclarationSyntax theSyntax)
        {
            const string summary = "summary";

            var summaryStart = XmlElementStartTag(XmlName(Identifier(summary)))
                .WithLessThanToken(Token(SyntaxKind.LessThanToken))
                .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken)).NormalizeWhitespace();

            var summaryEnd = XmlElementEndTag(XmlName(Identifier(summary))).NormalizeWhitespace()
                .WithLessThanSlashToken(Token(SyntaxKind.LessThanSlashToken))
                .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken));

            var accessors = theSyntax.AccessorList.ChildNodes().OfType<AccessorDeclarationSyntax>();

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

            var isBool = (theSyntax.Type as PredefinedTypeSyntax)?.Keyword.Kind() == SyntaxKind.BoolKeyword;

            var summaryComment = " " + Convert.Property(theSyntax.Identifier.ValueText, hasGetter, hasSetter, isBool);

            var summaryText = SingletonList<XmlNodeSyntax>(
                XmlText().NormalizeWhitespace()
                    .WithTextTokens(
                        TokenList(
                            XmlTextNewLine(TriviaList(), "\n", "\n", TriviaList()).NormalizeWhitespace(),
                            XmlTextLiteral(
                                TriviaList(DocumentationCommentExterior("///")),
                                summaryComment,
                                summaryComment,
                                TriviaList()).NormalizeWhitespace(),
                            XmlTextNewLine(TriviaList(), "\n", "\n", TriviaList()).NormalizeWhitespace(),
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

            var newLine = XmlText().WithTextTokens(TokenList(XmlTextNewLine(TriviaList(), "\n", "\n", TriviaList()))).NormalizeWhitespace();

            var summaryElement = XmlElement(summaryStart, summaryEnd).WithContent(summaryText);

            var list = List(new XmlNodeSyntax[] { xmlComment, summaryElement, newLine });

            // Add exceptions comments
            var throws = theSyntax.DescendantNodes().OfType<ThrowStatementSyntax>();
            foreach (var syntax in throws)
            {
                if (syntax.ChildNodes().OfType<ObjectCreationExpressionSyntax>().Any())
                {
                    var identifier = syntax.DescendantNodes().OfType<IdentifierNameSyntax>().First();
                    var argumentList = syntax.DescendantNodes().OfType<ArgumentListSyntax>().First();
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
                                    XmlElementEndTag(XmlName(Identifier("param"))))
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