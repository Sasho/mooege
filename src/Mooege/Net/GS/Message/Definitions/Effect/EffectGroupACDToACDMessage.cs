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

using System.Text;

namespace Mooege.Net.GS.Message.Definitions.Effect
{
    public class EffectGroupACDToACDMessage : GameMessage
    {
        public int effectSNO;
        public uint fromActorID;
        public uint toActorID;




        public override void Parse(GameBitBuffer buffer)
        {
            effectSNO = buffer.ReadInt(32);
            fromActorID = buffer.ReadUInt(32);
            toActorID = buffer.ReadUInt(32);
        }

        public override void Encode(GameBitBuffer buffer)
        {
            buffer.WriteInt(32, effectSNO);
            buffer.WriteUInt(32, fromActorID);
            buffer.WriteUInt(32, toActorID);
        }

        public override void AsText(StringBuilder b, int pad)
        {
            b.Append(' ', pad);
            b.AppendLine("EffectGroupACDToACDMessage:");
            b.Append(' ', pad++);
            b.AppendLine("{");
            b.Append(' ', pad); b.AppendLine("Field0: 0x" + effectSNO.ToString("X8"));
            b.Append(' ', pad); b.AppendLine("Field1: 0x" + fromActorID.ToString("X8") + " (" + fromActorID + ")");
            b.Append(' ', pad); b.AppendLine("Field2: 0x" + toActorID.ToString("X8") + " (" + toActorID + ")");
            b.Append(' ', --pad);
            b.AppendLine("}");
        }


    }
}