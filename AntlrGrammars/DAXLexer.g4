lexer grammar DAXLexer;

channels { COMMENTS_CHANNEL, KEYWORD_CHANNEL }

SINGLE_LINE_COMMENT:     '//'  InputCharacter*    -> channel(COMMENTS_CHANNEL);
DELIMITED_COMMENT:       '/*'  .*? '*/'           -> channel(COMMENTS_CHANNEL);

WHITESPACES:   (Whitespace | NewLine)+            -> channel(HIDDEN);

ABS:                   [Aa][Bb][Ss]                                                                             -> channel(KEYWORD_CHANNEL);
ACOS:                  [Aa][Cc][Oo][Ss]                                                                         -> channel(KEYWORD_CHANNEL);
ACOSH:                 [Aa][Cc][Oo][Ss][Hh]                                                                     -> channel(KEYWORD_CHANNEL);
ACOT:                  [Aa][Cc][Oo][Tt]                                                                         -> channel(KEYWORD_CHANNEL);
ACOTH:                 [Aa][Cc][Oo][Tt][Hh]                                                                     -> channel(KEYWORD_CHANNEL);
ADDCOLUMNS:            [Aa][Dd][Dd][Cc][Oo][Ll][Uu][Mm][Nn][Ss]                                                 -> channel(KEYWORD_CHANNEL);
ADDMISSINGITEMS:       [Aa][Dd][Dd][Mm][Ii][Ss][Ss][Ii][Nn][Gg][Ii][Tt][Ee][Mm][Ss]                             -> channel(KEYWORD_CHANNEL);
ALL:                   [Aa][Ll][Ll]                                                                             -> channel(KEYWORD_CHANNEL);
ALLEXCEPT:             [Aa][Ll][Ll][Ee][Xx][Cc][Ee][Pp][Tt]                                                     -> channel(KEYWORD_CHANNEL);
ALLNOBLANKROW:         [Aa][Ll][Ll][Nn][Oo][Bb][Ll][Aa][Nn][Kk][Rr][Oo][Ww]                                     -> channel(KEYWORD_CHANNEL);
ALLSELECTED:           [Aa][Ll][Ll][Ss][Ee][Ll][Ee][Cc][Tt][Ee][Dd]                                             -> channel(KEYWORD_CHANNEL);
AND:                   [Aa][Nn][Dd]                                                                             -> channel(KEYWORD_CHANNEL);
ASIN:                  [Aa][Ss][Ii][Nn]                                                                         -> channel(KEYWORD_CHANNEL);
ASINH:                 [Aa][Ss][Ii][Nn][Hh]                                                                     -> channel(KEYWORD_CHANNEL);
ATAN:                  [Aa][Tt][Aa][Nn]                                                                         -> channel(KEYWORD_CHANNEL);
ATANH:                 [Aa][Tt][Aa][Nn][Hh]                                                                     -> channel(KEYWORD_CHANNEL);
AVERAGE:               [Aa][Vv][Ee][Rr][Aa][Gg][Ee]                                                             -> channel(KEYWORD_CHANNEL);
AVERAGEA:              [Aa][Vv][Ee][Rr][Aa][Gg][Ee][Aa]                                                         -> channel(KEYWORD_CHANNEL);
AVERAGEX:              [Aa][Vv][Ee][Rr][Aa][Gg][Ee][Xx]                                                         -> channel(KEYWORD_CHANNEL);
BETADIST:              [Bb][Ee][Tt][Aa]'.'[Dd][Ii][Ss][Tt]                                                         -> channel(KEYWORD_CHANNEL);
BETAINV:               [Bb][Ee][Tt][Aa]'.'[Ii][Nn][Vv]                                                             -> channel(KEYWORD_CHANNEL);
BLANK:                 [Bb][Ll][Aa][Nn][Kk]                                                                     -> channel(KEYWORD_CHANNEL);
CALCULATE:             [Cc][Aa][Ll][Cc][Uu][Ll][Aa][Tt][Ee]                                                     -> channel(KEYWORD_CHANNEL);
CALCULATETABLE:        [Cc][Aa][Ll][Cc][Uu][Ll][Aa][Tt][Ee][Tt][Aa][Bb][Ll][Ee]                                 -> channel(KEYWORD_CHANNEL);
CALENDAR:              [Cc][Aa][Ll][Ee][Nn][Dd][Aa][Rr]                                                         -> channel(KEYWORD_CHANNEL);
CALENDARAUTO:          [Cc][Aa][Ll][Ee][Nn][Dd][Aa][Rr][Aa][Uu][Tt][Oo]                                         -> channel(KEYWORD_CHANNEL);
CEILING:               [Cc][Ee][Ii][Ll][Ii][Nn][Gg]                                                             -> channel(KEYWORD_CHANNEL);
CHISQDIST:             [Cc][Hh][Ii][Ss][Qq]'.'[Dd][Ii][Ss][Tt]                                                     -> channel(KEYWORD_CHANNEL);
CHISQDISTRT:           [Cc][Hh][Ii][Ss][Qq]'.'[Dd][Ii][Ss][Tt]'.'[Rr][Tt]                                             -> channel(KEYWORD_CHANNEL);
CHISQINV:              [Cc][Hh][Ii][Ss][Qq]'.'[Ii][Nn][Vv]                                                         -> channel(KEYWORD_CHANNEL);
CHISQINVRT:            [Cc][Hh][Ii][Ss][Qq]'.'[Ii][Nn][Vv]'.'[Rr][Tt]                                                 -> channel(KEYWORD_CHANNEL);
CLOSINGBALANCEMONTH:   [Cc][Ll][Oo][Ss][Ii][Nn][Gg][Bb][Aa][Ll][Aa][Nn][Cc][Ee][Mm][Oo][Nn][Tt][Hh]             -> channel(KEYWORD_CHANNEL);
CLOSINGBALANCEQUARTER: [Cc][Ll][Oo][Ss][Ii][Nn][Gg][Bb][Aa][Ll][Aa][Nn][Cc][Ee][Qq][Uu][Aa][Rr][Tt][Ee][Rr]     -> channel(KEYWORD_CHANNEL);
CLOSINGBALANCEYEAR:    [Cc][Ll][Oo][Ss][Ii][Nn][Gg][Bb][Aa][Ll][Aa][Nn][Cc][Ee][Yy][Ee][Aa][Rr]                 -> channel(KEYWORD_CHANNEL);
COMBIN:                [Cc][Oo][Mm][Bb][Ii][Nn]                                                                 -> channel(KEYWORD_CHANNEL);
COMBINA:               [Cc][Oo][Mm][Bb][Ii][Nn][Aa]                                                             -> channel(KEYWORD_CHANNEL);
CONCATENATE:           [Cc][Oo][Nn][Cc][Aa][Tt][Ee][Nn][Aa][Tt][Ee]                                             -> channel(KEYWORD_CHANNEL);
CONCATENATEX:          [Cc][Oo][Nn][Cc][Aa][Tt][Ee][Nn][Aa][Tt][Ee][Xx]                                         -> channel(KEYWORD_CHANNEL);
CONFIDENCENORM:        [Cc][Oo][Nn][Ff][Ii][Dd][Ee][Nn][Cc][Ee]'.'[Nn][Oo][Rr][Mm]                                 -> channel(KEYWORD_CHANNEL);
CONFIDENCET:           [Cc][Oo][Nn][Ff][Ii][Dd][Ee][Nn][Cc][Ee]'.'[Tt]                                             -> channel(KEYWORD_CHANNEL);
CONTAINS:              [Cc][Oo][Nn][Tt][Aa][Ii][Nn][Ss]                                                         -> channel(KEYWORD_CHANNEL);
COS:                   [Cc][Oo][Ss]                                                                             -> channel(KEYWORD_CHANNEL);
COSH:                  [Cc][Oo][Ss][Hh]                                                                         -> channel(KEYWORD_CHANNEL);
COT:                   [Cc][Oo][Tt]                                                                             -> channel(KEYWORD_CHANNEL);
COTH:                  [Cc][Oo][Tt][Hh]                                                                         -> channel(KEYWORD_CHANNEL);
COUNT:                 [Cc][Oo][Uu][Nn][Tt]                                                                     -> channel(KEYWORD_CHANNEL);
COUNTA:                [Cc][Oo][Uu][Nn][Tt][Aa]                                                                 -> channel(KEYWORD_CHANNEL);
COUNTAX:               [Cc][Oo][Uu][Nn][Tt][Aa][Xx]                                                             -> channel(KEYWORD_CHANNEL);
COUNTBLANK:            [Cc][Oo][Uu][Nn][Tt][Bb][Ll][Aa][Nn][Kk]                                                 -> channel(KEYWORD_CHANNEL);
COUNTROWS:             [Cc][Oo][Uu][Nn][Tt][Rr][Oo][Ww][Ss]                                                     -> channel(KEYWORD_CHANNEL);
COUNTX:                [Cc][Oo][Uu][Nn][Tt][Xx]                                                                 -> channel(KEYWORD_CHANNEL);
CROSSFILTER:           [Cc][Rr][Oo][Ss][Ss][Ff][Ii][Ll][Tt][Ee][Rr]                                             -> channel(KEYWORD_CHANNEL);
CROSSJOIN:             [Cc][Rr][Oo][Ss][Ss][Jj][Oo][Ii][Nn]                                                     -> channel(KEYWORD_CHANNEL);
CURRENCY:              [Cc][Uu][Rr][Rr][Ee][Nn][Cc][Yy]                                                         -> channel(KEYWORD_CHANNEL);
CURRENTGROUP:          [Cc][Uu][Rr][Rr][Ee][Nn][Tt][Gg][Rr][Oo][Uu][Pp]                                         -> channel(KEYWORD_CHANNEL);
CUSTOMDATA:            [Cc][Uu][Ss][Tt][Oo][Mm][Dd][Aa][Tt][Aa]                                                 -> channel(KEYWORD_CHANNEL);
DATATABLE:             [Dd][Aa][Tt][Aa][Tt][Aa][Bb][Ll][Ee]                                                     -> channel(KEYWORD_CHANNEL);
DATE:                  [Dd][Aa][Tt][Ee]                                                                         -> channel(KEYWORD_CHANNEL);
DATEADD:               [Dd][Aa][Tt][Ee][Aa][Dd][Dd]                                                             -> channel(KEYWORD_CHANNEL);
DATEDIFF:              [Dd][Aa][Tt][Ee][Dd][Ii][Ff][Ff]                                                         -> channel(KEYWORD_CHANNEL);
DATESBETWEEN:          [Dd][Aa][Tt][Ee][Ss][Bb][Ee][Tt][Ww][Ee][Ee][Nn]                                         -> channel(KEYWORD_CHANNEL);
DATESINPERIOD:         [Dd][Aa][Tt][Ee][Ss][Ii][Nn][Pp][Ee][Rr][Ii][Oo][Dd]                                     -> channel(KEYWORD_CHANNEL);
DATESMTD:              [Dd][Aa][Tt][Ee][Ss][Mm][Tt][Dd]                                                         -> channel(KEYWORD_CHANNEL);
DATESQTD:              [Dd][Aa][Tt][Ee][Ss][Qq][Tt][Dd]                                                         -> channel(KEYWORD_CHANNEL);
DATESYTD:              [Dd][Aa][Tt][Ee][Ss][Yy][Tt][Dd]                                                         -> channel(KEYWORD_CHANNEL);
DATEVALUE:             [Dd][Aa][Tt][Ee][Vv][Aa][Ll][Uu][Ee]                                                     -> channel(KEYWORD_CHANNEL);
DAY:                   [Dd][Aa][Yy]                                                                             -> channel(KEYWORD_CHANNEL);
DEGREES:               [Dd][Ee][Gg][Rr][Ee][Ee][Ss]                                                             -> channel(KEYWORD_CHANNEL);
DISTINCT:              [Dd][Ii][Ss][Tt][Ii][Nn][Cc][Tt]                                                         -> channel(KEYWORD_CHANNEL);
DISTINCTCOUNT:         [Dd][Ii][Ss][Tt][Ii][Nn][Cc][Tt][Cc][Oo][Uu][Nn][Tt]                                     -> channel(KEYWORD_CHANNEL);
DIVIDE:                [Dd][Ii][Vv][Ii][Dd][Ee]                                                                 -> channel(KEYWORD_CHANNEL);
EARLIER:               [Ee][Aa][Rr][Ll][Ii][Ee][Rr]                                                             -> channel(KEYWORD_CHANNEL);
EARLIEST:              [Ee][Aa][Rr][Ll][Ii][Ee][Ss][Tt]                                                         -> channel(KEYWORD_CHANNEL);
EDATE:                 [Ee][Dd][Aa][Tt][Ee]                                                                     -> channel(KEYWORD_CHANNEL);
ENDOFMONTH:            [Ee][Nn][Dd][Oo][Ff][Mm][Oo][Nn][Tt][Hh]                                                 -> channel(KEYWORD_CHANNEL);
ENDOFQUARTER:          [Ee][Nn][Dd][Oo][Ff][Qq][Uu][Aa][Rr][Tt][Ee][Rr]                                         -> channel(KEYWORD_CHANNEL);
ENDOFYEAR:             [Ee][Nn][Dd][Oo][Ff][Yy][Ee][Aa][Rr]                                                     -> channel(KEYWORD_CHANNEL);
EOMONTH:               [Ee][Oo][Mm][Oo][Nn][Tt][Hh]                                                             -> channel(KEYWORD_CHANNEL);
EVEN:                  [Ee][Vv][Ee][Nn]                                                                         -> channel(KEYWORD_CHANNEL);
EXACT:                 [Ee][Xx][Aa][Cc][Tt]                                                                     -> channel(KEYWORD_CHANNEL);
EXCEPT:                [Ee][Xx][Cc][Ee][Pp][Tt]                                                                 -> channel(KEYWORD_CHANNEL);
EXP:                   [Ee][Xx][Pp]                                                                             -> channel(KEYWORD_CHANNEL);
EXPONDIST:             [Ee][Xx][Pp][Oo][Nn]'.'[Dd][Ii][Ss][Tt]                                                     -> channel(KEYWORD_CHANNEL);
FACT:                  [Ff][Aa][Cc][Tt]                                                                         -> channel(KEYWORD_CHANNEL);
FALSE:                 [Ff][Aa][Ll][Ss][Ee]                                                                     -> channel(KEYWORD_CHANNEL);
FILTER:                [Ff][Ii][Ll][Tt][Ee][Rr]                                                                 -> channel(KEYWORD_CHANNEL);
FILTERS:               [Ff][Ii][Ll][Tt][Ee][Rr][Ss]                                                             -> channel(KEYWORD_CHANNEL);
FIND:                  [Ff][Ii][Nn][Dd]                                                                         -> channel(KEYWORD_CHANNEL);
FIRSTDATE:             [Ff][Ii][Rr][Ss][Tt][Dd][Aa][Tt][Ee]                                                     -> channel(KEYWORD_CHANNEL);
FIRSTNONBLANK:         [Ff][Ii][Rr][Ss][Tt][Nn][Oo][Nn][Bb][Ll][Aa][Nn][Kk]                                     -> channel(KEYWORD_CHANNEL);
FIXED:                 [Ff][Ii][Xx][Ee][Dd]                                                                     -> channel(KEYWORD_CHANNEL);
FLOOR:                 [Ff][Ll][Oo][Oo][Rr]                                                                     -> channel(KEYWORD_CHANNEL);
FORMAT:                [Ff][Oo][Rr][Mm][Aa][Tt]                                                                 -> channel(KEYWORD_CHANNEL);
GCD:                   [Gg][Cc][Dd]                                                                             -> channel(KEYWORD_CHANNEL);
GENERATE:              [Gg][Ee][Nn][Ee][Rr][Aa][Tt][Ee]                                                         -> channel(KEYWORD_CHANNEL);
GENERATEALL:           [Gg][Ee][Nn][Ee][Rr][Aa][Tt][Ee][Aa][Ll][Ll]                                             -> channel(KEYWORD_CHANNEL);
GEOMEAN:               [Gg][Ee][Oo][Mm][Ee][Aa][Nn]                                                             -> channel(KEYWORD_CHANNEL);
GEOMEANX:              [Gg][Ee][Oo][Mm][Ee][Aa][Nn][Xx]                                                         -> channel(KEYWORD_CHANNEL);
GROUPBY:               [Gg][Rr][Oo][Uu][Pp][Bb][Yy]                                                             -> channel(KEYWORD_CHANNEL);
HASONEFILTER:          [Hh][Aa][Ss][Oo][Nn][Ee][Ff][Ii][Ll][Tt][Ee][Rr]                                         -> channel(KEYWORD_CHANNEL);
HASONEVALUE:           [Hh][Aa][Ss][Oo][Nn][Ee][Vv][Aa][Ll][Uu][Ee]                                             -> channel(KEYWORD_CHANNEL);
HOUR:                  [Hh][Oo][Uu][Rr]                                                                         -> channel(KEYWORD_CHANNEL);
IF:                    [Ii][Ff]                                                                                 -> channel(KEYWORD_CHANNEL);
IFERROR:               [Ii][Ff][Ee][Rr][Rr][Oo][Rr]                                                             -> channel(KEYWORD_CHANNEL);
IGNORE:                [Ii][Gg][Nn][Oo][Rr][Ee]                                                                 -> channel(KEYWORD_CHANNEL);
INT:                   [Ii][Nn][Tt]                                                                             -> channel(KEYWORD_CHANNEL);
INTERSECT:             [Ii][Nn][Tt][Ee][Rr][Ss][Ee][Cc][Tt]                                                     -> channel(KEYWORD_CHANNEL);
ISBLANK:               [Ii][Ss][Bb][Ll][Aa][Nn][Kk]                                                             -> channel(KEYWORD_CHANNEL);
ISCROSSFILTERED:       [Ii][Ss][Cc][Rr][Oo][Ss][Ss][Ff][Ii][Ll][Tt][Ee][Rr][Ee][Dd]                             -> channel(KEYWORD_CHANNEL);
ISEMPTY:               [Ii][Ss][Ee][Mm][Pp][Tt][Yy]                                                             -> channel(KEYWORD_CHANNEL);
ISERROR:               [Ii][Ss][Ee][Rr][Rr][Oo][Rr]                                                             -> channel(KEYWORD_CHANNEL);
ISEVEN:                [Ii][Ss][Ee][Vv][Ee][Nn]                                                                 -> channel(KEYWORD_CHANNEL);
ISFILTERED:            [Ii][Ss][Ff][Ii][Ll][Tt][Ee][Rr][Ee][Dd]                                                 -> channel(KEYWORD_CHANNEL);
ISLOGICAL:             [Ii][Ss][Ll][Oo][Gg][Ii][Cc][Aa][Ll]                                                     -> channel(KEYWORD_CHANNEL);
ISNONTEXT:             [Ii][Ss][Nn][Oo][Nn][Tt][Ee][Xx][Tt]                                                     -> channel(KEYWORD_CHANNEL);
ISNUMBER:              [Ii][Ss][Nn][Uu][Mm][Bb][Ee][Rr]                                                         -> channel(KEYWORD_CHANNEL);
ISOCEILING:            [Ii][Ss][Oo]'.'[Cc][Ee][Ii][Ll][Ii][Nn][Gg]                                                 -> channel(KEYWORD_CHANNEL);
ISODD:                 [Ii][Ss][Oo][Dd][Dd]                                                                     -> channel(KEYWORD_CHANNEL);
ISONORAFTER:           [Ii][Ss][Oo][Nn][Oo][Rr][Aa][Ff][Tt][Ee][Rr]                                             -> channel(KEYWORD_CHANNEL);
ISSUBTOTAL:            [Ii][Ss][Ss][Uu][Bb][Tt][Oo][Tt][Aa][Ll]                                                 -> channel(KEYWORD_CHANNEL);
ISTEXT:                [Ii][Ss][Tt][Ee][Xx][Tt]                                                                 -> channel(KEYWORD_CHANNEL);
KEEPFILTERS:           [Kk][Ee][Ee][Pp][Ff][Ii][Ll][Tt][Ee][Rr][Ss]                                             -> channel(KEYWORD_CHANNEL);
LASTDATE:              [Ll][Aa][Ss][Tt][Dd][Aa][Tt][Ee]                                                         -> channel(KEYWORD_CHANNEL);
LASTNONBLANK:          [Ll][Aa][Ss][Tt][Nn][Oo][Nn][Bb][Ll][Aa][Nn][Kk]                                         -> channel(KEYWORD_CHANNEL);
LCM:                   [Ll][Cc][Mm]                                                                             -> channel(KEYWORD_CHANNEL);
LEFT:                  [Ll][Ee][Ff][Tt]                                                                         -> channel(KEYWORD_CHANNEL);
LEN:                   [Ll][Ee][Nn]                                                                             -> channel(KEYWORD_CHANNEL);
LN:                    [Ll][Nn]                                                                                 -> channel(KEYWORD_CHANNEL);
LOG:                   [Ll][Oo][Gg]                                                                             -> channel(KEYWORD_CHANNEL);
LOG10:                 [Ll][Oo][Gg][11][00]                                                                     -> channel(KEYWORD_CHANNEL);
LOOKUPVALUE:           [Ll][Oo][Oo][Kk][Uu][Pp][Vv][Aa][Ll][Uu][Ee]                                             -> channel(KEYWORD_CHANNEL);
LOWER:                 [Ll][Oo][Ww][Ee][Rr]                                                                     -> channel(KEYWORD_CHANNEL);
MAX:                   [Mm][Aa][Xx]                                                                             -> channel(KEYWORD_CHANNEL);
MAXA:                  [Mm][Aa][Xx][Aa]                                                                         -> channel(KEYWORD_CHANNEL);
MAXX:                  [Mm][Aa][Xx][Xx]                                                                         -> channel(KEYWORD_CHANNEL);
MEDIAN:                [Mm][Ee][Dd][Ii][Aa][Nn]                                                                 -> channel(KEYWORD_CHANNEL);
MEDIANX:               [Mm][Ee][Dd][Ii][Aa][Nn][Xx]                                                             -> channel(KEYWORD_CHANNEL);
MID:                   [Mm][Ii][Dd]                                                                             -> channel(KEYWORD_CHANNEL);
MIN:                   [Mm][Ii][Nn]                                                                             -> channel(KEYWORD_CHANNEL);
MINA:                  [Mm][Ii][Nn][Aa]                                                                         -> channel(KEYWORD_CHANNEL);
MINUTE:                [Mm][Ii][Nn][Uu][Tt][Ee]                                                                 -> channel(KEYWORD_CHANNEL);
MINX:                  [Mm][Ii][Nn][Xx]                                                                         -> channel(KEYWORD_CHANNEL);
MOD:                   [Mm][Oo][Dd]                                                                             -> channel(KEYWORD_CHANNEL);
MONTH:                 [Mm][Oo][Nn][Tt][Hh]                                                                     -> channel(KEYWORD_CHANNEL);
MROUND:                [Mm][Rr][Oo][Uu][Nn][Dd]                                                                 -> channel(KEYWORD_CHANNEL);
NATURALINNERJOIN:      [Nn][Aa][Tt][Uu][Rr][Aa][Ll][Ii][Nn][Nn][Ee][Rr][Jj][Oo][Ii][Nn]                         -> channel(KEYWORD_CHANNEL);
NATURALLEFTOUTERJOIN:  [Nn][Aa][Tt][Uu][Rr][Aa][Ll][Ll][Ee][Ff][Tt][Oo][Uu][Tt][Ee][Rr][Jj][Oo][Ii][Nn]         -> channel(KEYWORD_CHANNEL);
NEXTDAY:               [Nn][Ee][Xx][Tt][Dd][Aa][Yy]                                                             -> channel(KEYWORD_CHANNEL);
NEXTMONTH:             [Nn][Ee][Xx][Tt][Mm][Oo][Nn][Tt][Hh]                                                     -> channel(KEYWORD_CHANNEL);
NEXTQUARTER:           [Nn][Ee][Xx][Tt][Qq][Uu][Aa][Rr][Tt][Ee][Rr]                                             -> channel(KEYWORD_CHANNEL);
NEXTYEAR:              [Nn][Ee][Xx][Tt][Yy][Ee][Aa][Rr]                                                         -> channel(KEYWORD_CHANNEL);
NOT:                   [Nn][Oo][Tt]                                                                             -> channel(KEYWORD_CHANNEL);
NOW:                   [Nn][Oo][Ww]                                                                             -> channel(KEYWORD_CHANNEL);
ODD:                   [Oo][Dd][Dd]                                                                             -> channel(KEYWORD_CHANNEL);
OPENINGBALANCEMONTH:   [Oo][Pp][Ee][Nn][Ii][Nn][Gg][Bb][Aa][Ll][Aa][Nn][Cc][Ee][Mm][Oo][Nn][Tt][Hh]             -> channel(KEYWORD_CHANNEL);
OPENINGBALANCEQUARTER: [Oo][Pp][Ee][Nn][Ii][Nn][Gg][Bb][Aa][Ll][Aa][Nn][Cc][Ee][Qq][Uu][Aa][Rr][Tt][Ee][Rr]     -> channel(KEYWORD_CHANNEL);
OPENINGBALANCEYEAR:    [Oo][Pp][Ee][Nn][Ii][Nn][Gg][Bb][Aa][Ll][Aa][Nn][Cc][Ee][Yy][Ee][Aa][Rr]                 -> channel(KEYWORD_CHANNEL);
OR:                    [Oo][Rr]                                                                                 -> channel(KEYWORD_CHANNEL);
PARALLELPERIOD:        [Pp][Aa][Rr][Aa][Ll][Ll][Ee][Ll][Pp][Ee][Rr][Ii][Oo][Dd]                                 -> channel(KEYWORD_CHANNEL);
PATH:                  [Pp][Aa][Tt][Hh]                                                                         -> channel(KEYWORD_CHANNEL);
PATHCONTAINS:          [Pp][Aa][Tt][Hh][Cc][Oo][Nn][Tt][Aa][Ii][Nn][Ss]                                         -> channel(KEYWORD_CHANNEL);
PATHITEM:              [Pp][Aa][Tt][Hh][Ii][Tt][Ee][Mm]                                                         -> channel(KEYWORD_CHANNEL);
PATHITEMREVERSE:       [Pp][Aa][Tt][Hh][Ii][Tt][Ee][Mm][Rr][Ee][Vv][Ee][Rr][Ss][Ee]                             -> channel(KEYWORD_CHANNEL);
PATHLENGTH:            [Pp][Aa][Tt][Hh][Ll][Ee][Nn][Gg][Tt][Hh]                                                 -> channel(KEYWORD_CHANNEL);
PERCENTILEEXC:         [Pp][Ee][Rr][Cc][Ee][Nn][Tt][Ii][Ll][Ee]'.'[Ee][Xx][Cc]                                     -> channel(KEYWORD_CHANNEL);
PERCENTILEINC:         [Pp][Ee][Rr][Cc][Ee][Nn][Tt][Ii][Ll][Ee]'.'[Ii][Nn][Cc]                                     -> channel(KEYWORD_CHANNEL);
PERCENTILEXEXC:        [Pp][Ee][Rr][Cc][Ee][Nn][Tt][Ii][Ll][Ee][Xx]'.'[Ee][Xx][Cc]                                 -> channel(KEYWORD_CHANNEL);
PERCENTILEXINC:        [Pp][Ee][Rr][Cc][Ee][Nn][Tt][Ii][Ll][Ee][Xx]'.'[Ii][Nn][Cc]                                 -> channel(KEYWORD_CHANNEL);
PERMUT:                [Pp][Ee][Rr][Mm][Uu][Tt]                                                                 -> channel(KEYWORD_CHANNEL);
PI:                    [Pp][Ii]                                                                                 -> channel(KEYWORD_CHANNEL);
POISSONDIST:           [Pp][Oo][Ii][Ss][Ss][Oo][Nn]'.'[Dd][Ii][Ss][Tt]                                             -> channel(KEYWORD_CHANNEL);
POWER:                 [Pp][Oo][Ww][Ee][Rr]                                                                     -> channel(KEYWORD_CHANNEL);
PREVIOUSDAY:           [Pp][Rr][Ee][Vv][Ii][Oo][Uu][Ss][Dd][Aa][Yy]                                             -> channel(KEYWORD_CHANNEL);
PREVIOUSMONTH:         [Pp][Rr][Ee][Vv][Ii][Oo][Uu][Ss][Mm][Oo][Nn][Tt][Hh]                                     -> channel(KEYWORD_CHANNEL);
PREVIOUSQUARTER:       [Pp][Rr][Ee][Vv][Ii][Oo][Uu][Ss][Qq][Uu][Aa][Rr][Tt][Ee][Rr]                             -> channel(KEYWORD_CHANNEL);
PREVIOUSYEAR:          [Pp][Rr][Ee][Vv][Ii][Oo][Uu][Ss][Yy][Ee][Aa][Rr]                                         -> channel(KEYWORD_CHANNEL);
PRODUCT:               [Pp][Rr][Oo][Dd][Uu][Cc][Tt]                                                             -> channel(KEYWORD_CHANNEL);
PRODUCTX:              [Pp][Rr][Oo][Dd][Uu][Cc][Tt][Xx]                                                         -> channel(KEYWORD_CHANNEL);
QUOTIENT:              [Qq][Uu][Oo][Tt][Ii][Ee][Nn][Tt]                                                         -> channel(KEYWORD_CHANNEL);
RADIANS:               [Rr][Aa][Dd][Ii][Aa][Nn][Ss]                                                             -> channel(KEYWORD_CHANNEL);
RAND:                  [Rr][Aa][Nn][Dd]                                                                         -> channel(KEYWORD_CHANNEL);
RANDBETWEEN:           [Rr][Aa][Nn][Dd][Bb][Ee][Tt][Ww][Ee][Ee][Nn]                                             -> channel(KEYWORD_CHANNEL);
RANKEQ:                [Rr][Aa][Nn][Kk]'.'[Ee][Qq]                                                                 -> channel(KEYWORD_CHANNEL);
RANKX:                 [Rr][Aa][Nn][Kk][Xx]                                                                     -> channel(KEYWORD_CHANNEL);
RELATED:               [Rr][Ee][Ll][Aa][Tt][Ee][Dd]                                                             -> channel(KEYWORD_CHANNEL);
RELATEDTABLE:          [Rr][Ee][Ll][Aa][Tt][Ee][Dd][Tt][Aa][Bb][Ll][Ee]                                         -> channel(KEYWORD_CHANNEL);
REPLACE:               [Rr][Ee][Pp][Ll][Aa][Cc][Ee]                                                             -> channel(KEYWORD_CHANNEL);
REPT:                  [Rr][Ee][Pp][Tt]                                                                         -> channel(KEYWORD_CHANNEL);
RIGHT:                 [Rr][Ii][Gg][Hh][Tt]                                                                     -> channel(KEYWORD_CHANNEL);
ROLLUP:                [Rr][Oo][Ll][Ll][Uu][Pp]                                                                 -> channel(KEYWORD_CHANNEL);
ROLLUPADDISSUBTOTAL:   [Rr][Oo][Ll][Ll][Uu][Pp][Aa][Dd][Dd][Ii][Ss][Ss][Uu][Bb][Tt][Oo][Tt][Aa][Ll]             -> channel(KEYWORD_CHANNEL);
ROLLUPGROUP:           [Rr][Oo][Ll][Ll][Uu][Pp][Gg][Rr][Oo][Uu][Pp]                                             -> channel(KEYWORD_CHANNEL);
ROLLUPISSUBTOTAL:      [Rr][Oo][Ll][Ll][Uu][Pp][Ii][Ss][Ss][Uu][Bb][Tt][Oo][Tt][Aa][Ll]                         -> channel(KEYWORD_CHANNEL);
ROUND:                 [Rr][Oo][Uu][Nn][Dd]                                                                     -> channel(KEYWORD_CHANNEL);
ROUNDDOWN:             [Rr][Oo][Uu][Nn][Dd][Dd][Oo][Ww][Nn]                                                     -> channel(KEYWORD_CHANNEL);
ROUNDUP:               [Rr][Oo][Uu][Nn][Dd][Uu][Pp]                                                             -> channel(KEYWORD_CHANNEL);
ROW:                   [Rr][Oo][Ww]                                                                             -> channel(KEYWORD_CHANNEL);
SAMEPERIODLASTYEAR:    [Ss][Aa][Mm][Ee][Pp][Ee][Rr][Ii][Oo][Dd][Ll][Aa][Ss][Tt][Yy][Ee][Aa][Rr]                 -> channel(KEYWORD_CHANNEL);
SAMPLE:                [Ss][Aa][Mm][Pp][Ll][Ee]                                                                 -> channel(KEYWORD_CHANNEL);
SEARCH:                [Ss][Ee][Aa][Rr][Cc][Hh]                                                                 -> channel(KEYWORD_CHANNEL);
SECOND:                [Ss][Ee][Cc][Oo][Nn][Dd]                                                                 -> channel(KEYWORD_CHANNEL);
SELECTCOLUMNS:         [Ss][Ee][Ll][Ee][Cc][Tt][Cc][Oo][Ll][Uu][Mm][Nn][Ss]                                     -> channel(KEYWORD_CHANNEL);
SIGN:                  [Ss][Ii][Gg][Nn]                                                                         -> channel(KEYWORD_CHANNEL);
SIN:                   [Ss][Ii][Nn]                                                                             -> channel(KEYWORD_CHANNEL);
SINH:                  [Ss][Ii][Nn][Hh]                                                                         -> channel(KEYWORD_CHANNEL);
SQRT:                  [Ss][Qq][Rr][Tt]                                                                         -> channel(KEYWORD_CHANNEL);
SQRTPI:                [Ss][Qq][Rr][Tt][Pp][Ii]                                                                 -> channel(KEYWORD_CHANNEL);
STARTOFMONTH:          [Ss][Tt][Aa][Rr][Tt][Oo][Ff][Mm][Oo][Nn][Tt][Hh]                                         -> channel(KEYWORD_CHANNEL);
STARTOFQUARTER:        [Ss][Tt][Aa][Rr][Tt][Oo][Ff][Qq][Uu][Aa][Rr][Tt][Ee][Rr]                                 -> channel(KEYWORD_CHANNEL);
STARTOFYEAR:           [Ss][Tt][Aa][Rr][Tt][Oo][Ff][Yy][Ee][Aa][Rr]                                             -> channel(KEYWORD_CHANNEL);
STDEVP:                [Ss][Tt][Dd][Ee][Vv]'.'[Pp]                                                                 -> channel(KEYWORD_CHANNEL);
STDEVS:                [Ss][Tt][Dd][Ee][Vv]'.'[Ss]                                                                 -> channel(KEYWORD_CHANNEL);
STDEVXP:               [Ss][Tt][Dd][Ee][Vv][Xx]'.'[Pp]                                                             -> channel(KEYWORD_CHANNEL);
STDEVXS:               [Ss][Tt][Dd][Ee][Vv][Xx]'.'[Ss]                                                             -> channel(KEYWORD_CHANNEL);
SUBSTITUTE:            [Ss][Uu][Bb][Ss][Tt][Ii][Tt][Uu][Tt][Ee]                                                 -> channel(KEYWORD_CHANNEL);
SUBSTITUTEWITHINDEX:   [Ss][Uu][Bb][Ss][Tt][Ii][Tt][Uu][Tt][Ee][Ww][Ii][Tt][Hh][Ii][Nn][Dd][Ee][Xx]             -> channel(KEYWORD_CHANNEL);
SUM:                   [Ss][Uu][Mm]                                                                             -> channel(KEYWORD_CHANNEL);
SUMMARIZE:             [Ss][Uu][Mm][Mm][Aa][Rr][Ii][Zz][Ee]                                                     -> channel(KEYWORD_CHANNEL);
SUMMARIZECOLUMNS:      [Ss][Uu][Mm][Mm][Aa][Rr][Ii][Zz][Ee][Cc][Oo][Ll][Uu][Mm][Nn][Ss]                         -> channel(KEYWORD_CHANNEL);
SUMX:                  [Ss][Uu][Mm][Xx]                                                                         -> channel(KEYWORD_CHANNEL);
SWITCH:                [Ss][Ww][Ii][Tt][Cc][Hh]                                                                 -> channel(KEYWORD_CHANNEL);
TAN:                   [Tt][Aa][Nn]                                                                             -> channel(KEYWORD_CHANNEL);
TANH:                  [Tt][Aa][Nn][Hh]                                                                         -> channel(KEYWORD_CHANNEL);
TIME:                  [Tt][Ii][Mm][Ee]                                                                         -> channel(KEYWORD_CHANNEL);
TIMEVALUE:             [Tt][Ii][Mm][Ee][Vv][Aa][Ll][Uu][Ee]                                                     -> channel(KEYWORD_CHANNEL);
TODAY:                 [Tt][Oo][Dd][Aa][Yy]                                                                     -> channel(KEYWORD_CHANNEL);
TOPN:                  [Tt][Oo][Pp][Nn]                                                                         -> channel(KEYWORD_CHANNEL);
TOTALMTD:              [Tt][Oo][Tt][Aa][Ll][Mm][Tt][Dd]                                                         -> channel(KEYWORD_CHANNEL);
TOTALQTD:              [Tt][Oo][Tt][Aa][Ll][Qq][Tt][Dd]                                                         -> channel(KEYWORD_CHANNEL);
TOTALYTD:              [Tt][Oo][Tt][Aa][Ll][Yy][Tt][Dd]                                                         -> channel(KEYWORD_CHANNEL);
TRIM:                  [Tt][Rr][Ii][Mm]                                                                         -> channel(KEYWORD_CHANNEL);
TRUE:                  [Tt][Rr][Uu][Ee]                                                                         -> channel(KEYWORD_CHANNEL);
TRUNC:                 [Tt][Rr][Uu][Nn][Cc]                                                                     -> channel(KEYWORD_CHANNEL);
UNICODE:               [Uu][Nn][Ii][Cc][Oo][Dd][Ee]                                                             -> channel(KEYWORD_CHANNEL);
UNION:                 [Uu][Nn][Ii][Oo][Nn]                                                                     -> channel(KEYWORD_CHANNEL);
UPPER:                 [Uu][Pp][Pp][Ee][Rr]                                                                     -> channel(KEYWORD_CHANNEL);
USERELATIONSHIP:       [Uu][Ss][Ee][Rr][Ee][Ll][Aa][Tt][Ii][Oo][Nn][Ss][Hh][Ii][Pp]                             -> channel(KEYWORD_CHANNEL);
USERNAME:              [Uu][Ss][Ee][Rr][Nn][Aa][Mm][Ee]                                                         -> channel(KEYWORD_CHANNEL);
VALUE:                 [Vv][Aa][Ll][Uu][Ee]                                                                     -> channel(KEYWORD_CHANNEL);
VALUES:                [Vv][Aa][Ll][Uu][Ee][Ss]                                                                 -> channel(KEYWORD_CHANNEL);
VARP:                  [Vv][Aa][Rr]'.'[Pp]                                                                         -> channel(KEYWORD_CHANNEL);
VARS:                  [Vv][Aa][Rr]'.'[Ss]                                                                         -> channel(KEYWORD_CHANNEL);
VARXP:                 [Vv][Aa][Rr][Xx]'.'[Pp]                                                                     -> channel(KEYWORD_CHANNEL);
VARXS:                 [Vv][Aa][Rr][Xx]'.'[Ss]                                                                     -> channel(KEYWORD_CHANNEL);
WEEKDAY:               [Ww][Ee][Ee][Kk][Dd][Aa][Yy]                                                             -> channel(KEYWORD_CHANNEL);
WEEKNUM:               [Ww][Ee][Ee][Kk][Nn][Uu][Mm]                                                             -> channel(KEYWORD_CHANNEL);
XIRR:                  [Xx][Ii][Rr][Rr]                                                                         -> channel(KEYWORD_CHANNEL);
XNPV:                  [Xx][Nn][Pp][Vv]                                                                         -> channel(KEYWORD_CHANNEL);
YEAR:                  [Yy][Ee][Aa][Rr]                                                                         -> channel(KEYWORD_CHANNEL);
YEARFRAC:              [Yy][Ee][Aa][Rr][Ff][Rr][Aa][Cc]                                                         -> channel(KEYWORD_CHANNEL);
VAR:                   [Vv][Aa][Rr]                                                                             -> channel(KEYWORD_CHANNEL);
RETURN:                [Rr][Ee][Tt][Uu][Rr][Nn]                                                                 -> channel(KEYWORD_CHANNEL);

