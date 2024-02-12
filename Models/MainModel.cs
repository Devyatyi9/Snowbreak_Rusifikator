using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Snowbreak_Rusifikator.IConfigs;
using System.Runtime.Versioning;
using Avalonia.Platform.Storage;
using Snowbreak_Rusifikator.ViewModels;

namespace Snowbreak_Rusifikator.Models
{
    internal class MainModel
    {
        static public bool isTester { get; set; }
        static protected string programConfigPath;
        static public ProgramConfig programConfig;
        static HttpClient Client = new();

        [SupportedOSPlatform("windows")]
        public static async void StartUpdate()
        {
            await AutoDetectingGamePathAsync(programConfig, programConfigPath);
        }

        public static async Task BaseProgramConfig() 
        {
            // https://learn.microsoft.com/ru-ru/dotnet/api/system.environment.specialfolder
            //string localAppdata = Environment.GetEnvironmentVariable("LocalAppdata");
            string programConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Snowbreak_rusificator\config.json";
            IConfigs.ProgramConfig programConfig = await CreateLoadProgramConfig(programConfigPath);
            MainModel.programConfigPath = programConfigPath;
            MainModel.programConfig = programConfig;
        }

        public static async Task ChangeTesterState() 
        {
            programConfig.isTester = MainModel.isTester;
            await SaveProgramConfig(programConfig, programConfigPath);
        }

        public static void RunLauncher() 
        {
            if (programConfig.launcherPath != "") 
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(programConfig.launcherPath);
                Process.Start(startInfo);
            }
        }

        public static async Task GetGameFolder(IReadOnlyList<IStorageFolder> folder)
        {
            programConfig.gamePath = folder[0].TryGetLocalPath();
            await SaveProgramConfig(programConfig, programConfigPath);
        }

        static async Task AutoDetectingGamePathAsync(ProgramConfig programConfig, string programConfigPath)
        {
            // "C:\\Program Files\\Snow\\data"
            // "F:\\Games\\Snow\\the_game"
            if (programConfig.gamePath == "")
            {
                // StandaloneGame (launcher version) чекинг
                CheckGamePath(programConfig);
            }
            else if (programConfig.gamePath == "")
            {
                // Проверка наличия игры через Steam
                programConfig.gamePath = CheckingSteam.Steam();
            }
            else if (programConfig.gamePath != "")
            {
                await SaveProgramConfig(programConfig, programConfigPath);
                await RunCheckUpdatesAsync(programConfig, programConfigPath);
            }
        }

        static async Task RunCheckUpdatesAsync(ProgramConfig programConfig, string programConfigPath)
        {
            // Проверка tester branch
            string repoLink = ProgramVariables.gitHubRepoLink;
            if (programConfig.isTester == true)
            {
                repoLink = ProgramVariables.gitHubRepoLink + ProgramVariables.gitHubTesterBranch;
            }

            // Проверка обновлений  // установка в \game\Game\Content\Paks\~mods
            HttpClient client = UseHttpClient(Client);
            List<RepositoryFile> repositoryFiles = await ProcessRepositoriesAsync(client, repoLink, ProgramVariables.gitHubToken);
            // Сортировка
            repositoryFiles = SortingRepositoryFiles(repositoryFiles);
            // Проверка sha: если запись пуста или она отличается
            if ((programConfig.sha == "") || (programConfig.sha != repositoryFiles[0].Sha))
            {
                // Функция скачивания и сохранения
                await DownloadAndSaveFileAsync(repositoryFiles, programConfig, client, programConfigPath);
            }
            else { Trace.WriteLine("Нет обновлений."); }
        }

