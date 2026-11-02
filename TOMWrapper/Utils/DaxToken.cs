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

        internal DaxToken(DaxLexerToken lexerToken, IList<DaxToken> collection, int index)
        {
            Type = lexerToken.Type;
            TokenIndex = index;
            StartIndex = lexerToken.StartIndex;
            StopIndex = lexerToken.StopIndex;
            Text = lexerToken.Text;
            CommentOrWhitespace = lexerToken.Channel == DaxTokenizer.CommentsChannel || lexerToken.Channel == DaxTokenizer.HiddenChannel;
            Collection = collection;
        }

        public override string ToString()
        {
            var cow = CommentOrWhitespace ? "*** " : "";
            return $"Token #{TokenIndex}, {cow}{StartIndex}-{StopIndex}: DaxToken.{DaxTokenizer.GetSymbolicName(Type)}: \"{Text}\"";
        }
    }
}
