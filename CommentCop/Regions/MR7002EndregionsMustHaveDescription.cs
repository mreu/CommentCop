// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7002EndregionsMustHaveDescription.cs" company="Michael Reukauff, Germany">
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
    /// The MR7002Endregions must have description class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7002EndregionsMustHaveDescription : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId7002 = Constants.DiagnosticPrefix + "7002";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategoryRegions;

        /// <summary>
        /// The title.
        /// </summary>
        public const string Title = "#endregions must have a description.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The help link (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7002.md".
        /// </summary>
        private const string HelpLink = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR7002.md";

        /// <summary>
        /// The rule 7002.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7002 = new DiagnosticDescriptor(DiagnosticId7002, Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7002);

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
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7002, node.GetLocation(), DiagnosticId7002));
                }
            }
        }
    }
}
