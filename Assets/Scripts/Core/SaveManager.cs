using System;
using System.IO;
using UnityEngine;

namespace BulletHeaven.Core
{
    // Persists cross-run meta-progression to disk. Not run-scoped — DontDestroyOnLoad.
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        public SaveData Data { get; private set; }

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) Save();
        }

        void OnApplicationQuit() => Save();

        public void Load()
        {
            if (File.Exists(SavePath))
            {
                try
                {
                    Data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"SaveManager: failed to read save file, starting fresh. {e.Message}");
                }
            }
            Data ??= new SaveData();
        }

        public void Save()
        {
            File.WriteAllText(SavePath, JsonUtility.ToJson(Data, prettyPrint: true));
        }
    }
}
