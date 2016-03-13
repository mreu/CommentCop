// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagnosticAnalyzer.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer
{
    using System;
    using System.Collections.Immutable;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The xml doc analyzer.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class XmlDocAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "MR0001";

        private const string Category = "Documentation";

        private static readonly string MessageFormat = $"Methods must have a xml documentation header ({DiagnosticId}).";

        private const string Title = "Methods must have a xml documentation header.";

        private const string Description = "Methods must have a xml documentation header.";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        private static readonly Action<CompilationStartAnalysisContext> CompilationStartAction = HandleCompilationStart;

        private static readonly Action<SyntaxNodeAnalysisContext> MethodDeclarationAction = AnalyzeSymbol;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationStartAction(CompilationStartAction);
        }

        private static void HandleCompilationStart(CompilationStartAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionHonorExclusions(MethodDeclarationAction, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeSymbol(SyntaxNodeAnalysisContext context)
        {
            if (context.GetDocumentationMode() == DocumentationMode.None)
            {
                return;
            }

            var declaration = (MethodDeclarationSyntax)context.Node;

            if (!HasDocumentation(declaration))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, declaration.Identifier.GetLocation()));
            }
        }

        /// <summary>
        /// Checks if a specific SyntaxNode has documentation in it's leading trivia.
        /// </summary>
        /// <param name="node">The syntax node that should be checked.</param>
        /// <returns>true if the node has documentation, false otherwise.</returns>
        internal static bool HasDocumentation(SyntaxNode node)
        {
            var commentTrivia = node.GetDocumentationCommentTriviaSyntax();

            return commentTrivia != null && !IsMissingOrEmpty(commentTrivia.ParentTrivia);
        }

        /// <summary>
        /// Checks if a SyntaxTrivia contains a DocumentationCommentTriviaSyntax and returns true if it is considered empty
        /// </summary>
        /// <param name="commentTrivia">A SyntaxTrivia containing possible documentation</param>
        /// <returns>true if commentTrivia does not have documentation in it or the documentation in SyntaxTriviais considered empty. False otherwise.</returns>
        internal static bool IsMissingOrEmpty(SyntaxTrivia commentTrivia)
        {
            if (!commentTrivia.HasStructure)
            {
                return true;
            }

            var structuredTrivia = commentTrivia.GetStructure() as DocumentationCommentTriviaSyntax;
            if (structuredTrivia != null)
            {
                return IsConsideredEmpty(structuredTrivia);
            }

            return true;
        }

        /// <summary>
        /// This helper is used by documentation diagnostics to check if a XML comment should be considered empty.
        /// A comment is empty if
        /// - it is null
        /// - it does not have any text in any XML element and it does not have an empty XML element in it.
        /// </summary>
        /// <param name="xmlComment">The xmlComment that should be checked</param>
        /// <returns>true, if the comment should be considered empty, false otherwise.</returns>
        internal static bool IsConsideredEmpty(DocumentationCommentTriviaSyntax xmlComment)
        {
            if (xmlComment == null)
            {
                return true;
            }

            foreach (XmlNodeSyntax syntax in xmlComment.Content)
            {
                if (!IsConsideredEmpty(syntax))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This helper is used by documentation diagnostics to check if a XML comment should be considered empty.
        /// A comment is empty if it does not have any text in any XML element and it does not have an empty XML element in it.
        /// </summary>
        /// <param name="xmlSyntax">The xmlSyntax that should be checked</param>
        /// <returns>true, if the comment should be considered empty, false otherwise.</returns>
        internal static bool IsConsideredEmpty(XmlNodeSyntax xmlSyntax)
        {
            var text = xmlSyntax as XmlTextSyntax;
            if (text != null)
            {
                foreach (SyntaxToken token in text.TextTokens)
                {
                    if (!string.IsNullOrWhiteSpace(token.ToString()))
                    {
                        return false;
                    }
                }

                return true;
            }

            var element = xmlSyntax as XmlElementSyntax;
            if (element != null)
            {
                foreach (XmlNodeSyntax syntax in element.Content)
                {
                    if (!IsConsideredEmpty(syntax))
                    {
                        return false;
                    }
                }

                return true;
            }

            var cdataElement = xmlSyntax as XmlCDataSectionSyntax;
            if (cdataElement != null)
            {
                foreach (SyntaxToken token in cdataElement.TextTokens)
                {
                    if (!string.IsNullOrWhiteSpace(token.ToString()))
                    {
                        return false;
                    }
                }

                return true;
            }

            var emptyElement = xmlSyntax as XmlEmptyElementSyntax;
            if (emptyElement != null)
            {
                // This includes <inheritdoc/>
                return false;
            }

            var processingElement = xmlSyntax as XmlProcessingInstructionSyntax;
            if (processingElement != null)
            {
                return false;
            }

            return true;
        }
    }
}
