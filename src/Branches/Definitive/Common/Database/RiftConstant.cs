/*
 * Copyright (C) 2011 APS
 *	http://AllPrivateServer.com
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public enum RiftGender
    {
        MALE = 0,
        FEMALE = 1,
    };

    public enum RiftRace
    {
        MATHOSIAN = 1,
        HIGH_ELVE = 2,
        DWARVE = 3,

        BAHMI = 2005,
        ETH = 2007,
        KELARI = 2008,
    };

    public enum WarriorSoul
    {
        CHAMPION,
        REAVER,
        PALADIN,
        WARLORD,
        PARAGON,
        RIFTBLADE,
        VOIDKNIGHT,
        BEASTMASTER,
    };

    public enum ClericSoul
    {
        PURIFIER,
        INQUISITOR,
        SENTINEL,
        JUSTICAR,
        SHAMAN,
        WARDEN,
        DRUID,
        CABALIST,
    };

    public enum RogueSoul
    {
        NIGHTBLADE,
        RANGER,
        BLADEDANCER,
        ASSASSIN,
        RIFTSTALKER,
        MARKSMAN,
        SABOTEUR,
        BARD,
    };

    public enum MageSoul
    {
        ELEMENTALIST,
        WARLOCK,
        PYROMANCER,
        STORMCALLER,
        ARCHON,
        NECROMANCER,
        DOMINATOR,
        CHLOROMANCER,
    };

}
