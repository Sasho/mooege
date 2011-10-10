/*
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
using Mooege.Common.Helpers;
using Mooege.Core.GS.Game;
using Mooege.Core.GS.Map;
using Mooege.Core.GS.Actors;
using Mooege.Net.GS;
using Mooege.Net.GS.Message;
using Mooege.Net.GS.Message.Fields;
using Mooege.Net.GS.Message.Definitions.ACD;
using Mooege.Net.GS.Message.Definitions.Animation;
using Mooege.Net.GS.Message.Definitions.Attribute;
using Mooege.Net.GS.Message.Definitions.Combat;
using Mooege.Net.GS.Message.Definitions.Effect;
using Mooege.Net.GS.Message.Definitions.Misc;

namespace Mooege.Core.GS.Actors
{
    public class Proxy : Actor
    {
        public override ActorType ActorType { get { return ActorType.Monster; } }

        // TODO: Setter needs to update world. Also, this is probably an ACD field. /komiga
        public int AnimationSNO { get; set; }

        public Proxy(World world, int actorSNO, Vector3D position)
            : base(world, world.NewActorID)
        {
            this.ActorSNO = actorSNO;
            // FIXME: This is hardcoded crap
            this.Field2 = 0x8;
            this.Field3 = 0x0;
            this.Scale = 1.35f;
            this.Position.Set(position);
            this.RotationAmount = (float)(RandomHelper.NextDouble() * 2.0f * Math.PI);
            this.RotationAxis.X = 0f; this.RotationAxis.Y = 0f; this.RotationAxis.Z = 1f;
            this.GBHandle.Type = (int)GBHandleType.Monster; this.GBHandle.GBID = 1;
            this.Field7 = 0x00000001;
            this.Field8 = this.ActorSNO;
            this.Field10 = 0x0;
            this.Field11 = 0x0;
            this.Field12 = 0x0;
            this.Field13 = 0x0;
            this.AnimationSNO = 0x11150;

            this.Attributes[GameAttribute.Is_Power_Proxy] = true;

            this.World.Enter(this); // Enter only once all fields have been initialized to prevent a run condition
        }

        public override void OnTargeted(Mooege.Core.GS.Player.Player player, TargetMessage message)
        {
            //this.Die(player);
            //Temp route to powermanager
        }

        public override bool Reveal(Mooege.Core.GS.Player.Player player)
        {
            if (!base.Reveal(player))
                return false;

            player.InGameClient.SendMessage(new SetIdleAnimationMessage
            {
                ActorID = this.DynamicID,
                AnimationSNO = this.AnimationSNO
            });

            player.InGameClient.PacketId += 30 * 2;
            player.InGameClient.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = player.InGameClient.PacketId,
            });
            player.InGameClient.Tick += 20;
            player.InGameClient.SendMessage(new EndOfTickMessage()
            {
                Id = 0x008D,
                Field0 = player.InGameClient.Tick - 20,
                Field1 = player.InGameClient.Tick
            });
            player.InGameClient.FlushOutgoingBuffer();
            return true;
        }

        // FIXME: Hardcoded hell. /komiga
        public void Die(Mooege.Core.GS.Player.Player player)
        {  
            this.Destroy();
        }
    }
}
