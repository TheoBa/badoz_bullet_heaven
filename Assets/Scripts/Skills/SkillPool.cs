using UnityEngine;

namespace BulletHeaven.Skills
{
    [CreateAssetMenu(fileName = "SkillPool", menuName = "BulletHeaven/SkillPool")]
    public class SkillPool : ScriptableObject
    {
        public SkillBase[] skills;
    }
}