INTEGER_LITERAL:       [0-9]+;
REAL_LITERAL:          [0-9]* '.' [0-9]+;
STRING_LITERAL:        '"' (~'"' | '""')* '"';
DATA:                  '{' .*? '}';
TABLE:                 '\'' (~["\'\r\n\u0085\u2028\u2029])* '\'';
COLUMN_OR_MEASURE:     '[' (~["\]\r\n\u0085\u2028\u2029])* ']';
TABLE_OR_VARIABLE:     IdentifierOrKeyword;


OPEN_PARENS:           '(';
CLOSE_PARENS:          ')';
COMMA:                 ',';
PLUS:                  '+';
MINUS:                 '-';
STAR:                  '*';
DIV:                   '/';
CARET:                 '^';
AMP:                   '&';
ASSIGNMENT:            '=';
LT:                    '<';
GT:                    '>';
OP_AND:                '&&';
OP_OR:                 '||';
OP_NE:                 '<>';
OP_LE:                 '<=';
OP_GE:                 '>=';


fragment InputCharacter:       ~[\r\n\u0085\u2028\u2029];

fragment NewLine
	: '\r\n' | '\r' | '\n'
	| '\u0085' // <Next Line CHARACTER (U+0085)>'
	| '\u2028' //'<Line Separator CHARACTER (U+2028)>'
	| '\u2029' //'<Paragraph Separator CHARACTER (U+2029)>'
	;

fragment Whitespace
	: UnicodeClassZS //'<Any Character With Unicode Class Zs>'
	| '\u0009' //'<Horizontal Tab Character (U+0009)>'
	| '\u000B' //'<Vertical Tab Character (U+000B)>'
	| '\u000C' //'<Form Feed Character (U+000C)>'
	;

fragment UnicodeClassZS
	: '\u0020' // SPACE
	| '\u00A0' // NO_BREAK SPACE
	| '\u1680' // OGHAM SPACE MARK
	| '\u180E' // MONGOLIAN VOWEL SEPARATOR
	| '\u2000' // EN QUAD
	| '\u2001' // EM QUAD
	| '\u2002' // EN SPACE
	| '\u2003' // EM SPACE
	| '\u2004' // THREE_PER_EM SPACE
	| '\u2005' // FOUR_PER_EM SPACE
	| '\u2006' // SIX_PER_EM SPACE
	| '\u2008' // PUNCTUATION SPACE
	| '\u2009' // THIN SPACE
	| '\u200A' // HAIR SPACE
	| '\u202F' // NARROW NO_BREAK SPACE
	| '\u3000' // IDEOGRAPHIC SPACE
	| '\u205F' // MEDIUM MATHEMATICAL SPACE
	;


fragment IdentifierOrKeyword
	: IdentifierStartCharacter IdentifierPartCharacter*
	;

fragment IdentifierStartCharacter
	: LetterCharacter
	| '_'
	;

fragment LetterCharacter
	: UnicodeClassLU
	| UnicodeClassLL
	;

fragment IdentifierPartCharacter
	: UnicodeClassLU
	| UnicodeClassLL
	| UnicodeClassND
	;

fragment UnicodeClassLU
	: '\u0041'..'\u005a'
	;

fragment UnicodeClassLL
	: '\u0061'..'\u007A'
	;

fragment UnicodeClassND
	: '\u0030'..'\u0039'
	;