// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR5001-5005EventsMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Events
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
    /// MR5001 - 5005 events must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR5001_5005EventsMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The r5001 (const). Value: "5001".
        /// </summary>
        private const string R5001 = "5001";

        /// <summary>
        /// The r5002 (const). Value: "5002".
        /// </summary>
        private const string R5002 = "5002";

        /// <summary>
        /// The r5003 (const). Value: "5003".
        /// </summary>
        private const string R5003 = "5003";

        /// <summary>
        /// The r5004 (const). Value: "5004".
        /// </summary>
        private const string R5004 = "5004";

        /// <summary>
        /// The r5005 (const). Value: "5005".
        /// </summary>
        private const string R5005 = "5005";

        /// <summary>
        /// The diagnostic id5001 (const). Value: Constants.DiagnosticPrefix + R5001.
        /// </summary>
        public const string DiagnosticId5001 = Constants.DiagnosticPrefix + R5001;

        /// <summary>
        /// The diagnostic id5002 (const). Value: Constants.DiagnosticPrefix + R5002.
        /// </summary>
        public const string DiagnosticId5002 = Constants.DiagnosticPrefix + R5002;

        /// <summary>
        /// The diagnostic id5003 (const). Value: Constants.DiagnosticPrefix + R5003.
        /// </summary>
        public const string DiagnosticId5003 = Constants.DiagnosticPrefix + R5003;

        /// <summary>
        /// The diagnostic id5004 (const). Value: Constants.DiagnosticPrefix + R5004.
        /// </summary>
        public const string DiagnosticId5004 = Constants.DiagnosticPrefix + R5004;

        /// <summary>
        /// The diagnostic id5005 (const). Value: Constants.DiagnosticPrefix + R5005.
        /// </summary>
        public const string DiagnosticId5005 = Constants.DiagnosticPrefix + R5005;

        /// <summary>
        /// The category (const). Value: Constants.DiagnosticCategory.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title (const). Value: " events" + Constants.MustHaveXmlHeader.
        /// </summary>
        private const string Title = " events" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The message (const). Value: "{0}" + Title + " ({1})".
        /// </summary>
        private const string Message = "{0}" + Title + " ({1})";

        /// <summary>
        /// The help link5001 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5001 + ".md".
        /// </summary>
        private const string HelpLink5001 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5001 + ".md";

        /// <summary>
        /// The help link5002 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5002 + ".md".
        /// </summary>
        private const string HelpLink5002 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5002 + ".md";

        /// <summary>
        /// The help link5003 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5003 + ".md".
        /// </summary>
        private const string HelpLink5003 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5003 + ".md";

        /// <summary>
        /// The help link5004 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5004 + ".md".
        /// </summary>
        private const string HelpLink5004 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5004 + ".md";

        /// <summary>
        /// The help link5005 (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5005 + ".md".
        /// </summary>
        private const string HelpLink5005 = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR" + R5005 + ".md";

        /// <summary>
        /// The rule5001 (readonly). Value: new DiagnosticDescriptor(DiagnosticId5001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5001 = new DiagnosticDescriptor(DiagnosticId5001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink5001);

        /// <summary>
        /// The rule5002 (readonly). Value: new DiagnosticDescriptor(DiagnosticId5002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5002 = new DiagnosticDescriptor(DiagnosticId5002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink5002);

        /// <summary>
        /// The rule5003 (readonly). Value: new DiagnosticDescriptor(DiagnosticId5003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5003 = new DiagnosticDescriptor(DiagnosticId5003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink5003);

        /// <summary>
        /// The rule5004 (readonly). Value: new DiagnosticDescriptor(DiagnosticId5004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5004 = new DiagnosticDescriptor(DiagnosticId5004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink5004);

        /// <summary>
        /// The rule5005 (readonly). Value: new DiagnosticDescriptor(DiagnosticId5005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5005 = new DiagnosticDescriptor(DiagnosticId5005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true, null, HelpLink5005);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule5001, Rule5002, Rule5003, Rule5004, Rule5005);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(CheckField, SyntaxKind.EventFieldDeclaration);
            context.RegisterSyntaxNodeAction(CheckEvent, SyntaxKind.EventDeclaration);
        }

        /// <summary>
        /// Check if xml comment exists.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The systax node analysis context.</param>
        private void CheckField(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            if (CodeCracker.GeneratedCodeAnalysisExtensions.IsGenerated(syntaxNodeAnalysisContext))
            {
                return;
            }

            var node = syntaxNodeAnalysisContext.Node as EventFieldDeclarationSyntax;

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

            var field = node.DescendantNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault();

            if (node.Modifiers.Any(SyntaxKind.PublicKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5001, field.GetLocation(), Constants.Public, DiagnosticId5001));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5005, field.GetLocation(), Constants.Private, DiagnosticId5005));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5003, field.GetLocation(), Constants.InternalProtected, DiagnosticId5003));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5002, field.GetLocation(), Constants.Internal, DiagnosticId5002));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5004, field.GetLocation(), Constants.Protected, DiagnosticId5004));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5002, field.GetLocation(), Constants.Internal, DiagnosticId5002));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5005, field.GetLocation(), Constants.Private, DiagnosticId5005));
        }

        /// <summary>
        /// Check if xml comment exists.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The systax node analysis context.</param>
        private void CheckEvent(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            var node = syntaxNodeAnalysisContext.Node as EventDeclarationSyntax;

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
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5001, node.Identifier.GetLocation(), Constants.Public, DiagnosticId5001));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.PrivateKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId5005));
                return;
            }

            if (node.Modifiers.Any(SyntaxKind.InternalKeyword))
            {
                if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5003, node.Identifier.GetLocation(), Constants.InternalProtected, DiagnosticId5003));
                }
                else
                {
                    syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId5002));
                }

                return;
            }

            if (node.Modifiers.Any(SyntaxKind.ProtectedKeyword))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5004, node.Identifier.GetLocation(), Constants.Protected, DiagnosticId5004));
                return;
            }

            if (node.Parent is NamespaceDeclarationSyntax)
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5002, node.Identifier.GetLocation(), Constants.Internal, DiagnosticId5002));
                return;
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule5005, node.Identifier.GetLocation(), Constants.Private, DiagnosticId5005));
        }
    }
}