// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7003DescriptionInEndregionsMustBeginWithUppercaseCharacter.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlCommenter.Regions
{
    using System.Collections.Immutable;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The MR7003Description in endregions must begin with uppercase character class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7003DescriptionInEndregionsMustBeginWithUppercaseCharacter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId7003 = Constants.DiagnosticPrefix + "7003";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Description in endregions must begin with uppercase characters.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The rule 9001.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7003 = new DiagnosticDescriptor(DiagnosticId7003, Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7003);

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
            var node = (EndRegionDirectiveTriviaSyntax)syntaxNodeAnalysisContext.Node;

            var token = node.ChildTokens().LastOrDefault();

            if (token.IsKind(SyntaxKind.EndOfDirectiveToken))
            {
                if (token.HasLeadingTrivia)
                {
                    var text1 = token.LeadingTrivia.ToString();

                    var words = text1.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                    foreach (var word in words)
                    {
                        if (!char.IsLetter(word[0]))
                        {
                            continue;
                        }

                        if (!char.IsUpper(word[0]))
                        {
                            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7003, token.LeadingTrivia[0].GetLocation(), DiagnosticId7003));
                            break;
                        }
                    }
                }
            }
        }
    }
}
