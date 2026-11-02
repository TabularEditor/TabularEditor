# Parlot for the DAX lexer

This branch replaces the ANTLR lexer of DAX with one built on [Parlot](https://github.com/sebastienros/parlot)'s
`Cursor`, and removes ANTLR from `TOMWrapper` and `BPALib`. It returns the same tokens as ANTLR on every input tested:
type, channel, text, start and stop index, line and column. The token IDs of `DaxToken`, which Best Practice Analyzer
rules use by name, don't change.

On .NET Framework 4.8, which Tabular Editor 2 runs on, rebuilding the dependency tree of a model with about 2,100
expressions is **3.2x faster and allocates 5.6x less**. The lexing part of a keystroke in the expression editor is
**6.4x faster and allocates 4.1x less**.

The C# lexer (`CSharpLexer`, for scripts) still uses ANTLR, so `Antlr4.Runtime` still ships with the application. See
[What this leaves out](#what-this-leaves-out).

## What changed

| Area | Files |
|---|---|
| Lexer | `TOMWrapper/TextServices/DaxTokenizer.cs`, `DaxLexerToken.cs` (a struct, the text is only built when read), `DaxKeywords.cs`, `DaxKeywords.Generated.cs` (the 392 keywords of the grammar) |
| Consumers | `FormulaFixup`, `DaxDependencyHelper`, `DaxToken`, `ExpressionParser`, `UIController_ExpressionEditor` |
| Removed | `DAXCharStream`, `DAXLexerExtensions`, `DaxToken.tt` (its source no longer exists, `DaxToken.Generated.cs` is unchanged), the DAX grammar from `AntlrGrammars` and the Java step for it |
| Dependencies | `Parlot` 2.2.1 from nuget.org in `TOMWrapper` and `TabularEditor`, `Antlr4.Runtime` out of `TOMWrapper` |
| `BPALib.csproj` | Didn't build before this branch (it used TOM 19.90 and lacked newer shims). Now uses 19.117, as the application does, and builds on macOS and Windows |
| New projects, in `BPALib.sln` | `DaxLexerOracle` (the original grammar with ANTLR, only for comparing), `DaxLexerTests`, `DaxLexerBenchmarks` |

`DAXLexer`, `DAXCharStream` and `DAXLexerExtensions` were public types of `TOMWrapper`, so a script that used them
directly stops compiling. `DaxToken` and `Tokenize()`, the documented API, behave as before.

### Decisions

- **A token struct rather than an `ITokenSource` adapter.** The DAX side only touched `IToken` in five files, so the
  direct change was small. The adapter takes 1.5x to 2.5x the time of the struct and allocates twice as much (tables
  below).
- **`FormulaFixup` no longer builds a list.** It only ever looks one token ahead. It also reads each token's text once
  now, and its measure lookup no longer allocates a closure for every token.
- **Lines and columns are computed when read.** No consumer reads them, and tracking them per token through
  `Cursor.Position` took about half of the lexing time on .NET 8 on an Apple M4 Pro.

## How parity was checked

The lexer deliberately copies what ANTLR does, including its error recovery: a string that is never closed vanishes,
`1.x` gives `1` and drops `.x`, `U+2007` isn't white space, and in the non-US format `STDEV,P` is the keyword `STDEV.P`.
Nested comments follow ANTLR's non-greedy reading, which depends on the text that comes after a `*/`.

`DaxLexerTests` compares every token with the ANTLR lexer generated from the original grammar, in both the US and non-US
formats:

| Test | Input |
|---|---|
| `EveryCharInEveryContext` | All 65,536 UTF-16 code units in 23 contexts (in strings, names, comments, after `1.`, ...) |
| `AllShortStringsOfTheStructuralChars` | Every string of 6 to 11 pieces or fewer over 10 alphabets of structural chars |
| `RandomStrings` | 502,000 random strings of quotes, brackets, comments, numbers, separators and keywords |
| `EveryKeywordInAnyCase` | All keywords in three casings, with suffixes, and all their prefixes |
| `RealExpressions`, `RealExpressionsCutAnywhere` | All expressions of the test models, and every prefix of 300 of them |
| `TokenTypesMatchTheGrammar` | Every token ID and name of `DaxToken` against the grammar |
| `ConsumerTests` | A BPA rule using `Tokenize().Any(Type = DIV)` on a real model, and the `DaxToken` output |

`DaxLexerBenchmarks --verify` compares the dependencies found by `FormulaFixup` with a copy of its ANTLR version, on
all 2,077 expressions of the benchmark model (6,978 references): all identical.

## Benchmarks

BenchmarkDotNet, 8 iterations, x64 on an AMD EPYC 7763 Windows VM unless noted. The Windows numbers are from one run per
runtime on a VM whose noise can reach 27%, so read the ratios as approximate.

```bash
dotnet run -c Release --project DaxLexerBenchmarks -f net8.0 -- --filter '*'
```

### The layer above the lexer: `FormulaFixup`

`AdventureWorks.bim` from the test data, loaded through the real TOM wrapper, plus 2,000 generated measures (variables,
time intelligence, comments, switches) because its own 77 expressions are short: 2,077 expressions, 399 KB of DAX.

| Runtime | Benchmark | ANTLR | Parlot | Time | Allocated (ANTLR → Parlot) |
|---|---|---:|---:|---:|---:|
| .NET Framework 4.8 | `BuildDependencyTree` | 60.2 ms | 18.8 ms | 3.2x faster | 16.3 MB → 2.9 MB (5.6x less) |
| .NET Framework 4.8 | `GetDependencies` | 57.3 ms | 20.2 ms | 2.8x faster | 17.8 MB → 4.5 MB (4.0x less) |
| .NET 8 x64 | `BuildDependencyTree` | 26.4 ms | 9.5 ms | 2.8x faster | 15.9 MB → 2.8 MB (5.7x less) |
| .NET 8 x64 | `GetDependencies` | 23.0 ms | 8.8 ms | 2.6x faster | 16.9 MB → 3.7 MB (4.5x less) |
| .NET 8 Apple M4 Pro | `BuildDependencyTree` | 14.2 ms | 4.1 ms | 3.5x faster | 15.9 MB → 2.8 MB (5.7x less) |
| .NET 8 Apple M4 Pro | `GetDependencies` | 12.3 ms | 4.0 ms | 3.1x faster | 16.9 MB → 3.7 MB (4.5x less) |

Going by the lex-only numbers below, lexing is now about a quarter of this time. The rest is looking objects up and
recording references, which this branch doesn't change.

### A keystroke in the expression editor

`SyntaxHighlight` lexes the whole text on each change below 2,000 chars. This is that part on a 1,654-char expression (479
tokens): the list of tokens, then a channel check per token. FastColoredTextBox's styling isn't included.

| Runtime | ANTLR | Parlot | Time | Allocated |
|---|---:|---:|---:|---:|
| .NET Framework 4.8 | 170.4 µs | 26.6 µs | 6.4x faster | 53.9 KB → 13.1 KB (4.1x less) |
| .NET 8 x64 | 62.2 µs | 7.4 µs | 8.4x faster | 53.0 KB → 13.1 KB (4.1x less) |
| .NET 8 Apple M4 Pro | 33.8 µs | 3.8 µs | 8.8x faster | 53.0 KB → 13.1 KB (4.1x less) |

### Lexing alone, .NET Framework 4.8

Small is 263 chars (88 tokens), Keystroke 1,654 chars (479), Large 49,668 chars (14,188).

| Size | ANTLR | `IToken` adapter | Struct list | No list |
|---|---:|---:|---:|---:|
| Small | 26.6 µs | 6.6 µs | 4.4 µs | 3.7 µs |
| Keystroke | 171.3 µs | 35.6 µs | 23.6 µs | 22.5 µs (noisy) |
| Large | 6,316 µs | 1,970 µs | 777 µs | 576 µs |
| Allocated, Large | 1.5 MB | 832 KB | 398 KB | 80 B |

### Lexing alone, .NET 8 x64

| Size | ANTLR | `IToken` adapter | Struct list | No list |
|---|---:|---:|---:|---:|
| Small | 9.0 µs | 2.2 µs | 1.3 µs | 0.9 µs |
| Keystroke | 57.2 µs | 16.3 µs | 7.0 µs | 5.1 µs |
| Large | 2,486 µs | 602 µs | 315 µs | 150 µs |
| Allocated, Large | 1.5 MB | 830 KB | 398 KB | 72 B |

## What was run on Windows

On a Windows 11 x64 VM with neither Visual Studio nor Java:

| What | Result |
|---|---|
| `DaxLexerTests` on net48 | 12 of 12 pass, with the framework's own `ToUpperInvariant` |
| `TOMWrapperTest` on net48 | 683 of 713 pass, same set as `master`. All 30 failures need an Analysis Services server (`TE_TestServer`) |
| `TabularEditorTest` on net48 | 40 of 55 pass, same set as `master`. 9 failures need that server, 5 hit a type initializer of the tree control (possibly because of the workaround below), 1 a missing folder of rule files |

These were built with `dotnet msbuild`, using a local, uncommitted `Directory.Build.targets`: preserialized resources and
`System.Resources.Extensions` for `TabularEditor`, MSTest v3 references instead of the Visual Studio ones for
`TabularEditorTest`. The ANTLR lexer was generated on a Mac and copied over, as generating it needs Java.

**Not run:**

- The tests that need a server, among them `FixupVariousColumnTest` and the existing `TokenizeTest`. `ConsumerTests` and
  `--verify` cover the same code on a model loaded from a file.
- The application itself: `TabularEditor.exe` compiles, but syntax highlighting in the editor wasn't tried by hand.
- A build with Visual Studio or MSBuild for .NET Framework.
- The installer (`.vdproj`). `Parlot.dll` must end up in it. Its detected dependencies should pick it up on a refresh in
  Visual Studio, which needs checking.

## What this leaves out

**The C# lexer.** `CSharpLexer` isn't on a hot path, as it only runs when a script runs or is completed, and it is a
harder port: five lexer modes, predicates, and stacks for interpolated strings. Until it's done `Antlr4.Runtime`
stays in `TabularEditor` and `AntlrGrammars` stays in the solution (its output is checked in, so only that grammar's
regeneration needs Java).

**Size.** `Parlot.dll` (net472) is 602 KB against 215 KB for `Antlr4.Runtime.dll`, and both ship until the above is
done. The lexer only needs `Cursor` from it.
