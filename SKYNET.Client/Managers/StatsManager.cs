using SKYNET.Helper;
using SKYNET.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace SKYNET.Managers
{
    public class StatsManager
    {
        private static ConcurrentDictionary<uint, List<Leaderboard>> Leaderboards;
        private static ConcurrentDictionary<uint, List<Achievement>> Achievements;
        private static ConcurrentDictionary<uint, List<PlayerStat>> PlayerStats;

        static StatsManager()
        {
            Leaderboards = new ConcurrentDictionary<uint, List<Leaderboard>>();
            Achievements = new ConcurrentDictionary<uint, List<Achievement>>();
            PlayerStats = new ConcurrentDictionary<uint, List<PlayerStat>>();
        }

        public static void Initialize()
        {
            try
            {
                string StoragePath = Path.Combine(modCommon.GetPath(), "Data", "Storage");
                foreach (var directory in Directory.GetDirectories(StoragePath))
                {
                    var appID = new DirectoryInfo(directory).Name;
                    foreach (var file in Directory.GetFiles(directory))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        switch (fileName)
                        {
                            case "Achievements":
                            {
                                if (uint.TryParse(appID, out var AppID))
                                {
                                    string fileContent = File.ReadAllText(file);
                                    var achievements = new JavaScriptSerializer().Deserialize<List<Achievement>>(fileContent);
                                    Achievements.TryAdd(AppID, achievements);
                                }
                            }
                            break;
                            case "Leaderboards":
                            {
                                if (uint.TryParse(appID, out var AppID))
                                {
                                    string fileContent = File.ReadAllText(file);
                                    var leaderboards = new JavaScriptSerializer().Deserialize<List<Leaderboard>>(fileContent);
                                    Leaderboards.TryAdd(AppID, leaderboards);
                                }
                            }
                            break;
                            case "PlayerStats":
                            {
                                if (uint.TryParse(appID, out var AppID))
                                {
                                    string fileContent = File.ReadAllText(file);
                                    var playerStats = new JavaScriptSerializer().Deserialize<List<PlayerStat>>(fileContent);
                                    PlayerStats.TryAdd(AppID, playerStats);
                                }
                            }
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        internal static void SetAchievement(uint appID, Achievement achievement)
        {
            MutexHelper.Wait("Achievements", delegate
            {
                if (Achievements.TryGetValue(appID, out var achievements))
                {
                    achievements.Add(achievement);
                }
                else
                {
                    Achievements.TryAdd(appID, new List<Achievement>() { achievement });
                }
            });

            SaveAchievements();
        }

        internal static void SetLeaderboard(uint appID, Leaderboard leaderboard)
        {
            MutexHelper.Wait("Leaderboards", delegate
            {
                if (Leaderboards.TryGetValue(appID, out var leaderboards))
                {
                    leaderboards.Add(leaderboard);
                }
                else
                {
                    Leaderboards.TryAdd(appID, new List<Leaderboard>() { leaderboard });
                }
            });

            SaveLeaderboards();
        }

        internal static void SetPlayerStat(uint appID, PlayerStat playerStat)
        {
            MutexHelper.Wait("PlayerStats", delegate
            {
                if (PlayerStats.TryGetValue(appID, out var achievements))
                {
                    achievements.Add(playerStat);
                }
                else
                {
                    PlayerStats.TryAdd(appID, new List<PlayerStat>() { playerStat });
                }
            });

            SavePlayerStats();
        }

        private static void SaveAchievements()
        {
            MutexHelper.Wait("Achievements", delegate
            {
                foreach (var KV in Achievements)
                {
                    try
                    {
                        var AppID = KV.Key.ToString();
                        var achievements = KV.Value;
                        var filePath = Path.Combine(modCommon.GetPath(), "Data", "Storage", AppID, "Achievements.json");
                        modCommon.EnsureDirectoryExists(filePath, true);
                        var JSON = new JavaScriptSerializer().Serialize(achievements);
                        File.WriteAllText(filePath, JSON);
                    }
                    catch
                    {
                    }
                }
            });
        }

        private static void SaveLeaderboards()
        {
            MutexHelper.Wait("Leaderboards", delegate
            {
                foreach (var KV in Leaderboards)
                {
                    try
                    {
                        var AppID = KV.Key.ToString();
                        var leaderboards = KV.Value;
                        var filePath = Path.Combine(modCommon.GetPath(), "Data", "Storage", AppID, "Leaderboards.json");
                        modCommon.EnsureDirectoryExists(filePath, true);
                        var JSON = new JavaScriptSerializer().Serialize(leaderboards);
                        File.WriteAllText(filePath, JSON);
                    }
                    catch
                    {
                    }
                }
            });
        }

        private static void SavePlayerStats()
        {
            MutexHelper.Wait("PlayerStats", delegate
            {
                foreach (var KV in PlayerStats)
                {
                    try
                    {
                        var AppID = KV.Key.ToString();
                        var playerStats = KV.Value;
                        var filePath = Path.Combine(modCommon.GetPath(), "Data", "Storage", AppID, "PlayerStats.json");
                        modCommon.EnsureDirectoryExists(filePath, true);
                        var JSON = new JavaScriptSerializer().Serialize(playerStats);
                        File.WriteAllText(filePath, JSON);
                    }
                    catch
                    {
                    }
                }
            });
        }

        public static List<Achievement> GetAchievements(uint appID)
        {
            if (Achievements.TryGetValue(appID, out var achievements))
            {
                return achievements;
            }
            return new List<Achievement>();
        }

        public static List<Leaderboard> GetLeaderboards(uint appID)
        {
            if (Leaderboards.TryGetValue(appID, out var leaderboards))
            {
                return leaderboards;
            }
            return new List<Leaderboard>();
        }

        public static List<PlayerStat> GetPlayerStats(uint appID)
        {
            if (PlayerStats.TryGetValue(appID, out var playerStats))
            {
                return playerStats;
            }
            return new List<PlayerStat>();
        }

        public static void UpdateAchievement(uint appID, Achievement achievement)
        {
            MutexHelper.Wait("Achievements", delegate
            {
                if (Achievements.TryGetValue(appID, out var achievements))
                {
                    var toUpdate = achievements.Find(a => a.Name == achievement.Name);
                    if (toUpdate != null)
                    {
                        toUpdate.Date = achievement.Date;
                        toUpdate.Earned = achievement.Earned;
                        toUpdate.MaxProgress = achievement.MaxProgress;
                        toUpdate.Name = achievement.Name;
                        toUpdate.Progress = achievement.Progress;
                    }
                }
                else
                {
                    Achievements.TryAdd(appID, new List<Achievement>() { achievement });
                }
            });

            SaveAchievements();
        }

        public static void ResetAllStats(uint appID, bool achievementsToo)
        {
            if (PlayerStats.TryGetValue(appID, out var playerStats))
            {
                playerStats.Clear();
            }
            if (achievementsToo)
            {
                if (Achievements.TryGetValue(appID, out var achievements))
                {
                    achievements.Clear();
                }
            }
        }


        #region Generate Data online

        public static void GenerateAchievements(uint app_id, string steam_apikey)
        {
            string URL = $"https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key={steam_apikey}&appid={app_id}";
            string achievementsPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", app_id.ToString(), "Achievements.json");
            modCommon.EnsureDirectoryExists(achievementsPath, true);

            try
            {
                WebRequest webrequest = HttpWebRequest.Create(URL);
                webrequest.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                File.WriteAllText(achievementsPath, content);
            }
            catch
            {

            }
        }

        public static void GenerateItems(uint app_id)
        {
            //string URL = $"https://api.steampowered.com/IInventoryService/GetItemDefMeta/v1?key={steam_apikey}&appid={app_id}";
            string URL = $"https://api.steampowered.com/IGameInventory/GetItemDefArchive/v0001?appid={app_id}";
            string achievementsPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", app_id.ToString(), "Items.json");
            modCommon.EnsureDirectoryExists(achievementsPath, true);

            try
            {
                WebRequest webrequest = HttpWebRequest.Create(URL);
                webrequest.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                File.WriteAllText(achievementsPath, content);
            }
            catch
            {

            }
        }

        public static void GenerateDLCs(uint app_id)
        {
            string URL = $"https://store.steampowered.com/api/appdetails/?appids={app_id}";
            string achievementsPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", app_id.ToString(), "DLCs.json");
            modCommon.EnsureDirectoryExists(achievementsPath, true);

            try
            {
                WebRequest webrequest = HttpWebRequest.Create(URL);
                webrequest.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                File.WriteAllText(achievementsPath, content);
            }
            catch
            {

            }
        }

        public static void GenerateAppDetails(uint app_id)
        {
            string URL = $"https://store.steampowered.com/api/appdetails/?appids={app_id}";
            string achievementsPath = Path.Combine(modCommon.GetPath(), "Data", "Storage", app_id.ToString(), "AppDetails.json");
            modCommon.EnsureDirectoryExists(achievementsPath, true);

            try
            {
                WebRequest webrequest = HttpWebRequest.Create(URL);
                webrequest.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string content = reader.ReadToEnd();
                File.WriteAllText(achievementsPath, content);
            }
            catch
            {

            }
        }
        #endregion
    }
}
