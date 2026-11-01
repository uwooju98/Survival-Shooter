
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class UIController : MonoBehaviour
{
    public enum PerkType
    {
        HpUp,
        AmmoUp,
        SpeedUp
    }


    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Gun gun;

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject fakeWinPanel;
    [SerializeField] private GameObject finalWavePanel;
    [SerializeField] private GameObject perkSelectPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameClearPanel;

    [Header("Reload UI")]
    [SerializeField] private GameObject reloadUI;
    [SerializeField] private Image reloadBar;

    [Header("HP UI")]
    [SerializeField] private Image hpBar;

    [Header("Wave UI")]
    [SerializeField] private GameObject wavePanel;

    [Header("UI Fade")]
    [SerializeField] private CanvasGroup hudCanvasGroup;
    [SerializeField] private float hudFadeDuration = 1f;

    [SerializeField] private CanvasGroup perkCanvasGroup;
    [SerializeField] private float perkFadeDuration = 0.5f;

    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private float gameOverFadeDuration = 0.5f;

    [FormerlySerializedAs("clearCanvasGroup")]
    [SerializeField] private CanvasGroup gameClearCanvasGroup;
    [FormerlySerializedAs("clearFadeDuration")]
    [SerializeField] private float gameClearFadeDuration = 0.5f;

    [FormerlySerializedAs("winCanvasGroup")]
    [SerializeField] private CanvasGroup fakeWinCanvasGroup;
    [FormerlySerializedAs("winFadeDuration")]
    [SerializeField] private float fakeWinFadeDuration = 0.5f;

    [Header("Cursor")]
    [SerializeField] private Texture2D cursor;

    [Header("Hit Effect")]
    [SerializeField] private Image hitEffect;


    private Coroutine reloadCoroutine;
    private Coroutine hitEffectCoroutine;


    public event Action<PerkType> OnPerkSelected;


    private void OnEnable()
    {
        gun.OnReloadStarted += StartReloadUI;
        gun.OnReloadFinished += EndReloadUI;

        playerHealth.OnHpChanged += UpdateHpUI;
        playerHealth.OnDamaged += ShowHitEffect;

        UpdateHpUI(playerHealth.CurrentHp, playerHealth.MaxHp);
    }

    private void OnDisable()
    {
        gun.OnReloadStarted -= StartReloadUI;
        gun.OnReloadFinished -= EndReloadUI;

        playerHealth.OnHpChanged -= UpdateHpUI;
        playerHealth.OnDamaged -= ShowHitEffect;
    }


    public void ShowReadyUI()
    {
        startPanel.SetActive(true);
        hudPanel.SetActive(false);
        hudCanvasGroup.alpha = 0f;

        gameOverPanel.SetActive(false);
        fakeWinPanel.SetActive(false);
        gameClearPanel.SetActive(false);
        perkSelectPanel.SetActive(false);

        wavePanel.SetActive(true);
        finalWavePanel.SetActive(false);

        reloadUI.SetActive(false);
        reloadBar.fillAmount = 0f;
    }

    public void ShowIntroUI()
    {
        startPanel.SetActive(false);
        hudPanel.SetActive(false);
    }

    public void ShowPlayingUI()
    {
        startPanel.SetActive(false);

        if (!hudPanel.activeSelf)
        {
            ShowHUDFadeIn();
        }
    }

    public void ShowPerkSelectUI()
    {
        perkSelectPanel.SetActive(true);

        StartCoroutine(FadeIn(perkCanvasGroup, perkFadeDuration));
    }

    public void HidePerkSelectUI()
    {
        perkSelectPanel.SetActive(false);
    }

    public void ShowFinalWaveUI()
    {
        wavePanel.SetActive(false);
        finalWavePanel.SetActive(true);
    }

    public void ShowWinUI()
    {
        fakeWinPanel.SetActive(true);

        StartCoroutine(FadeIn(fakeWinCanvasGroup, fakeWinFadeDuration));
    }

    public void HideWinUI()
    {
        StartCoroutine(FadeOut(fakeWinCanvasGroup, fakeWinFadeDuration));
    }

    public void ShowGameOverUI()
    {
        gameOverPanel.SetActive(true);

        StartCoroutine(FadeIn(gameOverCanvasGroup, gameOverFadeDuration));
    }

    public void ShowClearUI()
    {
        gameClearPanel.SetActive(true);

        StartCoroutine(FadeIn(gameClearCanvasGroup, gameClearFadeDuration));
    }

    public void HideClearUI()
    {
        gameClearPanel.SetActive(false);
    }

    public void HideHUD()
    {
        hudPanel.SetActive(false);
        finalWavePanel.SetActive(false);

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        reloadUI.SetActive(false);
    }


    public void SelectHpUp()
    {
        SelectPerk(PerkType.HpUp);
    }

    public void SelectAmmoUp()
    {
        SelectPerk(PerkType.AmmoUp);
    }

    public void SelectSpeedUp()
    {
        SelectPerk(PerkType.SpeedUp);
    }

    private void SelectPerk(PerkType perk)
    {
        OnPerkSelected?.Invoke(perk);
    }


    private void StartReloadUI(float reloadTime)
    {
        reloadUI.SetActive(true);

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
        }

        reloadBar.fillAmount = 0f;

        reloadCoroutine = StartCoroutine(CoReloadBar(reloadTime));
    }

    private IEnumerator CoReloadBar(float reloadTime)
    {
        float timer = 0f;

        reloadBar.fillAmount = 0f;

        while (timer < reloadTime)
        {
            timer += Time.deltaTime;

            reloadBar.fillAmount = timer / reloadTime;

            yield return null;
        }

        reloadBar.fillAmount = 1f;
    }

    private void EndReloadUI()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }

        reloadUI.SetActive(false);
        reloadBar.fillAmount = 0f;
    }


    private void UpdateHpUI(int currentHp, int maxHp)
    {
        hpBar.fillAmount = (float)currentHp / maxHp;
    }

    public void ShowHitEffect()
    {
        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
        }

        hitEffectCoroutine = StartCoroutine(HitEffectRoutine());
    }

    private IEnumerator HitEffectRoutine()
    {
        hitEffect.gameObject.SetActive(true);

        float duration = 0.3f;
        float elapsed = 0f;

        Color color = hitEffect.color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(0.6f, 0f, elapsed / duration);
            hitEffect.color = color;

            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = 0f;
        hitEffect.color = color;

        hitEffect.gameObject.SetActive(false);
    }


    public void SetCursorLocked(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;

        if (!isLocked)
        {
            Vector2 hotspot = new Vector2(30, 40);
            Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
        }
    }


    public void ShowHUDFadeIn()
    {
        hudPanel.SetActive(true);

        StartCoroutine(FadeIn(hudCanvasGroup, hudFadeDuration));
    }


    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, Mathf.Clamp01(elapsed / duration));

            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.gameObject.SetActive(false);
    }
}
