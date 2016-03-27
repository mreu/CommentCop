// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Properties.Tests.cs" company="Michael Reukauff">
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

    using XmlDocAnalyzer.Property;

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class PropertiesTests : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 3001
        /// </summary>
        [TestMethod]
        public void TestRule3001()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public bool Prop1 { get;  private set; }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR3001",
                Message = "Public properties must have a xml documentation header. (MR3001)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 25)
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
        /// Gets a value indicating whether 
        /// </summary>
        public bool Prop1 { get;  private set; }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 3002
        /// </summary>
        [TestMethod]
        public void TestRule3002()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            internal bool Prop1 { get;  private set; }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR3002",
                Message = "Internal properties must have a xml documentation header. (MR3002)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 27)
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
        /// Gets a value indicating whether 
        /// </summary>
        internal bool Prop1 { get;  private set; }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 3003
        /// </summary>
        [TestMethod]
        public void TestRule3003()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            protected internal bool Prop1 { get;  private set; }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR3003",
                Message = "Internal protected properties must have a xml documentation header. (MR3003)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 37)
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
        /// Gets a value indicating whether 
        /// </summary>
        protected internal bool Prop1 { get;  private set; }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 3004
        /// </summary>
        [TestMethod]
        public void TestRule3004()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            protected bool Prop1 { get;  private set; }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR3004",
                Message = "Protected properties must have a xml documentation header. (MR3004)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 28)
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
        /// Gets a value indicating whether 
        /// </summary>
        protected bool Prop1 { get;  private set; }
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 3005
        /// </summary>
        [TestMethod]
        public void TestRule3005()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            private bool Prop1 { get;  private set; }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR3005",
                Message = "Private properties must have a xml documentation header. (MR3005)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 26)
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
        /// Gets a value indicating whether 
        /// </summary>
        private bool Prop1 { get;  private set; }
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
            return new MR3001_3005CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR3001_3005PropertiesMustHaveXMLComment();
        }
        #endregion overrides
    }
}