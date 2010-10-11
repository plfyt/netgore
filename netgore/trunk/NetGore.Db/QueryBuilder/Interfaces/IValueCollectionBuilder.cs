﻿using System;
using System.Collections.Generic;

namespace NetGore.Db.QueryBuilder
{
    /// <summary>
    /// Interface for an object that can be used to build a collection of values.
    /// </summary>
    /// <typeparam name="T">The type to return.</typeparam>
    public interface IValueCollectionBuilder<out T>
    {
        /// <summary>
        /// Adds a raw value to the collection.
        /// </summary>
        /// <param name="value">The raw value string.</param>
        /// <returns>The return object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        T Add(string value);

        /// <summary>
        /// Adds raw values to the collection.
        /// </summary>
        /// <param name="values">The raw value strings.</param>
        /// <returns>The return object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        T Add(IEnumerable<string> values);

        /// <summary>
        /// Adds raw values to the collection.
        /// </summary>
        /// <param name="values">The raw value strings.</param>
        /// <returns>The return object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null.</exception>
        T Add(params string[] values);
    }
}