using System;
using System.Collections.Generic;

namespace ParkShark.Infrastructure
{
    /// <summary>
    /// The mapping key, for internal use in the dictionary
    /// </summary>
    class MappingKey
    {
        public Type From { get; set; }
        public Type To { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as MappingKey;
            if (other == null)
                return false;

            return this.To == other.To && this.From == other.From;
        }

        public override int GetHashCode()
        {
            return From.GetHashCode() + To.GetHashCode();
        }
    }

    public class Mapping<T1, T2> where T1 : class
        where T2 : class
    {
        private readonly Func<T1, Mapper, T2> mapping;
        public Mapping(Func<T1, Mapper, T2> mapping)
        {
            this.mapping = mapping;
        }
    }

    /// <summary>
    /// This is the Mapper class, it will map any object from A to B
    /// </summary>
    public class Mapper
    {
        /// <summary>
        /// The registry of silly mappings
        /// </summary>
        private Dictionary<MappingKey, Func<object, object>> mappings = new Dictionary<MappingKey, Func<object, object>>();

        /// <summary>
        /// Use this function to create a map from type A to type B
        /// </summary>
        /// <typeparam name="TFrom">Type A</typeparam>
        /// <typeparam name="TTo">Type B</typeparam>
        /// <param name="mapping">The actual mapping code</param>
        public Mapping<TFrom, TTo> CreateMap<TFrom, TTo>(Func<TFrom, Mapper, TTo> mapping) where TFrom : class
            where TTo : class
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            var key = new MappingKey
            {
                From = fromType,
                To = toType
            };

            if (mappings.ContainsKey(key))
                throw new NotSupportedException($"Mapping from {fromType.Name} to {toType.Name} already in registry.");

            //func translation
            object Translated(object a) => mapping(a as TFrom, this);

            mappings.Add(key, Translated);

            return new Mapping<TFrom, TTo>(mapping);
        }

        /// <summary>
        /// Resolves a mapping created from type TFrom to type TTo.
        /// </summary>
        /// <typeparam name="TTo">The requested mapping result</typeparam>
        /// <typeparam name="TFrom">The input type</typeparam>
        /// <param name="from">The input object, TFrom</param>
        /// <returns>A type TTo object, in case mapping was successful</returns>
        public TTo MapTo<TTo, TFrom>(TFrom from)
            where TTo : class
            where TFrom : class
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            var key = new MappingKey
            {
                From = fromType,
                To = toType
            };

            if (!mappings.ContainsKey(key))
                throw new NotSupportedException($"Mapping from {fromType.Name} to {toType.Name} not supported.");

            var mapping = mappings[key];
            var mapped = mapping(from);

            if (mapped == null)
                throw new NotSupportedException($"Mapping resulted in null for {fromType.Name} to {toType.Name}.");

            if (!(mapped is TTo))
                throw new NotSupportedException($"Mapping resulted in not supported result for {fromType.Name} to {toType.Name}.");

            return mapped as TTo;
        }

        /// <summary>
        /// Resolves a mapping created from type TFrom to type TTo.
        /// </summary>
        /// <typeparam name="TTo">The requested mapping result</typeparam>
        /// <typeparam name="TFrom">The input type</typeparam>
        /// <param name="from">The input object, TFrom</param>
        /// <returns>A type TTo object, in case mapping was successful</returns>
        public IEnumerable<TTo> MapToList<TTo, TFrom>(IEnumerable<TFrom> from)
            where TTo : class
            where TFrom : class
        {
            List<TTo> mappedList = new List<TTo>();
            foreach (var item in from)
            {
                mappedList.Add(MapTo<TTo, TFrom>(item));
            }

            return mappedList;
        }
    }
}
