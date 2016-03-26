// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR0006-MR0010StructsMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Structs
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
    /// MR0006 public structs must have XML comment.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR0006_MR0010StructsMustHaveXMLComment : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId0006 = Constants.DiagnosticPrefix + "0006";
        public const string DiagnosticId0007 = Constants.DiagnosticPrefix + "0007";
        public const string DiagnosticId0008 = Constants.DiagnosticPrefix + "0008";
        public const string DiagnosticId0009 = Constants.DiagnosticPrefix + "0009";
        public const string DiagnosticId0010 = Constants.DiagnosticPrefix + "0010";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title.
        /// </summary>
        private const string Title = " structs" + Constants.MustHaveXmlHeader;

        /// <summary>
        /// The message.
        /// </summary>
        private const string Message = "{0}" + Title + " ({1}).";

        /// <summary>
        /// The rule.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0006 = new DiagnosticDescriptor(DiagnosticId0006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule0007 = new DiagnosticDescriptor(DiagnosticId0007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule0008 = new DiagnosticDescriptor(DiagnosticId0008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule0009 = new DiagnosticDescriptor(DiagnosticId0009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true);
        private static readonly DiagnosticDescriptor Rule0010 = new DiagnosticDescriptor(DiagnosticId0010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true);

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
                    .Any(i => i.StartTag.Name.ToString().Equals("summary"));

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
