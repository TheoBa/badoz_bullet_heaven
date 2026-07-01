using System;
using System.IO;
using UnityEngine;
using BulletHeaven.Meta;
using BulletHeaven.Player;

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

        public bool IsNodeOwned(string nodeId) =>
            Data.passiveTreeAllocations.Exists(a => a.nodeId == nodeId);

        public bool CanPurchase(PassiveTreeData tree, string nodeId)
        {
            var node = tree.FindNode(nodeId);
            if (node == null || IsNodeOwned(nodeId)) return false;
            if (Data.macroResources < node.cost) return false;

            if (node.tier > 1)
            {
                var prevNode = tree.FindByStatTier(node.stat, node.tier - 1);
                if (prevNode == null || !IsNodeOwned(prevNode.nodeId)) return false;
            }
            return true;
        }

        public bool PurchaseNode(PassiveTreeData tree, string nodeId)
        {
            if (!CanPurchase(tree, nodeId)) return false;
            var node = tree.FindNode(nodeId);

            Data.macroResources -= node.cost;
            Data.passiveTreeAllocations.Add(new SaveData.PassiveAllocation { nodeId = nodeId, points = 1 });
            Save();
            return true;
        }

        // Free respec: refunds every spent resource and clears all allocations.
        public void ResetPassiveTree(PassiveTreeData tree)
        {
            int refund = 0;
            foreach (var alloc in Data.passiveTreeAllocations)
            {
                var node = tree.FindNode(alloc.nodeId);
                if (node != null) refund += node.cost;
            }

            Data.macroResources += refund;
            Data.passiveTreeAllocations.Clear();
            Save();
        }

        public float GetPassiveBonus(PassiveTreeData tree, StatType stat)
        {
            float total = 0f;
            foreach (var alloc in Data.passiveTreeAllocations)
            {
                var node = tree.FindNode(alloc.nodeId);
                if (node != null && node.stat == stat) total += node.statDelta;
            }
            return total;
        }
    }
}
