// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR4001-4005FieldsMustHaveXMLComment.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Fields
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
        /// The r4001 (const). Value: "4001".
        /// </summary>
        private const string R4001 = "4001";

        /// <summary>
        /// The r4002 (const). Value: "4002".
        /// </summary>
        private const string R4002 = "4002";

        /// <summary>
        /// The r4003 (const). Value: "4003".
        /// </summary>
        private const string R4003 = "4003";

        /// <summary>
        /// The r4004 (const). Value: "4004".
        /// </summary>
        private const string R4004 = "4004";

        /// <summary>
        /// The r4005 (const). Value: "4005".
        /// </summary>
        private const string R4005 = "4005";

        /// <summary>
        /// The diagnostic id4001 (const). Value: Constants.DiagnosticPrefix + R4001.
        /// </summary>
        public const string DiagnosticId4001 = Constants.DiagnosticPrefix + R4001;

        /// <summary>
        /// The diagnostic id4002 (const). Value: Constants.DiagnosticPrefix + R4002.
        /// </summary>
        public const string DiagnosticId4002 = Constants.DiagnosticPrefix + R4002;

        /// <summary>
        /// The diagnostic id4003 (const). Value: Constants.DiagnosticPrefix + R4003.
        /// </summary>
        public const string DiagnosticId4003 = Constants.DiagnosticPrefix + R4003;

        /// <summary>
        /// The diagnostic id4004 (const). Value: Constants.DiagnosticPrefix + R4004.
        /// </summary>
        public const string DiagnosticId4004 = Constants.DiagnosticPrefix + R4004;

        /// <summary>
        /// The diagnostic id4005 (const). Value: Constants.DiagnosticPrefix + R4005.
        /// </summary>
        public const string DiagnosticId4005 = Constants.DiagnosticPrefix + R4005;

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
        /// The help link4001 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R4001 + ".md".
        /// </summary>
        private const string HelpLink4001 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R4001 + ".md";

        /// <summary>
        /// The help link4002 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R4002 + ".md".
        /// </summary>
        private const string HelpLink4002 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R4002 + ".md";

        /// <summary>
        /// The help link4003 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R4003 + ".md".
        /// </summary>
        private const string HelpLink4003 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R4003 + ".md";

        /// <summary>
        /// The help link4004 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R4004 + ".md".
        /// </summary>
        private const string HelpLink4004 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R4004 + ".md";

        /// <summary>
        /// The help link4005 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R4005 + ".md".
        /// </summary>
        private const string HelpLink4005 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R4005 + ".md";

        /// <summary>
        /// The readonly rule4001. Value: new DiagnosticDescriptor(DiagnosticId4001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule4001 = new DiagnosticDescriptor(DiagnosticId4001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink4001);

        /// <summary>
        /// The readonly rule4002. Value: new DiagnosticDescriptor(DiagnosticId4002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule4002 = new DiagnosticDescriptor(DiagnosticId4002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink4002);

        /// <summary>
        /// The readonly rule4003. Value: new DiagnosticDescriptor(DiagnosticId4003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule4003 = new DiagnosticDescriptor(DiagnosticId4003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink4003);

        /// <summary>
        /// The readonly rule4004. Value: new DiagnosticDescriptor(DiagnosticId4004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule4004 = new DiagnosticDescriptor(DiagnosticId4004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink4004);

        /// <summary>
        /// The readonly rule4005. Value: new DiagnosticDescriptor(DiagnosticId4005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule4005 = new DiagnosticDescriptor(DiagnosticId4005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink4005);

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
