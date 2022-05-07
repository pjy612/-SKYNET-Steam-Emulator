﻿using SKYNET.Callback;
using SKYNET.Helper;
using SKYNET.Helper.JSON;
using SKYNET.Managers;
using SKYNET.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SteamAPICall_t = System.UInt64;
using SteamLeaderboard_t = System.UInt64;

namespace SKYNET.Steamworks.Implementation
{
    public class SteamUserStats : ISteamInterface
    {
        private List<Leaderboard> Leaderboards;
        private List<Achievement> Achievements;
        private SteamAPICall_t k_uAPICallInvalid = 0x0;

        public SteamUserStats()
        {
            InterfaceName = "SteamUserStats";
            Leaderboards = new List<Leaderboard>();
            Achievements = new List<Achievement>();

            string achievementsPath = Path.Combine(modCommon.GetPath(), "SKYNET", "Storage", "Achievements.json");
            if (File.Exists(achievementsPath))
            {
                string fileContent = File.ReadAllText(achievementsPath);
                Achievements = fileContent.FromJson<List<Achievement>>();
            }
        }

        public bool RequestCurrentStats()
        {
            try
            {
                Write($"RequestCurrentStats");
                UserStatsReceived_t data = new UserStatsReceived_t()
                {
                    m_nGameID = SteamEmulator.GameID,
                    m_eResult = EResult.k_EResultOK,
                    m_steamIDUser = (ulong)SteamEmulator.SteamId
                };
                CallbackManager.AddCallbackResult(data);
                return true;
            }
            catch (Exception ex)
            {
                Write($"RequestCurrentStats {ex}");
                return false;
            }
        }

        public bool GetStat(string pchName, ref int pData)
        {
            Write($"GetStat {pchName}");
            if (string.IsNullOrEmpty(pchName) || pData == 0) return false;
            return false;
        }

        public bool GetStat(string pchName, ref float pData)
        {
            Write($"GetStat {pchName}");
            if (string.IsNullOrEmpty(pchName) || pData == 0) return false;
            return false;
        }

        public bool SetStat(string pchName, uint nData)
        {
            Write($"SetStat {pchName}");
            return false;
        }

        public bool UpdateAvgRateStat(string pchName, float flCountThisSession, double dSessionLength)
        {
            Write($"UpdateAvgRateStat {pchName}");
            return false;
        }

