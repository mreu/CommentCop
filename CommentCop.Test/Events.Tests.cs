// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Events.Tests.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Test
{
    using CommentCop.Events;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestHelper;

    /// <summary>
    /// Test all events analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class EventsTests : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 5001
        /// </summary>
        [TestMethod]
        public void TestRule5001()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            public event Delegate1<TypeName> Event1;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR5001",
                Message = "Public events must have a xml documentation header. (MR5001)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 46)
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
        /// The Event1 of the <see cref=""Delegate1{TypeName}""/>.
        /// </summary>
        public event Delegate1<TypeName> Event1;
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 5002
        /// </summary>
        [TestMethod]
        public void TestRule5002()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            internal event Delegate1<TypeName> Event1;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR5002",
                Message = "Internal events must have a xml documentation header. (MR5002)",
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
        /// The Event1 of the <see cref=""Delegate1{TypeName}""/>.
        /// </summary>
        internal event Delegate1<TypeName> Event1;
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 5003
        /// </summary>
        [TestMethod]
        public void TestRule5003()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            protected internal event Delegate1<TypeName> Event1;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR5003",
                Message = "Internal protected events must have a xml documentation header. (MR5003)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 58)
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
        /// The Event1 of the <see cref=""Delegate1{TypeName}""/>.
        /// </summary>
        protected internal event Delegate1<TypeName> Event1;
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 5004
        /// </summary>
        [TestMethod]
        public void TestRule5004()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            protected event Delegate1<TypeName> Event1;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR5004",
                Message = "Protected events must have a xml documentation header. (MR5004)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 49)
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
        /// The Event1 of the <see cref=""Delegate1{TypeName}""/>.
        /// </summary>
        protected event Delegate1<TypeName> Event1;
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        /// <summary>
        /// Diagnostic and CodeFix both triggered and checked for rule 5005
        /// </summary>
        [TestMethod]
        public void TestRule5005()
        {
            const string test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            private event Delegate1<TypeName> Event1;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "MR5005",
                Message = "Private events must have a xml documentation header. (MR5005)",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 8, 47)
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
        /// The Event1 of the <see cref=""Delegate1{TypeName}""/>.
        /// </summary>
        private event Delegate1<TypeName> Event1;
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
            return new MR5001_5005CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR5001_5005EventsMustHaveXMLComment();
        }
        #endregion overrides
    }
}