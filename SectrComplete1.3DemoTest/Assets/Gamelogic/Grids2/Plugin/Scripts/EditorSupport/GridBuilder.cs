﻿using System;
using System.Collections.Generic;
using Gamelogic.Extensions;
using Gamelogic.Extensions.Algorithms;
using Gamelogic.Extensions.Internal;
using Gamelogic.Grids2.Graph;
using UnityEngine;

namespace Gamelogic.Grids2
{

	/// <summary>
	/// Types of rounding.
	/// </summary>
	public enum RoundType
	{
		Point,
		Rect,
		Hex,
		Block,

		[Version(2, 2)]
		HexBlock
	}


	/// <summary>
	/// Base class for grid builders. A grid builder builds a visual presentation of a grid.
	/// </summary>
	/// <typeparam name="TCell">The type of the t cell.</typeparam>
	/// <seealso cref="GLMonoBehaviour" />
	[ExecuteInEditMode]
	public abstract class GridBuilder<TCell> : GLMonoBehaviour
	{
		private abstract class GridHolder
		{
			public abstract void SetMap<TPoint>(GridMap<TPoint> map);
			public abstract void SetGrid<TPoint>(IGrid<TPoint, TCell> grid);

			public abstract IGrid<TPoint, TCell> GetGrid<TPoint>();
			public abstract GridMap<TPoint> GetMap<TPoint>();
		}

		private sealed class GridHolder<TPoint> : GridHolder
		{
			private IGrid<TPoint, TCell> grid;
			private GridMap<TPoint> map;

			public override void SetGrid<TPointDesired>(IGrid<TPointDesired, TCell> newGrid)
			{
				this.grid = (IGrid<TPoint, TCell>)newGrid;
			}

			public override IGrid<TPointDesired, TCell> GetGrid<TPointDesired>()
			{
				return (IGrid<TPointDesired, TCell>)grid;
			}

			public override GridMap<TPointDesired> GetMap<TPointDesired>()
			{
				return (GridMap<TPointDesired>)(object)map;
			}

			public override void SetMap<TPoint1>(GridMap<TPoint1> newMap)
			{
				this.map = (GridMap<TPoint>)(object)newMap;
			}
		}

		public GridShapeGraph gridShapeGraph;
		public SpaceMapGraph spaceMapGraph;
		public RoundType roundType;

		[SerializeField, HideInInspector]
		public TCell[] cellStorage;

		private GridHolder gridHolder;

		private void InitGridBehaviours()
		{
			var gridBehaviours = GetGridBehaviours();

			foreach (var gridBehaviour in gridBehaviours)
			{
				switch (gridShapeGraph.gridType)
				{
					case GridType.Grid1:
						gridBehaviour.__InitPrivateFields(
							this,
							gridHolder.GetGrid<int>(),
							gridHolder.GetMap<int>());
						break;

					case GridType.Grid2:
						gridBehaviour.__InitPrivateFields(
							this,
							gridHolder.GetGrid<GridPoint2>(),
							gridHolder.GetMap<GridPoint2>());
						break;

					case GridType.Grid3:
						gridBehaviour.__InitPrivateFields(
							this,
							gridHolder.GetGrid<GridPoint3>(),
							gridHolder.GetMap<GridPoint3>());
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				gridBehaviour.InitGrid();
			}
		}

		private IEnumerable<GridBehaviour<TCell>> GetGridBehaviours()
		{
			switch (gridShapeGraph.gridType)
			{
				case GridType.Grid1:
					return GetComponents<GridBehaviour<int, TCell>>();

				case GridType.Grid2:
					return GetComponents<GridBehaviour<GridPoint2, TCell>>();

				case GridType.Grid3:
					return GetComponents<GridBehaviour<GridPoint3, TCell>>();

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public virtual void MakeCells<TPoint>(
			IGrid<TPoint, TCell> grid,
			GridMap<TPoint> map)
		{
			throw new NotImplementedByException(GetType());
		}

		public virtual void RelinkCells<TPoint>(
			IGrid<TPoint, TCell> grid)
		{
			var pointIndex = 0;
			foreach (var point in grid.Points)
			{
				grid[point] = cellStorage[pointIndex];

				pointIndex++;
			}
		}

		public void Build()
		{
			switch (gridShapeGraph.gridType)
			{
				case GridType.Grid1:
					BuildGrid1();
					break;
				
				case GridType.Grid2:
					BuildGrid2();
					break;
				
				case GridType.Grid3:
					BuildGrid3();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			InitGridBehaviours();
		}

		private void BuildGrid3()
		{
			gridHolder = new GridHolder<GridPoint3>();

			var shape = gridShapeGraph.shape3Graph.GetShape();
			gridHolder.SetGrid(new Grid3<TCell>(shape));

			var spaceMap = spaceMapGraph.GetMap();
			var roundMap = MakeRoundMap<GridPoint3>();
			var gridMap = new GridMap<GridPoint3>(spaceMap, roundMap);

			gridHolder.SetMap(gridMap);

			if (cellStorage != null)
			{
				if (!cellStorage.IsEmpty())
				{
					RelinkCells(gridHolder.GetGrid<GridPoint3>());
					return;
				}
			}

			MakeCells(gridHolder.GetGrid<GridPoint3>(), gridHolder.GetMap<GridPoint3>());
		}

		private void BuildGrid2()
		{
			gridHolder = new GridHolder<GridPoint2>();

			var shape = gridShapeGraph.shape2Graph.GetShape();
			gridHolder.SetGrid(new Grid2<TCell>(shape));

			var spaceMap = spaceMapGraph.GetMap();
			var roundMap = MakeRoundMap<GridPoint2>();
			var gridMap = new GridMap<GridPoint2>(spaceMap, roundMap);
			gridHolder.SetMap(gridMap);

			if (cellStorage != null)
			{
				if (!cellStorage.IsEmpty())
				{
					RelinkCells(gridHolder.GetGrid<GridPoint2>());
					return;
				}
			}

			MakeCells(gridHolder.GetGrid<GridPoint2>(), gridHolder.GetMap<GridPoint2>());
		}

		private void BuildGrid1()
		{
			gridHolder = new GridHolder<int>();

			var shape = gridShapeGraph.shape1Graph.GetShape();
			gridHolder.SetGrid(new Grid1<TCell>(shape));

			var spaceMap = spaceMapGraph.GetMap();
			var roundMap = MakeRoundMap<int>();
			var gridMap = new GridMap<int>(spaceMap, roundMap);
			gridHolder.SetMap(gridMap);

			if (cellStorage != null)
			{
				if (!cellStorage.IsEmpty())
				{
					RelinkCells(gridHolder.GetGrid<int>());
					return;
				}
			}

			MakeCells(gridHolder.GetGrid<int>(), gridHolder.GetMap<int>());
		}

		private IMap<Vector3, TPoint> MakeRoundMap<TPoint>()
		{
			switch (roundType)
			{
				case RoundType.Point:
					return (IMap<Vector3, TPoint>)Map.PointRound();

				case RoundType.Rect:
					return (IMap<Vector3, TPoint>)Map.RectRound();

				case RoundType.Hex:
					return (IMap<Vector3, TPoint>)Map.HexRound();

				case RoundType.Block:
					return (IMap<Vector3, TPoint>)Map.BlockRound();

				case RoundType.HexBlock:
					return (IMap<Vector3, TPoint>)Map.HexBlockRound();

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected virtual void OnEnable()
		{
			if (!Application.isPlaying)
				return;

			if (gridHolder == null)
			{

				Build();
			}
		}
	}
}