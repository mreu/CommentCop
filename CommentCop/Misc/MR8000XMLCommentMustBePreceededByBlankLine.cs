// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MR8000XMLCommentMustBePreceededByBlankLine.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Misc
{
    using System.Collections.Immutable;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// MR8000XML comment must be preceeding by blank line class.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MR8000XMLCommentMustBePreceededByBlankLine : DiagnosticAnalyzer
    {
        /// <summary>
        /// The diagnostic id (const). Value: "MR8000".
        /// </summary>
        public const string DiagnosticId8000 = Constants.DiagnosticPrefix + "8000";

        /// <summary>
        /// The category (const). Value: "Documentation".
        /// </summary>
        private const string Category = Constants.DiagnosticCategory;

        /// <summary>
        /// The title (const). Value: "Operators must have a xml documentation header.".
        /// </summary>
        public const string Title = "XML Comment must be preceeded by blank line.";

        /// <summary>
        /// The message (readonly). Value: $"{Title} ({DiagnosticId})".
        /// </summary>
        private static readonly string Message = $"{Title} ({{0}})";

        /// <summary>
        /// The help link (const). Value: "https://github.com/mreu/CommentCop/blob/master/Documentation/MR8000.md".
        /// </summary>
        private const string HelpLink = "https://github.com/mreu/CommentCop/blob/master/Documentation/MR8000.md";

        /// <summary>
        /// The rule.
        /// </summary>
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId8000,
            Title,
            Message,
            Category,
            DiagnosticSeverity.Warning,
            true,
            null,
            HelpLink);

        /// <summary>
        /// Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(Check, SyntaxKind.SingleLineDocumentationCommentTrivia);
        }

        /// <summary>
        /// Check if xml comment is preceeded by blank line.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The systax node analysis context.</param>
        private void Check(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext)
        {
            if (CodeCracker.GeneratedCodeAnalysisExtensions.IsGenerated(syntaxNodeAnalysisContext))
            {
                return;
            }

            var node = syntaxNodeAnalysisContext.Node;
            var parentToken = node.ParentTrivia.Token;

            var triviaList = parentToken.LeadingTrivia;

            var idx = triviaList.IndexOfTrivia(node.FullSpan) - 1;
            if (idx < 0)
            {
                return;
            }

            if (triviaList[idx].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                if (--idx < 0)
                {
                    var prev = parentToken.GetPreviousToken();

                    if (prev.IsKind(SyntaxKind.OpenBraceToken))
                    {
                        return;
                    }

                    ShowDiagnostic(syntaxNodeAnalysisContext, triviaList);
                }
            }

            // if #region is preceeding a comment, no check here
            if (triviaList[idx].IsKind(SyntaxKind.RegionDirectiveTrivia))
            {
                return;
            }

            // if blank line preceeding the comment, it's ok
            if (triviaList[idx].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                return;
            }

            ShowDiagnostic(syntaxNodeAnalysisContext, triviaList);
        }

        /// <summary>
        /// Show the diagnostic.
        /// </summary>
        /// <param name="syntaxNodeAnalysisContext">The syntaxNodeAnalysisContext.</param>
        /// <param name="triviaList">The triviaList.</param>
        private static void ShowDiagnostic(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, SyntaxTriviaList triviaList)
        {
            var node = syntaxNodeAnalysisContext.Node;

            var idx = triviaList.IndexOfTrivia(node.FullSpan) - 1;

            Location loc;
            if (idx < 0)
            {
                loc = node.GetLocation();
            }
            else
            {
                loc = Location.Create(
                    node.SyntaxTree,
                    new TextSpan(triviaList[idx].FullSpan.Start, triviaList[idx].FullSpan.Length + triviaList[idx + 1].FullSpan.Length));
            }

            syntaxNodeAnalysisContext.ReportDiagnostic(Diagnostic.Create(Rule, loc, DiagnosticId8000));
        }
    }
}
