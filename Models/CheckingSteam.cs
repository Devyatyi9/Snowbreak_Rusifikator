using Avalonia;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Snowbreak_Rusifikator.IConfigs;

namespace Snowbreak_Rusifikator
{
    [SupportedOSPlatform("windows")]
    internal class CheckingSteam
    {
        static string gamePath = "";
        public static async Task<string> Steam()
        {
            Trace.WriteLine("Проверка наличия игры через Steam");
            string? steamDir =
                (string)Microsoft.Win32.Registry.GetValue(keyName: @"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "Steampath", null); // Ищем папку стима через реестр
            if (string.IsNullOrEmpty(steamDir))
            {
                Trace.WriteLine("Проверка второго пути");
                steamDir = (string)Microsoft.Win32.Registry.GetValue(keyName: @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", null);
            }

            if (string.IsNullOrEmpty(steamDir))
            {
                Trace.WriteLine("Steam не установлен");
                //Далее проверка Standalone (лаунчер) версии игры, переход к return
            }
            else
            {
                await ParseSteamLibrary(steamDir);
            }

            return gamePath;
        }
        static Task ParseSteamLibrary(string steamDir)
        {
            Trace.WriteLine("Парсинг библиотеки Steam");
            string steamConfigPath = steamDir + @"\SteamApps\libraryfolders.vdf";
            string gameId = "2668080";
            string pattern = "\"apps\"\\s+{\\s+(?<line>\"(?<value>\\d+)\"\\s+\"\\d+\"\\s+)+\\s+}"; // "apps"\s+{\s+(?<line>"(?<value>\d+)"\s+"\d+"\s+)+\s+}
            string input = File.ReadAllText(steamConfigPath);

            MatchCollection steamAppsCollection = Regex.Matches(input, pattern, RegexOptions.Singleline);
            int groupIndex = -1;
            if (steamAppsCollection.Count > 0)
            {
                for (int i = 0; i < steamAppsCollection.Count; i++)
                {
                    //Debug.WriteLine(steamAppsCollection[i].Value);
                    //Debug.WriteLine("'{0}' found at index {1}.", steamAppsCollection[i].Value, steamAppsCollection[i].Index);
                    //Debug.WriteLine("steamAppsCollection.Group: '{0}' из '{1}'", (i + 1), steamAppsCollection.Count);
                    bool isGameExist = steamAppsCollection[i].Value.Contains('\"' + gameId + '\"');
                    if (isGameExist)
                    {
                        groupIndex = i;
                        //Debug.WriteLine("end FOR");
                        break;
                    }
                }
            } else { Trace.WriteLine("Steam версия не установлена"); }
            if (groupIndex != -1)
            {
                pattern = "^\\s+\"(?<key>path)\"\\s+\"(?<value>.+)\"$"; // ^\s+"(?<key>path)"\s+"(?<value>.+)"$
                steamAppsCollection = Regex.Matches(input, pattern, RegexOptions.Multiline);
                if (steamAppsCollection.Count > 0)
                {
                    // F:\\SteamLibrary
                    string libraryPath = steamAppsCollection[groupIndex].Groups[2].Value;
                    steamAppsCollection = null;
                    input = null;
                    string tempGamePath = libraryPath + Path.DirectorySeparatorChar + @"steamapps\common\SNOWBREAK";
                    tempGamePath = Path.GetFullPath(tempGamePath); // for normalize path
                    bool isDirExist = Directory.Exists(tempGamePath);
                    if (isDirExist)
                    {
                        gamePath = tempGamePath;
                        Trace.WriteLine("Установлена Steam версия игры");
                    }
                }
            }
            //foreach (Match m in steamAppsCollection)
            //{
            //    Debug.WriteLine("'{0}' found at index {1}.", m.Value, m.Index);
            //    Debug.WriteLine("indexes.Count: '{0}'", steamAppsCollection.Count);
            //    //m.Groups[1].Captures
            //}
            //.Cast<Match>().ToList()
            //.Select(m => m.Groups[2].Captures).ToList()
            //.SelectMany(c => c.Cast<Capture>())
            //.Select(c => c.Value).ToList();

            //indexes.ForEach(i => { });
            //List<string>

            //List<Match> indexes = Regex.Matches(input, pattern, RegexOptions.Singleline)
            //   .Cast<Match>().ToList();
            ////for (int i = 0; i < indexes.Count; i++) { indexes.Count; }
            //foreach (Match match in indexes)
            //{
            //    indexes.Select(m => m.Groups[2].Captures).ToList()
            //        .SelectMany(c => c.Cast<Capture>())
            //        .Select(c => c.Value).ToList();
            //}
            //.Select(m => m.Groups[2].Captures).ToList()
            //.SelectMany(c => c.Cast<Capture>())
            //.Select(c => c.Value).ToList();

            //foreach (string s in indexes)
            //{
            //    Debug.WriteLine(s);
            //}

            //
            //

            //var testData = Gameloop.Vdf.VdfReader(steamConfig);
            //dynamic volvo = VdfConvert.Deserialize(File.ReadAllText(steamConfig));
            //Console.WriteLine(volvo.GetType("1"));
            //Console.WriteLine(volvo.GetType("libraryfolders"));
            //volvo.GetType(1);
            return Task.CompletedTask;
        }
    }
}
