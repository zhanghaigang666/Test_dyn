﻿//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

// Auto-generated File

using System;

namespace Gamelogic.Grids
{

	/// <summary>
	/// A wrapper that wraps tri points over a hexagon, similar to the 
	///	way hex points are wrapped in this example:
	///
	///	http://www.redblobgames.com/grids/hexagons/
	///	
	///	Since version 1.7
	/// </summary>
	public class PointyTriHexagonWrapper : IPointWrapper<PointyTriPoint>
	{
		private readonly PointyTriPoint[] wrappedPoints;
		private readonly Func<PointyTriPoint, int> colorFunc;

		/// <param name=side>Must be positive</param>
		public PointyTriHexagonWrapper(int side)
		{
			if (side <= 0)
			{
				throw new Exception("n Must be a positive integer.");
			}

			int colorCount = 3 * side * side;
			colorFunc = x => x.GetColor(colorCount/side, side, side);

			wrappedPoints = new PointyTriPoint[colorCount * 2];
			var grid = PointyTriGrid<int>.Hexagon(side);

			foreach (var point in grid)
			{
				int color = colorFunc(point);
				wrappedPoints[color] = point;
			}
		}

		public PointyTriPoint Wrap(PointyTriPoint point)
		{
			return wrappedPoints[colorFunc(point)];
		}
	}


	/// <summary>
	/// A wrapper that wraps tri points over a hexagon, similar to the 
	///	way hex points are wrapped in this example:
	///
	///	http://www.redblobgames.com/grids/hexagons/
	///	
	///	Since version 1.7
	/// </summary>
	public class FlatTriHexagonWrapper : IPointWrapper<FlatTriPoint>
	{
		private readonly FlatTriPoint[] wrappedPoints;
		private readonly Func<FlatTriPoint, int> colorFunc;

		/// <param name=side>Must be positive</param>
		public FlatTriHexagonWrapper(int side)
		{
			if (side <= 0)
			{
				throw new Exception("n Must be a positive integer.");
			}

			int colorCount = 3 * side * side;
			colorFunc = x => x.GetColor(colorCount/side, side, side);

			wrappedPoints = new FlatTriPoint[colorCount * 2];
			var grid = FlatTriGrid<int>.Hexagon(side);

			foreach (var point in grid)
			{
				int color = colorFunc(point);
				wrappedPoints[color] = point;
			}
		}

		public FlatTriPoint Wrap(FlatTriPoint point)
		{
			return wrappedPoints[colorFunc(point)];
		}
	}

}
