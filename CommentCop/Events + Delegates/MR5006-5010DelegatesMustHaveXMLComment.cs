// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR5006-5010DelegatesMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Delegates
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
        /// The r5006 (const). Value: "5006".
        /// </summary>
        private const string R5006 = "5006";

        /// <summary>
        /// The r5007 (const). Value: "5007".
        /// </summary>
        private const string R5007 = "5007";

        /// <summary>
        /// The r5008 (const). Value: "5008".
        /// </summary>
        private const string R5008 = "5008";

        /// <summary>
        /// The r5009 (const). Value: "5009".
        /// </summary>
        private const string R5009 = "5009";

        /// <summary>
        /// The r5010 (const). Value: "5010".
        /// </summary>
        private const string R5010 = "5010";

        /// <summary>
        /// The diagnostic id5006 (const). Value: Constants.DiagnosticPrefix + R5006.
        /// </summary>
        public const string DiagnosticId5006 = Constants.DiagnosticPrefix + R5006;

        /// <summary>
        /// The diagnostic id5007 (const). Value: Constants.DiagnosticPrefix + R5007.
        /// </summary>
        public const string DiagnosticId5007 = Constants.DiagnosticPrefix + R5007;

        /// <summary>
        /// The diagnostic id5008 (const). Value: Constants.DiagnosticPrefix + R5008.
        /// </summary>
        public const string DiagnosticId5008 = Constants.DiagnosticPrefix + R5008;

        /// <summary>
        /// The diagnostic id5009 (const). Value: Constants.DiagnosticPrefix + R5009.
        /// </summary>
        public const string DiagnosticId5009 = Constants.DiagnosticPrefix + R5009;

        /// <summary>
        /// The diagnostic id5010 (const). Value: Constants.DiagnosticPrefix + R5010.
        /// </summary>
        public const string DiagnosticId5010 = Constants.DiagnosticPrefix + R5010;

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
        /// The help link5006 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5006 + ".md".
        /// </summary>
        private const string HelpLink5006 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5006 + ".md";

        /// <summary>
        /// The help link5007 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5007 + ".md".
        /// </summary>
        private const string HelpLink5007 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5007 + ".md";

        /// <summary>
        /// The help link5008 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5008 + ".md".
        /// </summary>
        private const string HelpLink5008 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5008 + ".md";

        /// <summary>
        /// The help link5009 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5009 + ".md".
        /// </summary>
        private const string HelpLink5009 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5009 + ".md";

        /// <summary>
        /// The help link5010 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5010 + ".md".
        /// </summary>
        private const string HelpLink5010 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5010 + ".md";

        /// <summary>
        /// The readonly rule5006. Value: new DiagnosticDescriptor(DiagnosticId5006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5006 = new DiagnosticDescriptor(DiagnosticId5006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink5006);

        /// <summary>
        /// The readonly rule5007. Value: new DiagnosticDescriptor(DiagnosticId5007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5007 = new DiagnosticDescriptor(DiagnosticId5007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink5007);

        /// <summary>
        /// The readonly rule5008. Value: new DiagnosticDescriptor(DiagnosticId5008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5008 = new DiagnosticDescriptor(DiagnosticId5008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink5008);

        /// <summary>
        /// The readonly rule5009. Value: new DiagnosticDescriptor(DiagnosticId5009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5009 = new DiagnosticDescriptor(DiagnosticId5009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink5009);

        /// <summary>
        /// The readonly rule5010. Value: new DiagnosticDescriptor(DiagnosticId5010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5010 = new DiagnosticDescriptor(DiagnosticId5010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink5010);

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