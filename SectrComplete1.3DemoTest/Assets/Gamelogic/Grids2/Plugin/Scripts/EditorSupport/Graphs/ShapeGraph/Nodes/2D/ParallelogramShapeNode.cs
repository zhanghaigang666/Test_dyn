namespace Gamelogic.Grids2.Graph
{
	/// <summary>
	/// Node that generates a parallelogram shape.
	/// </summary>
	[ShapeNode("All/Parallelogram", 2)]
	public class ParallelogramShapeNode : PrimitiveShapeNode<GridPoint2>
	{
		public InspectableGridPoint2 dimensions;

		protected override IExplicitShape<GridPoint2> Generate()
		{
			var dimensions1 = dimensions.GetGridPoint();

			return ImplicitShape
				.Parallelogram(dimensions1)
				.ToExplicit(new GridRect(GridPoint2.Zero, dimensions1));
		}
	}
}