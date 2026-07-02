using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BulletHeaven.Core;
using BulletHeaven.Player;
using BulletHeaven.Skills;

namespace BulletHeaven.UI
{
    // Replaces LevelUpUI's placeholder Continue button with real skill selection.
    public class SkillPickUI : MonoBehaviour
    {
        [SerializeField] private SkillPool skillPool;

        private bool                _visible;
        private int                 _displayLevel;
        private int                 _pendingCount;
        private List<SkillBase>     _offered = new();
        private PlayerStatsRuntime  _playerStats;

        // Cached GUIStyles -- OnGUI runs every frame, so allocating these inline would GC every frame.
        private GUIStyle _titleStyle;
        private GUIStyle _nameStyle;
        private GUIStyle _descStyle;

        void Start()
        {
            // Start (not OnEnable) so this runs after every object's Awake() —
            // guarantees XPManager.Instance is already assigned regardless of hierarchy order.
            if (XPManager.Instance == null) return;
            XPManager.Instance.OnLevelUp += HandleLevelUp;

            var playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null) _playerStats = playerGO.GetComponent<PlayerStatsRuntime>();
        }

        void OnDestroy()
        {
            if (XPManager.Instance == null) return;
            XPManager.Instance.OnLevelUp -= HandleLevelUp;
        }

        private void HandleLevelUp(int level)
        {
            _displayLevel = level;
            _pendingCount++;
            if (!_visible) ShowNextPick();
        }

        private void ShowNextPick()
        {
            _offered       = RollOffers();
            _visible       = true;
            Time.timeScale = 0f;
        }

        private List<SkillBase> RollOffers()
        {
            var pool = skillPool != null && skillPool.skills != null ? skillPool.skills : System.Array.Empty<SkillBase>();
            var owned = GameManager.Instance?.RunData.OwnedSkills;

            var candidates = new List<SkillBase>(pool);
            bool allOwned = owned != null && owned.Count >= pool.Length;
            if (owned != null && !allOwned)
                candidates.RemoveAll(s => owned.Contains(s));

            var offers = new List<SkillBase>();
            for (int i = 0; i < 3 && candidates.Count > 0; i++)
            {
                int idx = Random.Range(0, candidates.Count);
                offers.Add(candidates[idx]);
                candidates.RemoveAt(idx);
            }
            return offers;
        }

        private void Pick(SkillBase skill)
        {
            if (_playerStats != null) skill.ApplyToPlayer(_playerStats);
            GameManager.Instance?.RunData.AddSkill(skill);

            _pendingCount--;
            if (_pendingCount > 0) { ShowNextPick(); return; } // more level-ups queued from this XP gain

            _visible       = false;
            Time.timeScale = 1f;
        }

        void OnGUI()
        {
            if (!_visible) return;

            var screenRect = new Rect(0, 0, Screen.width, Screen.height);
            GUI.color = new Color(0f, 0f, 0f, 0.7f);
            GUI.DrawTexture(screenRect, Texture2D.whiteTexture);
            GUI.color = Color.white;

            _titleStyle ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, wordWrap = true, normal = { textColor = Color.white } };
            _titleStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.05f);

            GUI.Label(new Rect(0, Screen.height * 0.1f, Screen.width, Screen.height * 0.15f),
                      $"LEVEL {_displayLevel}! Choose a skill", _titleStyle);

            if (_offered.Count == 0) return;

            float cardWidth  = Screen.width * 0.25f;
            float cardHeight = Screen.height * 0.5f;
            float spacing    = Screen.width * 0.03f;
            float totalWidth = _offered.Count * cardWidth + (_offered.Count - 1) * spacing;
            float startX     = (Screen.width - totalWidth) * 0.5f;
            float cardY       = Screen.height * 0.3f;

            _nameStyle ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter, wordWrap = true, fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
            _nameStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.035f);
            var nameStyle = _nameStyle;

            _descStyle ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter, wordWrap = true, normal = { textColor = Color.white } };
            _descStyle.fontSize = Mathf.RoundToInt(Screen.height * 0.022f);
            var descStyle = _descStyle;

            for (int i = 0; i < _offered.Count; i++)
            {
                var skill    = _offered[i];
                var cardRect = new Rect(startX + i * (cardWidth + spacing), cardY, cardWidth, cardHeight);

                GUI.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);
                GUI.DrawTexture(cardRect, Texture2D.whiteTexture);
                GUI.color = Color.white;

                GUI.Label(new Rect(cardRect.x, cardRect.y + 10, cardRect.width, cardHeight * 0.15f),
                          skill.category.ToString(), descStyle);
                GUI.Label(new Rect(cardRect.x, cardRect.y + cardHeight * 0.2f, cardRect.width, cardHeight * 0.2f),
                          skill.skillName, nameStyle);
                GUI.Label(new Rect(cardRect.x + 10, cardRect.y + cardHeight * 0.42f, cardRect.width - 20, cardHeight * 0.4f),
                          skill.description, descStyle);

                var btnRect = MobileUI.EnsureMinSize(new Rect(cardRect.x + cardWidth * 0.15f, cardRect.y + cardHeight * 0.85f,
                                       cardWidth * 0.7f, cardHeight * 0.1f));
                if (GUI.Button(btnRect, "Choose"))
                    Pick(skill);
            }
        }
    }
}
