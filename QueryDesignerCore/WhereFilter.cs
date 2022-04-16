namespace QueryDesignerCore
{
    /// <summary>
    /// Tree filter for queryable expression.
    /// </summary>
    public class WhereFilter
    {
        /// <summary>
        /// Tree filter for queryable expression.
        /// </summary>
        public WhereFilter()
        {
            Operator = "none";
        }

        /// <summary>
        /// Filter field name.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Type of field filtration.
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Value for filtering.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Operands of collection boolean expressions.
        /// </summary>
        public Filter OperandsOfCollections { get; set; }
    }
}