// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR0006-0010StructsMustHaveXMLComment.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
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
        /// The const diagnostic id0006. Value: Constants.DiagnosticPrefix + "0006".
        /// </summary>
        public const string DiagnosticId0006 = Constants.DiagnosticPrefix + "0006";

        /// <summary>
        /// The const diagnostic id0007. Value: Constants.DiagnosticPrefix + "0007".
        /// </summary>
        public const string DiagnosticId0007 = Constants.DiagnosticPrefix + "0007";

        /// <summary>
        /// The const diagnostic id0008. Value: Constants.DiagnosticPrefix + "0008".
        /// </summary>
        public const string DiagnosticId0008 = Constants.DiagnosticPrefix + "0008";

        /// <summary>
        /// The const diagnostic id0009. Value: Constants.DiagnosticPrefix + "0009".
        /// </summary>
        public const string DiagnosticId0009 = Constants.DiagnosticPrefix + "0009";

        /// <summary>
        /// The const diagnostic id0010. Value: Constants.DiagnosticPrefix + "0010".
        /// </summary>
        public const string DiagnosticId0010 = Constants.DiagnosticPrefix + "0010";

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
        private static readonly DiagnosticDescriptor Rule0006 = new DiagnosticDescriptor(DiagnosticId0006, Constants.Public + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule0007. Value: new DiagnosticDescriptor(DiagnosticId0007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0007 = new DiagnosticDescriptor(DiagnosticId0007, Constants.Internal + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule0008. Value: new DiagnosticDescriptor(DiagnosticId0008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0008 = new DiagnosticDescriptor(DiagnosticId0008, Constants.InternalProtected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule0009. Value: new DiagnosticDescriptor(DiagnosticId0009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
        private static readonly DiagnosticDescriptor Rule0009 = new DiagnosticDescriptor(DiagnosticId0009, Constants.Protected + Title, Message, Category, DiagnosticSeverity.Warning, true);

        /// <summary>
        /// The readonly rule0010. Value: new DiagnosticDescriptor(DiagnosticId0010, Constants.Private + Title, Message, Category, DiagnosticSeverity.Warning, true).
        /// </summary>
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
