/********************************************************************
                   DO NOT MANUALLY EDIT THIS FILE!

This file was automatically generated using the DbClassCreator
program. The only time you should ever alter this file is if you are
using an automated code formatter. The DbClassCreator will overwrite
this file every time it is run, so all manual changes will be lost.
If there is something in this file that you wish to change, you should
be able to do it through the DbClassCreator arguments.

Make sure that you re-run the DbClassCreator every time you alter your
game's database.

For more information on the DbClassCreator, please see:
    http://www.netgore.com/wiki/dbclasscreator.html
********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using DemoGame.DbObjs;
using NetGore;
using NetGore.IO;

namespace DemoGame.Server.DbObjs
{
    /// <summary>
    /// Provides a strongly-typed structure for the database table `world_stats_count_item_buy`.
    /// </summary>
    public class WorldStatsCountItemBuyTable : IWorldStatsCountItemBuyTable, IPersistable
    {
        /// <summary>
        /// The number of columns in the database table that this class represents.
        /// </summary>
        public const Int32 ColumnCount = 3;

        /// <summary>
        /// The name of the database table that this class represents.
        /// </summary>
        public const String TableName = "world_stats_count_item_buy";

        /// <summary>
        /// Array of the database column names.
        /// </summary>
        static readonly String[] _dbColumns = new string[] { "count", "item_template_id", "last_update" };

        /// <summary>
        /// Array of the database column names for columns that are primary keys.
        /// </summary>
        static readonly String[] _dbColumnsKeys = new string[] { "item_template_id" };

        /// <summary>
        /// Array of the database column names for columns that are not primary keys.
        /// </summary>
        static readonly String[] _dbColumnsNonKey = new string[] { "count", "last_update" };

        /// <summary>
        /// The field that maps onto the database column `count`.
        /// </summary>
        Int32 _count;

        /// <summary>
        /// The field that maps onto the database column `item_template_id`.
        /// </summary>
        UInt16 _itemTemplateID;

        /// <summary>
        /// The field that maps onto the database column `last_update`.
        /// </summary>
        DateTime _lastUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldStatsCountItemBuyTable"/> class.
        /// </summary>
        public WorldStatsCountItemBuyTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldStatsCountItemBuyTable"/> class.
        /// </summary>
        /// <param name="count">The initial value for the corresponding property.</param>
        /// <param name="itemTemplateID">The initial value for the corresponding property.</param>
        /// <param name="lastUpdate">The initial value for the corresponding property.</param>
        public WorldStatsCountItemBuyTable(Int32 @count, ItemTemplateID @itemTemplateID, DateTime @lastUpdate)
        {
            Count = @count;
            ItemTemplateID = @itemTemplateID;
            LastUpdate = @lastUpdate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldStatsCountItemBuyTable"/> class.
        /// </summary>
        /// <param name="source">IWorldStatsCountItemBuyTable to copy the initial values from.</param>
        public WorldStatsCountItemBuyTable(IWorldStatsCountItemBuyTable source)
        {
            CopyValuesFrom(source);
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the names of the database columns for the table that this class represents.
        /// </summary>
        public static IEnumerable<String> DbColumns
        {
            get { return _dbColumns; }
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the names of the database columns that are primary keys.
        /// </summary>
        public static IEnumerable<String> DbKeyColumns
        {
            get { return _dbColumnsKeys; }
        }

        /// <summary>
        /// Gets an IEnumerable of strings containing the names of the database columns that are not primary keys.
        /// </summary>
        public static IEnumerable<String> DbNonKeyColumns
        {
            get { return _dbColumnsNonKey; }
        }

        /// <summary>
        /// Copies the column values into the given Dictionary using the database column name
        /// with a prefixed @ as the key. The keys must already exist in the Dictionary;
        /// this method will not create them if they are missing.
        /// </summary>
        /// <param name="source">The object to copy the values from.</param>
        /// <param name="dic">The Dictionary to copy the values into.</param>
        public static void CopyValues(IWorldStatsCountItemBuyTable source, IDictionary<String, Object> dic)
        {
            dic["count"] = source.Count;
            dic["item_template_id"] = source.ItemTemplateID;
            dic["last_update"] = source.LastUpdate;
        }

        /// <summary>
        /// Copies the column values into the given Dictionary using the database column name
        /// with a prefixed @ as the key. The keys must already exist in the Dictionary;
        /// this method will not create them if they are missing.
        /// </summary>
        /// <param name="dic">The Dictionary to copy the values into.</param>
        public void CopyValues(IDictionary<String, Object> dic)
        {
            CopyValues(this, dic);
        }

        /// <summary>
        /// Copies the values from the given <paramref name="source"/> into this WorldStatsCountItemBuyTable.
        /// </summary>
        /// <param name="source">The IWorldStatsCountItemBuyTable to copy the values from.</param>
        public void CopyValuesFrom(IWorldStatsCountItemBuyTable source)
        {
            Count = source.Count;
            ItemTemplateID = source.ItemTemplateID;
            LastUpdate = source.LastUpdate;
        }

        /// <summary>
        /// Gets the data for the database column that this table represents.
        /// </summary>
        /// <param name="columnName">The database name of the column to get the data for.</param>
        /// <returns>
        /// The data for the database column with the name <paramref name="columnName"/>.
        /// </returns>
        public static ColumnMetadata GetColumnData(String columnName)
        {
            switch (columnName)
            {
                case "count":
                    return new ColumnMetadata("count", "", "int(11)", "0", typeof(Int32), false, false, false);

                case "item_template_id":
                    return new ColumnMetadata("item_template_id", "", "smallint(5) unsigned", null, typeof(UInt16), false, true,
                                              false);

                case "last_update":
                    return new ColumnMetadata("last_update", "", "timestamp", "0000-00-00 00:00:00", typeof(DateTime), false,
                                              false, false);

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

        /// <summary>
        /// Gets the value of a column by the database column's name.
        /// </summary>
        /// <param name="columnName">The database name of the column to get the value for.</param>
        /// <returns>
        /// The value of the column with the name <paramref name="columnName"/>.
        /// </returns>
        public Object GetValue(String columnName)
        {
            switch (columnName)
            {
                case "count":
                    return Count;

                case "item_template_id":
                    return ItemTemplateID;

                case "last_update":
                    return LastUpdate;

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

        /// <summary>
        /// Sets the <paramref name="value"/> of a column by the database column's name.
        /// </summary>
        /// <param name="columnName">The database name of the column to get the <paramref name="value"/> for.</param>
        /// <param name="value">Value to assign to the column.</param>
        public void SetValue(String columnName, Object value)
        {
            switch (columnName)
            {
                case "count":
                    Count = (Int32)value;
                    break;

                case "item_template_id":
                    ItemTemplateID = (ItemTemplateID)value;
                    break;

                case "last_update":
                    LastUpdate = (DateTime)value;
                    break;

                default:
                    throw new ArgumentException("Field not found.", "columnName");
            }
        }

        #region IPersistable Members

        /// <summary>
        /// Reads the state of the object from an <see cref="IValueReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="IValueReader"/> to read the values from.</param>
        public virtual void ReadState(IValueReader reader)
        {
            PersistableHelper.Read(this, reader);
        }

        /// <summary>
        /// Writes the state of the object to an <see cref="IValueWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="IValueWriter"/> to write the values to.</param>
        public virtual void WriteState(IValueWriter writer)
        {
            PersistableHelper.Write(this, writer);
        }

        #endregion

        #region IWorldStatsCountItemBuyTable Members

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `count`.
        /// The underlying database type is `int(11)` with the default value of `0`.
        /// </summary>
        [SyncValue]
        public Int32 Count
        {
            get { return _count; }
            set { _count = value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `item_template_id`.
        /// The underlying database type is `smallint(5) unsigned`.
        /// </summary>
        [SyncValue]
        public ItemTemplateID ItemTemplateID
        {
            get { return (ItemTemplateID)_itemTemplateID; }
            set { _itemTemplateID = (UInt16)value; }
        }

        /// <summary>
        /// Gets or sets the value for the field that maps onto the database column `last_update`.
        /// The underlying database type is `timestamp` with the default value of `0000-00-00 00:00:00`.
        /// </summary>
        [SyncValue]
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }

        /// <summary>
        /// Creates a deep copy of this table. All the values will be the same
        /// but they will be contained in a different object instance.
        /// </summary>
        /// <returns>
        /// A deep copy of this table.
        /// </returns>
        public virtual IWorldStatsCountItemBuyTable DeepCopy()
        {
            return new WorldStatsCountItemBuyTable(this);
        }

        #endregion
    }
}