using UnityEngine;
using TMPro;

public class PlayerCurrencyManager : MonoBehaviour
{
    public static PlayerCurrencyManager Instance { get; private set; }

    [Header("Currency Settings")]
    public int currentWax = 0;

    [Header("UI Reference")]
    public TextMeshProUGUI waxText; // HUD Text element for displaying current wax

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Optionally: DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddWax(int amount)
    {
        currentWax += amount;
        UpdateUI();
    }

    public void SubtractWax(int amount)
    {
        currentWax = Mathf.Max(0, currentWax - amount);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (waxText != null)
            waxText.text = "Wax: " + currentWax;
    }
}
