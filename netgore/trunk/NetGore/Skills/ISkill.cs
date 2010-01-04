﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetGore
{
    /// <summary>
    /// Interface for an object that describes a skill used by Characters.
    /// </summary>
    /// <typeparam name="TSkillType">The type of skill type enum.</typeparam>
    /// <typeparam name="TStatType">The type of stat type enum.</typeparam>
    /// <typeparam name="TCharacter">The type of character.</typeparam>
    public interface ISkill<TSkillType, TStatType, TCharacter>
        where TSkillType : struct, IComparable, IConvertible, IFormattable
        where TStatType : struct, IComparable, IConvertible, IFormattable
        where TCharacter : Entity
    {
        /// <summary>
        /// Gets an IEnumerable of stats required by this ISkill. Can be empty, but cannot
        /// be null.
        /// </summary>
        IEnumerable<KeyValuePair<TStatType, int>> RequiredStats { get; }

        /// <summary>
        /// Gets if this skill requires a target to be specified for the skill to be used. If this is true,
        /// the skill will never even attempt to be used unless there is a target.
        /// </summary>
        bool RequiresTarget { get; }

        /// <summary>
        /// Gets the type of skill that this class is for.
        /// </summary>
        TSkillType SkillType { get; }

        /// <summary>
        /// Checks if the given Character can use this Skill.
        /// </summary>
        /// <param name="user">The Character to check if can use this Skill.</param>
        /// <returns>True if the <paramref name="user"/> can use this Skill; otherwise false.</returns>
        bool CanUse(TCharacter user);

        /// <summary>
        /// Checks if the given Character can use this Skill.
        /// </summary>
        /// <param name="user">The Character to check if can use this Skill.</param>
        /// <param name="target">The optional Character that the skill was used on. Can be null if there was
        /// no targeted Character.</param>
        /// <returns>True if the <paramref name="user"/> can use this Skill; otherwise false.</returns>
        bool CanUse(TCharacter user, TCharacter target);

        /// <summary>
        /// Checks if the given <paramref name="character"/> has the required stats for using this Skill.
        /// </summary>
        /// <param name="character">The Character using the skill. Will not be null.</param>
        /// <returns>True if the <paramref name="character"/> has the required stats to use this skill; otherwise false.</returns>
        bool HasRequiredStats(TCharacter character);

        /// <summary>
        /// Uses this Skill without a target.
        /// </summary>
        /// <param name="user">User to make use this Skill.</param>
        /// <returns>True if the Skill was successfully used; otherwise false.</returns>
        bool Use(TCharacter user);

        /// <summary>
        /// Uses this Skill.
        /// </summary>
        /// <param name="user">User to make use this Skill.</param>
        /// <param name="target">The optional Character that the skill was used on. Can be null if there was
        /// no targeted Character.</param>
        /// <returns>True if the Skill was successfully used; otherwise false.</returns>
        bool Use(TCharacter user, TCharacter target);
    }

}
