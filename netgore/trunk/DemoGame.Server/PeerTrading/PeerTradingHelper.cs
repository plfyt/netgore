﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using DemoGame.Server.DbObjs;
using DemoGame.Server.Queries;
using log4net;
using NetGore;
using NetGore.Db;

namespace DemoGame.Server.PeerTrading
{
    /// <summary>
    /// Helper methods for the peer trading.
    /// </summary>
    public static class PeerTradingHelper
    {
        /// <summary>
        /// The <see cref="PeerTradingGetLostItemsQuery"/> instance to use.
        /// </summary>
        static readonly PeerTradingGetLostItemsQuery _getLostItemsQuery;

        /// <summary>
        /// The <see cref="PeerTradingInsertItemQuery"/> instance to use.
        /// </summary>
        static readonly PeerTradingInsertItemQuery _insertItemQuery;

        /// <summary>
        /// The <see cref="PeerTradingRemoveItemQuery"/> instance to use.
        /// </summary>
        static readonly PeerTradingRemoveItemQuery _removeItemQuery;

        /// <summary>
        /// Initializes the <see cref="PeerTradingHelper"/> class.
        /// </summary>
        static PeerTradingHelper()
        {
            // Cache the queries we will need for this class
            var dbController = DbControllerBase.GetInstance();
            _getLostItemsQuery = dbController.GetQuery<PeerTradingGetLostItemsQuery>();
            _insertItemQuery = dbController.GetQuery<PeerTradingInsertItemQuery>();
            _removeItemQuery = dbController.GetQuery<PeerTradingRemoveItemQuery>();
        }

        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Recovers items from the <see cref="ActiveTradeItemTable"/> for a character, and gives them to the character. Items can
        /// get stuck in this table when the server unexpectedly shuts down during a peer trade session. Calling this method
        /// will attempt to give the <see cref="Character"/> any items they might have in this table. This cannot be done if the
        /// <paramref name="character"/> is currently trading.
        /// </summary>
        /// <param name="character">The <see cref="Character"/> to get the lost items for.</param>
        /// <exception cref="ArgumentNullException"><paramref name="character"/> is null.</exception>
        public static void RecoverLostTradeItems(User character)
        {
            int recovered;
            int remaining;
            RecoverLostTradeItems(character, out recovered, out remaining);
        }

        /// <summary>
        /// Recovers items from the <see cref="ActiveTradeItemTable"/> for a character, and gives them to the character. Items can
        /// get stuck in this table when the server unexpectedly shuts down during a peer trade session. Calling this method
        /// will attempt to give the <see cref="Character"/> any items they might have in this table. This cannot be done if the
        /// <paramref name="character"/> is currently trading.
        /// </summary>
        /// <param name="character">The <see cref="Character"/> to get the lost items for.</param>
        /// <param name="recovered">The number of items that were able to be given back to the <paramref name="character"/>.</param>
        /// <param name="remaining">The number of items still remaining in the table for the <paramref name="character"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="character"/> is null.</exception>
        public static void RecoverLostTradeItems(User character, out int recovered, out int remaining)
        {
            recovered = 0;

            if (character == null)
                throw new ArgumentNullException("character");

            // Make sure that we do not try executing this while they are trading! Doing so would be very, very ugly.
            if (character.IsPeerTrading)
            {
                const string errmsg = "Tried to recover lost trade items for `{0}` while they were trading. Very bad idea...";
                if (log.IsWarnEnabled)
                    log.WarnFormat(errmsg, character);
                remaining = 0;
                return;
            }

            // Get the list of lost items
            var lostItemIDs = _getLostItemsQuery.Execute(character.ID);

            // Check if there are even any items to recover
            remaining = lostItemIDs.Count();
            if (remaining == 0)
                return;

            // One by one, try to add the lost items back to the character
            foreach (var lostItemID in lostItemIDs)
            {
                // Create the item instance
                var lostItem = ItemEntity.LoadFromDatabase(lostItemID);

                // Remove the entry from the table containing the lost items
                _removeItemQuery.Execute(lostItemID);

                remaining--;

                // Make sure the item was valid (which it should always be unless something went horribly wrong)
                // We do NOT try to add it back again, since who knows what is wrong with the item... its just not worth risking
                if (lostItem == null)
                {
                    Debug.Fail("Why did we fail to load the item... from a RELATIONAL database!?");
                    continue;
                }

                recovered++;

                // Try to give the item to the character
                var lostItemRemainder = character.GiveItem(lostItem);

                // If there was a remainder when we tried to give the character the item, then we will have to put it back into the
                // database table so we can try to give it to them again another time.
                if (lostItemRemainder != null)
                {
                    _insertItemQuery.Execute(new PeerTradingInsertItemQuery.QueryArgs(lostItemRemainder.ID, character.ID));
                    remaining++;
                    break;
                }
            }

            // Notify the character about the items recovered and remaining
            if (remaining > 0)
                character.Send(GameMessage.PeerTradingItemsRecovered, recovered, remaining);
            else
                character.Send(GameMessage.PeerTradingItemsRecoveredNoRemaining, recovered);
        }
    }
}