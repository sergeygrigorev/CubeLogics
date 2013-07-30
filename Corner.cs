namespace CubeLogics
{
	public enum CornerId
	{
		URF,
		ULF,
		ULB,
		URB,
		DRF,
		DLF,
		DLB,
		DRB
	};

	class Corner
	{
		private CornerId _pos;
		private int _ori;

		public Corner ()
		{
			_pos = 0;
			_ori = 0;
		}

		public Corner (CornerId x)
		{
			_pos = x;
			_ori = 0;
		}

		public Corner (CornerId x, int y)
		{
			_pos = x;
			_ori = y%3;
			if (_ori < 0)
				_ori += 3;
		}

		public void cw ()
		{
			_ori++;
			_ori %= 3;
			if (_ori < 0)
				_ori += 3;
		}

		public void ccw ()
		{
			_ori--;
			_ori %= 3;
			if (_ori < 0)
				_ori += 3;
		}

		public CornerId GetPos
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