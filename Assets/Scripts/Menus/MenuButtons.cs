using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButtons : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private TextMeshProUGUI exitText;

    [Header("Animation Settings")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverDuration = 0.2f;
    [SerializeField] private float clickScale = 0.9f;
    [SerializeField] private float clickDuration = 0.1f;

    [Header("Scene Management")]
    [SerializeField] private string gameSceneName = "GameScene";
    
    private void Start()
    {
        // Setup button listeners
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);

        // Setup hover animations
        SetupButtonAnimations(startButton, startText);
        SetupButtonAnimations(exitButton, exitText);
    }

    private void SetupButtonAnimations(Button button, TextMeshProUGUI text)
    {
        if (button == null) return;

        // Add hover event listeners using EventTrigger
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => 
        {
            button.transform.DOScale(hoverScale, hoverDuration);
            if (text != null)
                text.transform.DOScale(hoverScale, hoverDuration);
        });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => 
        {
            button.transform.DOScale(1f, hoverDuration);
            if (text != null)
                text.transform.DOScale(1f, hoverDuration);
        });
        trigger.triggers.Add(entryExit);

        // Add click animation
        button.onClick.AddListener(() => 
        {
            button.transform.DOScale(clickScale, clickDuration).OnComplete(() =>
            {
                button.transform.DOScale(1f, clickDuration);
            });
        });
    }

    private void StartGame()
    {
        // Fade out animation before scene transition
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
            {
                SceneManager.LoadScene(gameSceneName);
            });
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    private void ExitGame()
    {
        // Fade out animation before quitting
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
            {
                Application.Quit();
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
            });
        }
        else
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    private void OnDestroy()
    {
        // Clean up DOTween animations
        DOTween.Kill(startButton.transform);
        DOTween.Kill(exitButton.transform);
        if (startText != null) DOTween.Kill(startText.transform);
        if (exitText != null) DOTween.Kill(exitText.transform);
    }
}