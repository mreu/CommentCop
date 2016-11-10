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

            return attr.Any(x => verbs.Contains(x));
        }
    }
}
