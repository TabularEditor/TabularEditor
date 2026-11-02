using System;

namespace TabularEditor.TextServices
{
    /// <summary>
    /// The keywords and functions of DAX, looked up without allocating from the upper-cased text of an identifier.
    /// </summary>
    internal static partial class DaxKeywords
    {
        private const int BucketCount = 2048;
        private const uint BucketMask = BucketCount - 1;

        // Open addressing over the keywords that have the shape of an identifier: index in Texts + 1, or 0 when empty.
        private static readonly short[] Buckets;

        // Keywords with a period, such as STDEV.P: an identifier ends at the period so these are only tried after one.
        private static readonly int[] DottedKeywords;

        private static readonly int MaxLength;

        // In a constructor, which runs after the initializers of the generated tables
        static DaxKeywords()
        {
            Buckets = BuildBuckets();
            DottedKeywords = BuildDottedKeywords();
            MaxLength = GetMaxLength();
        }

        private static short[] BuildBuckets()
        {
            var buckets = new short[BucketCount];
            for (var i = 0; i < Texts.Length; i++)
            {
                if (Texts[i].IndexOf('.') >= 0) continue;

                var hash = 0u;
                foreach (var c in Texts[i]) hash = Hash(hash, c);

                var bucket = hash & BucketMask;
                while (buckets[bucket] != 0) bucket = (bucket + 1) & BucketMask;
                buckets[bucket] = (short)(i + 1);
            }
            return buckets;
        }

        private static int[] BuildDottedKeywords()
        {
            var count = 0;
            foreach (var text in Texts) if (text.IndexOf('.') >= 0) count++;

            var result = new int[count];
            count = 0;
            for (var i = 0; i < Texts.Length; i++)
            {
                if (Texts[i].IndexOf('.') >= 0) result[count++] = i;
            }
            return result;
        }

        private static int GetMaxLength()
        {
            var result = 0;
            foreach (var text in Texts) result = Math.Max(result, text.Length);
            return result;
        }

        internal static uint Hash(uint hash, char upper) => hash * 31 + upper;

        /// <summary>
        /// Returns the type of the keyword spelled by the identifier, or 0 when it isn't a keyword.
        /// </summary>
        /// <param name="identifier">The text of the identifier.</param>
        /// <param name="hash">The result of <see cref="Hash"/> over the upper-cased text of the identifier.</param>
        internal static int Find(ReadOnlySpan<char> identifier, uint hash)
        {
            if (identifier.Length > MaxLength) return 0;

            var bucket = hash & BucketMask;
            while (true)
            {
                var entry = Buckets[bucket];
                if (entry == 0) return 0;
                if (EqualsUpper(identifier, Texts[entry - 1])) return Types[entry - 1];
                bucket = (bucket + 1) & BucketMask;
            }
        }

        private static bool EqualsUpper(ReadOnlySpan<char> identifier, string keyword)
        {
            if (identifier.Length != keyword.Length) return false;
            for (var i = 0; i < identifier.Length; i++)
            {
                if (DaxTokenizer.ToUpper(identifier[i]) != keyword[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Finds the longest keyword with a period that <paramref name="text"/> starts with. The first period of
        /// <paramref name="text"/>, which may be a decimal comma in the non-US format, is at <paramref name="periodIndex"/>.
        /// </summary>
        internal static int FindDotted(ReadOnlySpan<char> text, int periodIndex, bool nonUs, out int length)
        {
            var type = 0;
            length = 0;
            foreach (var index in DottedKeywords)
            {
                var keyword = Texts[index];
                if (keyword.Length <= Math.Max(length, periodIndex) || keyword.Length > text.Length || keyword[periodIndex] != '.') continue;

                var matches = true;
                for (var i = 0; matches && i < keyword.Length; i++)
                {
                    var c = text[i];
                    matches = (c == ',' && nonUs ? '.' : DaxTokenizer.ToUpper(c)) == keyword[i];
                }

                if (matches)
                {
                    type = Types[index];
                    length = keyword.Length;
                }
            }
            return type;
        }
    }
}
