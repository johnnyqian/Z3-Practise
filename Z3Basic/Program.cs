using System;
using System.Collections.Generic;
using Microsoft.Z3;

namespace Z3Basic
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.ToggleWarningMessages(true);

            Console.Write("Z3 Full Version: ");
            Console.WriteLine(Microsoft.Z3.Version.ToString());

            Console.Write(Environment.NewLine);

            using (Context ctx = new Context(new Dictionary<string, string>() { { "model", "true" } }))
            {
                IntExpr x = ctx.MkIntConst("x");
                IntExpr y = ctx.MkIntConst("y");

                Solver solver = ctx.MkSolver();

                solver.Add(x + y < 5, x > 1, y > 1);
                Check(ctx, solver);

                solver.Reset();

                solver.Add(x + y < 3, x > 1, y > 1);
                Check(ctx, solver);
            }
        }

        static void Check(Context ctx, Solver solver)
        {
            Console.WriteLine(solver.Check());
            if (solver.Check() == Status.SATISFIABLE)
            {
                Model m = solver.Model;
                FuncDecl[] decls = m.ConstDecls;

                var ret = new Dictionary<string, Expr>((int)m.NumConsts);

                foreach (FuncDecl decl in decls)
                {
                    IntNum retVal = m.Evaluate(ctx.MkIntConst(decl.Name.ToString()), true) as IntNum;

                    if (retVal == null)
                    {
                        throw new Exception($"Failed to get parameter {decl.Name.ToString()}");
                    }

                    ret.Add(decl.Name.ToString(), retVal);
                }

                foreach (var pair in ret)
                {
                    Console.WriteLine($"{pair.Key} = {pair.Value}");
                }
            }
        }
    }
}
