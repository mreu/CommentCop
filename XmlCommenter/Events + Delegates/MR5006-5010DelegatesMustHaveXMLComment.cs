// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR5006-5010DelegatesMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlCommenter.Delegates
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
    /// MR5006 - 5010 delegates must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR5006_5010DelegatesMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The const diagnostic id5006. Value: Constants.DiagnosticPrefix + "5006".
        /// </summary>
        public const string DiagnosticId5006 = Constants.DiagnosticPrefix + "5006";

        /// <summary>
        /// The const diagnostic id5007. Value: Constants.DiagnosticPrefix + "5007".
        /// </summary>
        public const string DiagnosticId5007 = Constants.DiagnosticPrefix + "5007";

        /// <summary>
        /// The const diagnostic id5008. Value: Constants.DiagnosticPrefix + "5008".
        /// </summary>
        public const string DiagnosticId5008 = Constants.DiagnosticPrefix + "5008";

        /// <summary>
        /// The const diagnostic id5009. Value: Constants.DiagnosticPrefix + "5009".
        /// </summary>
        public const string DiagnosticId5009 = Constants.DiagnosticPrefix + "5009";

        /// <summary>
        /// The const diagnostic id5010. Value: Constants.DiagnosticPrefix + "5010".
        /// </summary>
        public const string DiagnosticId5010 = Constants.DiagnosticPrefix + "5010";

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
        /// The readonly rule5006. Value: new DiagnosticDescriptor(DiagnosticId5006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5006 = new DiagnosticDescriptor(DiagnosticId5006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule5007. Value: new DiagnosticDescriptor(DiagnosticId5007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5007= new DiagnosticDescriptor(DiagnosticId5007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule5008. Value: new DiagnosticDescriptor(DiagnosticId5008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5008 = new DiagnosticDescriptor(DiagnosticId5008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule5009. Value: new DiagnosticDescriptor(DiagnosticId5009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5009 = new DiagnosticDescriptor(DiagnosticId5009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule5010. Value: new DiagnosticDescriptor(DiagnosticId5010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5010 = new DiagnosticDescriptor(DiagnosticId5010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule5006, Rule5007, Rule5008, Rule5009, Rule5010);

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
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5006, node.Identifier.GetLocation(), Constants.Public, DiagnosticId5006));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5010, node.Identifier.GetLocation(), Constants.Private, DiagnosticId5010));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5008, node.Identifier.GetLocation(), Constants.InternalProtected, DiagnosticId5008));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5007, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId5007));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5009, node.Identifier.GetLocation(), Constants.Protected, DiagnosticId5009));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5007, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId5007));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5010, node.Identifier.GetLocation(), Constants.Private, DiagnosticId5010));
        }
    }
}