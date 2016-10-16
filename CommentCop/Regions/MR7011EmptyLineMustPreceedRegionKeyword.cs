// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7011EmptyLineMustPreceedRegionKeyword.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Regions
{
    using System.Collections.Immutable;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The MR7011 Region must be preceeded by blank line class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7011EmptyLineMustPreceedRegionKeyword : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId7011 = Constants.DiagnosticPrefix + "7011";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategoryRegions;

        /// <summary>
        /// The title.
        /// </summary>
        public const string Title = "#region must be preceeded by a blank line.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The help link (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7011.md".
        /// </summary>
        private const string HelpLink = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7011.md";

        /// <summary>
        /// The rule 7011.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7011 = new DiagnosticDescriptor(
            DiagnosticId7011,
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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7011);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(CheckRegion, SyntaxKind.RegionDirectiveTrivia);
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

            var node = (RegionDirectiveTriviaSyntax)syntaxNodeAnalysisContext.Node;

            if (node?.ParentTrivia == null)
            {
                return;
            }

            var loc = node.GetLocation().GetLineSpan();
            var lineSpan = node.SyntaxTree.GetText().Lines[loc.Span.Start.Line - 1].Span;
            var tokens = node.SyntaxTree.GetRoot().DescendantTokens(lineSpan);

            if (tokens.Any())
            {
                if (tokens.First().IsKind(SyntaxKind.OpenBraceToken))
                {
                    return;
                }

                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7011, node.GetLocation(), DiagnosticId7011));
            }
        }
    }
}
