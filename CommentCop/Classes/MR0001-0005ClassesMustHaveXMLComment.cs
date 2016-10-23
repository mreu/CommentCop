// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR0001-0005ClassesMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Classes
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
    /// MR0001 - MR0005 classes must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR0001_0005ClassesMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The r0001 (const). Value: "0001".
        /// </summary>
        private const string R0001 = "0001";

        /// <summary>
        /// The r0002 (const). Value: "0002".
        /// </summary>
        private const string R0002 = "0002";

        /// <summary>
        /// The r0003 (const). Value: "0003".
        /// </summary>
        private const string R0003 = "0003";

        /// <summary>
        /// The r0004 (const). Value: "0004".
        /// </summary>
        private const string R0004 = "0004";

        /// <summary>
        /// The r0005 (const). Value: "0005".
        /// </summary>
        private const string R0005 = "0005";

        /// <summary>
        /// The diagnostic id0001 (const). Value: Constants.DiagnosticPrefix + R0001.
        /// </summary>
        public const string DiagnosticId0001 = Constants.DiagnosticPrefix + R0001;

        /// <summary>
        /// The diagnostic id0002 (const). Value: Constants.DiagnosticPrefix + R0002.
        /// </summary>
        public const string DiagnosticId0002 = Constants.DiagnosticPrefix + R0002;

        /// <summary>
        /// The diagnostic id0003 (const). Value: Constants.DiagnosticPrefix + R0003.
        /// </summary>
        public const string DiagnosticId0003 = Constants.DiagnosticPrefix + R0003;

        /// <summary>
        /// The diagnostic id0004 (const). Value: Constants.DiagnosticPrefix + R0004.
        /// </summary>
        public const string DiagnosticId0004 = Constants.DiagnosticPrefix + R0004;

        /// <summary>
        /// The diagnostic id0005 (const). Value: Constants.DiagnosticPrefix + R0005.
        /// </summary>
        public const string DiagnosticId0005 = Constants.DiagnosticPrefix + R0005;

        /// <summary>
        /// The category (const). Value: Constants.DiagnosticCategory.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title (const). Value: " classes" + Constants.MustHaveXmlHeader.
        /// </summary>
        private const string Title = " classes" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The message (const). Value: "{0}" + Title + " ({1})".
        /// </summary>
        private const string Message = "{0}" + Title + " ({1})";

        /// <summary>
        /// The rule0001 (readonly). Value: new DiagnosticDescriptor(DiagnosticId0001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0001 = new DiagnosticDescriptor(DiagnosticId0001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink0001);

        /// <summary>
        /// The rule0002 (readonly). Value: new DiagnosticDescriptor(DiagnosticId0002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0002 = new DiagnosticDescriptor(DiagnosticId0002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink0001);

        /// <summary>
        /// The rule0003 (readonly). Value: new DiagnosticDescriptor(DiagnosticId0003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0003 = new DiagnosticDescriptor(DiagnosticId0003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink0001);

        /// <summary>
        /// The rule0004 (readonly). Value: new DiagnosticDescriptor(DiagnosticId0004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0004 = new DiagnosticDescriptor(DiagnosticId0004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink0001);

        /// <summary>
        /// The rule0005 (readonly). Value: new DiagnosticDescriptor(DiagnosticId0005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0005 = new DiagnosticDescriptor(DiagnosticId0005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink0001);

        /// <summary>
        /// The help link0001 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0001 + ".md".
        /// </summary>
        private const string HelpLink0001 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0001 + ".md";

        /// <summary>
        /// The help link0002 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0002 + ".md".
        /// </summary>
        private const string HelpLink0002 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0002 + ".md";

        /// <summary>
        /// The help link0003 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0003 + ".md".
        /// </summary>
        private const string HelpLink0003 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0003 + ".md";

        /// <summary>
        /// The help link0004 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0004 + ".md".
        /// </summary>
        private const string HelpLink0004 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0004 + ".md";

        /// <summary>
        /// The help link0005 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0005 + ".md".
        /// </summary>
        private const string HelpLink0005 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0005 + ".md";

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule0001, Rule0002, Rule0003, Rule0004, Rule0005);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.ClassDeclaration);
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

            var node = syntaxNodeAnalysisContext.Node as ClassDeclarationSyntax;

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
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0001, node.Identifier.GetLocation(), Constants.Public, DiagnosticId0001));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId0005));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0003, node.Identifier.GetLocation(), Constants.InternalProtected, DiagnosticId0003));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId0002));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0004, node.Identifier.GetLocation(), Constants.Protected, DiagnosticId0004));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId0002));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId0005));
        }
    }
}