// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

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
    }
}
