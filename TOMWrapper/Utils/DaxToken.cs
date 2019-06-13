using Antlr4.Runtime;
using System.Collections.Generic;
using TabularEditor.TextServices;

namespace TabularEditor.TOMWrapper.Utils
{
    public partial class DaxToken
    {
        public int Type { get; private set; }
        public int TokenIndex { get; private set; }
        public int StartIndex { get; private set; }
        public int StopIndex { get; private set; }
        public string Text { get; private set; }

        private readonly IList<DaxToken> Collection;

        /// <summary>
        /// Get the next token
        /// </summary>
        public DaxToken Prev
        {
            get
            {
                var ix = TokenIndex - 1;
                while(ix >= 0)
                {
                    var token = Collection[ix];
                    if (!token.CommentOrWhitespace) return token;
                    ix--;
                }
                return null;
            }
        }

        public DaxToken Next
        {
            get
            {
                var ix = TokenIndex + 1;
                while (ix < Collection.Count)
                {
                    var token = Collection[ix];
                    if (!token.CommentOrWhitespace) return token;
                    ix++;
                }
                return null;
            }
        }

        public bool CommentOrWhitespace { get; private set; }

        internal DaxToken(IToken antlrToken, IList<DaxToken> collection, int index)
        {
            Type = antlrToken.Type;
            TokenIndex = index;
            StartIndex = antlrToken.StartIndex;
            StopIndex = antlrToken.StopIndex;
            Text = antlrToken.Text;
            CommentOrWhitespace = antlrToken.Channel == DAXLexer.COMMENTS_CHANNEL || antlrToken.Channel == DAXLexer.Hidden;
            Collection = collection;
        }

        public override string ToString()
        {
            var cow = CommentOrWhitespace ? "*** " : "";
            return $"Token #{TokenIndex}, {cow}{StartIndex}-{StopIndex}: DaxToken.{DAXLexer.DefaultVocabulary.GetSymbolicName(Type)}: \"{Text}\"";
        }
    }
}
