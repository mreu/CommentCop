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
        /// Diagnostic and CodeFix both triggered and checked for rule 7001
        /// </summary>
        [TestMethod]
        public void TestRule7001()
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
                Id = "MR7001",
                Message = "Public delegates must have a xml documentation header. (MR7001)",
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7002
        /// </summary>
        [TestMethod]
        public void TestRule7002()
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
                Id = "MR7002",
                Message = "Internal delegates must have a xml documentation header. (MR7002)",
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7003
        /// </summary>
        [TestMethod]
        public void TestRule7003()
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
                Id = "MR7003",
                Message = "Internal protected delegates must have a xml documentation header. (MR7003)",
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7004
        /// </summary>
        [TestMethod]
        public void TestRule7004()
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
                Id = "MR7004",
                Message = "Protected delegates must have a xml documentation header. (MR7004)",
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7005
        /// </summary>
        [TestMethod]
        public void TestRule7005()
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
                Id = "MR7005",
                Message = "Private delegates must have a xml documentation header. (MR7005)",
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
            return new MR7001_7005CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7001_7005DelegatesMustHaveXMLComment();
        }
        #endregion overrides
    }
}