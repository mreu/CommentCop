﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR100nCodeFixProvider.cs" company="Michael Reukauff">
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

    using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

    /// <summary>
    /// The xml doc code fix provider.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MR100nCodeFixProvider))]
    [Shared]
    public class MR100nCodeFixProvider : CodeFixProvider
    {
        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Insert XML documentation header (MR1001 - MR1005)";

        /// <summary>
        /// Gets the fixable diagnostic ids.
        /// </summary>
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            =>
                ImmutableArray.Create(
                    MR1001PublicMethodsMustHaveXMLComment.DiagnosticId,
                    MR1002InternalMethodsMustHaveXMLComment.DiagnosticId,
                    MR1003InternalProtectedMethodsMustHaveXMLComment.DiagnosticId,
                    MR1004ProtectedMethodsMustHaveXMLComment.DiagnosticId,
                    MR1005PrivateMethodsMustHaveXMLComment.DiagnosticId);

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
                (MethodDeclarationSyntax)identifierToken.Parent,
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
            MethodDeclarationSyntax declaration,
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
        /// <param name="theMethod">The method to add to the summary.</param>
        /// <returns>The syntax list.</returns>
        private static DocumentationCommentTriviaSyntax GetSummary(MethodDeclarationSyntax theMethod)
        {
            const string summary = "summary";

            var summaryStart = XmlElementStartTag(XmlName(Identifier(summary)))
                .WithLessThanToken(Token(SyntaxKind.LessThanToken))
                .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken)).NormalizeWhitespace();

            var summaryEnd = XmlElementEndTag(XmlName(Identifier(summary))).NormalizeWhitespace()
                .WithLessThanSlashToken(Token(SyntaxKind.LessThanSlashToken))
                .WithGreaterThanToken(Token(SyntaxKind.GreaterThanToken));

            var summaryComment = " " + Convert.Method(theMethod.Identifier.ValueText);

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

            // Add parameter comments
            if (theMethod.ParameterList.Parameters.Any())
            {
                foreach (var parameter in theMethod.ParameterList.Parameters)
                {
                    list = list.AddRange(
                        List(
                            new XmlNodeSyntax[]
                            {
                                xmlComment,

                                XmlElement(
                                    XmlElementStartTag(XmlName(Identifier("param")))
                                    .WithAttributes(
                                        SingletonList<XmlAttributeSyntax>(
                                            XmlNameAttribute(
                                                XmlName(Identifier(TriviaList(Space), "name", TriviaList())),
                                                Token(SyntaxKind.DoubleQuoteToken),
                                                IdentifierName(parameter.Identifier.ValueText),
                                                Token(SyntaxKind.DoubleQuoteToken)))),
                                    XmlElementEndTag(XmlName(Identifier("param"))))
                                    .WithContent(
                                        SingletonList<XmlNodeSyntax>(
                                            XmlText()
                                    .WithTextTokens(
                                        TokenList(
                                            XmlTextLiteral(
                                                TriviaList(),
                                                Convert.Parameter(parameter.Identifier.ValueText),
                                                "comment",
                                                TriviaList()))))),

                                newLine
                            }));
                }
            }

            // Add returns comments
            string returntype;
            if (theMethod.ReturnType is PredefinedTypeSyntax)
            {
                returntype = ((PredefinedTypeSyntax)theMethod.ReturnType).Keyword.ValueText;
            }
            else
            {
                if (theMethod.ReturnType is IdentifierNameSyntax)
                {
                    returntype = ((IdentifierNameSyntax)theMethod.ReturnType).Identifier.ValueText;
                }
                else
                {
                    if (theMethod.ReturnType is GenericNameSyntax)
                    {
                        returntype = ((GenericNameSyntax)theMethod.ReturnType).Identifier.ValueText;
                    }
                    else
                    {
                        returntype = "Unknown return type: " + theMethod.ReturnType.GetType();
                    }
                }
            }

            if (returntype != "void")
            {
                var typeArgumentList = theMethod.ReturnType.ChildNodes().OfType<TypeArgumentListSyntax>().First();

                list = list.AddRange(
                    List(
                        new XmlNodeSyntax[]
                        {
                            xmlComment,

                            XmlElement(XmlElementStartTag(XmlName(Identifier("returns"))), XmlElementEndTag(XmlName(Identifier("returns"))))
                                .WithContent(
                                    SingletonList<XmlNodeSyntax>(
                                        XmlText().WithTextTokens(TokenList(XmlTextLiteral(TriviaList(), Convert.Returns(returntype, typeArgumentList), "comment", TriviaList()))))),

                            newLine
                        }));
            }

            // Add typeparams comments
            if (theMethod.TypeParameterList != null)
            {
                if (theMethod.TypeParameterList.Parameters.Any())
                {
                    foreach (var parameter in theMethod.TypeParameterList.Parameters)
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
                                    XmlElementEndTag(XmlName(Identifier("param"))))
                                    .WithContent(
                                        SingletonList<XmlNodeSyntax>(
                                            XmlText()
                                    .WithTextTokens(
                                        TokenList(
                                            XmlTextLiteral(
                                                TriviaList(),
                                                string.Empty,
                                                "comment",
                                                TriviaList()))))),

                                newLine
                                }));
                    }
                }
            }

            // Add exceptions comments
            var throws = theMethod.DescendantNodes().OfType<ThrowStatementSyntax>();
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