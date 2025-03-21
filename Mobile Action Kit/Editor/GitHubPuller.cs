using UnityEditor;
using UnityEngine;
using System.IO;
using System.Net;
using Unity.Plastic.Newtonsoft.Json;
using System.Collections.Generic;

public class GitHubPuller : EditorWindow
{
    private string repoOwner = GitHubConfig.RepositoryOwner;
    private string repoName = GitHubConfig.RepositoryName;

    private string latestVersion = "Unknown";
    private string currentVersion = "Unknown";
    private string whatsNew = "";
    private bool showUpdateWindow = false;
    private bool showProgressBar = false;
    private float progress = 0f;



    [MenuItem("Tools/Check For Updates")]
    public static void CheckForUpdates()
    {
        GitHubPuller window = GetWindow<GitHubPuller>("Mobile Action Kit");
        window.LoadCurrentVersion();
        window.CheckVersion();

        // If versions match, don't re-open the window.
        if (window.latestVersion == window.currentVersion)
        {
            // Close the window immediately after checking the version, if they're the same
            window.Close();
        }
    }


    private void LoadCurrentVersion()
    {
        string localVersionPath = Path.Combine(Application.dataPath, "version.txt");
        if (File.Exists(localVersionPath))
        {
            currentVersion = File.ReadAllText(localVersionPath).Trim();
        }
        else
        {
            currentVersion = "Unknown";
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("UPDATES", EditorStyles.boldLabel);


        if (showUpdateWindow)
        {
            // Display only the first line of the current version
            string firstLineOfVersion = currentVersion.Split(new[] { '\n', '\r' })[0]; // Get the first line
            GUILayout.Label("Current Version: " + firstLineOfVersion, EditorStyles.boldLabel); // Show current version


            //  GUILayout.Label("Current Version: " + currentVersion, EditorStyles.boldLabel); // Show current version
            GUILayout.Label("Update Available: " + latestVersion, EditorStyles.boldLabel); // Show update version
            //GUILayout.Label("What's New:", EditorStyles.boldLabel);
            //GUILayout.Label(whatsNew, EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Update"))
            {
                showProgressBar = true;
                showUpdateWindow = false;
                EditorApplication.update += UpdateProgressBar;
                PullLatestFiles();
            }
        }
        else if (showProgressBar)
        {
            EditorGUI.ProgressBar(new Rect(10, 50, position.width - 20, 20), progress, "Updating...");
        }
        else
        {
            GUILayout.Label("Latest Version: " + latestVersion, EditorStyles.boldLabel);
            if (GUILayout.Button("Check for Updates"))
            {
                LoadCurrentVersion();
                CheckVersion();
            }
        }
    }


    private void CheckVersion()
    {
        latestVersion = GetLatestVersion();
        //whatsNew = GetWhatsNew(); // Fetch the What's New content from GitHub

        if (latestVersion == "Unknown")
        {
            EditorUtility.DisplayDialog("Updater", "Could not check for updates. Please try again.", "Close");
            return;
        }

        if (latestVersion == currentVersion)
        {
            EditorUtility.DisplayDialog("Updater", "You are up to date!", "Close");
            return;
        }
        else
        {
            showUpdateWindow = true;
        }
    }


    private void UpdateProgressBar()
    {
        progress += 0.02f;
        if (progress >= 1f)
        {
            showProgressBar = false;
            progress = 0f;
            EditorApplication.update -= UpdateProgressBar;
            EditorUtility.DisplayDialog("Updater", "You are up to date!", "OK");
        }
        Repaint();
    }

    private string GetLatestVersion()
    {
        return GetFileContentFromGitHub("version.txt");
    }
    private string GetFileContentFromGitHub(string filePath)
    {
        try
        {
            string url = $"https://raw.githubusercontent.com/{repoOwner}/{repoName}/main/{filePath}";

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "UnityGitHubPuller");
                string content = client.DownloadString(url);
                return content.Trim();
            }
        }
        catch (WebException ex)
        {
            Debug.LogWarning($"Could not retrieve {filePath}: {ex.Message}");
        }
        return "Unknown";
    }
    private void PullLatestFiles(string folderPath = "")
    {
        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{folderPath}";
        using (WebClient client = new WebClient())
        {
            client.Headers.Add("User-Agent", "UnityGitHubPuller");

            try
            {
                string response = client.DownloadString(url);
                var files = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response);

                foreach (var file in files)
                {
                    if (file["type"].ToString() == "dir")
                    {
                        PullLatestFiles(file["path"].ToString());
                    }
                    else if (file.ContainsKey("path"))
                    {
                        string filePath = file["path"].ToString();
                        string downloadUrl = $"https://raw.githubusercontent.com/{repoOwner}/{repoName}/main/{filePath}";
                        DownloadFileIfChanged(downloadUrl, filePath);
                    }
                }
            }
            catch (WebException ex)
            {
                Debug.LogError($"Error fetching files: {ex.Message}");
            }
        }
    }

    private void DownloadFileIfChanged(string url, string filePath)
    {
        string localPath = Path.Combine(Application.dataPath, filePath);
        string directoryPath = Path.GetDirectoryName(localPath);

        if (string.IsNullOrEmpty(directoryPath))
        {
            Debug.LogError($"Invalid directory path for {filePath}");
            return;
        }

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string githubSHA = GetFileSHA(filePath);
        string localSHA = GetLocalFileSHA(localPath);

        if (!string.IsNullOrEmpty(localSHA) && githubSHA == localSHA)
        {
            Debug.Log($"File {filePath} is up to date. Skipping update.");
            return;
        }

        Debug.Log($"Updating file: {filePath}");

        try
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "UnityGitHubPuller");
                byte[] fileData = client.DownloadData(url);

                // Write new data to the file (no deletion)
                File.WriteAllBytes(localPath, fileData);
                Debug.Log($"Updated file: {localPath}");
            }

            // Refresh Unity without forcing full reimport unless needed
            string assetPath = "Assets/" + filePath;
            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath) != null)
            {
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.Default);
            }
        }
        catch (WebException ex)
        {
            Debug.LogError($"Error downloading file {filePath}: {ex.Message}");
        }

        AssetDatabase.Refresh();
    }

    private string GetFileSHA(string filePath)
    {
        try
        {
            string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{filePath}";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "UnityGitHubPuller");

                string response = client.DownloadString(url);
                var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

                if (jsonResponse.ContainsKey("sha"))
                {
                    return jsonResponse["sha"].ToString();
                }
            }
        }
        catch (WebException ex)
        {
            Debug.LogWarning($"Could not retrieve SHA for {filePath}: {ex.Message}");
        }
        return null;
    }


    private string GetLocalFileSHA(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        using (var sha = System.Security.Cryptography.SHA1.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                return System.BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLower();
            }
        }
    }
}




