using System.Collections.Generic;
using UnityEngine;
using BulletHeaven.Skills;

namespace BulletHeaven.Core
{
    [CreateAssetMenu(fileName = "RunData", menuName = "BulletHeaven/RunData")]
    public class RunData : ScriptableObject
    {
        public int   CurrentWave            { get; private set; }
        public int   CurrentTier            { get; private set; }
        public int   CurrentLevel           { get; private set; }
        public float CurrentXP              { get; private set; }
        public int   ResourcesEarnedThisRun { get; private set; }
        public int   EnemiesKilled          { get; private set; }
        public float RunStartTime           { get; private set; }

        private readonly List<SkillBase> _ownedSkills = new();
        public IReadOnlyList<SkillBase> OwnedSkills => _ownedSkills;

        public void StartRun()
        {
            CurrentWave            = 0;
            CurrentTier            = 1;
            CurrentLevel           = 1;
            CurrentXP              = 0f;
            ResourcesEarnedThisRun = 0;
            EnemiesKilled          = 0;
            RunStartTime           = Time.time;
            _ownedSkills.Clear();
        }

        public void IncrementWave()   => CurrentWave++;
        public void IncrementTier()   => CurrentTier++;
        public void IncrementLevel()  => CurrentLevel++;
        public void AddXP(float xp)   => CurrentXP += xp;
        public void AddResource(int n) => ResourcesEarnedThisRun += n;
        public void AddKill()          => EnemiesKilled++;
        public void AddSkill(SkillBase skill) => _ownedSkills.Add(skill);

        public float ElapsedTime => Time.time - RunStartTime;
    }
}
