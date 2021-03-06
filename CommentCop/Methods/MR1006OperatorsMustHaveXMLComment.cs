// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR1006OperatorsMustHaveXMLComment.cs" company="Michael Reukauff, Germany">
//   Copyright � 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Methods
{
    using System.Collections.Immutable;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    using DocumentationCommentTriviaSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.DocumentationCommentTriviaSyntax;
    using XmlElementSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax;

    /// <summary>
    /// MR1006 operators must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR1006OperatorsMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id (const). Value: "MR1006".
        /// </summary>
        public const string DiagnosticId1006 = Constants.DiagnosticPrefix + "1006";

        /// <summary>
        /// The category (const). Value: "Documentation".
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title (const). Value: "Operator must have a xml documentation header.".
        /// </summary>
        private const string Title = "Operators must have a xml documentation header.";

        /// <summary>
        /// The message (readonly). Value: $"{Title} ({DiagnosticId})".
        /// </summary>
        private static readonly string Message = $"{Title} ({DiagnosticId1006})";

        /// <summary>
        /// The description (const). Value: Title.
        /// </summary>
        private const string Description = Title;

        /// <summary>
        /// The help link1005 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR1006.md".
        /// </summary>
        private const string HelpLink = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR1006.md";

        /// <summary>
        /// The rule.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId1006,
            Title,
            Message,
            Category,
            DiagnosticSeverity.Warning,
            true,
            Description,
            HelpLink);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.OperatorDeclaration);
        }

        /// <summary>
        /// Check if xml comment exists.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The systax node analysis context.</param>
        private void Check(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            if (CodeCracker.GeneratedCodeAnalysisExtensions.IsGenerated(syntaxNodeAnalysisContext))
            {
                return;
            }

            var node = syntaxNodeAnalysisContext.Node as OperatorDeclarationSyntax;

            if (node == null)
            {
                return;
            }

            if (!node.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                return;
            }

            var xmlTrivia = node.GetLeadingTrivia()
                .Select(i => i.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();

            if (xmlTrivia != null)
            {
                var hasSummary = xmlTrivia.ChildNodes()
                    .OfType<XmlElementSyntax>()
                    .Any(i => i.StartTag.Name.ToString().Equals(Constants.Summary));

                if (hasSummary)
                {
                    return;
                }
            }

            var loc = node.OperatorKeyword.GetLocation();
            var key = node.OperatorToken.GetLocation();

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule, loc, Message));
            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule, key, Message));
        }
    }
}