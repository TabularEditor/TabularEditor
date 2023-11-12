lexer grammar DAXLexer;

channels { COMMENTS_CHANNEL, KEYWORD_CHANNEL }

SINGLE_LINE_COMMENT:     ( '//' | '--' )  InputCharacter*    -> channel(COMMENTS_CHANNEL);
DELIMITED_COMMENT:       '/*' ( DELIMITED_COMMENT | '/' ~'*' | ~'/' )*? ( '*/' | EOF )  -> channel(COMMENTS_CHANNEL);

WHITESPACES:   (Whitespace | NewLine)+            -> channel(HIDDEN);

// Note: Must use DAXCharStream as input, to make the lexer case-insensitive. More information here:
// https://github.com/antlr/antlr4/blob/master/doc/case-insensitive-lexing.md
ABS:                                     'ABS'                                     -> channel(KEYWORD_CHANNEL);
ACOS:                                    'ACOS'                                    -> channel(KEYWORD_CHANNEL);
ACOSH:                                   'ACOSH'                                   -> channel(KEYWORD_CHANNEL);
ACOT:                                    'ACOT'                                    -> channel(KEYWORD_CHANNEL);
ACOTH:                                   'ACOTH'                                   -> channel(KEYWORD_CHANNEL);
ADDCOLUMNS:                              'ADDCOLUMNS'                              -> channel(KEYWORD_CHANNEL);
ADDMISSINGITEMS:                         'ADDMISSINGITEMS'                         -> channel(KEYWORD_CHANNEL);
ALL:                                     'ALL'                                     -> channel(KEYWORD_CHANNEL);
ALLCROSSFILTERED:						 'ALLCROSSFILTERED'						   -> channel(KEYWORD_CHANNEL);
ALLEXCEPT:                               'ALLEXCEPT'                               -> channel(KEYWORD_CHANNEL);
ALLNOBLANKROW:                           'ALLNOBLANKROW'                           -> channel(KEYWORD_CHANNEL);
ALLSELECTED:                             'ALLSELECTED'                             -> channel(KEYWORD_CHANNEL);
AND:                                     'AND'                                     -> channel(KEYWORD_CHANNEL);
APPROXIMATEDISTINCTCOUNT:                'APPROXIMATEDISTINCTCOUNT'                -> channel(KEYWORD_CHANNEL);
ASIN:                                    'ASIN'                                    -> channel(KEYWORD_CHANNEL);
ASINH:                                   'ASINH'                                   -> channel(KEYWORD_CHANNEL);
ATAN:                                    'ATAN'                                    -> channel(KEYWORD_CHANNEL);
ATANH:                                   'ATANH'                                   -> channel(KEYWORD_CHANNEL);
AVERAGE:                                 'AVERAGE'                                 -> channel(KEYWORD_CHANNEL);
AVERAGEA:                                'AVERAGEA'                                -> channel(KEYWORD_CHANNEL);
AVERAGEX:                                'AVERAGEX'                                -> channel(KEYWORD_CHANNEL);
BETADIST:                                'BETA.DIST'                               -> channel(KEYWORD_CHANNEL);
BETAINV:                                 'BETA.INV'                                -> channel(KEYWORD_CHANNEL);
BLANK:                                   'BLANK'                                   -> channel(KEYWORD_CHANNEL);
CALCULATE:                               'CALCULATE'                               -> channel(KEYWORD_CHANNEL);
CALCULATETABLE:                          'CALCULATETABLE'                          -> channel(KEYWORD_CHANNEL);
CALENDAR:                                'CALENDAR'                                -> channel(KEYWORD_CHANNEL);
CALENDARAUTO:                            'CALENDARAUTO'                            -> channel(KEYWORD_CHANNEL);
CEILING:                                 'CEILING'                                 -> channel(KEYWORD_CHANNEL);
CHISQDIST:                               'CHISQ.DIST'                              -> channel(KEYWORD_CHANNEL);
CHISQDISTRT:                             'CHISQ.DIST.RT'                           -> channel(KEYWORD_CHANNEL);
CHISQINV:                                'CHISQ.INV'                               -> channel(KEYWORD_CHANNEL);
CHISQINVRT:                              'CHISQ.INV.RT'                            -> channel(KEYWORD_CHANNEL);
CLOSINGBALANCEMONTH:                     'CLOSINGBALANCEMONTH'                     -> channel(KEYWORD_CHANNEL);
CLOSINGBALANCEQUARTER:                   'CLOSINGBALANCEQUARTER'                   -> channel(KEYWORD_CHANNEL);
CLOSINGBALANCEYEAR:                      'CLOSINGBALANCEYEAR'                      -> channel(KEYWORD_CHANNEL);
COALESCE:								 'COALESCE'								   -> channel(KEYWORD_CHANNEL);
COMBIN:                                  'COMBIN'                                  -> channel(KEYWORD_CHANNEL);
COMBINA:                                 'COMBINA'                                 -> channel(KEYWORD_CHANNEL);
COMBINEVALUES:                           'COMBINEVALUES'                           -> channel(KEYWORD_CHANNEL);
CONCATENATE:                             'CONCATENATE'                             -> channel(KEYWORD_CHANNEL);
CONCATENATEX:                            'CONCATENATEX'                            -> channel(KEYWORD_CHANNEL);
CONFIDENCENORM:                          'CONFIDENCE.NORM'                         -> channel(KEYWORD_CHANNEL);
CONFIDENCET:                             'CONFIDENCE.T'                            -> channel(KEYWORD_CHANNEL);
CONTAINS:                                'CONTAINS'                                -> channel(KEYWORD_CHANNEL);
CONTAINSROW:                             'CONTAINSROW'                             -> channel(KEYWORD_CHANNEL);
CONTAINSSTRING:							 'CONTAINSSTRING'						   -> channel(KEYWORD_CHANNEL);
CONTAINSSTRINGEXACT:					 'CONTAINSSTRINGEXACT'					   -> channel(KEYWORD_CHANNEL);
CONVERT:								 'CONVERT'								   -> channel(KEYWORD_CHANNEL);
COS:                                     'COS'                                     -> channel(KEYWORD_CHANNEL);
COSH:                                    'COSH'                                    -> channel(KEYWORD_CHANNEL);
COT:                                     'COT'                                     -> channel(KEYWORD_CHANNEL);
COTH:                                    'COTH'                                    -> channel(KEYWORD_CHANNEL);
COUNT:                                   'COUNT'                                   -> channel(KEYWORD_CHANNEL);
COUNTA:                                  'COUNTA'                                  -> channel(KEYWORD_CHANNEL);
COUNTAX:                                 'COUNTAX'                                 -> channel(KEYWORD_CHANNEL);
COUNTBLANK:                              'COUNTBLANK'                              -> channel(KEYWORD_CHANNEL);
COUNTROWS:                               'COUNTROWS'                               -> channel(KEYWORD_CHANNEL);
COUNTX:                                  'COUNTX'                                  -> channel(KEYWORD_CHANNEL);
CROSSFILTER:                             'CROSSFILTER'                             -> channel(KEYWORD_CHANNEL);
CROSSJOIN:                               'CROSSJOIN'                               -> channel(KEYWORD_CHANNEL);
CURRENCY:                                'CURRENCY'                                -> channel(KEYWORD_CHANNEL);
CURRENTGROUP:                            'CURRENTGROUP'                            -> channel(KEYWORD_CHANNEL);
CUSTOMDATA:                              'CUSTOMDATA'                              -> channel(KEYWORD_CHANNEL);
DATATABLE:                               'DATATABLE'                               -> channel(KEYWORD_CHANNEL);
DATE:                                    'DATE'                                    -> channel(KEYWORD_CHANNEL);
DATEADD:                                 'DATEADD'                                 -> channel(KEYWORD_CHANNEL);
DATEDIFF:                                'DATEDIFF'                                -> channel(KEYWORD_CHANNEL);
DATESBETWEEN:                            'DATESBETWEEN'                            -> channel(KEYWORD_CHANNEL);
DATESINPERIOD:                           'DATESINPERIOD'                           -> channel(KEYWORD_CHANNEL);
DATESMTD:                                'DATESMTD'                                -> channel(KEYWORD_CHANNEL);
DATESQTD:                                'DATESQTD'                                -> channel(KEYWORD_CHANNEL);
DATESYTD:                                'DATESYTD'                                -> channel(KEYWORD_CHANNEL);
DATEVALUE:                               'DATEVALUE'                               -> channel(KEYWORD_CHANNEL);
DAY:                                     'DAY'                                     -> channel(KEYWORD_CHANNEL);
DEGREES:                                 'DEGREES'                                 -> channel(KEYWORD_CHANNEL);
DETAILROWS:                              'DETAILROWS'                              -> channel(KEYWORD_CHANNEL);
DISTINCT:                                'DISTINCT'                                -> channel(KEYWORD_CHANNEL);
DISTINCTCOUNT:                           'DISTINCTCOUNT'                           -> channel(KEYWORD_CHANNEL);
DISTINCTCOUNTNOBLANK:                    'DISTINCTCOUNTNOBLANK'                    -> channel(KEYWORD_CHANNEL);
DIVIDE:                                  'DIVIDE'                                  -> channel(KEYWORD_CHANNEL);
EARLIER:                                 'EARLIER'                                 -> channel(KEYWORD_CHANNEL);
EARLIEST:                                'EARLIEST'                                -> channel(KEYWORD_CHANNEL);
EDATE:                                   'EDATE'                                   -> channel(KEYWORD_CHANNEL);
ENDOFMONTH:                              'ENDOFMONTH'                              -> channel(KEYWORD_CHANNEL);
ENDOFQUARTER:                            'ENDOFQUARTER'                            -> channel(KEYWORD_CHANNEL);
ENDOFYEAR:                               'ENDOFYEAR'                               -> channel(KEYWORD_CHANNEL);
EOMONTH:                                 'EOMONTH'                                 -> channel(KEYWORD_CHANNEL);
ERROR:                                   'ERROR'                                   -> channel(KEYWORD_CHANNEL);
EVEN:                                    'EVEN'                                    -> channel(KEYWORD_CHANNEL);
EXACT:                                   'EXACT'                                   -> channel(KEYWORD_CHANNEL);
EXCEPT:                                  'EXCEPT'                                  -> channel(KEYWORD_CHANNEL);
EXP:                                     'EXP'                                     -> channel(KEYWORD_CHANNEL);
EXPONDIST:                               'EXPON.DIST'                              -> channel(KEYWORD_CHANNEL);
FACT:                                    'FACT'                                    -> channel(KEYWORD_CHANNEL);
FALSE:                                   'FALSE'                                   -> channel(KEYWORD_CHANNEL);
FILTER:                                  'FILTER'                                  -> channel(KEYWORD_CHANNEL);
FILTERS:                                 'FILTERS'                                 -> channel(KEYWORD_CHANNEL);
FIND:                                    'FIND'                                    -> channel(KEYWORD_CHANNEL);
FIRSTDATE:                               'FIRSTDATE'                               -> channel(KEYWORD_CHANNEL);
FIRSTNONBLANK:                           'FIRSTNONBLANK'                           -> channel(KEYWORD_CHANNEL);
FIRSTNONBLANKVALUE:						 'FIRSTNONBLANKVALUE'					   -> channel(KEYWORD_CHANNEL);
FIXED:                                   'FIXED'                                   -> channel(KEYWORD_CHANNEL);
FLOOR:                                   'FLOOR'                                   -> channel(KEYWORD_CHANNEL);
FORMAT:                                  'FORMAT'                                  -> channel(KEYWORD_CHANNEL);
GCD:                                     'GCD'                                     -> channel(KEYWORD_CHANNEL);
GENERATE:                                'GENERATE'                                -> channel(KEYWORD_CHANNEL);
GENERATEALL:                             'GENERATEALL'                             -> channel(KEYWORD_CHANNEL);
GENERATESERIES:                          'GENERATESERIES'                          -> channel(KEYWORD_CHANNEL);
GEOMEAN:                                 'GEOMEAN'                                 -> channel(KEYWORD_CHANNEL);
GEOMEANX:                                'GEOMEANX'                                -> channel(KEYWORD_CHANNEL);
GROUPBY:                                 'GROUPBY'                                 -> channel(KEYWORD_CHANNEL);
HASONEFILTER:                            'HASONEFILTER'                            -> channel(KEYWORD_CHANNEL);
HASONEVALUE:                             'HASONEVALUE'                             -> channel(KEYWORD_CHANNEL);
HOUR:                                    'HOUR'                                    -> channel(KEYWORD_CHANNEL);
IF:                                      'IF'                                      -> channel(KEYWORD_CHANNEL);
IFEAGER:                                 'IF.EAGER'                                -> channel(KEYWORD_CHANNEL);
IFERROR:                                 'IFERROR'                                 -> channel(KEYWORD_CHANNEL);
IGNORE:                                  'IGNORE'                                  -> channel(KEYWORD_CHANNEL);
INT:                                     'INT'                                     -> channel(KEYWORD_CHANNEL);
INTERSECT:                               'INTERSECT'                               -> channel(KEYWORD_CHANNEL);
ISBLANK:                                 'ISBLANK'                                 -> channel(KEYWORD_CHANNEL);
ISCROSSFILTERED:                         'ISCROSSFILTERED'                         -> channel(KEYWORD_CHANNEL);
ISEMPTY:                                 'ISEMPTY'                                 -> channel(KEYWORD_CHANNEL);
ISERROR:                                 'ISERROR'                                 -> channel(KEYWORD_CHANNEL);
ISEVEN:                                  'ISEVEN'                                  -> channel(KEYWORD_CHANNEL);
ISFILTERED:                              'ISFILTERED'                              -> channel(KEYWORD_CHANNEL);
ISINSCOPE:                               'ISINSCOPE'                               -> channel(KEYWORD_CHANNEL);
ISLOGICAL:                               'ISLOGICAL'                               -> channel(KEYWORD_CHANNEL);
ISNONTEXT:                               'ISNONTEXT'                               -> channel(KEYWORD_CHANNEL);
ISNUMBER:                                'ISNUMBER'                                -> channel(KEYWORD_CHANNEL);
ISOCEILING:                              'ISO.CEILING'                             -> channel(KEYWORD_CHANNEL);
ISODD:                                   'ISODD'                                   -> channel(KEYWORD_CHANNEL);
ISONORAFTER:                             'ISONORAFTER'                             -> channel(KEYWORD_CHANNEL);
ISSELECTEDMEASURE:                       'ISSELECTEDMEASURE'                       -> channel(KEYWORD_CHANNEL);
ISSUBTOTAL:                              'ISSUBTOTAL'                              -> channel(KEYWORD_CHANNEL);
ISTEXT:                                  'ISTEXT'                                  -> channel(KEYWORD_CHANNEL);
KEEPFILTERS:                             'KEEPFILTERS'                             -> channel(KEYWORD_CHANNEL);
KEYWORDMATCH:                            'KEYWORDMATCH'                            -> channel(KEYWORD_CHANNEL);
LASTDATE:                                'LASTDATE'                                -> channel(KEYWORD_CHANNEL);
LASTNONBLANK:                            'LASTNONBLANK'                            -> channel(KEYWORD_CHANNEL);
LASTNONBLANKVALUE:                       'LASTNONBLANKVALUE'                       -> channel(KEYWORD_CHANNEL);
LCM:                                     'LCM'                                     -> channel(KEYWORD_CHANNEL);
LEFT:                                    'LEFT'                                    -> channel(KEYWORD_CHANNEL);
LEN:                                     'LEN'                                     -> channel(KEYWORD_CHANNEL);
LN:                                      'LN'                                      -> channel(KEYWORD_CHANNEL);
LOG:                                     'LOG'                                     -> channel(KEYWORD_CHANNEL);
LOG10:                                   'LOG10'                                   -> channel(KEYWORD_CHANNEL);
LOOKUPVALUE:                             'LOOKUPVALUE'                             -> channel(KEYWORD_CHANNEL);
LOWER:                                   'LOWER'                                   -> channel(KEYWORD_CHANNEL);
MAX:                                     'MAX'                                     -> channel(KEYWORD_CHANNEL);
MAXA:                                    'MAXA'                                    -> channel(KEYWORD_CHANNEL);
MAXX:                                    'MAXX'                                    -> channel(KEYWORD_CHANNEL);
MEDIAN:                                  'MEDIAN'                                  -> channel(KEYWORD_CHANNEL);
MEDIANX:                                 'MEDIANX'                                 -> channel(KEYWORD_CHANNEL);
MID:                                     'MID'                                     -> channel(KEYWORD_CHANNEL);
MIN:                                     'MIN'                                     -> channel(KEYWORD_CHANNEL);
MINA:                                    'MINA'                                    -> channel(KEYWORD_CHANNEL);
MINUTE:                                  'MINUTE'                                  -> channel(KEYWORD_CHANNEL);
MINX:                                    'MINX'                                    -> channel(KEYWORD_CHANNEL);
MOD:                                     'MOD'                                     -> channel(KEYWORD_CHANNEL);
MONTH:                                   'MONTH'                                   -> channel(KEYWORD_CHANNEL);
MROUND:                                  'MROUND'                                  -> channel(KEYWORD_CHANNEL);
NATURALINNERJOIN:                        'NATURALINNERJOIN'                        -> channel(KEYWORD_CHANNEL);
NATURALLEFTOUTERJOIN:                    'NATURALLEFTOUTERJOIN'                    -> channel(KEYWORD_CHANNEL);
NEXTDAY:                                 'NEXTDAY'                                 -> channel(KEYWORD_CHANNEL);
NEXTMONTH:                               'NEXTMONTH'                               -> channel(KEYWORD_CHANNEL);
NEXTQUARTER:                             'NEXTQUARTER'                             -> channel(KEYWORD_CHANNEL);
NEXTYEAR:                                'NEXTYEAR'                                -> channel(KEYWORD_CHANNEL);
NONVISUAL:                               'NONVISUAL'                               -> channel(KEYWORD_CHANNEL);
NORMDIST:                                'NORM.DIST'                               -> channel(KEYWORD_CHANNEL);
NORMINV:                                 'NORM.INV'                                -> channel(KEYWORD_CHANNEL);
NORMSDIST:                               'NORM.S.DIST'                             -> channel(KEYWORD_CHANNEL);
NORMSINV:                                'NORM.S.INV'                              -> channel(KEYWORD_CHANNEL);
NOT:                                     'NOT'                                     -> channel(KEYWORD_CHANNEL);
NOW:                                     'NOW'                                     -> channel(KEYWORD_CHANNEL);
ODD:                                     'ODD'                                     -> channel(KEYWORD_CHANNEL);
OPENINGBALANCEMONTH:                     'OPENINGBALANCEMONTH'                     -> channel(KEYWORD_CHANNEL);
OPENINGBALANCEQUARTER:                   'OPENINGBALANCEQUARTER'                   -> channel(KEYWORD_CHANNEL);
OPENINGBALANCEYEAR:                      'OPENINGBALANCEYEAR'                      -> channel(KEYWORD_CHANNEL);
OR:                                      'OR'                                      -> channel(KEYWORD_CHANNEL);
PARALLELPERIOD:                          'PARALLELPERIOD'                          -> channel(KEYWORD_CHANNEL);
PATH:                                    'PATH'                                    -> channel(KEYWORD_CHANNEL);
PATHCONTAINS:                            'PATHCONTAINS'                            -> channel(KEYWORD_CHANNEL);
PATHITEM:                                'PATHITEM'                                -> channel(KEYWORD_CHANNEL);
PATHITEMREVERSE:                         'PATHITEMREVERSE'                         -> channel(KEYWORD_CHANNEL);
PATHLENGTH:                              'PATHLENGTH'                              -> channel(KEYWORD_CHANNEL);
PERCENTILEEXC:                           'PERCENTILE.EXC'                          -> channel(KEYWORD_CHANNEL);
PERCENTILEINC:                           'PERCENTILE.INC'                          -> channel(KEYWORD_CHANNEL);
PERCENTILEXEXC:                          'PERCENTILEX.EXC'                         -> channel(KEYWORD_CHANNEL);
PERCENTILEXINC:                          'PERCENTILEX.INC'                         -> channel(KEYWORD_CHANNEL);
PERMUT:                                  'PERMUT'                                  -> channel(KEYWORD_CHANNEL);
PI:                                      'PI'                                      -> channel(KEYWORD_CHANNEL);
POISSONDIST:                             'POISSON.DIST'                            -> channel(KEYWORD_CHANNEL);
POWER:                                   'POWER'                                   -> channel(KEYWORD_CHANNEL);
PREVIOUSDAY:                             'PREVIOUSDAY'                             -> channel(KEYWORD_CHANNEL);
PREVIOUSMONTH:                           'PREVIOUSMONTH'                           -> channel(KEYWORD_CHANNEL);
PREVIOUSQUARTER:                         'PREVIOUSQUARTER'                         -> channel(KEYWORD_CHANNEL);
PREVIOUSYEAR:                            'PREVIOUSYEAR'                            -> channel(KEYWORD_CHANNEL);
PRODUCT:                                 'PRODUCT'                                 -> channel(KEYWORD_CHANNEL);
PRODUCTX:                                'PRODUCTX'                                -> channel(KEYWORD_CHANNEL);
QUARTER:								 'QUARTER'								   -> channel(KEYWORD_CHANNEL);
QUOTIENT:                                'QUOTIENT'                                -> channel(KEYWORD_CHANNEL);
RADIANS:                                 'RADIANS'                                 -> channel(KEYWORD_CHANNEL);
RAND:                                    'RAND'                                    -> channel(KEYWORD_CHANNEL);
RANDBETWEEN:                             'RANDBETWEEN'                             -> channel(KEYWORD_CHANNEL);
RANKEQ:                                  'RANK.EQ'                                 -> channel(KEYWORD_CHANNEL);
RANKX:                                   'RANKX'                                   -> channel(KEYWORD_CHANNEL);
RELATED:                                 'RELATED'                                 -> channel(KEYWORD_CHANNEL);
RELATEDTABLE:                            'RELATEDTABLE'                            -> channel(KEYWORD_CHANNEL);
REMOVEFILTERS:							 'REMOVEFILTERS'						   -> channel(KEYWORD_CHANNEL);
REPLACE:                                 'REPLACE'                                 -> channel(KEYWORD_CHANNEL);
REPT:                                    'REPT'                                    -> channel(KEYWORD_CHANNEL);
RIGHT:                                   'RIGHT'                                   -> channel(KEYWORD_CHANNEL);
ROLLUP:                                  'ROLLUP'                                  -> channel(KEYWORD_CHANNEL);
ROLLUPADDISSUBTOTAL:                     'ROLLUPADDISSUBTOTAL'                     -> channel(KEYWORD_CHANNEL);
ROLLUPGROUP:                             'ROLLUPGROUP'                             -> channel(KEYWORD_CHANNEL);
ROLLUPISSUBTOTAL:                        'ROLLUPISSUBTOTAL'                        -> channel(KEYWORD_CHANNEL);
ROUND:                                   'ROUND'                                   -> channel(KEYWORD_CHANNEL);
ROUNDDOWN:                               'ROUNDDOWN'                               -> channel(KEYWORD_CHANNEL);
ROUNDUP:                                 'ROUNDUP'                                 -> channel(KEYWORD_CHANNEL);
ROW:                                     'ROW'                                     -> channel(KEYWORD_CHANNEL);
SAMEPERIODLASTYEAR:                      'SAMEPERIODLASTYEAR'                      -> channel(KEYWORD_CHANNEL);
SAMPLE:                                  'SAMPLE'                                  -> channel(KEYWORD_CHANNEL);
SEARCH:                                  'SEARCH'                                  -> channel(KEYWORD_CHANNEL);
SECOND:                                  'SECOND'                                  -> channel(KEYWORD_CHANNEL);
SELECTCOLUMNS:                           'SELECTCOLUMNS'                           -> channel(KEYWORD_CHANNEL);
SELECTEDMEASURE:                         'SELECTEDMEASURE'                         -> channel(KEYWORD_CHANNEL);
SELECTEDMEASUREFORMATSTRING:             'SELECTEDMEASUREFORMATSTRING'             -> channel(KEYWORD_CHANNEL);
SELECTEDMEASURENAME:                     'SELECTEDMEASURENAME'                     -> channel(KEYWORD_CHANNEL);
SELECTEDVALUE:                           'SELECTEDVALUE'                           -> channel(KEYWORD_CHANNEL);
SIGN:                                    'SIGN'                                    -> channel(KEYWORD_CHANNEL);
SIN:                                     'SIN'                                     -> channel(KEYWORD_CHANNEL);
SINH:                                    'SINH'                                    -> channel(KEYWORD_CHANNEL);
SQRT:                                    'SQRT'                                    -> channel(KEYWORD_CHANNEL);
SQRTPI:                                  'SQRTPI'                                  -> channel(KEYWORD_CHANNEL);
STARTOFMONTH:                            'STARTOFMONTH'                            -> channel(KEYWORD_CHANNEL);
STARTOFQUARTER:                          'STARTOFQUARTER'                          -> channel(KEYWORD_CHANNEL);
STARTOFYEAR:                             'STARTOFYEAR'                             -> channel(KEYWORD_CHANNEL);
STDEVP:                                  'STDEV.P'                                 -> channel(KEYWORD_CHANNEL);
STDEVS:                                  'STDEV.S'                                 -> channel(KEYWORD_CHANNEL);
STDEVXP:                                 'STDEVX.P'                                -> channel(KEYWORD_CHANNEL);
STDEVXS:                                 'STDEVX.S'                                -> channel(KEYWORD_CHANNEL);
SUBSTITUTE:                              'SUBSTITUTE'                              -> channel(KEYWORD_CHANNEL);
SUBSTITUTEWITHINDEX:                     'SUBSTITUTEWITHINDEX'                     -> channel(KEYWORD_CHANNEL);
SUM:                                     'SUM'                                     -> channel(KEYWORD_CHANNEL);
SUMMARIZE:                               'SUMMARIZE'                               -> channel(KEYWORD_CHANNEL);
SUMMARIZECOLUMNS:                        'SUMMARIZECOLUMNS'                        -> channel(KEYWORD_CHANNEL);
SUMX:                                    'SUMX'                                    -> channel(KEYWORD_CHANNEL);
SWITCH:                                  'SWITCH'                                  -> channel(KEYWORD_CHANNEL);
TDIST:                                   'T.DIST'                                  -> channel(KEYWORD_CHANNEL);
TDIST2T:                                 'T.DIST.2T'                               -> channel(KEYWORD_CHANNEL);
TDISTRT:                                 'T.DIST.RT'                               -> channel(KEYWORD_CHANNEL);
TINV:                                    'T.INV'                                   -> channel(KEYWORD_CHANNEL);
TINV2T:                                  'T.INV.2T'                                -> channel(KEYWORD_CHANNEL);
TAN:                                     'TAN'                                     -> channel(KEYWORD_CHANNEL);
TANH:                                    'TANH'                                    -> channel(KEYWORD_CHANNEL);
TIME:                                    'TIME'                                    -> channel(KEYWORD_CHANNEL);
TIMEVALUE:                               'TIMEVALUE'                               -> channel(KEYWORD_CHANNEL);
TODAY:                                   'TODAY'                                   -> channel(KEYWORD_CHANNEL);
TOPN:                                    'TOPN'                                    -> channel(KEYWORD_CHANNEL);
TOPNPERLEVEL:                            'TOPNPERLEVEL'                            -> channel(KEYWORD_CHANNEL);
TOPNSKIP:                                'TOPNSKIP'                                -> channel(KEYWORD_CHANNEL);
TOTALMTD:                                'TOTALMTD'                                -> channel(KEYWORD_CHANNEL);
TOTALQTD:                                'TOTALQTD'                                -> channel(KEYWORD_CHANNEL);
TOTALYTD:                                'TOTALYTD'                                -> channel(KEYWORD_CHANNEL);
TREATAS:                                 'TREATAS'                                 -> channel(KEYWORD_CHANNEL);
TRIM:                                    'TRIM'                                    -> channel(KEYWORD_CHANNEL);
TRUE:                                    'TRUE'                                    -> channel(KEYWORD_CHANNEL);
TRUNC:                                   'TRUNC'                                   -> channel(KEYWORD_CHANNEL);
UNICHAR:                                 'UNICHAR'                                 -> channel(KEYWORD_CHANNEL);
UNICODE:                                 'UNICODE'                                 -> channel(KEYWORD_CHANNEL);
UNION:                                   'UNION'                                   -> channel(KEYWORD_CHANNEL);
UPPER:                                   'UPPER'                                   -> channel(KEYWORD_CHANNEL);
USERELATIONSHIP:                         'USERELATIONSHIP'                         -> channel(KEYWORD_CHANNEL);
USERNAME:                                'USERNAME'                                -> channel(KEYWORD_CHANNEL);
USEROBJECTID:                            'USEROBJECTID'                            -> channel(KEYWORD_CHANNEL);
USERPRINCIPALNAME:                       'USERPRINCIPALNAME'                       -> channel(KEYWORD_CHANNEL);
UTCNOW:                                  'UTCNOW'                                  -> channel(KEYWORD_CHANNEL);
UTCTODAY:                                'UTCTODAY'                                -> channel(KEYWORD_CHANNEL);
VALUE:                                   'VALUE'                                   -> channel(KEYWORD_CHANNEL);
VALUES:                                  'VALUES'                                  -> channel(KEYWORD_CHANNEL);
VARP:                                    'VAR.P'                                   -> channel(KEYWORD_CHANNEL);
VARS:                                    'VAR.S'                                   -> channel(KEYWORD_CHANNEL);
VARXP:                                   'VARX.P'                                  -> channel(KEYWORD_CHANNEL);
VARXS:                                   'VARX.S'                                  -> channel(KEYWORD_CHANNEL);
WEEKDAY:                                 'WEEKDAY'                                 -> channel(KEYWORD_CHANNEL);
YEARFRAC:                                'YEARFRAC'                                -> channel(KEYWORD_CHANNEL);
WEEKNUM:                                 'WEEKNUM'                                 -> channel(KEYWORD_CHANNEL);
XIRR:                                    'XIRR'                                    -> channel(KEYWORD_CHANNEL);
XNPV:                                    'XNPV'                                    -> channel(KEYWORD_CHANNEL);
YEAR:                                    'YEAR'                                    -> channel(KEYWORD_CHANNEL);

