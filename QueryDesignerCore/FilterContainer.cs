using System.Collections.Generic;

namespace QueryDesignerCore
{
    /// <summary>
    /// Container for filters.
    /// </summary>
    public class FilterContainer
    {
        /// <summary>
        /// Container for filters.
        /// </summary>
        public FilterContainer()
        {
            Take = -1;
        }

        /// <summary>
        /// Where filters.
        /// </summary>
        public Filter Filter { get; set; }

        /// <summary>
        /// Order filters.
        /// </summary>
        public List<SortDescriptor> sort { get; set; }

        /// <summary>
        /// Skip number of elements.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Take number of elements.
        /// </summary>
        public int Take { get; set; }
    }
}
