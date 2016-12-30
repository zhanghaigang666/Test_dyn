//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using Gamelogic.Extensions.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Grids
{
	/// <summary>
	/// This class provide generic functions for common grid operations, such as finding a 
	/// shortest path or connected shapes.
	/// </summary>
	[Version(1)]
	public static partial class Algorithms
	{
		#region Graph Algorithms

		/// <summary>
		/// The set is connected if
		/// the set of points are neighbor-connected, and isNeighborsConnected return true for each
		/// two neighbors in the set.Two points are connected if they are neighbors, or one point has 
		/// a neighbor that is neighbor-connected with the other point.
		/// </summary>
		/// <example>
		/// checks whether the list of points are connected by color.
		/// <code>
		/// IsConnected(grid, points, (p, q) => grid[p].Color == grid[q].Color)
		/// </code>
		/// </example>
		/// <typeparam name="TCell">The type of cell of the grid that this algorithm takes.</typeparam>
		/// <typeparam name="TPoint">The type of point of the grid that this algorithm takes.</typeparam>
		/// <param name="grid">The grid on which to do the check</param>
		/// <param name="points">The set of points to check. It is assumed all points are in the grid.</param>
		/// <param name="isNeighborsConnected">The function to use to check whether two neighbors are connected</param>
		/// <returns>Returns true if the collection of points are connected.</returns>
		//TODO: what should be done about points not in the grid?
		public static bool IsConnected<TCell, TPoint>(
			IGrid<TCell, TPoint> grid,
			IEnumerable<TPoint> points,
			Func<TPoint, TPoint, bool> isNeighborsConnected)

			where TPoint : IGridPoint<TPoint>
		{
			//What if collection is empty?

			var openSet = new HashSet<TPoint>(new PointComparer<TPoint>());
			var closedSet = new HashSet<TPoint>(new PointComparer<TPoint>());

			openSet.Add(points.First());

			while (!openSet.IsEmpty())
			{
				var current = openSet.First();

				openSet.Remove(current);
				closedSet.Add(current);

				//Adds all connected neighbors that 
				//are in the grid and in the collection
				var connectedNeighbors = from neighbor in grid.GetNeighbors(current)
										 where !closedSet.Contains(neighbor)
											 && isNeighborsConnected(current, neighbor)
											 && points.Contains(neighbor)
										 select neighbor;

				openSet.AddRange(connectedNeighbors);
			}

			return points.All(closedSet.Contains);
		}

		/// <summary>
		/// The set is connected if
		/// the set of points are neighbor-connected, and isNeighborsConnected return true for each
		/// two neighbors in the set.Two points are connected if they are neighbors, or one point has
		/// 
		/// a neighbor that is neighbor-connected with the other point.
		/// 
		/// Another way to put this is, this function returns true if there is a set that connects point1
		/// to point2.
		/// </summary>
		/// <example>
		/// checks whether the two points are connected by color.
		/// <code>
		/// IsConnected(grid, p1, p2, (p, q) => grid[p].Color == grid[q].Color)
		/// </code>
		/// </example>
		/// <typeparam name="TCell">The type of cell of the grid that this algorithm takes.</typeparam>
		/// <typeparam name="TPoint">The type of point of the grid that this algorithm takes.</typeparam>
		/// <param name="grid">The grid on which to do the check</param>
		/// <param name="point1">The first point to check.</param>
		/// <param name="point2">The second point to check.</param>
		/// <param name="isNeighborsConnected">The function to use to check whether two neighbors are connected.</param>
		/// <returns>Returns true if the two points are in a connected set.</returns>
		public static bool IsConnected<TCell, TPoint>(
			IGrid<TCell, TPoint> grid,
			TPoint point1,
			TPoint point2,
			Func<TPoint, TPoint, bool> isNeighborsConnected)

			where TPoint : IGridPoint<TPoint>
		{
			var openSet = new HashSet<TPoint>(new PointComparer<TPoint>()) { point1 };
			var closedSet = new HashSet<TPoint>(new PointComparer<TPoint>());

			while (!openSet.IsEmpty())
			{
				var current = openSet.First();

				if (current.Equals(point2))
				{
					return true;
				}

				openSet.Remove(current);
				closedSet.Add(current);

				var connectedNeighbors =
					from neighbor in grid.GetNeighbors(current)
					where !closedSet.Contains(neighbor) && isNeighborsConnected(current, neighbor)
					select neighbor;

				openSet.AddRange(connectedNeighbors);
			}

			return false;
		}

		/// <typeparam name="TCell">The type of cell of the grid that this algorithm takes.</typeparam>
		/// <typeparam name="TPoint">The type of point of the grid that this algorithm takes.</typeparam>		
		/// <param name="isNeighborsConnected">The function to use to check whether two neighbors are connected</param>
		/// <returns>Returns a list of points connected to the given point.</returns>
		public static HashSet<TPoint> GetConnectedSet<TCell, TPoint>(
			IGrid<TCell, TPoint> grid,
			TPoint point,
			Func<TPoint, TPoint, bool> isNeighborsConnected)

			where TPoint : IGridPoint<TPoint>
		{
			var openSet = new HashSet<TPoint>(new PointComparer<TPoint>()) { point };
			var closedSet = new HashSet<TPoint>(new PointComparer<TPoint>());

			while (!openSet.IsEmpty())
			{
				var current = openSet.First();

				openSet.Remove(current);
				closedSet.Add(current);

				var connectedNeighbors =
					from neighbor in grid.GetNeighbors(current)
					where !closedSet.Contains(neighbor) && isNeighborsConnected(current, neighbor)
					select neighbor;

				openSet.AddRange(connectedNeighbors);
			}

			return closedSet;
		}

		/// <summary>
		/// Find the shortest path between a start and goal node. 
		/// 
		/// The distance between nodes(as defined by TPoint.DistanceFrom)
		/// are used as the heuristic and actual cost between nodes.In some cases the 
		/// result may be unintuitive, and an overload specifying a different
		/// cost should be used.
		/// 
		/// See AStar&lt;TCell, TPoint&gt;(IGrid&lt;TCell, TPoint&gt;, TPoint, TPoint, Func&lt;TPoint, TPoint, float&gt;, Func&lt;TCell, bool&gt;, Func&lt;TPoint, TPoint, float&gt;)
		/// </summary>
		/// <returns>The list of nodes on the path in order.If no path is possible, null is returned.</returns>
		public static IEnumerable<TPoint> AStar<TCell, TPoint>(
			IGrid<TCell, TPoint> grid,
			TPoint start,
			TPoint goal)

			where TPoint : IGridPoint<TPoint>
		{
			return AStar(grid, start, goal, (p1, p2) => p1.DistanceFrom(p2), x => true, (p1, p2) => p1.DistanceFrom(p2));
		}

		/// <summary>
		/// Find the shortest path between a start and goal node.
		/// 
		/// The distance between nodes(as defined by TPoint.DistanceFrom) 
		/// are used as the actual cost between nodes.In some cases the 
		/// result may be unintuitive, and an overload specifying a different
		/// cost should be used.
		/// 
		/// Using an overload with an appropriate distance function can solve the
		/// issue.
		/// 
		/// See AStar&lt;TCell, TPoint&gt;(IGrid&lt;TCell, TPoint&gt;, TPoint, TPoint, Func&lt;TPoint, TPoint, float&gt;, Func&lt;TCell, bool&gt;, Func&lt;TPoint, TPoint, float&gt;)
		/// </summary>
		[Obsolete("Use the overload instead that allows you to specify the cost in addition to the cost heuristic")]
		public static IEnumerable<TPoint> AStar<TCell, TPoint>(
			IGrid<TCell, TPoint> grid,
			TPoint start,
			TPoint goal,
			Func<TPoint, TPoint, float> heuristicCostEstimate,
			Func<TCell, bool> isAccessible)

			where TPoint : IGridPoint<TPoint>
		{
			return AStar(
				grid,
				start,
				goal,
				heuristicCostEstimate,
				isAccessible,
				(p, q) => p.DistanceFrom(q));
		}

		public static IEnumerable<TPoint> AStar<TCell, TPoint>(
			IGrid<TCell, TPoint> grid,
			TPoint start,
			TPoint goal,
			Func<TPoint, TPoint, float> heuristicCostEstimate,
			Func<TCell, bool> isAccessible,
			Func<TPoint, TPoint, float> neighborToNeighborCost)

			where TPoint : IGridPoint<TPoint>
		{
			var closedSet = new HashSet<TPoint>(new PointComparer<TPoint>());

			// The set of tentative nodes to be evaluated
			var openSet = new HashSet<TPoint>(new PointComparer<TPoint>()) {start};

			// The map of navigated nodes.
			var cameFrom = new Dictionary<TPoint, TPoint>(new PointComparer<TPoint>());

			// Cost from start along best known path.
			var gScore = new Dictionary<TPoint, float>(new PointComparer<TPoint>());
			gScore[start] = 0;

			// Estimated total cost from start to goal through y.
			var fScore = new Dictionary<TPoint, float>(new PointComparer<TPoint>());
			fScore[start] = gScore[start] + heuristicCostEstimate(start, goal);

			while (!openSet.IsEmpty())
			{
				var current = FindNodeWithLowestScore(openSet, fScore);

				if (current.Equals(goal))
				{
					return ReconstructPath(cameFrom, goal);
				}

				openSet.Remove(current);
				closedSet.Add(current);

				var currentNodeNeighbors = grid.GetNeighbors(current);

				var accessibleNeighbors = from neighbor in currentNodeNeighbors
										  where isAccessible(grid[neighbor])
										  select neighbor;

				foreach (var neighbor in accessibleNeighbors)
				{
					var tentativeGScore = gScore[current] + neighborToNeighborCost(current, neighbor);

					if (closedSet.Contains(neighbor))
					{
						if (tentativeGScore >= gScore[neighbor])
						{
							continue;
						}
					}

					if (!openSet.Contains(neighbor) || tentativeGScore < gScore[neighbor])
					{
						cameFrom[neighbor] = current;
						gScore[neighbor] = tentativeGScore;
						fScore[neighbor] = gScore[neighbor] + heuristicCostEstimate(neighbor, goal);

						if (!openSet.Contains(neighbor))
						{
							openSet.Add(neighbor);
						}
					}
				}
			}

			return null;
		}

		//TODO: Refactor with method below
		[Experimental]
		public static IEnumerable<TPoint> AStar2<TCell, TPoint>(
			IGrid<TCell, TPoint> grid,
			TPoint start,
			TPoint goal,
			Func<TPoint, TPoint, float> heuristicCostEstimate,
			Func<TPoint, bool> isAccessible,
			Func<TPoint, TPoint, float> neighborToNeighborCost)

			where TPoint : IGridPoint<TPoint>
		{
			var closedSet = new HashSet<TPoint>(new PointComparer<TPoint>());

			// The set of tentative nodes to be evaluated
			var openSet = new HashSet<TPoint>(new PointComparer<TPoint>()) { start };

			// The map of navigated nodes.
			var cameFrom = new Dictionary<TPoint, TPoint>(new PointComparer<TPoint>());

			// Cost from start along best known path.
			var gScore = new Dictionary<TPoint, float>(new PointComparer<TPoint>());
			gScore[start] = 0;

			// Estimated total cost from start to goal through y.
			var fScore = new Dictionary<TPoint, float>(new PointComparer<TPoint>());
			fScore[start] = gScore[start] + heuristicCostEstimate(start, goal);

			while (!openSet.IsEmpty())
			{
				var current = FindNodeWithLowestScore(openSet, fScore);

				if (current.Equals(goal))
				{
					return ReconstructPath(cameFrom, goal);
				}

				openSet.Remove(current);
				closedSet.Add(current);

				var currentNodeNeighbors = grid.GetNeighbors(current);

				var accessibleNeighbors = from neighbor in currentNodeNeighbors
											where isAccessible(neighbor)
											select neighbor;

				foreach (var neighbor in accessibleNeighbors)
				{
					var tentativeGScore = gScore[current] + neighborToNeighborCost(current, neighbor);

					if (closedSet.Contains(neighbor))
					{
						if (tentativeGScore >= gScore[neighbor])
						{
							continue;
						}
					}

					if (!openSet.Contains(neighbor) || tentativeGScore < gScore[neighbor])
					{
						cameFrom[neighbor] = current;
						gScore[neighbor] = tentativeGScore;
						fScore[neighbor] = gScore[neighbor] + heuristicCostEstimate(neighbor, goal);

						if (!openSet.Contains(neighbor))
						{
							openSet.Add(neighbor);
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// A generic function that returns the points in range
		/// based on a given start point, moveRange, and a function that returns the
		/// cost of moving between neighboring cells.
		/// 
		/// author Justin Kivak
		/// </summary>
		/// <example>
		/// Example usage:
		/// <code>
		/// var tilesInRange = Algorithms.GetPointsInRange(
		///		grid,
		///		start,
		///		tile1, tile2 => DistanceBetween(tile1, tile2),
		///		5f);
		/// </code>
		/// </example>
		/// <param name="grid">The grid to use for the calculations</param>
		/// <param name="start">The starting point for the range calculation</param>
		/// <param name="isAcessible">Whether the given cell can be reached</param>
		/// <param name="getCellMoveCost">A function that returns the move cost given a cell</param>
		/// <param name="moveRange">The range from which to return cells</param>
		[Version(1,10)]
		public static IEnumerable<TPoint> GetPointsInRange<TCell, TPoint>(
			IGrid<TCell, TPoint> grid,
			TPoint start,
			Func<TCell, bool> isAcessible,
			Func<TPoint, TPoint, float> getCellMoveCost,
			float moveRange)
			where TPoint : IGridPoint<TPoint>
		{
			// Nodes in range
			var closedSet = new HashSet<TPoint>(new PointComparer<TPoint>());
			var costToReach = new Dictionary<TPoint, float>(new PointComparer<TPoint>());
			var openSet = new PointList<TPoint>{start};
			
			// Cost from start along best known path up to current point
			var costToReachContains = grid.CloneStructure(false);

			costToReach[start] = 0;

			while (!openSet.IsEmpty())
			{
				// Process current node
				var current = FindNodeWithLowestScore(openSet, costToReach);
				openSet.Remove(current);
				closedSet.Add(current);

				//foreach (var neighbor in grid.GetNeighbors(current))
				var neighbors = grid.GetNeighbors(current).ToPointList();
				
				for(int i = 0; i < neighbors.Count; i++)
				{
					var neighbor = neighbors[i];

					if (!closedSet.Contains(neighbor) &&
						!openSet.Contains(neighbor) &&
						isAcessible(grid[neighbor]) &&
						(costToReach[current] + getCellMoveCost(current, neighbor) <= moveRange)
						)
					{
						// Cost of current node + neighbor's move cost
						float newCost = costToReach[current] + getCellMoveCost(current, neighbor);
						
						if (costToReachContains[neighbor])
						{
							if (costToReach[neighbor] > newCost)
							{
								costToReach[neighbor] = newCost;
							}
						}
						else
						{
							costToReach[neighbor] = newCost;
							costToReachContains[neighbor] = true;
						}
						
						openSet.Add(neighbor);
					}
				}
			}

			return closedSet;
		}

		public static IEnumerable<TPoint> GetPointsInRange<TCell, TPoint>(
			IGrid<TCell, TPoint> grid,
			TPoint start,
			Func<TCell, bool> isAcessible,
			Func<TPoint, TPoint, int> getCellMoveCost,
			int moveRange)
			where TPoint : IGridPoint<TPoint>
		{
			// Nodes in range
			var closedSet = new HashSet<TPoint>(new PointComparer<TPoint>());
			var costToReach = new Dictionary<TPoint, int>(new PointComparer<TPoint>());
			var openSet = new PointList<TPoint> {start};

			// Cost from start along best known path up to current point
			var costToReachContains = grid.CloneStructure(false);

			costToReach[start] = 0;

			while (!openSet.IsEmpty())
			{
				// Process current node
				var current = FindNodeWithLowestScore(openSet, costToReach);
				openSet.Remove(current);
				closedSet.Add(current);

				//foreach (var neighbor in grid.GetNeighbors(current))
				var neighbors = grid.GetNeighbors(current).ToPointList();

				for (int i = 0; i < neighbors.Count; i++)
				{
					var neighbor = neighbors[i];

					if (!closedSet.Contains(neighbor) &&
						!openSet.Contains(neighbor) &&
						isAcessible(grid[neighbor]) &&
						(costToReach[current] + getCellMoveCost(current, neighbor) <= moveRange)
						)
					{
						// Cost of current node + neighbor's move cost
						int newCost = costToReach[current] + getCellMoveCost(current, neighbor);

						if (costToReachContains[neighbor])
						{
							if (costToReach[neighbor] > newCost)
							{
								costToReach[neighbor] = newCost;
							}
						}
						else
						{
							costToReach[neighbor] = newCost;
							costToReachContains[neighbor] = true;
						}

						openSet.Add(neighbor);
					}
				}
			}

			return closedSet;
		}

		/// <summary>
		/// A generic function that returns the points in range
		/// based on a given start point, moveRange, and a function that returns the
		/// cost of moving between neighboring cells.
		/// 
		/// author Justin Kivak
		/// </summary>
		/// <example>
		/// Example usage:
		/// <code>
		/// var costs = Algorithms.GetPointsInRangeCost(
		///		grid,
		///		start,
		///		tile1, tile2 => DistanceBetween(tile1, tile2),
		///		5f);
		/// </code>
		/// </example>
		/// <param name="grid">The grid to use for the calculations</param>
		/// <param name="start">The starting point for the range calculation</param>		
		/// <param name="getCellMoveCost">A function that returns the move cost given a cell</param>
		/// <param name="moveRange">The range from which to return cells</param>		
		[Version(1,10)]
		public static Dictionary<TPoint, float> GetPointsInRangeCost<TCell, TPoint>(
			IGrid<TCell, TPoint> grid,
			TPoint start,
			Func<TCell, bool> isAcessible,
			Func<TPoint, TPoint, float> getCellMoveCost,
			float moveRange)
			where TPoint : IGridPoint<TPoint>
		{
			// Nodes in range
			var closedSet = new HashSet<TPoint>(new PointComparer<TPoint>());
			var costToReach = new Dictionary<TPoint, float>(new PointComparer<TPoint>());
			var openSet = new PointList<TPoint> {start};

			// Cost from start along best known path up to current point
			var costToReachContains = grid.CloneStructure(false);

			costToReach[start] = 0;

			while (!openSet.IsEmpty())
			{
				// Process current node
				var current = FindNodeWithLowestScore(openSet, costToReach);
				openSet.Remove(current);

				closedSet.Add(current);

				//foreach (var neighbor in grid.GetNeighbors(current))
				var neighbors = grid.GetNeighbors(current).ToPointList();

				for (int i = 0; i < neighbors.Count; i++)
				{
					var neighbor = neighbors[i];

					if (!closedSet.Contains(neighbor) &&
						!openSet.Contains(neighbor) &&
						isAcessible(grid[neighbor]) &&
						(costToReach[current] + getCellMoveCost(current, neighbor) <= moveRange)
						)
					{
						// Cost of current node + neighbor's move cost
						float newCost = costToReach[current] + getCellMoveCost(current, neighbor);

						if (costToReachContains[neighbor])
						{
							if (costToReach[neighbor] > newCost)
							{
								costToReach[neighbor] = newCost;
							}
						}
						else
						{
							costToReach[neighbor] = newCost;
							costToReachContains[neighbor] = true;
						}

						openSet.Add(neighbor);
					}
				}
			}

			return costToReach;
		}

		public static Dictionary<TPoint, int> GetPointsInRangeCost<TCell, TPoint>(
			IGrid<TCell, TPoint> grid,
			TPoint start,
			Func<TCell, bool> isAcessible,
			Func<TPoint, TPoint, int> getCellMoveCost,
			int moveRange)
			where TPoint : IGridPoint<TPoint>
		{
			// Nodes in range
			var closedSet = new HashSet<TPoint>(new PointComparer<TPoint>());
			var costToReach = new Dictionary<TPoint, int>(new PointComparer<TPoint>());
			var openSet = new PointList<TPoint>{start};

			// Cost from start along best known path up to current point
			var costToReachContains = grid.CloneStructure(false);

			costToReach[start] = 0;

			while (!openSet.IsEmpty())
			{
				// Process current node
				var current = FindNodeWithLowestScore(openSet, costToReach);
				openSet.Remove(current);

				closedSet.Add(current);

				//foreach (var neighbor in grid.GetNeighbors(current))
				var neighbors = grid.GetNeighbors(current).ToPointList();

				for (int i = 0; i < neighbors.Count; i++)
				{
					var neighbor = neighbors[i];

					if (!closedSet.Contains(neighbor) &&
						!openSet.Contains(neighbor) &&
						isAcessible(grid[neighbor]) &&
						(costToReach[current] + getCellMoveCost(current, neighbor) <= moveRange)
						)
					{
						// Cost of current node + neighbor's move cost
						int newCost = costToReach[current] + getCellMoveCost(current, neighbor);

						if (costToReachContains[neighbor])
						{
							if (costToReach[neighbor] > newCost)
							{
								costToReach[neighbor] = newCost;
							}
						}
						else
						{
							costToReach[neighbor] = newCost;
							costToReachContains[neighbor] = true;
						}

						openSet.Add(neighbor);
					}
				}
			}

			return costToReach;
		}
		#endregion

		#region Lines

		/// <summary>
		/// Returns a list containing lines connected to the given points. A line is a list of points.
		/// Only returns correct results for square or hex grids.
		/// </summary>
		/// <example>
		/// <code>
		/// private bool IsSameColour(point1, point2)
		/// {
		///		return grid[point1].Color == grid[point2].Color;
		/// }
		/// 
		/// private SomeMethod()
		/// {
		///		...
		///		var rays = GetConnectedRays&lt;ColourCell, PointyHexPoint, PointyHexNeighborIndex&gt;(
		///			grid, point, IsSameColour);
		///		...
		/// }
		/// </code>
		/// You can of course also use a lambda expression, like this:
		/// <code>
		/// //The following code returns all lines that radiate from the given point 
		/// GetConnectedRays&lt;ColourCell, PointyHexPoint, PointyHexNeighborIndex&gt;(
		///		grid, point, (x, y) => grid[x].Color == grid[y].Color);
		/// </code>
		/// </example>
		/// <typeparam name="TCell">The type of cell of the grid that this algorithm takes.</typeparam>
		/// <typeparam name="TPoint">The type of point of the grid that this algorithm takes.</typeparam>
		/// <param name="grid"></param>
		/// <param name="point"></param>
		/// <param name="isNeighborsConnected">
		/// A functions that returns true or false, depending on whether
		/// two points can be considered connected when they are neighbors.For example, if you want 
		/// rays of points that refer to cells of the same color, you can pass in a functions that
		/// compares the DefaultColors of cells.
		/// </param>
		public static IEnumerable<IEnumerable<TPoint>> GetConnectedRays<TCell, TPoint>(
			AbstractUniformGrid<TCell, TPoint> grid,
			TPoint point,
			Func<TPoint, TPoint, bool> isNeighborsConnected)

			where TPoint : IVectorPoint<TPoint>, IGridPoint<TPoint>
		{
			var lines = new List<IEnumerable<TPoint>>();

			foreach (var direction in grid.NeighborDirections)
			{
				var line = new PointList<TPoint>();

				var edge = point;

				while (grid.Contains(edge) && isNeighborsConnected(point, edge))
				{
					line.Add(edge);
					edge = edge.MoveBy(direction);
				}

				if (line.Count > 1)
				{
					lines.Add(line);
				}
			}

			return lines;
		}

		/// <summary>
		/// Gets the longest of the rays connected to this cell.
		/// <see cref="GetConnectedRays{TCell,TPoint}"/> 
		/// </summary>
		/// <typeparam name="TCell">The type of cell of the grid that this algorithm takes.</typeparam>
		/// <typeparam name="TPoint">The type of point of the grid that this algorithm takes.</typeparam>
		public static IEnumerable<TPoint> GetLongestConnectedRay<TCell, TPoint>(
			AbstractUniformGrid<TCell, TPoint> grid,
			TPoint point,
			Func<TPoint, TPoint, bool> isNeighborsConnected)

			where TPoint : IVectorPoint<TPoint>, IGridPoint<TPoint>
		{
			var rays = GetConnectedRays(grid, point, isNeighborsConnected);

			return GetBiggestShape(rays);
		}

		/// <summary>
		/// Gets the longest line of connected points that contains this point.
		/// <see cref="GetConnectedRays{TCell,TPoint}"/> 
		/// </summary>
		/// <typeparam name="TCell">The type of cell of the grid that this algorithm takes.</typeparam>
		/// <typeparam name="TPoint">The type of point of the grid that this algorithm takes.</typeparam>
		/// <typeparam name="TBasePoint">The base type of the point.</typeparam>
		public static IEnumerable<IEnumerable<TPoint>> GetConnectedLines<TCell, TPoint, TBasePoint>(
			IEvenGrid<TCell, TPoint, TBasePoint> grid,
			TPoint point,
			Func<TPoint, TPoint, bool> isNeighborsConnected)

			where TPoint : ISplicedVectorPoint<TPoint, TBasePoint>, IGridPoint<TPoint>
			where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
		{
			var lines = new List<IEnumerable<TPoint>>();

			foreach (var direction in grid.GetPrincipleNeighborDirections())
			{
				var line = new PointList<TPoint>();
				var edge = point;

				//go forwards
				while (grid.Contains(edge) && isNeighborsConnected(point, edge))
				{
					edge = edge.MoveBy(direction);
				}

				var oppositeDirection = direction.Negate();
				//TPoint oppositeNeighbor = point.MoveBy(direction.Negate());
				edge = edge.MoveBy(oppositeDirection);

				//go backwards
				while (grid.Contains(edge) && isNeighborsConnected(point, edge))
				{
					line.Add(edge);
					edge = edge.MoveBy(oppositeDirection);
				}

				if (line.Count > 1)
				{
					lines.Add(line);
				}
			}

			return lines;
		}

		/// <summary>
		/// Get the longest line of points connected to the given point
		/// </summary>
		/// <typeparam name="TCell">The type of cell of the grid that this algorithm takes.</typeparam>
		/// <typeparam name="TPoint">The type of point of the grid that this algorithm takes.</typeparam>
		/// <typeparam name="TBasePoint">The base type of the point.</typeparam>
		public static IEnumerable<TPoint> GetLongestConnectedLine<TCell, TPoint, TBasePoint>(
			IEvenGrid<TCell, TPoint, TBasePoint> grid,
			TPoint point,
			Func<TPoint, TPoint, bool> isNeighborsConnected)

			where TPoint : ISplicedVectorPoint<TPoint, TBasePoint>, IGridPoint<TPoint>
			where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
		{
			var lines = GetConnectedLines(grid, point, isNeighborsConnected);

			return GetBiggestShape(lines);
		}

		/// <summary>
		/// <see cref="GetLongestConnectedLine{TCell,TPoint,TBasePoint}"/> 
		/// </summary>
		[Obsolete("Use GetLongestConnectedLine instead. Will be removed in a future version.")]
		public static IEnumerable<TPoint> GetLongestConnected<TCell, TPoint, TBasePoint>(
			IEvenGrid<TCell, TPoint, TBasePoint> grid,
			TPoint point,
			Func<TPoint, TPoint, bool> isNeighborsConnected)

			where TPoint : ISplicedVectorPoint<TPoint, TBasePoint>, IGridPoint<TPoint>
			where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
		{
			return GetLongestConnectedLine(grid, point, isNeighborsConnected);
		}
		#endregion

		#region Shapes (Collections of points)

		/// <summary>
		/// Gets the biggest shape (by number of points) in the given list.
		/// </summary>
		/// <typeparam name="TPoint">The type of point of the shapes.</typeparam>
		/// <param name="shapes">Each shape is represented as a list of points.</param>
		public static IEnumerable<TPoint> GetBiggestShape<TPoint>(
			IEnumerable<IEnumerable<TPoint>> shapes)

			where TPoint : IGridPoint<TPoint>
		{
			return shapes.MaxBy(x => x.Count());
		}

		public static bool Contains<TPoint>(IEnumerable<TPoint> bigShape, IEnumerable<TPoint> smallShape)
			where TPoint : IGridPoint<TPoint>
		{
			return smallShape.All(bigShape.Contains);
		}

		public static bool IsEquivalent<TPoint>(IEnumerable<TPoint> shape1, IEnumerable<TPoint> shape2)
			where TPoint : IGridPoint<TPoint>
		{
			if (ReferenceEquals(shape1, shape2))
			{
				return true;
			}

			return Contains(shape1, shape2) && Contains(shape2, shape1);
		}

		/// <summary>
		/// Transform each point in the list with the give point transformation.
		/// </summary>
		//TODO: Should this be an extension instead?
		public static IEnumerable<TPoint> TransformShape<TPoint>(
			IEnumerable<TPoint> shape,
			Func<TPoint, TPoint> pointTransformation)

			where TPoint : IGridPoint<TPoint>
		{
			return from point in shape
				   select pointTransformation(point);
		}

		public static bool IsEquivalentUnderTranslation<TPoint>(
			IEnumerable<TPoint> shape1,
			IEnumerable<TPoint> shape2,
			Func<IEnumerable<TPoint>, IEnumerable<TPoint>> toCanonicalPosition)

			where TPoint : IGridPoint<TPoint>
		{
			return IsEquivalent(
				toCanonicalPosition(shape1),
				toCanonicalPosition(shape2));
		}

		public static bool IsEquivalentUnderTransformsAndTranslation<TPoint>(
			IEnumerable<TPoint> shape1,
			IEnumerable<TPoint> shape2,
			IEnumerable<Func<TPoint, TPoint>> pointTransformations,
			Func<IEnumerable<TPoint>, IEnumerable<TPoint>> toCanonicalPosition)

			where TPoint : IGridPoint<TPoint>
		{
			var cannicalShape1 = toCanonicalPosition(shape1);
			var cannicalShape2 = toCanonicalPosition(shape2);

			if (IsEquivalent<TPoint>(cannicalShape1, cannicalShape2))
			{
				return true;
			}

			foreach (var pointTransformation in pointTransformations)
			{
				cannicalShape2 = toCanonicalPosition(TransformShape<TPoint>(shape2, pointTransformation));

				if (IsEquivalent<TPoint>(cannicalShape1, cannicalShape2))
				{
					return true;
				}
			}

			return false;
		}
		#endregion

		#region FilterType

		public static TResultGrid
			AggregateNeighborhood<TCell, TPoint, TResultGrid, TResultCell>(
				IGrid<TCell, TPoint> grid,
				Func<TPoint, IEnumerable<TPoint>, TResultCell> aggregator)

			where TPoint : IGridPoint<TPoint>
			where TResultGrid : IGrid<TResultCell, TPoint>
		{
			var newGrid = (TResultGrid)grid.CloneStructure<TResultCell>();

			foreach (var point in newGrid)
			{
				newGrid[point] = aggregator(point, newGrid.GetNeighbors(point));
			}

			return newGrid;
		}

		public static void
			AggregateNeighborhood<TCell, TPoint>(
				IGrid<TCell, TPoint> grid,
				Func<TPoint, IEnumerable<TPoint>, TCell> aggregator)

			where TPoint : IGridPoint<TPoint>
		{
			var newGrid = grid.CloneStructure<TCell>();

			foreach (var point in newGrid)
			{
				newGrid[point] = aggregator(point, grid.GetNeighbors(point));
			}

			foreach (var point in grid)
			{
				grid[point] = newGrid[point];
			}
		}

		#endregion

		#region Helpers
		private static TPoint FindNodeWithLowestScore<TPoint>(IList<TPoint> list, IDictionary<TPoint, float> scoreTable)
		{
			var minItem = list[0];
			var minScore = scoreTable[minItem];

			for(int i = 1; i < list.Count; i++)
			{
				var item = list[i];
				var score = scoreTable[item];

				if (score < minScore)
				{
					minScore = score;
					minItem = item;
				}
			}
			
			return minItem;
		}

		private static TPoint FindNodeWithLowestScore<TPoint>(IEnumerable<TPoint> list, IDictionary<TPoint, float> scoreTable)
		{
			return list.MinBy(x=>scoreTable[x]);
		}

		private static TPoint FindNodeWithLowestScore<TPoint>(IList<TPoint> list, IDictionary<TPoint, int> scoreTable)
		{
			var minItem = list[0];
			var minScore = scoreTable[minItem];

			for (int i = 1; i < list.Count; i++)
			{
				var item = list[i];
				var score = scoreTable[item];

				if (score < minScore)
				{
					minScore = score;
					minItem = item;
				}
			}

			return minItem;
		}

		//TODO remove construcctions of list!
		private static IList<TPoint> ReconstructPath<TPoint>(
			Dictionary<TPoint, TPoint> cameFrom,
			TPoint currentNode)

			where TPoint : IGridPoint<TPoint>
		{
			IList<TPoint> path = new PointList<TPoint>();

			ReconstructPath(cameFrom, currentNode, path);

			return path;
		}

		private static void ReconstructPath<TPoint>(Dictionary<TPoint, TPoint> cameFrom, TPoint currentNode, IList<TPoint> path)

			where TPoint : IGridPoint<TPoint>
		{
			if (cameFrom.ContainsKey(currentNode))
			{
				ReconstructPath(cameFrom, cameFrom[currentNode], path);
			}

			path.Add(currentNode);
		}
		#endregion
	}

	
}