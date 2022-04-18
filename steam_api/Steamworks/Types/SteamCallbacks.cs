﻿using SKYNET.Callback;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Types;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
    // callbacks
    //---------------------------------------------------------------------------------
    // Purpose: Sent when a new app is installed
    //---------------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SteamAppInstalled_t
    {
        public const int k_iCallback = Constants.k_iSteamAppListCallbacks + 1;
        public AppId_t m_nAppID;            // ID of the app that installs
    }

    //---------------------------------------------------------------------------------
    // Purpose: Sent when an app is uninstalled
    //---------------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SteamAppUninstalled_t
    {
        public const int k_iCallback = Constants.k_iSteamAppListCallbacks + 2;
        public AppId_t m_nAppID;            // ID of the app that installs
    }

    // callbacks
    //-----------------------------------------------------------------------------
    // Purpose: posted after the user gains ownership of DLC & that DLC is installed
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct DlcInstalled_t
    {
        public const int k_iCallback = Constants.k_iSteamAppsCallbacks + 5;
        public AppId_t m_nAppID;        // AppID of the DLC
    }

    //-----------------------------------------------------------------------------
    // Purpose: response to RegisterActivationCode()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RegisterActivationCodeResponse_t
    {
        public const int k_iCallback = Constants.k_iSteamAppsCallbacks + 8;
        public ERegisterActivationCodeResult m_eResult;
        public uint m_unPackageRegistered;                      // package that was registered. Only set on success
    }

    //-----------------------------------------------------------------------------
    // Purpose: response to RegisterActivationCode()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct AppProofOfPurchaseKeyResponse_t
    {
        public const int k_iCallback = Constants.k_iSteamAppsCallbacks + 13;
        public EResult m_eResult;
        public uint m_nAppID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cubAppProofOfPurchaseKeyMax)]
        public string m_rgchKey;
    }

    //---------------------------------------------------------------------------------
    // Purpose: posted after the user gains executes a steam url with query parameters
    // such as steam://run/<appid>//?param1=value1;param2=value2;param3=value3; etc
    // while the game is already running.  The new params can be queried
    // with GetLaunchQueryParam.
    //---------------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct NewLaunchQueryParameters_t
    {
        public const int k_iCallback = Constants.k_iSteamAppsCallbacks + 14;
    }

    // callbacks
    //-----------------------------------------------------------------------------
    // Purpose: called when a friends' status changes
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct PersonaStateChange_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 4;

        public ulong m_ulSteamID;       // steamID of the friend who changed
        public EPersonaChange m_nChangeFlags;       // what's changed
    }

    //-----------------------------------------------------------------------------
    // Purpose: posted when game overlay activates or deactivates
    //			the game can use this to be pause or resume single player games
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GameOverlayActivated_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 31;
        public byte m_bActive;  // true if it's just been activated, false otherwise
    }

    //-----------------------------------------------------------------------------
    // Purpose: called when the user tries to join a different game server from their friends list
    //			game client should attempt to connect to specified server when this is received
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GameServerChangeRequested_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 32;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string m_rgchServer;     // server address ("127.0.0.1:27015", "tf2.valvesoftware.com")
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string m_rgchPassword;   // server password, if any
    }

    //-----------------------------------------------------------------------------
    // Purpose: called when the user tries to join a lobby from their friends list
    //			game client should attempt to connect to specified lobby when this is received
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GameLobbyJoinRequested_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 33;
        public SteamID m_steamIDLobby;

        // The friend they did the join via (will be invalid if not directly via a friend)
        //
        // On PS3, the friend will be invalid if this was triggered by a PSN invite via the XMB, but
        // the account type will be console user so you can tell at least that this was from a PSN friend
        // rather than a Steam friend.
        public SteamID m_steamIDFriend;
    }

    //-----------------------------------------------------------------------------
    // Purpose: called when an avatar is loaded in from a previous GetLargeFriendAvatar() call
    //			if the image wasn't already available
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct AvatarImageLoaded_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 34;
        public SteamID m_steamID; // steamid the avatar has been loaded for
        public int m_iImage; // the image index of the now loaded image
        public int m_iWide; // width of the loaded image
        public int m_iTall; // height of the loaded image
    }

    //-----------------------------------------------------------------------------
    // Purpose: marks the return of a request officer list call
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct ClanOfficerListResponse_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 35;
        public SteamID m_steamIDClan;
        public int m_cOfficers;
        public byte m_bSuccess;
    }

    //-----------------------------------------------------------------------------
    // Purpose: callback indicating updated data about friends rich presence information
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FriendRichPresenceUpdate_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 36;
        public SteamID m_steamIDFriend;    // friend who's rich presence has changed
        public AppId_t m_nAppID;            // the appID of the game (should always be the current game)
    }

    //-----------------------------------------------------------------------------
    // Purpose: called when the user tries to join a game from their friends list
    //			rich presence will have been set with the "connect" key which is set here
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GameRichPresenceJoinRequested_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 37;
        public SteamID m_steamIDFriend;        // the friend they did the join via (will be invalid if not directly via a friend)
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cchMaxRichPresenceValueLength)]
        public string m_rgchConnect;
    }

    //-----------------------------------------------------------------------------
    // Purpose: a chat message has been received for a clan chat the game has joined
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GameConnectedClanChatMsg_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 38;
        public SteamID m_steamIDClanChat;
        public SteamID m_steamIDUser;
        public int m_iMessageID;
    }

    //-----------------------------------------------------------------------------
    // Purpose: a user has joined a clan chat
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GameConnectedChatJoin_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 39;
        public SteamID m_steamIDClanChat;
        public SteamID m_steamIDUser;
    }

    //-----------------------------------------------------------------------------
    // Purpose: a user has left the chat we're in
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GameConnectedChatLeave_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 40;
        public SteamID m_steamIDClanChat;
        public SteamID m_steamIDUser;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bKicked;      // true if admin kicked
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bDropped; // true if Steam connection dropped
    }

    //-----------------------------------------------------------------------------
    // Purpose: a DownloadClanActivityCounts() call has finished
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct DownloadClanActivityCountsResult_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 41;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bSuccess;
    }

    //-----------------------------------------------------------------------------
    // Purpose: a JoinClanChatRoom() call has finished
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct JoinClanChatRoomCompletionResult_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 42;
        public SteamID m_steamIDClanChat;
        public EChatRoomEnterResponse m_eChatRoomEnterResponse;
    }

    //-----------------------------------------------------------------------------
    // Purpose: a chat message has been received from a user
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GameConnectedFriendChatMsg_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 43;
        public SteamID m_steamIDUser;
        public int m_iMessageID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FriendsGetFollowerCount_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 44;
        public EResult m_eResult;
        public SteamID m_steamID;
        public int m_nCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FriendsIsFollowing_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 45;
        public EResult m_eResult;
        public SteamID m_steamID;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bIsFollowing;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FriendsEnumerateFollowingList_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 46;
        public EResult m_eResult;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.k_cEnumerateFollowersMax)]
        public SteamID[] m_rgSteamID;
        public int m_nResultsReturned;
        public int m_nTotalResultCount;
    }

    //-----------------------------------------------------------------------------
    // Purpose: reports the result of an attempt to change the user's persona name
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SetPersonaNameResponse_t
    {
        public const int k_iCallback = Constants.k_iSteamFriendsCallbacks + 47;

        [MarshalAs(UnmanagedType.I1)]
        public bool m_bSuccess; // true if name change succeeded completely.
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bLocalSuccess; // true if name change was retained locally.  (We might not have been able to communicate with Steam)
        public EResult m_result; // detailed result code
    }

    // callbacks
    // callback notification - A new message is available for reading from the message queue
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GCMessageAvailable_t
    {
        public const int k_iCallback = Constants.k_iSteamGameCoordinatorCallbacks + 1;
        public uint m_nMessageSize;
    }

    // callback notification - A message failed to make it to the GC. It may be down temporarily
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct GCMessageFailed_t
    {
        public const int k_iCallback = Constants.k_iSteamGameCoordinatorCallbacks + 2;
    }

    // won't enforce authentication of users that connect to the server.
    // Useful when you run a server where the clients may not
    // be connected to the internet but you want them to play (i.e LANs)
    // callbacks
    // client has been approved to connect to this game server
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GSClientApprove_t
    {
        public const int k_iCallback = Constants.k_iSteamGameServerCallbacks + 1;
        public SteamID m_SteamID;          // SteamID of approved player
        public SteamID m_OwnerSteamID; // SteamID of original owner for game license
    }

    // client has been denied to connection to this game server
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GSClientDeny_t
    {
        public const int k_iCallback = Constants.k_iSteamGameServerCallbacks + 2;
        public SteamID m_SteamID;
        public EDenyReason m_eDenyReason;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string m_rgchOptionalText;
    }

    // request the game server should kick the user
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GSClientKick_t
    {
        public const int k_iCallback = Constants.k_iSteamGameServerCallbacks + 3;
        public SteamID m_SteamID;
        public EDenyReason m_eDenyReason;
    }

    // NOTE: callback values 4 and 5 are skipped because they are used for old deprecated callbacks,
    // do not reuse them here.
    // client achievement info
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GSClientAchievementStatus_t
    {
        public const int k_iCallback = Constants.k_iSteamGameServerCallbacks + 6;
        public ulong m_SteamID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string m_pchAchievement;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUnlocked;
    }

    // received when the game server requests to be displayed as secure (VAC protected)
    // m_bSecure is true if the game server should display itself as secure to users, false otherwise
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GSPolicyResponse_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 15;
        public byte m_bSecure;
    }

    // GS gameplay stats info
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GSGameplayStats_t
    {
        public const int k_iCallback = Constants.k_iSteamGameServerCallbacks + 7;
        public EResult m_eResult;                   // Result of the call
        public int m_nRank;                 // Overall rank of the server (0-based)
        public uint m_unTotalConnects;          // Total number of clients who have ever connected to the server
        public uint m_unTotalMinutesPlayed;     // Total number of minutes ever played on the server
    }

    // send as a reply to RequestUserGroupStatus()
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GSClientGroupStatus_t
    {
        public const int k_iCallback = Constants.k_iSteamGameServerCallbacks + 8;
        public SteamID m_SteamIDUser;
        public SteamID m_SteamIDGroup;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bMember;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bOfficer;
    }

    // Sent as a reply to GetServerReputation()
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GSReputation_t
    {
        public const int k_iCallback = Constants.k_iSteamGameServerCallbacks + 9;
        public EResult m_eResult;               // Result of the call;
        public uint m_unReputationScore;    // The reputation score for the game server
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bBanned;              // True if the server is banned from the Steam
                                            // master servers

        // The following members are only filled out if m_bBanned is true. They will all
        // be set to zero otherwise. Master server bans are by IP so it is possible to be
        // banned even when the score is good high if there is a bad server on another port.
        // This information can be used to determine which server is bad.

        public uint m_unBannedIP;       // The IP of the banned server
        public ushort m_usBannedPort;       // The port of the banned server
        public ulong m_ulBannedGameID;  // The game ID the banned server is serving
        public uint m_unBanExpires;     // Time the ban expires, expressed in the Unix epoch (seconds since 1/1/1970)
    }

    // Sent as a reply to AssociateWithClan()
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct AssociateWithClanResult_t
    {
        public const int k_iCallback = Constants.k_iSteamGameServerCallbacks + 10;
        public EResult m_eResult;               // Result of the call;
    }

    // Sent as a reply to ComputeNewPlayerCompatibility()
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct ComputeNewPlayerCompatibilityResult_t
    {
        public const int k_iCallback = Constants.k_iSteamGameServerCallbacks + 11;
        public EResult m_eResult;               // Result of the call;
        public int m_cPlayersThatDontLikeCandidate;
        public int m_cPlayersThatCandidateDoesntLike;
        public int m_cClanPlayersThatDontLikeCandidate;
        public SteamID m_SteamIDCandidate;
    }

    // callbacks
    //-----------------------------------------------------------------------------
    // Purpose: called when the latests stats and achievements have been received
    //			from the server
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GSStatsReceived_t
    {
        public const int k_iCallback = Constants.k_iSteamGameServerStatsCallbacks;
        public EResult m_eResult;       // Success / error fetching the stats
        public SteamID m_steamIDUser;  // The user for whom the stats are retrieved for
    }

    //-----------------------------------------------------------------------------
    // Purpose: result of a request to store the user stats for a game
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct GSStatsStored_t
    {
        public const int k_iCallback = Constants.k_iSteamGameServerStatsCallbacks + 1;
        public EResult m_eResult;       // success / error
        public SteamID m_steamIDUser;  // The user for whom the stats were stored
    }

    //-----------------------------------------------------------------------------
    // Purpose: Callback indicating that a user's stats have been unloaded.
    //  Call RequestUserStats again to access stats for this user
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GSStatsUnloaded_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 8;
        public SteamID m_steamIDUser;  // User whose stats have been unloaded
    }

    // callbacks
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct HTTPRequestCompleted_t
    {
        public const int k_iCallback = Constants.k_iClientHTTPCallbacks + 1;

        // Handle value for the request that has completed.
        public HTTPRequestHandle m_hRequest;

        // Context value that the user defined on the request that this callback is associated with, 0 if
        // no context value was set.
        public ulong m_ulContextValue;

        // This will be true if we actually got any sort of response from the server (even an error).
        // It will be false if we failed due to an internal error or client side network failure.
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bRequestSuccessful;

        // Will be the HTTP status code value returned by the server, k_EHTTPStatusCode200OK is the normal
        // OK response, if you get something else you probably need to treat it as a failure.
        public EHTTPStatusCode m_eStatusCode;

        public uint m_unBodySize; // Same as GetHTTPResponseBodySize()
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct HTTPRequestHeadersReceived_t
    {
        public const int k_iCallback = Constants.k_iClientHTTPCallbacks + 2;

        // Handle value for the request that has received headers.
        public HTTPRequestHandle m_hRequest;

        // Context value that the user defined on the request that this callback is associated with, 0 if
        // no context value was set.
        public ulong m_ulContextValue;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct HTTPRequestDataReceived_t
    {
        public const int k_iCallback = Constants.k_iClientHTTPCallbacks + 3;

        // Handle value for the request that has received data.
        public HTTPRequestHandle m_hRequest;

        // Context value that the user defined on the request that this callback is associated with, 0 if
        // no context value was set.
        public ulong m_ulContextValue;


        // Offset to provide to GetHTTPStreamingResponseBodyData to get this chunk of data
        public uint m_cOffset;

        // Size to provide to GetHTTPStreamingResponseBodyData to get this chunk of data
        public uint m_cBytesReceived;
    }

    // SteamInventoryResultReady_t callbacks are fired whenever asynchronous
    // results transition from "Pending" to "OK" or an error state. There will
    // always be exactly one callback per handle.
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SteamInventoryResultReady_t
    {
        public const int k_iCallback = Constants.k_iClientInventoryCallbacks + 0;
        public SteamInventoryResult_t m_handle;
        public EResult m_result;
    }

    // SteamInventoryFullUpdate_t callbacks are triggered when GetAllItems
    // successfully returns a result which is newer / fresher than the last
    // known result. (It will not trigger if the inventory hasn't changed,
    // or if results from two overlapping calls are reversed in flight and
    // the earlier result is already known to be stale/out-of-date.)
    // The normal ResultReady callback will still be triggered immediately
    // afterwards; this is an additional notification for your convenience.
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SteamInventoryFullUpdate_t
    {
        public const int k_iCallback = Constants.k_iClientInventoryCallbacks + 1;
        public SteamInventoryResult_t m_handle;
    }

    // A SteamInventoryDefinitionUpdate_t callback is triggered whenever
    // item definitions have been updated, which could be in response to
    // LoadItemDefinitions() or any other async request which required
    // a definition update in order to process results from the server.
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct SteamInventoryDefinitionUpdate_t
    {
        public const int k_iCallback = Constants.k_iClientInventoryCallbacks + 2;
    }

    //-----------------------------------------------------------------------------
    // Callbacks for ISteamMatchmaking (which go through the regular Steam callback registration system)
    //-----------------------------------------------------------------------------
    // Purpose: a server was added/removed from the favorites list, you should refresh now
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct FavoritesListChanged_t
    {
        public const int k_iCallback = Constants.k_iSteamMatchmakingCallbacks + 2;
        public uint m_nIP; // an IP of 0 means reload the whole list, any other value means just one server
        public uint m_nQueryPort;
        public uint m_nConnPort;
        public uint m_nAppID;
        public uint m_nFlags;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bAdd; // true if this is adding the entry, otherwise it is a remove
        public AccountID_t m_unAccountId;
    }

    //-----------------------------------------------------------------------------
    // Purpose: Someone has invited you to join a Lobby
    //			normally you don't need to do anything with this, since
    //			the Steam UI will also display a '<user> has invited you to the lobby, join?' dialog
    //
    //			if the user outside a game chooses to join, your game will be launched with the parameter "+connect_lobby <64-bit lobby id>",
    //			or with the callback GameLobbyJoinRequested_t if they're already in-game
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LobbyInvite_t
    {
        public const int k_iCallback = Constants.k_iSteamMatchmakingCallbacks + 3;

        public ulong m_ulSteamIDUser;       // Steam ID of the person making the invite
        public ulong m_ulSteamIDLobby;  // Steam ID of the Lobby
        public ulong m_ulGameID;            // GameID of the Lobby
    }

    //-----------------------------------------------------------------------------
    // Purpose: Sent on entering a lobby, or on failing to enter
    //			m_EChatRoomEnterResponse will be set to k_EChatRoomEnterResponseSuccess on success,
    //			or a higher value on failure (see enum EChatRoomEnterResponse)
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LobbyEnter_t
    {
        public const int k_iCallback = Constants.k_iSteamMatchmakingCallbacks + 4;

        public ulong m_ulSteamIDLobby;                          // SteamID of the Lobby you have entered
        public uint m_rgfChatPermissions;                       // Permissions of the current user
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bLocked;                                      // If true, then only invited users may join
        public uint m_EChatRoomEnterResponse;   // EChatRoomEnterResponse
    }

    //-----------------------------------------------------------------------------
    // Purpose: The lobby metadata has changed
    //			if m_ulSteamIDMember is the steamID of a lobby member, use GetLobbyMemberData() to access per-user details
    //			if m_ulSteamIDMember == m_ulSteamIDLobby, use GetLobbyData() to access lobby metadata
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LobbyDataUpdate_t
    {
        public const int k_iCallback = Constants.k_iSteamMatchmakingCallbacks + 5;

        public ulong m_ulSteamIDLobby;      // steamID of the Lobby
        public ulong m_ulSteamIDMember;     // steamID of the member whose data changed, or the room itself
        public byte m_bSuccess;             // true if we lobby data was successfully changed;
                                            // will only be false if RequestLobbyData() was called on a lobby that no longer exists
    }

    //-----------------------------------------------------------------------------
    // Purpose: The lobby chat room state has changed
    //			this is usually sent when a user has joined or left the lobby
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LobbyChatUpdate_t
    {
        public const int k_iCallback = Constants.k_iSteamMatchmakingCallbacks + 6;

        public ulong m_ulSteamIDLobby;          // Lobby ID
        public ulong m_ulSteamIDUserChanged;        // user who's status in the lobby just changed - can be recipient
        public ulong m_ulSteamIDMakingChange;       // Chat member who made the change (different from SteamIDUserChange if kicking, muting, etc.)
                                                    // for example, if one user kicks another from the lobby, this will be set to the id of the user who initiated the kick
        public uint m_rgfChatMemberStateChange; // bitfield of EChatMemberStateChange values
    }

    //-----------------------------------------------------------------------------
    // Purpose: A chat message for this lobby has been sent
    //			use GetLobbyChatEntry( m_iChatID ) to retrieve the contents of this message
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LobbyChatMsg_t
    {
        public const int k_iCallback = Constants.k_iSteamMatchmakingCallbacks + 7;

        public ulong m_ulSteamIDLobby;          // the lobby id this is in
        public ulong m_ulSteamIDUser;           // steamID of the user who has sent this message
        public byte m_eChatEntryType;           // type of message
        public uint m_iChatID;              // index of the chat entry to lookup
    }

    //-----------------------------------------------------------------------------
    // Purpose: A game created a game for all the members of the lobby to join,
    //			as triggered by a SetLobbyGameServer()
    //			it's up to the individual clients to take action on this; the usual
    //			game behavior is to leave the lobby and connect to the specified game server
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LobbyGameCreated_t
    {
        public const int k_iCallback = Constants.k_iSteamMatchmakingCallbacks + 9;

        public ulong m_ulSteamIDLobby;      // the lobby we were in
        public ulong m_ulSteamIDGameServer; // the new game server that has been created or found for the lobby members
        public uint m_unIP;                 // IP & Port of the game server (if any)
        public ushort m_usPort;
    }

    //-----------------------------------------------------------------------------
    // Purpose: Number of matching lobbies found
    //			iterate the returned lobbies with GetLobbyByIndex(), from values 0 to m_nLobbiesMatching-1
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LobbyMatchList_t
    {
        public const int k_iCallback = Constants.k_iSteamMatchmakingCallbacks + 10;
        public uint m_nLobbiesMatching;     // Number of lobbies that matched search criteria and we have SteamIDs for
    }

    //-----------------------------------------------------------------------------
    // Purpose: posted if a user is forcefully removed from a lobby
    //			can occur if a user loses connection to Steam
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LobbyKicked_t
    {
        public const int k_iCallback = Constants.k_iSteamMatchmakingCallbacks + 12;
        public ulong m_ulSteamIDLobby;          // Lobby
        public ulong m_ulSteamIDAdmin;          // User who kicked you - possibly the ID of the lobby itself
        public byte m_bKickedDueToDisconnect;       // true if you were kicked from the lobby due to the user losing connection to Steam (currently always true)
    }

    //-----------------------------------------------------------------------------
    // Purpose: Result of our request to create a Lobby
    //			m_eResult == k_EResultOK on success
    //			at this point, the lobby has been joined and is ready for use
    //			a LobbyEnter_t callback will also be received (since the local user is joining their own lobby)
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LobbyCreated_t
    {
        public const int k_iCallback = Constants.k_iSteamMatchmakingCallbacks + 13;

        public EResult m_eResult;       // k_EResultOK - the lobby was successfully created
                                        // k_EResultNoConnection - your Steam client doesn't have a connection to the back-end
                                        // k_EResultTimeout - you the message to the Steam servers, but it didn't respond
                                        // k_EResultFail - the server responded, but with an unknown internal error
                                        // k_EResultAccessDenied - your game isn't set to allow lobbies, or your client does haven't rights to play the game
                                        // k_EResultLimitExceeded - your game client has created too many lobbies

        public ulong m_ulSteamIDLobby;      // chat room, zero if failed
    }

    //-----------------------------------------------------------------------------
    // Purpose: Result of our request to create a Lobby
    //			m_eResult == k_EResultOK on success
    //			at this point, the lobby has been joined and is ready for use
    //			a LobbyEnter_t callback will also be received (since the local user is joining their own lobby)
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct FavoritesListAccountsUpdated_t
    {
        public const int k_iCallback = Constants.k_iSteamMatchmakingCallbacks + 16;

        public EResult m_eResult;
    }

    // callbacks
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct PlaybackStatusHasChanged_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicCallbacks + 1;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct VolumeHasChanged_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicCallbacks + 2;
        public float m_flNewVolume;
    }

    // callbacks
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct MusicPlayerRemoteWillActivate_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicRemoteCallbacks + 1;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct MusicPlayerRemoteWillDeactivate_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicRemoteCallbacks + 2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct MusicPlayerRemoteToFront_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicRemoteCallbacks + 3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct MusicPlayerWillQuit_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicRemoteCallbacks + 4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct MusicPlayerWantsPlay_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicRemoteCallbacks + 5;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct MusicPlayerWantsPause_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicRemoteCallbacks + 6;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct MusicPlayerWantsPlayPrevious_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicRemoteCallbacks + 7;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct MusicPlayerWantsPlayNext_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicRemoteCallbacks + 8;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct MusicPlayerWantsShuffled_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicRemoteCallbacks + 9;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bShuffled;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct MusicPlayerWantsLooped_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicRemoteCallbacks + 10;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bLooped;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct MusicPlayerWantsVolume_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicCallbacks + 11;
        public float m_flNewVolume;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct MusicPlayerSelectsQueueEntry_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicCallbacks + 12;
        public int nID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct MusicPlayerSelectsPlaylistEntry_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicCallbacks + 13;
        public int nID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct MusicPlayerWantsPlayingRepeatStatus_t
    {
        public const int k_iCallback = Constants.k_iSteamMusicRemoteCallbacks + 14;
        public int m_nPlayingRepeatStatus;
    }

    // callbacks
    // callback notification - a user wants to talk to us over the P2P channel via the SendP2PPacket() API
    // in response, a call to AcceptP2PPacketsFromUser() needs to be made, if you want to talk with them
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct P2PSessionRequest_t
    {
        public const int k_iCallback = Constants.k_iSteamNetworkingCallbacks + 2;
        public SteamID m_steamIDRemote;            // user who wants to talk to us
    }

    // callback notification - packets can't get through to the specified user via the SendP2PPacket() API
    // all packets queued packets unsent at this point will be dropped
    // further attempts to send will retry making the connection (but will be dropped if we fail again)
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct P2PSessionConnectFail_t
    {
        public const int k_iCallback = Constants.k_iSteamNetworkingCallbacks + 3;
        public SteamID m_steamIDRemote;            // user we were sending packets to
        public byte m_eP2PSessionError;         // EP2PSessionError indicating why we're having trouble
    }

    // callback notification - status of a socket has changed
    // used as part of the CreateListenSocket() / CreateP2PConnectionSocket()
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SocketStatusCallback_t
    {
        public const int k_iCallback = Constants.k_iSteamNetworkingCallbacks + 1;
        public SNetSocket_t m_hSocket;              // the socket used to send/receive data to the remote host
        public SNetListenSocket_t m_hListenSocket;  // this is the server socket that we were listening on; NULL if this was an outgoing connection
        public SteamID m_steamIDRemote;            // remote steamID we have connected to, if it has one
        public int m_eSNetSocketState;              // socket state, ESNetSocketState
    }

    // callbacks
    //-----------------------------------------------------------------------------
    // Purpose: sent when the local file cache is fully synced with the server for an app
    //          That means that an application can be started and has all latest files
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageAppSyncedClient_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 1;
        public AppId_t m_nAppID;
        public EResult m_eResult;
        public int m_unNumDownloads;
    }

    //-----------------------------------------------------------------------------
    // Purpose: sent when the server is fully synced with the local file cache for an app
    //          That means that we can shutdown Steam and our data is stored on the server
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageAppSyncedServer_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 2;
        public AppId_t m_nAppID;
        public EResult m_eResult;
        public int m_unNumUploads;
    }

    //-----------------------------------------------------------------------------
    // Purpose: Status of up and downloads during a sync session
    //
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageAppSyncProgress_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cchFilenameMax)]
        public string m_rgchCurrentFile;                // Current file being transferred
        public AppId_t m_nAppID;                            // App this info relates to
        public uint m_uBytesTransferredThisChunk;       // Bytes transferred this chunk
        public double m_dAppPercentComplete;                // Percent complete that this app's transfers are
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUploading;                           // if false, downloading
    }

    //
    // IMPORTANT! k_iClientRemoteStorageCallbacks + 4 is used, see iclientremotestorage.h
    //
    //-----------------------------------------------------------------------------
    // Purpose: Sent after we've determined the list of files that are out of sync
    //          with the server.
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageAppSyncStatusCheck_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 5;
        public AppId_t m_nAppID;
        public EResult m_eResult;
    }

    //-----------------------------------------------------------------------------
    // Purpose: Sent after a conflict resolution attempt.
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageConflictResolution_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 6;
        public AppId_t m_nAppID;
        public EResult m_eResult;
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to FileShare()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageFileShareResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 7;
        public EResult m_eResult;           // The result of the operation
        public UGCHandle_t m_hFile;     // The handle that can be shared with users and features
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cchFilenameMax)]
        public string m_rgchFilename; // The name of the file that was shared
    }

    // k_iClientRemoteStorageCallbacks + 8 is deprecated! Do not reuse
    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to PublishFile()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStoragePublishFileResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 9;
        public EResult m_eResult;               // The result of the operation.
        public PublishedFileId_t m_nPublishedFileId;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUserNeedsToAcceptWorkshopLegalAgreement;
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to DeletePublishedFile()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageDeletePublishedFileResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 11;
        public EResult m_eResult;               // The result of the operation.
        public PublishedFileId_t m_nPublishedFileId;
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to EnumerateUserPublishedFiles()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageEnumerateUserPublishedFilesResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 12;
        public EResult m_eResult;               // The result of the operation.
        public int m_nResultsReturned;
        public int m_nTotalResultCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.k_unEnumeratePublishedFilesMaxResults)]
        public PublishedFileId_t[] m_rgPublishedFileId;
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to SubscribePublishedFile()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageSubscribePublishedFileResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 13;
        public EResult m_eResult;               // The result of the operation.
        public PublishedFileId_t m_nPublishedFileId;
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to EnumerateSubscribePublishedFiles()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageEnumerateUserSubscribedFilesResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 14;
        public EResult m_eResult;               // The result of the operation.
        public int m_nResultsReturned;
        public int m_nTotalResultCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.k_unEnumeratePublishedFilesMaxResults)]
        public PublishedFileId_t[] m_rgPublishedFileId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.k_unEnumeratePublishedFilesMaxResults)]
        public uint[] m_rgRTimeSubscribed;
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to UnsubscribePublishedFile()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageUnsubscribePublishedFileResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 15;
        public EResult m_eResult;               // The result of the operation.
        public PublishedFileId_t m_nPublishedFileId;
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to CommitPublishedFileUpdate()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageUpdatePublishedFileResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 16;
        public EResult m_eResult;               // The result of the operation.
        public PublishedFileId_t m_nPublishedFileId;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUserNeedsToAcceptWorkshopLegalAgreement;
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to UGCDownload()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageDownloadUGCResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 17;
        public EResult m_eResult;               // The result of the operation.
        public UGCHandle_t m_hFile;         // The handle to the file that was attempted to be downloaded.
        public AppId_t m_nAppID;                // ID of the app that created this file.
        public int m_nSizeInBytes;          // The size of the file that was downloaded, in bytes.
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cchFilenameMax)]
        public string m_pchFileName;        // The name of the file that was downloaded.
        public ulong m_ulSteamIDOwner;      // Steam ID of the user who created this content.
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to GetPublishedFileDetails()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageGetPublishedFileDetailsResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 18;
        public EResult m_eResult;               // The result of the operation.
        public PublishedFileId_t m_nPublishedFileId;
        public AppId_t m_nCreatorAppID;     // ID of the app that created this file.
        public AppId_t m_nConsumerAppID;        // ID of the app that will consume this file.
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cchPublishedDocumentTitleMax)]
        public string m_rgchTitle;      // title of document
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cchPublishedDocumentDescriptionMax)]
        public string m_rgchDescription;    // description of document
        public UGCHandle_t m_hFile;         // The handle of the primary file
        public UGCHandle_t m_hPreviewFile;      // The handle of the preview file
        public ulong m_ulSteamIDOwner;      // Steam ID of the user who created this content.
        public uint m_rtimeCreated;         // time when the published file was created
        public uint m_rtimeUpdated;         // time when the published file was last updated
        public ERemoteStoragePublishedFileVisibility m_eVisibility;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bBanned;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cchTagListMax)]
        public string m_rgchTags;   // comma separated list of all tags associated with this file
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bTagsTruncated;           // whether the list of tags was too long to be returned in the provided buffer
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cchFilenameMax)]
        public string m_pchFileName;        // The name of the primary file
        public int m_nFileSize;             // Size of the primary file
        public int m_nPreviewFileSize;      // Size of the preview file
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cchPublishedFileURLMax)]
        public string m_rgchURL;    // URL (for a video or a website)
        public EWorkshopFileType m_eFileType;   // Type of the file
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bAcceptedForUse;          // developer has specifically flagged this item as accepted in the Workshop
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageEnumerateWorkshopFilesResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 19;
        public EResult m_eResult;
        public int m_nResultsReturned;
        public int m_nTotalResultCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.k_unEnumeratePublishedFilesMaxResults)]
        public PublishedFileId_t[] m_rgPublishedFileId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.k_unEnumeratePublishedFilesMaxResults)]
        public float[] m_rgScore;
        public AppId_t m_nAppId;
        public uint m_unStartIndex;
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of GetPublishedItemVoteDetails
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageGetPublishedItemVoteDetailsResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 20;
        public EResult m_eResult;
        public PublishedFileId_t m_unPublishedFileId;
        public int m_nVotesFor;
        public int m_nVotesAgainst;
        public int m_nReports;
        public float m_fScore;
    }

    //-----------------------------------------------------------------------------
    // Purpose: User subscribed to a file for the app (from within the app or on the web)
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStoragePublishedFileSubscribed_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 21;
        public PublishedFileId_t m_nPublishedFileId;    // The published file id
        public AppId_t m_nAppID;                        // ID of the app that will consume this file.
    }

    //-----------------------------------------------------------------------------
    // Purpose: User unsubscribed from a file for the app (from within the app or on the web)
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStoragePublishedFileUnsubscribed_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 22;
        public PublishedFileId_t m_nPublishedFileId;    // The published file id
        public AppId_t m_nAppID;                        // ID of the app that will consume this file.
    }

    //-----------------------------------------------------------------------------
    // Purpose: Published file that a user owns was deleted (from within the app or the web)
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStoragePublishedFileDeleted_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 23;
        public PublishedFileId_t m_nPublishedFileId;    // The published file id
        public AppId_t m_nAppID;                        // ID of the app that will consume this file.
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to UpdateUserPublishedItemVote()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageUpdateUserPublishedItemVoteResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 24;
        public EResult m_eResult;               // The result of the operation.
        public PublishedFileId_t m_nPublishedFileId;    // The published file id
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to GetUserPublishedItemVoteDetails()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageUserVoteDetails_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 25;
        public EResult m_eResult;               // The result of the operation.
        public PublishedFileId_t m_nPublishedFileId;    // The published file id
        public EWorkshopVote m_eVote;           // what the user voted
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageEnumerateUserSharedWorkshopFilesResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 26;
        public EResult m_eResult;               // The result of the operation.
        public int m_nResultsReturned;
        public int m_nTotalResultCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.k_unEnumeratePublishedFilesMaxResults)]
        public PublishedFileId_t[] m_rgPublishedFileId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageSetUserPublishedFileActionResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 27;
        public EResult m_eResult;               // The result of the operation.
        public PublishedFileId_t m_nPublishedFileId;    // The published file id
        public EWorkshopFileAction m_eAction;   // the action that was attempted
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStorageEnumeratePublishedFilesByUserActionResult_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 28;
        public EResult m_eResult;               // The result of the operation.
        public EWorkshopFileAction m_eAction;   // the action that was filtered on
        public int m_nResultsReturned;
        public int m_nTotalResultCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.k_unEnumeratePublishedFilesMaxResults)]
        public PublishedFileId_t[] m_rgPublishedFileId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Constants.k_unEnumeratePublishedFilesMaxResults)]
        public uint[] m_rgRTimeUpdated;
    }

    //-----------------------------------------------------------------------------
    // Purpose: Called periodically while a PublishWorkshopFile is in progress
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStoragePublishFileProgress_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 29;
        public double m_dPercentFile;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bPreview;
    }

    //-----------------------------------------------------------------------------
    // Purpose: Called when the content for a published file is updated
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct RemoteStoragePublishedFileUpdated_t
    {
        public const int k_iCallback = Constants.k_iClientRemoteStorageCallbacks + 30;
        public PublishedFileId_t m_nPublishedFileId;    // The published file id
        public AppId_t m_nAppID;                        // ID of the app that will consume this file.
        public UGCHandle_t m_hFile;                 // The new content
    }

    // callbacks
    //-----------------------------------------------------------------------------
    // Purpose: Screenshot successfully written or otherwise added to the library
    // and can now be tagged
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct ScreenshotReady_t
    {
        public const int k_iCallback = Constants.k_iSteamScreenshotsCallbacks + 1;
        public ScreenshotHandle m_hLocal;
        public EResult m_eResult;
    }

    //-----------------------------------------------------------------------------
    // Purpose: Screenshot has been requested by the user.  Only sent if
    // HookScreenshots() has been called, in which case Steam will not take
    // the screenshot itself.
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct ScreenshotRequested_t
    {
        public const int k_iCallback = Constants.k_iSteamScreenshotsCallbacks + 2;
    }

    //-----------------------------------------------------------------------------
    // Purpose: Callback for querying UGC
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SteamUGCQueryCompleted_t
    {
        public const int k_iCallback = Constants.k_iClientUGCCallbacks + 1;
        public UGCQueryHandle_t m_handle;
        public EResult m_eResult;
        public uint m_unNumResultsReturned;
        public uint m_unTotalMatchingResults;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bCachedData;  // indicates whether this data was retrieved from the local on-disk cache
    }

    //-----------------------------------------------------------------------------
    // Purpose: Callback for requesting details on one piece of UGC
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SteamUGCRequestUGCDetailsResult_t
    {
        public const int k_iCallback = Constants.k_iClientUGCCallbacks + 2;
        public SteamUGCDetails_t m_details;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bCachedData; // indicates whether this data was retrieved from the local on-disk cache
    }

    //-----------------------------------------------------------------------------
    // Purpose: result for ISteamUGC::CreateItem()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct CreateItemResult_t
    {
        public const int k_iCallback = Constants.k_iClientUGCCallbacks + 3;
        public EResult m_eResult;
        public PublishedFileId_t m_nPublishedFileId; // new item got this UGC PublishFileID
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUserNeedsToAcceptWorkshopLegalAgreement;
    }

    //-----------------------------------------------------------------------------
    // Purpose: result for ISteamUGC::SubmitItemUpdate()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SubmitItemUpdateResult_t
    {
        public const int k_iCallback = Constants.k_iClientUGCCallbacks + 4;
        public EResult m_eResult;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bUserNeedsToAcceptWorkshopLegalAgreement;
    }

    //-----------------------------------------------------------------------------
    // Purpose: a Workshop item has been installed or updated
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct ItemInstalled_t
    {
        public const int k_iCallback = Constants.k_iClientUGCCallbacks + 5;
        public AppId_t m_unAppID;
        public PublishedFileId_t m_nPublishedFileId;
    }

    //-----------------------------------------------------------------------------
    // Purpose: result of DownloadItem(), existing item files can be accessed again
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct DownloadItemResult_t
    {
        public const int k_iCallback = Constants.k_iClientUGCCallbacks + 6;
        public AppId_t m_unAppID;
        public PublishedFileId_t m_nPublishedFileId;
        public EResult m_eResult;
    }

    //-----------------------------------------------------------------------------
    // Purpose: result of AddItemToFavorites() or RemoveItemFromFavorites()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct UserFavoriteItemsListChanged_t
    {
        public const int k_iCallback = Constants.k_iClientUGCCallbacks + 7;
        public PublishedFileId_t m_nPublishedFileId;
        public EResult m_eResult;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bWasAddRequest;
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to SetUserItemVote()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SetUserItemVoteResult_t
    {
        public const int k_iCallback = Constants.k_iClientUGCCallbacks + 8;
        public PublishedFileId_t m_nPublishedFileId;
        public EResult m_eResult;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bVoteUp;
    }

    //-----------------------------------------------------------------------------
    // Purpose: The result of a call to GetUserItemVote()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GetUserItemVoteResult_t
    {
        public const int k_iCallback = Constants.k_iClientUGCCallbacks + 9;
        public PublishedFileId_t m_nPublishedFileId;
        public EResult m_eResult;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bVotedUp;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bVotedDown;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bVoteSkipped;
    }


    // callbacks
    //-----------------------------------------------------------------------------
    // Purpose: called when a connections to the Steam back-end has been established
    //			this means the Steam client now has a working connection to the Steam servers
    //			usually this will have occurred before the game has launched, and should
    //			only be seen if the user has dropped connection due to a networking issue
    //			or a Steam server update
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct SteamServersConnected_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 1;
    }

    //-----------------------------------------------------------------------------
    // Purpose: called when a connection attempt has failed
    //			this will occur periodically if the Steam client is not connected,
    //			and has failed in it's retry to establish a connection
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SteamServerConnectFailure_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 2;
        public EResult m_eResult;
    }

    //-----------------------------------------------------------------------------
    // Purpose: called if the client has lost connection to the Steam servers
    //			real-time services will be disabled until a matching SteamServersConnected_t has been posted
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SteamServersDisconnected_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 3;
        public EResult m_eResult;
    }

    //-----------------------------------------------------------------------------
    // Purpose: Sent by the Steam server to the client telling it to disconnect from the specified game server,
    //			which it may be in the process of or already connected to.
    //			The game client should immediately disconnect upon receiving this message.
    //			This can usually occur if the user doesn't have rights to play on the game server.
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct ClientGameServerDeny_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 13;

        public uint m_uAppID;
        public uint m_unGameServerIP;
        public ushort m_usGameServerPort;
        public ushort m_bSecure;
        public uint m_uReason;
    }

    //-----------------------------------------------------------------------------
    // Purpose: called when the callback system for this client is in an error state (and has flushed pending callbacks)
    //			When getting this message the client should disconnect from Steam, reset any stored Steam state and reconnect.
    //			This usually occurs in the rare event the Steam client has some kind of fatal error.
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct IPCFailure_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 17;
        public byte m_eFailureType;
    }

    //-----------------------------------------------------------------------------
    // Purpose: Signaled whenever licenses change
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct LicensesUpdated_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 25;
    }

    //-----------------------------------------------------------------------------
    // callback for BeginAuthSession
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct ValidateAuthTicketResponse_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 43;
        public SteamID m_SteamID;
        public EAuthSessionResponse m_eAuthSessionResponse;
        public SteamID m_OwnerSteamID; // different from m_SteamID if borrowed
    }

    //-----------------------------------------------------------------------------
    // Purpose: called when a user has responded to a microtransaction authorization request
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct MicroTxnAuthorizationResponse_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 52;

        public uint m_unAppID;          // AppID for this microtransaction
        public ulong m_ulOrderID;           // OrderID provided for the microtransaction
        public byte m_bAuthorized;      // if user authorized transaction
    }

    //-----------------------------------------------------------------------------
    // Purpose: Result from RequestEncryptedAppTicket
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct EncryptedAppTicketResponse_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 54;

        public EResult m_eResult;
    }

    //-----------------------------------------------------------------------------
    // callback for GetAuthSessionTicket
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GetAuthSessionTicketResponse_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 63;
        public HAuthTicket m_hAuthTicket;
        public EResult m_eResult;
    }

    //-----------------------------------------------------------------------------
    // Purpose: sent to your game in response to a steam://gamewebcallback/ command
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GameWebCallback_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 64;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string m_szURL;
    }

    //-----------------------------------------------------------------------------
    // Purpose: sent to your game in response to ISteamUser::RequestStoreAuthURL
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct StoreAuthURLResponse_t
    {
        public const int k_iCallback = Constants.k_iSteamUserCallbacks + 65;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string m_szURL;
    }

    // callbacks
    //-----------------------------------------------------------------------------
    // Purpose: called when the latests stats and achievements have been received
    //			from the server
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Explicit, Pack = Packsize.value)]
    public struct UserStatsReceived_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 1;
        [FieldOffset(0)]
        public ulong m_nGameID;     // Game these stats are for
        [FieldOffset(8)]
        public EResult m_eResult;       // Success / error fetching the stats
        [FieldOffset(12)]
        public SteamID m_steamIDUser;  // The user for whom the stats are retrieved for
    }

    //-----------------------------------------------------------------------------
    // Purpose: result of a request to store the user stats for a game
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct UserStatsStored_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 2;
        public ulong m_nGameID;     // Game these stats are for
        public EResult m_eResult;       // success / error
    }

    //-----------------------------------------------------------------------------
    // Purpose: result of a request to store the achievements for a game, or an
    //			"indicate progress" call. If both m_nCurProgress and m_nMaxProgress
    //			are zero, that means the achievement has been fully unlocked.
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct UserAchievementStored_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 3;

        public ulong m_nGameID;             // Game this is for
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bGroupAchievement;    // if this is a "group" achievement
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cchStatNameMax)]
        public string m_rgchAchievementName;        // name of the achievement
        public uint m_nCurProgress;         // current progress towards the achievement
        public uint m_nMaxProgress;         // "out of" this many
    }

    //-----------------------------------------------------------------------------
    // Purpose: call result for finding a leaderboard, returned as a result of FindOrCreateLeaderboard() or FindLeaderboard()
    //			use CCallResult<> to map this async result to a member function
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LeaderboardFindResult_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 4;
        public SteamLeaderboard_t m_hSteamLeaderboard;  // handle to the leaderboard serarched for, 0 if no leaderboard found
        public byte m_bLeaderboardFound;                // 0 if no leaderboard found
    }

    //-----------------------------------------------------------------------------
    // Purpose: call result indicating scores for a leaderboard have been downloaded and are ready to be retrieved, returned as a result of DownloadLeaderboardEntries()
    //			use CCallResult<> to map this async result to a member function
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LeaderboardScoresDownloaded_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 5;
        public SteamLeaderboard_t m_hSteamLeaderboard;
        public SteamLeaderboardEntries_t m_hSteamLeaderboardEntries;    // the handle to pass into GetDownloadedLeaderboardEntries()
        public int m_cEntryCount; // the number of entries downloaded
    }

    //-----------------------------------------------------------------------------
    // Purpose: call result indicating scores has been uploaded, returned as a result of UploadLeaderboardScore()
    //			use CCallResult<> to map this async result to a member function
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LeaderboardScoreUploaded_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 6;
        public byte m_bSuccess;         // 1 if the call was successful
        public SteamLeaderboard_t m_hSteamLeaderboard;  // the leaderboard handle that was
        public int m_nScore;                // the score that was attempted to set
        public byte m_bScoreChanged;        // true if the score in the leaderboard change, false if the existing score was better
        public int m_nGlobalRankNew;        // the new global rank of the user in this leaderboard
        public int m_nGlobalRankPrevious;   // the previous global rank of the user in this leaderboard; 0 if the user had no existing entry in the leaderboard
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct NumberOfCurrentPlayers_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 7;
        public byte m_bSuccess;         // 1 if the call was successful
        public int m_cPlayers;          // Number of players currently playing
    }

    //-----------------------------------------------------------------------------
    // Purpose: Callback indicating that a user's stats have been unloaded.
    //  Call RequestUserStats again to access stats for this user
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct UserStatsUnloaded_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 8;
        public SteamID m_steamIDUser;  // User whose stats have been unloaded
    }

    //-----------------------------------------------------------------------------
    // Purpose: Callback indicating that an achievement icon has been fetched
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct UserAchievementIconFetched_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 9;

        public CGameID m_nGameID;               // Game this is for
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Constants.k_cchStatNameMax)]
        public string m_rgchAchievementName;        // name of the achievement
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bAchieved;        // Is the icon for the achieved or not achieved version?
        public int m_nIconHandle;       // Handle to the image, which can be used in SteamUtils()->GetImageRGBA(), 0 means no image is set for the achievement
    }

    //-----------------------------------------------------------------------------
    // Purpose: Callback indicating that global achievement percentages are fetched
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GlobalAchievementPercentagesReady_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 10;

        public ulong m_nGameID;             // Game this is for
        public EResult m_eResult;               // Result of the operation
    }

    //-----------------------------------------------------------------------------
    // Purpose: call result indicating UGC has been uploaded, returned as a result of SetLeaderboardUGC()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LeaderboardUGCSet_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 11;
        public EResult m_eResult;               // The result of the operation
        public SteamLeaderboard_t m_hSteamLeaderboard;  // the leaderboard handle that was
    }

    //-----------------------------------------------------------------------------
    // Purpose: callback indicating global stats have been received.
    //	Returned as a result of RequestGlobalStats()
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GlobalStatsReceived_t
    {
        public const int k_iCallback = Constants.k_iSteamUserStatsCallbacks + 12;
        public ulong m_nGameID;             // Game global stats were requested for
        public EResult m_eResult;               // The result of the request
    }

    // callbacks
    //-----------------------------------------------------------------------------
    // Purpose: The country of the user changed
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct IPCountry_t
    {
        public const int k_iCallback = Constants.k_iSteamUtilsCallbacks + 1;
    }

    //-----------------------------------------------------------------------------
    // Purpose: Fired when running on a laptop and less than 10 minutes of battery is left, fires then every minute
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct LowBatteryPower_t
    {
        public const int k_iCallback = Constants.k_iSteamUtilsCallbacks + 2;
        public byte m_nMinutesBatteryLeft;
    }

    //-----------------------------------------------------------------------------
    // Purpose: called when a SteamAsyncCall_t has completed (or failed)
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct SteamAPICallCompleted_t
    {
        public const int k_iCallback = Constants.k_iSteamUtilsCallbacks + 3;
        public SteamAPICall_t m_hAsyncCall;
    }

    //-----------------------------------------------------------------------------
    // called when Steam wants to shutdown
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct SteamShutdown_t
    {
        public const int k_iCallback = Constants.k_iSteamUtilsCallbacks + 4;
    }

    //-----------------------------------------------------------------------------
    // callback for CheckFileSignature
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct CheckFileSignature_t
    {
        public const int k_iCallback = Constants.k_iSteamUtilsCallbacks + 5;
        public ECheckFileSignature m_eCheckFileSignature;
    }

    // k_iSteamUtilsCallbacks + 13 is taken
    //-----------------------------------------------------------------------------
    // Big Picture gamepad text input has been closed
    //-----------------------------------------------------------------------------
    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GamepadTextInputDismissed_t
    {
        public const int k_iCallback = Constants.k_iSteamUtilsCallbacks + 14;
        [MarshalAs(UnmanagedType.I1)]
        public bool m_bSubmitted;                                       // true if user entered & accepted text (Call ISteamUtils::GetEnteredGamepadTextInput() for text), false if canceled input
        public uint m_unSubmittedText;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value, Size = 1)]
    public struct BroadcastUploadStart_t
    {
        public const int k_iCallback = Constants.k_iClientVideoCallbacks + 4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct BroadcastUploadStop_t
    {
        public const int k_iCallback = Constants.k_iClientVideoCallbacks + 5;
        public EBroadcastUploadResult m_eResult;
    }

    [StructLayout(LayoutKind.Sequential, Pack = Packsize.value)]
    public struct GetVideoURLResult_t
    {
        public const int k_iCallback = Constants.k_iClientVideoCallbacks + 11;
        public EResult m_eResult;
        public AppId_t m_unVideoAppID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string m_rgchURL;
    }

}

