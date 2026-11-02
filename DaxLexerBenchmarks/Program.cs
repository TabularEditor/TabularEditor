using System;
using System.Linq;
using BenchmarkDotNet.Running;
using TabularEditor.TextServices;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Utils;

namespace DaxLexerBenchmarks
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--verify") return Verify();
            if (args.Length > 0 && args[0] == "--sizes")
            {
                Console.WriteLine($"Small {DaxSamples.Small.Length}, Keystroke {DaxSamples.Keystroke.Length}, Large {DaxSamples.Large.Length} chars, tokens {DaxTokenizer.GetAllTokens(DaxSamples.Small).Count}/{DaxTokenizer.GetAllTokens(DaxSamples.Keystroke).Count}/{DaxTokenizer.GetAllTokens(DaxSamples.Large).Count}");
                return 0;
            }
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
            return 0;
        }

        // The dependencies of each expression of the model must be the same as with the ANTLR lexer
        private static int Verify()
        {
            DaxModel.Load(generateMeasures: true);
            var references = 0;
            var totalChars = DaxModel.Expressions.Sum(e => e.Dax.Length);
            foreach (var (obj, property, dax) in DaxModel.Expressions)
            {
                var expected = Describe(AntlrFormulaFixup.GetDependencies(obj, dax, property));
                var actual = Describe(FormulaFixup.GetDependencies(obj, dax, property));
                if (expected != actual)
                {
                    Console.WriteLine($"Different dependencies for {obj}:\n{dax}\n--- antlr\n{expected}\n--- parlot\n{actual}");
                    return 1;
                }
                references += expected.Count(c => c == '\n');
            }
            Console.WriteLine($"Model: {DaxModel.Objects.Count} objects, {DaxModel.Expressions.Count} expressions, {totalChars} chars, {references} references: all identical");
            return 0;
        }

        private static string Describe(DependsOnList list)
        {
            return string.Concat(list.Keys.Select(k => k.DaxObjectFullName + ": " + string.Join(";", list[k].Select(r => $"{r.property} {r.from}-{r.to} {r.fullyQualified}")) + "\n"));
        }
    }
}
