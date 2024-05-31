using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
using Avalonia.Data;
using System.Text.Json.Serialization;
using static Snowbreak_Rusifikator.Models.RepositoryFile;
using System.Data;
using System.Threading;
using System.Xml.Linq;

namespace Snowbreak_Rusifikator.Models
{
    internal class MainModel
    {
        static public bool isTester { get; set; }
        static protected string? programConfigPath;
        static public ProgramConfig? programConfig;
        static public string programStatus { get; set; } = string.Empty;
        static readonly HttpClient Client = new();

        [SupportedOSPlatform("windows")]
        public static async Task<Task> StartUpdate(CancellationToken cancellationToken = default)
        {
            await RunCheckUpdatesAsync(programConfig, programConfigPath, cancellationToken);
            //OperationCanceledException
            return Task.CompletedTask;
        }
        
        public static async Task<Task> BaseProgramConfig() 
        {
            // https://learn.microsoft.com/ru-ru/dotnet/api/system.environment.specialfolder
            string programConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Snowbreak_rusificator\config.json";
            IConfigs.ProgramConfig programConfig = await CreateLoadProgramConfig(programConfigPath);
            MainModel.programConfigPath = programConfigPath;
            MainModel.programConfig = programConfig;
            if (programConfig.gamePath.Length == 0)
            {
                Task? localTask = await AutoDetectingGamePathAsync(programConfig, programConfigPath);
                if (localTask.IsCompletedSuccessfully)
                {
                    MainModel.programConfig = programConfig;
                }
            }
            return Task.CompletedTask;
        }

        public static async Task<Task> ChangeTesterState() 
        {
            programConfig.isTester = MainModel.isTester;
            await SaveProgramConfig(programConfig, programConfigPath);
            return Task.CompletedTask;
        }

        public static void RunLauncher() 
        {
            if (programConfig.launcherPath != "") 
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(programConfig.launcherPath);
                Process.Start(startInfo);
            }
        }

        public static async Task<Task> GetGameFolder(IReadOnlyList<IStorageFolder> folder)
        {
            programConfig.gamePath = folder[0].TryGetLocalPath();
            string gamePathDataCheck = programConfig.gamePath + Path.DirectorySeparatorChar + "data";
            bool isDirExist = Directory.Exists(gamePathDataCheck);
            if (isDirExist)
            {
                programConfig.gamePath = gamePathDataCheck;
            }
            programConfig.fileName = "";
            programConfig.sha = "";
            programConfig.launcherPath = "";
            await SaveProgramConfig(programConfig, programConfigPath);
            return Task.CompletedTask;
        }

        public static async Task<Task> RemoveFile()
        {
            bool steam = await CheckFolderStructure();
            await InternalRemoveFile(programConfig, programConfigPath, steam);
            return Task.CompletedTask;
        }

        static async Task<Task> AutoDetectingGamePathAsync(ProgramConfig programConfig, string programConfigPath)
        {
            // "C:\\Program Files\\Snow\\data"
            // "F:\\Games\\Snow\\the_game"
            if (programConfig.gamePath == "")
            {
                // StandaloneGame (launcher version) чекинг
                CheckGamePath(programConfig);
            }
            if (programConfig.gamePath == "")
            {
                // Проверка наличия игры через Steam
                programConfig.gamePath = await CheckingSteam.Steam();
            }
            if (programConfig.gamePath != "")
            {
                await SaveProgramConfig(programConfig, programConfigPath);
            }
            return Task.CompletedTask;
        }

        static async Task RunCheckUpdatesAsync(ProgramConfig programConfig, string programConfigPath, CancellationToken cancellationToken)
        {
            Debug.WriteLine("MainModel.isTester: " + MainModel.isTester);
            Debug.WriteLine("programConfig.isTester: " + programConfig.isTester);

            // Проверка обновлений  // установка в \game\Game\Content\Paks\~mods
            HttpClient client = UseHttpClient(Client);
            List<RepositoryFile> repositoryFiles = await ProcessRepositoriesAsync(client, programConfig.isTester, cancellationToken);
            // Сортировка
            repositoryFiles = SortingRepositoryFiles(repositoryFiles); // сделать выдачу статуса "Репозиторий пуст" и завершать функцию
            if (repositoryFiles.Count > 0)
            {
                // Проверка sha: если запись пуста или она отличается
                if ((programConfig.sha == "") || (programConfig.sha != repositoryFiles[0].Sha))
                {
                    bool steam = await CheckFolderStructure();
                    // Функция скачивания и сохранения
                    await DownloadAndSaveFileAsync(repositoryFiles, programConfig, client, programConfigPath, steam);
                }
                else { Trace.WriteLine("Нет обновлений."); programStatus = "Нет обновлений."; }
            } else { Trace.WriteLine("FileList Empty!"); }
        }

