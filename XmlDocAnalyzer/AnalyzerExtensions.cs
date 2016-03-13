// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace XmlDocAnalyzer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// Provides extension methods to deal for analyzers.
    /// </summary>
    internal static class AnalyzerExtensions
    {
        /// <summary>
        /// A cache of the result of computing whether a document has an auto-generated header.
        /// </summary>
        /// <remarks>
        /// This allows many analyzers that run on every token in the file to avoid checking
        /// the same state in the document repeatedly.
        /// </remarks>
        private static Tuple<WeakReference<Compilation>, ConcurrentDictionary<SyntaxTree, bool>> generatedHeaderCache =
            Tuple.Create(new WeakReference<Compilation>(null), default(ConcurrentDictionary<SyntaxTree, bool>));

        /// <summary>
        /// Register an action to be executed at completion of parsing of a code document. A syntax tree action reports
        /// diagnostics about the <see cref="SyntaxTree"/> of a document.
        /// </summary>
        /// <remarks>This method honors exclusions.</remarks>
        /// <param name="context">The analysis context.</param>
        /// <param name="action">Action to be executed at completion of parsing of a document.</param>
        public static void RegisterSyntaxTreeActionHonorExclusions(this CompilationStartAnalysisContext context, Action<SyntaxTreeAnalysisContext> action)
        {
            Compilation compilation = context.Compilation;
            ConcurrentDictionary<SyntaxTree, bool> cache = GetOrCreateGeneratedDocumentCache(compilation);

            context.RegisterSyntaxTreeAction(
                c =>
                {
                    if (c.IsGeneratedDocument(cache))
                    {
                        return;
                    }

                    // Honor the containing document item's ExcludeFromStylecop=True
                    // MSBuild metadata, if analyzers have access to it.
                    //// TODO: code here

                    action(c);
                });
        }

        /// <summary>
        /// Gets or creates a cache which can be used with <see cref="GeneratedCodeAnalysisExtensions"/> methods to
        /// efficiently determine whether or not a source file is considered generated.
        /// </summary>
        /// <param name="compilation">The compilation which the cache applies to.</param>
        /// <returns>A cache which tracks the syntax trees in a compilation which are considered generated.</returns>
        public static ConcurrentDictionary<SyntaxTree, bool> GetOrCreateGeneratedDocumentCache(this Compilation compilation)
        {
            var headerCache = generatedHeaderCache;

            Compilation cachedCompilation;
            if (!headerCache.Item1.TryGetTarget(out cachedCompilation) || cachedCompilation != compilation)
            {
                var replacementCache = Tuple.Create(new WeakReference<Compilation>(compilation), new ConcurrentDictionary<SyntaxTree, bool>());
                while (true)
                {
                    var prior = Interlocked.CompareExchange(ref generatedHeaderCache, replacementCache, headerCache);
                    if (prior == headerCache)
                    {
                        headerCache = replacementCache;
                        break;
                    }

                    headerCache = prior;
                    if (headerCache.Item1.TryGetTarget(out cachedCompilation) && cachedCompilation == compilation)
                    {
                        break;
                    }
                }
            }

            return headerCache.Item2;
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a <see cref="SyntaxNode"/> with an
        /// appropriate kind. A syntax node action can report diagnostics about a <see cref="SyntaxNode"/>, and can also
        /// collect state information to be used by other syntax node actions or code block end actions.
        /// </summary>
        /// <remarks>This method honors exclusions.</remarks>
        /// <param name="context">Action will be executed only if the kind of a <see cref="SyntaxNode"/> matches
        /// <paramref name="syntaxKind"/>.</param>
        /// <param name="action">Action to be executed at completion of semantic analysis of a
        /// <see cref="SyntaxNode"/>.</param>
        /// <param name="syntaxKind">The kind of syntax that should be analyzed.</param>
        /// <typeparam name="TLanguageKindEnum">Enum type giving the syntax node kinds of the source language for which
        /// the action applies.</typeparam>
        public static void RegisterSyntaxNodeActionHonorExclusions<TLanguageKindEnum>(
            this CompilationStartAnalysisContext context,
            Action<SyntaxNodeAnalysisContext> action,
            TLanguageKindEnum syntaxKind)
            where TLanguageKindEnum : struct
        {
            context.RegisterSyntaxNodeActionHonorExclusions(action, LanguageKindArrays<TLanguageKindEnum>.GetOrCreateArray(syntaxKind));
        }

        /// <summary>
        /// Register an action to be executed at completion of semantic analysis of a <see cref="SyntaxNode"/> with an
        /// appropriate kind. A syntax node action can report diagnostics about a <see cref="SyntaxNode"/>, and can also
        /// collect state information to be used by other syntax node actions or code block end actions.
        /// </summary>
        /// <remarks>This method honors exclusions.</remarks>
        /// <param name="context">Action will be executed only if the kind of a <see cref="SyntaxNode"/> matches one of
        /// the <paramref name="syntaxKinds"/> values.</param>
        /// <param name="action">Action to be executed at completion of semantic analysis of a
        /// <see cref="SyntaxNode"/>.</param>
        /// <param name="syntaxKinds">The kinds of syntax that should be analyzed.</param>
        /// <typeparam name="TLanguageKindEnum">Enum type giving the syntax node kinds of the source language for which
        /// the action applies.</typeparam>
        public static void RegisterSyntaxNodeActionHonorExclusions<TLanguageKindEnum>(
            this CompilationStartAnalysisContext context,
            Action<SyntaxNodeAnalysisContext> action,
            ImmutableArray<TLanguageKindEnum> syntaxKinds)
            where TLanguageKindEnum : struct
        {
            Compilation compilation = context.Compilation;
            ConcurrentDictionary<SyntaxTree, bool> cache = GetOrCreateGeneratedDocumentCache(compilation);

            context.RegisterSyntaxNodeAction(
                c =>
                {
                    if (c.IsGenerated(cache))
                    {
                        return;
                    }

                    // Honor the containing document item's ExcludeFromStylecop=True
                    // MSBuild metadata, if analyzers have access to it.
                    //// TODO: code here

                    action(c);
                },
                syntaxKinds);
        }

        /// <summary>
        /// Checks whether the given node or its containing document is auto generated by a tool.
        /// </summary>
        /// <remarks>
        /// <para>This method uses <see cref="IsGeneratedDocument(SyntaxTree, ConcurrentDictionary{SyntaxTree, bool}, CancellationToken)"/> to determine which
        /// code is considered "generated".</para>
        /// </remarks>
        /// <param name="context">The analysis context for a <see cref="SyntaxNode"/>.</param>
        /// <param name="cache">The concurrent results cache.</param>
        /// <returns>
        /// <para><see langword="true"/> if the <see cref="SyntaxNode"/> contained in <paramref name="context"/> is
        /// located in generated code; otherwise, <see langword="false"/>.</para>
        /// </returns>
        internal static bool IsGenerated(this SyntaxNodeAnalysisContext context, ConcurrentDictionary<SyntaxTree, bool> cache)
        {
            return IsGeneratedDocument(context.Node.SyntaxTree, cache, context.CancellationToken);
        }

        /// <summary>
        /// Checks whether the given document is auto generated by a tool.
        /// </summary>
        /// <remarks>
        /// <para>This method uses <see cref="IsGeneratedDocument(SyntaxTree, ConcurrentDictionary{SyntaxTree, bool}, CancellationToken)"/> to determine which
        /// code is considered "generated".</para>
        /// </remarks>
        /// <param name="context">The analysis context for a <see cref="SyntaxTree"/>.</param>
        /// <param name="cache">The concurrent results cache.</param>
        /// <returns>
        /// <para><see langword="true"/> if the <see cref="SyntaxTree"/> contained in <paramref name="context"/> is
        /// located in generated code; otherwise, <see langword="false"/>.</para>
        /// </returns>
        internal static bool IsGeneratedDocument(this SyntaxTreeAnalysisContext context, ConcurrentDictionary<SyntaxTree, bool> cache)
        {
            return IsGeneratedDocument(context.Tree, cache, context.CancellationToken);
        }

        /// <summary>
        /// Checks whether the given document is auto generated by a tool
        /// (based on filename or comment header).
        /// </summary>
        /// <remarks>
        /// <para>The exact conditions used to identify generated code are subject to change in future releases. The current algorithm uses the following checks.</para>
        /// <para>Code is considered generated if it meets any of the following conditions.</para>
        /// <list type="bullet">
        /// <item>The code is contained in a file which starts with a comment containing the text
        /// <c>&lt;auto-generated</c>.</item>
        /// <item>The code is contained in a file with a name matching certain patterns (case-insensitive):
        /// <list type="bullet">
        /// <item>*.designer.cs</item>
        /// </list>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="tree">The syntax tree to examine.</param>
        /// <param name="cache">The concurrent results cache.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para><see langword="true"/> if <paramref name="tree"/> is located in generated code; otherwise,
        /// <see langword="false"/>. If <paramref name="tree"/> is <see langword="null"/>, this method returns
        /// <see langword="false"/>.</para>
        /// </returns>
        public static bool IsGeneratedDocument(this SyntaxTree tree, ConcurrentDictionary<SyntaxTree, bool> cache, CancellationToken cancellationToken)
        {
            if (tree == null)
            {
                return false;
            }

            bool result;
            if (cache.TryGetValue(tree, out result))
            {
                return result;
            }

            bool generated = IsGeneratedDocumentNoCache(tree, cancellationToken);
            cache.TryAdd(tree, generated);
            return generated;
        }

        private static bool IsGeneratedDocumentNoCache(SyntaxTree tree, CancellationToken cancellationToken)
        {
            return IsGeneratedFileName(tree.FilePath)
                || HasAutoGeneratedComment(tree, cancellationToken)
                || IsEmpty(tree, cancellationToken);
        }
        /// <summary>
        /// Checks whether the given document has a filename that indicates it is a generated file.
        /// </summary>
        /// <param name="filePath">The source file name, without any path.</param>
        /// <returns>
        /// <para><see langword="true"/> if <paramref name="filePath"/> is the name of a generated file; otherwise,
        /// <see langword="false"/>.</para>
        /// </returns>
        /// <seealso cref="IsGeneratedDocument(SyntaxTree, ConcurrentDictionary{SyntaxTree, bool}, CancellationToken)"/>
        private static bool IsGeneratedFileName(string filePath)
        {
            return Regex.IsMatch(
                Path.GetFileName(filePath),
                @"\.designer\.cs$",
                RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        }

        /// <summary>
        /// Checks whether the given document has an auto-generated comment as its header.
        /// </summary>
        /// <param name="tree">The syntax tree to examine.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para><see langword="true"/> if <paramref name="tree"/> starts with a comment containing the text
        /// <c>&lt;auto-generated</c>; otherwise, <see langword="false"/>.</para>
        /// </returns>
        private static bool HasAutoGeneratedComment(SyntaxTree tree, CancellationToken cancellationToken)
        {
            var root = tree.GetRoot(cancellationToken);
            var firstToken = root.GetFirstToken();
            SyntaxTriviaList trivia;
            if (firstToken == default(SyntaxToken))
            {
                var token = ((CompilationUnitSyntax)root).EndOfFileToken;
                if (!token.HasLeadingTrivia)
                {
                    return false;
                }

                trivia = token.LeadingTrivia;
            }
            else
            {
                if (!firstToken.HasLeadingTrivia)
                {
                    return false;
                }

                trivia = firstToken.LeadingTrivia;
            }

            var comments = trivia.Where(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia) || t.IsKind(SyntaxKind.MultiLineCommentTrivia));
            return comments.Any(t =>
            {
                string s = t.ToString();
                return s.Contains("<auto-generated") || s.Contains("<autogenerated");
            });
        }

        /// <summary>
        /// Checks if a given <see cref="SyntaxTree"/> only contains whitespaces. We don't want to analyze empty files.
        /// </summary>
        /// <param name="tree">The syntax tree to examine.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that the task will observe.</param>
        /// <returns>
        /// <para><see langword="true"/> if <paramref name="tree"/> only contains whitespaces; otherwise, <see langword="false"/>.</para>
        /// </returns>
        private static bool IsEmpty(SyntaxTree tree, CancellationToken cancellationToken)
        {
            var root = tree.GetRoot(cancellationToken);
            var firstToken = root.GetFirstToken(includeZeroWidth: true);

            return firstToken.IsKind(SyntaxKind.EndOfFileToken)
                && IndexOfFirstNonWhitespaceTrivia(firstToken.LeadingTrivia) == -1;
        }

        /// <summary>
        /// Returns the index of the first non-whitespace trivia in the given trivia list.
        /// </summary>
        /// <param name="triviaList">The trivia list to process.</param>
        /// <param name="endOfLineIsWhitespace"><see langword="true"/> to treat <see cref="SyntaxKind.EndOfLineTrivia"/>
        /// as whitespace; otherwise, <see langword="false"/>.</param>
        /// <typeparam name="T">The type of the trivia list.</typeparam>
        /// <returns>The index where the non-whitespace starts, or -1 if there is no non-whitespace trivia.</returns>
        internal static int IndexOfFirstNonWhitespaceTrivia<T>(T triviaList, bool endOfLineIsWhitespace = true)
            where T : IReadOnlyList<SyntaxTrivia>
        {
            for (var index = 0; index < triviaList.Count; index++)
            {
                var currentTrivia = triviaList[index];
                switch (currentTrivia.Kind())
                {
                    case SyntaxKind.EndOfLineTrivia:
                        if (!endOfLineIsWhitespace)
                        {
                            return index;
                        }

                        break;

                    case SyntaxKind.WhitespaceTrivia:
                        break;

                    default:
                        // encountered non-whitespace trivia -> the search is done.
                        return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the effective <see cref="DocumentationMode"/> used when parsing the <see cref="SyntaxTree"/> containing
        /// the specified context.
        /// </summary>
        /// <param name="context">The analysis context.</param>
        /// <returns>
        /// <para>The <see cref="DocumentationMode"/> of the <see cref="SyntaxTree"/> containing the context.</para>
        /// <para>-or-</para>
        /// <para><see cref="DocumentationMode.Diagnose"/>, if the documentation mode could not be determined.</para>
        /// </returns>
        internal static DocumentationMode GetDocumentationMode(this SyntaxNodeAnalysisContext context)
        {
            return context.Node.SyntaxTree?.Options.DocumentationMode ?? DocumentationMode.Diagnose;
        }

        public static DocumentationCommentTriviaSyntax GetDocumentationCommentTriviaSyntax(this SyntaxNode node)
        {
            if (node == null)
            {
                return null;
            }

            foreach (var leadingTrivia in node.GetLeadingTrivia())
            {
                var structure = leadingTrivia.GetStructure() as DocumentationCommentTriviaSyntax;

                if (structure != null)
                {
                    return structure;
                }
            }

            return null;
        }

        private static class LanguageKindArrays<TLanguageKindEnum>
            where TLanguageKindEnum : struct
        {
            private static readonly ConcurrentDictionary<TLanguageKindEnum, ImmutableArray<TLanguageKindEnum>> Arrays =
                new ConcurrentDictionary<TLanguageKindEnum, ImmutableArray<TLanguageKindEnum>>();

            private static readonly Func<TLanguageKindEnum, ImmutableArray<TLanguageKindEnum>> CreateValueFactory = CreateValue;

            public static ImmutableArray<TLanguageKindEnum> GetOrCreateArray(TLanguageKindEnum syntaxKind)
            {
                return Arrays.GetOrAdd(syntaxKind, CreateValueFactory);
            }

            private static ImmutableArray<TLanguageKindEnum> CreateValue(TLanguageKindEnum syntaxKind)
            {
                return ImmutableArray.Create(syntaxKind);
            }
        }
        internal static bool HasBuiltinEndLine(this SyntaxTrivia trivia)
        {
            return trivia.IsDirective
                || trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                || trivia.IsKind(SyntaxKind.EndOfLineTrivia);
        }
    }
}
