// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR3006-3010IndexersMustHaveXMLComment.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Property
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
        /// The r3006 (const). Value: "3006".
        /// </summary>
        private const string R3006 = "3006";

        /// <summary>
        /// The r3007 (const). Value: "3007".
        /// </summary>
        private const string R3007 = "3007";

        /// <summary>
        /// The r3008 (const). Value: "3008".
        /// </summary>
        private const string R3008 = "3008";

        /// <summary>
        /// The r3009 (const). Value: "3009".
        /// </summary>
        private const string R3009 = "3009";

        /// <summary>
        /// The r3010 (const). Value: "3010".
        /// </summary>
        private const string R3010 = "3010";

        /// <summary>
        /// The diagnostic id3006 (const). Value: Constants.DiagnosticPrefix + R3006.
        /// </summary>
        public const string DiagnosticId3006 = Constants.DiagnosticPrefix + R3006;

        /// <summary>
        /// The diagnostic id3007 (const). Value: Constants.DiagnosticPrefix + R3007.
        /// </summary>
        public const string DiagnosticId3007 = Constants.DiagnosticPrefix + R3007;

        /// <summary>
        /// The diagnostic id3008 (const). Value: Constants.DiagnosticPrefix + R3008.
        /// </summary>
        public const string DiagnosticId3008 = Constants.DiagnosticPrefix + R3008;

        /// <summary>
        /// The diagnostic id3009 (const). Value: Constants.DiagnosticPrefix + R3009.
        /// </summary>
        public const string DiagnosticId3009 = Constants.DiagnosticPrefix + R3009;

        /// <summary>
        /// The diagnostic id3010 (const). Value: Constants.DiagnosticPrefix + R3010.
        /// </summary>
        public const string DiagnosticId3010 = Constants.DiagnosticPrefix + R3010;

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
        /// The help link3006 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R3006 + ".md".
        /// </summary>
        private const string HelpLink3006 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R3006 + ".md";

        /// <summary>
        /// The help link3007 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R3007 + ".md".
        /// </summary>
        private const string HelpLink3007 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R3007 + ".md";

        /// <summary>
        /// The help link3008 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R3008 + ".md".
        /// </summary>
        private const string HelpLink3008 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R3008 + ".md";

        /// <summary>
        /// The help link3009 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R3009 + ".md".
        /// </summary>
        private const string HelpLink3009 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R3009 + ".md";

        /// <summary>
        /// The help link3010 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R3010 + ".md".
        /// </summary>
        private const string HelpLink3010 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R3010 + ".md";

        /// <summary>
        /// The readonly rule3006. Value: new DiagnosticDescriptor(DiagnosticId3006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3006 = new DiagnosticDescriptor(DiagnosticId3006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink3006);

        /// <summary>
        /// The readonly rule3007. Value: new DiagnosticDescriptor(DiagnosticId3007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3007 = new DiagnosticDescriptor(DiagnosticId3007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink3007);

        /// <summary>
        /// The readonly rule3008. Value: new DiagnosticDescriptor(DiagnosticId3008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3008 = new DiagnosticDescriptor(DiagnosticId3008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink3008);

        /// <summary>
        /// The readonly rule3009. Value: new DiagnosticDescriptor(DiagnosticId3009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3009 = new DiagnosticDescriptor(DiagnosticId3009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink3009);

        /// <summary>
        /// The readonly rule3010. Value: new DiagnosticDescriptor(DiagnosticId3010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule3010 = new DiagnosticDescriptor(DiagnosticId3010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink3010);

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