        static async Task InstallUpdate() { }

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
                if (programConfigInfo.Exists && programConfigInfo.Length > 120)
                {
                    JsonSerializerOptions options = new()
                    {
                        TypeInfoResolver = ConfigContext.Default
                    };
                    // Чтение конфига
                    string jsonText = File.ReadAllText(programConfigPath);
                    IConfigs.ProgramConfig? jsonContent = JsonSerializer.Deserialize<IConfigs.ProgramConfig>(jsonText, options);
                    Trace.WriteLine(value: $"Config game path: {jsonContent.gamePath}");
                    programConfig = jsonContent;
                    MainModel.isTester = programConfig.isTester;
                    // вызов функции проверки наличия файла и папки
                    programConfig = CheckIsFileExist(programConfig);
                    await SaveProgramConfig(programConfig, programConfigPath);
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

        static ProgramConfig CheckIsFileExist(ProgramConfig programConfig) 
        {
            string filePath = programConfig.gamePath + Path.DirectorySeparatorChar + "game\\Game\\Content\\Paks\\~mods\\" + programConfig.fileName;
            bool isExist = File.Exists(filePath);
            if (!isExist) 
            {
                programConfig.fileName = "";
                programConfig.sha = "";
            }
            bool isDirExist = Directory.Exists(programConfig.gamePath);
            if (!isDirExist) 
            {
                programConfig.gamePath = "";
                programConfig.launcherPath = "";
            }
            return programConfig;
        }

        static void CheckGamePath(IConfigs.ProgramConfig programConfig)
        {
            Trace.WriteLine("Проверка наличия игры через лаунчер");
            string tempGameDir = (string)Microsoft.Win32.Registry.GetValue(keyName: @"HKEY_LOCAL_MACHINE\SOFTWARE\ProjectSnow", "InstPath", null);
            if (tempGameDir != null)
            {
                JsonSerializerOptions options = new()
                {
                    TypeInfoResolver = ConfigContext.Default
                };
                string jsonPath = tempGameDir + Path.DirectorySeparatorChar + "preference.json";
                if (File.Exists(jsonPath) && new FileInfo(jsonPath).Length > 0)
                {
                    string jsonText = File.ReadAllText(jsonPath);
                    JsonGamePreference? jsonContent = JsonSerializer.Deserialize<JsonGamePreference>(jsonText, options);
                    Trace.WriteLine(value: $"Game path: {jsonContent.dataPath}");
                    programConfig.gamePath = jsonContent.dataPath;
                    programConfig.launcherPath = tempGameDir + Path.DirectorySeparatorChar + "snow_launcher.exe";
                    if (!File.Exists(programConfig.launcherPath)) { programConfig.launcherPath = ""; }
                    MainModel.programConfig = programConfig;
                }
            }
        }

        static Task SaveProgramConfig(ProgramConfig programConfig, string programConfigPath)
        {
            MainModel.programConfig = programConfig;
            string jsonString = JsonSerializer.Serialize(programConfig, ConfigContext.Default.ProgramConfig);
            File.WriteAllText(programConfigPath, jsonString);
            Trace.WriteLine("Settings has been saved.");
            programStatus = "Настройки сохранены.";

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
        static async Task<List<RepositoryFile>> ProcessRepositoriesAsync(HttpClient client, bool isTester, CancellationToken cancellationToken)
        {
            List<RepositoryFile> repositoryFiles = [];
            Random rnd = new();
            List<string> listLinks = [.. ProgramVariables.arrayLinks];

            JsonSerializerOptions gitLabOptions = new()
            {
                TypeInfoResolver = RepositoryFileGitLabContext.Default
            };
            // добавить проверку на наличие интернет соединения, и завершать функцию со статусом "Нет соединения"
            JsonSerializerOptions options = new()
            {
                TypeInfoResolver = RepositoryFileContext.Default
            };
            try
            {
                //listLinks.Clear();
                //listLinks[0] = "http://localhost:443/test";
                while ((repositoryFiles == null || repositoryFiles.Count == 0) || listLinks.Count > 0) {
                    int rndValue = rnd.Next(listLinks.Count);
                    string urlLink = listLinks[rndValue];
                    listLinks.RemoveAt(rndValue);
                    if (isTester == true)
                    {
                        urlLink += ProgramVariables.testerBranch;
                    }
                    using HttpResponseMessage response = await client.GetAsync(urlLink);
                if (response.IsSuccessStatusCode)
                {
                    if (urlLink.Contains("gitlab"))
                    {
                            List<RepositoryFileGitLab> repositoryFilesGitLab = await JsonSerializer.DeserializeAsync<List<RepositoryFileGitLab>>(response.Content.ReadAsStream(), gitLabOptions);
                            foreach (RepositoryFileGitLab repositoryFile in repositoryFilesGitLab)
                            {
                                // gitLabRepoLink (urlLink)
                                // tree/
                                //urlLink = urlLink.Split(["tree/"])
                                string strlLink = "https://gitlab.com/api/v4/projects/55335200/repository/files/" + repositoryFile.Path + "/raw";
                                if (isTester) { strlLink += ProgramVariables.testerBranch; }
                                Uri uriLink = new(strlLink);
                                
                                Debug.WriteLine("request: " + uriLink);
                                RepositoryFile repositoryObject = new()
                                {
                                    Name = repositoryFile.Name,
                                    Sha = repositoryFile.Id,
                                    DownloadUrl = uriLink
                                };
                                repositoryFiles.Add(repositoryObject);
                            }
                            if (repositoryFiles.Count > 0) { 
                                foreach (var repositoryFile in repositoryFiles) { Debug.WriteLine("Link: " + repositoryFile.DownloadUrl); }
                                break; }
                    } else {
                            Debug.WriteLine("request: " + urlLink);
                        repositoryFiles = await JsonSerializer.DeserializeAsync<List<RepositoryFile>>(response.Content.ReadAsStream(), options);
                            if (repositoryFiles.Count > 0) {
                                foreach (var repositoryFile in repositoryFiles) { Debug.WriteLine("Link: " + repositoryFile.DownloadUrl); }
                                break; }
                        }
                }
                if (!response.IsSuccessStatusCode)
                {
                    Trace.WriteLine("Error response!");
                    Trace.WriteLine("Status code: " + response.StatusCode);
                    Trace.WriteLine("At: " + urlLink);
                    Trace.WriteLine("Content: " + response.Content);
                    programStatus = $"Error response! ${response.StatusCode}";
                }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("\nException Caught!");
                Trace.WriteLine("Message :{0} ", e.Message);
                programStatus = $"Message :${e.Message}";
                repositoryFiles = [];
            }
            return repositoryFiles;
        }

        static List<RepositoryFile> SortingRepositoryFiles(List<RepositoryFile> oldFileList)
        {
            List<RepositoryFile> newFileList = [];
            if (oldFileList.Count > 0)
            {
                foreach (RepositoryFile element in oldFileList)
                {
                    string fileExt = Path.GetExtension(element.Name);
                    if (fileExt == ".pak")
                    {
                        newFileList.Add(element);
                        //element.DownloadUrl
                        if (element.DownloadUrl != null)
                        {
                            Debug.WriteLine(element.DownloadUrl);
                        }
                    }
                }
            }
            return newFileList;
        }

        static async Task<bool> CheckFolderStructure()
        {
            // проверка на Steam-версию игры
            // "game\\Game\\Binaries\\Win64\\Game.exe"
            bool value = false;
            string nonSteamEngineDirPath = programConfig.gamePath + Path.DirectorySeparatorChar + "game\\Engine";
            string steamEngineDirPath = programConfig.gamePath + Path.DirectorySeparatorChar + "Engine";
            if (Directory.Exists(nonSteamEngineDirPath))
            { value = false; }
            else if (Directory.Exists(steamEngineDirPath))
            { value = true; }
            return value;
        }

        static async Task DownloadAndSaveFileAsync(List<RepositoryFile> fileList, IConfigs.ProgramConfig programConfig, HttpClient client, string programConfigPath, bool steam = false)
        {
            string savePath;
            if (!steam)
            {
                savePath = programConfig.gamePath + Path.DirectorySeparatorChar + "game\\Game\\Content\\Paks\\~mods\\" + fileList[0].Name;
            } else 
            {
                savePath = programConfig.gamePath + Path.DirectorySeparatorChar + "Game\\Content\\Paks\\~mods\\" + fileList[0].Name;
            }
            try
            {
                _ = Directory.CreateDirectory(path: Path.GetDirectoryName(savePath));
                using Stream s = await client.GetStreamAsync(fileList[0].DownloadUrl);
                using FileStream fs = new(savePath, FileMode.Create);
                await s.CopyToAsync(fs);
                Trace.WriteLine("Файл загружен и сохранён.");
                programStatus = "Файл загружен и сохранён.";
            }
            catch(Exception e)
            {
                Trace.WriteLine("\nException Caught!");
                Trace.WriteLine("Message :{0} ", e.Message);
                programStatus = $"Message :${e.Message}";
            }
            if (File.Exists(savePath))
            {
                programConfig.fileName = fileList[0].Name;
                programConfig.sha = fileList[0].Sha;
                await SaveProgramConfig(programConfig, programConfigPath);
            }
            fileList.Clear();
            savePath = string.Empty;
        }
        static Task InternalRemoveFile(IConfigs.ProgramConfig programConfig, string programConfigPath, bool steam = false)
        {
            string filePath;
            if (!steam)
            {
                filePath = programConfig.gamePath + Path.DirectorySeparatorChar + "game\\Game\\Content\\Paks\\~mods\\" + programConfig.fileName;
            }
            else
            {
                filePath = programConfig.gamePath + Path.DirectorySeparatorChar + "Game\\Content\\Paks\\~mods\\" + programConfig.fileName;
            }
            File.Delete(filePath);
            programConfig.fileName = "";
            programConfig.sha = "";
            Trace.WriteLine("Файл удалён.");
            programStatus = "Файл удалён.";
            SaveProgramConfig(programConfig, programConfigPath);
            return Task.CompletedTask;
        }
    }
}
