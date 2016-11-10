// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR0006-0010StructsMustHaveXMLComment.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Structs
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
    /// MR0006 - 0010 structs must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR0006_0010StructsMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The r0006 (const). Value: "0006".
        /// </summary>
        private const string R0006 = "0006";

        /// <summary>
        /// The r0007 (const). Value: "0007".
        /// </summary>
        private const string R0007 = "0007";

        /// <summary>
        /// The r0008 (const). Value: "0008".
        /// </summary>
        private const string R0008 = "0008";

        /// <summary>
        /// The r0009 (const). Value: "0009".
        /// </summary>
        private const string R0009 = "0009";

        /// <summary>
        /// The r0010 (const). Value: "0010".
        /// </summary>
        private const string R0010 = "0010";

        /// <summary>
        /// The diagnostic id0006 (const). Value: Constants.DiagnosticPrefix + R0006.
        /// </summary>
        public const string DiagnosticId0006 = Constants.DiagnosticPrefix + R0006;

        /// <summary>
        /// The diagnostic id0007 (const). Value: Constants.DiagnosticPrefix + R0007.
        /// </summary>
        public const string DiagnosticId0007 = Constants.DiagnosticPrefix + R0007;

        /// <summary>
        /// The diagnostic id0008 (const). Value: Constants.DiagnosticPrefix + R0008.
        /// </summary>
        public const string DiagnosticId0008 = Constants.DiagnosticPrefix + R0008;

        /// <summary>
        /// The diagnostic id0009 (const). Value: Constants.DiagnosticPrefix + R0009.
        /// </summary>
        public const string DiagnosticId0009 = Constants.DiagnosticPrefix + R0009;

        /// <summary>
        /// The diagnostic id0010 (const). Value: Constants.DiagnosticPrefix + R0010.
        /// </summary>
        public const string DiagnosticId0010 = Constants.DiagnosticPrefix + R0010;

        /// <summary>
        /// The help link0006 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0006 + ".md".
        /// </summary>
        private const string HelpLink0006 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0006 + ".md";

        /// <summary>
        /// The help link0007 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0007 + ".md".
        /// </summary>
        private const string HelpLink0007 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0007 + ".md";

        /// <summary>
        /// The help link0008 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0008 + ".md".
        /// </summary>
        private const string HelpLink0008 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0008 + ".md";

        /// <summary>
        /// The help link0009 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0009 + ".md".
        /// </summary>
        private const string HelpLink0009 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0009 + ".md";

        /// <summary>
        /// The help link0100 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0010 + ".md".
        /// </summary>
        private const string HelpLink0010 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R0010 + ".md";

        /// <summary>
        /// The const category. Value: Constants.DiagnosticCategory.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The const title. Value: " structs" + Constants.MustHaveXmlHeader.
        /// </summary>
        private const string Title = " structs" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The const message. Value: "{0}" + Title + " ({1})".
        /// </summary>
        private const string Message = "{0}" + Title + " ({1})";

        /// <summary>
        /// The readonly rule0006. Value: new DiagnosticDescriptor(DiagnosticId0006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0006 = new DiagnosticDescriptor(DiagnosticId0006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink0006);

        /// <summary>
        /// The readonly rule0007. Value: new DiagnosticDescriptor(DiagnosticId0007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0007 = new DiagnosticDescriptor(DiagnosticId0007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink0007);

        /// <summary>
        /// The readonly rule0008. Value: new DiagnosticDescriptor(DiagnosticId0008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0008 = new DiagnosticDescriptor(DiagnosticId0008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink0008);

        /// <summary>
        /// The readonly rule0009. Value: new DiagnosticDescriptor(DiagnosticId0009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0009 = new DiagnosticDescriptor(DiagnosticId0009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink0009);

        /// <summary>
        /// The readonly rule0010. Value: new DiagnosticDescriptor(DiagnosticId0010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0010 = new DiagnosticDescriptor(DiagnosticId0010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink0010);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule0006, Rule0007, Rule0008, Rule0009, Rule0010);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.StructDeclaration);
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

            var node = syntaxNodeAnalysisContext.Node as StructDeclarationSyntax;

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
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0006, node.Identifier.GetLocation(), Constants.Public, DiagnosticId0006));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0010, node.Identifier.GetLocation(), Constants.Private, DiagnosticId0010));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0008, node.Identifier.GetLocation(), Constants.InternalProtected, DiagnosticId0008));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0007, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId0007));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0009, node.Identifier.GetLocation(), Constants.Protected, DiagnosticId0009));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0007, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId0007));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule0010, node.Identifier.GetLocation(), Constants.Private, DiagnosticId0010));
        }
    }
}
