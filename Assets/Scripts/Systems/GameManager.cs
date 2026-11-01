
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Ready,
        Intro,
        Playing,
        WaveClear,
        FinalWave,
        GameOver,
        GameClear
    }


    [Header("References")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private MonsterSpawner monsterSpawner;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private UIController uiController;
    [SerializeField] private FinalWaveUI finalWaveUI;
    [SerializeField] private Gun gun;

    [SerializeField] private GameObject safeZoneBlock1;
    [SerializeField] private GameObject safeZoneBlock2;


    private Coroutine finalWaveIntroCoroutine;


    public GameState CurrentState { get; private set; }

    public event Action<GameState> OnGameStateChanged;


    private void OnEnable()
    {
        playerHealth.OnDead += playerController.Die;
        playerHealth.OnDead += GameOver;

        monsterSpawner.OnWaveCleared += WaveCleared;
        monsterSpawner.OnFinalWaveReady += StartFinalWave;
        monsterSpawner.OnFinalTimerChanged += finalWaveUI.UpdateFinalTimer;
        monsterSpawner.OnFinalClear += FinalClear;

        uiController.OnPerkSelected += HandlePerkSelected;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        ChangeState(GameState.Ready);
    }

    private void Update()
    {
        if (CurrentState == GameState.Ready && Input.GetKeyDown(KeyCode.A))
        {
            StartGame();
        }

        if (CurrentState == GameState.GameClear && Input.GetKeyDown(KeyCode.A))
        {
            ExitGame();
        }
    }

    private void OnDisable()
    {
        playerHealth.OnDead -= playerController.Die;
        playerHealth.OnDead -= GameOver;

        monsterSpawner.OnWaveCleared -= WaveCleared;
        monsterSpawner.OnFinalWaveReady -= StartFinalWave;
        monsterSpawner.OnFinalTimerChanged -= finalWaveUI.UpdateFinalTimer;
        monsterSpawner.OnFinalClear -= FinalClear;

        uiController.OnPerkSelected -= HandlePerkSelected;
    }


    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        OnGameStateChanged?.Invoke(newState);

        switch (CurrentState)
        {
            case GameState.Ready:
                gun.DisableGun();
                uiController.SetCursorLocked(true);
                uiController.ShowReadyUI();
                break;

            case GameState.Intro:
                gun.DisableGun();
                uiController.SetCursorLocked(true);
                uiController.ShowIntroUI();
                break;

            case GameState.Playing:
                gun.EnableGun();
                uiController.SetCursorLocked(false);
                uiController.ShowPlayingUI();
                break;

            case GameState.WaveClear:
                gun.DisableGun();
                playerController.StopAction();
                uiController.SetCursorLocked(false);
                uiController.ShowPerkSelectUI();
                break;

            case GameState.FinalWave:
                gun.EnableGun();
                uiController.SetCursorLocked(false);
                uiController.ShowFinalWaveUI();
                break;

            case GameState.GameOver:
                gun.DisableGun();
                uiController.SetCursorLocked(true);
                break;

            case GameState.GameClear:
                gun.DisableGun();
                uiController.SetCursorLocked(false);
                break;
        }
    }


    public void StartGame()
    {
        if (CurrentState != GameState.Ready)
        {
            return;
        }

        ChangeState(GameState.Intro);

        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(cameraController.PlayStartCamera());

        ChangeState(GameState.Playing);

        monsterSpawner.StartWave();
    }


    private void WaveCleared(int wave)
    {
        ChangeState(GameState.WaveClear);
    }

    private void HandlePerkSelected(UIController.PerkType perk)
    {
        if (CurrentState != GameState.WaveClear)
        {
            return;
        }

        switch (perk)
        {
            case UIController.PerkType.HpUp:
                playerHealth.Heal(20);
                break;

            case UIController.PerkType.AmmoUp:
                gun.IncreaseMagCapacity(5);
                break;

            case UIController.PerkType.SpeedUp:
                playerController.IncreaseMoveSpeed(0.5f);
                break;
        }

        uiController.HidePerkSelectUI();

        playerInput.BlockFireUntilRelease();

        ChangeState(GameState.Playing);

        monsterSpawner.StartWave();
    }


    private void StartFinalWave()
    {
        finalWaveIntroCoroutine = StartCoroutine(FinalWaveIntroRoutine());
    }

    private IEnumerator FinalWaveIntroRoutine()
    {
        uiController.ShowWinUI();

        safeZoneBlock1.SetActive(false);
        safeZoneBlock2.SetActive(false);

        ChangeState(GameState.FinalWave);

        monsterSpawner.StartWave();

        yield return new WaitForSeconds(3f);

        if (CurrentState != GameState.FinalWave)
        {
            yield break;
        }

        uiController.HideWinUI();

        finalWaveIntroCoroutine = null;
    }

    private void FinalClear()
    {
        GameClear();
    }


    private void GameOver()
    {
        if (CurrentState == GameState.GameOver || CurrentState == GameState.GameClear)
        {
            return;
        }

        if (finalWaveIntroCoroutine != null)
        {
            StopCoroutine(finalWaveIntroCoroutine);
            finalWaveIntroCoroutine = null;
        }

        ChangeState(GameState.GameOver);

        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        monsterSpawner.StopSpawning();

        uiController.HideHUD();
        uiController.HideWinUI();
        uiController.HideClearUI();

        yield return StartCoroutine(playerController.WaitForDeathAnimation());

        yield return StartCoroutine(cameraController.PlayGameOverCamera());

        uiController.ShowGameOverUI();

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A));

        RestartScene();
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    private void GameClear()
    {
        if (CurrentState == GameState.GameClear || CurrentState == GameState.GameOver)
        {
            return;
        }

        ChangeState(GameState.GameClear);

        monsterSpawner.StopSpawning();

        uiController.HideHUD();
        uiController.HideWinUI();
        uiController.ShowClearUI();

        Time.timeScale = 0f;
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
