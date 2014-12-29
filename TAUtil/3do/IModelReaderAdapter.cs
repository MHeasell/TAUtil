namespace TAUtil._3do
{
    /// <summary>
    /// Adapter for the <see cref="ModelReader"/> class.
    /// Users of <see cref="ModelReader"/> should implement this interface
    /// to control how models are processed as they are read.
    /// </summary>
    public interface IModelReaderAdapter
    {
        /// <summary>
        /// Creates a child of the current object.
        /// Once the child is created, it becomes the current object.
        /// </summary>
        /// <param name="name">The name of the child.</param>
        /// <param name="position">The position of the child relative to the current object.</param>
        void CreateChild(string name, Vector position);

        /// <summary>
        /// Changes the current object to be the parent of the current object.
        /// </summary>
        void BackToParent();

        /// <summary>
        /// Adds a vertex to the current object's vertex list.
        /// </summary>
        /// <param name="v">The position of the vertex.</param>
        void AddVertex(Vector v);

        /// <summary>
        /// Adds a primitive (polygon) to the current object.
        /// </summary>
        /// <param name="color">
        /// The index of the color of the primitive
        /// in the color palette.
        /// </param>
        /// <param name="texture">
        /// The name of the texture image applied to the primitive.
        /// </param>
        /// <param name="vertexIndices">
        /// An array of indices into the object's vertex array,
        /// representing the position of each vertex.
        /// </param>
        /// <param name="isSelectionPrimitive">
        /// Indicates whether the primitive is used
        /// as the selection polygon for the model.
        /// The selection primitive is used to draw an outline around a model
        /// when it is selected in the game.
        /// There should only be one of these per model.
        /// </param>
        void AddPrimitive(int color, string texture, int[] vertexIndices, bool isSelectionPrimitive);
    }
}
