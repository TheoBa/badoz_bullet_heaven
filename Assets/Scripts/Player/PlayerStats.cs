using UnityEngine;

namespace BulletHeaven.Player
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "BulletHeaven/PlayerStats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("Vitals")]
        public float maxHP            = 100f;

        [Header("Movement")]
        public float speed            = 5f;

        [Header("Combat")]
        public float damageMultiplier = 1f;

        [Header("Progression")]
        public float xpGainMultiplier = 1f;
        public float pickupRadius     = 2f;
    }
}
