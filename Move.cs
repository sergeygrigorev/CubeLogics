using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubeLogics
{
	public enum FaceId
	{
		R,
		L,
		U,
		D,
		F,
		B
	};

	public enum TurnDir
	{
		CW,
		DOUBLE,
		CCW
	};

	class Move
	{
		private FaceId _face;
		private TurnDir _dir;

		public Move ()
		{
			_face = 0;
			_dir = 0;
		}

		public Move (FaceId f, TurnDir d)
		{
			_face = f;
			_dir = d;
		}

		public FaceId Face
		{
			get { return _face; }
			set { _face = value; }
		}

		public TurnDir Dir
		{
			get { return _dir; }
			set { _dir = value; }
		}

		public Move Invert ()
		{
			TurnDir x;

			if (_dir == TurnDir.CW)
				x = TurnDir.CCW;
			else if (_dir == TurnDir.CCW)
				x = TurnDir.CW;
			else x = TurnDir.DOUBLE;

			return new Move(_face, x);
		}

		public static Move operator ++ (Move a)
		{
			if (a.Dir == TurnDir.CCW)
			{
				a._dir = TurnDir.CW;
				if (a._face == FaceId.B)
					a._face = FaceId.R;
				else a._face++;
			}
			else a._dir++;

			return a;
		}

		public override string ToString()
		{
			string s = _face.ToString();
			if (_dir == TurnDir.DOUBLE)
				s += "2";
			if (_dir == TurnDir.CCW)
				s += "'";

			return s;
		}
	}
}
