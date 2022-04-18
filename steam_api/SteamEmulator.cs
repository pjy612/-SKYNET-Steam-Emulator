﻿#define LOG
using SKYNET;
using SKYNET.Helpers;
using SKYNET.Managers;
using SKYNET.Steamworks;
using SKYNET.Steamworks.Implementation;
using SKYNET.Steamworks.Types;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class SteamEmulator
{
    public static SteamEmulator Instance;

    public static event EventHandler<GameMessage> OnMessage;

    #region Client Info

    public static string Language;
    public static string PersonaName;
    public static string SteamApiPath;
    public static string EmulatorPath;

    public static SteamID SteamId;
    public static SteamID SteamId_GS;
    public static ulong GameID;
    public static uint AppId;
    public static bool Initialized;
    public static bool SendLog;

    public static int HSteamUser;
    public static int HSteamPipe;

    public static int HSteamUser_GS;
    public static int HSteamPipe_GS;

    #endregion

    #region Interfaces 

    //Client
    public static SteamClient SteamClient;
    public static SteamUser SteamUser;
    public static SteamFriends SteamFriends;
    public static SteamUtils SteamUtils;
    public static SteamMatchmaking SteamMatchmaking;
    public static SteamMatchMakingServers SteamMatchMakingServers;
    public static SteamUserStats SteamUserStats;
    public static SteamApps SteamApps;
    public static SteamNetworking SteamNetworking;
    public static SteamRemoteStorage SteamRemoteStorage;
    public static SteamScreenshots SteamScreenshots;
    public static SteamHTTP SteamHTTP;
    public static SteamController SteamController;
    public static SteamUGC SteamUGC;
    public static SteamAppList SteamAppList;
    public static SteamMusic SteamMusic;
    public static SteamMusicRemote SteamMusicRemote;
    public static SteamHTMLSurface SteamHTMLSurface;
    public static SteamInventory SteamInventory;
    public static SteamVideo SteamVideo;
    public static SteamParentalSettings SteamParentalSettings;
    public static SteamNetworkingSockets SteamNetworkingSockets;
    public static SteamNetworkingSocketsSerialized SteamNetworkingSocketsSerialized;
    public static SteamNetworkingMessages SteamNetworkingMessages;
    public static SteamGameCoordinator SteamGameCoordinator;
    public static SteamNetworkingUtils SteamNetworkingUtils;
    public static SteamGameSearch SteamGameSearch;
    public static SteamInput SteamInput;
    public static SteamParties SteamParties;
    public static SteamRemotePlay SteamRemotePlay;
    public static SteamTV SteamTV;

    //GameServer
    public static SteamGameServer SteamGameServer;
    public static SteamUtils SteamGameServerUtils;
    public static SteamGameServerStats SteamGameServerStats;
    public static SteamNetworking SteamGameServerNetworking;
    public static SteamHTTP SteamGameServerHTTP;
    public static SteamInventory SteamGameServerInventory;
    public static SteamUGC SteamGameServerUgc;
    public static SteamApps SteamGameServerApps;
    public static SteamNetworkingSockets SteamGameServerNetworkingSockets;
    public static SteamNetworkingSocketsSerialized SteamGameServerNetworkingSocketsSerialized;
    public static SteamNetworkingMessages SteamGameServerNetworkingMessages;
    public static SteamGameCoordinator SteamGameServerGamecoordinator;
    public static SteamMasterServerUpdater SteamMasterServerUpdater;

    #endregion

    public SteamEmulator()
    {
        Instance = this;
    }

    public static void Initialize()
    {
        if (Initialized) return;

        LoadCustomVars();

        SteamId_GS = new SteamID();
        SteamId_GS.Set((uint)new Random().Next(1000, 9999), SKYNET.Steamworks.EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeGameServer);

        InterfaceManager.Initialize();

        #region Interface Initialization

        // Client Interfaces

        SteamClient = new SteamClient();

        SteamUser = new SteamUser();

        SteamFriends = new SteamFriends();

        SteamUtils = new SteamUtils();

        SteamMatchmaking = new SteamMatchmaking();

        SteamMatchMakingServers = new SteamMatchMakingServers();

        SteamUserStats = new SteamUserStats();

        SteamApps = new SteamApps();

        SteamNetworking = new SteamNetworking();

        SteamRemoteStorage = new SteamRemoteStorage();

        SteamScreenshots = new SteamScreenshots();

        SteamHTTP = new SteamHTTP();

        SteamController = new SteamController();

        SteamUGC = new SteamUGC();

        SteamAppList = new SteamAppList();

        SteamMusic = new SteamMusic();

        SteamMusicRemote = new SteamMusicRemote();

        SteamHTMLSurface = new SteamHTMLSurface();

        SteamInventory = new SteamInventory();

        SteamVideo = new SteamVideo();

        SteamParentalSettings = new SteamParentalSettings();

        SteamNetworkingSockets = new SteamNetworkingSockets();

        SteamNetworkingSocketsSerialized = new SteamNetworkingSocketsSerialized();

        SteamNetworkingMessages = new SteamNetworkingMessages();

        SteamGameCoordinator = new SteamGameCoordinator();

        SteamNetworkingUtils = new SteamNetworkingUtils();

        SteamGameSearch = new SteamGameSearch();

        SteamParties = new SteamParties();

        SteamRemotePlay = new SteamRemotePlay();

        SteamTV = new SteamTV();

        SteamInput = new SteamInput();


        // Server Interfaces

        SteamGameServer = new SteamGameServer();

        SteamGameServerUtils = new SteamUtils();

        SteamGameServerStats = new SteamGameServerStats();

        SteamGameServerNetworking = new SteamNetworking();

        SteamHTTP = new SteamHTTP();

        SteamGameServerInventory = new SteamInventory();

        SteamGameServerUgc = new SteamUGC();

        SteamGameServerApps = new SteamApps();

        SteamGameServerNetworkingSockets = new SteamNetworkingSockets();

        SteamGameServerNetworkingSocketsSerialized = new SteamNetworkingSocketsSerialized();

        SteamGameServerNetworkingMessages = new SteamNetworkingMessages();

        SteamGameServerGamecoordinator = new SteamGameCoordinator();

        SteamMasterServerUpdater = new SteamMasterServerUpdater();

        #endregion

        HSteamUser = 1;
        HSteamPipe = 1;

        HSteamUser_GS = 2;
        HSteamPipe_GS = 2;

        Initialized = true;

    }

    public static int CreateSteamUser()
    {
        if (HSteamUser == 0)
        {
            HSteamUser = 1;
            Write($"Creating user {HSteamUser}");
        }
        return HSteamUser;
    }

    public static int CreateSteamPipe()
    {
        if (HSteamPipe == 0)
        {
            HSteamPipe = 1;
            Write($"Creating pipe {HSteamPipe}");
        }
        return HSteamPipe;
    }

    private static void Write(string msg)
    {
        Write("Steam Emulator", msg);
    }

#if LOG

    
    public static void Write(string sender, object msg)
    {
        if (SendLog)
        {
            OnMessage?.Invoke(Instance, new GameMessage(AppId, sender, msg));
        }

        Console.WriteLine(sender + ": " + msg);
        Log.AppEnd(sender + ": " + msg);
    }

#else
    public static void Write(string sender, object msg)
    {
        // TODO
    }

#endif

    private static void LoadCustomVars()
    {
        modCommon.ActiveConsoleOutput();

        string fileName = modCommon.GetPath() + "/[SKYNET] steam_api.log";
        File.WriteAllLines(fileName, new List<string>());

        Language = "English";
        PersonaName = "Hackerprod";
        //SteamId = new SteamID(76561198429375037); My online SteamID
        SteamId = new SteamID(76561197960266730);
        
        SteamId_GS = 1;
        AppId = 570;

        HSteamUser = 1;
        HSteamPipe = 1;

        HSteamUser_GS = 2;
        HSteamPipe_GS = 2;


        SteamApiPath = Path.Combine(modCommon.GetPath(), "steam_api64.dll");
        EmulatorPath = @"D:\Instaladores\Programación\Projects\[SKYNET] Steam Emulator\[SKYNET] Steam Emulator\bin\Debug";
        SendLog = true;
    }
}



