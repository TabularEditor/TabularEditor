# Parlot for the lexers

These branches replace the ANTLR lexers of DAX and C# with ones built on
[Parlot](https://github.com/sebastienros/parlot)'s `Cursor`, and remove ANTLR and its Java build step from the product:
`TOMWrapper`, `BPALib`, `TabularEditor`, the installer and `TabularEditor.sln`. ANTLR only remains in the two oracle
projects of `BPALib.sln`, which the tests compare with. The lexers return the same tokens as ANTLR on every input tested:
type, channel, text, start and stop index, line and column. The token IDs of `DaxToken`, which Best Practice Analyzer
rules use by name, don't change.

On .NET Framework 4.8, which Tabular Editor 2 runs on, rebuilding the dependency tree of a model with about 2,100
expressions is **3.2x faster and allocates 5.6x less**. The lexing part of a keystroke in the expression editor is
**6.4x faster and allocates 4.1x less**.

Lexing a script is 5x to 9x faster and allocates 3x to 4x less on .NET Framework 4.8. See
[The C# lexer](#the-c-lexer), which has its own section below; the sections in between are about DAX.

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

## The C# lexer

`feature/parlot-csharp-lexer`, on top of the DAX work.

| Area | Files |
|---|---|
| Lexer | `TabularEditor/TextServices/CSharpTokenizer.cs`, `CSharpLexerToken.cs` (a struct, as for DAX), `CSharpToken.cs` (the token types, same names and values as the grammar) |
| Consumers | `ScriptParser`, `ScriptEngine`: `IToken` becomes `CSharpLexerToken`, the rest is the same logic |
| Removed | `CSharpLexer.cs` (generated), the `AntlrGrammars` project, `Antlr4.Runtime` from `TabularEditor` and from the installer |
| New projects, in `BPALib.sln` | `CSharpLexerOracle` (the original grammar with ANTLR, only for comparing), `CSharpLexerTests`, `CSharpLexerBenchmarks`. `TabularEditor` is a Windows Forms application, so the tests compile the lexer and `ScriptParser` in as linked files, which also runs them on macOS |

`CSharpLexer` was a public type of `TabularEditor`, and `MethodCall` exposed `IToken`, so code that used them directly
stops compiling.

The output generated from the grammar with ANTLR 4.6.6 is identical to the `CSharpLexer.cs` that was checked in, apart
from its namespace, so the oracle is what used to ship.

### What the lexer copies from the grammar

- Identifiers only have ASCII letters, digits and unicode escapes, and `_` can only start one: `a_b` is two identifiers
  and `é` is dropped. `__arglist1` is the keyword, then a number.
- The error recovery of the DAX lexer: `"a\q"` loses `"a\q` and then starts a string at the last quote.
- `/**/ x */` is one documentation comment, as that's the longest match, but `/**/ x` is a comment and then `x`.
- `/* never closed` is a division, a star, and what follows.
- Modes: a `{` in an interpolated string enters the default mode with a count of braces, and a colon with a `}` before
  the next `:` or `)` switches to the format string. This looks at chars, not tokens, so a `)` in a string counts.

**The only difference, on purpose.** The grammar's action for that colon doesn't stop at the end of the script: for
`$"{a:` it keeps reading until the index overflows, and throws `IndexOutOfRangeException` after about 2^31 reads, which
takes 13 to 27 seconds here. The new lexer throws the same
exception, after the same tokens, right away. In the differential tests the ANTLR side has an input stream that also
throws right away, and `ColonLooksPastTheEndUntilTheIndexOverflows` checks once that the real stream ends the same way.

The stacks can't underflow, as far as the tests and the reasoning go: each hole of a string pushes one level and each way
out pops one, whatever the mode. The lexer keeps the same operations, so it would throw the same `InvalidOperationException`
from `Stack<T>` for a stack of braces. No test reached it on either side. Only the colon's exception was observed.

### How parity was checked

| Test | Input |
|---|---|
| `EveryCharInEveryContext` | All 65,536 UTF-16 code units in 93 contexts, covering the five modes |
| `AllShortStringsOfTheStructuralChars` | Every string of 5 to 10 pieces or fewer over 17 alphabets: comments, strings, numbers, identifiers, interpolations in all modes, directives |
| `RandomStrings`, `RandomInterpolatedStrings` | 1,002,000 random strings, of which 500,000 start with an interpolated string |
| `EveryLiteralOfTheGrammar` | All keywords and operators in 22 contexts, and all their prefixes |
| `RealSources`, `RealSourcesCutAnywhere` | Every `.cs` file of the application and its tests, and every prefix of the 100 shortest |
| `TokenTypesMatchTheGrammar` | Every token type and channel against the grammar |
| `ConsumerTests` | `ScriptParser` against a copy of its ANTLR version: tokens, method calls, property references on all sources, `GetTypeAtPos` at every position of some scripts, and tokens kept after an exception |

Changing a line of the lexer on purpose (the end of a documentation comment, restoring `verbatium` after a nested string,
one white space char) fails between 3 and 5 of the 6 test cases of the random and literal tests.

### Benchmarks

BenchmarkDotNet, 8 iterations, x64 on the same VM as above, one run per runtime. Small is one step of a script (251 chars,
78 tokens), Medium 40 of them (17,979 chars, 6,055), Large 400 (182,079 chars, 60,415), with interpolated strings,
comments and literals. "List" is what `ScriptParser.Lex` does, "no list" only reads the tokens.

```bash
dotnet run -c Release --project CSharpLexerBenchmarks/CSharpLexerBenchmarks.csproj -f net8.0 -- --filter '*'
```

| Runtime | Size | ANTLR | Parlot list | Parlot no list | Time, list | Allocated, ANTLR → list |
|---|---|---:|---:|---:|---:|---:|
| .NET Framework 4.8 | Small | 57.5 µs | 6.6 µs | 5.3 µs | 8.8x faster | 27.9 KB → 8.6 KB |
| .NET Framework 4.8 | Medium | 3.14 ms | 0.56 ms | 0.40 ms | 5.6x faster | 1.5 MB → 526 KB |
| .NET Framework 4.8 | Large | 41.8 ms | 4.9 ms | 4.0 ms | 8.6x faster | 16.1 MB → 4.2 MB |
| .NET 8 x64 | Small | 27.5 µs | 2.9 µs | 1.4 µs | 9.4x faster | 26.6 KB → 8.6 KB |
| .NET 8 x64 | Medium | 1.51 ms | 0.31 ms | 0.10 ms | 4.9x faster | 1.5 MB → 525 KB |
| .NET 8 x64 | Large | 19.7 ms | 2.2 ms | 0.96 ms | 9.0x faster | 15.4 MB → 4.2 MB |

Without a list the lexer allocates 344 to 384 bytes, for the stacks of the interpolated strings.

The first script of a process, in a new process each time, with the JIT: `CSharpLexerBenchmarks --first antlr|parlot`,
Small, three runs. ANTLR also loads its automaton.

| Runtime | ANTLR | Parlot |
|---|---:|---:|
| .NET Framework 4.8 | 95 to 105 ms | 43 to 65 ms |
| .NET 8 x64 | 58 to 61 ms | 8 to 17 ms |

### What was run on Windows

On the same VM, with the same workarounds as above, and the ANTLR lexer of the oracle generated on a Mac and copied over.

| What | Result |
|---|---|
| `CSharpLexerTests` on net48 and net8.0 | 15 of 15 pass on each |
| `TOMWrapperTest` on net48 | 683 of 713 pass, the same outcome as `master` for each test |
| `TabularEditorTest` on net48 | 40 of 55 pass, the same outcome as `master` for each test, including the `ReplaceGlobalMethodCalls*` and script tests that go through `ScriptEngine` |
| `TabularEditor.exe` | Compiles, and no `Antlr4.Runtime.dll` in its output |

**Not run:** as above, the application by hand (the script editor's AutoComplete is only covered by comparing
`GetTypeAtPos`), a build with Visual Studio, and the installer. `Antlr4.Runtime.dll` was removed from `.vdproj` by editing
the file, and `Parlot.dll` still isn't listed in it, so a refresh of its detected dependencies in Visual Studio needs checking.

## What this leaves out

**Size.** `Parlot.dll` (net472) is 602 KB, where `Antlr4.Runtime.dll` was 215 KB. The lexers only need `Cursor` from it.
