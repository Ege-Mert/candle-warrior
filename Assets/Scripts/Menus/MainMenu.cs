using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Title Settings")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private float titlePunchScale = 1.2f;
    [SerializeField] private float titleAnimDuration = 0.5f;
    
    [Header("Menu Buttons")]
    [SerializeField] private RectTransform buttonsContainer;
    [SerializeField] private float buttonStaggerDelay = 0.1f;
    [SerializeField] private float buttonAnimDuration = 0.3f;
    [SerializeField] private float buttonHoverScale = 1.1f;
    
    [Header("Background")]
    [SerializeField] private Image backgroundDim;
    [SerializeField] private float dimAlpha = 0.7f;
    
    [Header("Scene Management")]
    [SerializeField] private string gameSceneName = "GameScene";
    
    private Button[] menuButtons;
    private Sequence startupSequence;

    private void Awake()
    {
        // Get all buttons in the container
        menuButtons = buttonsContainer.GetComponentsInChildren<Button>();
        
        // Initialize background dim
        if (backgroundDim != null)
        {
            Color dimColor = backgroundDim.color;
            dimColor.a = 0;
            backgroundDim.color = dimColor;
        }
        
        // Reset initial states
        if (titleText != null)
            titleText.transform.localScale = Vector3.zero;
            
        if (buttonsContainer != null)
        {
            buttonsContainer.localPosition = new Vector3(Screen.width, buttonsContainer.localPosition.y, 0);
            foreach (Button btn in menuButtons)
            {
                btn.transform.localScale = Vector3.zero;
            }
        }
    }

    private void Start()
    {
        PlayStartupAnimation();
    }

    private void PlayStartupAnimation()
    {
        startupSequence = DOTween.Sequence();

        // Fade in background dim
        if (backgroundDim != null)
        {
            startupSequence.Join(backgroundDim.DOFade(dimAlpha, 1f).From(0));
        }

        // Animate title
        if (titleText != null)
        {
            startupSequence.Append(titleText.transform.DOScale(1, titleAnimDuration).From(0)
                .SetEase(Ease.OutBack));
            startupSequence.Append(titleText.transform.DOPunchScale(Vector3.one * titlePunchScale, titleAnimDuration, 1, 0.5f));
        }

        // Slide in buttons container
        if (buttonsContainer != null)
        {
            startupSequence.Append(buttonsContainer.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack));
            
            // Pop in each button with stagger
            for (int i = 0; i < menuButtons.Length; i++)
            {
                int index = i; // Capture for lambda
                startupSequence.Insert(1f + (index * buttonStaggerDelay), 
                    menuButtons[index].transform.DOScale(1, buttonAnimDuration)
                    .From(0).SetEase(Ease.OutBack));
            }
        }
    }


    private void PlayButtonHoverSound()
    {
        // TODO: Add sound effect here
    }

    // Button click handlers
    public void OnPlayClicked()
    {
        // Animate out and load game scene
        DOTween.Sequence()
            .Append(buttonsContainer.DOScale(0, 0.3f))
            .Join(titleText.DOFade(0, 0.3f))
            .Join(backgroundDim.DOFade(1f, 0.3f))
            .OnComplete(() => SceneManager.LoadScene(gameSceneName));
    }

    public void OnSettingsClicked()
    {
        // TODO: Implement settings menu
        Debug.Log("Settings clicked!");
    }

    public void OnQuitClicked()
    {
        DOTween.Sequence()
            .Append(buttonsContainer.DOScale(0, 0.3f))
            .Join(titleText.DOFade(0, 0.3f))
            .Join(backgroundDim.DOFade(0, 0.3f))
            .OnComplete(() => Application.Quit());
    }
}

// Helper component for button hover effects