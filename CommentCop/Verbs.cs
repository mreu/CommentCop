// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Verbs.cs" company="Michael Reukauff, Germany">
//   Copyright © 2016 Michael Reukauff, Germany. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CommentCop
{
    using System;
    using System.Linq;

    /// <summary>
    /// The verbs class.
    /// </summary>
    internal class Verbs
    {
        /// <summary>
        /// The work items.
        /// </summary>
        private static readonly string[] verbs =
        {
            "add",
            "alter",
            "build",
            "calculate",
            "call",
            "check",
            "clean",
            "clear",
            "clone",
            "close",
            "copy",
            "commit",
            "convert",
            "create",
            "delete",
            "discard",
            "display",
            "dispose",
            "do",
            "export",
            "fetch",
            "fill",
            "find",
            "get",
            "goto",
            "import",
            "initialize",
            "insert",
            "load",
            "move",
            "open",
            "parse",
            "process",
            "pull",
            "push",
            "put",
            "read",
            "rebuild",
            "reconvert",
            "register",
            "remove",
            "reset",
            "rollback",
            "save",
            "search",
            "select",
            "set",
            "show",
            "start",
            "stop",
            "switch",
            "test",
            "translate",
            "try",
            "update",
            "validate",
            "write"
        };

        /// <summary>
        /// Check if verb is in list.
        /// </summary>
        /// <param name="verb2Check">The verb to check.</param>
        /// <returns>The <see cref="bool"/>.True if verb is in list otherwise false.</returns>
        internal static bool IsVerb(string verb2Check)
        {
            return verbs.Any(x => x.Equals(verb2Check, StringComparison.OrdinalIgnoreCase));
        }
    }
}
