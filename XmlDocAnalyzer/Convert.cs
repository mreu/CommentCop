// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Convert.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer
{
    using System;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The convert class.
    /// </summary>
    public class Convert
    {
        /// <summary>
        /// The work items.
        /// </summary>
        private static readonly string[] WorkItems = { "get", "put", "set", "display", "show", "register", "initialize", "insert", "delete", "update", "select" };

        /// <summary>
        /// Try to convert the name of a class to useful comment.
        /// </summary>
        /// <param name="name">The name of the class.</param>
        /// <returns>The comment.</returns>
        public static string Class(string name)
        {
            var parts = SplitName(name);

            if (parts.Length > 0)
            {
                MakeLowerCase(parts, true);

                string newName;
                if (parts[parts.Length - 1].Contains("class"))
                {
                    newName = "The " + string.Join(" ", parts) + '.';
                }
                else
                {
                    newName = "The " + string.Join(" ", parts) + " class.";
                }

                return newName;
            }

            return "The " + name.ToLower() + " class.";
        }

        /// <summary>
        /// Try to convert the name of an interface to useful comment.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <returns>The comment.</returns>
        public static string Interface(string name)
        {
            return "The " + name + " interface.";
        }

        /// <summary>
        /// Try to convert the name of a method to useful comment.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <returns>The comment.</returns>
        public static string Method(string name)
        {
            var special = CheckForSpecialMethodNames(name);
            if (special != null)
            {
                return special;
            }

            var parts = SplitName(name);

            if (parts.Length > 0)
            {
                if (WorkItems.Any(x => x.Equals(parts[0], StringComparison.OrdinalIgnoreCase)))
                {
                    MakeLowerCase(parts, false);

                    var newName = parts[0] + ' ' + string.Join(" ", parts.Skip(1)) + '.';

                    return newName;
                }

                MakeLowerCase(parts, true);
                return "The " + string.Join(" ", parts) + '.';
            }

            return "The " + name.ToLower() + '.';
        }

        /// <summary>
        /// Try to convert the return type of a method to useful comment.
        /// </summary>
        /// <param name="name">The name of the return type.</param>
        /// <returns>The comment.</returns>
        public static string Returns(string name)
        {
            return $"The <see cref=\"{name}\"/>.";
        }

        /// <summary>
        /// Split the name at the uppercase letter.
        /// </summary>
        /// <param name="name">The name to split.</param>
        /// <returns>An array of words.</returns>
        private static string[] SplitName(string name)
        {
            // split the name at the uppercase letterss
            var regex = new Regex(@"([A-Z])(?<=[a-z]\1|[A-Za-z]\1(?=[a-z]))", RegexOptions.CultureInvariant | RegexOptions.Singleline);
            var str = regex.Replace(name, " $1");
            var parts = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts;
        }

        /// <summary>
        /// Check for special method names.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>The comment text for the special method name or null if not special.</returns>
        private static string CheckForSpecialMethodNames(string name)
        {
            switch (name.ToLower())
            {
                case "main":

                    return "The main entry point.";
            }

            return null;
        }

        /// <summary>
        /// Change all words (except the first and except words complete uppercase) to lowercase.
        /// </summary>
        /// <param name="parts">The list word words to change.</param>
        /// <param name="includeFirst">True if first entry should be made lowercase otherwise false.</param>
        private static void MakeLowerCase(string[] parts, bool includeFirst)
        {
            // skip the first word in the list.
            for (var ix = includeFirst ? 0 : 1; ix < parts.Length; ix++)
            {
                // if word is longer than 1 letter check if second letter is uppercase
                // if yes do not change the word
                if (parts[ix].Length > 1)
                {
                    if (parts[ix][1] >= 'A' && parts[ix][1] <= 'Z')
                    {
                        continue;
                    }

                    parts[ix] = parts[ix].ToLower();
                }
            }
        }

        public static string Parameter(string name)
        {
            return "The " + name + '.';
        }
    }
}
