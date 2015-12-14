using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingLibrary.JSS.IO.Files
{
    /// <summary>
    /// Holds different types of rules.
    /// </summary>
    public enum RuleTypes
    {
        /// <summary>
        /// Flags that the supplied Value MUST be contained within the filtered string for the string to pass.
        /// </summary>
        MustContain,

        /// <summary>
        /// Flags that the supplied Value MUST NOT be contained within the filtered string for the string to pass.
        /// </summary>
        ShouldNotContain,

        /// <summary>
        /// Flags that the supplied Value MUST EXACTLY EQUAL the filtered string.
        /// </summary>
        MustExactlyEqual
    }
}
