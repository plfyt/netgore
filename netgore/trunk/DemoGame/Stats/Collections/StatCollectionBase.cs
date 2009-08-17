using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NetGore.Collections;

namespace DemoGame
{
    /// <summary>
    /// An IStatCollection implementation that can contain as few or as many StatTypes as needed.
    /// </summary>
    public class StatCollectionBase : IStatCollection
    {
        readonly StatCollectionType _statCollectionType;
        readonly Dictionary<StatType, IStat> _stats = new Dictionary<StatType, IStat>(EnumComparer<StatType>.Instance);

        /// <summary>
        /// StatCollectionBase constructor.
        /// </summary>
        /// <param name="statCollectionType">The type of the collectoin.</param>
        protected StatCollectionBase(StatCollectionType statCollectionType)
        {
            _statCollectionType = statCollectionType;
        }

        /// <summary>
        /// Adds an IStat to the collection.
        /// </summary>
        /// <param name="stat">IStat to add to the collection.</param>
        protected void Add(IStat stat)
        {
            _stats.Add(stat.StatType, stat);
        }

        /// <summary>
        /// Adds IStats to the collection.
        /// </summary>
        /// <param name="stats">IStats to add to the collection.</param>
        protected void Add(IEnumerable<IStat> stats)
        {
            foreach (IStat stat in stats)
            {
                Add(stat);
            }
        }

        /// <summary>
        /// Adds IStats to the collection.
        /// </summary>
        /// <param name="stats">IStats to add to the collection.</param>
        protected void Add(params IStat[] stats)
        {
            foreach (IStat stat in stats)
            {
                Add(stat);
            }
        }

        /// <summary>
        /// Gets an IStat from this StatCollectionBase, or creates the IStat for the <paramref name="statType"/>
        /// if the IStat did not already exist in the collection.
        /// </summary>
        /// <param name="statType">Type of stat to get.</param>
        /// <returns>The IStat in this StatCollectionBase for Stat type <param name="statType"</returns>
        protected IStat GetStatOrCreate(StatType statType)
        {
            IStat stat;
            if (!_stats.TryGetValue(statType, out stat))
            {
                stat = StatFactory.CreateStat(statType, StatCollectionType);
                Add(stat);
            }

            return stat;
        }

        /// <summary>
        /// When overridden in the derived class, handles when an IStat is added to this StatCollectionBase. This will
        /// be invoked once and only once for every IStat added to this StatCollectionBase.
        /// </summary>
        /// <param name="stat">The IStat that was added to this StatCollectionBase.</param>
        protected virtual void HandleStatAdded(IStat stat)
        {
        }

        public IEnumerable<KeyValuePair<StatType, int>> ToKeyValuePairs()
        {
            return this.Select(x => new KeyValuePair<StatType, int>(x.StatType, x.Value)).ToArray();
        }

        #region IStatCollection Members

        public IEnumerator<IStat> GetEnumerator()
        {
            return _stats.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int this[StatType statType]
        {
            get
            {
                IStat stat;
                if (!_stats.TryGetValue(statType, out stat))
                    return 0;

                return stat.Value;
            }
            set
            {
                IStat stat = GetStat(statType);
                stat.Value = value;
            }
        }

        public bool Contains(StatType statType)
        {
            return _stats.ContainsKey(statType);
        }

        public virtual IStat GetStat(StatType statType)
        {
            return _stats[statType];
        }

        public bool TryGetStat(StatType statType, out IStat stat)
        {
            return _stats.TryGetValue(statType, out stat);
        }

        public bool TryGetStatValue(StatType statType, out int value)
        {
            IStat stat;
            if (!TryGetStat(statType, out stat))
            {
                value = 0;
                return false;
            }

            value = stat.Value;
            return true;
        }

        /// <summary>
        /// Copies the values from the given IEnumerable of <paramref name="values"/> using the given StatType
        /// into this IStatCollection.
        /// </summary>
        /// <param name="values">IEnumerable of StatTypes and stat values to copy into this IStatCollection.</param>
        /// <param name="checkContains">If true, each StatType in <paramref name="values"/> will first be checked
        /// if it is in this IStatCollection before trying to copy over the value. Any StatType in
        /// <paramref name="values"/> but not in this IStatCollection will be skipped. If false, no checking will
        /// be done. Any StatType in <paramref name="values"/> but not in this IStatCollection will behave
        /// the same as if the value of a StatType not in this IStatCollection was attempted to be assigned
        /// in any other way.</param>
        public void CopyValuesFrom(IEnumerable<KeyValuePair<StatType, int>> values, bool checkContains)
        {
            foreach (var value in values)
            {
                if (checkContains && !Contains(value.Key))
                    continue;

                this[value.Key] = value.Value;
            }
        }

        /// <summary>
        /// Copies the values from the given IEnumerable of <paramref name="values"/> using the given StatType
        /// into this IStatCollection.
        /// </summary>
        /// <param name="values">IEnumerable of StatTypes and stat values to copy into this IStatCollection.</param>
        /// <param name="checkContains">If true, each StatType in <paramref name="values"/> will first be checked
        /// if it is in this IStatCollection before trying to copy over the value. Any StatType in
        /// <paramref name="values"/> but not in this IStatCollection will be skipped. If false, no checking will
        /// be done. Any StatType in <paramref name="values"/> but not in this IStatCollection will behave
        /// the same as if the value of a StatType not in this IStatCollection was attempted to be assigned
        /// in any other way.</param>
        public void CopyValuesFrom(IEnumerable<IStat> values, bool checkContains)
        {
            CopyValuesFrom(values.Select(x => new KeyValuePair<StatType, int>(x.StatType, x.Value)), checkContains);
        }

        public StatCollectionType StatCollectionType
        {
            get { return _statCollectionType; }
        }

        #endregion
    }
}