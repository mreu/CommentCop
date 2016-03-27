// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR9001-9005MethodNamesMustBeLongerThen3Chars.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Experimental
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
    /// MR1001 public methods must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR9001_9005MethodNamesMustBeLongerThen3Chars : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId9001 = "MR9001";
        public const string DiagnosticId9002 = "MR9002";
        public const string DiagnosticId9003 = "MR9003";
        public const string DiagnosticId9004 = "MR9004";
        public const string DiagnosticId9005 = "MR9005";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = "Documentation";

        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = "Method names must be longer then 3 charachters.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = "{0} method names must be longer then 3 charachters. ({1})";

        /// <summary>
        /// The rule 9001.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule9001 = new DiagnosticDescriptor(DiagnosticId9001, Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule9002 = new DiagnosticDescriptor(DiagnosticId9002, Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule9003 = new DiagnosticDescriptor(DiagnosticId9003, Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule9004 = new DiagnosticDescriptor(DiagnosticId9004, Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule9005 = new DiagnosticDescriptor(DiagnosticId9005, Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule9001, Rule9002, Rule9003, Rule9004, Rule9005);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            //context.RegisterSyntaxNodeAction(Check, SyntaxKind.MethodDeclaration);
        }

        /// <summary>
        /// Check if xml comment exists.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The systax node analysis context.</param>
        private void Check(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            var node = syntaxNodeAnalysisContext.Node as MethodDeclarationSyntax;

            if (node == null)
            {
                return;
            }

            ////var xmlTrivia = node.GetLeadingTrivia()
            ////    .Select(i => i.GetStructure())
            ////    .OfType<DocumentationCommentTriviaSyntax>()
            ////    .FirstOrDefault();

            ////if (xmlTrivia != null)
            ////{
            ////    var hasSummary = xmlTrivia.ChildNodes()
            ////        .OfType<XmlElementSyntax>()
            ////        .Any(i => i.StartTag.Name.ToString().Equals(Constants.Summary));

            ////    if (hasSummary)
            ////    {
            ////        return;
            ////    }
            ////}

            if (node.Identifier.ToString().Length > 3)
            {
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule9001, node.Identifier.GetLocation(), "Public", "MR9001"));
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule9005, node.Identifier.GetLocation(), "Private", "MR9005"));
            }
        }
    }
}