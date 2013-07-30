using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CubeLogics
{
    class Program
    {
		static void Main(string[] args)
		{
			Cube a = new Cube("D2  F'  U  R'  F  D2  B2  U2  D2  R'  B'  R  D  B2  D2  B'  F'  U2  D'  B'  F'  L'  R2  U  R2");
			Solver s = new Solver(a);
			Algo alg = s.MegaSolver();
			a.Rotate(alg);
			Console.WriteLine(alg);
			Console.WriteLine(a);

		}
    }
}
