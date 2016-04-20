// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Struct.Tests.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlCommenter.Test
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TestHelper;

    using XmlCommenter.Structs;

    /// <summary>
    /// Test all struct analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class StructsTests : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 0006
        /// </summary>
        [TestMethod]
        public void TestRule0006()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        public struct TypeName
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR0006",
                Message = "Public structs must have a xml documentation header. (MR0006)",
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
    /// The type name struct.
    /// </summary>
    public struct TypeName
        {
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 0007
        /// </summary>
        [TestMethod]
        public void TestRule0007_1()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        internal struct TypeName
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR0007",
                Message = "Internal structs must have a xml documentation header. (MR0007)",
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
    /// The type name struct.
    /// </summary>
    internal struct TypeName
        {
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 0007
        /// </summary>
        [TestMethod]
        public void TestRule0007_2()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        struct TypeName
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR0007",
                Message = "Internal structs must have a xml documentation header. (MR0007)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 6, 16)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
    /// <summary>
    /// The type name struct.
    /// </summary>
    struct TypeName
        {
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 0008
        /// </summary>
        [TestMethod]
        public void TestRule0008()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        protected internal struct TypeName
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR0008",
                Message = "Internal protected structs must have a xml documentation header. (MR0008)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 6, 35)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
    /// <summary>
    /// The type name struct.
    /// </summary>
    protected internal struct TypeName
        {
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 0009
        /// </summary>
        [TestMethod]
        public void TestRule0009()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        protected struct TypeName
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR0009",
                Message = "Protected structs must have a xml documentation header. (MR0009)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 6, 26)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
    using System;

    namespace ConsoleApplication1
    {
    /// <summary>
    /// The type name struct.
    /// </summary>
    protected struct TypeName
        {
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 0010
        /// </summary>
        [TestMethod]
        public void TestRule0010()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        private struct TypeName
        {
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR0010",
                Message = "Private structs must have a xml documentation header. (MR0010)",
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
    /// The type name struct.
    /// </summary>
    private struct TypeName
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
            return new MR0006_0010CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR0006_0010StructsMustHaveXMLComment();
        }
        #endregion overrides
    }
}