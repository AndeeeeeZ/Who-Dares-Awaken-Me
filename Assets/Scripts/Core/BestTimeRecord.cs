using System;
using System.IO;
using UnityEngine;

public static class BestTimeRecord
{
    private const string FileName = "best-time.json";
    private const string LegacyPlayerPrefsKey = "WhoDaresAwakenMe.BestWinTimeSeconds";
    private const int CurrentVersion = 1;

    [Serializable]
    private sealed class BestTimeData
    {
        public int version;
        public float bestTimeSeconds;
    }

    public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static bool TryGet(out float bestTimeSeconds)
    {
        bestTimeSeconds = 0f;
        if (!File.Exists(FilePath))
            return TryMigrateLegacy(out bestTimeSeconds);

        try
        {
            BestTimeData data = JsonUtility.FromJson<BestTimeData>(File.ReadAllText(FilePath));
            if (data != null && data.version == CurrentVersion && IsValid(data.bestTimeSeconds))
            {
                bestTimeSeconds = data.bestTimeSeconds;
                return true;
            }

            Debug.LogWarning($"Best time file is invalid or has an unsupported version: {FilePath}");
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not read best time file at {FilePath}: {exception.Message}");
        }

        return false;
    }

    public static void Record(float elapsedSeconds)
    {
        if (!IsValid(elapsedSeconds))
            return;

        if (TryGet(out float bestTimeSeconds) && elapsedSeconds >= bestTimeSeconds)
            return;

        Save(elapsedSeconds);
    }

    private static bool TryMigrateLegacy(out float bestTimeSeconds)
    {
        bestTimeSeconds = 0f;
        if (!PlayerPrefs.HasKey(LegacyPlayerPrefsKey))
            return false;

        float legacyTime = PlayerPrefs.GetFloat(LegacyPlayerPrefsKey);
        if (!IsValid(legacyTime))
            return false;

        bestTimeSeconds = legacyTime;
        if (Save(legacyTime))
        {
            PlayerPrefs.DeleteKey(LegacyPlayerPrefsKey);
            PlayerPrefs.Save();
        }

        return true;
    }

    private static bool Save(float bestTimeSeconds)
    {
        try
        {
            Directory.CreateDirectory(Application.persistentDataPath);
            BestTimeData data = new BestTimeData
            {
                version = CurrentVersion,
                bestTimeSeconds = bestTimeSeconds
            };
            string temporaryPath = FilePath + ".tmp";
            File.WriteAllText(temporaryPath, JsonUtility.ToJson(data, true));
            if (File.Exists(FilePath))
                File.Replace(temporaryPath, FilePath, null);
            else
                File.Move(temporaryPath, FilePath);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not save best time file at {FilePath}: {exception.Message}");
            return false;
        }
    }

    private static bool IsValid(float seconds)
    {
        return seconds >= 0f && !float.IsNaN(seconds) && !float.IsInfinity(seconds);
    }
}
