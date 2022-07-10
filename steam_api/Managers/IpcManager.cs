﻿using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Helper.JSON;
using SKYNET.IPC;
using SKYNET.IPC.Types;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Implementation;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SKYNET.Managers
{
    public class IPCManager
    {
        private const ulong IPC_ToServer  = 0;
        private const ulong IPC_Broadcast = 1;
        private const ulong IPC_Client = 2;
        
        private static PipeClient IPCClient;

        static IPCManager()
        {
            IPCClient = new PipeClient("SKYNET");
            IPCClient.AutoReconnect = true;
            IPCClient.Connected += IPCClient_Connected;
            IPCClient.Disconnected += IPCClient_Disconnected;
            IPCClient.MessageReceived += IPCClient_MessageReceived;
        }

        public static void Initialize()
        {
            ThreadPool.QueueUserWorkItem(Connect); 
        }

        private async static void Connect(object state)
        {
            await IPCClient.ConnectAsync();
        }

        private static void IPCClient_Connected(object sender, ConnectionEventArgs<IPCMessage> e)
        {
            Write("IPCClient Connected to server");
            SendClientHello();
        }

        private static void IPCClient_Disconnected(object sender, ConnectionEventArgs<IPCMessage> e)
        {
            Write("IPCClient Disconnected");
            ThreadPool.QueueUserWorkItem(Connect);
        }

        private static void IPCClient_MessageReceived(object sender, ConnectionMessageEventArgs<IPCMessage> message)
        {
            ProcessMessage(message.Message);
        }

        private static void ProcessMessage(IPCMessage message)
        {
            Write($"Received IPC message {(IPCMessageType)message.MessageType}");
            switch ((IPCMessageType)message.MessageType)
            {
                case IPCMessageType.IPC_ClientWelcome:
                    ProcessClientWelcome(message);
                    break;
                case IPCMessageType.IPC_AvatarResponse:
                    ProcessAvatarResponse(message);
                    break;
                case IPCMessageType.IPC_UserDataUpdated:
                    ProcessUserDataUpdated(message);
                    break;
                case IPCMessageType.IPC_LobbyListRequest:
                    ProcessLobbyListRequest(message);
                    break;
                case IPCMessageType.IPC_LobbyListResponse:
                    ProcessLobbyListResponse(message);
                    break;
                case IPCMessageType.IPC_LobbyJoinRequest:
                    ProcessLobbyJoinRequest(message);
                    break;
                case IPCMessageType.IPC_LobbyJoinResponse:
                    ProcessLobbyJoinResponse(message);
                    break;
                case IPCMessageType.IPC_LobbyDataUpdate:
                    ProcessLobbyDataUpdate(message);
                    break;
                case IPCMessageType.IPC_LobbyChatUpdate:
                    ProcessLobbyChatUpdate(message);
                    break;
                case IPCMessageType.IPC_LobbyLeave:
                    ProcessLobbyLeave(message);
                    break;
                case IPCMessageType.IPC_LobbyRemove:
                    ProcessLobbyRemove(message);
                    break;
                case IPCMessageType.IPC_LobbyGameserver:
                    ProcessLobbyGameserver(message);
                    break;
                case IPCMessageType.IPC_Achievements:
                    ProcessAchievements(message);
                    break;
                case IPCMessageType.IPC_Leaderboards:
                    ProcessLeaderboards(message);
                    break;
                case IPCMessageType.IPC_PlayerStats:
                    ProcessPlayerStats(message);
                    break;
                case IPCMessageType.IPC_P2PPacket:
                    ProcessP2PPacket(message);
                    break;
                case IPCMessageType.IPC_ModifyFileLog:
                    ProcessModifyFileLog(message);
                    break;
                default:
                    Write($"Not found Handle for message {(IPCMessageType)message.MessageType}");
                    break;
            }

        }

        private static void ProcessModifyFileLog(IPCMessage message)
        {
            var ModifyFileLog = message.ParsedBody.FromJson<IPC_ModifyFileLog>();
            if (ModifyFileLog != null)
            {
                SteamEmulator.LogToFile = ModifyFileLog.Enabled;
                if (ModifyFileLog.Enabled)
                {
                    string logPath = Path.Combine(modCommon.GetPath(), "SKYNET");
                    modCommon.EnsureDirectoryExists(logPath);
                }
            }
        }

        private static void ProcessDisableFileLog(IPCMessage message)
        {
            SteamEmulator.LogToFile = false;
        }

        #region IPC_ClientHello

        private static void SendClientHello()
        {
            var ClientHello = new IPC_ClientHello()
            {
                ExecutablePath = modCommon.GetExecutablePath(),
                ProcessID = Process.GetCurrentProcess().Id
            };
            var message = CreateIPCMessage(ClientHello, IPCMessageType.IPC_ClientHello);

            var messageWelcome = SendTo(IPC_ToServer, message, true);
            if (messageWelcome != null)
            {
                ProcessClientWelcome(messageWelcome);
            }
        }

        private static void ProcessClientWelcome(IPCMessage message)
        {
            try
            {
                var ClientWelcome = message.ParsedBody.FromJson<IPC_ClientWelcome>();
                SteamEmulator.PersonaName = ClientWelcome.PersonaName;
                SteamEmulator.SteamID = new CSteamID(ClientWelcome.AccountID);
                SteamEmulator.SteamID_GS = new CSteamID(ClientWelcome.GameServerID);
                SteamEmulator.GameOverlay = ClientWelcome.GameOverlay;
                SteamEmulator.LogToFile = ClientWelcome.LogToFile;
                SteamEmulator.LogToConsole = ClientWelcome.LogToConsole;
                SteamEmulator.RunCallbacks = ClientWelcome.RunCallbacks;
                SteamEmulator.AppID = ClientWelcome.AppID;
                SteamEmulator.ISteamHTTP = ClientWelcome.ISteamHTTP;
                SteamEmulator.SteamRemoteStorage.StoragePath = ClientWelcome.RemoteStoragePath;

                if (ClientWelcome.LogToFile)
                {
                    string logPath = Path.Combine(modCommon.GetPath(), "SKYNET");
                    modCommon.EnsureDirectoryExists(logPath);
                }
                if (ClientWelcome.LogToConsole)
                {
                    ConsoleHelper.CreateConsole("SKYNET");
                }

                SteamFriends.Instance.Initialize();
            }
            catch
            {

            }
        }

        #endregion

        private static void ProcessPlayerStats(IPCMessage message)
        {
            var PlayerStats = message.ParsedBody.FromJson<IPC_PlayerStats>();
            if (PlayerStats != null)
            {
                SteamUserStats.Instance?.SetPlayerStats(PlayerStats.SteamID, PlayerStats.PlayerStats);
            }
        }

        private static void ProcessAchievements(IPCMessage message)
        {
            var Achievements = message.ParsedBody.FromJson<IPC_Achievements>();
            if (Achievements != null)
            {
                SteamUserStats.Instance?.SetAchievements(Achievements.Achievements);
            }
        }

        private static void ProcessLeaderboards(IPCMessage message)
        {
            var Leaderboards = message.ParsedBody.FromJson<IPC_Leaderboards>();
            if (Leaderboards != null)
            {
                SteamUserStats.Instance?.SetLeaderboards(Leaderboards.Leaderboards);
            }
        }

        public static void SendP2PTo(ulong steamIDRemote, IPC_P2PPacket packet)
        {
            var message = CreateIPCMessage(packet, IPCMessageType.IPC_P2PPacket);
            SendTo(steamIDRemote, message);
        }

        private static void ProcessP2PPacket(IPCMessage message)
        {
            var P2PPacket = message.ParsedBody.FromJson<IPC_P2PPacket>();
            if (P2PPacket == null) return;
            SteamNetworking.Instance.ProcessP2PPacket(P2PPacket); 
        }

        private static void ProcessLobbyGameserver(IPCMessage message)
        {
            var lobbyGameserver = message.ParsedBody.FromJson<IPC_LobbyGameserver>();
            if (lobbyGameserver != null)
            {
                if (SteamMatchmaking.Instance.GetLobby(lobbyGameserver.LobbyID, out var lobby))
                {
                    Write($"Received gameserver data for lobby {lobbyGameserver.LobbyID}, IP = {lobbyGameserver.IP}, Port = {lobbyGameserver.Port}");
                    lobby.Gameserver.SteamID = lobbyGameserver.SteamID;
                    lobby.Gameserver.IP = lobbyGameserver.IP;
                    lobby.Gameserver.Port = lobbyGameserver.Port;
                    lobby.Gameserver.Filled = true;

                    // TODO: Necessary?
                    GameServerChangeRequested_t data = new GameServerChangeRequested_t()
                    {
                        m_rgchServer = $"{lobbyGameserver.IP}:{lobbyGameserver.Port}"
                    };
                    CallbackManager.AddCallbackResult(data);
                }
            }
        }

        private static void ProcessLobbyLeave(IPCMessage message)
        {
            var lobbyLeave = message.ParsedBody.FromJson<IPC_LobbyLeave>();
            if (lobbyLeave != null)
            {
                if (SteamMatchmaking.Instance.GetLobby(lobbyLeave.LobbyID, out var lobby))
                {
                    var lobbyChatUpdate = new IPC_LobbyChatUpdate()
                    {
                        SteamIDLobby = lobby.SteamID,
                        SteamIDUserChanged = lobbyLeave.SteamID,
                        SteamIDMakingChange = lobbyLeave.SteamID,
                        ChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeLeft
                    };

                    lobby.Members.RemoveAll(m => m.m_SteamID == lobbyLeave.SteamID);

                    var ChatUpdateMessage = CreateIPCMessage(lobbyChatUpdate, IPCMessageType.IPC_LobbyChatUpdate);

                    foreach (var member in lobby.Members)
                    {
                        var user = GetUser(member.m_SteamID);
                        if (user != null)
                        {
                            SendTo(user.SteamID, ChatUpdateMessage);
                            //SteamMatchmaking.Instance.LobbyDataUpdated();
                        }
                    }

                    var data = new LobbyChatUpdate_t()
                    {
                        m_ulSteamIDLobby = lobbyLeave.LobbyID,
                        m_ulSteamIDUserChanged = lobbyLeave.SteamID,
                        m_ulSteamIDMakingChange = lobbyLeave.SteamID,
                        m_rgfChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeLeft
                    };
                    CallbackManager.AddCallback(data);
                }
            }
        }

        public static void SendCreateLobby(SteamLobby lobby)
        {
            IPC_LobbyCreate CreateLobby = new IPC_LobbyCreate()
            {
                SerializedLobby = lobby.ToJson()
            };
            var message = CreateIPCMessage(CreateLobby, IPCMessageType.IPC_LobbyCreate);
            SendTo(IPC_ToServer, message);
        }


        public static void SendAchievement(Achievement achievement)
        {
            IPC_SetAchievement Achievement = new IPC_SetAchievement()
            {
                AppID = SteamEmulator.AppID,
                Achievement = achievement
            };
            var message = CreateIPCMessage(Achievement, IPCMessageType.IPC_SetAchievement);
            SendTo(IPC_ToServer, message);
        }

        public static void SendUpdateAchievement(Achievement achievement)
        {
            IPC_UpdateAchievement Achievement = new IPC_UpdateAchievement()
            {
                AppID = SteamEmulator.AppID,
                Achievement = achievement
            };
            var message = CreateIPCMessage(Achievement, IPCMessageType.IPC_UpdateAchievement);
            SendTo(IPC_ToServer, message);
        }

        public static void SendPlayerStat(PlayerStat playerStat)
        {
            var PlayerStat = new IPC_SetPlayerStat()
            {
                AppID = SteamEmulator.AppID,
                PlayerStat = playerStat
            };
            var message = CreateIPCMessage(PlayerStat, IPCMessageType.IPC_SetPlayerStat);
            SendTo(IPC_ToServer, message);
        }

        public static void SendLeaderboard(Leaderboard leaderboard)
        {
            var Leaderboard = new IPC_SetLeaderboard()
            {
                AppID = SteamEmulator.AppID,
                Leaderboard = leaderboard
            };
            var message = CreateIPCMessage(Leaderboard, IPCMessageType.IPC_SetLeaderboard);
            SendTo(IPC_ToServer, message);
        }

        public static void SendResetAllStats(bool bAchievementsToo)
        {
            var ResetAllStats = new IPC_ResetAllStats()
            {
                AppID = SteamEmulator.AppID,
                AchievementsToo = bAchievementsToo
            };
            var message = CreateIPCMessage(ResetAllStats, IPCMessageType.IPC_ResetAllStats);
            SendTo(IPC_ToServer, message);
        }

        private static void ProcessLobbyRemove(IPCMessage message)
        {
            var lobbyRemove = message.ParsedBody.FromJson<IPC_LobbyRemove>();
            if (lobbyRemove != null)
            {
                SteamMatchmaking.Instance.RemoveLobby(lobbyRemove.LobbyID);
            }
        }

        private static void ProcessLobbyChatUpdate(IPCMessage message)
        {
            var lobbyChatUpdate = message.ParsedBody.FromJson<IPC_LobbyChatUpdate>();
            if (lobbyChatUpdate != null)
            {
                SteamMatchmaking.Instance.LobbyChatUpdated(lobbyChatUpdate);
            }
        }

        private static void ProcessLobbyDataUpdate(IPCMessage message)
        {
            var lobbyDataUpdate = message.ParsedBody.FromJson<IPC_LobbyDataUpdate>();
            if (lobbyDataUpdate != null)
            {
                SteamMatchmaking.Instance.LobbyDataUpdated(lobbyDataUpdate);
            }
        }

        private static void ProcessLobbyJoinRequest(IPCMessage message)
        {
            var lobbyJoinRequest = message.ParsedBody.FromJson<IPC_LobbyJoinRequest>();
            var lobbyJoinResponse = new IPC_LobbyJoinResponse()
            {
                CallbackHandle = lobbyJoinRequest.CallbackHandle,
                ChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess,
            };
            if (lobbyJoinRequest != null)
            {
                if (SteamMatchmaking.Instance.GetLobby(lobbyJoinRequest.LobbyID, out var lobby))
                {
                    if (lobby.Members.Count >= lobby.MaxMembers)
                    {
                        lobbyJoinResponse.ChatRoomEnterResponse = (uint)EChatRoomEnterResponse.k_EChatRoomEnterResponseFull;
                    }

                    var Member = lobby.Members.Find(m => m.m_SteamID == lobbyJoinRequest.SteamID);
                    if (Member == null)
                    {
                        lobby.Members.Add(new SteamLobby.LobbyMember()
                        {
                            m_SteamID = lobbyJoinRequest.SteamID
                        });
                    }

                    SteamFriends.Instance.UpdateUserLobby(lobbyJoinRequest.SteamID, lobbyJoinRequest.LobbyID);

                    string serialized = lobby.ToJson();

                    lobbyJoinResponse.SerializedLobby = serialized;

                    var lobbyJoinMessage = CreateIPCMessage(lobbyJoinResponse, IPCMessageType.IPC_LobbyJoinResponse);

                    try
                    {
                        SendTo(lobbyJoinRequest.SteamID, lobbyJoinMessage);
                    }
                    catch
                    {
                    }

                    // LobbyDataUpdate
                    var lobbyDataUpdate = new IPC_LobbyDataUpdate()
                    {
                        SteamIDLobby = lobby.SteamID,
                        SteamIDMember = lobbyJoinRequest.SteamID,
                        SerializedLobby = serialized
                    };

                    var lobbyChatUpdate = new IPC_LobbyChatUpdate()
                    {
                        SteamIDLobby = lobby.SteamID,
                        SteamIDUserChanged = lobbyJoinRequest.SteamID,
                        SteamIDMakingChange = lobbyJoinRequest.SteamID,
                        ChatMemberStateChange = (int)EChatMemberStateChange.k_EChatMemberStateChangeEntered
                    };

                    var DataUpdateMessage = CreateIPCMessage(lobbyDataUpdate, IPCMessageType.IPC_LobbyDataUpdate);
                    var ChatUpdateMessage = CreateIPCMessage(lobbyChatUpdate, IPCMessageType.IPC_LobbyChatUpdate);

                    foreach (var member in lobby.Members)
                    {
                        var user = GetUser(member.m_SteamID);
                        if (user != null)
                        {
                            SendTo(user.SteamID, DataUpdateMessage);
                            SendTo(user.SteamID, ChatUpdateMessage);
                        }
                    }
                }
            }
        }

        private static void ProcessLobbyJoinResponse(IPCMessage message)
        {
            try
            {
                var lobbyJoinResponse = message.ParsedBody.FromJson<IPC_LobbyJoinResponse>();
                if (lobbyJoinResponse != null)
                {
                    var lobby = lobbyJoinResponse.SerializedLobby.FromJson<SteamLobby>();
                    if (lobby != null)
                    {
                        SteamMatchmaking.Instance.JoinResponse(lobbyJoinResponse, lobby);
                    }
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        private static void ProcessLobbyListRequest(IPCMessage message)
        {
            //try
            //{
            //    var lobbyListRequest = message.ParsedBody.FromJson<IPC_LobbyListRequest>();
            //    if (lobbyListRequest != null)
            //    {
            //        //if (lobbyListRequest.RequestID == SteamMatchmaking.Instance.CurrentRequest)
            //        //{

            //        //    return;
            //        //}
            //        if (lobbyListRequest.AppID != 0 && lobbyListRequest.AppID != SteamEmulator.AppID)
            //        {
            //            return;
            //        }
            //    }
            //    var lobby = SteamMatchmaking.Instance.GetLobbyByOwner((ulong)SteamEmulator.SteamID);
            //    if (lobby == null)
            //    {

            //    }
            //    else
            //    {
            //        string serialized = lobby.ToJson();

            //        var lobbyListResponse = new IPC_LobbyListResponse()
            //        {
            //            SerializedLobby = serialized
            //        };

            //        var messageResponse = CreateNetworkMessage(lobbyListResponse, IPCMessageType.IPC_LobbyListResponse);

            //        SendTo(, messageResponse);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Write(ex);
            //}
        }

        internal static int GetUsersOnline(uint appID)
        {
            // TODO: Request to client
            return 100;
        }

        private static void ProcessLobbyListResponse(IPCMessage message)
        {
            try
            {
                var lobbyListResponse = message.ParsedBody.FromJson<IPC_LobbyListResponse>();
                var lobby = lobbyListResponse.SerializedLobby.FromJson<SteamLobby>();
                if (lobby != null)
                {
                    Write($"Adding lobby {lobby.SteamID}");
                    SteamMatchmaking.Instance.Lobbies.TryAdd(lobby.SteamID, lobby);
                }
            }
            catch (Exception ex)
            {
                Write(ex);
            }

        }

        private static void ProcessAvatarResponse(IPCMessage message)
        {
            try
            {
                var AvatarResponse = message.ParsedBody.FromJson<IPC_AvatarResponse>();
                if (AvatarResponse != null)
                {
                    var imageBytes = Convert.FromBase64String(AvatarResponse.HexAvatar);
                    if (imageBytes.Length != 0)
                    {
                        Bitmap Avatar = (Bitmap)ImageHelper.ImageFromBytes(imageBytes);
                        if (AvatarResponse.AccountID == 0)
                        {
                            SteamFriends.Instance.AddOrUpdateAvatar(Avatar, 0);
                        }
                        else
                        {
                            ulong SteamID = (ulong)new CSteamID(AvatarResponse.AccountID);
                            SteamFriends.Instance.AddOrUpdateAvatar(Avatar, SteamID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Write($"{ex}");
            }
        }

        private static void ProcessUserDataUpdated(IPCMessage message)
        {
            try
            {
                var StatusChanged = message.ParsedBody.FromJson<IPC_UserDataUpdated>();
                if (StatusChanged != null)
                {
                    SteamFriends.Instance.UpdateUserStatus(StatusChanged);
                }
            }
            catch
            {
                //
            }
        }

        public static void SendUserDataUpdated(SteamPlayer user, IPC_UserDataUpdated.UpdateType Type)
        {
            var status = new IPC_UserDataUpdated()
            {
                PersonaName = user.PersonaName,
                AccountID = (uint)SteamEmulator.SteamID.AccountID, 
                LobbyID = user.LobbyID.GetAccountID(),
                Type = Type
            };

            var message = CreateIPCMessage(status, IPCMessageType.IPC_UserDataUpdated);
            SendTo(IPC_Broadcast, message);
        }

        public static void SendLobbyJoinRequest(ulong APICall, SteamLobby lobby)
        {
            Write($"Sending lobby request");
            try
            {
                var JoinRequest = new IPC_LobbyJoinRequest()
                {
                    LobbyID = lobby.SteamID,
                    SteamID = (ulong)SteamEmulator.SteamID,
                    CallbackHandle = APICall
                };

                var message = CreateIPCMessage(JoinRequest, IPCMessageType.IPC_LobbyJoinRequest);

                var user = GetUser(lobby.Owner);
                if (user == null)
                {
                    Write($"Not found user to send LobbyJoinRequest, SteamID {new CSteamID(lobby.Owner).ToString()}");
                    return;
                }

                SendTo(user.SteamID, message);
            }
            catch (Exception ex)
            {
                Write(ex);
            }
        }

        public static void SendLobbyGameServer(SteamLobby lobby)
        {
            var lobbyGameserver = new IPC_LobbyGameserver()
            {
                LobbyID = lobby.SteamID,
                SteamID = lobby.Gameserver.SteamID, 
                IP = lobby.Gameserver.IP,
                Port = lobby.Gameserver.Port
            };

            var GameserverMessage = CreateIPCMessage(lobbyGameserver, IPCMessageType.IPC_LobbyGameserver);

            foreach (var member in lobby.Members)
            {
                if (member.m_SteamID != (ulong)SteamEmulator.SteamID)
                {
                    var user = GetUser(member.m_SteamID);
                    if (user != null)
                    {
                        SendTo(user.SteamID, GameserverMessage);
                    }
                }
            }
        }

        public static void SendLobbyListRequest(uint currentRequest)
        {
            var lobbyListRequest = new IPC_LobbyListRequest()
            {
                AppID = SteamEmulator.AppID,
                RequestID = currentRequest
            };

            var message = CreateIPCMessage(lobbyListRequest, IPCMessageType.IPC_LobbyListRequest);

            SendTo(IPC_Broadcast, message);
        }

        public static void SendLobbyDataUpdate(ulong IDTarget, ulong IDLobby, ulong IDMember, SteamLobby lobby)
        {
            var lobbyDataUpdate = new IPC_LobbyDataUpdate()
            { 
                TargetSteamID = IDTarget,
                SteamIDLobby = IDLobby,
                SteamIDMember = IDMember,
                SerializedLobby = lobby.ToJson()
            };

            var message = CreateIPCMessage(lobbyDataUpdate, IPCMessageType.IPC_LobbyDataUpdate);

            var user = GetUser(IDTarget);
            if (user != null)
            {
                SendTo(user.SteamID, message);
            }
        }

        public static void RequestAvatar(ulong SteamID)
        {
            var message = CreateIPCMessage(new IPC_MessageBase(), IPCMessageType.IPC_AvatarRequest);
            SendTo(SteamID, message);
        }

        public static void SendLobbyLeave(ulong owner, ulong lobbyID)
        {
            var user = GetUser(owner);
            if (user != null)
            {
                var lobbyLeave = new IPC_LobbyLeave()
                {
                    LobbyID = lobbyID,
                    SteamID = (ulong)SteamEmulator.SteamID
                };

                var message = CreateIPCMessage(lobbyLeave, IPCMessageType.IPC_LobbyLeave);

                SendTo(user.SteamID, message);
            }
        }

        public static void SendLobbyRemove(SteamLobby lobby)
        {
            foreach (var member in lobby.Members)
            {
                if (member.m_SteamID != lobby.Owner)
                {
                    var user = GetUser(member.m_SteamID);
                    if (user != null)
                    {
                        var lobbyRemove = new IPC_LobbyRemove()
                        {
                            LobbyID = lobby.SteamID
                        };

                        var message = CreateIPCMessage(lobbyRemove, IPCMessageType.IPC_LobbyRemove);
                        SendTo(user.SteamID, message);
                    }
                }
            }
        }

        public static List<SteamPlayer> GetFriends()
        {
            var FriendsRequest = new IPC_GetFriendsRequest();
            var messageRequest = CreateIPCMessage(FriendsRequest, IPCMessageType.IPC_GetFriendsRequest);

            var messageResponse = SendTo(IPC_Client, messageRequest, true);
            if (messageResponse != null)
            {
                var UserResponse = messageResponse.ParsedBody.FromJson<IPC_GetFriendsResponse>();
                return UserResponse.Friends;
            }
            return new List<SteamPlayer>();
        }

        public static SteamPlayer GetUser(ulong steamID)
        {
            var UserRequest = new IPC_GetUserRequest()
            {
                SteamID = steamID,
            };
            var messageRequest = CreateIPCMessage(UserRequest, IPCMessageType.IPC_GetUserRequest);

            var messageResponse = SendTo(IPC_ToServer, messageRequest, true);
            if (messageResponse != null)
            {
                var UserResponse = messageResponse.ParsedBody.FromJson<IPC_GetUserResponse>();
                return UserResponse.User;
            }
            return null;
        }

        public static void GCRequest(uint MsgType, byte[] bytes)
        {
            var IPC_GCRequest = new IPC_GCMessageRequest()
            {
                MsgType = MsgType,
                Buffer = bytes
            };
            var message = CreateIPCMessage(IPC_GCRequest, IPCMessageType.IPC_GCMessageRequest);
            SendTo(IPC_ToServer, message);
        }


        private static IPCMessage SendTo(ulong SteamID, IPCMessage message, bool WaitResponse = false)
        {
            message.To = SteamID;
            if (IPCClient.IsConnected)
            {
                var TaskResponse = IPCClient.WriteAsync(message, WaitResponse);
                if (TaskResponse.Result == null) return null;
                return TaskResponse.Result;
            }
            else
            {
                IPCClient.ConnectAsync();
            }
            return null;
        }

        private static IPCMessage CreateIPCMessage(IPC_MessageBase Base, IPCMessageType type, ulong jobID = 0)
        {
            var message = new IPCMessage()
            {
                JobID = jobID,
                MessageType = (int)type,
                ParsedBody = Base.ToJson()
            };
            return message;
        }

        private static void Write(object v)
        {
            SteamEmulator.Write("IPCManager", v);
        }
    }
}
