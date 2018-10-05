/* Copyright(c) 2012-2017 The ANTLR Project.All rights reserved.

* Use of this file is governed by the BSD 3-clause license that
* can be found in the LICENSE.txt file in the project root.

*/
using System;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime;

namespace TabularEditor.TextServices
{
    /// <summary>
    /// This class supports case-insensitive lexing by wrapping an existing
    /// <see cref="ICharStream"/> and forcing the lexer to see uppercase characters. 
    /// Grammar literals should then be uppercase, such as 'BEGIN'. The text of the character
    /// stream is unaffected. Example: input 'BeGiN' would match lexer rule
    /// 'BEGIN' but getText() would return 'BeGiN'. In addition, the 'convertFromNonUs'
    /// constructor parameter indicates whether semicolons (;) are converted to commas (,), and
    /// commas (,) converted to periods (.) indicating a non-US DAX locale.
    /// </summary>
    public class DAXCharStream : ICharStream
    {
        private ICharStream stream;
        private bool convertFromNonUS;

        public DAXCharStream(string dax, bool convertFromNonUs):this(new AntlrInputStream(dax), convertFromNonUs)
        {

        }

        /// <summary>
        /// Constructs a new CaseChangingCharStream wrapping the given <paramref name="stream"/> forcing
        /// all characters to upper case or lower case.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="upper">If true force each symbol to upper case, otherwise force to lower.</param>
        public DAXCharStream(ICharStream stream, bool convertFromNonUs)
        {
            this.stream = stream;
            this.convertFromNonUS = convertFromNonUs;
        }

        public int Index
        {
            get
            {
                return stream.Index;
            }
        }

        public int Size
        {
            get
            {
                return stream.Size;
            }
        }

        public string SourceName
        {
            get
            {
                return stream.SourceName;
            }
        }

        public void Consume()
        {
            stream.Consume();
        }

        [return: NotNull]
        public string GetText(Interval interval)
        {
            return stream.GetText(interval);
        }

        public int La(int i)
        {
            int c = stream.La(i);

            if (c <= 0)
            {
                return c;
            }

            char o = (char)c;

            if(convertFromNonUS)
            {
                if (o == ';') return (int)',';
                if (o == ',') return (int)'.';
            }
            return (int)char.ToUpperInvariant(o);
        }

        public int Mark()
        {
            return stream.Mark();
        }

        public void Release(int marker)
        {
            stream.Release(marker);
        }

        public void Seek(int index)
        {
            stream.Seek(index);
        }
    }
}