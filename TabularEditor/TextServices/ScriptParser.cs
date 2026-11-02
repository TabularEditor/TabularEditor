using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TextServices
{
    /// <summary>
    /// A simple parser for C# statements that are commonly used in the Script Editor.
    /// Supports variable assignments and lambda expressions. Used for AutoComplete in
    /// the Advanced Scripting window.
    /// </summary>
    public class ScriptParser
    {


        int FindTokenAtPos(IList<CSharpLexerToken> ts, int pos)
        {
            var l = 0; var r = ts.Count - 1;
            while (l <= r)
            {
                var m = (l + r) / 2;
                var t = ts[m];
                if (t.StopIndex < pos) l = m + 1;
                else if (t.StartIndex > pos) r = m - 1;
                else return m;
            }
            return -1;
        }

        private List<CSharpLexerToken> Ts;

        private int FindPrevID(IList<CSharpLexerToken> tokens, int fromIndex)
        {
            int terminator;
            int result = FindPrevID(tokens, fromIndex, out terminator);
            if (terminator != -1) return -1;
            return result;
        }

        private int FindPrevID(IList<CSharpLexerToken> tokens, int fromIndex, out int terminator)
        {
            terminator = -1;
            if (fromIndex <= 0) return -1;
            return FindValid(tokens, fromIndex-1, true, true, out terminator, DefaultTerminators);
        }

        private readonly int[] DefaultTerminators = new[] { CSharpToken.SEMICOLON, CSharpToken.OP_LAMBDA, CSharpToken.ASSIGNMENT };

        private int FindNext(IList<CSharpLexerToken> tokens, int fromIndex, int findTokenType)
        {
            int terminator = 0;
            return Find(tokens, fromIndex, findTokenType, false, true, out terminator);
        }
        
        private int FindValid(IList<CSharpLexerToken> tokens, int fromIndex, bool backward, bool skipParens, out int terminator, params int[] terminators)
        {
            terminator = -1;
            CSharpLexerToken token;
            var i = fromIndex;
            var parenDepth = 0;
            while (true)
            {
                token = tokens[i];
                if (terminators.Contains(token.Type) && (parenDepth == 0 || !skipParens)) { terminator = token.Type; return i; }
                else if (ValidObjectType(token.Type) && (parenDepth == 0 || !skipParens)) return i;
                else if (token.Type == CSharpToken.OPEN_PARENS || token.Type == CSharpToken.OPEN_BRACKET) parenDepth += (backward ? -1 : 1);
                else if (token.Type == CSharpToken.CLOSE_PARENS || token.Type == CSharpToken.CLOSE_BRACKET) parenDepth += backward ? 1 : -1;
                if (parenDepth < 0) return -1;
                if (backward) i--;
                else i++;
                if (i < 0 || i >= Ts.Count) return -1;
            }
        }
        
        private int Find(IList<CSharpLexerToken> tokens, int fromIndex, int findTokenType, bool backward, bool skipParens, out int terminator, params int[] terminators )
        {
            terminator = -1;
            CSharpLexerToken token;
            var i = fromIndex;
            var parenDepth = 0;
            while (true)
            {
                token = tokens[i];
                if (terminators.Contains(token.Type) && (parenDepth == 0 || !skipParens)) { terminator = token.Type; return i; }
                else if (token.Type == findTokenType && (parenDepth == 0 || !skipParens)) return i;
                else if (token.Type == CSharpToken.OPEN_PARENS || token.Type == CSharpToken.OPEN_BRACKET) parenDepth += (backward ? -1 : 1);
                else if (token.Type == CSharpToken.CLOSE_PARENS || token.Type == CSharpToken.CLOSE_BRACKET) parenDepth += backward ? 1 : -1;
                if (parenDepth < 0) return -1;
                if (backward) i--;
                else i++;
                if (i < 0 || i >= Ts.Count) return -1;
            }
        }

        private IList<CSharpLexerToken> FindPrevIDs(IList<CSharpLexerToken> tokens, ref int fromIndex)
        {
            var result = new List<CSharpLexerToken>();
            int t;
            var i = fromIndex;
            while(i != -1)
            {
                i = FindValid(tokens, i-1, true, false, out t, new[] { CSharpToken.SEMICOLON, CSharpToken.OP_LAMBDA });
                if (t == -1 && i != -1)
                {
                    result.Add(tokens[i]);
                }
                else break;
                fromIndex = i-1;
            }
            return result;
        }

        public Dictionary<string, Type> Types = new Dictionary<string, Type>();
        public Dictionary<string, Type> Classes = new Dictionary<string, Type>();
        private Dictionary<string, Type> localTypes = new Dictionary<string, Type>();

        public void Lex(string script)
        {
            Ts = new List<CSharpLexerToken>();
            CSharpTokenizer.GetAllTokens(script, Ts);
        }

        public IList<CSharpLexerToken> GetTokens()
        {
            return Ts.Where(t => t.Channel == CSharpTokenizer.DefaultChannel)
                .Select((t, index) => t.WithTokenIndex(index)).ToList();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "mx")]
        private MethodInfo GetLinqExtensionMethod(string name, Type collectionType)
        {
            var genericType = collectionType.GetGenericTypeDefinition();

            // TODO: This could be improved by filtering methods on those that have the exact number
            // and order of parameters, as the code that is being parsed. Is only a problem for overloaded
            // methods that return different types, so will rarely cause problems on the LINQ extension methods.
            var methods = typeof(Enumerable).GetMethods();
            var mx = methods.First();

            return typeof(Enumerable).GetMethods()
                        .Where(m => m.Name == name)
                        .FirstOrDefault(m =>
                        {
                            var par = m.GetParameters()[0].ParameterType;
                            return par.IsGenericType && par.GetGenericTypeDefinition() == genericType;
                        });
        }

        private Type GetPropPathType(IEnumerable<DotPath> paths, IList<CSharpLexerToken> list)
        {
            if (paths == null) return null;
            Type type = null;
            foreach(var path in paths)
            {
                var p = path.Name;
                if(type == null)
                {
                    switch (path.TokenType)
                    {
                        case CSharpToken.HEX_INTEGER_LITERAL:
                        case CSharpToken.INTEGER_LITERAL:
                            type = typeof(int); continue;
                        case CSharpToken.CHARACTER_LITERAL:
                            type = typeof(char); continue;
                        case CSharpToken.REAL_LITERAL:
                            type = typeof(float); continue;
                        case CSharpToken.REGULAR_STRING:
                        case CSharpToken.VERBATIUM_STRING:
                            type = typeof(string); continue;
                    }
                    if (localTypes.TryGetValue(p, out type)) continue;
                    if (!Types.TryGetValue(p, out type))
                    {
                        // Type not found. Perhaps we're looking at an assignment?
                        type = GetAssignmentType(list, path.TokenIndex);
                        // TODO: Recurse upwards
                        if (type == null) return null;
                    }
                    continue;
                } else {
                    // TODO: By looking at the next token, we could determine up front if we're dealing with a property or a method
                    // Property:
                    var propInfo = type.GetProperty(p);
                    if(propInfo != null)
                    {
                        if (path.Indexed)
                            type = propInfo.PropertyType.GetDefaultMembers().OfType<PropertyInfo>().FirstOrDefault().PropertyType;
                        else
                            type = propInfo.PropertyType;
                        continue;
                    }
                    // Instance method (this will likely fail in case of overloads):
                    var methodInfo = type.GetMethods().FirstOrDefault(m => m.Name == p);
                    if (methodInfo != null)
                    {
                        type = methodInfo.ReturnType;
                        continue;
                    }
                    // Extension methods of System.Linq.Enumerable:
                    var cType = type.Name == "IEnumerable`1" ? type : type.GetInterface("IEnumerable`1");
                    if (cType != null)
                    {
                        var exMethodInfo = GetLinqExtensionMethod(p, cType);
                        if (exMethodInfo != null)
                        {
                            if(exMethodInfo.GetGenericArguments().Length == 1)
                                type = exMethodInfo.MakeGenericMethod(cType.GetGenericArguments()[0]).ReturnType;
                            else if (exMethodInfo.GetGenericArguments().Length == 2)
                                type = exMethodInfo.MakeGenericMethod(cType.GetGenericArguments()[0], typeof(object)).ReturnType;
                            continue;
                        }
                    }

                    return null;
                }
            }
            return type;
        } 

        public Type GetTypeAtPos(int pos)
        {
            try
            {
                var list = Ts.Where(t => t.Channel == CSharpTokenizer.DefaultChannel).ToList();

                // Safe-cast (obj as Type)
                var tokenIx = FindTokenAtPos(list, pos - 1);
                if(tokenIx >= 3 
                    && list[tokenIx].Type == CSharpToken.DOT 
                    && list[tokenIx - 1].Type == CSharpToken.CLOSE_PARENS 
                    && list[tokenIx - 2].Type == CSharpToken.IDENTIFIER
                    && list[tokenIx - 3].Type == CSharpToken.AS)
                {
                    if (Classes.TryGetValue(list[tokenIx - 2].Text, out Type found)) return found;
                }

                localTypes.Clear();
                var p = GetPropPathFrom(pos, list);
                var type = GetPropPathType(p, list);
                return type;
            } catch
            {
                return null;
            }
        }

        private Type GetAssignmentType(IList<CSharpLexerToken> list, int identifierToken)
        {
            // Only search tokens that occurred before the last terminator:
            int terminatorType;
            var lastTerminator = Find(list, identifierToken, CSharpToken.SEMICOLON, true, false, out terminatorType, new[] { CSharpToken.OP_LAMBDA, CSharpToken.SEMICOLON });

            if (lastTerminator <= 0) return null;

            for(int i = lastTerminator - 1; i >= 0; i--)
            {
                if(list[i].Type == CSharpToken.IDENTIFIER && list[i].Text == list[identifierToken].Text && list[i+1].Type == CSharpToken.ASSIGNMENT)
                {
                    var assignmentEnd = FindNext(list, i, CSharpToken.SEMICOLON);
                    if (assignmentEnd != -1)
                    {
                        if(assignmentEnd > 2 && list[assignmentEnd - 2].Type == CSharpToken.AS && list[assignmentEnd - 1].Type == CSharpToken.IDENTIFIER && Classes.TryGetValue(list[assignmentEnd - 1].Text, out Type result))
                        {
                            return result;
                        }

                        var propPath = GetPropPathFrom(list[assignmentEnd].StopIndex+1, list);
                        return GetPropPathType(propPath, list);
                    }
                }
            }
            return null;
        }

        bool ValidObjectType(int objectType)
        {
            return Array.Exists(objectTypes, t => t == objectType);
        }
        int[] objectTypes = new[] { CSharpToken.IDENTIFIER, CSharpToken.CHARACTER_LITERAL, CSharpToken.INTEGER_LITERAL, CSharpToken.REAL_LITERAL, CSharpToken.HEX_INTEGER_LITERAL, CSharpToken.REGULAR_STRING, CSharpToken.VERBATIUM_STRING };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "method")]
        private Stack<DotPath> GetPropPathFrom(int pos, IList<CSharpLexerToken> list)
        {
            var i = FindTokenAtPos(list, pos - 1);
            if(i >= 0 && !ValidObjectType(list[i].Type)) i = FindPrevID(list, i);

            if (i == -1 || list[i].Type == CSharpToken.Eof) return null;

            var token = list[i];
            var propStack = new Stack<DotPath>();

            while (true)
            {
                propStack.Push(new DotPath()
                {
                    Name = token.Text,
                    TokenIndex = i,
                    Indexed = i < list.Count && list[i + 1].Type == CSharpToken.OPEN_BRACKET,
                    TokenType = token.Type
                });
                int t;
                i = FindPrevID(list, i, out t);
                if (t == CSharpToken.OP_LAMBDA)
                {
                    // LINQ lambda methods - supports only one parameter, which is assumed to be of the same type as the collection upon which it is called:
                    var lambdaArgs = FindPrevIDs(list, ref i);
                    if (list[i - 1].Type == CSharpToken.OPEN_PARENS) i--;
                    if(i > 1 && list[i].Type == CSharpToken.OPEN_PARENS && list[i-1].Type == CSharpToken.IDENTIFIER)
                    {
                        var method = list[i - 1].Text;
                        var p = GetPropPathFrom(list[i - 1].StartIndex - 1, list); // <-- This must include variable assignments
                        if (p == null || p.Count == 0) break;
                        var pType = GetPropPathType(p, list);
                        if (pType == null) break;
                        if (!pType.IsGenericType && pType.BaseType.IsGenericType) pType = pType.BaseType;
                        var lambdaType = pType.GetGenericArguments().FirstOrDefault();
                        if (lambdaType != null) localTypes.Add(lambdaArgs[0].Text, lambdaType);
                    }
                    break;
                }
                if (t != -1) break;
                if (i == -1) break;
                token = list[i];
            }

            return propStack;

        }

        public struct DotPath
        {
            public string Name;
            public bool Indexed;
            public int TokenIndex;
            public int TokenType;
        }

    }

    public class MethodCall
    {
        public CSharpLexerToken StartToken;
        public CSharpLexerToken StopToken;
        public int ParamCount;
    }

    public static class ScriptParserHelper
    {
        public static List<MethodCall> FindGlobalPropertyRefs(this IList<CSharpLexerToken> tokens, HashSet<string> names)
        {
            var result = new List<MethodCall>();
            for(int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == CSharpToken.IDENTIFIER && names.Contains(tokens[i].Text))
                {
                    for(int j = i - 1; j >= 0; j--)
                    {
                        if (tokens[j].Channel == CSharpTokenizer.HiddenChannel) continue;
                        if (tokens[j].Type == CSharpToken.DOT) break;
                        result.Add(new MethodCall { StartToken = tokens[i], StopToken = tokens[i] });
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Loops through the tokens and returns all tokens matching the pattern: IDENTIFIER(*)
        /// These indicate method calls.
        /// </summary>
        /// <param name="tokens">The current tokens to scan through (as outputted from GetTokens)</param>
        /// <param name="methodName">An optional method name to search for</param>
        /// <param name="parameters">The number of parameters the method should have. Use -1 for any number of parameters.</param>
        /// <returns></returns>
        public static List<MethodCall> FindMethodCall(this IList<CSharpLexerToken> tokens, string methodName = null, int parameters = -1)
        {
            var result = new List<MethodCall>();

            for(int i = 0; i < tokens.Count; i++)
            {
                if(tokens[i].Type == CSharpToken.IDENTIFIER &&
                    (methodName == null || tokens[i].Text == methodName) && 
                    i < tokens.Count - 2 && 
                    tokens[i+1].Type == CSharpToken.OPEN_PARENS)
                {
                    // Find the corresponding close parens:
                    var parenDepth = 0;
                    var paramCount = 0;
                    for(int ix = i + 2; ix < tokens.Count; ix++)
                    {
                        if (tokens[ix].Type != CSharpToken.CLOSE_PARENS && paramCount == 0) paramCount++;
                        if (tokens[ix].Type == CSharpToken.COMMA && parenDepth == 0) paramCount++;
                        if (tokens[ix].Type == CSharpToken.OPEN_PARENS) parenDepth++;
                        if (tokens[ix].Type == CSharpToken.CLOSE_PARENS)
                        {
                            if(parenDepth == 0)
                            {
                                if(parameters == -1 || paramCount == parameters) result.Add(new MethodCall { StartToken = tokens[i], StopToken = tokens[ix], ParamCount = paramCount });
                                break;
                            }
                            parenDepth--;
                        }
                    }
                }
            }
            return result;
        }
    }
}
