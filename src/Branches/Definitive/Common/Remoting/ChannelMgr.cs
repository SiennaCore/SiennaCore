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

using FrameWork;

namespace Common
{
    public enum ChannelResult
    {
        CHANNEL_OK = 0,
        CHANNEL_FULL = 1,
        CHANNEL_INVALID_PASSWORD = 2,
        CHANNEL_ALREADY_EXIST = 3,
        CHANNEL_NOT_FOUND = 4,
        CHANNEl_ERROR = 5,
        
        CHANNEL_PLAYER_NOT_FOUND = 6,
    };

    public class Channel
    {
        public delegate void OnPlayerJoinOrLeave(Channel Chan, string PlayerName, int TickCount);
        public delegate void OnMessage(Channel Chan, string PlayerName, string Message, int TickCount);

        public string ChannelName;
        public string ChannelPassword;
        public string ChannelDescription;
        public int MaxPlayers;

        public Channel(string ChannelName, string ChannelPassword, string ChannelDescription, int MaxPlayers)
        {
            this.ChannelName = ChannelName;
            this.ChannelPassword = ChannelPassword;
            this.ChannelDescription = ChannelDescription;
            this.MaxPlayers = MaxPlayers;
        }

        #region Messages

        public List<OnMessage> OnMessages = new List<OnMessage>();
        public void AddMessageEvent(OnMessage MsgEvent)
        {
            OnMessages.Add(MsgEvent);
        }
        public void RemoveMessageEvent(OnMessage MsgEvent)
        {
            OnMessages.Add(MsgEvent);
        }
        public bool DispatchMessage(string PlayerName, string Message, bool CheckExist)
        {
            if(CheckExist && !HasPlayer(PlayerName))
                return false;

            foreach (OnMessage MsgEvent in OnMessages.ToArray())
                MsgEvent.Invoke(this, PlayerName, Message, Environment.TickCount);

            return true;
        }

        #endregion

        #region Players

        public List<string> Players = new List<string>();

        public int PlayersCount
        {
            get
            {
                return Players.ToArray().Length;
            }
        }

        public bool HasPlayer(string PlayerName)
        {
            return Players.Contains(PlayerName);
        }
        public ChannelResult CanJoin(string PlayerName, string Password)
        {
            if (Password != ChannelPassword)
                return ChannelResult.CHANNEL_INVALID_PASSWORD;

            if (PlayersCount >= MaxPlayers)
                return ChannelResult.CHANNEL_FULL;

            return ChannelResult.CHANNEL_OK;
        }
        public ChannelResult Join(string PlayerName, string Password, bool Check)
        {
            ChannelResult Result = CanJoin(PlayerName, Password);
            if (Check && Result != ChannelResult.CHANNEL_OK)
                return Result;

            OnPlayerJoin(PlayerName);
            Players.Add(PlayerName);

            return ChannelResult.CHANNEL_OK;
        }
        public ChannelResult Remove(string PlayerName)
        {
            if (Players.Remove(PlayerName))
            {
                OnPlayerLeave(PlayerName);
                return ChannelResult.CHANNEL_OK;
            }
            else return ChannelResult.CHANNEL_PLAYER_NOT_FOUND;
        }

        public List<OnPlayerJoinOrLeave> PlayerJoinEvents = new List<OnPlayerJoinOrLeave>();
        public List<OnPlayerJoinOrLeave> PlayerLeaveEvents = new List<OnPlayerJoinOrLeave>();

        public void AddPlayerJoinEvent(OnPlayerJoinOrLeave PlayerEvent)
        {
            PlayerJoinEvents.Add(PlayerEvent);
        }
        public void AddPlayerLeaveEvent(OnPlayerJoinOrLeave PlayerEvent)
        {
            PlayerLeaveEvents.Add(PlayerEvent);
        }
        public void OnPlayerJoin(string PlayerName)
        {
            foreach (OnPlayerJoinOrLeave Event in PlayerJoinEvents.ToArray())
                Event.Invoke(this, PlayerName, Environment.TickCount);
        }
        public void OnPlayerLeave(string PlayerName)
        {
            foreach (OnPlayerJoinOrLeave Event in PlayerLeaveEvents.ToArray())
                Event.Invoke(this, PlayerName, Environment.TickCount);
        }

