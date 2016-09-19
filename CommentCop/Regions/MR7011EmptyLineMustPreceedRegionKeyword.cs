﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR7011EmptyLineMustPreceedRegionKeyword.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Regions
{
    using System.Collections.Immutable;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// The MR7011 Region must be preceeded by blank line class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR7011EmptyLineMustPreceedRegionKeyword : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id.
        /// </summary>
        public const string DiagnosticId7011 = Constants.DiagnosticPrefix + "7011";

        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title.
        /// </summary>
        public const string Title = "#region must be preceeded by a blank line.";

        /// <summary>
        /// The message.
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The rule 7010.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule7011 = new DiagnosticDescriptor(
            DiagnosticId7011,
            Title,
            Message,
            Category,
            DiagnosticSeverity.Warning,
            true);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule7011);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(CheckRegion, SyntaxKind.RegionDirectiveTrivia);
        }

        /// <summary>
        /// Check endregion keyword is preceeded by an empty line.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The syntaxNodeAnalysisContext.</param>
        private void CheckRegion(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            if (CodeCracker.GeneratedCodeAnalysisExtensions.IsGenerated(syntaxNodeAnalysisContext))
            {
                return;
            }

            var node = (RegionDirectiveTriviaSyntax)syntaxNodeAnalysisContext.Node;

            if (node?.ParentTrivia == null)
            {
                return;
            }

            var spanStart = node.Span.Start;
            var parent = node.ParentTrivia.Token.Parent.ChildThatContainsPosition(spanStart - 1);
            var lt = parent.GetLeadingTrivia();

            var idx = lt.IndexOfTrivia(node.FullSpan) - 1;
            if (idx < 0)
            {
                return;
            }

            if (lt[idx].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                idx--;
            }

            if (idx < 0)
            {
                if (node.ParentTrivia.Token.IsKind(SyntaxKind.OpenBraceToken))
                {
                    return;
                }

                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7011, node.GetLocation(), DiagnosticId7011));
                return;
            }

            if (!lt[idx].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule7011, node.GetLocation(), DiagnosticId7011));
            }
        }
    }
}