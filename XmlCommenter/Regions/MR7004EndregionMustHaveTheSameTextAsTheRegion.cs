// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7004EndregionMustHaveTheSameTextAsTheRegion.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlCommenter.Regions
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

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
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Description in #endregion must be the same as in #region.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The rule 9001.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7004 = new DiagnosticDescriptor(
            DiagnosticId7004,
            Title,
            Message,
            Category,
            DiagnosticSeverity.Warning,
            true);

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

                var root = node.SyntaxTree.GetRoot();

                var regionNodesList = new List<RegionNodes>();

                var regions = root.DescendantNodes(null, true).OfType<RegionDirectiveTriviaSyntax>();
                foreach (var regionDirective in regions)
                {
                    regionNodesList.Add(new RegionNodes { RegionDirective = regionDirective });
                }

                var endregions = root.DescendantNodes(null, true).OfType<EndRegionDirectiveTriviaSyntax>();
                foreach (var endRegionDirective in endregions)
                {
                    var reg = regionNodesList.Last(x => x.RegionDirective.SpanStart < endRegionDirective.SpanStart && x.EndRegionDirective == null);
                    reg.EndRegionDirective = endRegionDirective;
                }

                var region = regionNodesList.FirstOrDefault(x => x.EndRegionDirective.FullSpan.Equals(node.FullSpan));
                if (region == null)
                {
                    return;
                }

                var regionToken = region.RegionDirective.EndOfDirectiveToken;
                if (regionToken.HasLeadingTrivia)
                {
                    var regiontext1 = regionToken.LeadingTrivia.ToString();

                    var endregionToken = region.EndRegionDirective.EndOfDirectiveToken;
                    if (endregionToken.HasLeadingTrivia)
                    {
                        var endregiontext1 = endregionToken.LeadingTrivia.ToString();

                        if (!regiontext1.Equals(endregiontext1))
                        {
                            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7004, node.GetLocation(), DiagnosticId7004));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The region nodes class.
        /// </summary>
        private class RegionNodes
        {
            /// <summary>
            /// Gets or sets the region directive.
            /// </summary>
            public RegionDirectiveTriviaSyntax RegionDirective { get; set; }

            /// <summary>
            /// Gets or sets the end region directive.
            /// </summary>
            public EndRegionDirectiveTriviaSyntax EndRegionDirective { get; set; }
        }
    }
}