        #endregion
    }

    [Rpc(true, System.Runtime.Remoting.WellKnownObjectMode.Singleton, 1)]
    public class ChannelMgr : RpcObject
    {
        public Dictionary<string, Channel> Channels = new Dictionary<string, Channel>();

        public bool HasChannel(string ChannelName)
        {
            return Channels.ContainsKey(ChannelName);
        }
        public Channel GetChannel(string ChannelName)
        {
            Channel Chan;
            Channels.TryGetValue(ChannelName, out Chan);
            return Chan;
        }

        public ChannelResult CreateChannel(out Channel Chan,string ChannelName, string ChannelPassword, string ChannelDescription, int MaxPlayers)
        {
            Chan = null;

            if (HasChannel(ChannelName))
                return ChannelResult.CHANNEL_ALREADY_EXIST;

            Chan = new Channel(ChannelName, ChannelPassword, ChannelDescription, MaxPlayers);
            Channels.Add(ChannelName, Chan);

            return ChannelResult.CHANNEL_OK;
        }
        public ChannelResult RemoveChannel(out Channel Chan, string ChannelName)
        {
            Chan = GetChannel(ChannelName);
            if (Chan == null)
                return ChannelResult.CHANNEL_NOT_FOUND;

            Channels.Remove(ChannelName);

            return ChannelResult.CHANNEL_OK;
        }

        public ChannelResult JoinChannel(out Channel Chan, string ChannelName, string ChannelPassword, string PlayerName, int MaxPlayers, bool Create)
        {
            ChannelResult Result = ChannelResult.CHANNEL_OK;

            Chan = GetChannel(ChannelName);
            if (Chan == null && Create)
                Result = CreateChannel(out Chan, ChannelName, ChannelPassword, "", MaxPlayers);

            if (Chan == null)
                return Result;

            return Chan.Join(PlayerName, ChannelPassword, true);
        }
        public List<Channel> JoinChannels(string PlayerName, bool CheckPassword)
        {
            List<Channel> Chans = new List<Channel>();

            foreach (Channel Chan in Channels.Values.ToArray())
            {
                if (Chan.Join(PlayerName, "", CheckPassword) == ChannelResult.CHANNEL_OK)
                    Chans.Add(Chan);
            }

            return Chans;
        }

        public Channel LeaveChannel(string ChannelName, string PlayerName)
        {
            Channel Chan = GetChannel(ChannelName);
            if (Chan == null)
                return null;

            Chan.Remove(PlayerName);
            return Chan;
        }
        public void LeaveChannels(string PlayerName)
        {
            foreach (Channel Chan in Channels.Values.ToArray())
                Chan.Remove(PlayerName);
        }

        public Channel SendMessage(string ChannelName, string PlayerName, string Message, bool CheckNameExist)
        {
            Channel Chan = GetChannel(ChannelName);
            if (Chan == null || !Chan.DispatchMessage(PlayerName, Message, CheckNameExist))
                return null;

            return Chan;
        }
        public List<Channel> SendMessages(string PlayerName, string Message, bool CheckNameExist)
        {
            List<Channel> Chans = new List<Channel>();

            foreach (Channel Chan in Channels.Values.ToArray())
                if (Chan.DispatchMessage(PlayerName, Message, CheckNameExist))
                    Chans.Add(Chan);

            return Chans;
        }

        public List<Channel> SearchChannels(string Key, bool ByName)
        {
            List<Channel> Chans = new List<Channel>();

            foreach (Channel Chan in Channels.Values.ToArray())
                if (ByName && Chan.ChannelName.Contains(Key))
                    Chans.Add(Chan);
                else if (!ByName && Chan.ChannelDescription.Contains(Key))
                    Chans.Add(Chan);

            return Chans;
        }
    }
}
