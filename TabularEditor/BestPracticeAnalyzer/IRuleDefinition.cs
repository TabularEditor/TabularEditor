#if BPALib
using System.Collections.Generic;

namespace TabularEditor.BestPracticeAnalyzer
{
    public interface IRuleDefinition
    {
        string Name { get; }
        IEnumerable<BestPracticeRule> Rules { get; }
        bool Internal { get; }
    }
}
#endif