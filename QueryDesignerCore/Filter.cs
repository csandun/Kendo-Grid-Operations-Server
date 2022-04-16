using System.Collections.Generic;

namespace QueryDesignerCore
{
    /// <summary>
    /// Filters with infinite nesting and boolean operations therebetween.
    /// </summary>
    public class Filter : WhereFilter
    {
        /// <summary>
        /// Filters with infinite nesting and boolean operations therebetween.
        /// </summary>
        public Filter()
        {
            Logic = "none";
        }

        /// <summary>
        /// Type of logical operator.
        /// </summary>
        public string Logic { get; set; }

        /// <summary>
        /// Operands of boolean expressions.
        /// </summary>
        public List<Filter> Filters { get; set; } 
    }
}
