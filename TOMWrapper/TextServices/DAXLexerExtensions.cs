using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TextServices
{
    public static class DAXLexerExtensions
    {
        /// <summary>
        /// Returns all non-whitespace, non-comment tokens
        /// </summary>
        /// <param name="lexer"></param>
        /// <returns></returns>
        public static IEnumerable<IToken> GetDefaultTokens(this DAXLexer lexer)
        {
            var token = lexer.NextToken();
            while(token.Type != -1)
            {
                if(token.Channel == DAXLexer.DefaultTokenChannel || token.Channel == DAXLexer.KEYWORD_CHANNEL) yield return token;
                token = lexer.NextToken();
            }
        }
    }
}
