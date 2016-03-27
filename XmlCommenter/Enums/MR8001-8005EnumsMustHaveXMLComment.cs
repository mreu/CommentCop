// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR8001-8005EnumsMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Enums
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
    /// MR8001 - 8005 enums must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR8001_MR8005EnumsMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The const diagnostic id8001. Value: Constants.DiagnosticPrefix + "8001".
        /// </summary>
        public const string DiagnosticId8001 = Constants.DiagnosticPrefix + "8001";

        /// <summary>
        /// The const diagnostic id8002. Value: Constants.DiagnosticPrefix + "8002".
        /// </summary>
        public const string DiagnosticId8002 = Constants.DiagnosticPrefix + "8002";

        /// <summary>
        /// The const diagnostic id8003. Value: Constants.DiagnosticPrefix + "8003".
        /// </summary>
        public const string DiagnosticId8003 = Constants.DiagnosticPrefix + "8003";

        /// <summary>
        /// The const diagnostic id8004. Value: Constants.DiagnosticPrefix + "8004".
        /// </summary>
        public const string DiagnosticId8004 = Constants.DiagnosticPrefix + "8004";

        /// <summary>
        /// The const diagnostic id8005. Value: Constants.DiagnosticPrefix + "8005".
        /// </summary>
        public const string DiagnosticId8005 = Constants.DiagnosticPrefix + "8005";

        /// <summary>
        /// The const category. Value: Constants.DiagnosticCategory.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The const title. Value: " enums" + Constants.MustHaveXmlHeader.
        /// </summary>
        private const string Title = " enums" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The const message. Value: "{0}" + Title + " ({1})".
        /// </summary>
        private const string Message = "{0}" + Title + " ({1})";

        /// <summary>
        /// The readonly rule8001. Value: new DiagnosticDescriptor(DiagnosticId8001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule8001 = new DiagnosticDescriptor(DiagnosticId8001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule8002. Value: new DiagnosticDescriptor(DiagnosticId8002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule8002 = new DiagnosticDescriptor(DiagnosticId8002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule8003. Value: new DiagnosticDescriptor(DiagnosticId8003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule8003 = new DiagnosticDescriptor(DiagnosticId8003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule8004. Value: new DiagnosticDescriptor(DiagnosticId8004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule8004 = new DiagnosticDescriptor(DiagnosticId8004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule8005. Value: new DiagnosticDescriptor(DiagnosticId8005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule8005 = new DiagnosticDescriptor(DiagnosticId8005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule8001, Rule8002, Rule8003, Rule8004, Rule8005);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.EnumDeclaration);
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

            var node = syntaxNodeAnalysisContext.Node as EnumDeclarationSyntax;

            if (node == null)
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

            if (node.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule8001, node.Identifier.GetLocation(), Constants.Public, DiagnosticId8001));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule8005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId8005));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule8003, node.Identifier.GetLocation(), Constants.InternalProtected, DiagnosticId8003));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule8002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId8002));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule8004, node.Identifier.GetLocation(), Constants.Protected, DiagnosticId8004));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule8002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId8002));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule8005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId8005));
        }
    }
}