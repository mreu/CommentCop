// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7004EndregionMustHaveTheSameTextAsTheRegion.cs" company="Michael Reukauff">
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
    /// MR7004 Endregion must have the same text as the region class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7004EndregionMustHaveTheSameTextAsTheRegion : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId7004 = Constants.DiagnosticPrefix + "7004";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategoryRegions;

        /// <summary>
        /// The title.
        /// </summary>
        public const string Title = "Description in #endregion must be the same as in #region.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The help link (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7004.md".
        /// </summary>
        private const string HelpLink = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7004.md";

        /// <summary>
        /// The rule 7004.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7004 = new DiagnosticDescriptor(
            DiagnosticId7004,
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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7004);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(CheckRegion, SyntaxKind.EndRegionDirectiveTrivia);
        }

        /// <summary>
        /// Check region keyword is followed by a description.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The syntaxNodeAnalysisContext.</param>
        private void CheckRegion(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            if (CodeCracker.GeneratedCodeAnalysisExtensions.IsGenerated(syntaxNodeAnalysisContext))
            {
                return;
            }

            var node = (EndRegionDirectiveTriviaSyntax)syntaxNodeAnalysisContext.Node;

            var token = node.ChildTokens().LastOrDefault();

            if (token.IsKind(SyntaxKind.EndOfDirectiveToken))
            {
                if (!token.HasLeadingTrivia)
                {
                    return;
                }

                var task = Helper.RegionText.GetTextFromRegion(node, node.SpanStart);

                // if #region has no text, do not check
                if (string.IsNullOrEmpty(task.Result?.Item1))
                {
                    return;
                }

                if (!task.Result.Item1.Equals(task.Result.Item2))
                {
                    var diag = Diagnostic.Create(Rule7004, token.LeadingTrivia[0].GetLocation(), DiagnosticId7004);
                    syntaxNodeAnalysisContext.ReportDiagnostic(diag);
                }
            }
        }
    }
}
