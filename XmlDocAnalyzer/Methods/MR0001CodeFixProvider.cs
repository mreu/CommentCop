// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR0001CodeFixProvider.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Methods
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
    using Microsoft.CodeAnalysis.Formatting;

    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
    /// <summary>
    /// The xml doc code fix provider.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MR0001CodeFixProvider))]
    [Shared]
    public class MR0001CodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Insert XML documentation header (MR0001)";

        /// <summary>
        /// Gets the fixable diagnostic ids.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MR0001PublicMethodsMustHaveXMLComment.DiagnosticId, "MR0002", "MR0003", "MR0004", "MR0005", "SA1600");

        /// <summary>
        /// Get fix all provider.
        /// </summary>
        /// <returns>The fic all provider.</returns>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            // return WellKnownFixAllProviders.BatchFixer;
            return null;
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

            // Find the type declaration identified by the diagnostic.
            // var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            var identifierToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
            context.RegisterCodeFix(
                CodeAction.Create(
                    Title,
                    cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, root, identifierToken, cancellationToken),
                    Title),
                diagnostic);
        }

        /// <summary>
        /// Get transformed document async.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <param name="root">The syntax root.</param>
        /// <param name="identifierToken">The syntax token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task{Document}</returns>
        private static async Task<Document> GetTransformedDocumentAsync(
            Document document,
            Diagnostic diagnostic,
            SyntaxNode root,
            SyntaxToken identifierToken,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            return GetTransformedDocumentForMethodDeclaration(
                document,
                diagnostic,
                root,
                semanticModel,
                (MethodDeclarationSyntax)identifierToken.Parent,
                cancellationToken);
        }

        /// <summary>
        /// Get transformed document for method declaration.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <param name="root">The syntax root.</param>
        /// <param name="semanticModel">The semantic model.</param>
        /// <param name="methodDeclaration">The method declaration syntax.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The modified or unmodified document.</returns>
        private static Document GetTransformedDocumentForMethodDeclaration(
            Document document,
            Diagnostic diagnostic,
            SyntaxNode root,
            SemanticModel semanticModel,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            if (methodDeclaration.ExplicitInterfaceSpecifier == null && !methodDeclaration.Modifiers.Any(SyntaxKind.OverrideKeyword))
            {
                ISymbol declaredSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);
                if (declaredSymbol == null)
                {
                    return document;
                }
            }

            return InsertInheritdocComment(document, diagnostic, root, methodDeclaration, cancellationToken);
        }

        /// <summary>
        /// Insert xml comment.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="diagnostic">The diagnostic.</param>
        /// <param name="root">The root syntaxnode.</param>
        /// <param name="syntaxNode">The syntax node.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The modified or unmodified document.</returns>
        private static Document InsertInheritdocComment(
            Document document,
            Diagnostic diagnostic,
            SyntaxNode root,
            SyntaxNode syntaxNode,
            CancellationToken cancellationToken)
        {
            var leadingTrivia = syntaxNode.GetLeadingTrivia();
            var insertionIndex = leadingTrivia.Count;
            while (insertionIndex > 0 && !leadingTrivia[insertionIndex - 1].HasBuiltinEndLine())
            {
                insertionIndex--;
            }

            var method = syntaxNode as MethodDeclarationSyntax;
            if (method != null)
            {
                var xmldoc = GetSummary(Convert.Method(method.Identifier.ValueText));
                var xmldoc1 = Trivia(xmldoc);

                var newLeadingTrivia = leadingTrivia.Insert(insertionIndex, xmldoc1);
                var newElement = syntaxNode.WithLeadingTrivia(newLeadingTrivia);

                return document.WithSyntaxRoot(root.ReplaceNode(syntaxNode, newElement));
            }

            return document;
        }

        /// <summary>
        /// Get summary.
        /// </summary>
        /// <returns>The syntax list.</returns>
        private static DocumentationCommentTriviaSyntax GetSummary(string name)
        {
            const string summary = "summary";

            var summaryStart = XmlElementStartTag(XmlName(Identifier(summary)))
                .WithLessThanToken(Token(SyntaxKind.LessThanToken))
                .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken)).NormalizeWhitespace();

            var summaryEnd = XmlElementEndTag(XmlName(Identifier(summary))).NormalizeWhitespace()
                .WithLessThanSlashToken(Token(SyntaxKind.LessThanSlashToken))
                .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken));

            var summaryText = SingletonList<XmlNodeSyntax>(
                XmlText().NormalizeWhitespace()
                    .WithTextTokens(
                        TokenList(
                            XmlTextNewLine(TriviaList(), "\n", "\n", TriviaList()).NormalizeWhitespace(),
                            XmlTextLiteral(
                                TriviaList(DocumentationCommentExterior("///")),
                                " " + name,
                                " " + name,
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

            return DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                List(new XmlNodeSyntax[] { xmlComment, summaryElement, newLine }));
        }
    }
}