//using UnityEditor;
//using UnityEngine;
//using System.IO;
//using System.Net;
//using Unity.Plastic.Newtonsoft.Json;
//using System.Collections.Generic;

//public class GitHubPuller : EditorWindow
//{
//    private string repoOwner = GitHubConfig.RepositoryOwner;
//    private string repoName = GitHubConfig.RepositoryName;
//    private string token = GitHubConfig.Token;
//    private string latestVersion = "Unknown";
//    private string currentVersion = "Unknown";
//    private string whatsNew = "";
//    private bool showUpdateWindow = false;
//    private bool showProgressBar = false;
//    private float progress = 0f;



//    [MenuItem("Tools/Check For Updates")]
//    public static void CheckForUpdates()
//    {
//        GitHubPuller window = GetWindow<GitHubPuller>("Mobile Action Kit");
//        window.LoadCurrentVersion();
//        window.CheckVersion();

//        // If versions match, don't re-open the window.
//        if (window.latestVersion == window.currentVersion)
//        {
//            // Close the window immediately after checking the version, if they're the same
//            window.Close();
//        }
//    }


//    private void LoadCurrentVersion()
//    {
//        string localVersionPath = Path.Combine(Application.dataPath, "version.txt");
//        if (File.Exists(localVersionPath))
//        {
//            currentVersion = File.ReadAllText(localVersionPath).Trim();
//        }
//        else
//        {
//            currentVersion = "Unknown";
//        }
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("GitHub Updater", EditorStyles.boldLabel);


//        if (showUpdateWindow)
//        {
//            // Display only the first line of the current version
//            string firstLineOfVersion = currentVersion.Split(new[] { '\n', '\r' })[0]; // Get the first line
//            GUILayout.Label("Current Version: " + firstLineOfVersion, EditorStyles.boldLabel); // Show current version


//            //  GUILayout.Label("Current Version: " + currentVersion, EditorStyles.boldLabel); // Show current version
//            GUILayout.Label("Update Available: " + latestVersion, EditorStyles.boldLabel); // Show update version
//            //GUILayout.Label("What's New:", EditorStyles.boldLabel);
//            //GUILayout.Label(whatsNew, EditorStyles.wordWrappedLabel);

//            if (GUILayout.Button("Update"))
//            {
//                showProgressBar = true;
//                showUpdateWindow = false;
//                EditorApplication.update += UpdateProgressBar;
//                PullLatestFiles();
//            }
//        }
//        else if (showProgressBar)
//        {
//            EditorGUI.ProgressBar(new Rect(10, 50, position.width - 20, 20), progress, "Updating...");
//        }
//        else
//        {
//            GUILayout.Label("Latest Version: " + latestVersion, EditorStyles.boldLabel);
//            if (GUILayout.Button("Check for Updates"))
//            {
//                LoadCurrentVersion();
//                CheckVersion();
//            }
//        }
//    }


//    private void CheckVersion()
//    {
//        latestVersion = GetLatestVersion();
//        //whatsNew = GetWhatsNew(); // Fetch the What's New content from GitHub

//        if (latestVersion == "Unknown")
//        {
//            EditorUtility.DisplayDialog("Updater", "Could not check for updates. Please try again.", "Close");
//            return;
//        }

//        if (latestVersion == currentVersion)
//        {
//            EditorUtility.DisplayDialog("Updater", "You are up to date!", "Close");
//            return;
//        }
//        else
//        {
//            showUpdateWindow = true;
//        }
//    }


