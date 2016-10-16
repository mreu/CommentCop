// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR6001-6005EnumsMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Enums
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
    /// MR6001 - 6005 enums must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR6001_6005EnumsMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The r6001 (const). Value: "6001".
        /// </summary>
        private const string R6001 = "6001";

        /// <summary>
        /// The r6002 (const). Value: "6002".
        /// </summary>
        private const string R6002 = "6002";

        /// <summary>
        /// The r6003 (const). Value: "6003".
        /// </summary>
        private const string R6003 = "6003";

        /// <summary>
        /// The r6004 (const). Value: "6004".
        /// </summary>
        private const string R6004 = "6004";

        /// <summary>
        /// The r6005 (const). Value: "6005".
        /// </summary>
        private const string R6005 = "6005";

        /// <summary>
        /// The diagnostic id6001 (const). Value: Constants.DiagnosticPrefix + "6001".
        /// </summary>
        public const string DiagnosticId6001 = Constants.DiagnosticPrefix + "6001";

        /// <summary>
        /// The diagnostic id6002 (const). Value: Constants.DiagnosticPrefix + "6002".
        /// </summary>
        public const string DiagnosticId6002 = Constants.DiagnosticPrefix + "6002";

        /// <summary>
        /// The diagnostic id6003 (const). Value: Constants.DiagnosticPrefix + "6003".
        /// </summary>
        public const string DiagnosticId6003 = Constants.DiagnosticPrefix + "6003";

        /// <summary>
        /// The diagnostic id6004 (const). Value: Constants.DiagnosticPrefix + "6004".
        /// </summary>
        public const string DiagnosticId6004 = Constants.DiagnosticPrefix + "6004";

        /// <summary>
        /// The diagnostic id6005 (const). Value: Constants.DiagnosticPrefix + "6005".
        /// </summary>
        public const string DiagnosticId6005 = Constants.DiagnosticPrefix + "6005";

        /// <summary>
        /// The help link6001 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R6001 + ".md".
        /// </summary>
        private const string HelpLink6001 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R6001 + ".md";

        /// <summary>
        /// The help link6002 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R6002 + ".md".
        /// </summary>
        private const string HelpLink6002 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R6002 + ".md";

        /// <summary>
        /// The help link6003 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R6003 + ".md".
        /// </summary>
        private const string HelpLink6003 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R6003 + ".md";

        /// <summary>
        /// The help link6004 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R6004 + ".md".
        /// </summary>
        private const string HelpLink6004 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R6004 + ".md";

        /// <summary>
        /// The help link6005 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R6005 + ".md".
        /// </summary>
        private const string HelpLink6005 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R6005 + ".md";

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
        /// The readonly rule6001. Value: new DiagnosticDescriptor(DiagnosticId6001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule6001 = new DiagnosticDescriptor(DiagnosticId6001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink6001);

        /// <summary>
        /// The readonly rule6002. Value: new DiagnosticDescriptor(DiagnosticId6002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule6002 = new DiagnosticDescriptor(DiagnosticId6002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink6002);

        /// <summary>
        /// The readonly rule6003. Value: new DiagnosticDescriptor(DiagnosticId6003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule6003 = new DiagnosticDescriptor(DiagnosticId6003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink6003);

        /// <summary>
        /// The readonly rule6004. Value: new DiagnosticDescriptor(DiagnosticId6004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule6004 = new DiagnosticDescriptor(DiagnosticId6004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink6004);

        /// <summary>
        /// The readonly rule6005. Value: new DiagnosticDescriptor(DiagnosticId6005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule6005 = new DiagnosticDescriptor(DiagnosticId6005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink6005);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule6001, Rule6002, Rule6003, Rule6004, Rule6005);

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
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule6001, node.Identifier.GetLocation(), Constants.Public, DiagnosticId6001));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule6005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId6005));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule6003, node.Identifier.GetLocation(), Constants.InternalProtected, DiagnosticId6003));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule6002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId6002));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule6004, node.Identifier.GetLocation(), Constants.Protected, DiagnosticId6004));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule6002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId6002));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule6005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId6005));
        }
    }
}