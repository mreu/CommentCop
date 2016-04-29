﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegionText.cs" author="Michael Reukauff">
//   Copyright © 2016 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace XmlCommenter.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// The region text class.
    /// </summary>
    internal class RegionText
    {
        /// <summary>
        /// Get text from region.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="spanStart">The start of the span of the endregion directive.</param>
        /// <returns>The <see cref="T:Tuple{string, string}"/>.The region and endregion texts.</returns>
        internal static async Task<Tuple<string, string>> GetTextFromRegion(SyntaxNode node, int spanStart)
        {
            return await Task.Run(
               () =>
               {
                   var root = node.SyntaxTree.GetRoot();

                   var regionNodesList = new List<RegionNodes>();

                   var regions = root.DescendantNodes(null, true).OfType<RegionDirectiveTriviaSyntax>();
                   foreach (var regionDirective in regions)
                   {
                       regionNodesList.Add(new RegionNodes { RegionDirective = regionDirective });
                   }

                   var endregions = root.DescendantNodes(null, true).OfType<EndRegionDirectiveTriviaSyntax>();
                   foreach (var endRegionDirective in endregions)
                   {
                       var reg = regionNodesList.Last(x => x.RegionDirective.SpanStart < endRegionDirective.SpanStart && x.EndRegionDirective == null);
                       reg.EndRegionDirective = endRegionDirective;
                   }

                   var region = regionNodesList.FirstOrDefault(x => x.EndRegionDirective.EndOfDirectiveToken.SpanStart.Equals(spanStart))
                                ?? regionNodesList.FirstOrDefault(x => x.EndRegionDirective.EndOfDirectiveToken.FullSpan.Start.Equals(spanStart))
                                ?? regionNodesList.FirstOrDefault(x => x.EndRegionDirective.SpanStart.Equals(spanStart));

                   if (region != null)
                   {
                       var regionToken = region.RegionDirective.EndOfDirectiveToken;
                       var endregionToken = region.EndRegionDirective.EndOfDirectiveToken;
                       if (regionToken.HasLeadingTrivia)
                       {
                           var regiontext = regionToken.LeadingTrivia.ToString();

                           if (endregionToken.HasLeadingTrivia)
                           {
                               var endregiontext = endregionToken.LeadingTrivia.ToString();

                               return new Tuple<string, string>(regiontext, endregiontext);
                           }

                           return new Tuple<string, string>(regiontext, string.Empty);
                       }

                       if (endregionToken.HasLeadingTrivia)
                       {
                           var endregiontext = endregionToken.LeadingTrivia.ToString();

                           return new Tuple<string, string>(string.Empty, endregiontext);
                       }
                   }

                   return null;
               });
        }

        /// <summary>
        /// The region nodes class.
        /// </summary>
        private class RegionNodes
        {
            /// <summary>
            /// Gets or sets the region directive.
            /// </summary>
            public RegionDirectiveTriviaSyntax RegionDirective { get; set; }

            /// <summary>
            /// Gets or sets the end region directive.
            /// </summary>
            public EndRegionDirectiveTriviaSyntax EndRegionDirective { get; set; }
        }
    }
}