//    private void UpdateProgressBar()
//    {
//        progress += 0.02f;
//        if (progress >= 1f)
//        {
//            showProgressBar = false;
//            progress = 0f;
//            EditorApplication.update -= UpdateProgressBar;
//            EditorUtility.DisplayDialog("Updater", "You are up to date!", "OK");
//        }
//        Repaint();
//    }

//    private string GetLatestVersion()
//    {
//        return GetFileContentFromGitHub("version.txt");
//    }

//    private string GetFileContentFromGitHub(string filePath)
//    {
//        try
//        {
//            string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{filePath}";
//            using (WebClient client = new WebClient())
//            {
//                client.Headers.Add("Authorization", "token " + token);
//                client.Headers.Add("User-Agent", "UnityGitHubPuller");
//                string response = client.DownloadString(url);

//                if (!string.IsNullOrEmpty(response))
//                {
//                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
//                    if (jsonResponse != null && jsonResponse.ContainsKey("content"))
//                    {
//                        string encodedContent = jsonResponse["content"].ToString();
//                        return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(encodedContent)).Trim();
//                    }
//                }
//            }
//        }
//        catch (WebException ex)
//        {
//            Debug.LogWarning($"Could not retrieve {filePath}: {ex.Message}");
//        }
//        return "Unknown";
//    }
//    private void PullLatestFiles(string folderPath = "")
//    {
//        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{folderPath}";
//        using (WebClient client = new WebClient())
//        {
//            client.Headers.Add("Authorization", "token " + token);
//            client.Headers.Add("User-Agent", "UnityGitHubPuller");

//            try
//            {
//                string response = client.DownloadString(url);
//                if (string.IsNullOrEmpty(response))
//                {
//                    Debug.LogError("GitHub response was empty.");
//                    return;
//                }

//                var files = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response);

//                if (files == null || files.Count == 0)
//                {
//                    Debug.LogWarning("No files found in the repository.");
//                    return;
//                }

//                foreach (var file in files)
//                {
//                    if (file.ContainsKey("type") && file["type"].ToString() == "dir")
//                    {
//                        string localFolderPath = Path.Combine(Application.dataPath, file["path"].ToString());
//                        if (!Directory.Exists(localFolderPath))
//                        {
//                            Directory.CreateDirectory(localFolderPath);
//                            Debug.Log($"Created directory: {localFolderPath}");
//                        }

//                        PullLatestFiles(file["path"].ToString()); // Recursively fetch contents inside this folder
//                    }
//                    else if (file.ContainsKey("download_url") && file.ContainsKey("path"))
//                    {
//                        string fileUrl = file["download_url"].ToString();
//                        string filePath = file["path"].ToString();
//                        if (!string.IsNullOrEmpty(fileUrl) && !string.IsNullOrEmpty(filePath))
//                        {
//                            DownloadFileIfChanged(fileUrl, filePath);
//                        }
//                        else
//                        {
//                            Debug.LogWarning($"Invalid file entry: {file}");
//                        }
//                    }
//                }
//            }
//            catch (WebException ex)
//            {
//                Debug.LogError($"Error fetching files from GitHub: {ex.Message}");
//            }
//        }
//    }
//    private void DownloadFileIfChanged(string url, string filePath)
//    {
//        string localPath = Path.Combine(Application.dataPath, filePath);
//        string directoryPath = Path.GetDirectoryName(localPath);

//        if (string.IsNullOrEmpty(directoryPath))
//        {
//            Debug.LogError($"Invalid directory path for {filePath}");
//            return;
//        }

//        if (!Directory.Exists(directoryPath))
//        {
//            Directory.CreateDirectory(directoryPath);
//        }

//        string githubSHA = GetFileSHA(filePath);
//        string localSHA = GetLocalFileSHA(localPath);

//        if (!string.IsNullOrEmpty(localSHA) && githubSHA == localSHA)
//        {
//            Debug.Log($"File {filePath} is up to date. Skipping update.");
//            return;
//        }

//        Debug.Log($"Updating file: {filePath}");

//        try
//        {
//            using (WebClient client = new WebClient())
//            {
//                client.Headers.Add("Authorization", "token " + token);
//                byte[] fileData = client.DownloadData(url);

//                // Write new data to the file (no deletion)
//                File.WriteAllBytes(localPath, fileData);
//                Debug.Log($"Updated file: {localPath}");
//            }

//            // Refresh Unity without forcing full reimport unless needed
//            string assetPath = "Assets/" + filePath;
//            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath) != null)
//            {
//                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.Default);
//            }
//        }
//        catch (WebException ex)
//        {
//            Debug.LogError($"Error downloading file {filePath}: {ex.Message}");
//        }

//        AssetDatabase.Refresh();
//    }
//    private string GetFileSHA(string filePath)
//    {
//        try
//        {
//            string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{filePath}";
//            using (WebClient client = new WebClient())
//            {
//                client.Headers.Add("Authorization", "token " + token);
//                client.Headers.Add("User-Agent", "UnityGitHubPuller");

//                string response = client.DownloadString(url);
//                var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

//                if (jsonResponse.ContainsKey("sha"))
//                {
//                    return jsonResponse["sha"].ToString();
//                }
//            }
//        }
//        catch (WebException ex)
//        {
//            Debug.LogWarning($"Could not retrieve SHA for {filePath}: {ex.Message}");
//        }
//        return null;
//    }

