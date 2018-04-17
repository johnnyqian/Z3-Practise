using System;
using System.Collections.Generic;
using Microsoft.Z3;

namespace AncientChinaMathematics
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Context ctx = new Context(new Dictionary<string, string>() { { "model", "true" } }))
            {
                ChickenAndRabbit(ctx);

                Console.Write(Environment.NewLine);

                AHundredChicken(ctx);
            }
        }

        /// <summary>
        /// 鸡兔同笼问题，是我国古代著名数学趣题之一，大约在1500年前，《孙子算经》里记载了这个有趣的问题。
        /// “今有雉（鸡）兔同笼，上（共）有三十五头，下（共）有九十四足，问雉兔各几何？”
        /// </summary>
        /// <param name="ctx"></param>
        static void ChickenAndRabbit(Context ctx)
        {
            IntExpr chicken = ctx.MkIntConst("chicken");
            IntExpr rabbit = ctx.MkIntConst("rabbit");

            Solver solver = ctx.MkSolver();

            var exp1 = ctx.MkEq(ctx.MkAdd(chicken, rabbit), ctx.MkInt(35));
            var exp2 = ctx.MkEq(ctx.MkAdd(chicken * 2, rabbit * 4), ctx.MkInt(94));

            solver.Add(exp1, exp2);

            Output(ctx, solver);
        }

        /// <summary>
        /// 百鸡问题出自中国古代约5—6世纪成书的《张邱建算经》，该问题是一个三元一次不定方程组，其重要之处在于开创“一问多答”的先例。
        /// “今有鸡翁一，值钱五；鸡母一，值钱三；鸡雏三，值钱一。凡百钱买鸡百只。问鸡翁母雏各几何？”
        /// </summary>
        /// <param name="ctx"></param>
        static void AHundredChicken(Context ctx)
        {
            IntExpr rooster = ctx.MkIntConst("rooster");
            IntExpr hen = ctx.MkIntConst("hen");
            IntExpr chick = ctx.MkIntConst("chick");

            Solver solver = ctx.MkSolver();

            var exp1 = ctx.MkEq(ctx.MkAdd(rooster, hen, chick), ctx.MkInt(100));
            // var exp2 = ctx.MkEq(ctx.MkAdd(rooster * 5, hen * 3, chick / 3), ctx.MkInt(100));
            var exp2 = ctx.MkEq(ctx.MkAdd(rooster * 15, hen * 9, chick), ctx.MkInt(300));

            solver.Add(exp1, exp2, rooster > 0, hen > 0, chick > 0);

            OutputAllModels(ctx, solver);
        }

        static void Output(Context ctx, Solver solver)
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

        static void OutputAllModels(Context ctx, Solver solver)
        {
            while (solver.Check() == Status.SATISFIABLE)
            {
                Console.WriteLine(solver.Check());

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

                    solver.Add(ctx.MkNot(ctx.MkEq(ctx.MkIntConst(decl.Name), retVal)));
                }

                foreach (var pair in ret)
                {
                    Console.WriteLine($"{pair.Key} = {pair.Value}");
                }

                Console.Write(Environment.NewLine);
            }

            Console.WriteLine("No satisfiable model anymore.");
        }
    }
}
