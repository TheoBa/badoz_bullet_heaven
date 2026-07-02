# Badoz Bullet Heaven — v0 TODO

## Target goal for v0

A playable end-to-end core loop, buildable on **desktop + Android** (Unity 6, URP):

- **Hub** → spend cross-run resources on a re-assignable passive stat tree → launch run
- **Run** → auto-fire combat (nearest-enemy targeting, player only controls movement), escalating enemy waves, level-up skill picks, periodic boss waves
- **Boss kill** → win the wave tier, earn rare macro resources, unlock next tier of enemies
- **Death or all 3 tiers cleared** → return to Hub with earned resources

Locked design choices: run ends on death or after 3 tiers/3 bosses; Hub stat tree has free respec; 9 in-run skills (3 offensive / 3 defensive / 3 utility); 3 enemy types per tier; placeholder/prototype art throughout v0.

Full design detail lives in the original planning doc; this file tracks live status per the `feature/<name>` branching plan in `CLAUDE.md`.

## Done (merged to `main`)

- [x] 1. Project structure & scenes — `feature/project-structure`
- [x] 2. Dual-platform input — `feature/input-system`
- [x] 3. Game state & scene management — `feature/game-manager`
- [x] 4. Player movement — `feature/player-movement`
- [x] 5. Player stats system — `feature/player-movement` (bundled)
- [x] 6. Player death — `feature/player-movement` (bundled)
- [x] 7. Auto-fire weapon — `feature/weapon-auto-fire`
- [x] 8. Enemy base class + 3 enemy types (Walker, Tank, Charger) — `feature/wave-spawner` (bundled)
- [x] 9. Enemy AI (chase + Charger dash state machine) — `feature/wave-spawner` (bundled)
- [x] 10. XP orb + macro resource drop — `feature/wave-spawner` (bundled)
- [x] 11. Wave spawner — `feature/wave-spawner`
- [x] 12. Boss wave + tier unlock — `feature/boss-tier`
- [x] 13. XP & level-up system — `feature/xp-levelup`
  - Runtime-verified: quadratic XP curve, multi-level-up handling, `Time.timeScale` pause/resume via `LevelUpUI`
  - Found + fixed during verification: `LevelUpUI` subscribed to `XPManager.OnLevelUp` in `OnEnable()`, which could run before `XPManager.Awake()` set `Instance` — moved subscription to `Start()`
  - Known follow-up: `TierCompleteUI` uses the same `OnEnable`-subscribe pattern against `TierManager.Instance` and may have the identical latent ordering bug — not yet checked

- [x] 14. 9 in-run skills (3 offensive / 3 defensive / 3 utility) — `feature/skills`

- [x] 15. Skill pick UI (replaces `LevelUpUI`'s placeholder Continue button) — `feature/skill-pick-ui`
  - `LevelUpUI` GameObject/script removed; replaced by `SkillPickUI` in the Run scene
  - Known follow-up (still open, unrelated to this feature): `TierCompleteUI` likely has the same `OnEnable`-subscribe-before-`Awake()` ordering bug `LevelUpUI` had — confirmed identical code pattern this session, not yet fixed

- [x] 16. Persistent save system (`SaveData` / `SaveManager`) — `feature/save-system`
  - `SaveManager` added to the shared `Managers` prefab (alongside `GameManager`/`SceneLoader`/`InputReader`) so it's present in both `Hub` and `Run` scenes

- [x] 17. Passive tree (hub upgrades, free respec) — `feature/passive-tree`
  - `PassiveTreeData` asset holds 15 nodes (5 `StatType` categories x 3 tiers)

- [x] 18. Hub scene UI (passive tree panel, start run) — `feature/hub-scene`
  - **Found and fixed a real, previously-silent bug**: `Hub.unity` and `Run.unity` were never registered in Build Settings, so `SceneLoader.LoadScene` (Start Run, tier transitions, return-to-hub) failed at runtime — the run flow was silently broken since the scenes were first created. Fixed by registering both in Build Settings.

- [x] 19. Run → Hub transition / run summary screen — `feature/run-summary`
  - Closed a real gap: death previously had no screen and no way back to the Hub at all — the run just silently froze
  - `TierCompleteUI`'s `OnEnable`-subscribe-before-`Awake()` ordering bug fixed (same class as `LevelUpUI`'s, flagged as an open follow-up in three prior PRs)

- [x] 20. In-run HUD (HP bar, XP bar, wave/tier badge) — `feature/hud`
  - HUD hides only during `GameState.GameOver` (where `RunSummaryUI` takes over); visible otherwise

- [x] 23. Game over & win screens — delivered by `RunSummaryUI` in `feature/run-summary` (item 19); no separate work needed

- [x] 21. Main menu — `feature/menus`
- [x] 22. Pause menu — `feature/menus`

- [x] 24. Mobile UI adaptation (safe area, touch targets, joystick visibility) — `feature/mobile-ui`
  - **Found and built a real gap**: the on-screen virtual joystick called for by item 2 (`input-system`, marked done) was never actually implemented despite the plan requiring it. Added a `ScreenSpaceOverlay` Canvas + `OnScreenStick` in the `Run` scene, bound to `<Gamepad>/leftStick` (matching the `Move` action's existing gamepad binding, no `.inputactions` changes needed), gated to Android/iOS only via `MobileControlsVisibility`

## In review

- [ ] 25. Android build configuration — `feature/android-build`, PR open, awaiting merge
  - Package name `com.badoz.bulletheavensurvivors` (was still the URP template's placeholder identifier), target SDK 35 (min SDK 26 was already correct), portrait orientation locked (was auto-rotate, all four orientations allowed), IL2CPP scripting backend was already set, added ARMv7 alongside the existing ARM64 architecture, 512x512 placeholder icon assigned across all required Android launcher sizes

## Not started
- [ ] 26. Performance pass (object pool audit, sprite atlasing, GC) — `feature/performance-pass`
- [ ] 27. PC + Android builds (smoke test) — `feature/v0-builds`

## v0 done when

- [ ] Desktop build: launch → hub → assign passive points → start run → auto-weapons fire → waves spawn and escalate → level-up skill pick works → wave 5 boss appears → kill boss → tier 2 waves → die → hub shows earned resources
- [ ] Android APK: sideload → same flow works with virtual joystick → no <30fps during peak enemy count (~50 enemies)
- [ ] Respec: hub passive tree reset button refunds all points correctly
- [ ] Save persists: close and reopen game, resources and unlocked tiers are preserved
