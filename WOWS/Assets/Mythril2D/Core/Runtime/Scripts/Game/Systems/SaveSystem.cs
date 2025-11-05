using System;
using System.IO;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class SaveDataBlock : DataBlock
    {
        public string header;
        public MapDataBlock map;
        public JournalDataBlock journal;
        public InventoryDataBlock inventory;
        public GameFlagsDataBlock gameFlags;
        public PlayerDataBlock player;
        public PersistenceDataBlock persistence;
    }

    public class SaveSystem : AGameSystem, IDataBlockHandler<SaveDataBlock>
    {
        public void LoadDefaultSaveFile(SaveFile saveFile)
        {
            LoadDataBlock(DuplicateSaveData(saveFile.content));
        }

        /**
        * Never use the m_defaultSaveFile as-is, but instead, always duplicate it (deep copy) to prevent changing the initial scriptable object data.
        * This is especially useful in editor. (TODO: make it #if UNITY_EDITOR, otherwise directly use the data without cloning it)
        */
        public SaveDataBlock DuplicateSaveData(SaveDataBlock saveFile)
        {
            string saveData = JsonUtility.ToJson(saveFile, true);
            return JsonUtility.FromJson<SaveDataBlock>(saveData);
        }

        public static void EraseSaveData(string saveFileName)
        {
            string filepath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, saveFileName));

            File.Delete(filepath);
        }

        public static SaveDataBlock ExtractSaveDataFromFile(string saveFileName)
        {
            string filepath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, saveFileName));

            if (!File.Exists(filepath))
            {
                return null;
            }

            try
            {
                using (FileStream stream = new(filepath, FileMode.Open))
                {
                    using (StreamReader reader = new(stream))
                    {
                        string dataToLoad = reader.ReadToEnd();
                        return JsonUtility.FromJson<SaveDataBlock>(dataToLoad);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public void LoadFromFile(string saveFileName)
        {
            SaveDataBlock saveData = ExtractSaveDataFromFile(saveFileName);

            if (saveData != null)
            {
                LoadDataBlock(saveData);
                Debug.Log($"Save <b>{saveFileName}</b> loaded successfully!");
            }
            else
            {
                Debug.LogError($"Save <b>{saveFileName}</b> failed to load!");
            }
        }

        public void SaveToFile(string saveFileName)
        {
            string filepath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, saveFileName));

            try
            {
                SaveDataBlock saveFile = CreateDataBlock();

                string dataToStore = JsonUtility.ToJson(saveFile, true);

                using (FileStream stream = new(filepath, FileMode.Create))
                {
                    using (StreamWriter writer = new(stream))
                    {
                        writer.Write(dataToStore);
                        long dataSize = dataToStore.Length;
                        string[] sizes = { "B", "KB", "MB", "GB" };
                        int order = 0;
                        while (dataSize >= 1024 && order < sizes.Length - 1)
                        {
                            order++;
                            dataSize = dataSize / 1024;
                        }

                        Debug.Log($"Saved <b>{saveFileName}</b> to disk ({dataSize:0.##} {sizes[order]}).\n{filepath}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save {filepath} to disk: {e.Message}");
            }
        }

        public string GenerateSavefileHeader()
        {
            string header;

            Hero player = GameManager.PlayerSystem.PlayerInstance;

            header = string.Format("{0} {1}{2}",
                player.characterSheet.displayName,
                GameManager.Config.GetTermDefinition("level").shortName,
                player.level
                );

            return header;
        }

        public SaveDataBlock CreateDataBlock()
        {
            return new SaveDataBlock
            {
                header = GenerateSavefileHeader(),
                map = GameManager.MapSystem.CreateDataBlock(),
                gameFlags = GameManager.GameFlagSystem.CreateDataBlock(),
                inventory = GameManager.InventorySystem.CreateDataBlock(),
                journal = GameManager.JournalSystem.CreateDataBlock(),
                player = GameManager.PlayerSystem.CreateDataBlock(),
                persistence = GameManager.PersistenceSystem.CreateDataBlock(),
            };
        }

        public void LoadDataBlock(SaveDataBlock block)
        {
            GameManager.GameFlagSystem.LoadDataBlock(block.gameFlags);
            GameManager.InventorySystem.LoadDataBlock(block.inventory);
            GameManager.JournalSystem.LoadDataBlock(block.journal);
            GameManager.PlayerSystem.LoadDataBlock(block.player);
            GameManager.PersistenceSystem.LoadDataBlock(block.persistence);
            GameManager.MapSystem.LoadDataBlock(block.map);
            GameManager.NotificationSystem.saveFileLoaded.Invoke();
        }
    }
}
