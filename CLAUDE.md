# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 6 (6000.5.1f1) bullet-heaven / survivor-style game using the Universal Render Pipeline (URP 17.5.0). The project is in early development — game scripts live under `Assets/` and the only scene so far is `Assets/Scenes/SampleScene.unity`.

## Unity MCP Integration

This project is configured with the **AI Game Developer MCP** (`com.ivanmurzak.unity.mcp` v0.82.2), which exposes the Unity Editor as a set of MCP tools. Claude Code connects to it in cloud mode via `.mcp.json`.

Use MCP tools (prefixed `mcp__ai-game-developer__`) to interact with the live Unity Editor instead of editing `.unity` or `.asset` files by hand:

- **Inspect/modify scenes**: `scene-get-data`, `scene-save`, `gameobject-*`, `gameobject-component-*`
- **Create/edit scripts**: `script-update-or-create`, `script-read`, `script-execute`
- **Asset operations**: `assets-find`, `assets-get-data`, `assets-modify`, `assets-prefab-*`
- **Debug**: `console-get-logs`, `screenshot-game-view`, `profiler-*`

Always call `console-get-logs` after `script-execute` or entering play mode to catch compilation errors.

## Key Packages

| Package | Version | Purpose |
|---|---|---|
| URP | 17.5.0 | Render pipeline |
| Input System | 1.19.0 | Player input (`Assets/InputSystem_Actions.inputactions`) |
| Unity Test Framework | 1.7.0 | Edit/Play mode tests |
| AI Navigation | 2.0.13 | NavMesh |
| Timeline | 1.8.12 | Cutscenes/sequences |
| uGUI | 2.5.0 | UI |
| Visual Scripting | 1.9.11 | Node-based scripting |

NuGet DLLs under `Assets/Plugins/NuGet/` are SignalR/ASP.NET dependencies bundled by the MCP plugin — do not modify them.

## Running Tests

Use the MCP tool to run tests without leaving Claude Code:

```
mcp__ai-game-developer__tests-run
```

Or use the Unity Test Runner window (Window → General → Test Runner) in the Editor for Edit Mode and Play Mode test suites.

## Git & GitHub Workflow

### Branching strategy

- `main` — always stable and shippable; never commit directly
- `feature/<kebab-name>` — one branch per feature (e.g. `feature/player-movement`, `feature/enemy-spawning`, `feature/weapon-system`)
- `fix/<kebab-name>` — for bug fixes (e.g. `fix/bullet-collision`)

Always branch from the latest `main`:

```bash
git checkout main && git pull
git checkout -b feature/<name>
```

### Commit message format

Use [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <short description>

# Types: feat | fix | refactor | chore | docs | perf | test
# Examples:
feat(player): add top-down movement with joystick support
fix(bullets): prevent double-hit on same enemy frame
chore(packages): add DOTween via Package Manager
```

Keep the subject line under 72 characters. No period at the end.

### Opening a Pull Request

When a feature branch is ready:

```bash
git push -u origin feature/<name>
gh pr create --title "<type>(<scope>): <description>" --body "$(cat <<'EOF'
## Summary
- <bullet: what this PR does>

## How to test
- [ ] Open SampleScene in Unity Editor
- [ ] Enter Play Mode and verify <behaviour>

## Notes
<anything reviewers should know — perf tradeoffs, known edge cases, follow-up tickets>
EOF
)"
```

The PR title must follow the same Conventional Commits format as commit messages.

## Code Conventions

- All game C# scripts go under `Assets/Scripts/` in feature subdirectories (e.g., `Assets/Scripts/Player/`, `Assets/Scripts/Enemies/`, `Assets/Scripts/Weapons/`).
- Use the new **Input System** (not `Input.GetKey`). The default action asset is `Assets/InputSystem_Actions.inputactions`.
- URP is the render pipeline — use URP-compatible shaders and `UniversalRenderPipelineAsset`. PC and Mobile renderer assets live in `Assets/Settings/`.
- `Assets/TutorialInfo/` is boilerplate from the URP starter template; ignore it.