//    private string GetLocalFileSHA(string filePath)
//    {
//        if (!File.Exists(filePath))
//        {
//            return null;
//        }

//        using (var sha = System.Security.Cryptography.SHA1.Create())
//        {
//            using (var stream = File.OpenRead(filePath))
//            {
//                return System.BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLower();
//            }
//        }
//    }
//}




//using UnityEditor;
//using UnityEngine;
//using System.IO;
//using System.Net;
//using Unity.Plastic.Newtonsoft.Json;
//using System.Collections.Generic;

//public class GitHubPuller : EditorWindow
//{
//    private string repoOwner = GitHubConfig.RepositoryOwner;
//    private string repoName = GitHubConfig.RepositoryName;
//    private string token = GitHubConfig.Token;
//    private string latestVersion = "Unknown";
//    private string currentVersion = "Unknown";
//    private string whatsNew = "";
//    private bool showUpdateWindow = false;
//    private bool showProgressBar = false;
//    private float progress = 0f;

//    [MenuItem("Tools/Check For Updates")]
//    public static void CheckForUpdates()
//    {
//        GitHubPuller window = GetWindow<GitHubPuller>("Mobile Action Kit");
//        window.LoadCurrentVersion();
//        window.CheckVersion();

//        // If versions match, don't re-open the window.
//        if (window.latestVersion == window.currentVersion)
//        {
//            // Close the window immediately after checking the version, if they're the same
//            window.Close();
//        }
//    }


//    private void LoadCurrentVersion()
//    {
//        string localVersionPath = Path.Combine(Application.dataPath, "version.txt");
//        if (File.Exists(localVersionPath))
//        {
//            currentVersion = File.ReadAllText(localVersionPath).Trim();
//        }
//        else
//        {
//            currentVersion = "Unknown";
//        }
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("GitHub Updater", EditorStyles.boldLabel);

//        if (showUpdateWindow)
//        {
//            // Display only the first line of the current version
//            string firstLineOfVersion = currentVersion.Split(new[] { '\n', '\r' })[0]; // Get the first line
//            GUILayout.Label("Current Version: " + firstLineOfVersion, EditorStyles.boldLabel); // Show current version


//            //  GUILayout.Label("Current Version: " + currentVersion, EditorStyles.boldLabel); // Show current version
//            GUILayout.Label("Update Available: " + latestVersion, EditorStyles.boldLabel); // Show update version
//            //GUILayout.Label("What's New:", EditorStyles.boldLabel);
//            //GUILayout.Label(whatsNew, EditorStyles.wordWrappedLabel);

//            if (GUILayout.Button("Update"))
//            {
//                showProgressBar = true;
//                showUpdateWindow = false;
//                EditorApplication.update += UpdateProgressBar;
//                PullLatestFiles();
//            }
//        }
//        else if (showProgressBar)
//        {
//            EditorGUI.ProgressBar(new Rect(10, 50, position.width - 20, 20), progress, "Updating...");
//        }
//        else
//        {
//            GUILayout.Label("Latest Version: " + latestVersion, EditorStyles.boldLabel);
//            if (GUILayout.Button("Check for Updates"))
//            {
//                LoadCurrentVersion();
//                CheckVersion();
//            }
//        }
//    }


//    private void CheckVersion()
//    {
//        latestVersion = GetLatestVersion();
//        //whatsNew = GetWhatsNew(); // Fetch the What's New content from GitHub

//        if (latestVersion == "Unknown")
//        {
//            EditorUtility.DisplayDialog("Updater", "Could not check for updates. Please try again.", "Close");
//            return;
//        }

//        if (latestVersion == currentVersion)
//        {
//            EditorUtility.DisplayDialog("Updater", "You are up to date!", "Close");
//            return;
//        }
//        else
//        {
//            showUpdateWindow = true;
//        }
//    }


//    private void UpdateProgressBar()
//    {
//        progress += 0.02f;
//        if (progress >= 1f)
//        {
//            showProgressBar = false;
//            progress = 0f;
//            EditorApplication.update -= UpdateProgressBar;
//            EditorUtility.DisplayDialog("Updater", "You are up to date!", "OK");
//        }
//        Repaint();
//    }

//    private string GetLatestVersion()
//    {
//        return GetFileContentFromGitHub("version.txt");
//    }

//    //private string GetWhatsNew()
//    //{
//    //    return GetFileContentFromGitHub("whatsnew.txt");
//    //}

//    private string GetFileContentFromGitHub(string filePath)
//    {
//        try
//        {
//            string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{filePath}";
//            using (WebClient client = new WebClient())
//            {
//                client.Headers.Add("Authorization", "token " + token);
//                client.Headers.Add("User-Agent", "UnityGitHubPuller");
//                string response = client.DownloadString(url);

