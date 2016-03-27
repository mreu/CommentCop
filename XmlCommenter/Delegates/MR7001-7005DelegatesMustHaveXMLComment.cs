// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7001-7005DelegatesMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Delegates
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
    /// MR7001 - 7005 delegates must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7001_7005DelegatesMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The const diagnostic id7001. Value: Constants.DiagnosticPrefix + "7001".
        /// </summary>
        public const string DiagnosticId7001 = Constants.DiagnosticPrefix + "7001";

        /// <summary>
        /// The const diagnostic id7002. Value: Constants.DiagnosticPrefix + "7002".
        /// </summary>
        public const string DiagnosticId7002 = Constants.DiagnosticPrefix + "7002";

        /// <summary>
        /// The const diagnostic id7003. Value: Constants.DiagnosticPrefix + "7003".
        /// </summary>
        public const string DiagnosticId7003 = Constants.DiagnosticPrefix + "7003";

        /// <summary>
        /// The const diagnostic id7004. Value: Constants.DiagnosticPrefix + "7004".
        /// </summary>
        public const string DiagnosticId7004 = Constants.DiagnosticPrefix + "7004";

        /// <summary>
        /// The const diagnostic id7005. Value: Constants.DiagnosticPrefix + "7005".
        /// </summary>
        public const string DiagnosticId7005 = Constants.DiagnosticPrefix + "7005";

        /// <summary>
        /// The const category. Value: "Documentation".
        /// </summary>
        private const string Category = "Documentation";

        /// <summary>
        /// The const title. Value: " delegates" + Constants.MustHaveXmlHeader.
        /// </summary>
        private const string Title = " delegates" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The const message. Value: "{0}" + Title + " ({1})".
        /// </summary>
        private const string Message = "{0}" + Title + " ({1})";

        /// <summary>
        /// The readonly rule7001. Value: new DiagnosticDescriptor(DiagnosticId7001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7001 = new DiagnosticDescriptor(DiagnosticId7001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule7002. Value: new DiagnosticDescriptor(DiagnosticId7002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7002 = new DiagnosticDescriptor(DiagnosticId7002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule7003. Value: new DiagnosticDescriptor(DiagnosticId7003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7003 = new DiagnosticDescriptor(DiagnosticId7003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule7004. Value: new DiagnosticDescriptor(DiagnosticId7004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7004 = new DiagnosticDescriptor(DiagnosticId7004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule7005. Value: new DiagnosticDescriptor(DiagnosticId7005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7005 = new DiagnosticDescriptor(DiagnosticId7005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7001, Rule7002, Rule7003, Rule7004, Rule7005);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.DelegateDeclaration);
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

            var node = syntaxNodeAnalysisContext.Node as DelegateDeclarationSyntax;

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
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7001, node.Identifier.GetLocation(), Constants.Public, DiagnosticId7001));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId7005));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7003, node.Identifier.GetLocation(), Constants.InternalProtected, DiagnosticId7003));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId7002));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7004, node.Identifier.GetLocation(), Constants.Protected, DiagnosticId7004));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId7002));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId7005));
        }
    }
}