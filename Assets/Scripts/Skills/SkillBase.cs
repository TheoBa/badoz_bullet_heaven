using UnityEngine;
using BulletHeaven.Player;

namespace BulletHeaven.Skills
{
    public enum SkillCategory { Offensive, Defensive, Utility }

    public abstract class SkillBase : ScriptableObject
    {
        [Header("Display")]
        public string        skillName;
        [TextArea] public string description;
        public Sprite         icon;
        public SkillCategory  category;

        public abstract void ApplyToPlayer(PlayerStatsRuntime runtime);
    }
}