// New financial DAX functions added June 2020:
ACCRINT:                                  'ACCRINT'                                -> channel(KEYWORD_CHANNEL);
ACCRINTM:                                 'ACCRINTM'                               -> channel(KEYWORD_CHANNEL);
AMORDEGRC:                                'AMORDEGRC'                              -> channel(KEYWORD_CHANNEL);
AMORLINC:                                 'AMORLINC'                               -> channel(KEYWORD_CHANNEL);
COUPDAYBS:                                'COUPDAYBS'                              -> channel(KEYWORD_CHANNEL);
COUPDAYS:                                 'COUPDAYS'                               -> channel(KEYWORD_CHANNEL);
COUPDAYSNC:                               'COUPDAYSNC'                             -> channel(KEYWORD_CHANNEL);
COUPNCD:                                  'COUPNCD'                                -> channel(KEYWORD_CHANNEL);
COUPNUM:                                  'COUPNUM'                                -> channel(KEYWORD_CHANNEL);
COUPPCD:                                  'COUPPCD'                                -> channel(KEYWORD_CHANNEL);
CUMIPMT:                                  'CUMIPMT'                                -> channel(KEYWORD_CHANNEL);
CUMPRINC:                                 'CUMPRINC'                               -> channel(KEYWORD_CHANNEL);
DB:                                       'DB'                                     -> channel(KEYWORD_CHANNEL);
DDB:                                      'DDB'                                    -> channel(KEYWORD_CHANNEL);
DISC:                                     'DISC'                                   -> channel(KEYWORD_CHANNEL);
DOLLARDE:                                 'DOLLARDE'                               -> channel(KEYWORD_CHANNEL);
DOLLARFR:                                 'DOLLARFR'                               -> channel(KEYWORD_CHANNEL);
DURATION:                                 'DURATION'                               -> channel(KEYWORD_CHANNEL);
EFFECT:                                   'EFFECT'                                 -> channel(KEYWORD_CHANNEL);
FV:                                       'FV'                                     -> channel(KEYWORD_CHANNEL);
INTRATE:                                  'INTRATE'                                -> channel(KEYWORD_CHANNEL);
IPMT:                                     'IPMT'                                   -> channel(KEYWORD_CHANNEL);
ISPMT:                                    'ISPMT'                                  -> channel(KEYWORD_CHANNEL);
MDURATION:                                'MDURATION'                              -> channel(KEYWORD_CHANNEL);
NOMINAL:                                  'NOMINAL'                                -> channel(KEYWORD_CHANNEL);
NPER:                                     'NPER'                                   -> channel(KEYWORD_CHANNEL);
ODDFPRICE:                                'ODDFPRICE'                              -> channel(KEYWORD_CHANNEL);
ODDFYIELD:                                'ODDFYIELD'                              -> channel(KEYWORD_CHANNEL);
ODDLPRICE:                                'ODDLPRICE'                              -> channel(KEYWORD_CHANNEL);
ODDLYIELD:                                'ODDLYIELD'                              -> channel(KEYWORD_CHANNEL);
PDURATION:                                'PDURATION'                              -> channel(KEYWORD_CHANNEL);
PMT:                                      'PMT'                                    -> channel(KEYWORD_CHANNEL);
PPMT:                                     'PPMT'                                   -> channel(KEYWORD_CHANNEL);
PRICE:                                    'PRICE'                                  -> channel(KEYWORD_CHANNEL);
PRICEDISC:                                'PRICEDISC'                              -> channel(KEYWORD_CHANNEL);
PRICEMAT:                                 'PRICEMAT'                               -> channel(KEYWORD_CHANNEL);
PV:                                       'PV'                                     -> channel(KEYWORD_CHANNEL);
RATE:                                     'RATE'                                   -> channel(KEYWORD_CHANNEL);
RECEIVED:                                 'RECEIVED'                               -> channel(KEYWORD_CHANNEL);
RRI:                                      'RRI'                                    -> channel(KEYWORD_CHANNEL);
SLN:                                      'SLN'                                    -> channel(KEYWORD_CHANNEL);
SYD:                                      'SYD'                                    -> channel(KEYWORD_CHANNEL);
TBILLEQ:                                  'TBILLEQ'                                -> channel(KEYWORD_CHANNEL);
TBILLPRICE:                               'TBILLPRICE'                             -> channel(KEYWORD_CHANNEL);
TBILLYIELD:                               'TBILLYIELD'                             -> channel(KEYWORD_CHANNEL);
VDB:                                      'VDB'                                    -> channel(KEYWORD_CHANNEL);
YIELD:                                    'YIELD'                                  -> channel(KEYWORD_CHANNEL);
YIELDDISC:                                'YIELDDISC'                              -> channel(KEYWORD_CHANNEL);
YIELDMAT:                                 'YIELDMAT'                               -> channel(KEYWORD_CHANNEL);

