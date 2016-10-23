// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR1101-1106ConstructorsMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Constructors
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
    /// MR1101 - 1106 constructors must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR1101_1106ConstructorsMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The r1101 (const). Value: "1101".
        /// </summary>
        public const string R1101 = "1101";

        /// <summary>
        /// The r1102 (const). Value: "1102".
        /// </summary>
        public const string R1102 = "1102";

        /// <summary>
        /// The r1103 (const). Value: "1103".
        /// </summary>
        public const string R1103 = "1103";

        /// <summary>
        /// The r1104 (const). Value: "1104".
        /// </summary>
        public const string R1104 = "1104";

        /// <summary>
        /// The r1105 (const). Value: "1105".
        /// </summary>
        public const string R1105 = "1105";

        /// <summary>
        /// The r1106 (const). Value: "1106".
        /// </summary>
        public const string R1106 = "1106";

        /// <summary>
        /// The diagnostic id1101 (const). Value: Constants.DiagnosticPrefix + R1101.
        /// </summary>
        public const string DiagnosticId1101 = Constants.DiagnosticPrefix + R1101;

        /// <summary>
        /// The diagnostic id1102 (const). Value: Constants.DiagnosticPrefix + R1102.
        /// </summary>
        public const string DiagnosticId1102 = Constants.DiagnosticPrefix + R1102;

        /// <summary>
        /// The diagnostic id1103 (const). Value: Constants.DiagnosticPrefix + R1103.
        /// </summary>
        public const string DiagnosticId1103 = Constants.DiagnosticPrefix + R1103;

        /// <summary>
        /// The diagnostic id1104 (const). Value: Constants.DiagnosticPrefix + R1104.
        /// </summary>
        public const string DiagnosticId1104 = Constants.DiagnosticPrefix + R1104;

        /// <summary>
        /// The diagnostic id1105 (const). Value: Constants.DiagnosticPrefix + R1105.
        /// </summary>
        public const string DiagnosticId1105 = Constants.DiagnosticPrefix + R1105;

        /// <summary>
        /// The diagnostic id1106 (const). Value: Constants.DiagnosticPrefix + R1106.
        /// </summary>
        public const string DiagnosticId1106 = Constants.DiagnosticPrefix + R1106;

        /// <summary>
        /// The const category. Value: Constants.DiagnosticCategory.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The const title. Value: " constructors" + Constants.MustHaveXmlHeader.
        /// </summary>
        private const string Title = " constructors" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The const message. Value: "{0}" + Title + " ({1})".
        /// </summary>
        private const string Message = "{0}" + Title + " ({1})";

        /// <summary>
        /// The help link1101 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1101 + ".md".
        /// </summary>
        private const string HelpLink1101 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1101 + ".md";

        /// <summary>
        /// The help link1102 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1102 + ".md".
        /// </summary>
        private const string HelpLink1102 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1102 + ".md";

        /// <summary>
        /// The help link1103 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1103 + ".md".
        /// </summary>
        private const string HelpLink1103 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1103 + ".md";

        /// <summary>
        /// The help link1104 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1104 + ".md".
        /// </summary>
        private const string HelpLink1104 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1104 + ".md";

        /// <summary>
        /// The help link1105 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1105 + ".md".
        /// </summary>
        private const string HelpLink1105 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1105 + ".md";

        /// <summary>
        /// The help link1106 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1106 + ".md".
        /// </summary>
        private const string HelpLink1106 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R1106 + ".md";

        /// <summary>
        /// The readonly rule1101. Value: new DiagnosticDescriptor(DiagnosticId1101, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule1101 = new DiagnosticDescriptor(DiagnosticId1101, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink1101);

        /// <summary>
        /// The readonly rule1102. Value: new DiagnosticDescriptor(DiagnosticId1102, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule1102 = new DiagnosticDescriptor(DiagnosticId1102, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink1102);

        /// <summary>
        /// The readonly rule1103. Value: new DiagnosticDescriptor(DiagnosticId1103, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule1103 = new DiagnosticDescriptor(DiagnosticId1103, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink1103);

        /// <summary>
        /// The readonly rule1104. Value: new DiagnosticDescriptor(DiagnosticId1104, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule1104 = new DiagnosticDescriptor(DiagnosticId1104, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink1104);

        /// <summary>
        /// The readonly rule1105. Value: new DiagnosticDescriptor(DiagnosticId1105, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule1105 = new DiagnosticDescriptor(DiagnosticId1105, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink1105);

        /// <summary>
        /// The readonly rule1106. Value: new DiagnosticDescriptor(DiagnosticId1106, Constants.Static + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule1106 = new DiagnosticDescriptor(DiagnosticId1106, Constants.Static + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink1106);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule1101, Rule1102, Rule1103, Rule1104, Rule1105, Rule1106);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.ConstructorDeclaration);
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

            var node = syntaxNodeAnalysisContext.Node as ConstructorDeclarationSyntax;

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
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1101, node.Identifier.GetLocation(), Constants.Public, DiagnosticId1101));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1105, node.Identifier.GetLocation(), Constants.Private, DiagnosticId1105));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.StaticKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1106, node.Identifier.GetLocation(), Constants.Static, DiagnosticId1106));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1103, node.Identifier.GetLocation(), Constants.InternalProtected, DiagnosticId1103));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1102, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId1102));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1104, node.Identifier.GetLocation(), Constants.Protected, DiagnosticId1104));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule1105, node.Identifier.GetLocation(), Constants.Private, DiagnosticId1105));
        }
    }
}