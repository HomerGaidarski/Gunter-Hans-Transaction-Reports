using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingLibrary.JSS.IO.Files
{
    /// <summary>
    /// Good for scanning strings to see if they match certain criteria.
    /// </summary>
    public class StringFilter
    {
        private bool _isCaseSensitive = true;

        /// <summary>
        /// Whether the text should be case sensitive or not.
        /// </summary>
        public bool IsCaseSensitive
        {
            get
            {
                return _isCaseSensitive;
            }
            set
            {
                _isCaseSensitive = value;
            }
        }

        private List<FilterRule> _filterRules = new List<FilterRule>();

        /// <summary>
        /// Gets the rules currently being applied to the StringFilter.
        /// </summary>
        /// <returns>A readonly list of rules.</returns>
        public IEnumerable<FilterRule> FilterRules 
        {
            get
            {
                return _filterRules.AsEnumerable<FilterRule>();
            }
        }

        /// <summary>
        /// 
        /// Adds a rule to be applied to the StringFilter.
        /// </summary>
        /// <param name="value">The value to be processed in accordance to the selected rule.</param>
        /// <param name="ruleType">Determines how a value is used.</param>
        public void AddFilterRule(string value, RuleTypes ruleType)
        {
            _filterRules.Add(new FilterRule(value, ruleType));
        }

        /// <summary>
        /// Removes all the current rules being applied to the StringFilter.
        /// </summary>
        public void ClearFilterRules()
        {
            _filterRules.Clear();
        }

        /// <summary>
        /// Checks a string against the supplied rules.
        /// </summary>
        /// <param name="str">The string to check against the supplied rules.</param>
        /// <returns>Returns True if the str matches the supplied filter rules, else returns False.</returns>
        public bool StringPassesRules(string str)
        {
            foreach (FilterRule rule in _filterRules)
            {
                if (rule.PassesThrough(str, IsCaseSensitive) == false)
                {
                    //Does not pass the rule set
                    return false;
                }
            }

            //none of the rules were broken, so the item passes the filter
            return true;
        }
    }
}
