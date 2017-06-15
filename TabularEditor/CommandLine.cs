using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections;

namespace TabularEditor
{
    /*
    public enum SwitchType
    {
        Bool,
        SingleArg,
        ListArg
    }

    public class CommandLineArgument: CommandLineObject
    {
        public string Value { get; private set; }
    }

    public class CommandLineSwitch: CommandLineObject
    {
        public string Alias { get; private set; }
        public ArgumentCollection Arguments { get; private set; }
        public SwitchCollection Subswitches { get; private set; }
    }

    public class CommandLineParser
    {
        private enum CLObjType
        {
            Arg,
            Switch
        }

        private class CLObjDef
        {
            public CLObjDef Parent;
            public CLObjType Type;
            public string Name;
            public string Alias;
            public bool Optional;
        }
        private List<CLObjDef> CLObjs = new List<CLObjDef>();

        public CommandLineSwitch AddSwitch(string name, string alias = null, SwitchType type = SwitchType.Bool)
        {
            CLObjs.Add(new CLObjDef { Name = name, Alias = alias, Optional = optional, Type = CLObjType.Switch });
        }
        public int AddArgument(string name, bool optional = true)
        {
            CLObjs.Add
        }

        public CommandLine Parse(params string[] args)
        {

        }
    }

    public class CommandLine
    {
        private List<CommandLineArgument> arguments;
        public ArgumentCollection Arguments { get; private set; }
    }


    public abstract class CommandLineObject
    {
        public string Name { get; private set; }
        public bool Optional { get; private set; }
    }
    public abstract class CommandLineObjectCollection<T> : IReadOnlyDictionary<string, T>, IReadOnlyList<T>
        where T: CommandLineObject
    {
        protected List<T> _args;

        public virtual T this[string key] => _args.FirstOrDefault(a => a.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));

        public T this[int index] => _args.ElementAtOrDefault(index);

        public IEnumerable<string> Keys => _args.Select(a => a.Name);

        public IEnumerable<T> Values => _args;

        public int Count => _args.Count;

        public virtual bool ContainsKey(string key)
        {
            return _args.Any(a => a.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return _args.Select(a => new KeyValuePair<string, T>(a.Name, a)).GetEnumerator();
        }

        public bool TryGetValue(string key, out T value)
        {
            value = this[key];
            return value != null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _args.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _args.GetEnumerator();
        }
    }
    public class ArgumentCollection : CommandLineObjectCollection<CommandLineArgument> { }
    public class SwitchCollection : CommandLineObjectCollection<CommandLineSwitch>
    {
        public override CommandLineSwitch this[string key] => 
            base[key] ?? _args.FirstOrDefault(a => a.Alias.Equals(key, StringComparison.InvariantCultureIgnoreCase));
        public override bool ContainsKey(string key)
        {
            return base.ContainsKey(key) || _args.Any(a => a.Alias.Equals(key, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    */
}
