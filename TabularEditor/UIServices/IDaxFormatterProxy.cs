using System.Collections.Generic;

namespace TabularEditor.Dax
{
    public interface IDaxFormatterProxy
    {
        DaxFormatterResult FormatDax(string query, bool useSemicolonsAsSeparators, bool shortFormat, bool skipSpaceAfterFunctionName);
        List<DaxFormatterResult> FormatDaxMulti(List<string> dax, bool useSemicolonsAsSeparators, bool shortFormat, bool skipSpaceAfterFunctionName);
    }
}