SAMPLEAXISWITHLOCALMINMAX:				  'SAMPLEAXISWITHLOCALMINMAX'			   -> channel(KEYWORD_CHANNEL);
EVALUATEANDLOG:							  'EVALUATEANDLOG'						   -> channel(KEYWORD_CHANNEL);
OFFSET:									  'OFFSET'								   -> channel(KEYWORD_CHANNEL);
INDEX:									  'INDEX'								   -> channel(KEYWORD_CHANNEL);
WINDOW:									  'WINDOW'								   -> channel(KEYWORD_CHANNEL);
ORDERBY:								  'ORDERBY'								   -> channel(KEYWORD_CHANNEL);
RANK:								      'RANK'								   -> channel(KEYWORD_CHANNEL);
ROWNUMBER:								  'ROWNUMBER'							   -> channel(KEYWORD_CHANNEL);
PARTITIONBY:							  'PARTITIONBY'							   -> channel(KEYWORD_CHANNEL);
EXTERNALMEASURE:                          'EXTERNALMEASURE'                        -> channel(KEYWORD_CHANNEL);
KMEANSCLUSTERING:                         'KMEANSCLUSTERING'                       -> channel(KEYWORD_CHANNEL);

// Statements:
DEFINE:                                  'DEFINE'                                  -> channel(KEYWORD_CHANNEL);
EVALUATE:                                'EVALUATE'                                -> channel(KEYWORD_CHANNEL);
ORDER:                                   'ORDER'                                   -> channel(KEYWORD_CHANNEL);
BY:                                      'BY'                                      -> channel(KEYWORD_CHANNEL);
START:                                   'START'                                   -> channel(KEYWORD_CHANNEL);
AT:                                      'AT'                                      -> channel(KEYWORD_CHANNEL);
RETURN:                                  'RETURN'                                  -> channel(KEYWORD_CHANNEL);
VAR:                                     'VAR'                                     -> channel(KEYWORD_CHANNEL);
IN:                                      'IN'                                      -> channel(KEYWORD_CHANNEL);
ASC:                                     'ASC'                                     -> channel(KEYWORD_CHANNEL);
DESC:                                    'DESC'                                    -> channel(KEYWORD_CHANNEL);
SKIP_:                                   'SKIP'                                    -> channel(KEYWORD_CHANNEL);
DENSE:                                   'DENSE'                                   -> channel(KEYWORD_CHANNEL);
BLANKS:                                  'BLANKS'                                  -> channel(KEYWORD_CHANNEL);
LAST:                                    'LAST'                                    -> channel(KEYWORD_CHANNEL);
FIRST:                                   'FIRST'                                   -> channel(KEYWORD_CHANNEL);

