// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7000RegionsMustHaveDescription.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlCommenter.Regions
{
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// MR1001 public methods must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7000RegionsMustHaveDescription : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId7000 = Constants.DiagnosticPrefix + "7000";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "#regions must have a description.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The rule 9001.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7000 = new DiagnosticDescriptor(DiagnosticId7000, Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7000);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(CheckRegion, SyntaxKind.RegionDirectiveTrivia);
        }

        /// <summary>
        /// Check region keyword is followed by a description.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The syntaxNodeAnalysisContext.</param>
        private async void CheckRegion(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            await Task.Run(() =>
            {
                if (CodeCracker.GeneratedCodeAnalysisExtensions.IsGenerated(syntaxNodeAnalysisContext))
                {
                    return;
                }

                var node = (RegionDirectiveTriviaSyntax)syntaxNodeAnalysisContext.Node;

                var token = node.ChildTokens().LastOrDefault();

                if (token.IsKind(SyntaxKind.EndOfDirectiveToken))
                {
                    if (!token.HasLeadingTrivia)
                    {
                        syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7000, node.GetLocation(), DiagnosticId7000));
                    }
                }
            });
        }
    }
}