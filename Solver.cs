using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CubeLogics
{
	class Solver
	{
		private Cube c;
		private Algo _twoPhaseSoluton;
		private CubeTable table;

		//private int[] flip, twist, slice, minDistPhase1, ax, po;

		public Solver () : this (new Cube())
		{
		}

		public Solver (Cube a)
		{
			c = (Cube)a.Clone();
			_twoPhaseSoluton = new Algo();
			table = new CubeTable(a);
		}

		/*public Algo SuperSolver ()
		{
			int i, j, k, startPrune;
			Move a = new Move();
			Algo solution = new Algo();

			k = 0;
			startPrune = table.TwistFlipPruneTable[c.Twist * CubeTable.FLIP + c.Flip];
			for (i=0;i<startPrune;i++)
			{
				for (j = 0; j < 6; j++)
				{
					a = new Move((FaceId) j, 0);
					for (k = 0; k < 4; k++)
					{
						c.Rotate(a);
						if (startPrune-i > table.TwistFlipPruneTable[c.Twist*CubeTable.FLIP+c.Flip])
							goto metka;
					}
				}
				metka:
				a.Dir = (TurnDir) k;
				solution = solution + new Algo(a.ToString());
			}

			return solution;
		}*/

		public Algo SuperSolver()
		{
			int i, j, k, startPrune;
			Move a = new Move();
			Algo solution = new Algo();

			k = 0;
			startPrune = getPrune(c.Twist, c.Flip, c.Slice/24);
			for (i = 0; i < startPrune; i++)
			{
				for (j = 0; j < 6; j++)
				{
					a = new Move((FaceId)j, 0);
					if (solution.Length > 0 && (a.Face == solution[solution.Length - 1].Face || ((int)a.Face % 2 == 0 && a.Face + 1 == solution[solution.Length - 1].Face) || ((int)a.Face % 2 == 1 && a.Face - 1 == solution[solution.Length - 1].Face)))
						continue;
					for (k = 0; k < 3; k++)
					{
						c.Rotate(a);
						if (startPrune - i > getPrune(c.Twist, c.Flip, c.Slice/24))
							goto metka;
					}
					c.Rotate(a);
				}
				if (j == 6)
				{
					i--;
					startPrune++;
					continue;
				}
				metka:
				a.Dir = (TurnDir)k;
				solution = solution + new Algo(a.ToString());
			}

			return solution;
		}

		int getPrune (int t, int f, int s)
		{
			return Math.Max(getFS(f,s),getTS(t,s));
		}

		int getTF (int t, int f)
		{
			return table.TwistFlipPruneTable[t*CubeTable.FLIP + f];
		}

		int getTS(int t, int s)
		{
			return table.TwistSlicePruneTable[t * CubeTable.SLICE1 + s];
		}

		int getFS (int f, int s)
		{
			return table.FlipSlicePruneTable[f * CubeTable.SLICE1 + s];
		}

		int getPrune2 (int c, int e, int s)
		{
			return Math.Max(getCS(c, s), getES(e, s));
		}

		int getCS (int c, int s)
		{
			return table.CornerSlicePruneTable[c*CubeTable.SLICE2+s];
		}

		int getES (int e, int s)
		{
			return table.EdgeSlicePruneTable[e * CubeTable.SLICE2 + s];
		}
		
		public Algo MegaSolver ()
		{
			Algo alg = new Algo();

			int[] flip, twist, slice, minDistPhase1, ax, po;

			po = new int[31];
			ax = new int[31];
			flip = new int[31];
			twist = new int[31];
			slice = new int[31];
			minDistPhase1 = new int[31];

			po[0] = 0;
			ax[0] = 0;
			flip[0] = c.Flip;
			twist[0] = c.Twist;
			slice[0] = c.Slice / 24;

			minDistPhase1[1] = 1;// else failure for depth=1, n=0
			int mv = 0, n = 0;
			bool busy = false;
			int depthPhase1 = 1;

			
			// +++++++++++++++++++ Main loop ++++++++++++++++++++++++++++++++++++++++++
			do
			{
				do
				{



					//Этот ебаный код разбираем
					if ((depthPhase1 - n > minDistPhase1[n + 1]) && !busy)
					{

						if (ax[n] == 0 || ax[n] == 1)
							ax[++n] = 2;
						else
							ax[++n] = 0;
						po[n] = 1;
					}
					else if (++po[n] > 3)
					{
						do
						{
							if (++ax[n] > 5)
							{
								if (n == 0)
								{
									depthPhase1++;
									ax[n] = 0;
									po[n] = 1;
									busy = false;
									break;
								}
								else
								{
									n--;
									busy = true;
									break;
								}
							}
							else
							{
								po[n] = 1;
								busy = false;
							}
						} while (n != 0 && (ax[n - 1] == ax[n] || (ax[n - 1] % 2 == 0 && ax[n - 1] + 1 == ax[n]) || (ax[n - 1] % 2 == 1 && ax[n - 1] - 1 == ax[n])));
					}
					else busy = false;   // и до сюда епт
				} while (busy);

				// +++++++++++++ compute new coordinates and new minDistPhase1 ++++++++++
				// if minDistPhase1 =0, the H subgroup is reached
				mv = 3 * ax[n] + po[n] - 1;
				flip[n + 1] = table.flipMoveTable[flip[n],mv];
				twist[n + 1] = table.twistMoveTable[twist[n],mv];
				slice[n + 1] = table.sliceMoveTable[slice[n],mv];
				minDistPhase1[n + 1] = getPrune(twist[n + 1], flip[n + 1], slice[n + 1]);
				// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

				if (minDistPhase1[n + 1] == 0)
					break;

				/*if (minDistPhase1[n + 1] == 0 && n >= depthPhase1 - 5)
				{
					minDistPhase1[n + 1] = 10;// instead of 10 any value >5 is possible
					if (n == depthPhase1 - 1 && (s = totalDepth(depthPhase1, maxDepth)) >= 0)
					{
						if (s == depthPhase1
								|| (ax[depthPhase1 - 1] != ax[depthPhase1] && ax[depthPhase1 - 1] != ax[depthPhase1] + 3))
							return useSeparator ? solutionToString(s, depthPhase1) : solutionToString(s);
					}

				}*/
			} while (true);

			for (int i = 0; i <= n; i++)
				alg = alg + new Algo((new Move((FaceId)ax[i],(TurnDir)(po[i]-1))).ToString());

			c.Rotate(alg);
			alg += GigaSolver();

			return alg;
		}


		public Algo GigaSolver ()
		{
			Algo alg = new Algo();

			int[] corner, edge, slice, minDistPhase2, ax, po;

			po = new int[31];
			ax = new int[31];
			corner = new int[31];
			edge = new int[31];
			slice = new int[31];
			minDistPhase2 = new int[31];

			po[0] = 1;
			ax[0] = 0;
			corner[0] = c.CornerPerm;
			edge[0] = c.Edge8Perm;
			slice[0] = c.Slice % 24;

			minDistPhase2[1] = 1;
			int mv = 0, n = 0;
			bool busy = false;
			int depthPhase2 = 1;

			do
			{
				do
				{
					if ((depthPhase2 - n > minDistPhase2[n + 1]) && !busy)
					{

						if (ax[n] == 0 || ax[n] == 1)
							ax[++n] = 2;
						else
							ax[++n] = 0;
						po[n] = (ax[n] == 2 || ax[n] == 3) ? 1 : 2;
					}
					else if (++po[n] > 3 || (ax[n] != 2 && ax[n] != 3 && po[n] != 2))
					{
						do
						{
							if (++ax[n] > 5)
							{
								if (n == 0)
								{
									depthPhase2++;
									ax[n] = 0;
									po[n] = 2;
									busy = false;
									break;
								}
								else
								{
									n--;
									busy = true;
									break;
								}
							}
							else
							{
								po[n] = (ax[n] == 2 || ax[n] == 3) ? 1 : 2;
								busy = false;
							}
						} while (n != 0 &&
						         (ax[n - 1] == ax[n] || (ax[n - 1]%2 == 0 && ax[n - 1] + 1 == ax[n]) ||
						          (ax[n - 1]%2 == 1 && ax[n - 1] - 1 == ax[n])));
					}
					else busy = false; 
				} while (busy);

				mv = 3 * ax[n] + po[n] - 1;
				mv = CubeTable.getMoveNumber(mv);
				corner[n + 1] = table.cornerPermMoveTable[corner[n], mv];
				edge[n + 1] = table.edgePermMoveTable[edge[n], mv];
				slice[n + 1] = table.slicePermMoveTable[slice[n], mv];
				minDistPhase2[n + 1] = getPrune2(corner[n + 1], edge[n + 1], slice[n + 1]);

				if (minDistPhase2[n + 1] == 0)
					break;

			} while (true);

			for (int i = 0; i <= n; i++)
				alg = alg + new Algo((new Move((FaceId)ax[i], (TurnDir)(po[i] - 1))).ToString());

			return alg;
		}

		// ololo
	}
}
