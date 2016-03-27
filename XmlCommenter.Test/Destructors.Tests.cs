// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Destructors.Tests.cs" company="Michael Reukauff">
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

    using XmlDocAnalyzer.Destructors;

    /// <summary>
    /// Test all destructors analyzers and code fixes.
    /// </summary>
    [TestClass]

    public class DestructorsTests : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 1201
        /// </summary>
        [TestMethod]
        public void TestRule1201()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            ~TypeName()
            {
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR1201",
                Message = "Destructors must have a xml documentation header. (MR1201)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 14)
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
        /// Finalizes an instance of the <see cref=""TypeName""/> class.
        /// </summary>
        ~TypeName()
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
            return new MR1201CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR1201DestructorsMustHaveXMLComment();
        }
        #endregion overrides
    }
}
