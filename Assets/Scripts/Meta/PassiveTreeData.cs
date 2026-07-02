using System;
using UnityEngine;
using BulletHeaven.Player;

namespace BulletHeaven.Meta
{
    [CreateAssetMenu(fileName = "PassiveTreeData", menuName = "BulletHeaven/PassiveTreeData")]
    public class PassiveTreeData : ScriptableObject
    {
        [Serializable]
        public class TreeNode
        {
            public string   nodeId;
            public StatType stat;
            [Min(1)] public int tier; // 1-based; must own (tier - 1) of the same stat first
            public int       cost;    // macroResources cost
            public float     statDelta;
        }

        public TreeNode[] nodes;

        public TreeNode FindNode(string nodeId)
        {
            foreach (var n in nodes)
                if (n.nodeId == nodeId) return n;
            return null;
        }

        public TreeNode FindByStatTier(StatType stat, int tier)
        {
            foreach (var n in nodes)
                if (n.stat == stat && n.tier == tier) return n;
            return null;
        }
    }
}
