// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR4001-4005FieldsMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Fields
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
    /// MR4001 - 0005 fields must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR4001_4005FieldsMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The const diagnostic id4001. Value: Constants.DiagnosticPrefix + "4001".
        /// </summary>
        public const string DiagnosticId4001 = Constants.DiagnosticPrefix + "4001";
        /// <summary>
        /// The const diagnostic id4002. Value: Constants.DiagnosticPrefix + "4002".
        /// </summary>
        public const string DiagnosticId4002 = Constants.DiagnosticPrefix + "4002";
        /// <summary>
        /// The const diagnostic id4003. Value: Constants.DiagnosticPrefix + "4003".
        /// </summary>
        public const string DiagnosticId4003 = Constants.DiagnosticPrefix + "4003";
        /// <summary>
        /// The const diagnostic id4004. Value: Constants.DiagnosticPrefix + "4004".
        /// </summary>
        public const string DiagnosticId4004 = Constants.DiagnosticPrefix + "4004";

        /// <summary>
        /// The const diagnostic id4005. Value: Constants.DiagnosticPrefix + "4005".
        /// </summary>
        public const string DiagnosticId4005 = Constants.DiagnosticPrefix + "4005";

        /// <summary>
        /// The const category. Value: Constants.DiagnosticCategory.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The const title. Value: " fields" + Constants.MustHaveXmlHeader.
        /// </summary>
        private const string Title = " fields" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The const message. Value: "{0}" + Title + " ({1})".
        /// </summary>
        private const string Message = "{0}" + Title + " ({1})";

        /// <summary>
        /// The readonly rule4001. Value: new DiagnosticDescriptor(DiagnosticId4001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule4001 = new DiagnosticDescriptor(DiagnosticId4001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule4002. Value: new DiagnosticDescriptor(DiagnosticId4002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule4002 = new DiagnosticDescriptor(DiagnosticId4002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule4003. Value: new DiagnosticDescriptor(DiagnosticId4003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule4003 = new DiagnosticDescriptor(DiagnosticId4003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule4004. Value: new DiagnosticDescriptor(DiagnosticId4004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule4004 = new DiagnosticDescriptor(DiagnosticId4004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule4005. Value: new DiagnosticDescriptor(DiagnosticId4005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule4005 = new DiagnosticDescriptor(DiagnosticId4005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule4001, Rule4002, Rule4003, Rule4004, Rule4005);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.FieldDeclaration);
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

            var node = syntaxNodeAnalysisContext.Node as FieldDeclarationSyntax;

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

            var field = node.DescendantNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault();

            if (node.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule4001, field.GetLocation(), Constants.Public, DiagnosticId4001));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule4005, field.GetLocation(), Constants.Private, DiagnosticId4005));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule4003, field.GetLocation(), Constants.InternalProtected, DiagnosticId4003));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule4002, field.GetLocation(), Constants.Internal, DiagnosticId4002));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule4004, field.GetLocation(), Constants.Protected, DiagnosticId4004));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule4002, field.GetLocation(), Constants.Internal, DiagnosticId4002));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule4005, field.GetLocation(), Constants.Private, DiagnosticId4005));
        }
    }
}
