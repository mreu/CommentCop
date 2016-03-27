// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR1001-1005MethodsMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright � 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Methods
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
    /// MR1001 - 1005 methods must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR1001_1005MethodsMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The const diagnostic id1001. Value: Constants.DiagnosticPrefix + "1001".
        /// </summary>
        public const string DiagnosticId1001 = Constants.DiagnosticPrefix + "1001";

        /// <summary>
        /// The const diagnostic id1002. Value: Constants.DiagnosticPrefix + "1002".
        /// </summary>
        public const string DiagnosticId1002 = Constants.DiagnosticPrefix + "1002";

        /// <summary>
        /// The const diagnostic id1003. Value: Constants.DiagnosticPrefix + "1003".
        /// </summary>
        public const string DiagnosticId1003 = Constants.DiagnosticPrefix + "1003";

        /// <summary>
        /// The const diagnostic id1004. Value: Constants.DiagnosticPrefix + "1004".
        /// </summary>
        public const string DiagnosticId1004 = Constants.DiagnosticPrefix + "1004";

        /// <summary>
        /// The const diagnostic id1005. Value: Constants.DiagnosticPrefix + "1005".
        /// </summary>
        public const string DiagnosticId1005 = Constants.DiagnosticPrefix + "1005";

        /// <summary>
        /// The const category. Value: Constants.DiagnosticCategory.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The const title. Value: " methods" + Constants.MustHaveXmlHeader.
        /// </summary>
        private const string Title = " methods" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The const message. Value: "{0}" + Title + " ({1})".
        /// </summary>
        private const string Message = "{0}" + Title + " ({1})";

        /// <summary>
        /// The readonly rule1001. Value: new DiagnosticDescriptor(DiagnosticId1001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule1001 = new DiagnosticDescriptor(DiagnosticId1001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule1002. Value: new DiagnosticDescriptor(DiagnosticId1002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule1002 = new DiagnosticDescriptor(DiagnosticId1002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule1003. Value: new DiagnosticDescriptor(DiagnosticId1003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule1003 = new DiagnosticDescriptor(DiagnosticId1003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule1004. Value: new DiagnosticDescriptor(DiagnosticId1004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule1004 = new DiagnosticDescriptor(DiagnosticId1004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule1005. Value: new DiagnosticDescriptor(DiagnosticId1005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule1005 = new DiagnosticDescriptor(DiagnosticId1005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule1001, Rule1002, Rule1003, Rule1004, Rule1005);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.MethodDeclaration);
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

            var node = syntaxNodeAnalysisContext.Node as MethodDeclarationSyntax;

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
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1001, node.Identifier.GetLocation(), Constants.Public, DiagnosticId1001));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId1005));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1003, node.Identifier.GetLocation(), Constants.InternalProtected, DiagnosticId1003));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId1002));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1004, node.Identifier.GetLocation(), Constants.Protected, DiagnosticId1004));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId1002));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId1005));
        }
    }
}