//                if (!string.IsNullOrEmpty(response))
//                {
//                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
//                    if (jsonResponse != null && jsonResponse.ContainsKey("content"))
//                    {
//                        string encodedContent = jsonResponse["content"].ToString();
//                        return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(encodedContent)).Trim();
//                    }
//                }
//            }
//        }
//        catch (WebException ex)
//        {
//            Debug.LogWarning($"Could not retrieve {filePath}: {ex.Message}");
//        }
//        return "Unknown";
//    }
//    private void PullLatestFiles(string folderPath = "")
//    {
//        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{folderPath}";
//        using (WebClient client = new WebClient())
//        {
//            client.Headers.Add("Authorization", "token " + token);
//            client.Headers.Add("User-Agent", "UnityGitHubPuller");

//            try
//            {
//                string response = client.DownloadString(url);
//                if (string.IsNullOrEmpty(response))
//                {
//                    Debug.LogError("GitHub response was empty.");
//                    return;
//                }

//                var files = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response);

//                if (files == null || files.Count == 0)
//                {
//                    Debug.LogWarning("No files found in the repository.");
//                    return;
//                }

//                foreach (var file in files)
//                {
//                    if (file.ContainsKey("type") && file["type"].ToString() == "dir")
//                    {
//                        string localFolderPath = Path.Combine(Application.dataPath, file["path"].ToString());
//                        if (!Directory.Exists(localFolderPath))
//                        {
//                            Directory.CreateDirectory(localFolderPath);
//                            Debug.Log($"Created directory: {localFolderPath}");
//                        }

//                        PullLatestFiles(file["path"].ToString()); // Recursively fetch contents inside this folder
//                    }
//                    else if (file.ContainsKey("download_url") && file.ContainsKey("path"))
//                    {
//                        string fileUrl = file["download_url"].ToString();
//                        string filePath = file["path"].ToString();
//                        if (!string.IsNullOrEmpty(fileUrl) && !string.IsNullOrEmpty(filePath))
//                        {
//                            DownloadFileIfChanged(fileUrl, filePath);
//                        }
//                        else
//                        {
//                            Debug.LogWarning($"Invalid file entry: {file}");
//                        }
//                    }
//                }
//            }
//            catch (WebException ex)
//            {
//                Debug.LogError($"Error fetching files from GitHub: {ex.Message}");
//            }
//        }
//    }
//    private void DownloadFileIfChanged(string url, string filePath)
//    {
//        string localPath = Path.Combine(Application.dataPath, filePath);
//        string directoryPath = Path.GetDirectoryName(localPath);

//        if (string.IsNullOrEmpty(directoryPath))
//        {
//            Debug.LogError($"Invalid directory path for {filePath}");
//            return;
//        }

//        if (!Directory.Exists(directoryPath))
//        {
//            Directory.CreateDirectory(directoryPath);
//        }

//        string githubSHA = GetFileSHA(filePath);
//        string localSHA = GetLocalFileSHA(localPath);

//        if (!string.IsNullOrEmpty(localSHA) && githubSHA == localSHA)
//        {
//            Debug.Log($"File {filePath} is up to date. Skipping update.");
//            return;
//        }

//        Debug.Log($"Updating file: {filePath}");

//        try
//        {
//            using (WebClient client = new WebClient())
//            {
//                client.Headers.Add("Authorization", "token " + token);
//                byte[] fileData = client.DownloadData(url);

//                // Write new data to the file (no deletion)
//                File.WriteAllBytes(localPath, fileData);
//                Debug.Log($"Updated file: {localPath}");
//            }

//            // Refresh Unity without forcing full reimport unless needed
//            string assetPath = "Assets/" + filePath;
//            if (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath) != null)
//            {
//                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.Default);
//            }
//        }
//        catch (WebException ex)
//        {
//            Debug.LogError($"Error downloading file {filePath}: {ex.Message}");
//        }

//        AssetDatabase.Refresh();
//    }

//private void DownloadFileIfChanged(string url, string filePath)
//{
//    string localPath = Path.Combine(Application.dataPath, filePath);
//    string directoryPath = Path.GetDirectoryName(localPath);

//    if (string.IsNullOrEmpty(directoryPath))
//    {
//        Debug.LogError($"Invalid directory path for {filePath}");
//        return;
//    }

//    if (!Directory.Exists(directoryPath))
//    {
//        Directory.CreateDirectory(directoryPath);
//    }

//    string githubSHA = GetFileSHA(filePath);
//    string localSHA = GetLocalFileSHA(localPath);

//    if (githubSHA == localSHA)
//    {
//        Debug.Log($"File {filePath} is up to date. Skipping update.");
//        return;
//    }

//    Debug.Log($"Updating file: {filePath} (SHA mismatch detected)");

//    using (WebClient client = new WebClient())
//    {
//        client.Headers.Add("Authorization", "token " + token);

//        try
//        {
//            byte[] fileData = client.DownloadData(url);
//            File.WriteAllBytes(localPath, fileData);
//            Debug.Log($"Downloaded and replaced file: {localPath}");
//        }
//        catch (WebException ex)
//        {
//            Debug.LogError($"Error downloading file {filePath}: {ex.Message}");
//        }
//    }

//    AssetDatabase.ImportAsset("Assets/" + filePath, ImportAssetOptions.ForceUpdate);
//    AssetDatabase.Refresh();
//}
//    private string GetFileSHA(string filePath)
//    {
//        try
//        {
//            string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{filePath}";
//            using (WebClient client = new WebClient())
//            {
//                client.Headers.Add("Authorization", "token " + token);
//                client.Headers.Add("User-Agent", "UnityGitHubPuller");