// DATEADD / DATEDIFF interval arguments (in addition to Day, Month, Quarter, Year, which are also functions):
WEEK:                                    'WEEK'                                    -> channel(KEYWORD_CHANNEL);

// CROSSFILTER direction arguments:
BOTH:                                    'BOTH'                                    -> channel(KEYWORD_CHANNEL);
NONE:                                    'NONE'                                    -> channel(KEYWORD_CHANNEL);
ONEWAY:                                  'ONEWAY'                                  -> channel(KEYWORD_CHANNEL);
ONEWAYRIGHTFILTERSLEFT:                  'ONEWAY_RIGHTFILTERSLEFT'                 -> channel(KEYWORD_CHANNEL);
ONEWAYLEFTFILTERSRIGHT:                  'ONEWAY_LEFTFILTERSRIGHT'                 -> channel(KEYWORD_CHANNEL);

// DATATABLE type arguments (in addition to Currency, which is also a function):
INTEGER:                                 'INTEGER'                                 -> channel(KEYWORD_CHANNEL);
DOUBLE:                                  'DOUBLE'                                  -> channel(KEYWORD_CHANNEL);
STRING:                                  'STRING'                                  -> channel(KEYWORD_CHANNEL);
BOOLEAN:                                 'BOOLEAN'                                 -> channel(KEYWORD_CHANNEL);
DATETIME:                                'DATETIME'                                -> channel(KEYWORD_CHANNEL);
VARIANT:                                 'VARIANT'                                 -> channel(KEYWORD_CHANNEL);

