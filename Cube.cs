using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubeLogics
{
	class Cube : ICloneable
	{
		private Edge[] edges = new Edge[12];
		private Corner[] corners = new Corner[8];

		public Cube ()
		{
			int i;

			for (i=0;i<12;i++)
				edges[i] = new Edge((EdgeId)i);
			for (i=0;i<8;i++)
				corners[i] = new Corner((CornerId)i);
		}

		public Cube (string ep, string eo, string cp, string co) : this ()
		{
			string[] s1, s2;
			int i;

			s1 = ep.Split(' ');
			s2 = eo.Split(' ');
			for (i = 0; i < 12; i++)
				edges[i] = new Edge((EdgeId)Int32.Parse(s1[i]),Int32.Parse(s2[i]));
			s1 = cp.Split(' ');
			s2 = co.Split(' ');
			for (i = 0; i < 8; i++)
				corners[i] = new Corner((CornerId)Int32.Parse(s1[i]), Int32.Parse(s2[i]));
		}

		public Cube (Algo a) : this ()
		{
			Rotate(a);
		}

		public Cube (string s) : this (new Algo(s))
		{
		}

		public object this[int i]
		{
			get
			{
				if (i < 0 || i > 19)
					throw new Exception("Cube index out of range");
				if (i < 12)
					return edges[i];
				else return corners[i - 12];
			}
		}

		public override string ToString()
		{
			string s = string.Empty;
			int i;

			for (i = 0; i < 12; i++)
				if ((EdgeId)i != edges[i].GetPos || edges[i].GetOri != 0)
					s += ((EdgeId)i).ToString() + " = " + edges[i].ToString() + "\n";
			for (i = 0; i < 8; i++)
				if ((CornerId)i != corners[i].GetPos || corners[i].GetOri != 0)
					s += ((CornerId)i).ToString() + " = " + corners[i].ToString() + "\n";
			if (s == string.Empty)
				s = "Solved cube\n";

			return s;
		}

		public object Clone ()
		{
			Cube a = new Cube();
			int i;

			for (i = 0; i < 8; i++)
				a.corners[i] = new Corner(this.corners[i].GetPos, this.corners[i].GetOri);
			for (i = 0; i < 12; i++)
				a.edges[i] = new Edge(this.edges[i].GetPos, this.edges[i].GetOri);

			return a;
		}

		public bool IsSolved ()
		{
			int i;

			for (i = 0; i < 12; i++)
				if ((EdgeId)i != edges[i].GetPos || edges[i].GetOri != 0)
					return false;
			for (i = 0; i < 8; i++)
				if ((CornerId)i != corners[i].GetPos || corners[i].GetOri != 0)
					return false;

			return true;
		}

		public int Flip
		{
			get
			{
				int i, r;

				r = 0;
				for (i = 0; i < 11; i++)
					r = r*2 + (int)edges[i].GetOri;

				return r;
			}
			set
			{
				int i, j, p;

				p = 0;
				for (i=0;i<11;i++)
				{
					j = value%2;
					value /= 2;
					p += j;
					if (j != edges[10-i].GetOri)
						edges[10-i].flip();
				}
				p %= 2;
				if (p != edges[11].GetOri)
					edges[11].flip();
			}
		}

		public int Twist
		{
			get 
			{ 
				int i, r;

				r = 0;
				for (i = 0; i < 7; i++)
					r = r * 3 + (int)corners[i].GetOri;

				return r;
			}
			set 
			{ 
				int i, j, p;

				p = 0;
				for (i=6;i>=0;i--)
				{
					j = value%3;
					value /= 3;
					p += j;
					if ((j+3-corners[i].GetOri)%3 == 1)
						corners[i].cw();
					else if ((j+3-corners[i].GetOri)%3 == 2)
						corners[i].ccw();
				}
				p %= 3;
				p = (3 - p)%3;
				if ((p + 3 - corners[7].GetOri) % 3 == 1)
					corners[7].cw();
				else if ((p + 3 - corners[7].GetOri) % 3 == 2)
					corners[7].ccw();
			}
		}

		public int Slice
		{
			get
			{
				int i, j, n, p, r;
				Edge[] a = new Edge[4];

				r = 0;
				j = -1;
				for (i = 0; i < 12; i++)
				{
					if (edges[i].GetPos > EdgeId.DR)
					{
						j++;
						a[j] = edges[i];
					}
					else r += binom(i, j);
				}
				p = 0;
				for (i = 1; i < 4; i++)
				{
					n = 0;
					for (j = 0; j < i; j++)
						if (a[j].GetPos > a[i].GetPos)
							n++;
					p += n * fact(i);
				}

				return r*24 + p;
			}
			set
			{
				int i, j, n, p, r;
				bool[] taken = new bool[4];
				Edge[] a = new Edge[4];

				r = value/24;
				p = value%24;

				for (i = 0; i < 4; i++)
					taken[i] = false;
				for (i = 3; i >= 0; i--)
				{
					n = p / fact(i);
					p %= fact(i);
					for (j = 3; j >= 0; j--)
						if (!taken[j])
						{
							if (n == 0) break;
							n--;
						}
					a[i] = new Edge((EdgeId) (j + 8));
					taken[j] = true;
				}

				for (i = 11, j = 3, n = 0; i >= 0; i--)
				{
					if (binom(i, j) > r)
					{
						edges[i] = a[j];
						j--;
					}
					else
					{
						n++;
						edges[i] = new Edge((EdgeId) (8 - n));
						r -= binom(i, j);
					}
				}

				/*if (verify() == 1)
				{
					Corner c = corners[(int)CornerId.URF];
					corners[(int)CornerId.URF] = corners[(int)CornerId.ULF];
					corners[(int)CornerId.ULF] = c;
				}*/
			}
		}

		public int CornerPerm
		{
			get
			{
				int i, j, r, n;

				r = 0;
				for (i = 1; i < 8; i++)
				{
					n = 0;
					for (j = 0; j < i; j++)
						if (corners[j].GetPos > corners[i].GetPos)
							n++;
					r += n * fact(i);
				}

				return r;
			}

			set
			{
				int i, j, r, n;
				bool[] taken = new bool[8];

				for (i = 0; i < 8; i++)
					taken[i] = false;
				r = value;
				for (i = 7; i >= 0; i--)
				{
					n = r / fact(i);
					r %= fact(i);
					for (j = 7; j >= 0; j--)
						if (!taken[j])
						{
							if (n == 0) break;
							n--;
						}
					corners[i] = new Corner((CornerId)j, corners[i].GetOri);
					taken[j] = true;
				}
			}
		}

		public int Edge8Perm
		{
			get
			{
				int i, j, r, n;

				r = 0;
				for (i = 1; i < 8; i++)
				{
					n = 0;
					for (j = 0; j < i; j++)
						if (edges[j].GetPos > edges[i].GetPos)
							n++;
					r += n*fact(i);
				}

				return r;
			}

			set
			{
				int i, j, r, n;
				bool[] taken = new bool[8];

				for (i=0;i<8;i++)
					taken[i] = false;
				r = value;
				for (i=7;i>=0;i--)
				{
					n = r/fact(i);
					r %= fact(i);
					for (j = 7; j >=0; j--)
						if (!taken[j])
						{
							if (n == 0) break;
							n--;
						}
					edges[i] = new Edge((EdgeId)j,edges[i].GetOri);
					taken[j] = true;
				}
				for (i=8;i<12;i++)
					edges[i] = new Edge((EdgeId)i,edges[i].GetOri);
			}
		}

		public int EdgePerm
		{
			get
			{
				int i, j, r, n;

				r = 0;
				for (i = 1; i < 12; i++)
				{
					n = 0;
					for (j = 0; j < i; j++)
						if (edges[j].GetPos > edges[i].GetPos)
							n++;
					r += n * fact(i);
				}

				return r;
			}

			set
			{
				int i, j, r, n;
				bool[] taken = new bool[12];

				for (i = 0; i < 12; i++)
					taken[i] = false;
				r = value;
				for (i = 11; i >= 0; i--)
				{
					n = r / fact(i);
					r %= fact(i);
					for (j = 11; j >= 0; j--)
						if (!taken[j])
						{
							if (n == 0) break;
							n--;
						}
					edges[i] = new Edge((EdgeId)j, edges[i].GetOri);
					taken[j] = true;
				}
			}
		}

		void rightMove (Edge[] a, int m, int n)
		{
			Edge b;
			int i;

			m %= a.Length;
			n %= a.Length;
			if (m == n)
				return;
			b = a[n];
			if (m > n)
				n += a.Length;
			for (i = n; i > m; i--)
				a[i%a.Length] = a[(i - 1)%a.Length];
			a[m] = b;
		}

		int binom (int n, int k)
		{
			if (k < 0 || k > n)
				return 0;
			return fact(n)/fact(k)/fact(n - k);
		}

		int fact (int n)
		{
			int i = 1;

			while (n > 1)
				i *= n--;

			return i;
		}

		int verify ()
		{
			/* 0 - all OK
			 * 1 - Perm parity
			 * 2 - Edge ori parity
			 * 3 - Corner ori parity cw
			 * 4 - Corner ori parity ccw
			 */

			int i, p, j;

			p = 0;
			for (i = 0; i < 12; i++)
				for (j = 0; j < i; j++)
					if (edges[j].GetPos > edges[i].GetPos)
						p++;
			for (i = 0; i < 8; i++)
				for (j = 0; j < i; j++)
					if (corners[j].GetPos > corners[i].GetPos)
						p++;

			if (p % 2 == 1)
				return 1;

			return 0;
		}

		public void Rotate (Algo a)
		{
			for (int i=0;i<a.Length;i++)
				Rotate(a[i]);
		}

		public void Rotate (Move a)
		{
			switch (a.Face)
			{
				case FaceId.R:
					R(a.Dir);break;
				case FaceId.L:
					L(a.Dir); break;
				case FaceId.U:
					U(a.Dir); break;
				case FaceId.D:
					D(a.Dir); break;
				case FaceId.F:
					F(a.Dir); break;
				case FaceId.B:
					B(a.Dir); break;
			}
		}

		public void R (TurnDir x)
		{
			Edge a;
			Corner b;

			if (x == TurnDir.CW)
			{
				a = edges[(int) EdgeId.UR];
				edges[(int) EdgeId.UR] = edges[(int) EdgeId.FR];
				edges[(int) EdgeId.FR] = edges[(int) EdgeId.DR];
				edges[(int) EdgeId.DR] = edges[(int) EdgeId.BR];
				edges[(int) EdgeId.BR] = a;
				corners[(int) CornerId.URF].cw();
				corners[(int) CornerId.DRB].cw();
				corners[(int) CornerId.URB].ccw();
				corners[(int) CornerId.DRF].ccw();
				b = corners[(int) CornerId.URF];
				corners[(int) CornerId.URF] = corners[(int) CornerId.DRF];
				corners[(int) CornerId.DRF] = corners[(int) CornerId.DRB];
				corners[(int) CornerId.DRB] = corners[(int) CornerId.URB];
				corners[(int) CornerId.URB] = b;
			}
			if (x == TurnDir.CCW)
			{
				a = edges[(int)EdgeId.UR];
				edges[(int)EdgeId.UR] = edges[(int)EdgeId.BR];
				edges[(int)EdgeId.BR] = edges[(int)EdgeId.DR];
				edges[(int)EdgeId.DR] = edges[(int)EdgeId.FR];
				edges[(int)EdgeId.FR] = a;
				corners[(int)CornerId.URF].cw();
				corners[(int)CornerId.DRB].cw();
				corners[(int)CornerId.DRF].ccw();
				corners[(int)CornerId.URB].ccw();
				b = corners[(int)CornerId.URF];
				corners[(int)CornerId.URF] = corners[(int)CornerId.URB];
				corners[(int)CornerId.URB] = corners[(int)CornerId.DRB];
				corners[(int)CornerId.DRB] = corners[(int)CornerId.DRF];
				corners[(int)CornerId.DRF] = b;
			}
			if (x == TurnDir.DOUBLE)
			{
				a = edges[(int)EdgeId.UR];
				edges[(int)EdgeId.UR] = edges[(int)EdgeId.DR];
				edges[(int)EdgeId.DR] = a;
				a = edges[(int)EdgeId.FR];
				edges[(int)EdgeId.FR] = edges[(int)EdgeId.BR];
				edges[(int)EdgeId.BR] = a;
				b = corners[(int)CornerId.URF];
				corners[(int)CornerId.URF] = corners[(int)CornerId.DRB];
				corners[(int)CornerId.DRB] = b;
				b = corners[(int)CornerId.DRF];
				corners[(int)CornerId.DRF] = corners[(int)CornerId.URB];
				corners[(int)CornerId.URB] = b;
			}
		}

		public void L (TurnDir x)
		{
			Edge a;
			Corner b;

			if (x == TurnDir.CW)
			{
				a = edges[(int)EdgeId.UL];
				edges[(int)EdgeId.UL] = edges[(int)EdgeId.BL];
				edges[(int)EdgeId.BL] = edges[(int)EdgeId.DL];
				edges[(int)EdgeId.DL] = edges[(int)EdgeId.FL];
				edges[(int)EdgeId.FL] = a;
				corners[(int)CornerId.DLF].cw();
				corners[(int)CornerId.ULB].cw();
				corners[(int)CornerId.ULF].ccw();
				corners[(int)CornerId.DLB].ccw();
				b = corners[(int)CornerId.ULF];
				corners[(int)CornerId.ULF] = corners[(int)CornerId.ULB];
				corners[(int)CornerId.ULB] = corners[(int)CornerId.DLB];
				corners[(int)CornerId.DLB] = corners[(int)CornerId.DLF];
				corners[(int)CornerId.DLF] = b;
			}
			if (x == TurnDir.CCW)
			{
				a = edges[(int)EdgeId.UL];
				edges[(int)EdgeId.UL] = edges[(int)EdgeId.FL];
				edges[(int)EdgeId.FL] = edges[(int)EdgeId.DL];
				edges[(int)EdgeId.DL] = edges[(int)EdgeId.BL];
				edges[(int)EdgeId.BL] = a;
				corners[(int)CornerId.DLF].cw();
				corners[(int)CornerId.ULB].cw();
				corners[(int)CornerId.ULF].ccw();
				corners[(int)CornerId.DLB].ccw();
				b = corners[(int)CornerId.ULF];
				corners[(int)CornerId.ULF] = corners[(int)CornerId.DLF];
				corners[(int)CornerId.DLF] = corners[(int)CornerId.DLB];
				corners[(int)CornerId.DLB] = corners[(int)CornerId.ULB];
				corners[(int)CornerId.ULB] = b;
			}
			if (x == TurnDir.DOUBLE)
			{
				a = edges[(int)EdgeId.UL];
				edges[(int)EdgeId.UL] = edges[(int)EdgeId.DL];
				edges[(int)EdgeId.DL] = a;
				a = edges[(int)EdgeId.BL];
				edges[(int)EdgeId.BL] = edges[(int)EdgeId.FL];
				edges[(int)EdgeId.FL] = a;
				b = corners[(int)CornerId.ULF];
				corners[(int)CornerId.ULF] = corners[(int)CornerId.DLB];
				corners[(int)CornerId.DLB] = b;
				b = corners[(int)CornerId.ULB];
				corners[(int)CornerId.ULB] = corners[(int)CornerId.DLF];
				corners[(int)CornerId.DLF] = b;
			}
		}

		public void U (TurnDir x)
		{
			Edge a;
			Corner b;

			if (x == TurnDir.CW)
			{
				a = edges[(int)EdgeId.UR];
				edges[(int)EdgeId.UR] = edges[(int)EdgeId.UB];
				edges[(int)EdgeId.UB] = edges[(int)EdgeId.UL];
				edges[(int)EdgeId.UL] = edges[(int)EdgeId.UF];
				edges[(int)EdgeId.UF] = a;
				b = corners[(int)CornerId.URF];
				corners[(int)CornerId.URF] = corners[(int)CornerId.URB];
				corners[(int)CornerId.URB] = corners[(int)CornerId.ULB];
				corners[(int)CornerId.ULB] = corners[(int)CornerId.ULF];
				corners[(int)CornerId.ULF] = b;
			}
			if (x == TurnDir.CCW)
			{
				a = edges[(int)EdgeId.UR];
				edges[(int)EdgeId.UR] = edges[(int)EdgeId.UF];
				edges[(int)EdgeId.UF] = edges[(int)EdgeId.UL];
				edges[(int)EdgeId.UL] = edges[(int)EdgeId.UB];
				edges[(int)EdgeId.UB] = a;
				b = corners[(int)CornerId.URF];
				corners[(int)CornerId.URF] = corners[(int)CornerId.ULF];
				corners[(int)CornerId.ULF] = corners[(int)CornerId.ULB];
				corners[(int)CornerId.ULB] = corners[(int)CornerId.URB];
				corners[(int)CornerId.URB] = b;
			}
			if (x == TurnDir.DOUBLE)
			{
				a = edges[(int)EdgeId.UR];
				edges[(int)EdgeId.UR] = edges[(int)EdgeId.UL];
				edges[(int)EdgeId.UL] = a;
				a = edges[(int)EdgeId.UF];
				edges[(int)EdgeId.UF] = edges[(int)EdgeId.UB];
				edges[(int)EdgeId.UB] = a;
				b = corners[(int)CornerId.URF];
				corners[(int)CornerId.URF] = corners[(int)CornerId.ULB];
				corners[(int)CornerId.ULB] = b;
				b = corners[(int)CornerId.URB];
				corners[(int)CornerId.URB] = corners[(int)CornerId.ULF];
				corners[(int)CornerId.ULF] = b;
			}
		}

		public void D (TurnDir x)
		{
			Edge a;
			Corner b;

			if (x == TurnDir.CW)
			{
				a = edges[(int)EdgeId.DF];
				edges[(int)EdgeId.DF] = edges[(int)EdgeId.DL];
				edges[(int)EdgeId.DL] = edges[(int)EdgeId.DB];
				edges[(int)EdgeId.DB] = edges[(int)EdgeId.DR];
				edges[(int)EdgeId.DR] = a;
				b = corners[(int)CornerId.DLF];
				corners[(int)CornerId.DLF] = corners[(int)CornerId.DLB];
				corners[(int)CornerId.DLB] = corners[(int)CornerId.DRB];
				corners[(int)CornerId.DRB] = corners[(int)CornerId.DRF];
				corners[(int)CornerId.DRF] = b;
			}
			if (x == TurnDir.CCW)
			{
				a = edges[(int)EdgeId.DF];
				edges[(int)EdgeId.DF] = edges[(int)EdgeId.DR];
				edges[(int)EdgeId.DR] = edges[(int)EdgeId.DB];
				edges[(int)EdgeId.DB] = edges[(int)EdgeId.DL];
				edges[(int)EdgeId.DL] = a;
				b = corners[(int)CornerId.DLF];
				corners[(int)CornerId.DLF] = corners[(int)CornerId.DRF];
				corners[(int)CornerId.DRF] = corners[(int)CornerId.DRB];
				corners[(int)CornerId.DRB] = corners[(int)CornerId.DLB];
				corners[(int)CornerId.DLB] = b;
			}
			if (x == TurnDir.DOUBLE)
			{
				a = edges[(int)EdgeId.DF];
				edges[(int)EdgeId.DF] = edges[(int)EdgeId.DB];
				edges[(int)EdgeId.DB] = a;
				a = edges[(int)EdgeId.DL];
				edges[(int)EdgeId.DL] = edges[(int)EdgeId.DR];
				edges[(int)EdgeId.DR] = a;
				b = corners[(int)CornerId.DLF];
				corners[(int)CornerId.DLF] = corners[(int)CornerId.DRB];
				corners[(int)CornerId.DRB] = b;
				b = corners[(int)CornerId.DRF];
				corners[(int)CornerId.DRF] = corners[(int)CornerId.DLB];
				corners[(int)CornerId.DLB] = b;
			}
		}

		public void F (TurnDir x)
		{
			Edge a;
			Corner b;

			if (x == TurnDir.CW)
			{
				edges[(int)EdgeId.UF].flip();
				edges[(int)EdgeId.FR].flip();
				edges[(int)EdgeId.DF].flip();
				edges[(int)EdgeId.FL].flip();
				a = edges[(int)EdgeId.UF];
				edges[(int)EdgeId.UF] = edges[(int)EdgeId.FL];
				edges[(int)EdgeId.FL] = edges[(int)EdgeId.DF];
				edges[(int)EdgeId.DF] = edges[(int)EdgeId.FR];
				edges[(int)EdgeId.FR] = a;
				corners[(int)CornerId.DRF].cw();
				corners[(int)CornerId.ULF].cw();
				corners[(int)CornerId.URF].ccw();
				corners[(int)CornerId.DLF].ccw();
				b = corners[(int)CornerId.URF];
				corners[(int)CornerId.URF] = corners[(int)CornerId.ULF];
				corners[(int)CornerId.ULF] = corners[(int)CornerId.DLF];
				corners[(int)CornerId.DLF] = corners[(int)CornerId.DRF];
				corners[(int)CornerId.DRF] = b;
			}
			if (x == TurnDir.CCW)
			{
				edges[(int)EdgeId.UF].flip();
				edges[(int)EdgeId.FR].flip();
				edges[(int)EdgeId.DF].flip();
				edges[(int)EdgeId.FL].flip();
				a = edges[(int)EdgeId.UF];
				edges[(int)EdgeId.UF] = edges[(int)EdgeId.FR];
				edges[(int)EdgeId.FR] = edges[(int)EdgeId.DF];
				edges[(int)EdgeId.DF] = edges[(int)EdgeId.FL];
				edges[(int)EdgeId.FL] = a;
				corners[(int)CornerId.DRF].cw();
				corners[(int)CornerId.ULF].cw();
				corners[(int)CornerId.URF].ccw();
				corners[(int)CornerId.DLF].ccw();
				b = corners[(int)CornerId.URF];
				corners[(int)CornerId.URF] = corners[(int)CornerId.DRF];
				corners[(int)CornerId.DRF] = corners[(int)CornerId.DLF];
				corners[(int)CornerId.DLF] = corners[(int)CornerId.ULF];
				corners[(int)CornerId.ULF] = b;
			}
			if (x == TurnDir.DOUBLE)
			{
				a = edges[(int)EdgeId.UF];
				edges[(int)EdgeId.UF] = edges[(int)EdgeId.DF];
				edges[(int)EdgeId.DF] = a;
				a = edges[(int)EdgeId.FR];
				edges[(int)EdgeId.FR] = edges[(int)EdgeId.FL];
				edges[(int)EdgeId.FL] = a;
				b = corners[(int)CornerId.URF];
				corners[(int)CornerId.URF] = corners[(int)CornerId.DLF];
				corners[(int)CornerId.DLF] = b;
				b = corners[(int)CornerId.DRF];
				corners[(int)CornerId.DRF] = corners[(int)CornerId.ULF];
				corners[(int)CornerId.ULF] = b;
			}
		}

		public void B (TurnDir x)
		{
			Edge a;
			Corner b;

			if (x == TurnDir.CW)
			{
				edges[(int)EdgeId.UB].flip();
				edges[(int)EdgeId.BR].flip();
				edges[(int)EdgeId.DB].flip();
				edges[(int)EdgeId.BL].flip();
				a = edges[(int)EdgeId.UB];
				edges[(int)EdgeId.UB] = edges[(int)EdgeId.BR];
				edges[(int)EdgeId.BR] = edges[(int)EdgeId.DB];
				edges[(int)EdgeId.DB] = edges[(int)EdgeId.BL];
				edges[(int)EdgeId.BL] = a;
				corners[(int)CornerId.URB].cw();
				corners[(int)CornerId.DLB].cw();
				corners[(int)CornerId.ULB].ccw();
				corners[(int)CornerId.DRB].ccw();
				b = corners[(int)CornerId.URB];
				corners[(int)CornerId.URB] = corners[(int)CornerId.DRB];
				corners[(int)CornerId.DRB] = corners[(int)CornerId.DLB];
				corners[(int)CornerId.DLB] = corners[(int)CornerId.ULB];
				corners[(int)CornerId.ULB] = b;
			}
			if (x == TurnDir.CCW)
			{
				edges[(int)EdgeId.UB].flip();
				edges[(int)EdgeId.BR].flip();
				edges[(int)EdgeId.DB].flip();
				edges[(int)EdgeId.BL].flip();
				a = edges[(int)EdgeId.UB];
				edges[(int)EdgeId.UB] = edges[(int)EdgeId.BL];
				edges[(int)EdgeId.BL] = edges[(int)EdgeId.DB];
				edges[(int)EdgeId.DB] = edges[(int)EdgeId.BR];
				edges[(int)EdgeId.BR] = a;
				corners[(int)CornerId.URB].cw();
				corners[(int)CornerId.DLB].cw();
				corners[(int)CornerId.ULB].ccw();
				corners[(int)CornerId.DRB].ccw();
				b = corners[(int)CornerId.URB];
				corners[(int)CornerId.URB] = corners[(int)CornerId.ULB];
				corners[(int)CornerId.ULB] = corners[(int)CornerId.DLB];
				corners[(int)CornerId.DLB] = corners[(int)CornerId.DRB];
				corners[(int)CornerId.DRB] = b;
			}
			if (x == TurnDir.DOUBLE)
			{
				a = edges[(int)EdgeId.UB];
				edges[(int)EdgeId.UB] = edges[(int)EdgeId.DB];
				edges[(int)EdgeId.DB] = a;
				a = edges[(int)EdgeId.BR];
				edges[(int)EdgeId.BR] = edges[(int)EdgeId.BL];
				edges[(int)EdgeId.BL] = a;
				b = corners[(int)CornerId.URB];
				corners[(int)CornerId.URB] = corners[(int)CornerId.DLB];
				corners[(int)CornerId.DLB] = b;
				b = corners[(int)CornerId.DRB];
				corners[(int)CornerId.DRB] = corners[(int)CornerId.ULB];
				corners[(int)CornerId.ULB] = b;
			}
		}
	}
}
