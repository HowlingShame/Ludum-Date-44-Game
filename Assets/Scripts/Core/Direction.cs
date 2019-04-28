using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Direction : int
{
	None	= 0, 
	Left	= 1,
	Right	= 2,
	Top		= 4,
	Bottom	= 8,
	
	Cross2D	= Left | Right | Top | Bottom,
	Cross3D	= Left | Right | Top | Bottom | Up | Down,

	Any =	(Left | Right | Top | Bottom) | 
			(Up | Down) | 
			(CLeftTop | CRightTop | CRightBottom | CLeftBottom),

	Up		= 16,
	Down	= 32,

	Horizontal	= Left | Right,
	Vertical	= Top | Bottom,
	
	CLeftTop		= 64,
	CRightTop		= 128,
	CRightBottom	= 256,
	CLeftBottom		= 1024,

	Custom			= 2048,
	
	UpLeft	= 4096,
	UpRight	= 8192,
	UpTop		= 16384,
	UpBottom	= 32768,

	UpCLeftTop			= 65536,
	UpCRightTop			= 131072,
	UpCRightBottom		= 262144,
	UpCLeftBottom		= 524288,
	
	DownLeft	= 1048576,
	DownRight	= 2097152,
	DownTop		= 4194304,
	DownBottom	= 8388608,

	DownCLeftTop		= 16777216,
	DownCRightTop		= 33554432,
	DownCRightBottom	= 67108864,
	DownCLeftBottom		= 134217728,
}

public class DirectionHelper
{
	static public Direction DetermineDirection4Dim(float angleDeg)
	{
		foreach(var n in g_DirectionRange4Dim)
			if(n.InRange(angleDeg))
				return n.m_Direction;

		return Direction.None;
	}
	static public Direction DetermineDirection8Dim(float angleDeg)
	{
		foreach(var n in g_DirectionRange8Dim)
			if(n.InRange(angleDeg))
				return n.m_Direction;

		return Direction.None;
	}
//	static public Direction DetermibeDirection(Vector2 orientationNormal)
//	{
//	}
	static public int DirectionCount4Dim(Direction direction)
	{
		int result = 0;
		foreach(var n in g_DirectionPure4Dim)
			if(direction.HasFlag(n))
				result ++;

		return result;
	}

	static public bool IsHorizontal(Direction direction)
	{
		return Direction.Horizontal.HasFlag(direction);
	}
	static public bool IsVertical(Direction direction)
	{
		return Direction.Vertical.HasFlag(direction);
	}

	static public readonly SortedDictionary<int, Direction>			g_AngleToDirection = new SortedDictionary<int, Direction>()
	{
		{ 180, Direction.Left},
		{ 0, Direction.Right},
		{ 90, Direction.Top},
		{ 270, Direction.Bottom},
		
		{ 135, Direction.CLeftTop },
		{ 45, Direction.CRightTop },
		{ 315, Direction.CRightBottom },
		{ 225, Direction.CLeftBottom },


		{ -180, Direction.Left},
		{ -270, Direction.Top},
		{ -90, Direction.Bottom},

		{ -225, Direction.CLeftTop },
		{ -315, Direction.CRightTop },
		{ -45, Direction.CRightBottom },
		{ -135, Direction.CLeftBottom }
	};

	static public readonly SortedDictionary<Direction, Direction>	g_OppositeDirection = new SortedDictionary<Direction, Direction>()
	{
		{ Direction.None, Direction.None },

		{ Direction.Left, Direction.Right },
		{ Direction.Right, Direction.Left },
		{ Direction.Top, Direction.Bottom },
		{ Direction.Bottom, Direction.Top },

		{ Direction.Up, Direction.Down },
		{ Direction.Down, Direction.Up },

		{ Direction.Horizontal, Direction.Vertical },
		{ Direction.Vertical, Direction.Horizontal },

		{ Direction.CLeftTop, Direction.CRightBottom },
		{ Direction.CRightTop, Direction.CLeftBottom },
		{ Direction.CRightBottom, Direction.CLeftTop },
		{ Direction.CLeftBottom, Direction.CRightTop }
	};
	