TEXT:                                    'TEXT'                                    -> channel(KEYWORD_CHANNEL);
ALPHABETICAL:                            'ALPHABETICAL'                            -> channel(KEYWORD_CHANNEL);
KEEP:									 'KEEP'									   -> channel(KEYWORD_CHANNEL);
REL:									 'REL'									   -> channel(KEYWORD_CHANNEL);

DATE_LITERAL:          'DT"' (~'"' | '""')* '"' {Text = Text.Substring(3, Text.Length - 4);};
INTEGER_LITERAL:       [0-9]+;
REAL_LITERAL:          [0-9]* '.' [0-9]+;
STRING_LITERAL:        '"' (~'"' | '""')* '"' {Text = Text.Substring(1, Text.Length - 2);};
TABLE:                 '\'' (~["\'\r\n\u0085\u2028\u2029] | '\'\'')* '\'' {Text = Text.Substring(1, Text.Length - 2).Replace("''","'");};
COLUMN_OR_MEASURE:     '[' (~["\]\r\n\u0085\u2028\u2029] | ']]')* ']'   {Text = Text.Substring(1, Text.Length - 2).Replace("]]","]");};
TABLE_OR_VARIABLE:     IdentifierOrKeyword;

OPEN_CURLY:			   '{';
CLOSE_CURLY:		   '}';
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
	: UnicodeClassLU
	| UnicodeClassLL
	| '_'
	;

fragment IdentifierPartCharacter
	: IdentifierStartCharacter
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