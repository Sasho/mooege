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

namespace Mooege.Core.GS.Powers
{
    public enum DamageType : int
    {
        Normal_fast = 0,
        Normal_medium = 1,
        Normal_slow = 2,
        Yellow = 3,
        Red = 4,
        Dodge = 6,
        Dodged = 7,
        Blocked = 8,
        Parray = 9,
        Green_heal = 10,
        Absorbed = 11,
        Rooted = 12,
        Stunned = 13,
        Blinded = 14,
        Frozen = 15,
        Feared = 16,
        Charmed = 17,
        Taunted = 18,
        Snared = 19,
        Attack_slowed = 20,
        Broke_freeze = 21,
        Broke_blind = 22,
        Broke_stunt = 23,
        Broke_root = 24,
        Broke_snare = 25,
        Broke_fear = 26
    }
}

