using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace Snowbreak_Rusifikator
{
    [SupportedOSPlatform("windows")]
    internal class CheckingSteam
    {
        public static string Steam()
        {
            // Заменить Console на Trace
            Console.WriteLine("Проверка наличия игры через Steam");
            string gamePath = "";
            string? steamDir =
                (string)Microsoft.Win32.Registry.GetValue(keyName: @"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "Steampath", null); // Ищем папку стима через реестр
            if (string.IsNullOrEmpty(steamDir))
            {
                Console.WriteLine("Проверка второго пути");
                steamDir = (string)Microsoft.Win32.Registry.GetValue(keyName: @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", null);
            }

            if (string.IsNullOrEmpty(steamDir))
            {
                Console.WriteLine("Steam версия не установлена");
                //Далее проверка Standalone (лаунчер) версии игры, переход к return
            }
            else
            {
                //gamePath = steamDir;
                //Console.WriteLine("Установлена Steam версия игры");
                string steamConfig = steamDir + @"\SteamApps\libraryfolders.vdf";
                //Конвертация vdf в json
                //


                //
                //

                //var testData = Gameloop.Vdf.VdfReader(steamConfig);
                //dynamic volvo = VdfConvert.Deserialize(File.ReadAllText(steamConfig));
                //Console.WriteLine(volvo.GetType("1"));
                //Console.WriteLine(volvo.GetType("libraryfolders"));
                //volvo.GetType(1);
            }

            return gamePath;
        }
    }
}
