using System;
using UnityEngine;

namespace BulletHeaven.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private RunData runData;

        public GameState  CurrentState   { get; private set; }
        public RunData    RunData        => runData;
        public RunResult  LastRunResult  { get; private set; }

        public event Action<GameState> OnStateChanged;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void OnEnable()
        {
            if (InputReader.Instance != null)
                InputReader.Instance.OnPausePressed += HandlePauseInput;
        }

        void OnDisable()
        {
            if (InputReader.Instance != null)
                InputReader.Instance.OnPausePressed -= HandlePauseInput;
        }

        // Called from Hub "Start Run" button
        public void StartRun()
        {
            runData.StartRun();
            SetState(GameState.InRun);
            SceneLoader.Instance.LoadScene(SceneLoader.SceneRun);
        }

        // Called by player death, tier complete, or full clear
        public void EndRun(RunResult result)
        {
            LastRunResult = result;
            SetState(GameState.GameOver);

            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.Data.macroResources += runData.ResourcesEarnedThisRun;
                SaveManager.Instance.Data.unlockedTiers = Math.Max(SaveManager.Instance.Data.unlockedTiers, runData.CurrentTier);
                SaveManager.Instance.Save();
            }
            // Scene transition happens after the end screen is dismissed (handled in UI)
        }

        public void ReturnToHub()
        {
            SetState(GameState.Hub);
            SceneLoader.Instance.LoadScene(SceneLoader.SceneHub);
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.InRun && CurrentState != GameState.LevelUp) return;
            Time.timeScale = 0f;
            SetState(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            Time.timeScale = 1f;
            SetState(GameState.InRun);
        }

        // Pause toggle used during a run; ignored in hub/menus
        private void HandlePauseInput()
        {
            if (CurrentState == GameState.InRun)   PauseGame();
            else if (CurrentState == GameState.Paused) ResumeGame();
        }

        private void SetState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }
    }
}