        static async Task<ProgramConfig> CreateLoadProgramConfig(string programConfigPath)
        {
            FileInfo programConfigInfo = new(Path.GetFullPath(programConfigPath));
            Debug.Write("Config path: ");
            Debug.WriteLine(Path.GetFullPath(programConfigPath));
            IConfigs.ProgramConfig programConfig = new()
            {
                gamePath = "",
                launcherPath = "",
                fileName = "",
                sha = "",
                isTester = false
            };
            try
            {
                if (programConfigInfo.Exists && programConfigInfo.Length > 70)
                {
                    // Чтение конфига
                    string jsonText = File.ReadAllText(programConfigPath);
                    IConfigs.ProgramConfig? jsonContent = JsonSerializer.Deserialize<IConfigs.ProgramConfig>(jsonText);
                    Trace.WriteLine(value: $"Config game path: {jsonContent.gamePath}");
                    programConfig = jsonContent;
                    MainModel.isTester = programConfig.isTester;
                }
                else
                {
                    // Создание пустого конфига
                    _ = Directory.CreateDirectory(path: Path.GetDirectoryName(programConfigPath));
                    using (StreamWriter createFile = File.CreateText(programConfigPath))
                    {
                        createFile.Close();
                    }
                    await SaveProgramConfig(programConfig, programConfigPath);
                    //var fs3 = FileSystem.Dir(ProgramConfigPath);
                    //programConfigInfo.CreateText("test.txt");
                    //programConfigInfo.CreateText().WriteLine();
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("The process failed: {0}", e.ToString());
            }
            finally
            {
                Trace.WriteLine("Конфиг загружен.");
            }
            return programConfig;
        }

        static void CheckGamePath(IConfigs.ProgramConfig programConfig)
        {
            Trace.WriteLine("Проверка наличия игры через лаунчер");
            string tempGameDir = (string)Microsoft.Win32.Registry.GetValue(keyName: @"HKEY_LOCAL_MACHINE\SOFTWARE\ProjectSnow", "InstPath", null);
            if (tempGameDir != null)
            {
                string jsonPath = tempGameDir + Path.DirectorySeparatorChar + "preference.json";
                string jsonText = File.ReadAllText(jsonPath);
                JsonGamePreference? jsonContent = JsonSerializer.Deserialize<JsonGamePreference>(jsonText);
                Trace.WriteLine(value: $"Game path: {jsonContent.dataPath}");
                programConfig.gamePath = jsonContent.dataPath;
                programConfig.launcherPath = tempGameDir + Path.DirectorySeparatorChar + "snow_launcher.exe";
            }
        }

        static Task SaveProgramConfig(object programConfig, string programConfigPath)
        {
            string jsonString = JsonSerializer.Serialize(programConfig);
            File.WriteAllText(programConfigPath, jsonString);
            Trace.WriteLine("Settings has been saved.");
            return Task.CompletedTask;
        }

        static HttpClient UseHttpClient(HttpClient Client)
        {
            // GitHub REST API limit is 60 requests per hour for unauthenticated requests
            // User Access Token Requests are limited to 5,000 requests per hour
            // https://docs.github.com/en/rest/using-the-rest-api/rate-limits-for-the-rest-api
            //HttpClient client = new();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            Client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            return Client;
        }

        // https://learn.microsoft.com/dotnet/fundamentals/networking/http/httpclient-guidelines#recommended-use
        static async Task<List<RepositoryFile>> ProcessRepositoriesAsync(HttpClient client, string urlLink, string token)
        {
            //programConfig.fileName;
            //programConfig.sha;
            //await using Stream stream = await client.GetStreamAsync(urlLink);
            using HttpResponseMessage response = await client.GetAsync(urlLink);
            List<RepositoryFile> repositoryFiles;
            if (response.IsSuccessStatusCode)
            {
                repositoryFiles = await JsonSerializer.DeserializeAsync<List<RepositoryFile>>(response.Content.ReadAsStream());
            }
            else
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", parameter: token);
                using HttpResponseMessage secondResponse = await client.GetAsync(urlLink);
                repositoryFiles = await JsonSerializer.DeserializeAsync<List<RepositoryFile>>(secondResponse.Content.ReadAsStream());
            }

            if (repositoryFiles == null)
            {
                throw new Exception();
            }

            return repositoryFiles;
            //return repositoryFiles ?? new();
            //var json = await client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");
            //Console.Write(json);
            //var repositoryFiles = await ProcessRepositoriesAsync(client);
            //foreach (var repo in repositoryFiles)
            //    Console.Write(repo.Name);
        }

        static List<RepositoryFile> SortingRepositoryFiles(List<RepositoryFile> oldFileList)
        {
            List<RepositoryFile> newFileList = [];
            foreach (RepositoryFile element in oldFileList)
            {
                string fileExt = Path.GetExtension(element.Name);
                if (fileExt == ".pak")
                {
                    newFileList.Add(element);
                }
            }
            return newFileList;
        }

        static async Task DownloadAndSaveFileAsync(List<RepositoryFile> fileList, IConfigs.ProgramConfig programConfig, HttpClient client, string programConfigPath)
        {
            string savePath = programConfig.gamePath + Path.DirectorySeparatorChar + "game\\Game\\Content\\Paks\\~mods\\" + fileList[0].Name;
            _ = Directory.CreateDirectory(path: Path.GetDirectoryName(savePath));
            using Stream s = await client.GetStreamAsync(fileList[0].DownloadUrl);
            using FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate);
            await s.CopyToAsync(fs);
            programConfig.fileName = fileList[0].Name;
            programConfig.sha = fileList[0].Sha;
            await SaveProgramConfig(programConfig, programConfigPath);
            Trace.WriteLine("File saved.");
        }
        static Task RemoveFile(IConfigs.ProgramConfig programConfig, string programConfigPath)
        {
            string filePath = programConfig.gamePath + Path.DirectorySeparatorChar + "game\\Game\\Content\\Paks\\~mods\\" + programConfig.fileName;
            File.Delete(filePath);
            programConfig.fileName = "";
            programConfig.sha = "";
            Trace.WriteLine("File removed.");
            SaveProgramConfig(programConfig, programConfigPath);
            return Task.CompletedTask;
        }
    }
}
