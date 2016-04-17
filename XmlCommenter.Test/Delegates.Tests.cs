// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Delegates.Tests.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer.Test
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestHelper;

    using XmlDocAnalyzer.Delegates;

    /// <summary>
    /// Test all delegates analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class DelegatesTests : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 5006
        /// </summary>
        [TestMethod]
        public void TestRule5006()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public delegate int Delegate1<T>(T p1, int x, string abc);
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR5006",
                Message = "Public delegates must have a xml documentation header. (MR5006)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 33)
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
        /// The delegate1.
        /// </summary>
        /// <param name=""p1"">The p1.</param>
        /// <param name=""x"">The x.</param>
        /// <param name=""abc"">The abc.</param>
        /// <returns>The <see cref=""int""/>.</returns>
        /// <typeparam name=""T""></param>
        public delegate int Delegate1<T>(T p1, int x, string abc);
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 5007
        /// </summary>
        [TestMethod]
        public void TestRule5007()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            internal delegate int Delegate1<T>(T p1, int x, string abc);
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR5007",
                Message = "Internal delegates must have a xml documentation header. (MR5007)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 35)
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
        /// The delegate1.
        /// </summary>
        /// <param name=""p1"">The p1.</param>
        /// <param name=""x"">The x.</param>
        /// <param name=""abc"">The abc.</param>
        /// <returns>The <see cref=""int""/>.</returns>
        /// <typeparam name=""T""></param>
        internal delegate int Delegate1<T>(T p1, int x, string abc);
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 5008
        /// </summary>
        [TestMethod]
        public void TestRule5008()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            protected internal delegate int Delegate1<T>(T p1, int x, string abc);
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR5008",
                Message = "Internal protected delegates must have a xml documentation header. (MR5008)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 45)
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
        /// The delegate1.
        /// </summary>
        /// <param name=""p1"">The p1.</param>
        /// <param name=""x"">The x.</param>
        /// <param name=""abc"">The abc.</param>
        /// <returns>The <see cref=""int""/>.</returns>
        /// <typeparam name=""T""></param>
        protected internal delegate int Delegate1<T>(T p1, int x, string abc);
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 5009
        /// </summary>
        [TestMethod]
        public void TestRule5009()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            protected delegate int Delegate1<T>(T p1, int x, string abc);
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR5009",
                Message = "Protected delegates must have a xml documentation header. (MR5009)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 36)
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
        /// The delegate1.
        /// </summary>
        /// <param name=""p1"">The p1.</param>
        /// <param name=""x"">The x.</param>
        /// <param name=""abc"">The abc.</param>
        /// <returns>The <see cref=""int""/>.</returns>
        /// <typeparam name=""T""></param>
        protected delegate int Delegate1<T>(T p1, int x, string abc);
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 5010
        /// </summary>
        [TestMethod]
        public void TestRule5010()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            private delegate int Delegate1<T>(T p1, int x, string abc);
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR5010",
                Message = "Private delegates must have a xml documentation header. (MR5010)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 34)
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
        /// The delegate1.
        /// </summary>
        /// <param name=""p1"">The p1.</param>
        /// <param name=""x"">The x.</param>
        /// <param name=""abc"">The abc.</param>
        /// <returns>The <see cref=""int""/>.</returns>
        /// <typeparam name=""T""></param>
        private delegate int Delegate1<T>(T p1, int x, string abc);
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        #region overrides
        /// <summary>
        /// Get CSharp code fix provider.
        /// </summary>
        /// <returns>The code fix provider.</returns>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new MR5006_5010CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR5006_5010DelegatesMustHaveXMLComment();
        }
        #endregion overrides
    }
}