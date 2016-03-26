// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Classes.Tests.cs" company="Michael Reukauff">
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

    using XmlDocAnalyzer.Classes;
    using XmlDocAnalyzer.Methods;

    /// <summary>
    /// Test all class analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class ClassesTests : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 0001
        /// </summary>
        [TestMethod]
        public void TestRule0001()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        public class TypeName
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR0001",
                Message = "Public classes must have a xml documentation header. (MR0001)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 6, 22)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
    /// <summary>
    /// The type name class.
    /// </summary>
    public class TypeName
        {
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 0002
        /// </summary>
        [TestMethod]
        public void TestRule0002()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        internal class TypeName
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR0002",
                Message = "Internal classes must have a xml documentation header. (MR0002)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 6, 24)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
    /// <summary>
    /// The type name class.
    /// </summary>
    internal class TypeName
        {
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 0003
        /// </summary>
        [TestMethod]
        public void TestRule0003()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        protected internal class TypeName
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR0003",
                Message = "Internal protected classes must have a xml documentation header. (MR0003)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 6, 34)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
    /// <summary>
    /// The type name class.
    /// </summary>
    protected internal class TypeName
        {
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 0004
        /// </summary>
        [TestMethod]
        public void TestRule0004()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        protected class TypeName
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR0004",
                Message = "Protected classes must have a xml documentation header. (MR0004)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 6, 25)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
    /// <summary>
    /// The type name class.
    /// </summary>
    protected class TypeName
        {
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 0005
        /// </summary>
        [TestMethod]
        public void TestRule0005()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        private class TypeName
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR0005",
                Message = "Private classes must have a xml documentation header. (MR0005)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 6, 23)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
    /// <summary>
    /// The type name class.
    /// </summary>
    private class TypeName
        {
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
            return new MR0001_MR0005CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR0001_MR0005ClassesMustHaveXMLComment();
        }
        #endregion overrides
    }
}