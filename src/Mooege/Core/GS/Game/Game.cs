﻿/*
 * Copyright (C) 2011 mooege project
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Mooege.Common;
using Mooege.Core.GS.Objects;
using Mooege.Core.GS.Generators;
using Mooege.Core.GS.Map;
using Mooege.Net.GS;
using Mooege.Net.GS.Message;
using Mooege.Net.GS.Message.Definitions.Player;

// TODO: Move scene stuff into a Map class (which can also handle the efficiency stuff and object grouping)

namespace Mooege.Core.GS.Game
{
    public class Game : IMessageConsumer
    {
        static readonly Logger Logger = LogManager.CreateLogger();

        public int GameId { get; private set; }

        public PowerManager PowerManager { get; private set; }

        public ConcurrentDictionary<GameClient, Player.Player> Players = new ConcurrentDictionary<GameClient, Player.Player>();

        public int PlayerIndexCounter = -1;

        private readonly Dictionary<uint, DynamicObject> _objects;
        private readonly Dictionary<int, World> _worlds; // NOTE: This tracks by WorldSNO rather than by DynamicID; this.Objects _does_ still contain the world since it is a DynamicObject

        public int StartWorldSNO { get; private set; }
        public World StartWorld { get { return GetWorld(this.StartWorldSNO); } }

        public readonly int TicksPerSecond = 30;
        private Thread _tickThread;

        private readonly WorldGenerator WorldGenerator;

        private uint _lastObjectID = 0x00000001;
        private uint _lastSceneID  = 0x04000000;
        private uint _lastWorldID  = 0x07000000;

        // TODO: Need overrun handling and existence checking
        public uint NewObjectID { get { return _lastObjectID++; } }
        public uint NewSceneID { get { return _lastSceneID++; } }
        public uint NewWorldID { get { return _lastWorldID++; } }

        public Game(int gameId)
        {
            this.GameId = gameId;
            this._objects = new Dictionary<uint, DynamicObject>();
            this._worlds = new Dictionary<int, World>();
            this.WorldGenerator = new WorldGenerator(this);
            this.PowerManager = new PowerManager(this);
            // FIXME: This must be set according to the game settings (start quest/act). Better yet, track the player's save point and toss this stuff
            this.StartWorldSNO = 71150;

            //Quick implementation of gametick
            _tickThread = new Thread(() => _tickThread_Run());
            _tickThread.Start();
        }

        public void _tickThread_Run()
        {
            // TODO: needs to have exit condition, probably either PlayerManager.Players.Count or a manual shutdown flag
            while (true)
            {
                lock (this)
                {
                    PowerManager.Tick();
                }
                Thread.Sleep(1000 / TicksPerSecond);
            }
        }

        public void Route(GameClient client, GameMessage message)
        {
            switch (message.Consumer)
            {
                case Consumers.Game:
                    this.Consume(client, message);
                    break;
                case Consumers.Inventory:
                    client.Player.Inventory.Consume(client, message);
                    break;
                case Consumers.Player:
                    client.Player.Consume(client, message);
                    break;
              }
        }

        public void Consume(GameClient client, GameMessage message)
        {
            // for possile future messages consumed by game.
        }

        public void Enter(Player.Player joinedPlayer)
        {
            this.Players.TryAdd(joinedPlayer.InGameClient, joinedPlayer);

            // send all players in the game to new player that just joined (including him)
            foreach (var pair in this.Players)
            {
                this.SendNewPlayerMessage(joinedPlayer, pair.Value);
            }

            // notify other players about or new player too.
            foreach (var pair in this.Players.Where(pair => pair.Value != joinedPlayer))
            {
                this.SendNewPlayerMessage(pair.Value, joinedPlayer);
            }

            joinedPlayer.World.Enter(joinedPlayer); // Enter only once all fields have been initialized to prevent a run condition
        }

        private void SendNewPlayerMessage(Player.Player target, Player.Player joinedPlayer)
        {
            target.InGameClient.SendMessage(new NewPlayerMessage
            {
                PlayerIndex = joinedPlayer.PlayerIndex, // player index
                Field1 = "", //Owner name?
                ToonName = joinedPlayer.Properties.Name,
                Field3 = 0x00000002, //party frame class
                Field4 = 0x00000004, //party frame level
                snoActorPortrait = joinedPlayer.ClassSNO, //party frame portrait
                Field6 = 0x00000001,
                StateData = joinedPlayer.GetStateData(),
                Field8 = this.Players.Count != 1, //announce party join
                Field9 = 0x00000001,
                ActorID = joinedPlayer.DynamicID,
            });
            Logger.Debug("{0}[PlayerIndex: {1}] is notified about {2}[PlayerIndex: {3}] joining the game.", target.Properties.Name, target.PlayerIndex, joinedPlayer.Properties.Name, joinedPlayer.PlayerIndex);
        }


        #region Tracking

        public void StartTracking(DynamicObject obj)
        {
            if (obj.DynamicID == 0 || IsTracking(obj))
                throw new Exception(String.Format("Object has an invalid ID or was already being tracked (ID = {0})", obj.DynamicID));
            this._objects.Add(obj.DynamicID, obj);
        }

        public void EndTracking(DynamicObject obj)
        {
            if (obj.DynamicID == 0 || !IsTracking(obj))
                throw new Exception(String.Format("Object has an invalid ID or was not being tracked (ID = {0})", obj.DynamicID));
            this._objects.Remove(obj.DynamicID);
        }

        public DynamicObject GetObject(uint dynamicID)
        {
            DynamicObject obj;
            this._objects.TryGetValue(dynamicID, out obj);
            return obj;
        }

        public bool IsTracking(uint dynamicID)
        {
            return this._objects.ContainsKey(dynamicID);
        }

        public bool IsTracking(DynamicObject obj)
        {
            return this._objects.ContainsKey(obj.DynamicID);
        }

        #endregion // Tracking

        #region World collection

        public void AddWorld(World world)
        {
            if (world.WorldSNO == -1 || WorldExists(world.WorldSNO))
                throw new Exception(String.Format("World has an invalid SNO or was already being tracked (ID = {0}, SNO = {1})", world.DynamicID, world.WorldSNO));
            this._worlds.Add(world.WorldSNO, world);
        }

        public void RemoveWorld(World world)
        {
            if (world.WorldSNO == -1 || !WorldExists(world.WorldSNO))
                throw new Exception(String.Format("World has an invalid SNO or was not being tracked (ID = {0}, SNO = {1})", world.DynamicID, world.WorldSNO));
            this._worlds.Remove(world.WorldSNO);
        }

        public World GetWorld(int worldSNO)
        {
            World world;
            this._worlds.TryGetValue(worldSNO, out world);
            // If it doesn't exist, try to load it
            if (world == null)
            {
                world = this.WorldGenerator.GenerateWorld(worldSNO);
                if (world == null)
                    Logger.Warn(String.Format("Failed to generate world (SNO = {0})", worldSNO));
            }
            return world;
        }

        public bool WorldExists(int worldSNO)
        {
            return this._worlds.ContainsKey(worldSNO);
        }

        #endregion // World collection
    }
}
