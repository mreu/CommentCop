// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Test.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop.Helper
{
    using System.Linq;

    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// The test class.
    /// </summary>
    public class Test
    {
        /// <summary>
        /// The special unit test verbs.
        /// </summary>
        private static readonly string[] SpecialVerbs = { "TestInitialize", "TestInitializeAttribute",
                "TestCleanup", "TestCleanupAttribute",
                "ClassInitialize", "ClassInitializeAttribute",
                "ClassCleanup", "ClassCleanupAttribute",
                "SetUp", "SetUpAttribute",
                "TearDown", "TearDownAttribute",
                "TestFixtureSetup", "TestFixtureSetupAttribute",
                "TestFixtureTearDown", "TestFixtureTearDownAttribute"
                };

        /// <summary>
        /// The class initialize text (const). Value: "ClassInitialize runs code before running the first test in the class.".
        /// </summary>
        private const string ClassInitialize = "ClassInitialize runs code before running the first test in the class.";

        /// <summary>
        /// The class cleanup text (const). Value: "ClassCleanup runs code after all tests in a class have run.".
        /// </summary>
        private const string ClassCleanup = "ClassCleanup runs code after all tests in a class have run.";

        /// <summary>
        /// The test initialize text (const). Value: "TestInitialize runs code before running each test.".
        /// </summary>
        private const string TestInitialize = "TestInitialize runs code before running each test.";

        /// <summary>
        /// The test cleanup text (const). Value: "TestCleanup runs code after each test has run.".
        /// </summary>
        private const string TestCleanup = "TestCleanup runs code after each test has run.";

        /// <summary>
        /// Checks if the class is a unittest class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsUnittest(ClassDeclarationSyntax @class)
        {
            string[] verbs = { "TestClass", "TestClassAttribute", "TestFixture", "TestFixtureAttribute" };

            if (!@class.AttributeLists.Any())
            {
                return false;
            }

            var attr = @class.AttributeLists.SelectMany(x => x.Attributes).Select(x => ((IdentifierNameSyntax)x.Name).Identifier.Text);

            return attr.Any(x => verbs.Contains(x));
        }

        /// <summary>
        /// Is the method a unittest.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The <see cref="bool"/>.True if method is a unittest otherwise false.</returns>
        public static bool IsUnittest(MethodDeclarationSyntax method)
        {
            string[] verbs = { "TestMethod", "TestMethodAttribute", "Test", "TestAttribute", "Fact", "FactAttribute" };

            if (!method.AttributeLists.Any())
            {
                return false;
            }

            var attr = method.AttributeLists.SelectMany(x => x.Attributes).Select(x => ((IdentifierNameSyntax)x.Name).Identifier.Text);

            if (attr.Any(x => verbs.Contains(x)))
            {
                return true;
            }

            return IsSpecialAttribute(method);
        }

        /// <summary>
        /// Check if there is a special attribute.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>The <see cref="bool"/>.True if there is a special attribute otherwise false.</returns>
        public static bool IsSpecialAttribute(MethodDeclarationSyntax method)
        {
            if (!method.AttributeLists.Any())
            {
                return false;
            }

            var attr = method.AttributeLists.SelectMany(x => x.Attributes).Select(x => ((IdentifierNameSyntax)x.Name).Identifier.Text);

            return attr.Any(x => SpecialVerbs.Contains(x));
        }

        public static string GetSpecialAttributeText(MethodDeclarationSyntax method)
        {
            var attr = method.AttributeLists.SelectMany(x => x.Attributes).Select(x => ((IdentifierNameSyntax)x.Name).Identifier.Text);

            var list = SpecialVerbs.Intersect(attr).ToList();

            if (list.Any())
            {
                switch (list.First())
                {
                    case "TestInitialize":
                    case "TestInitializeAttribute":
                    case "SetUp":
                    case "SetUpAttribute":
                        return TestInitialize;
                    case "TestCleanup":
                    case "TestCleanupAttribute":
                    case "TearDown":
                    case "TearDownAttribute":
                        return TestCleanup;
                    case "ClassInitialize":
                    case "ClassInitializeAttribute":
                    case "TestFixtureSetup":
                    case "TestFixtureSetupAttribute":
                        return ClassInitialize;
                    case "ClassCleanup":
                    case "ClassCleanupAttribute":
                    case "TestFixtureTearDown":
                    case "TestFixtureTearDownAttribute":
                        return ClassCleanup;
                }
            }

            return string.Empty;
        }
    }
}