        public bool GetAchievement(string pchName, ref bool pbAchieved)
        {
            Write($"GetAchievement {pchName}");
            var Result = false;
            var achieved = false;
            MutexHelper.Wait("GetAchievement", delegate
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement == null)
                {
                    achieved = true;
                    Result = false;
                }
                else
                {
                    achieved = achievement.Earned;
                    Result = true;
                }
            });
            pbAchieved = achieved;
            return Result;
        }

        public bool SetAchievement(string pchName)
        {
            Write($"SetAchievement {pchName}");
            var Result = false;
            MutexHelper.Wait("GetAchievement", delegate
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement == null)
                {
                    achievement = new Achievement()
                    {
                        Name = pchName,
                        Date = DateTime.Now,
                        Earned = true
                    };
                    Achievements.Add(achievement);
                    SaveAchievements();
                    // TODO: Show Overlay with Achievement
                    Result = true;
                }
            });

            return Result;
        }

        public bool ClearAchievement(string pchName)
        {
            Write($"ClearAchievement {pchName}");
            MutexHelper.Wait("ClearAchievement", delegate
            {
                for (int i = 0; i < Achievements.Count; i++)
                {
                    var achievement = Achievements[i];
                    achievement.Earned = false;
                    achievement.Progress = 0;
                }
                SaveAchievements();
            });
            return true;
        }

        public bool GetAchievementAndUnlockTime(string pchName, ref bool pbAchieved, ref uint punUnlockTime)
        {
            Write($"GetAchievementAndUnlockTime {pchName}");
            var Result = false;
            var Archived = false;
            uint UnlockTime = 0;
            MutexHelper.Wait("GetAchievementAndUnlockTime", delegate
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement != null)
                {
                    Archived = achievement.Earned;
                    UnlockTime = (uint)(new DateTimeOffset(achievement.Date)).ToUnixTimeSeconds();
                    Result = true;
                }
            });
            pbAchieved = Archived;
            punUnlockTime = UnlockTime;
            return Result;
        }

        public bool StoreStats()
        {
            try
            {
                Write($"StoreStats");
                UserStatsStored_t data = new UserStatsStored_t()
                {
                    m_nGameID = SteamEmulator.GameID,
                    m_eResult = SKYNET.Types.EResult.k_EResultOK
                };
                CallbackManager.AddCallbackResult(data);
                return true;
            }
            catch (Exception ex)
            {
                Write($"StoreStats {ex}");
                return false;
            }
        }

        public int GetAchievementIcon(string pchName)
        {
            Write($"GetAchievementIcon");
            return 0;
        }

        public string GetAchievementDisplayAttribute(string pchName, string pchKey)
        {
            Write($"GetAchievementDisplayAttribute");
            return "";
        }

        public bool IndicateAchievementProgress(string pchName, uint nCurProgress, uint nMaxProgress)
        {
            Write($"IndicateAchievementProgress");
            var Result = false;
            var Archived = false;

            MutexHelper.Wait("IndicateAchievementProgress", delegate
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement != null)
                {
                    achievement.Progress = nCurProgress;
                    achievement.MaxProgress = nMaxProgress;
                    Archived = achievement.Earned;
                    Result = true;
                }
                SaveAchievements();
            });

            UserAchievementStored_t data = new UserAchievementStored_t()
            {
                m_nGameID = SteamEmulator.AppId,
                m_bGroupAchievement = false,
                m_rgchAchievementName = Encoding.UTF8.GetBytes(pchName),
                m_nCurProgress = Archived ? nCurProgress : 0,
                m_nMaxProgress = Archived ? nMaxProgress : 0
            };

            CallbackManager.AddCallbackResult(data);

            return Result;
        }

        public uint GetNumAchievements()
        {
            var achievements = (uint)Achievements.Count;
            Write($"GetNumAchievements {achievements}");
            return achievements;
        }

        public string GetAchievementName(uint iAchievement)
        {
            string achievementName = "";
            try
            {
                if (Achievements.Count <= iAchievement)
                    return "";
                achievementName = Achievements[(int)iAchievement].Name;
            }
            catch { }
            Write($"GetAchievementName {iAchievement} {achievementName}");
            return achievementName;
        }

        public SteamAPICall_t RequestUserStats(ulong steamIDUser)
        {
            try
            {
                Write($"RequestUserStats");
                UserStatsReceived_t data = new UserStatsReceived_t()
                {
                    m_nGameID = SteamEmulator.GameID,
                    m_eResult = EResult.k_EResultOK,
                    m_steamIDUser = steamIDUser
                };
                return CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"RequestUserStats {ex}");
            }
            return k_uAPICallInvalid;
        }

        public bool GetUserStat(ulong steamIDUser, string pchName, uint pData)
        {
            Write($"GetUserStat");
            return false;
        }

        public bool GetUserAchievement(ulong steamIDUser, string pchName, bool pbAchieved)
        {
            Write($"GetUserAchievement");
            bool Result = false;
            bool Archived = false;
            MutexHelper.Wait("GetUserAchievement", delegate
            {
                var achievement = Achievements.Find(a => a.Name == pchName);
                if (achievement != null)
                {
                    Archived = achievement.Earned;
                    pbAchieved = false;
                    Result = true;
                }
            });
            return Result;
        }

        public bool GetUserAchievementAndUnlockTime(ulong steamIDUser, string pchName, bool pbAchieved, uint punUnlockTime)
        {
            Write($"GetUserAchievementAndUnlockTime");
            return false;
        }

        public bool ResetAllStats(bool bAchievementsToo)
        {
            Write($"ResetAllStats");
            return false;
        }

        public SteamAPICall_t FindOrCreateLeaderboard(string pchLeaderboardName, int eLeaderboardSortMethod, int eLeaderboardDisplayType)
        {
            try
            {
                Write($"FindOrCreateLeaderboard");

                Leaderboard leaderboard = Leaderboards.Find( l => l.Name == pchLeaderboardName);

                if (leaderboard == null)
                {
                    leaderboard = new Leaderboard()
                    {
                        Name = pchLeaderboardName,
                        ShortMethod = (ELeaderboardSortMethod)eLeaderboardSortMethod,
                        DisplayType = (ELeaderboardDisplayType)eLeaderboardDisplayType
                    };
                    Leaderboards.Add(leaderboard);
                }

                //LeaderboardFindResult_t data = new LeaderboardFindResult_t()
                //{
                //    m_bLeaderboardFound = 1,
                //    m_hSteamLeaderboard = default
                //};

                //return CallbackManager.AddCallbackResult(data, LeaderboardFindResult_t.k_iCallback);
            }
            catch (Exception ex)
            {
                Write($"FindOrCreateLeaderboard {ex}");
            }
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t FindLeaderboard(string pchLeaderboardName)
        {
            try
            {
                Write($"FindOrCreateLeaderboard");

                Leaderboard leaderboard = Leaderboards.Find(l => l.Name == pchLeaderboardName);

                if (leaderboard == null)
                {
                    leaderboard = new Leaderboard()
                    {
                        Name = pchLeaderboardName,
                        ShortMethod = ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,
                        DisplayType = ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric
                    };
                    Leaderboards.Add(leaderboard);
                }

                //LeaderboardFindResult_t data = new LeaderboardFindResult_t()
                //{
                //    m_bLeaderboardFound = 1,
                //    m_hSteamLeaderboard = default
                //};

                //return CallbackManager.AddCallbackResult(data, LeaderboardFindResult_t.k_iCallback);
            }
            catch (Exception ex)
            {
                Write($"FindOrCreateLeaderboard {ex}");
            }
            return k_uAPICallInvalid;
        }

        public string GetLeaderboardName(ulong hSteamLeaderboard)
        {
            Write($"GetLeaderboardName");
            return "";
        }

        public int GetLeaderboardEntryCount(ulong hSteamLeaderboard)
        {
            Write($"GetLeaderboardEntryCount");
            return 0;
        }

        public int GetLeaderboardSortMethod(ulong hSteamLeaderboard)
        {
            Write($"GetLeaderboardSortMethod");
            return default;
        }

        public int GetLeaderboardDisplayType(ulong hSteamLeaderboard)
        {
            Write($"GetLeaderboardDisplayType");
            return 0;
        }

        public SteamAPICall_t DownloadLeaderboardEntries(ulong hSteamLeaderboard, IntPtr eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
        {
            Write($"DownloadLeaderboardEntries");
            // LeaderboardScoresDownloaded_t
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t DownloadLeaderboardEntriesForUsers(ulong hSteamLeaderboard, ulong prgUsers, int cUsers)
        {
            Write($"DownloadLeaderboardEntriesForUsers");
            // LeaderboardScoresDownloaded_t
            return k_uAPICallInvalid;
        }

        public bool GetDownloadedLeaderboardEntry(ulong hSteamLeaderboardEntries, int index, IntPtr pLeaderboardEntry, uint pDetails, int cDetailsMax)
        {
            Write($"GetDownloadedLeaderboardEntry");
            return false;
        }

        public SteamAPICall_t UploadLeaderboardScore(ulong hSteamLeaderboard, int eLeaderboardUploadScoreMethod, uint nScore, uint pScoreDetails, int cScoreDetailsCount)
        {
            Write($"UploadLeaderboardScore");
            // LeaderboardScoreUploaded_t
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t AttachLeaderboardUGC(ulong hSteamLeaderboard, ulong hUGC)
        {
            Write($"AttachLeaderboardUGC");
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t GetNumberOfCurrentPlayers()
        {
            try
            {
                NumberOfCurrentPlayers_t data = new NumberOfCurrentPlayers_t()
                {
                    m_bSuccess = 1,
                    m_cPlayers = SteamEmulator.SteamFriends.Users.Count
                };

                Write($"GetNumberOfCurrentPlayers {SteamEmulator.SteamFriends.Users.Count}");

                return CallbackManager.AddCallbackResult(data);
            }
            catch (Exception ex)
            {
                Write($"GetNumberOfCurrentPlayers {ex}");
            }
            return k_uAPICallInvalid;
        }

        public SteamAPICall_t RequestGlobalAchievementPercentages()
        {
            Write($"RequestGlobalAchievementPercentages");
            // GlobalAchievementPercentagesReady_t
            return k_uAPICallInvalid;
        }

        public int GetMostAchievedAchievementInfo(string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved)
        {
            Write($"GetMostAchievedAchievementInfo");
            return -1;
        }

        public int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, string pchName, uint unNameBufLen, float pflPercent, bool pbAchieved)
        {
            Write($"GetNextMostAchievedAchievementInfo");
            return -1;
        }

        public bool GetAchievementAchievedPercent(string pchName, float pflPercent)
        {
            Write($"GetAchievementAchievedPercent");
            return false;
        }

        public SteamAPICall_t RequestGlobalStats(int nHistoryDays)
        {
            try
            {
                Write($"RequestGlobalStats");
                //GlobalStatsReceived_t data = new GlobalStatsReceived_t()
                //{
                //    m_eResult = EResult.k_EResultOK,
                //    m_nGameID = SteamEmulator.GameID
                //};
                //return CallbackManager.AddCallbackResult(data, GlobalStatsReceived_t.k_iCallback);
            }
            catch (Exception ex)
            {
                Write($"RequestGlobalStats {ex}");
            }
            return k_uAPICallInvalid;
        }

        public bool GetGlobalStat(string pchStatName, uint pData)
        {
            Write($"GetGlobalStat {pchStatName}");
            return false;
        }

        public uint GetGlobalStatHistory(string pchStatName, uint pData, uint cubData)
        {
            Write($"GetGlobalStatHistory {pchStatName}");
            return 0;
        }

        public bool GetAchievementProgressLimits(string pchName, uint pnMinProgress, uint pnMaxProgress)
        {
            Write($"GetAchievementProgressLimits");
            return false;
        }

        private void SaveAchievements()
        {
            try
            {
                string achievementsPath = Path.Combine(modCommon.GetPath(), "SKYNET", "Storage", "Achievements.json");
                modCommon.EnsureDirectoryExists(achievementsPath, true);
                string json = Achievements.ToJson();
                File.WriteAllText(achievementsPath, json);
            }
            catch
            {
            }
        }

        private class Achievement
        {
            public string Name { get; set; }
            public bool Earned { get; set; }
            public DateTime Date { get; set; }
            public uint Progress { get; set; }
            public uint MaxProgress { get; set; }
        }

        private class Leaderboard
        {
            public string Name { get; set; }
            public ELeaderboardSortMethod ShortMethod { get; set; }
            public ELeaderboardDisplayType DisplayType { get; set; }
            public SteamLeaderboard_t SteamLeaderboard { get; set; }
        }
    }
}