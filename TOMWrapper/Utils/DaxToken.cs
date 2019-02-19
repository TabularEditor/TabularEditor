using Antlr4.Runtime;
using System.Collections.Generic;
using TabularEditor.TextServices;

namespace TabularEditor.TOMWrapper.Utils
{
    public class DaxToken
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
            CommentOrWhitespace = antlrToken.Channel == 1 || antlrToken.Channel == 2;
            Collection = collection;
        }

        public override string ToString()
        {
            var cow = CommentOrWhitespace ? "*** " : "";
            return $"Token #{TokenIndex}, {cow}{StartIndex}-{StopIndex}: DaxToken.{DAXLexer.DefaultVocabulary.GetSymbolicName(Type)}: \"{Text}\"";
        }

        public const int
        SINGLE_LINE_COMMENT = 1, DELIMITED_COMMENT = 2, WHITESPACES = 3, ABS = 4, ACOS = 5,
        ACOSH = 6, ACOT = 7, ACOTH = 8, ADDCOLUMNS = 9, ADDMISSINGITEMS = 10, ALL = 11, ALLEXCEPT = 12,
        ALLNOBLANKROW = 13, ALLSELECTED = 14, AND = 15, ASIN = 16, ASINH = 17, ATAN = 18,
        ATANH = 19, AVERAGE = 20, AVERAGEA = 21, AVERAGEX = 22, BETADIST = 23, BETAINV = 24,
        BLANK = 25, CALCULATE = 26, CALCULATETABLE = 27, CALENDAR = 28, CALENDARAUTO = 29,
        CEILING = 30, CHISQDIST = 31, CHISQDISTRT = 32, CHISQINV = 33, CHISQINVRT = 34,
        CLOSINGBALANCEMONTH = 35, CLOSINGBALANCEQUARTER = 36, CLOSINGBALANCEYEAR = 37,
        COMBIN = 38, COMBINA = 39, CONCATENATE = 40, CONCATENATEX = 41, CONFIDENCENORM = 42,
        CONFIDENCET = 43, CONTAINS = 44, COS = 45, COSH = 46, COT = 47, COTH = 48, COUNT = 49,
        COUNTA = 50, COUNTAX = 51, COUNTBLANK = 52, COUNTROWS = 53, COUNTX = 54, CROSSFILTER = 55,
        CROSSJOIN = 56, CURRENCY = 57, CURRENTGROUP = 58, CUSTOMDATA = 59, DATATABLE = 60,
        DATE = 61, DATEADD = 62, DATEDIFF = 63, DATESBETWEEN = 64, DATESINPERIOD = 65, DATESMTD = 66,
        DATESQTD = 67, DATESYTD = 68, DATEVALUE = 69, DAY = 70, DEGREES = 71, DISTINCT = 72,
        DISTINCTCOUNT = 73, DIVIDE = 74, EARLIER = 75, EARLIEST = 76, EDATE = 77, ENDOFMONTH = 78,
        ENDOFQUARTER = 79, ENDOFYEAR = 80, EOMONTH = 81, EVEN = 82, EXACT = 83, EXCEPT = 84,
        EXP = 85, EXPONDIST = 86, FACT = 87, FALSE = 88, FILTER = 89, FILTERS = 90, FIND = 91,
        FIRSTDATE = 92, FIRSTNONBLANK = 93, FIXED = 94, FLOOR = 95, FORMAT = 96, GCD = 97,
        GENERATE = 98, GENERATEALL = 99, GEOMEAN = 100, GEOMEANX = 101, GROUPBY = 102, HASONEFILTER = 103,
        HASONEVALUE = 104, HOUR = 105, IF = 106, IFERROR = 107, IGNORE = 108, INT = 109, INTERSECT = 110,
        ISBLANK = 111, ISCROSSFILTERED = 112, ISEMPTY = 113, ISERROR = 114, ISEVEN = 115,
        ISFILTERED = 116, ISLOGICAL = 117, ISNONTEXT = 118, ISNUMBER = 119, ISOCEILING = 120,
        ISODD = 121, ISONORAFTER = 122, ISSUBTOTAL = 123, ISTEXT = 124, KEEPFILTERS = 125,
        LASTDATE = 126, LASTNONBLANK = 127, LCM = 128, LEFT = 129, LEN = 130, LN = 131, LOG = 132,
        LOG10 = 133, LOOKUPVALUE = 134, LOWER = 135, MAX = 136, MAXA = 137, MAXX = 138, MEDIAN = 139,
        MEDIANX = 140, MID = 141, MIN = 142, MINA = 143, MINUTE = 144, MINX = 145, MOD = 146,
        MONTH = 147, MROUND = 148, NATURALINNERJOIN = 149, NATURALLEFTOUTERJOIN = 150,
        NEXTDAY = 151, NEXTMONTH = 152, NEXTQUARTER = 153, NEXTYEAR = 154, NOT = 155, NOW = 156,
        ODD = 157, OPENINGBALANCEMONTH = 158, OPENINGBALANCEQUARTER = 159, OPENINGBALANCEYEAR = 160,
        OR = 161, PARALLELPERIOD = 162, PATH = 163, PATHCONTAINS = 164, PATHITEM = 165,
        PATHITEMREVERSE = 166, PATHLENGTH = 167, PERCENTILEEXC = 168, PERCENTILEINC = 169,
        PERCENTILEXEXC = 170, PERCENTILEXINC = 171, PERMUT = 172, PI = 173, POISSONDIST = 174,
        POWER = 175, PREVIOUSDAY = 176, PREVIOUSMONTH = 177, PREVIOUSQUARTER = 178, PREVIOUSYEAR = 179,
        PRODUCT = 180, PRODUCTX = 181, QUOTIENT = 182, RADIANS = 183, RAND = 184, RANDBETWEEN = 185,
        RANKEQ = 186, RANKX = 187, RELATED = 188, RELATEDTABLE = 189, REPLACE = 190, REPT = 191,
        RIGHT = 192, ROLLUP = 193, ROLLUPADDISSUBTOTAL = 194, ROLLUPGROUP = 195, ROLLUPISSUBTOTAL = 196,
        ROUND = 197, ROUNDDOWN = 198, ROUNDUP = 199, ROW = 200, SAMEPERIODLASTYEAR = 201,
        SAMPLE = 202, SEARCH = 203, SECOND = 204, SELECTCOLUMNS = 205, SIGN = 206, SIN = 207,
        SINH = 208, SQRT = 209, SQRTPI = 210, STARTOFMONTH = 211, STARTOFQUARTER = 212,
        STARTOFYEAR = 213, STDEVP = 214, STDEVS = 215, STDEVXP = 216, STDEVXS = 217, SUBSTITUTE = 218,
        SUBSTITUTEWITHINDEX = 219, SUM = 220, SUMMARIZE = 221, SUMMARIZECOLUMNS = 222,
        SUMX = 223, SWITCH = 224, TAN = 225, TANH = 226, TIME = 227, TIMEVALUE = 228, TODAY = 229,
        TOPN = 230, TOTALMTD = 231, TOTALQTD = 232, TOTALYTD = 233, TRIM = 234, TRUE = 235,
        TRUNC = 236, UNICODE = 237, UNION = 238, UPPER = 239, USERELATIONSHIP = 240, USERNAME = 241,
        VALUE = 242, VALUES = 243, VARP = 244, VARS = 245, VARXP = 246, VARXS = 247, WEEKDAY = 248,
        WEEKNUM = 249, XIRR = 250, XNPV = 251, YEAR = 252, YEARFRAC = 253, VAR = 254, RETURN = 255,
        CONTAINSROW = 256, ERROR = 257, USEROBJECTID = 258, USERPRINCIPALNAME = 259, UNICHAR = 260,
        DETAILROWS = 261, GENERATESERIES = 262, SELECTEDVALUE = 263, TREATAS = 264, TOPNSKIP = 265,
        QUARTER = 266, BOTH = 267, NONE = 268, ONEWAY = 269, INTEGER_LITERAL = 270, REAL_LITERAL = 271,
        STRING_LITERAL = 272, DATA = 273, TABLE = 274, COLUMN_OR_MEASURE = 275, TABLE_OR_VARIABLE = 276,
        OPEN_PARENS = 277, CLOSE_PARENS = 278, COMMA = 279, PLUS = 280, MINUS = 281, STAR = 282,
        DIV = 283, CARET = 284, AMP = 285, ASSIGNMENT = 286, LT = 287, GT = 288, OP_AND = 289,
        OP_OR = 290, OP_NE = 291, OP_LE = 292, OP_GE = 293;
    }
}
