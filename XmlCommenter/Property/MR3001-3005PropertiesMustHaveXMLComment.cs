// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR3001-3005PropertiesMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Property
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
    /// MR3001 - 3005 properties must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR3001_3005PropertiesMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The const diagnostic id3001. Value: Constants.DiagnosticPrefix + "3001".
        /// </summary>
        public const string DiagnosticId3001 = Constants.DiagnosticPrefix + "3001";

        /// <summary>
        /// The const diagnostic id3002. Value: Constants.DiagnosticPrefix + "3002".
        /// </summary>
        public const string DiagnosticId3002 = Constants.DiagnosticPrefix + "3002";

        /// <summary>
        /// The const diagnostic id3003. Value: Constants.DiagnosticPrefix + "3003".
        /// </summary>
        public const string DiagnosticId3003 = Constants.DiagnosticPrefix + "3003";

        /// <summary>
        /// The const diagnostic id3004. Value: Constants.DiagnosticPrefix + "3004".
        /// </summary>
        public const string DiagnosticId3004 = Constants.DiagnosticPrefix + "3004";

        /// <summary>
        /// The const diagnostic id3005. Value: Constants.DiagnosticPrefix + "3005".
        /// </summary>
        public const string DiagnosticId3005 = Constants.DiagnosticPrefix + "3005";

        /// <summary>
        /// The const category. Value: Constants.DiagnosticCategory.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The const title. Value: " properties" + Constants.MustHaveXmlHeader.
        /// </summary>
        private const string Title = " properties" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The const message. Value: "{0}" + Title + " ({1})".
        /// </summary>
        private const string Message = "{0}" + Title + " ({1})";

        /// <summary>
        /// The readonly rule3001. Value: new DiagnosticDescriptor(DiagnosticId3001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3001 = new DiagnosticDescriptor(DiagnosticId3001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule3002. Value: new DiagnosticDescriptor(DiagnosticId3002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3002 = new DiagnosticDescriptor(DiagnosticId3002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule3003. Value: new DiagnosticDescriptor(DiagnosticId3003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3003 = new DiagnosticDescriptor(DiagnosticId3003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule3004. Value: new DiagnosticDescriptor(DiagnosticId3004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3004 = new DiagnosticDescriptor(DiagnosticId3004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule3005. Value: new DiagnosticDescriptor(DiagnosticId3005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3005 = new DiagnosticDescriptor(DiagnosticId3005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule3001, Rule3002, Rule3003, Rule3004, Rule3005);

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

            if (node == null)
            {
                return;
            }

            // if inside of an interface declaration do nothing
            if (node.Parent is InterfaceDeclarationSyntax)
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
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3001, node.Identifier.GetLocation(), Constants.Public, DiagnosticId3001));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId3005));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3003, node.Identifier.GetLocation(), Constants.InternalProtected, DiagnosticId3003));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId3002));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3004, node.Identifier.GetLocation(), Constants.Protected, DiagnosticId3004));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId3002));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId3005));
        }
    }
}