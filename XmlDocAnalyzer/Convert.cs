﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Convert.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            "create",
            "delete",
            "display",
            "fill",
            "get",
            "initialize",
            "insert",
            "put",
            "register",
            "remove",
            "select",
            "set",
            "show",
            "start",
            "stop",
            "test",
            "update"
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
        /// <param name="typeArgumentList">The type argument list or null</param>
        /// <returns>The comment.</returns>
        public static string Returns(string name, TypeArgumentListSyntax typeArgumentList)
        {
            // if this list is not null the return value is generic.
            var generics = new List<string>();

            if (typeArgumentList != null)
            {
                foreach (var argument in typeArgumentList.Arguments)
                {
                    if (argument is IdentifierNameSyntax)
                    {
                        generics.Add(((IdentifierNameSyntax)argument).Identifier.ValueText);
                    }
                    else
                    {
                        if (argument is PredefinedTypeSyntax)
                        {
                            generics.Add(((PredefinedTypeSyntax)argument).Keyword.ValueText);
                        }
                        else
                        {
                            generics.Add($"Unknown type: {argument.GetType()}");
                        }
                    }
                }
            }

            if (generics.Any())
            {
                return $"The <see cref=\"{name}{{{string.Join(", ", generics)}}}\"/>.";
            }

            return $"The <see cref=\"{name}\"/>.";
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
            return $"The <see cref=\"{propType}\"/>.";
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