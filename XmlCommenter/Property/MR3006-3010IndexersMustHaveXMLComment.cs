// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR3006-3010IndexersMustHaveXMLComment.cs" company="Michael Reukauff">
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
    /// MR3006 - 3010 indexers must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR3006_3010IndexersMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The const diagnostic id3006. Value: Constants.DiagnosticPrefix + "3006".
        /// </summary>
        public const string DiagnosticId3006 = Constants.DiagnosticPrefix + "3006";

        /// <summary>
        /// The const diagnostic id3007. Value: Constants.DiagnosticPrefix + "3007".
        /// </summary>
        public const string DiagnosticId3007 = Constants.DiagnosticPrefix + "3007";

        /// <summary>
        /// The const diagnostic id3008. Value: Constants.DiagnosticPrefix + "3008".
        /// </summary>
        public const string DiagnosticId3008 = Constants.DiagnosticPrefix + "3008";

        /// <summary>
        /// The const diagnostic id3009. Value: Constants.DiagnosticPrefix + "3009".
        /// </summary>
        public const string DiagnosticId3009 = Constants.DiagnosticPrefix + "3009";

        /// <summary>
        /// The const diagnostic id3010. Value: Constants.DiagnosticPrefix + "3010".
        /// </summary>
        public const string DiagnosticId3010 = Constants.DiagnosticPrefix + "3010";

        /// <summary>
        /// The const category. Value: Constants.DiagnosticCategory.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The const title. Value: " indexers" + Constants.MustHaveXmlHeader.
        /// </summary>
        private const string Title = " indexers" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The const message. Value: "{0}" + Title + " ({1})".
        /// </summary>
        private const string Message = "{0}" + Title + " ({1})";

        /// <summary>
        /// The readonly rule3006. Value: new DiagnosticDescriptor(DiagnosticId3006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3006 = new DiagnosticDescriptor(DiagnosticId3006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule3007. Value: new DiagnosticDescriptor(DiagnosticId3007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3007 = new DiagnosticDescriptor(DiagnosticId3007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule3008. Value: new DiagnosticDescriptor(DiagnosticId3008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3008 = new DiagnosticDescriptor(DiagnosticId3008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule3009. Value: new DiagnosticDescriptor(DiagnosticId3009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3009 = new DiagnosticDescriptor(DiagnosticId3009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule3010. Value: new DiagnosticDescriptor(DiagnosticId3010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3010 = new DiagnosticDescriptor(DiagnosticId3010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule3006, Rule3007, Rule3008, Rule3009, Rule3010);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.IndexerDeclaration);
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

            var node = syntaxNodeAnalysisContext.Node as IndexerDeclarationSyntax;

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
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3006, node.ThisKeyword.GetLocation(), Constants.Public, DiagnosticId3006));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3010, node.ThisKeyword.GetLocation(), Constants.Private, DiagnosticId3010));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3008, node.ThisKeyword.GetLocation(), Constants.InternalProtected, DiagnosticId3008));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3007, node.ThisKeyword.GetLocation(), Constants.Internal, DiagnosticId3007));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3009, node.ThisKeyword.GetLocation(), Constants.Protected, DiagnosticId3009));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3007, node.ThisKeyword.GetLocation(), Constants.Internal, DiagnosticId3007));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule3010, node.ThisKeyword.GetLocation(), Constants.Private, DiagnosticId3010));
        }
    }
}