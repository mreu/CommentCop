// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Misc.Tests.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Test
{
    using CommentCop.Misc;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TestHelper;

    /// <summary>
    /// Test all methods analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class MiscTests : CodeFixVerifier
    {
        /// <summary>
        /// No diagnostics expected to show up
        /// </summary>
        [TestMethod]
        public void TestNoDiagnostics()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 8000
        /// </summary>
        [TestMethod]
        public void TestRule8000_1()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            /// <summary>
            /// The method1.
            /// </summary>
            private int f1;
            /// <summary>
            /// The method1.
            /// </summary>
            private int f1;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = MR8000XMLCommentMustBePreceededByBlankLine.DiagnosticId8000,
                Message = $"{MR8000XMLCommentMustBePreceededByBlankLine.Title} ({MR8000XMLCommentMustBePreceededByBlankLine.DiagnosticId8000})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 13)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {

        /// <summary>
        /// The method1.
        /// </summary>
        public void Method1()
            {
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        #region Overrides
        /// <summary>
        /// Get CSharp code fix provider.
        /// </summary>
        /// <returns>The code fix provider.</returns>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return null;
            ////return new MR8000CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR8000XMLCommentMustBePreceededByBlankLine();
        }
        #endregion Overrides
    }
}
