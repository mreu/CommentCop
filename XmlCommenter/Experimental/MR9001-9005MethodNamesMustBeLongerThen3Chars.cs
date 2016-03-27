// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR9001-9005MethodNamesMustBeLongerThen3Chars.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Experimental
{
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    using DocumentationCommentTriviaSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.DocumentationCommentTriviaSyntax;
    using XmlElementSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.XmlElementSyntax;

    /// <summary>
    /// MR1001 public methods must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR9001_9005MethodNamesMustBeLongerThen3Chars : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId9001 = "MR9001";
        /// <summary>
        /// The const diagnostic id9002. Value: "MR9002".
        /// </summary>
        public const string DiagnosticId9002 = "MR9002";
        /// <summary>
        /// The const diagnostic id9003. Value: "MR9003".
        /// </summary>
        public const string DiagnosticId9003 = "MR9003";
        /// <summary>
        /// The const diagnostic id9004. Value: "MR9004".
        /// </summary>
        public const string DiagnosticId9004 = "MR9004";
        /// <summary>
        /// The const diagnostic id9005. Value: "MR9005".
        /// </summary>
        public const string DiagnosticId9005 = "MR9005";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = "Documentation";

        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Xml comment must be preceeded by blank line.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = "Xml comment must be preceeded by blank line. ({0})";

        /// <summary>
        /// The rule 9001.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule9001 = new DiagnosticDescriptor(DiagnosticId9001, Title, Message, Category, DiagnosticSeverity.Warning, true);
        ////private static readonly DiagnosticDescriptor Rule9002 = new DiagnosticDescriptor(DiagnosticId9002, Title, Message, Category, DiagnosticSeverity.Warning, true);
        ////private static readonly DiagnosticDescriptor Rule9003 = new DiagnosticDescriptor(DiagnosticId9003, Title, Message, Category, DiagnosticSeverity.Warning, true);
        ////private static readonly DiagnosticDescriptor Rule9004 = new DiagnosticDescriptor(DiagnosticId9004, Title, Message, Category, DiagnosticSeverity.Warning, true);
        ////private static readonly DiagnosticDescriptor Rule9005 = new DiagnosticDescriptor(DiagnosticId9005, Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        ////public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule9001, Rule9002, Rule9003, Rule9004, Rule9005);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule9001);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            // context.RegisterSyntaxNodeAction(Check, SyntaxKind.FieldDeclaration);
        }

        /// <summary>
        /// Check if xml comment exists.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The systax node analysis context.</param>
        private void Check(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            var node = syntaxNodeAnalysisContext.Node;

            if (!node.HasLeadingTrivia)
            {
                return;
            }

            var xmlTrivia = node.GetLeadingTrivia()
                .Select(i => i.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();

            if (xmlTrivia == null)
            {
                return;
            }

            var hasSummary = xmlTrivia.ChildNodes()
                .OfType<XmlElementSyntax>()
                .Any(i => i.StartTag.Name.ToString().Equals(Constants.Summary));

            if (!hasSummary)
            {
                return;
            }

            var trivia = node.GetLeadingTrivia();
            foreach (var tr in trivia)
            {
                if (tr.Kind() == SyntaxKind.EndOfLineTrivia)
                {
                    return;
                }

                if (tr.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia)
                {
                    break;
                }
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule9001, node.GetLocation(), DiagnosticId9001));
        }
    }
}