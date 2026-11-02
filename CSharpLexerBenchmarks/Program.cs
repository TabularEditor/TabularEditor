using System;
using System.Diagnostics;
using Antlr4.Runtime;
using BenchmarkDotNet.Running;
using TabularEditor.TextServices;
using Oracle = CSharpLexerOracle;

namespace CSharpLexerBenchmarks
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--sizes")
            {
                foreach (var (name, script) in new[] { ("Small", ScriptSamples.Small), ("Medium", ScriptSamples.Medium), ("Large", ScriptSamples.Large) })
                {
                    Console.WriteLine($"{name}: {script.Length} chars, {new LexBenchmarks { Size = name }.Count()} tokens");
                }
                return 0;
            }

            // The first script of a process, in a new process each time: ANTLR has its automaton to load
            if (args.Length > 1 && args[0] == "--first")
            {
                var watch = Stopwatch.StartNew();
                if (args[1] == "antlr")
                {
                    var lexer = new Oracle.CSharpLexer(new AntlrInputStream(ScriptSamples.Small));
                    lexer.RemoveErrorListeners();
                    new CommonTokenStream(lexer).Fill();
                }
                else
                {
                    var tokens = new System.Collections.Generic.List<CSharpLexerToken>();
                    CSharpTokenizer.GetAllTokens(ScriptSamples.Small, tokens);
                }
                Console.WriteLine($"{args[1]}: {watch.Elapsed.TotalMilliseconds:F1} ms");
                return 0;
            }

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            return 0;
        }
    }
}
