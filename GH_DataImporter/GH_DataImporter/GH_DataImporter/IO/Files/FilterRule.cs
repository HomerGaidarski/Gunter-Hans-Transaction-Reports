using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodingLibrary.JSS.IO.Files
{
    /// <summary>
    /// A description of specified rules that must be Passed when a string is filtered.
    /// </summary>
    public class FilterRule
    {
        /// <summary>
        /// The string value to be used according to the RuleType.
        /// </summary>
        public string Value;

        private RuleTypes _ruleType;

        /// <summary>
        /// Flag for how the Value should be treated.
        /// </summary>
        public RuleTypes RuleType
        {
            get
            {
                return this._ruleType;
            }
            set
            {
                //Depending on this rule's type, return the proper logic associated with the rule to see if the rule was broken for a given string.
                switch (value)
                {
                    case RuleTypes.MustContain:
                        this.PassesThrough = this.MustContainString;
                        break;
                    case RuleTypes.ShouldNotContain:
                        this.PassesThrough = this.ShouldNotContainString;
                        break;
                    case RuleTypes.MustExactlyEqual:
                        this.PassesThrough = this.MustEqualStringExactly;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                //Set for the get statement
                _ruleType = value;
            }
        }

        /// <summary>
        /// Checks to see if this rule is broken with a supplied string.
        /// Two arguments:
        ///     <br>string: The string to check if it passes the filter</br>
        ///     <br>bool: Whether or not the test should be case-sensitive or not</br>
        /// </summary>
        public Func<string, bool, bool> PassesThrough
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public FilterRule(string value, RuleTypes type)
        {
            Value = value;
            RuleType = type;
        }

        /// <summary>
        /// Checks to see if the supplied string has the value assigned to this Rule inside of it somewhere.
        /// </summary>
        /// <param name="valueToCheck">The string to check for a substring in.</param>
        /// <returns>True if the string contains the substring in this ScannerRule. False if the string does not contain the substring.</returns>
        private bool MustContainString(string valueToCheck, bool caseSensitive)
        {
            if (caseSensitive == true)
            {
                //If the valueToCheck (lowercase) contains the string found in this rule, then the test passes
                if (valueToCheck.ToLower().Contains(Value.ToLower()))
                {
                    return true;
                }
            }

            //else, case sensitive is not ture
            if (valueToCheck.Contains(Value))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the supplied string does not have the value assigned to this Rule inside of it somewhere.
        /// </summary>
        /// <param name="valueToCheck">The string to check for a substring in.</param>
        /// <returns>True if the string doesn't contain the substring in this ScannerRule. False if the string does contain the substring.</returns>
        private bool ShouldNotContainString(string valueToCheck, bool caseSensitive)
        {
            return !MustContainString(valueToCheck, caseSensitive);
        }


        /// <summary>
        /// Checks to see if the supplied string equals the Value assigned to this ScannerRule exactly.
        /// </summary>
        /// <param name="valueToCheck">The value to check for a perfect match.</param>
        /// <returns>True if they are exactly equal to one another and false if they aren't.</returns>
        private bool MustEqualStringExactly(string valueToCheck, bool caseSensitive)
        {
            if (caseSensitive == true)
            {
                return valueToCheck.ToLower() == Value.ToLower();
            }

            return valueToCheck == Value;
        }

    }
}
