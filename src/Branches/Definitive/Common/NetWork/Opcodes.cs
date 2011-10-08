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

public enum Opcodes
{
    ProtocolHandshakeVersion = 0x01B7,

    ProtocolPingPong = 0x0002,
    ProtocolHandshakeCompression = 0x0019,

    ProtocolHandshakeClientKey = 0x040A,
    ProtocolHandshakeServerKey = 0x040B,

    ProtocolHandshakeAuthenticationRequest = 0x01C5,
    ProtocolHandshakeAuthenticationResponse = 0x01AF,

    LobbyWorldListRequest = 0x01C0,
    LobbyWorldListResponse = 0x01C1,
    LobbyWorldEntry = 0x01E5,

    LobbyWorldSelectRequest = 0x019D,
    LobbyWorldSelectResponse = 0x019E,

    LobbyCharacterListRequest = 0x01B3,
    LobbyCharacterListResponse = 0x01B4,

    LobbyCharacterDeleteRequest = 0x01E8,
    LobbyCharacterDeleteResponse = 0x01E9,

    LobbyCharacterEntry = 0x01D7,
    LobbyCharacterInfo = 0x01DD,
    CharacterInfoDesc = 0x0DBF,
    CharacterInfoCache = 0x0E10,
    LobbyCharacterCustom = 0x00DE,

    LobbyCharacterCreationCacheRequest = 0x01C2,
    LobbyCharacterCreationCacheResponse = 0x01C3,

    LobbyCharacterCreateRequest = 0x01D5,
    LobbyCharacterCreateResponse = 0x01D6,

    LobbyCharacterRandomNameRequest = 0x01EF,
    LobbyCharacterRandomNameResponse = 0x01F0,

    CacheUpdate = 0x0025,
    CacheTemplate = 0x027E,
    CacheData = 0x0274,
    CacheReference = 0x1C9C,
    TemplateCreationUnknown1Data = 0x0ECD,
    TemplateCreationSubUnknown1Data = 0x1C97,
    TemplateCreationSubUnknown2Data = 0x1C9D,
    TemplateCreationSubUnknown4Data = 0x1CD2,
    
    WorldText = 0x1E17,

    LobbyCharacterSelectRequest = 0x01B1,
    LobbyCharacterSelectResponse = 0x01B2,

    IPCWorldRegisterRequest = 0x010000,
    IPCWorldRegisterResponse = 0x010001,
    IPCWorldPopulationUpdate = 0x010002,

    WorldAuthenticationRequest = 0x03E8,
    WorldAuthenticationResponse = 0x03E9,
    WorldCacheUpdated = 0x03EA,

    WorldZoneInfo = 0x0080,
    WorldMapInfo = 0x1E17,
    WorldStartingPosition = 0x03EF,
    WorldEntityUpdate = 0x03EC,
    WorldStartingPositionExtra = 0x03ED,
    WorldPositionExtra = 0x025B,

    WorldMapLoaded = 0x0DAF,
    WorldCanConnect = 0x1E99,
    WorldTemplateUpdate = 0x0271,
    WorldChannelJoinned = 0x10CE,

    WorldServerMOTD = 0x010E9,

    WorldServerPositionUpdate = 0x0007,
    WorldPorticulumTeleport = 0x0E55,
    WorldPorticulumConfirm = 0x11CE,
    WorldTeleport = 0x0F04,
    WorldChatMessage = 0x010CC,

    WorldCharacterInformation = 0x02AE,
}