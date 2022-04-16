namespace QueryDesignerCore
{
    /// <summary>
    /// Sort by the field.
    /// </summary>
    public class SortDescriptor
    {
        /// <summary>
        /// Sort by the field.
        /// </summary>
        public SortDescriptor()
        {
            Order = OrderFilterType.Asc;
        }

        /// <summary>
        /// Sort field name.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Sorting order.
        /// </summary>
        public OrderFilterType Order { get; set; }
    }
}