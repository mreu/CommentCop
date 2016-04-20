// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7001DescriptionInRegionsMustBeginWithUppercaseCharacter.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Region
{
    using System.Collections.Immutable;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// MR1001 public methods must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7001DescriptionInRegionsMustBeginWithUppercaseCharacter : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId7001 = Constants.DiagnosticPrefix + "7001";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Description in regions must begin with uppercase characters.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = "Description in regions must begin with uppercase characters. ({0})";

        /// <summary>
        /// The rule 9001.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7001 = new DiagnosticDescriptor(DiagnosticId7001, Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7001);

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
        private void CheckRegion(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            var node = (RegionDirectiveTriviaSyntax)syntaxNodeAnalysisContext.Node;

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
                            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7001, token.LeadingTrivia[0].GetLocation(), DiagnosticId7001));
                            break;
                        }
                    }
                }
            }
        }
    }
}