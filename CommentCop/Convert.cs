// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Convert.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CommentCop
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
        /// Split at uppercase letter regex.
        /// </summary>
        private static readonly Regex SplitAtUppercase = new Regex(@"([A-Z])(?<=[a-z]\1|[A-Za-z]\1(?=[a-z]))", RegexOptions.CultureInvariant | RegexOptions.Singleline);

        /// <summary>
        /// Try to convert the name of a class to useful comment.
        /// </summary>
        /// <param name="name">The name of the class.</param>
        /// <param name="isTestClass">True if the class is a test class.</param>
        /// <returns>The comment.</returns>
        public static string Class(string name, bool isTestClass)
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
                    if (isTestClass)
                    {
                        newName = "The " + string.Join(" ", parts) + " unit test class.";
                    }
                    else
                    {
                        newName = "The " + string.Join(" ", parts) + " class.";
                    }
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
        /// <param name="isUnittest">True if it is a unit test method otherwise false.</param>
        /// <returns>The comment.</returns>
        public static string Method(string name, bool isUnittest)
        {
            if (isUnittest)
            {
                if (name.Contains("_"))
                {
                    return UnittestName(name);
                }
            }

            var special = CheckForSpecialMethodNames(name);
            if (special != null)
            {
                return special;
            }

            var parts = SplitName(name);

            if (parts.Length > 0)
            {
                // if name of method starts with "on", assuming it is a raise event method
                if (parts[0].Equals("on", StringComparison.OrdinalIgnoreCase))
                {
                    MakeLowerCase(parts, false);

                    return "Raises the " + string.Join(" ", parts.Skip(1)) + " event.";
                }

                if (Verbs.IsVerb(parts[0]))
                {
                    MakeLowerCase(parts, false);

                    if (parts.Length > 1)
                    {
                        return parts[0] + " the " + string.Join(" ", parts.Skip(1)) + '.';
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
        /// The unittest name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string UnittestName(string name)
        {
            var newName = string.Empty;

            var parts = name.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

            var first = true;
            foreach (var part in parts)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    newName += " --> ";
                }

                var x = SplitName(part);
                MakeLowerCase(x, false);
                if (x.Length > 1)
                {
                    newName += string.Join(" ", x);
                }
            }

            return newName;
        }

        /// <summary>
        /// Try to convert the return type of a method to useful comment.
        /// </summary>
        /// <param name="name">The name of the return type.</param>
        /// <returns>The comment.</returns>
        public static string Returns(string name)
        {
            if (name.EndsWith("?"))
            {
                name = name.Replace("?", string.Empty);
            }

            if (name.EndsWith("[]") || name.EndsWith(">"))
            {
                name = "T:" + name;
            }

            return $"The <see cref=\"{name.Replace('<', '{').Replace('>', '}')}\"/>.";
        }

        /// <summary>
        /// Try to convert the parameter to a useful comment.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="paramType">The type of the parameter.</param>
        /// <returns>The comment.</returns>
        public static string Parameter(string name, string paramType)
        {
            if (paramType.EndsWith("eventargs", StringComparison.OrdinalIgnoreCase))
            {
                // if full qualified, get the last qualifier
                var parts = paramType.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                parts = SplitName(parts.Last());

                MakeLowerCase(parts, true);

                parts[parts.Length - 1] = "arguments.";

                return "The " + string.Join(" ", parts);
            }

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
                var parts = SplitName(name);
                MakeLowerCase(parts, true);
                comment += " the " + string.Join(" ", parts) + '.';
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
                if (Verbs.IsVerb(parts[0]))
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
        /// <param name="isConst">True if const otherwise false.</param>
        /// <param name="isReadOnly">True if readonyl otherwise false.</param>
        /// <param name="equals">The equal synatx clause or null if not present in code.</param>
        /// <returns>The comment.</returns>
        public static string Field(string name, bool isConst, bool isReadOnly, EqualsValueClauseSyntax equals)
        {
            var sb = new StringBuilder("The ");

            var parts = SplitName(name);
            MakeLowerCase(parts, true);

            sb.Append(string.Join(" ", parts));

            if (isConst)
            {
                sb.Append(" (const)");
            }

            if (isReadOnly)
            {
                sb.Append(" (readonly)");
            }

            sb.Append('.');

            if (isConst || isReadOnly)
            {
                if (equals != null)
                {
                    sb.Append(" Value: ");
                    sb.Append(MultiLineToSingleLine(equals.Value.ToString()));
                    sb.Append('.');
                }
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

            var parts = SplitName(name);
            MakeLowerCase(parts, true);

            sb.Append(string.Join(" ", parts));

            if (name.ToLower().Contains("event"))
            {
                sb.Append(" of the <see cref=\"");
            }
            else
            {
                sb.Append(" event of the <see cref=\"");
            }

            sb.Append(delegatenName.Replace('<', '{').Replace('>', '}'));
            sb.Append("\"/>.");

            return sb.ToString();
        }

        /// <summary>
        /// Generate the comment for an enum.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>The comment.</returns>
        public static string Enum(string name)
        {
            var parts = SplitName(name);
            MakeLowerCase(parts, true);

            if (parts[parts.Length - 1].ToLower().Contains("enum"))
            {
                return "The " + string.Join(" ", parts) + '.';
            }

            return "The " + string.Join(" ", parts) + " enum.";
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
            str = str.Replace('_', ' ');
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
            // skip the first word in the list if includefirst is false
            for (var ix = includeFirst ? 0 : 1; ix < parts.Length; ix++)
            {
                // if word is longer than 1 character check if second character is uppercase
                // if yes do not change the word
                if (parts[ix].Length > 1)
                {
                    if (parts[ix][1] >= 'A' && parts[ix][1] <= 'Z')
                    {
                        continue;
                    }

                    parts[ix] = parts[ix].ToLower();
                }
                else
                {
                    // only I is uppercase all others are lowercase
                    if (parts[ix].Equals("i", StringComparison.OrdinalIgnoreCase))
                    {
                        parts[ix] = "I";
                    }
                    else
                    {
                        parts[ix] = parts[ix].ToLower();
                    }
                }
            }
        }

        /// <summary>
        /// Convert a multi line text to a single line text.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>One single line of text.</returns>
        private static string MultiLineToSingleLine(string text)
        {
            var temp = text.Replace(Environment.NewLine, "\n").Replace("\r", "\n");

            var parts = temp.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (var index = 0; index < parts.Length; index++)
            {
                parts[index] = parts[index].Trim();
            }

            temp = string.Join(" ", parts);
            temp = temp.Replace("( ", "(").Replace("<", "&lt;").Replace(">", "&gt;");

            return temp;
        }
    }
}
