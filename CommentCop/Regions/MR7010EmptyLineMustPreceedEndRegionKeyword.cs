﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7010EmptyLineMustPreceedEndRegionKeyword.cs" author="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Regions
{
    using System.Collections.Immutable;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The MR7010 Empty line must preceed endregion keyword class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7010EmptyLineMustPreceedEndRegionKeyword : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId7010 = Constants.DiagnosticPrefix + "7010";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategoryRegions;

        /// <summary>
        /// The title.
        /// </summary>
        public const string Title = "#endregion must be preceeding by a blank line.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The help link (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7010.md".
        /// </summary>
        private const string HelpLink = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7010.md";

        /// <summary>
        /// The rule 7010.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7010 = new DiagnosticDescriptor(
                                                                    DiagnosticId7010,
                                                                    Title,
                                                                    Message,
                                                                    Category,
                                                                    DiagnosticSeverity.Warning,
#if DEBUG
                                                                    true, // set to true to simplify debugging
#else
                                                                    false,
#endif
                                                                    null,
                                                                    HelpLink);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7010);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(CheckRegion, SyntaxKind.EndRegionDirectiveTrivia);
        }

        /// <summary>
        /// Check endregion keyword is preceeded by an empty line.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The syntaxNodeAnalysisContext.</param>
        private void CheckRegion(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            if (CodeCracker.GeneratedCodeAnalysisExtensions.IsGenerated(syntaxNodeAnalysisContext))
            {
                return;
            }

            var node = (EndRegionDirectiveTriviaSyntax)syntaxNodeAnalysisContext.Node;

            if (node?.ParentTrivia == null)
            {
                return;
            }

            var spanStart = node.Span.Start;
            var parent = node.ParentTrivia.Token.Parent.ChildThatContainsPosition(spanStart - 1);
            var lt = parent.GetLeadingTrivia();

            var idx = lt.IndexOfTrivia(node.FullSpan) - 1;
            if (idx < 0)
            {
                return;
            }

            if (lt[idx].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                idx--;
            }

            if (idx < 0)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7010, node.GetLocation(), DiagnosticId7010));
                return;
            }

            if (!lt[idx].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7010, node.GetLocation(), DiagnosticId7010));
            }
        }
    }
}
