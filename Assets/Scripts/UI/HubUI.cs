using UnityEngine;
using BulletHeaven.Core;
using BulletHeaven.Meta;
using BulletHeaven.Player;

namespace BulletHeaven.UI
{
    // Placeholder Hub screen: passive tree panel (left) + resources/Start Run (right).
    public class HubUI : MonoBehaviour
    {
        [SerializeField] private PassiveTreeData passiveTree;

        private static readonly StatType[] Categories =
        {
            StatType.MaxHP, StatType.Speed, StatType.DamageMultiplier,
            StatType.XPGainMultiplier, StatType.PickupRadius
        };

        void OnGUI()
        {
            if (SaveManager.Instance == null || passiveTree == null) return;
            var save = SaveManager.Instance;

            DrawBackground();
            DrawPassiveTreePanel(save);
            DrawResourcePanel(save);
        }

        private void DrawBackground()
        {
            GUI.color = new Color(0.08f, 0.08f, 0.1f, 1f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.05f),
                alignment = TextAnchor.MiddleCenter,
                normal    = { textColor = Color.white }
            };
            GUI.Label(new Rect(0, Screen.height * 0.02f, Screen.width, Screen.height * 0.08f), "HUB", titleStyle);
        }

        private void DrawPassiveTreePanel(SaveManager save)
        {
            float panelX = Screen.width * 0.05f;
            float panelY = Screen.height * 0.15f;
            float panelW = Screen.width * 0.6f;
            float panelH = Screen.height * 0.75f;

            GUI.color = new Color(0.15f, 0.15f, 0.18f, 0.9f);
            GUI.DrawTexture(new Rect(panelX, panelY, panelW, panelH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.height * 0.025f),
                normal   = { textColor = Color.white }
            };
            var btnStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(Screen.height * 0.018f),
                wordWrap = true
            };

            float rowHeight = panelH / Categories.Length;
            for (int i = 0; i < Categories.Length; i++)
            {
                var stat = Categories[i];
                float rowY = panelY + i * rowHeight;

                GUI.Label(new Rect(panelX + 10, rowY + rowHeight * 0.35f, panelW * 0.2f, rowHeight * 0.3f),
                          stat.ToString(), labelStyle);

                for (int tier = 1; tier <= 3; tier++)
                {
                    var node = passiveTree.FindByStatTier(stat, tier);
                    if (node == null) continue;

                    bool owned  = save.IsNodeOwned(node.nodeId);
                    bool canBuy = save.CanPurchase(passiveTree, node.nodeId);

                    float btnX = panelX + panelW * 0.22f + (tier - 1) * (panelW * 0.24f);
                    float btnY = rowY + rowHeight * 0.1f;
                    float btnW = panelW * 0.22f;
                    float btnH = rowHeight * 0.8f;

                    string label = owned
                        ? $"T{tier} OWNED\n+{node.statDelta}"
                        : $"T{tier}\n+{node.statDelta}\ncost {node.cost}";

                    GUI.enabled = !owned && canBuy;
                    if (GUI.Button(MobileUI.EnsureMinSize(new Rect(btnX, btnY, btnW, btnH)), label, btnStyle))
                        save.PurchaseNode(passiveTree, node.nodeId);
                    GUI.enabled = true;
                }
            }

            var resetStyle = new GUIStyle(GUI.skin.button) { fontSize = Mathf.RoundToInt(Screen.height * 0.022f) };
            var resetRect  = MobileUI.EnsureMinSize(new Rect(panelX + 10, panelY + panelH - Screen.height * 0.06f, panelW * 0.28f, Screen.height * 0.05f));
            if (GUI.Button(resetRect, "Reset Tree (Free Respec)", resetStyle))
                save.ResetPassiveTree(passiveTree);
        }

        private void DrawResourcePanel(SaveManager save)
        {
            float panelX = Screen.width * 0.68f;
            float panelY = Screen.height * 0.15f;
            float panelW = Screen.width * 0.27f;
            float panelH = Screen.height * 0.75f;

            GUI.color = new Color(0.15f, 0.15f, 0.18f, 0.9f);
            GUI.DrawTexture(new Rect(panelX, panelY, panelW, panelH), Texture2D.whiteTexture);
            GUI.color = Color.white;

            var labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = Mathf.RoundToInt(Screen.height * 0.03f),
                alignment = TextAnchor.MiddleCenter,
                wordWrap  = true,
                normal    = { textColor = Color.white }
            };

            GUI.Label(new Rect(panelX, panelY + 20, panelW, panelH * 0.15f),
                      $"Resources: {save.Data.macroResources}", labelStyle);
            GUI.Label(new Rect(panelX, panelY + panelH * 0.2f, panelW, panelH * 0.15f),
                      $"Tiers Unlocked: {save.Data.unlockedTiers}", labelStyle);

            var startBtnStyle = new GUIStyle(GUI.skin.button) { fontSize = Mathf.RoundToInt(Screen.height * 0.035f) };
            var startRect = MobileUI.EnsureMinSize(new Rect(panelX + panelW * 0.1f, panelY + panelH * 0.75f, panelW * 0.8f, panelH * 0.15f));
            if (GUI.Button(startRect, "Start Run", startBtnStyle))
                GameManager.Instance?.StartRun();
        }
    }
}
