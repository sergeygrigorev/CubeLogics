using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubeLogics
{
	class Algo : ICloneable
	{
		const string AllowedSymbols = "RLUDFB 2'";
		private Move[] _moves;

		public Algo ()
		{

		}

		public Algo (int n)
		{
			_moves = new Move[n];
		}

		public Algo (string s)
		{
			string[] ss;
			int i, m, n;

			s = s.Trim();
			s = s.ToUpper();
			if (!checkString(s,AllowedSymbols)) throw new Exception("Invalid algorithm string"); ;
			ss = s.Split(new char[1]{' '},StringSplitOptions.RemoveEmptyEntries);
			_moves = new Move[ss.Length];
			for (i=0;i<ss.Length;i++)
			{
				if (ss[i].Length > 2)
					throw new Exception("Invalid algorithm string");
				if (!checkString(ss[i].Substring(0, 1), AllowedSymbols.Substring(0, 6)))
					throw new Exception("Invalid algorithm string");
				if (ss[i].Length == 2)
				{
					if (!checkString(ss[i].Substring(1, 1), AllowedSymbols.Substring(7, 2)))
						throw new Exception("Invalid algorithm string");
					n = (ss[i][1] == '2') ? 1 : 2;
				}
				else n = 0;
				
				m = AllowedSymbols.IndexOf(ss[i][0]);
				_moves[i] = new Move((FaceId) m, (TurnDir) n);
			}
		}

		static bool checkString (string a, string b)
		{
			int i, j;

			for (i = 0; i < a.Length; i++)
			{
				for (j = 0; j < b.Length; j++)
					if (a[i] == b[j])
						break;
				if (j == b.Length)
					return false;
			}

			return true;
		}

		public override string ToString()
		{
			string s = string.Empty;

			foreach (Move m in _moves)
				s += m.ToString() + " ";
			s = s.Trim();

			return s;
		}

		public Move this[int index]
		{
			get { return _moves[index]; }
			set { _moves[index] = value; }
		}

		public static Algo operator + (Algo a, Algo b)
		{
			Algo c;
			Move m = null;
			int i, j, k;

			for (i=a.Length-1,j=0;i>=0&&j<b.Length;)
			{
				if (a[i].Face != b[j].Face)
					break;
				if ((int)(a[i].Dir) + (int)(b[j].Dir) == 2)
				{
					i--;
					j++;
					continue;
				}
				if (a[i].Dir == b[j].Dir)
					m = new Move(a[i].Face,TurnDir.DOUBLE);
				else if ((int)(a[i].Dir) + (int)(b[j].Dir) == 1)
					m = new Move(a[i].Face, TurnDir.CCW);
				else m = new Move(a[i].Face, TurnDir.CW);
				i--;
				j++;
				break;
			}
			c = new Algo(i+1+b.Length-j+((m == null)?(0):(1)));
			for (k = 0; k <= i; k++)
				c[k] = new Move(a[k].Face, a[k].Dir);
			if (m != null)
			{
				c[k] = m;
				k++;
			}
			for (; j < b.Length; k++, j++)
				c[k] = new Move(b[j].Face, b[j].Dir);

			return c;
		}

		public static Algo operator ++ (Algo a)
		{
			int i;

			if (a.Length == 0)
			{
				a._moves = new Move[1];
				a._moves[0] = new Move(FaceId.R, TurnDir.CW);
				return a;
			}
			for (i=a.Length-1;i>0;i--)
			{
				if (a[i].Face == FaceId.B && a[i].Dir == TurnDir.CCW)
					continue;
				a[i]++;
				if (a[i].Face == a[i - 1].Face && a[i].Face != FaceId.B)
				{
					a[i]++;
					a[i]++;
					a[i]++;
				}
				else if (a[i].Face == a[i - 1].Face) continue;
				break;
			}
			if (a.Length == 1)
			{
				if (a[0].Face == FaceId.B && a[0].Dir == TurnDir.CCW)
				{
					a._moves = new Move[a.Length + 1];
					a.fillRL(0);
					return a;
				}
				a[0]++;
			}
			if (i == a.Length-1)
				return a;
			if (i == 0)
			{
				a[0]++;
				if (a[0].Face == FaceId.R && a[0].Dir == TurnDir.CW)
				{
					a._moves = new Move[a.Length + 1];
					a.fillRL(0);
					return a;
				}
			}
			if (a[i].Face == FaceId.R)
			{
				a[i + 1].Face = FaceId.L;
				a[i + 1].Dir = TurnDir.CW;
				a.fillRL(i + 2);
			}
			else a.fillRL(i + 1);
			
			return a;
		}

		void fillRL (int n)
		{
			int i;

			for (i=n;i<Length;i++)
				_moves[i] = new Move((FaceId) ((i-n)%2), TurnDir.CW);
		}

		public int Length
		{
			get { return (_moves == null)?0:(_moves.Length); }
		}

		public Algo Invert ()
		{
			int i;
			Algo a = new Algo(Length);

			for (i = 0; i < Length; i++)
				a._moves[i] = _moves[_moves.Length - i - 1].Invert();

			return a;
		}

		public object Clone()
		{
			Algo a = new Algo(Length);
			int i;

			for (i=0;i<Length;i++)
				a._moves[i] = new Move(_moves[i].Face, _moves[i].Dir);

			return a;
		}
	}
}
