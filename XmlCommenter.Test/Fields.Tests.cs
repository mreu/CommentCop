// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Fields.Tests.cs" company="Michael Reukauff">
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

    using XmlDocAnalyzer.Fields;

    /// <summary>
    /// Test all fields analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class FieldsTests : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 4001
        /// </summary>
        [TestMethod]
        public void TestRule4001()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public readonly string field41 = Guid.NewGuid().ToString;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR4001",
                Message = "Public fields must have a xml documentation header. (MR4001)",
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
        /// The readonly field41. Value: Guid.NewGuid().ToString.
        /// </summary>
        public readonly string field41 = Guid.NewGuid().ToString;
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 4002
        /// </summary>
        [TestMethod]
        public void TestRule4002()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            internal readonly string field41 = Guid.NewGuid().ToString;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR4002",
                Message = "Internal fields must have a xml documentation header. (MR4002)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 38)
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
        /// The readonly field41. Value: Guid.NewGuid().ToString.
        /// </summary>
        internal readonly string field41 = Guid.NewGuid().ToString;
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 4003
        /// </summary>
        [TestMethod]
        public void TestRule4003()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            protected internal readonly string field41 = Guid.NewGuid().ToString;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR4003",
                Message = "Internal protected fields must have a xml documentation header. (MR4003)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 48)
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
        /// The readonly field41. Value: Guid.NewGuid().ToString.
        /// </summary>
        protected internal readonly string field41 = Guid.NewGuid().ToString;
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 4004
        /// </summary>
        [TestMethod]
        public void TestRule4004()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            protected readonly string test_41 = Guid.NewGuid().ToString;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR4004",
                Message = "Protected fields must have a xml documentation header. (MR4004)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 39)
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
        /// The readonly test 41. Value: Guid.NewGuid().ToString.
        /// </summary>
        protected readonly string test_41 = Guid.NewGuid().ToString;
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 4005
        /// </summary>
        [TestMethod]
        public void TestRule4005()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            private readonly string parameterName = Guid.NewGuid().ToString;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR4005",
                Message = "Private fields must have a xml documentation header. (MR4005)",
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
        /// The readonly parameter name. Value: Guid.NewGuid().ToString.
        /// </summary>
        private readonly string parameterName = Guid.NewGuid().ToString;
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 4001
        /// </summary>
        [TestMethod]
        public void TestRule4001_WithLongValueWithMoreThenOneLine()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                ""FontFamily"",
                typeof(FontFamily),
                typeof(string),
                new FrameworkPropertyMetadata(
                    new FontFamily(FontFamilyValue),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR4001",
                Message = "Public fields must have a xml documentation header. (MR4001)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 55)
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
        /// The readonly FontFamilyProperty. Value: DependencyProperty.Register(""FontFamily"", typeof(FontFamily), typeof(string), new FrameworkPropertyMetadata(new FontFamily(FontFamilyValue), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure)).
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(
                ""FontFamily"",
                typeof(FontFamily),
                typeof(string),
                new FrameworkPropertyMetadata(
                    new FontFamily(FontFamilyValue),
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));
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
            return new MR4001_4005CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR4001_4005FieldsMustHaveXMLComment();
        }
        #endregion overrides
    }
}