//                string response = client.DownloadString(url);
//                var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

//                if (jsonResponse.ContainsKey("sha"))
//                {
//                    return jsonResponse["sha"].ToString();
//                }
//            }
//        }
//        catch (WebException ex)
//        {
//            Debug.LogWarning($"Could not retrieve SHA for {filePath}: {ex.Message}");
//        }
//        return null;
//    }

//    private string GetLocalFileSHA(string filePath)
//    {
//        if (!File.Exists(filePath))
//        {
//            return null;
//        }

//        using (var sha = System.Security.Cryptography.SHA1.Create())
//        {
//            using (var stream = File.OpenRead(filePath))
//            {
//                return System.BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLower();
//            }
//        }
//    }
//}








//using UnityEditor;
//using UnityEngine;
//using System.IO;
//using System.Net;
//using Unity.Plastic.Newtonsoft.Json;
//using System.Collections.Generic;

//public class GitHubPuller : EditorWindow
//{
//    private string repoOwner = GitHubConfig.RepositoryOwner;
//    private string repoName = GitHubConfig.RepositoryName;
//    private string token = GitHubConfig.Token;
//    private string latestVersion = "Unknown";

//    [MenuItem("Tools/GitHub Puller")]
//    public static void ShowWindow()
//    {
//        GetWindow<GitHubPuller>("GitHub Puller");
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("GitHub Updater", EditorStyles.boldLabel);

//        if (GUILayout.Button("Check for Updates"))
//        {
//            latestVersion = GetLatestVersion();
//            Debug.Log("Latest Version on GitHub: " + latestVersion);
//        }

//        GUILayout.Label("Latest Version: " + latestVersion, EditorStyles.boldLabel);

//        if (GUILayout.Button("Pull Latest Changes"))
//        {
//            PullLatestFiles();
//        }
//    }

//    private string GetLatestVersion()
//    {
//        try
//        {
//            string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/version.txt";
//            using (WebClient client = new WebClient())
//            {
//                client.Headers.Add("Authorization", "token " + token);
//                client.Headers.Add("User-Agent", "UnityGitHubPuller");
//                string response = client.DownloadString(url);

//                if (!string.IsNullOrEmpty(response))
//                {
//                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
//                    if (jsonResponse != null && jsonResponse.ContainsKey("content"))
//                    {
//                        string encodedContent = jsonResponse["content"].ToString();
//                        return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(encodedContent)).Trim();
//                    }
//                }
//            }
//        }
//        catch (WebException ex)
//        {
//            Debug.LogWarning($"Could not retrieve the latest version: {ex.Message}");
//        }
//        return "Unknown";
//    }

//    private void PullLatestFiles(string folderPath = "")
//    {
//        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{folderPath}";
//        using (WebClient client = new WebClient())
//        {
//            client.Headers.Add("Authorization", "token " + token);
//            client.Headers.Add("User-Agent", "UnityGitHubPuller");

//            try
//            {
//                string response = client.DownloadString(url);
//                if (string.IsNullOrEmpty(response))
//                {
//                    Debug.LogError("GitHub response was empty.");
//                    return;
//                }

//                var files = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response);

//                if (files == null || files.Count == 0)
//                {
//                    Debug.LogWarning("No files found in the repository.");
//                    return;
//                }

//                foreach (var file in files)
//                {
//                    if (file.ContainsKey("type") && file["type"].ToString() == "dir")
//                    {
//                        string localFolderPath = Path.Combine(Application.dataPath, file["path"].ToString());
//                        if (!Directory.Exists(localFolderPath))
//                        {
//                            Directory.CreateDirectory(localFolderPath);
//                            Debug.Log($"Created directory: {localFolderPath}");
//                        }

//                        PullLatestFiles(file["path"].ToString()); // Recursively fetch contents inside this folder
//                    }
//                    else if (file.ContainsKey("download_url") && file.ContainsKey("path"))
//                    {
//                        string fileUrl = file["download_url"].ToString();
//                        string filePath = file["path"].ToString();
//                        if (!string.IsNullOrEmpty(fileUrl) && !string.IsNullOrEmpty(filePath))
//                        {
//                            DownloadFileIfChanged(fileUrl, filePath);
//                        }
//                        else
//                        {
//                            Debug.LogWarning($"Invalid file entry: {file}");
//                        }
//                    }
//                }
//            }
//            catch (WebException ex)
//            {
//                Debug.LogError($"Error fetching files from GitHub: {ex.Message}");
//            }
//        }
//    }

//    private void DownloadFileIfChanged(string url, string filePath)
//    {
//        string localPath = Path.Combine(Application.dataPath, filePath);
//        string directoryPath = Path.GetDirectoryName(localPath);

//        if (string.IsNullOrEmpty(directoryPath))
//        {
//            Debug.LogError($"Invalid directory path for {filePath}");
//            return;
//        }

//        if (!Directory.Exists(directoryPath))
//        {
//            Directory.CreateDirectory(directoryPath);
//        }

//        string githubSHA = GetFileSHA(filePath);
//        string localSHA = GetLocalFileSHA(localPath);

