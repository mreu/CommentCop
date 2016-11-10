// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7005NoMultipleWhitespacesFollowingTheRegionKeyword.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
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
    /// The MR7005 No multiple whitespaces following the region keyword class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7005NoMultipleWhitespacesFollowingTheRegionKeyword : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId7005 = Constants.DiagnosticPrefix + "7005";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategoryRegions;

        /// <summary>
        /// The title.
        /// </summary>
        public const string Title = "No multiple whitespaces following the #region keyword.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The help link (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7005.md".
        /// </summary>
        private const string HelpLink = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7005.md";

        /// <summary>
        /// The rule 7005.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7005 = new DiagnosticDescriptor(
            DiagnosticId7005,
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
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7005);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(CheckRegion, SyntaxKind.RegionDirectiveTrivia);
        }

        /// <summary>
        /// Check region keyword is followed by multiple whitspaces.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The syntaxNodeAnalysisContext.</param>
        private void CheckRegion(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            if (CodeCracker.GeneratedCodeAnalysisExtensions.IsGenerated(syntaxNodeAnalysisContext))
            {
                return;
            }

            var node = (RegionDirectiveTriviaSyntax)syntaxNodeAnalysisContext.Node;

            var token = node.ChildTokens().FirstOrDefault(x => x.IsKind(SyntaxKind.RegionKeyword));

            if (!token.IsKind(SyntaxKind.None))
            {
                if (!token.HasTrailingTrivia)
                {
                    return;
                }

                var trivia = token.TrailingTrivia.FirstOrDefault(x => x.IsKind(SyntaxKind.WhitespaceTrivia));

                if (trivia.Span.Length > 1)
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7005, trivia.GetLocation(), DiagnosticId7005));
                }
            }
        }
    }
}