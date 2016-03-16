// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Convert.cs" company="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlDocAnalyzer
{
    /// <summary>
    /// The convert class.
    /// </summary>
    public class Convert
    {
        public static string Method(string name)
        {
            return $"The {name.ToLower()}.";
        }
    }
}
