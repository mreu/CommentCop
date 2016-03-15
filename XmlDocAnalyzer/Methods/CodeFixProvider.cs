// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CodeFixProvider.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Methods
{
    using System;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XmlDocAnalyzerCodeFixProvider)), Shared]
    public class XmlDocAnalyzerCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Insert XML documentation header (MR0001)";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MR0001.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            // return WellKnownFixAllProviders.BatchFixer;
            return null;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            var identifierToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
            context.RegisterCodeFix(
                CodeAction.Create(
                    Title,
                    cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, root, identifierToken, cancellationToken),
                    Title),
                diagnostic);
        }

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
            SyntaxTriviaList leadingTrivia = syntaxNode.GetLeadingTrivia();
            int insertionIndex = leadingTrivia.Count;
            while (insertionIndex > 0 && !leadingTrivia[insertionIndex - 1].HasBuiltinEndLine())
            {
                insertionIndex--;
            }

            string newLineText = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.NewLine, LanguageNames.CSharp);

            var code = XmlEmptyElement(XmlName("Test text"));
            var codeList = new SyntaxList<XmlNodeSyntax> { code };
            var documentationComment = DocumentationCommentTrivia(SyntaxKind.MultiLineDocumentationCommentTrivia)
                .WithLeadingTrivia(DocumentationCommentExterior("/// <summary>"))
                .WithTrailingTrivia(EndOfLine(newLineText));

            var node = DocumentationCommentTrivia(
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SingletonList<XmlNodeSyntax>(
                    XmlElement(
                        XmlElementStartTag(XmlName("summary")),
                        SingletonList<XmlNodeSyntax>(
                            XmlText(
                                TokenList(
                                    new SyntaxToken[]
                                    {
                                        XmlTextLiteral(TriviaList(), "summary text.", "summary text.", TriviaList()),
                                        XmlTextNewLine(TriviaList(), "\n", "\n", TriviaList())
                                    }))),
                        XmlElementEndTag(XmlName("summary")))))
                .WithLeadingTrivia(ElasticMarker, DocumentationCommentExterior("/// ")).NormalizeWhitespace();

            var xmldoc = GetSummary();
            var doc = DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, xmldoc);

            var xmldoc2 = GetXmlDoc();

            var trivia = Trivia(doc);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(insertionIndex, xmldoc2);
            SyntaxNode newElement = syntaxNode.WithLeadingTrivia(newLeadingTrivia);

            return document.WithSyntaxRoot(root.ReplaceNode(syntaxNode, newElement));
        }

        private static SyntaxTrivia GetXmlDoc()
        {
            var code = "/// <summary>" + Environment.NewLine +
                       "/// Summary text." + Environment.NewLine +
                       "/// </summary>" + Environment.NewLine +
                       "/// <param name=\"x\">The parm.</param>" + Environment.NewLine +
                       "/// <returns>Return text</returns>" + Environment.NewLine;

            var comment = Comment(code).WithAdditionalAnnotations(SyntaxAnnotation.ElasticAnnotation);

            return comment;
        }

        private static SyntaxList<XmlNodeSyntax> GetSummary()
        {
            return List(
                new XmlNodeSyntax[]
                {
                    XmlText()
                        .WithTextTokens(
                            TokenList(
                                XmlTextLiteral(
                                    TriviaList(DocumentationCommentExterior("///")),
                                    " ",
                                    " ",
                                    TriviaList()))),

                    XmlElement(
                        XmlElementStartTag(XmlName(Identifier("summary")))
                        .WithLessThanToken(Token(SyntaxKind.LessThanToken))
                        .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken)),

                        XmlElementEndTag(XmlName(Identifier("summary")))
                        .WithLessThanSlashToken(Token(SyntaxKind.LessThanSlashToken))
                        .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken)))
                        .WithContent(
                            SingletonList<XmlNodeSyntax>(
                                XmlText()
                        .WithTextTokens(
                            TokenList(
                                XmlTextNewLine(TriviaList(), "\n", "\n", TriviaList()),
                                XmlTextLiteral(TriviaList(DocumentationCommentExterior("///")),
                                                            " Summary text here",
                                                            " Summary text here",
                                                            TriviaList()),
                                XmlTextNewLine(TriviaList(), "\n", "\n", TriviaList()),
                                XmlTextLiteral(TriviaList(DocumentationCommentExterior("///")),
                                                                " ",
                                                                " ",
                                                                TriviaList()))))),

                                XmlText().WithTextTokens(TokenList(XmlTextNewLine(TriviaList(), "\n", "\n", TriviaList())))
                });
        }
    }
}