	static public readonly SortedDictionary<Direction, Vector3>		g_DirectionVector = new SortedDictionary<Direction, Vector3>()
	{
		{ Direction.None, new Vector3(0.0f, 0.0f, 0.0f) },

		{ Direction.Left,  new Vector3(-1.0f, 0.0f, 0.0f) },
		{ Direction.Right,  new Vector3(1.0f, 0.0f, 0.0f) },
		{ Direction.Top,  new Vector3(0.0f, 1.0f, 0.0f) },
		{ Direction.Bottom, new Vector3(0.0f, -1.0f, 0.0f) },

		{ Direction.Up, new Vector3(0.0f, 1.0f, 0.0f) },
		{ Direction.Down, new Vector3(0.0f, -1.0f, 0.0f) },
		
		{ Direction.CLeftTop, new Vector3(-0.707106769084930419921875f, 0.707106769084930419921875f, 0.0f) },
		{ Direction.CRightTop, new Vector3(0.707106769084930419921875f, 0.707106769084930419921875f, 0.0f) },
		{ Direction.CRightBottom, new Vector3(0.707106769084930419921875f, -0.707106769084930419921875f, 0.0f) },
		{ Direction.CLeftBottom, new Vector3(-0.707106769084930419921875f, -0.707106769084930419921875f, 0.0f) }
	};
	
	static public readonly SortedDictionary<Direction, float>		g_RotationDegrees = new SortedDictionary<Direction, float>()
	{
		{ Direction.None, 0.0f },

		{ Direction.Left, 180.0f },
		{ Direction.Right, 0.0f },
		{ Direction.Top, 90.0f },
		{ Direction.Bottom, -90.0f },

		{ Direction.CLeftTop, 135.0f },
		{ Direction.CRightTop, 45.0f },
		{ Direction.CRightBottom, -45.0f },
		{ Direction.CLeftBottom, -135.0f }
	};
	
	static public readonly SortedDictionary<Direction, Direction>	g_LeftRotation = new SortedDictionary<Direction, Direction>()
	{
		{ Direction.None, Direction.None },

		{ Direction.Left, Direction.Right },
		{ Direction.Right, Direction.Left },
		{ Direction.Top, Direction.Bottom },
		{ Direction.Bottom, Direction.Top },

		{ Direction.Up, Direction.Up },
		{ Direction.Down, Direction.Down },

		{ Direction.Horizontal, Direction.Horizontal },
		{ Direction.Vertical, Direction.Vertical },

		{ Direction.CLeftTop, Direction.CRightBottom },
		{ Direction.CRightTop, Direction.CLeftBottom },
		{ Direction.CRightBottom, Direction.CLeftTop },
		{ Direction.CLeftBottom, Direction.CRightTop }
	};

	static public readonly SortedDictionary<Direction, Direction>	g_RightRotation = new SortedDictionary<Direction, Direction>()
	{
		{ Direction.None, Direction.None },

		{ Direction.Left, Direction.Left },
		{ Direction.Right, Direction.Right },
		{ Direction.Top, Direction.Top },
		{ Direction.Bottom, Direction.Bottom },

		{ Direction.Up, Direction.Up },
		{ Direction.Down, Direction.Down },

		{ Direction.Horizontal, Direction.Vertical },
		{ Direction.Vertical, Direction.Horizontal },

		{ Direction.CLeftTop, Direction.CLeftTop },
		{ Direction.CRightTop, Direction.CRightTop },
		{ Direction.CRightBottom, Direction.CRightBottom },
		{ Direction.CLeftBottom, Direction.CLeftBottom }
	};

	static public readonly SortedDictionary<Direction, Direction>	g_TopRotation = new SortedDictionary<Direction, Direction>()
	{
		{ Direction.None, Direction.None },

		{ Direction.Left, Direction.Bottom },
		{ Direction.Right, Direction.Top },
		{ Direction.Top, Direction.Left },
		{ Direction.Bottom, Direction.Right },

		{ Direction.Up, Direction.Up },
		{ Direction.Down, Direction.Down },

		{ Direction.Horizontal, Direction.Vertical },
		{ Direction.Vertical, Direction.Horizontal },

		{ Direction.CLeftTop, Direction.CLeftBottom },
		{ Direction.CRightTop, Direction.CLeftTop },
		{ Direction.CRightBottom, Direction.CRightTop },
		{ Direction.CLeftBottom, Direction.CRightBottom }
	};

	static public readonly SortedDictionary<Direction, Direction>	g_BottomRotation = new SortedDictionary<Direction, Direction>()
	{
		{ Direction.None, Direction.None },

		{ Direction.Left, Direction.Top },
		{ Direction.Right, Direction.Bottom },
		{ Direction.Top, Direction.Right },
		{ Direction.Bottom, Direction.Left },

		{ Direction.Up, Direction.Up },
		{ Direction.Down, Direction.Down },

		{ Direction.Horizontal, Direction.Vertical },
		{ Direction.Vertical, Direction.Horizontal },

		{ Direction.CLeftTop, Direction.CRightTop },
		{ Direction.CRightTop, Direction.CRightBottom },
		{ Direction.CRightBottom, Direction.CLeftBottom },
		{ Direction.CLeftBottom, Direction.CLeftTop }
	};

