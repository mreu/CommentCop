// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Regions.Tests.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Test
{
    using CommentCop.Regions;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestHelper;

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class Regions7000 : CodeFixVerifier
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
        /// Diagnostic triggered and checked for rule 7000
        /// </summary>
        [TestMethod]
        public void TestRule7000()
        {
            const string test = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region 
            {
            }
            #endregion 1111
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MR7000RegionsMustHaveDescription.DiagnosticId7000,
                Message = $"{MR7000RegionsMustHaveDescription.Title} ({MR7000RegionsMustHaveDescription.DiagnosticId7000})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 13)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        #region Overrides
        /// <summary>
        /// Get CSharp code fix provider.
        /// </summary>
        /// <returns>The code fix provider.</returns>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return null;
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7000RegionsMustHaveDescription();
        }
        #endregion Overrides
    }

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class Regions7001 : CodeFixVerifier
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
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MR7001DescriptionInRegionsMustBeginWithUppercaseCharacter.DiagnosticId7001,
                Message = $"{MR7001DescriptionInRegionsMustBeginWithUppercaseCharacter.Title} ({MR7001DescriptionInRegionsMustBeginWithUppercaseCharacter.DiagnosticId7001})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 21)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region Aa of Bb
            {
            }
            #endregion
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
            return new MR7001CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7001DescriptionInRegionsMustBeginWithUppercaseCharacter();
        }
        #endregion Overrides
    }

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class Regions7002 : CodeFixVerifier
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
        public void TestRule7002()
        {
            const string test = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MR7002EndregionsMustHaveDescription.DiagnosticId7002,
                Message = $"{MR7002EndregionsMustHaveDescription.Title} ({MR7002EndregionsMustHaveDescription.DiagnosticId7002})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 17, 13)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion aa of bb
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
            return new MR7002CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7002EndregionsMustHaveDescription();
        }
        #endregion Overrides
    }

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class Regions7003 : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7003
        /// </summary>
        [TestMethod]
        public void TestRule7003()
        {
            const string test = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region
            {
            }
            #endregion aa of bb
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MR7003DescriptionInEndregionsMustBeginWithUppercaseCharacter.DiagnosticId7003,
                Message = $"{MR7003DescriptionInEndregionsMustBeginWithUppercaseCharacter.Title} ({MR7003DescriptionInEndregionsMustBeginWithUppercaseCharacter.DiagnosticId7003})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 17, 24)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region
            {
            }
            #endregion Aa of Bb
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
            return new MR7003CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7003DescriptionInEndregionsMustBeginWithUppercaseCharacter();
        }
        #endregion Overrides
    }

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class Regions7004 : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7004
        /// </summary>
        [TestMethod]
        public void TestRule7004()
        {
            const string test = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion sdf sdf
            #region cc and dd
            #endregion sdf sdf
        }
    }
}";
            var expected1 = new DiagnosticResult
            {
                Id = MR7004EndregionMustHaveTheSameTextAsTheRegion.DiagnosticId7004,
                Message = $"{MR7004EndregionMustHaveTheSameTextAsTheRegion.Title} ({MR7004EndregionMustHaveTheSameTextAsTheRegion.DiagnosticId7004})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 17, 24)
                    }
            };

            var expected2 = new DiagnosticResult
            {
                Id = MR7004EndregionMustHaveTheSameTextAsTheRegion.DiagnosticId7004,
                Message = $"{MR7004EndregionMustHaveTheSameTextAsTheRegion.Title} ({MR7004EndregionMustHaveTheSameTextAsTheRegion.DiagnosticId7004})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 19, 24)
                    }
            };

            VerifyCSharpDiagnostic(test, expected1, expected2);

            const string fixtest = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion aa of bb
            #region cc and dd
            #endregion cc and dd
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
            return new MR7004CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7004EndregionMustHaveTheSameTextAsTheRegion();
        }
        #endregion Overrides
    }

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class Regions7005 : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7005
        /// </summary>
        [TestMethod]
        public void TestRule7005()
        {
            const string test = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region   aa of bb
            {
            }
            #endregion
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MR7005NoMultipleWhitespacesFollowingTheRegionKeyword.DiagnosticId7005,
                Message = $"{MR7005NoMultipleWhitespacesFollowingTheRegionKeyword.Title} ({MR7005NoMultipleWhitespacesFollowingTheRegionKeyword.DiagnosticId7005})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 20)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion
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
            return new MR7005CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7005NoMultipleWhitespacesFollowingTheRegionKeyword();
        }
        #endregion Overrides
    }

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class Regions7006 : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7006
        /// </summary>
        [TestMethod]
        public void TestRule7006()
        {
            const string test = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion  aa of bb
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MR7006NoMultipleWhitespacesFollowingTheEndregionKeyword.DiagnosticId7006,
                Message = $"{MR7006NoMultipleWhitespacesFollowingTheEndregionKeyword.Title} ({MR7006NoMultipleWhitespacesFollowingTheEndregionKeyword.DiagnosticId7006})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 17, 23)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion aa of bb
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
            return new MR7006CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7006NoMultipleWhitespacesFollowingTheEndregionKeyword();
        }
        #endregion Overrides
    }

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class Regions7007 : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7007
        /// </summary>
        [TestMethod]
        public void TestRule7007()
        {
            const string test = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb


            {
            }
            #endregion aa of bb
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MR7007NoEmptyLineFollowingRegionKeyword.DiagnosticId7007,
                Message = $"{MR7007NoEmptyLineFollowingRegionKeyword.Title} ({MR7007NoEmptyLineFollowingRegionKeyword.DiagnosticId7007})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 13)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion aa of bb
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
            return new MR7007CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7007NoEmptyLineFollowingRegionKeyword();
        }
        #endregion Overrides
    }

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class Regions7008 : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7007
        /// </summary>
        [TestMethod]
        public void TestRule7008()
        {
            const string test = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }


            #endregion aa of bb
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MR7008NoEmptyLinePreceedingEndRegionKeyword.DiagnosticId7008,
                Message = $"{MR7008NoEmptyLinePreceedingEndRegionKeyword.Title} ({MR7008NoEmptyLinePreceedingEndRegionKeyword.DiagnosticId7008})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 19, 13)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion aa of bb
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
            return new MR7008CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7008NoEmptyLinePreceedingEndRegionKeyword();
        }
        #endregion Overrides
    }

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class Regions7009 : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7007
        /// </summary>
        [TestMethod]
        public void TestRule7009()
        {
            const string test = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion aa of bb
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MR7009EmptyLineMustFollowingRegionKeyword.DiagnosticId7009,
                Message = $"{MR7009EmptyLineMustFollowingRegionKeyword.Title} ({MR7009EmptyLineMustFollowingRegionKeyword.DiagnosticId7009})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 13)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb

            {
            }
            #endregion aa of bb
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
            return new MR7009CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7009EmptyLineMustFollowingRegionKeyword();
        }
        #endregion Overrides
    }

    /// <summary>
    /// Test all properties analyzers and code fixes.
    /// </summary>
    [TestClass]
    public class Regions7010 : CodeFixVerifier
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
        /// Diagnostic and CodeFix both triggered and checked for rule 7010
        /// </summary>
        [TestMethod]
        public void TestRule7010()
        {
            const string test = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }
            #endregion aa of bb
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = MR7010EmptyLineMustPreceedEndRegionKeyword.DiagnosticId7010,
                Message = $"{MR7010EmptyLineMustPreceedEndRegionKeyword.Title} ({MR7010EmptyLineMustPreceedEndRegionKeyword.DiagnosticId7010})",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 17, 13)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            const string fixtest = @"
namespace TestProjectForRoslyn
{
    /// <summary>
    /// The regions class.
    /// </summary>
    public class Regions
    {
        /// <summary>
        /// The method1.
        /// </summary>
        private void Method1()
        {
            #region aa of bb
            {
            }

            #endregion aa of bb
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
            return new MR7010CodeFixProvider();
        }

        /// <summary>
        /// Get CSharp diagnostic analyer.
        /// </summary>
        /// <returns>The diagnostic analyer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MR7010EmptyLineMustPreceedEndRegionKeyword();
        }
        #endregion Overrides
    }
}