namespace CubeLogics
{
	public enum EdgeId
	{
		UF,
		UL,
		UB,
		UR,
		DF,
		DL,
		DB,
		DR,
		FL,
		BL,
		BR,
		FR
	};

	class Edge
	{
		private EdgeId _pos;
		private int _ori;

		public Edge ()
		{
			_pos = 0;
			_ori = 0;
		}

		public Edge (EdgeId x)
		{
			_pos = x;
			_ori = 0;
		}

		public Edge (EdgeId x, int y)
		{
			_pos = x;
			_ori = y%2;
		}

		public void flip ()
		{
			_ori++;
			_ori %= 2;
		}

		public EdgeId GetPos
		{
			get { return _pos; }
		}

		public int GetOri
		{
			get { return _ori; }
		}

		public override string ToString()
		{
			return "(" + _pos.ToString() + ", " + _ori.ToString() + ")";
		}
	}
}