	static public readonly SortedDictionary<Direction, SortedDictionary<Direction, Direction>>	g_Rotations = new SortedDictionary<Direction, SortedDictionary<Direction, Direction>>()
	{
		{ Direction.None, g_RightRotation },

		{ Direction.Left, g_LeftRotation },
		{ Direction.Right, g_RightRotation },
		{ Direction.Top, g_TopRotation },
		{ Direction.Bottom, g_BottomRotation }
	};

	static public readonly SortedDictionary<Direction, Direction>	g_RotationClockwise = new SortedDictionary<Direction, Direction>()
	{
		{ Direction.Bottom, Direction.Left },
		{ Direction.Left, Direction.Top },
		{ Direction.Top, Direction.Right },
		{ Direction.Right, Direction.Bottom },
	};
	static public readonly SortedDictionary<Direction, Direction>	g_RotationCounterClockwise = new SortedDictionary<Direction, Direction>()
	{
		{ Direction.Bottom, Direction.Right },
		{ Direction.Right, Direction.Top },
		{ Direction.Top, Direction.Left },
		{ Direction.Left, Direction.Bottom },
	};
	
	static public readonly SortedDictionary<Direction, Vector3Int>	g_WallMapDirectonOffset = new SortedDictionary<Direction, Vector3Int>()
	{
		{ Direction.Left, new Vector3Int(0, 0, 0) },
		{ Direction.Right, new Vector3Int(1, 0, 0) },
		{ Direction.Bottom, new Vector3Int(0, 0, 0) },
		{ Direction.Top, new Vector3Int(0, 1, 0) },
	};
	
	public struct AngleDirection
	{
		public float		m_Max;
		public float		m_Min;
		public Direction	m_Direction;

		public bool InRange(float angle)
		{
			return angle >= m_Min && angle < m_Max;
		}
	}

	static public readonly List<Direction>	g_DirectionPure4Dim = new List<Direction>()
	{
		Direction.Left,
		Direction.Right,
		Direction.Top,
		Direction.Bottom
	};

	static public readonly List<AngleDirection>	g_DirectionRange4Dim = new List<AngleDirection>()
	{
		new AngleDirection(){ m_Min = 45.0f, m_Max = 135.0f, m_Direction = Direction.Top},
		new AngleDirection(){ m_Min = -135.0f, m_Max = -45.0f, m_Direction = Direction.Bottom},

		new AngleDirection(){ m_Min = 0.0f, m_Max = 45.0f, m_Direction = Direction.Right},
		new AngleDirection(){ m_Min = -45.0f, m_Max = 0.0f, m_Direction = Direction.Right},

		new AngleDirection(){ m_Min = 135.0f, m_Max = 180.0f, m_Direction = Direction.Left},
		new AngleDirection(){ m_Min = -180.0f, m_Max = -135.0f, m_Direction = Direction.Left},
	};
	
	static public readonly List<AngleDirection>	g_DirectionRange8Dim = new List<AngleDirection>()
	{
		new AngleDirection(){ m_Min = 67.5f, m_Max = 112.5f, m_Direction = Direction.Top},
		new AngleDirection(){ m_Min = -112.5f, m_Max = -67.5f, m_Direction = Direction.Bottom},
		
		new AngleDirection(){ m_Min = 22.5f, m_Max = 67.5f, m_Direction = Direction.CRightTop},
		new AngleDirection(){ m_Min = -67.5f, m_Max = -22.5f, m_Direction = Direction.CRightBottom},

		new AngleDirection(){ m_Min = 112.5f, m_Max = 157.5f, m_Direction = Direction.CLeftTop},
		new AngleDirection(){ m_Min = -157.5f, m_Max = -112.5f, m_Direction = Direction.CLeftBottom},

		new AngleDirection(){ m_Min = 0.0f, m_Max = 22.5f, m_Direction = Direction.Right},
		new AngleDirection(){ m_Min = -22.5f, m_Max = 0.0f, m_Direction = Direction.Right},

		new AngleDirection(){ m_Min = 157.5f, m_Max = 180.0f, m_Direction = Direction.Left},
		new AngleDirection(){ m_Min = -180.0f, m_Max = -157.5f, m_Direction = Direction.Left}
	};

	
}

