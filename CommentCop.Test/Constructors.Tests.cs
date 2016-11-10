// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constructors.Tests.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Test
{
    using CommentCop.Constructors;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestHelper;

    /// <summary>
    /// Test all constructors analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class ConstructorsTests : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 1101
        /// </summary>
        [TestMethod]
        public void TestRule1101()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public TypeName()
            {
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR1101",
                Message = "Public constructors must have a xml documentation header. (MR1101)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 20)
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
        /// Initializes a new instance of the <see cref=""TypeName""/> class.
        /// </summary>
        public TypeName()
            {
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 1102
        /// </summary>
        [TestMethod]
        public void TestRule1102()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            internal TypeName()
            {
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR1102",
                Message = "Internal constructors must have a xml documentation header. (MR1102)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 22)
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
        /// Initializes a new instance of the <see cref=""TypeName""/> class.
        /// </summary>
        internal TypeName()
            {
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 1103
        /// </summary>
        [TestMethod]
        public void TestRule1103()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            protected internal TypeName()
            {
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR1103",
                Message = "Internal protected constructors must have a xml documentation header. (MR1103)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 32)
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
        /// Initializes a new instance of the <see cref=""TypeName""/> class.
        /// </summary>
        protected internal TypeName()
            {
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 1104
        /// </summary>
        [TestMethod]
        public void TestRule1104()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            protected TypeName()
            {
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR1104",
                Message = "Protected constructors must have a xml documentation header. (MR1104)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 23)
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
        /// Initializes a new instance of the <see cref=""TypeName""/> class.
        /// </summary>
        protected TypeName()
            {
            }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 1105
        /// </summary>
        [TestMethod]
        public void TestRule1105()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            private TypeName()
            {
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR1105",
                Message = "Private constructors must have a xml documentation header. (MR1105)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 21)
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
        /// Initializes a new instance of the <see cref=""TypeName""/> class.
        /// </summary>
        private TypeName()
            {
            }
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
            return new MR1101_1106CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR1101_1106ConstructorsMustHaveXMLComment();
        }
        #endregion overrides
    }
}