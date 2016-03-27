// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR5001-5005EventsMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Events
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
    /// MR5001 public events must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR5001_5005EventsMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId5001 = Constants.DiagnosticPrefix + "5001";
        public const string DiagnosticId5002 = Constants.DiagnosticPrefix + "5002";
        public const string DiagnosticId5003 = Constants.DiagnosticPrefix + "5003";
        public const string DiagnosticId5004 = Constants.DiagnosticPrefix + "5004";
        public const string DiagnosticId5005 = Constants.DiagnosticPrefix + "5005";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = " events" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The message.
        /// </summary>
        private const string Message = "{0}" + Title + " ({1})";

        /// <summary>
        /// The rule.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule5001 = new DiagnosticDescriptor(DiagnosticId5001, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule5002 = new DiagnosticDescriptor(DiagnosticId5002, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule5003 = new DiagnosticDescriptor(DiagnosticId5003, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule5004 = new DiagnosticDescriptor(DiagnosticId5004, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule5005 = new DiagnosticDescriptor(DiagnosticId5005, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true);

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
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.EventFieldDeclaration);
        }

        /// <summary>
        /// Check if xml comment exists.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The systax node analysis context.</param>
        private void Check(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
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
    }
}