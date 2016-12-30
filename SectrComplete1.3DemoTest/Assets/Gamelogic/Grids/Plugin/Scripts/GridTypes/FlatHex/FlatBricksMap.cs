﻿//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//


using Gamelogic.Extensions;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Grids
{
	/// <summary>
	/// A map that can be used with a PointyHexGrid to get a brick-wall pattern 
	/// (rotated by 90 degrees). The cells are rectangular.
	/// </summary>
	[Version(1)]
	public class FlatBrickMap : AbstractMap<FlatHexPoint>
	{
		public FlatBrickMap(Vector2 cellDimensions) :
			base(cellDimensions)
		{ }

		public override Vector2 GridToWorld(FlatHexPoint point)
		{
			float sY = (point.X / 2.0f + point.Y) * cellDimensions.y;
			float sX = point.X * cellDimensions.x;

			return new Vector2(sX, sY);
		}

		public override FlatHexPoint RawWorldToGrid(Vector2 point)
		{
			int x = GLMathf.FloorToInt((point.x + cellDimensions.x / 2) / cellDimensions.x);
			int y = GLMathf.FloorToInt((point.y - x * cellDimensions.y / 2 + cellDimensions.y / 2) / cellDimensions.y);

			return new FlatHexPoint(x, y);
		}
	}
}