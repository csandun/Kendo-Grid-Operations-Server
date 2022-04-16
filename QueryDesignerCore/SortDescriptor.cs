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
            Dir = "asc";
        }

        /// <summary>
        /// Sort field name.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Sorting order.
        /// </summary>
        public string Dir { get; set; }
    }
}