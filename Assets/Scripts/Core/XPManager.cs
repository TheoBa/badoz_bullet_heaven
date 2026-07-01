using System;
using UnityEngine;

namespace BulletHeaven.Core
{
    // Run-scoped XP/level tracker. Not DontDestroyOnLoad — lives for the duration of one run.
    public class XPManager : MonoBehaviour
    {
        public static XPManager Instance { get; private set; }

        [Header("Level Curve")]
        [SerializeField] private float baseXPToLevel = 20f;
        [SerializeField] private float xpCurveGrowth  = 8f; // quadratic term

        public event Action<int> OnLevelUp;

        public int   CurrentLevel  { get; private set; } = 1;
        public float XPIntoLevel   { get; private set; }
        public float XPToNextLevel => baseXPToLevel + xpCurveGrowth * CurrentLevel * CurrentLevel;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void AddXP(float amount)
        {
            XPIntoLevel += amount;
            GameManager.Instance?.RunData.AddXP(amount);

            while (XPIntoLevel >= XPToNextLevel)
            {
                XPIntoLevel -= XPToNextLevel;
                CurrentLevel++;
                GameManager.Instance?.RunData.IncrementLevel();
                OnLevelUp?.Invoke(CurrentLevel);
            }
        }
    }
}
