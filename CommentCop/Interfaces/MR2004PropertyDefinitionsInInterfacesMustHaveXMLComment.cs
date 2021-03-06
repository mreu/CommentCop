﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR2004PropertyDefinitionsInInterfacesMustHaveXMLComment.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Interfaces
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
    /// MR2004 property definitions in interfaces must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR2004PropertyDefinitionsInInterfacesMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id (const). Value: Constants.DiagnosticPrefix + "2004".
        /// </summary>
        public const string DiagnosticId = Constants.DiagnosticPrefix + "2004";

        /// <summary>
        /// The category (const). Value: Constants.DiagnosticCategory.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title (const). Value: "Property definitions in interfaces" + Constants.MustHaveXmlHeader.
        /// </summary>
        private const string Title = "Property definitions in interfaces" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The message (readonly). Value: $"{Title} ({DiagnosticId})".
        /// </summary>
        private static readonly string Message = $"{Title} ({DiagnosticId})";

        /// <summary>
        /// The help link (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR2004.md".
        /// </summary>
        private const string HelpLink = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR2004.md";

        /// <summary>
        /// The rule.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink);

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
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.PropertyDeclaration);
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

            var node = syntaxNodeAnalysisContext.Node as PropertyDeclarationSyntax;

            // ReSharper disable once UseNullPropagation
            if (node == null)
            {
                return;
            }

            // if not inside of an interface declaration do nothing
            if (!(node.Parent is InterfaceDeclarationSyntax))
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

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule, node.Identifier.GetLocation(), Message));
        }
    }
}