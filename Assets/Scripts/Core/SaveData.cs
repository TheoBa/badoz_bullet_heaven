using System;
using System.Collections.Generic;

namespace BulletHeaven.Core
{
    [Serializable]
    public class SaveData
    {
        public int macroResources;
        public int passiveTreePoints;
        public int unlockedTiers = 1;
        public List<PassiveAllocation> passiveTreeAllocations = new();

        // JsonUtility can't serialize Dictionary directly, hence the list-of-pairs shape.
        [Serializable]
        public class PassiveAllocation
        {
            public string nodeId;
            public int    points;
        }
    }
}
