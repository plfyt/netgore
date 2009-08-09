using System;
using System.Linq;
namespace DemoGame.Server.DbObjs
{
/// <summary>
/// Interface for a class that can be used to serialize values to the database table `character_template`.
/// </summary>
public interface ICharacterTemplateTable
{
/// <summary>
/// Gets the value of the database column in the column collection `{0}`
/// that corresponds to the given <paramref name="key"/>.
/// </summary>
/// <param name="key">The key that represents the column in this column collection.</param>
/// <returns>
/// The value of the database column with the corresponding <paramref name="key"/>.
/// </returns>
System.Int32 GetStat(DemoGame.StatType key);

/// <summary>
/// Gets the <paramref name="value"/> of the database column in the column collection `{0}`
/// that corresponds to the given <paramref name="key"/>.
/// </summary>
/// <param name="key">The key that represents the column in this column collection.</param>
/// <param name="value">The value to assign to the column with the corresponding <paramref name="key"/>.</param>
void SetStat(DemoGame.StatType key, System.Int32 value);

System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<DemoGame.StatType, System.Int32>> Stats
{
get;
}
/// <summary>
/// Gets the value of the database column `ai`.
/// </summary>
System.String AI
{
get;
}
/// <summary>
/// Gets the value of the database column `alliance_id`.
/// </summary>
DemoGame.Server.AllianceID AllianceID
{
get;
}
/// <summary>
/// Gets the value of the database column `body_id`.
/// </summary>
DemoGame.BodyIndex BodyID
{
get;
}
/// <summary>
/// Gets the value of the database column `exp`.
/// </summary>
System.UInt32 Exp
{
get;
}
/// <summary>
/// Gets the value of the database column `give_cash`.
/// </summary>
System.UInt16 GiveCash
{
get;
}
/// <summary>
/// Gets the value of the database column `give_exp`.
/// </summary>
System.UInt16 GiveExp
{
get;
}
/// <summary>
/// Gets the value of the database column `id`.
/// </summary>
System.UInt16 ID
{
get;
}
/// <summary>
/// Gets the value of the database column `level`.
/// </summary>
System.Byte Level
{
get;
}
/// <summary>
/// Gets the value of the database column `name`.
/// </summary>
System.String Name
{
get;
}
/// <summary>
/// Gets the value of the database column `respawn`.
/// </summary>
System.UInt16 Respawn
{
get;
}
/// <summary>
/// Gets the value of the database column `statpoints`.
/// </summary>
System.UInt32 StatPoints
{
get;
}
}

}
