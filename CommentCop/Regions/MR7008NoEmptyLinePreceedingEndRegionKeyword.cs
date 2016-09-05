// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7008NoEmptyLinePreceedingEndRegionKeyword.cs" author="Michael Reukauff">
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
    /// The MR7008 No empty line preceeding endregion keyword class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7008NoEmptyLinePreceedingEndRegionKeyword : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId7008 = Constants.DiagnosticPrefix + "7008";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title.
        /// </summary>
        public const string Title = "#endregion must not be preceeded by blank line(s).";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The rule 9001.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7008 = new DiagnosticDescriptor(
            DiagnosticId7008,
            Title,
            Message,
            Category,
            DiagnosticSeverity.Warning,
            true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7008);

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
                return;
            }

            if (lt[idx].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7008, node.GetLocation(), DiagnosticId7008));
            }
        }
    }
}