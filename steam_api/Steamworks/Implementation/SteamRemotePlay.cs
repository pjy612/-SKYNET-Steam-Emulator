﻿using SKYNET.Steamworks.Interfaces;

using RemotePlaySessionID_t = System.UInt32;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamRemotePlay : ISteamInterface
    {
        public static SteamRemotePlay Instance;

        public SteamRemotePlay()
        {
            Instance = this;
            InterfaceName = "SteamRemotePlay";
            InterfaceVersion = "STEAMREMOTEPLAY_INTERFACE_VERSION001";
        }

        public uint GetSessionCount()
        {
            Write("GetSessionCount");
            return 0;
        }

        public RemotePlaySessionID_t GetSessionID(int iSessionIndex)
        {
            Write("GetSessionID");
            return 0;
        }

        public CSteamID GetSessionSteamID(RemotePlaySessionID_t unSessionID)
        {
            Write("GetSessionSteamID");
            return CSteamID.CreateOne();
        }

        public string GetSessionClientName(RemotePlaySessionID_t unSessionID)
        {
            Write("GetSessionClientName");
            return "";
        }

        public int GetSessionClientFormFactor(RemotePlaySessionID_t unSessionID)
        {
            Write("GetSessionClientFormFactor");
            return 0;
        }

        public bool BGetSessionClientResolution(RemotePlaySessionID_t unSessionID, int pnResolutionX, int pnResolutionY)
        {
            Write("BGetSessionClientResolution");
            return default;
        }

        public bool BSendRemotePlayTogetherInvite(ulong steamIDFriend)
        {
            Write("BSendRemotePlayTogetherInvite");
            return false;
        }
    }
}