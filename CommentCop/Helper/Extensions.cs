// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.Text;

    /// <summary>
    /// Extensions used by the analyzer.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Is this trivia a SingleLineDocumentationCommentTrivia or a EndOfLineTrivia.
        /// </summary>
        /// <param name="trivia">The syntax trivia.</param>
        /// <returns>True if yes otherwise false.</returns>
        internal static bool HasBuiltinEndLine(this SyntaxTrivia trivia)
        {
            return trivia.IsDirective
                   || trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                   || trivia.IsKind(SyntaxKind.EndOfLineTrivia);
        }

        /// <summary>
        /// The index of trivia.
        /// </summary>
        /// <param name="triviaList">The syntax trivia list.</param>
        /// <param name="span">The span to find.</param>
        /// <returns>The <see cref="int"/> index of the triva. -1 if not found.</returns>
        internal static int IndexOfTrivia(this SyntaxTriviaList triviaList, TextSpan span)
        {
            for (var ix = 0; ix < triviaList.Count; ix++)
            {
                if (triviaList[ix].FullSpan.Start == span.Start)
                {
                    return ix;
                }
            }

            return -1;
        }
    }
}
