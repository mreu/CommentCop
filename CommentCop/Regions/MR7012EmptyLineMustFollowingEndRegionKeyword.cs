// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7012EmptyLineMustFollowingEndRegionKeyword.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
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
    /// The MT7012Empty line must following end region keyword class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7012EmptyLineMustFollowingEndRegionKeyword : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId7012 = Constants.DiagnosticPrefix + "7012";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategoryRegions;

        /// <summary>
        /// The title.
        /// </summary>
        public const string Title = "#endregion must be followed by blank line.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The help link (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7012.md".
        /// </summary>
        private const string HelpLink = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7012.md";

        /// <summary>
        /// The rule 7012.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7012 = new DiagnosticDescriptor(
            DiagnosticId7012,
            Title,
            Message,
            Category,
            DiagnosticSeverity.Warning,
            true,
            null,
            HelpLink);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7012);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(CheckRegion, SyntaxKind.EndRegionDirectiveTrivia);
        }

        /// <summary>
        /// Check region keyword is followed by an empty line.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The syntaxNodeAnalysisContext.</param>
        private void CheckRegion(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            if (CodeCracker.GeneratedCodeAnalysisExtensions.IsGenerated(syntaxNodeAnalysisContext))
            {
                return;
            }

            var node = (EndRegionDirectiveTriviaSyntax)syntaxNodeAnalysisContext.Node;
            if (node == null)
            {
                return;
            }

            if (!node.HasTrailingTrivia)
            {
                return;
            }

            var trailingTrivia = node.GetTrailingTrivia();
            if (trailingTrivia.Count == 0)
            {
                return;
            }

            if (!trailingTrivia[0].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                return;
            }

            var endOfLineTrivia = trailingTrivia[0].Span.End;
            var parent = node.ParentTrivia.Token.Parent.ChildThatContainsPosition(endOfLineTrivia + 1);

            if (parent.IsToken && parent.IsKind(SyntaxKind.CloseBraceToken))
            {
                return;
            }

            var lt = parent.GetLeadingTrivia();
            var idx = lt.IndexOf(SyntaxKind.EndRegionDirectiveTrivia);
            if (idx == -1)
            {
                return;
            }

            if (idx < lt.Count - 1)
            {
                var trivias = node.ParentTrivia.Token.LeadingTrivia;

                var ix = trivias.IndexOfTrivia(node.Span);

                if (!trivias[ix + 1].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7012, node.GetLocation(), DiagnosticId7012));
                }
            }
        }
    }
}