//        if (githubSHA == localSHA)
//        {
//            Debug.Log($"File {filePath} is up to date. Skipping update.");
//            return;
//        }

//        Debug.Log($"Updating file: {filePath} (SHA mismatch detected)");

//        using (WebClient client = new WebClient())
//        {
//            client.Headers.Add("Authorization", "token " + token);

//            try
//            {
//                byte[] fileData = client.DownloadData(url);
//                File.WriteAllBytes(localPath, fileData);
//                Debug.Log($"Downloaded and replaced file: {localPath}");
//            }
//            catch (WebException ex)
//            {
//                Debug.LogError($"Error downloading file {filePath}: {ex.Message}");
//            }
//        }

//        AssetDatabase.ImportAsset("Assets/" + filePath, ImportAssetOptions.ForceUpdate);
//        AssetDatabase.Refresh();
//    }

//    private string GetFileSHA(string filePath)
//    {
//        try
//        {
//            string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{filePath}";
//            using (WebClient client = new WebClient())
//            {
//                client.Headers.Add("Authorization", "token " + token);
//                client.Headers.Add("User-Agent", "UnityGitHubPuller");

//                string response = client.DownloadString(url);
//                var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);

//                if (jsonResponse.ContainsKey("sha"))
//                {
//                    return jsonResponse["sha"].ToString();
//                }
//            }
//        }
//        catch (WebException ex)
//        {
//            Debug.LogWarning($"Could not retrieve SHA for {filePath}: {ex.Message}");
//        }
//        return null;
//    }

//    private string GetLocalFileSHA(string filePath)
//    {
//        if (!File.Exists(filePath))
//        {
//            return null;
//        }

//        using (var sha = System.Security.Cryptography.SHA1.Create())
//        {
//            using (var stream = File.OpenRead(filePath))
//            {
//                return System.BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLower();
//            }
//        }
//    }
//}




//using UnityEditor;
//using UnityEngine;
//using System.IO;
//using System.Net;
//using Unity.Plastic.Newtonsoft.Json;
//using System.Collections.Generic;

//public class GitHubPuller : EditorWindow
//{
//    private string repoOwner = GitHubConfig.RepositoryOwner;
//    private string repoName = GitHubConfig.RepositoryName;
//    private string token = GitHubConfig.Token;
//    private string latestVersion = "Unknown";

//    [MenuItem("Tools/GitHub Puller")]
//    public static void ShowWindow()
//    {
//        GetWindow<GitHubPuller>("GitHub Puller");
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("GitHub Updater", EditorStyles.boldLabel);

//        if (GUILayout.Button("Check for Updates"))
//        {
//            latestVersion = GetLatestVersion();
//            Debug.Log("Latest Version on GitHub: " + latestVersion);
//        }

//        GUILayout.Label("Latest Version: " + latestVersion, EditorStyles.boldLabel);

//        if (GUILayout.Button("Pull Latest Changes"))
//        {
//            PullLatestFiles();
//        }
//    }

//    private string GetLatestVersion()
//    {
//        try
//        {
//            string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/version.txt";
//            using (WebClient client = new WebClient())
//            {
//                client.Headers.Add("Authorization", "token " + token);
//                client.Headers.Add("User-Agent", "UnityGitHubPuller");
//                string response = client.DownloadString(url);

//                if (!string.IsNullOrEmpty(response))
//                {
//                    var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
//                    if (jsonResponse != null && jsonResponse.ContainsKey("content"))
//                    {
//                        string encodedContent = jsonResponse["content"].ToString();
//                        return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(encodedContent)).Trim();
//                    }
//                }
//            }
//        }
//        catch (WebException ex)
//        {
//            Debug.LogWarning($"Could not retrieve the latest version: {ex.Message}");
//        }
//        return "Unknown";
//    }

//    private void PullLatestFiles(string folderPath = "")
//    {
//        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/{folderPath}";
//        using (WebClient client = new WebClient())
//        {
//            client.Headers.Add("Authorization", "token " + token);
//            client.Headers.Add("User-Agent", "UnityGitHubPuller");

//            try
//            {
//                string response = client.DownloadString(url);
//                if (string.IsNullOrEmpty(response))
//                {
//                    Debug.LogError("GitHub response was empty.");
//                    return;
//                }

//                var files = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response);

//                if (files == null || files.Count == 0)
//                {
//                    Debug.LogWarning("No files found in the repository.");
//                    return;
//                }

//                foreach (var file in files)
//                {
//                    if (file.ContainsKey("type") && file["type"].ToString() == "dir")
//                    {
//                        // Create the directory if it's missing
//                        string localFolderPath = Path.Combine(Application.dataPath, file["path"].ToString());
//                        if (!Directory.Exists(localFolderPath))
//                        {
//                            Directory.CreateDirectory(localFolderPath);
//                            Debug.Log($"Created directory: {localFolderPath}");
//                        }

