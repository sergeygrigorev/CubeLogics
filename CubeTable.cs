using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubeLogics
{
	class CubeTable
	{
		public const int FLIP = 2048;
		public const int TWIST = 2187;
		public const int SLICE1 = 495;
		public const int SLICE2 = 24;
		public const int PERM = 40320;

		const int MOVES = 18;
		const int MVS = 10;

		private int flip;
		private int twist;
		private int slice;

		public int[,] flipMoveTable;
		public int[,] twistMoveTable;
		public int[,] sliceMoveTable;
		public int[,] cornerPermMoveTable;
		public int[,] edgePermMoveTable;
		public int[,] slicePermMoveTable;

		public int[] TwistSlicePruneTable;
		public int[] FlipSlicePruneTable;
		public int[] TwistFlipPruneTable;
		public int[] CornerSlicePruneTable;
		public int[] EdgeSlicePruneTable;

		public CubeTable():this(new Cube())
		{
			
		}

		public CubeTable (Cube a)
		{
			flip = a.Flip;
			twist = a.Twist;
			slice = a.Slice;

			MakeFlipMoveTable();
			MakeTwistMoveTable();
			MakeSlice1MoveTable();

			MakeTwistSlicePruneTable();
			MakeFlipSlicePruneTable();

			MakeCornerPermMoveTable();
			MakeEdgePermMoveTable();
			MakeSlicePermMoveTable();

			MakeCornerSlicePruneTable();
			MakeEdgeSlicePruneTable();
		}

		public static int getMoveNumber (int n)
		{
			int i;

			for (i = 0; i < MVS; i++)
				if (n == getMove(i))
					return i;
			throw new Exception("Invalid move!");
		}

		static int getMove (int n)
		{
			int m = 1;
			int i;

			for (i = 0; i < n; i++)
				m = nextMove(m);

			return m;
		}

		public static int nextMove (int n)
		{
			if (n == 16)
				return 1;
			if (n > 5 && n < 11)
				return n + 1;
			if (n == 11 || n == 4)
				return n + 2;
			if (n == 1 || n == 13)
				return n + 3;
			else throw new Exception("Wrong move in phase 2!");
		}

		void MakeFlipMoveTable ()
		{
			int i, j;
			Cube a = new Cube();

			flipMoveTable = new int[FLIP, MOVES];
			for (i=0;i<FLIP;i++)
				for (j=0;j<MOVES;j++)
				{
					a.Flip = i;
					a.Rotate(new Move((FaceId) (j/3), (TurnDir) (j%3)));
					flipMoveTable[i,j] = a.Flip;
				}
		}

		void MakeTwistMoveTable ()
		{
			int i, j;
			Cube a = new Cube();

			twistMoveTable = new int[TWIST, MOVES];
			for (i = 0; i < TWIST; i++)
				for (j = 0; j < MOVES; j++)
				{
					a.Twist = i;
					a.Rotate(new Move((FaceId)(j / 3), (TurnDir)(j % 3)));
					twistMoveTable[i, j] = a.Twist;
				}
		}

		void MakeSlice1MoveTable ()
		{
			int i, j;
			Cube a = new Cube();

			sliceMoveTable = new int[SLICE1, MOVES];
			for (i = 0; i < SLICE1; i++)
				for (j = 0; j < MOVES; j++)
				{
					a.Slice = i*24;
					a.Rotate(new Move((FaceId)(j / 3), (TurnDir)(j % 3)));
					sliceMoveTable[i, j] = a.Slice/24;
				}
		}

		void MakeCornerPermMoveTable ()
		{
			int i, j, k;
			Cube a = new Cube();

			cornerPermMoveTable = new int[PERM, MVS];
			for (i = 0, j = 1; i < PERM; i++, j = 1)
				for (k = 0; k < MVS; k++)
				{
					a.CornerPerm = i;
					a.Rotate(new Move((FaceId)(j / 3), (TurnDir)(j % 3)));
					cornerPermMoveTable[i, k] = a.CornerPerm;
					j = nextMove(j);
				}
		}

		void MakeEdgePermMoveTable()
		{
			int i, j, k;
			Cube a = new Cube();

			edgePermMoveTable = new int[PERM, MVS];
			for (i = 0, j = 1; i < PERM; i++, j = 1)
				for (k = 0; k < MVS; k++)
				{
					a.Edge8Perm = i;
					a.Rotate(new Move((FaceId) (j/3), (TurnDir) (j%3)));
					edgePermMoveTable[i, k] = a.Edge8Perm;
					j = nextMove(j);
				}
		}

		void MakeSlicePermMoveTable()
		{
			int i, j, k;
			Cube a = new Cube();

			slicePermMoveTable = new int[SLICE2, MVS];
			for (i = 0, j = 1; i < SLICE2; i++, j = 1)
				for (k = 0; k < MVS; k++)
				{
					a.Slice = i;
					a.Rotate(new Move((FaceId)(j / 3), (TurnDir)(j % 3)));
					slicePermMoveTable[i, k] = a.Slice%SLICE2;
					j = nextMove(j);
				}
		}

		void MakeTwistSlicePruneTable ()
		{
			int i, j, done, depth, nt, ns;

			TwistSlicePruneTable = new int[TWIST*SLICE1];
			TwistSlicePruneTable[0] = 0;
			for (i = 1; i < TWIST * SLICE1; i++)
				TwistSlicePruneTable[i] = -1;
			done = 1;
			depth = 1;
			while (done < TWIST * SLICE1)
			{
				for (i = 0; i < TWIST * SLICE1; i++)
				{
					if (TwistSlicePruneTable[i] == depth - 1)
						for (j = 0; j < MOVES; j++)
						{
							nt = twistMoveTable[i/SLICE1, j];
							ns = sliceMoveTable[i%SLICE1, j];
							if (TwistSlicePruneTable[nt*SLICE1+ns] == -1)
							{
								TwistSlicePruneTable[nt * SLICE1 + ns] = depth;
								done++;
							}
						}
				}
				depth++;
			}
		}

		void MakeFlipSlicePruneTable()
		{
			int i, j, done, depth, nf, ns;

			FlipSlicePruneTable = new int[FLIP * SLICE1];
			FlipSlicePruneTable[0] = 0;
			for (i = 1; i < FLIP * SLICE1; i++)
				FlipSlicePruneTable[i] = -1;
			done = 1;
			depth = 1;
			while (done < FLIP * SLICE1)
			{
				for (i = 0; i < FLIP * SLICE1; i++)
				{
					if (FlipSlicePruneTable[i] == depth - 1)
						for (j = 0; j < MOVES; j++)
						{
							nf = flipMoveTable[i / SLICE1, j];
							ns = sliceMoveTable[i % SLICE1, j];
							if (FlipSlicePruneTable[nf * SLICE1 + ns] == -1)
							{
								FlipSlicePruneTable[nf * SLICE1 + ns] = depth;
								done++;
							}
						}
				}
				depth++;
			}
		}

		void MakeTwistFlipPruneTable ()
		{
			int i, j, done, depth, newTwist, newFlip;

			TwistFlipPruneTable = new int[TWIST * FLIP];
			TwistFlipPruneTable[0] = 0;
			for (i = 1; i < TWIST * FLIP; i++)
				TwistFlipPruneTable[i] = -1;
			done = 1;
			depth = 1;
			while (done < TWIST*FLIP)
			{
				for (i = 0; i < TWIST * FLIP;i++ )
				{
					if (TwistFlipPruneTable[i] == depth - 1)
						for (j = 0; j < MOVES; j++)
						{
							newTwist = twistMoveTable[i/FLIP, j];
							newFlip = flipMoveTable[i%FLIP, j];
							if (TwistFlipPruneTable[newTwist * FLIP + newFlip] == -1)
							{
								TwistFlipPruneTable[newTwist * FLIP + newFlip] = depth;
								done++;
							}
						}
				}
				depth++;
			}
		}

		void MakeCornerSlicePruneTable()
		{
			int i, j, done, depth, newCornerPerm, newSlice;

			CornerSlicePruneTable = new int[PERM * SLICE2];
			CornerSlicePruneTable[0] = 0;
			for (i = 1; i < PERM * SLICE2; i++)
				CornerSlicePruneTable[i] = -1;
			done = 1;
			depth = 1;
			while (done < PERM * SLICE2)
			{
				for (i = 0; i < PERM * SLICE2; i++)
				{
					if (CornerSlicePruneTable[i] == depth - 1)
						for (j = 0; j < MVS; j++)
						{
							newCornerPerm = cornerPermMoveTable[i / SLICE2, j];
							newSlice = slicePermMoveTable[i % SLICE2, j];
							if (CornerSlicePruneTable[newCornerPerm * SLICE2 + newSlice] == -1)
							{
								CornerSlicePruneTable[newCornerPerm * SLICE2 + newSlice] = depth;
								done++;
							}
						}
				}
				depth++;
			}
		}

		void MakeEdgeSlicePruneTable()
		{
			int i, j, done, depth, newEdgePerm, newSlice;

			EdgeSlicePruneTable = new int[PERM * SLICE2];
			EdgeSlicePruneTable[0] = 0;
			for (i = 1; i < PERM * SLICE2; i++)
				EdgeSlicePruneTable[i] = -1;
			done = 1;
			depth = 1;
			while (done < PERM * SLICE2)
			{
				for (i = 0; i < PERM * SLICE2; i++)
				{
					if (EdgeSlicePruneTable[i] == depth - 1)
						for (j = 0; j < MVS; j++)
						{
							newEdgePerm = edgePermMoveTable[i / SLICE2, j];
							newSlice = slicePermMoveTable[i % SLICE2, j];
							if (EdgeSlicePruneTable[newEdgePerm * SLICE2 + newSlice] == -1)
							{
								EdgeSlicePruneTable[newEdgePerm * SLICE2 + newSlice] = depth;
								done++;
							}
						}
				}
				depth++;
			}
		}
		
	}
}
