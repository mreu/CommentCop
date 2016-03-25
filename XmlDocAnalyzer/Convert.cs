﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Convert.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// The convert class.
    /// </summary>
    public class Convert
    {
        /// <summary>
        /// The work items.
        /// </summary>
        private static readonly string[] WorkItems =
        {
            "build",
            "check",
            "copy",
            "commit",
            "create",
            "delete",
            "discard",
            "display",
            "fetch",
            "fill",
            "find",
            "get",
            "initialize",
            "insert",
            "move",
            "pull",
            "push",
            "put",
            "read",
            "register",
            "remove",
            "rollback",
            "search",
            "select",
            "set",
            "show",
            "start",
            "stop",
            "test",
            "update",
            "write"
        };

        /// <summary>
        /// Split at uppercase letter regex.
        /// </summary>
        private static readonly Regex SplitAtUppercase = new Regex(@"([A-Z])(?<=[a-z]\1|[A-Za-z]\1(?=[a-z]))", RegexOptions.CultureInvariant | RegexOptions.Singleline);

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

                    if (parts.Length > 1)
                    {
                        return parts[0] + ' ' + string.Join(" ", parts.Skip(1)) + '.';
                    }

                    return parts[0] + '.';
                }

                MakeLowerCase(parts, true);

                if (parts.Length > 1)
                {
                    return "The " + string.Join(" ", parts) + '.';
                }

                return "The " + parts[0] + '.';
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
            return $"The <see cref=\"{name.Replace('<', '{').Replace('>', '}')}\"/>.";
        }

        /// <summary>
        /// Try to convert the parameter to a useful comment.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The comment.</returns>
        public static string Parameter(string name)
        {
            return "The " + name + '.';
        }

        /// <summary>
        /// Generate the comment for a property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="hasGetter">True if property has a getter otherwise false.</param>
        /// <param name="hasSetter">True if property has a setter otherwise false.</param>
        /// <param name="isBool">True if property is of type boolean otherwise false.</param>
        /// <returns>The comment.</returns>
        public static object Property(string name, bool hasGetter, bool hasSetter, bool isBool)
        {
            string comment;

            if (hasGetter)
            {
                comment = "Gets";

                if (hasSetter)
                {
                    comment += " or sets";
                }
            }
            else
            {
                comment = "Sets";
            }

            if (isBool)
            {
                comment += " a value indicating whether ";
            }
            else
            {
                comment += " the " + name + '.';
            }

            return comment;
        }

        /// <summary>
        /// Generate the comment for a property.
        /// </summary>
        /// <param name="propType">The type of the property.</param>
        /// <returns>The comment.</returns>
        public static string PropertyType(string propType)
        {
            return $"The <see cref=\"{propType.Replace('<', '{').Replace('>', '}')}\"/>.";
        }

        /// <summary>
        /// Try to convert the name of a struct to useful comment.
        /// </summary>
        /// <param name="name">The name of the struct.</param>
        /// <returns>The comment.</returns>
        public static string Struct(string name)
        {
            var parts = SplitName(name);

            if (parts.Length > 0)
            {
                MakeLowerCase(parts, true);

                if (parts[parts.Length - 1].Contains("struct"))
                {
                    return "The " + string.Join(" ", parts) + '.';
                }

                return "The " + string.Join(" ", parts) + " struct.";
            }

            return "The " + name.ToLower() + " struct.";
        }

        /// <summary>
        /// Try to convert the name of a delegate to useful comment.
        /// </summary>
        /// <param name="name">The name of the delegate.</param>
        /// <returns>The comment.</returns>
        public static string Delegate(string name)
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

                    if (parts.Length > 1)
                    {
                        return parts[0] + ' ' + string.Join(" ", parts.Skip(1)) + '.';
                    }

                    return parts[0] + '.';
                }

                MakeLowerCase(parts, true);

                if (parts.Length > 1)
                {
                    if (parts[parts.Length - 1].Contains("delegate"))
                    {
                        return "The " + string.Join(" ", parts) + '.';
                    }

                    return "The " + string.Join(" ", parts) + " delegate.";
                }

                return "The " + parts[0] + '.';
            }

            return "The " + name.ToLower() + '.';
        }

        /// <summary>
        /// Generate the comment for a field.
        /// </summary>
        /// <param name="name">Name of the field.</param>
        /// <param name="hasConst">True if const otherwise false.</param>
        /// <param name="hasReadOnly">True if readonyl otherwise false.</param>
        /// <param name="equals">The equal synatx clause or null if not present in code.</param>
        /// <returns>The comment.</returns>
        public static string Field(string name, bool hasConst, bool hasReadOnly, EqualsValueClauseSyntax equals)
        {
            var sb = new StringBuilder("The ");

            if (hasConst)
            {
                sb.Append("const ");
            }

            if (hasReadOnly)
            {
                sb.Append("readonly ");
            }

            sb.Append(name);

            sb.Append('.');

            if (hasConst || hasReadOnly)
            {
                sb.Append(" Value: ");
                sb.Append(@equals.Value);
                sb.Append('.');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generate the comment for an event.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <param name="delegatenName">The name of the delegate.</param>
        /// <returns>The comment.</returns>
        public static string Event(string name, string delegatenName)
        {
            var sb = new StringBuilder("The ");

            sb.Append(name);

            sb.Append(" event of the <see cref=\"");
            sb.Append(delegatenName.Replace('<', '{').Replace('>', '}'));
            sb.Append("\"/>.");

            return sb.ToString();
        }

        /// <summary>
        /// Split the name at the uppercase letter.
        /// </summary>
        /// <param name="name">The name to split.</param>
        /// <returns>An array of words.</returns>
        private static string[] SplitName(string name)
        {
            // split the name at the uppercase letterss
            var str = SplitAtUppercase.Replace(name, " $1");
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
    }
}