//                        // Recursively fetch contents inside this folder
//                        PullLatestFiles(file["path"].ToString());
//                    }
//                    else if (file.ContainsKey("download_url") && file.ContainsKey("path"))
//                    {
//                        string fileUrl = file["download_url"].ToString();
//                        string filePath = file["path"].ToString();
//                        if (!string.IsNullOrEmpty(fileUrl) && !string.IsNullOrEmpty(filePath))
//                        {
//                            DownloadFile(fileUrl, filePath);
//                        }
//                        else
//                        {
//                            Debug.LogWarning($"Invalid file entry: {file}");
//                        }
//                    }
//                }
//            }
//            catch (WebException ex)
//            {
//                Debug.LogError($"Error fetching files from GitHub: {ex.Message}");
//            }
//        }
//    }


//    private void DownloadFile(string url, string filePath)
//    {
//        string localPath = Path.Combine(Application.dataPath, filePath);
//        string directoryPath = Path.GetDirectoryName(localPath);

//        if (string.IsNullOrEmpty(directoryPath))
//        {
//            Debug.LogError($"Invalid directory path for {filePath}");
//            return;
//        }

//        if (!Directory.Exists(directoryPath))
//        {
//            Directory.CreateDirectory(directoryPath);
//        }

//        // 🔴 Delete the existing file to ensure a fresh update
//        if (File.Exists(localPath))
//        {
//            File.Delete(localPath);
//            Debug.Log($"Deleted old file: {localPath}");
//        }

//        using (WebClient client = new WebClient())
//        {
//            client.Headers.Add("Authorization", "token " + token);

//            try
//            {
//                byte[] fileData = client.DownloadData(url); // Download as byte array
//                File.WriteAllBytes(localPath, fileData); // Overwrite with new content
//                Debug.Log($"Downloaded and replaced file: {localPath}");
//            }
//            catch (WebException ex)
//            {
//                Debug.LogError($"Error downloading file {filePath}: {ex.Message}");
//            }
//        }

//        // 🔄 Force Unity to re-import the asset
//        string relativePath = "Assets/" + filePath;
//        AssetDatabase.ImportAsset(relativePath, ImportAssetOptions.ForceUpdate);
//        AssetDatabase.Refresh();
//    }


//}





//using UnityEditor;
//using UnityEngine;
//using System.IO;
//using System.Net;
//using Unity.Plastic.Newtonsoft.Json; // Install Newtonsoft JSON via Package Manager
//using System.Collections.Generic;

//public class GitHubPuller : EditorWindow
//{
//    private string repoOwner = GitHubConfig.RepositoryOwner;
//    private string repoName = GitHubConfig.RepositoryName;
//    private string token = GitHubConfig.Token;
//    private string latestVersion = "Unknown";

//    [MenuItem("Tools/GitHub Puller")]
//    public static void ShowWindow()
//    {
//        GetWindow<GitHubPuller>("GitHub Puller");
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("GitHub Updater", EditorStyles.boldLabel);

//        if (GUILayout.Button("Check for Updates"))
//        {
//            latestVersion = GetLatestVersion();
//            Debug.Log("Latest Version on GitHub: " + latestVersion);
//        }

//        GUILayout.Label("Latest Version: " + latestVersion, EditorStyles.boldLabel);

//        if (GUILayout.Button("Pull Latest Changes"))
//        {
//            PullLatestFiles();
//        }
//    }

//    private string GetLatestVersion()
//    {
//        try
//        {
//            string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/version.txt";
//            using (WebClient client = new WebClient())
//            {
//                client.Headers.Add("Authorization", "token " + token);
//                client.Headers.Add("User-Agent", "UnityGitHubPuller");
//                string response = client.DownloadString(url);

//                var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
//                if (jsonResponse.ContainsKey("content"))
//                {
//                    string encodedContent = jsonResponse["content"].ToString();
//                    return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(encodedContent)).Trim();
//                }
//            }
//        }
//        catch (WebException)
//        {
//            Debug.LogWarning("Could not retrieve the latest version. Defaulting to unknown.");
//        }
//        return "Unknown";
//    }

//    private void PullLatestFiles()
//    {
//        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/";
//        using (WebClient client = new WebClient())
//        {
//            client.Headers.Add("Authorization", "token " + token);
//            client.Headers.Add("User-Agent", "UnityGitHubPuller");

//            string response = client.DownloadString(url);
//            var files = JsonConvert.DeserializeObject<object[]>(response);

//            foreach (var file in files)
//            {
//                var jsonFile = JsonConvert.DeserializeObject<Dictionary<string, object>>(file.ToString());
//                if (jsonFile.ContainsKey("download_url") && jsonFile.ContainsKey("path"))
//                {
//                    string fileUrl = jsonFile["download_url"].ToString();
//                    string filePath = jsonFile["path"].ToString();

//                    DownloadFile(fileUrl, filePath);
//                }
//            }
//        }
//    }

//    private void DownloadFile(string url, string filePath)
//    {
//        string localPath = Path.Combine(Application.dataPath, filePath);
//        string directoryPath = Path.GetDirectoryName(localPath);

//        // Ensure the directory exists
//        if (!Directory.Exists(directoryPath))
//        {
//            Directory.CreateDirectory(directoryPath);
//        }

//        using (WebClient client = new WebClient())
//        {
//            client.Headers.Add("Authorization", "token " + token);
//            client.DownloadFile(url, localPath);
//        }

//        Debug.Log($"Updated file: {localPath}");
//        AssetDatabase.Refresh();
//    }

//}
