// ============================================
// AlphabetMatchGame.cs - Main game manager
// ============================================
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlphabetMatchGame : MonoBehaviour
{
    [Header("Letter Data")]
    public LetterData[] allLetterData; // Assign 26 letter data in inspector

    [Header("UI References")]
    public Transform letterDisplayParent; // Parent for 3 letter displays
    public TextMeshProUGUI[] letterTexts; // 3 text elements for letters
    public GameObject[] letterBackgrounds; // 3 backgrounds for letters

    public Transform itemGridParent; // Parent for 9 item buttons
    public GameObject itemButtonPrefab; // Prefab with ItemButton component

    public TextMeshProUGUI messageText;
    public TextMeshProUGUI scoreText;
    public GameObject[] liveHearts; // 3 heart images

    public Button restartButton;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("Colors")]
    public Color normalLetterColor = Color.blue;
    public Color activeLetterColor = Color.yellow;
    public Color completedLetterColor = Color.green;

    // Game state
    private List<char> selectedLetters = new List<char>();
    private List<GameItem> gameItems = new List<GameItem>();
    private List<ItemButton> itemButtons = new List<ItemButton>();
    private List<GameItem> selectedItems = new List<GameItem>();
    private List<ItemButton> selectedButtons = new List<ItemButton>();

    private char? currentLetter = null;
    private int lives = 3;
    private int score = 0;
    private bool gameOver = false;
    private List<char> completedLetters = new List<char>();

    private void Start()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(StartNewGame);

        StartNewGame();
    }

    public void StartNewGame()
    {
        // Clear previous game
        ClearGame();

        // Select 3 random letters
        List<LetterData> shuffledLetters = allLetterData.ToList();
        shuffledLetters = shuffledLetters.OrderBy(x => Random.value).ToList();
        selectedLetters = shuffledLetters.Take(3).Select(ld => ld.letter).ToList();

        // Create game items
        gameItems.Clear();
        foreach (char letter in selectedLetters)
        {
            LetterData letterData = allLetterData.FirstOrDefault(ld => ld.letter == letter);
            if (letterData != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i < letterData.itemSprites.Length && i < letterData.itemNames.Length)
                    {
                        GameItem item = new GameItem(letter, letterData.itemNames[i], letterData.itemSprites[i]);
                        gameItems.Add(item);
                    }
                }
            }
        }

        // Shuffle items
        gameItems = gameItems.OrderBy(x => Random.value).ToList();

        // Initialize UI
        InitializeLetterDisplay();
        InitializeItemGrid();

        // Reset game state
        selectedItems.Clear();
        selectedButtons.Clear();
        currentLetter = null;
        lives = 3;
        score = 0;
        gameOver = false;
        completedLetters.Clear();

        UpdateUI();
        UpdateMessage("Match items to their starting letters!");

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    private void ClearGame()
    {
        foreach (ItemButton btn in itemButtons)
        {
            if (btn != null)
                Destroy(btn.gameObject);
        }
        itemButtons.Clear();
    }

    private void InitializeLetterDisplay()
    {
        for (int i = 0; i < letterTexts.Length && i < selectedLetters.Count; i++)
        {
            letterTexts[i].text = selectedLetters[i].ToString();
            if (letterBackgrounds[i] != null)
            {
                Image bg = letterBackgrounds[i].GetComponent<Image>();
                if (bg != null)
                    bg.color = normalLetterColor;
            }
        }
    }

    private void InitializeItemGrid()
    {
        foreach (GameItem item in gameItems)
        {
            GameObject btnObj = Instantiate(itemButtonPrefab, itemGridParent);
            ItemButton itemBtn = btnObj.GetComponent<ItemButton>();

            if (itemBtn != null)
            {
                itemBtn.Initialize(item, this);
                itemButtons.Add(itemBtn);
            }
        }
    }

    public void OnItemClicked(GameItem item, ItemButton button)
    {
        Debug.Log("ItemBtnClicked");
        if (gameOver || completedLetters.Contains(item.letter))
            return;

        // NEW: Check if this button is already selected (prevent duplicate selections)
        if (selectedButtons.Contains(button))
        {
            Debug.Log("Button already selected, ignoring duplicate click");
            return;
        }

        // Debug.Log("Not Return");
        // First selection - set the current letter
        if (selectedItems.Count == 0)
        {
            currentLetter = item.letter;
            selectedItems.Add(item);
            selectedButtons.Add(button);
            button.SetSelected(true);
            UpdateMessage($"Great! Keep selecting items that start with \"{item.letter}\"");
            UpdateLetterDisplay();
            return;
        }

        // Check if clicked item matches current letter
        if (item.letter == currentLetter)
        {
            selectedItems.Add(item);
            selectedButtons.Add(button);
            button.SetSelected(true);

            // Check if all 3 items for this letter are selected
            if (selectedItems.Count == 3)
            {
                StartCoroutine(CompleteLetterSequence());
            }
            else
            {
                int remaining = 3 - selectedItems.Count;
                UpdateMessage($"Awesome! {remaining} more \"{currentLetter}\" item{(remaining > 1 ? "s" : "")} to go!");
            }
        }
        else
        {
            // Wrong selection
            StartCoroutine(WrongSelectionSequence(item.letter));
        }
    }

    private IEnumerator CompleteLetterSequence()
    {
        score++;
        completedLetters.Add(currentLetter.Value);

        UpdateMessage($"Perfect! You matched all \"{currentLetter}\" items! 🎉");

        yield return new WaitForSeconds(0.5f);

        // Mark as completed
        foreach (ItemButton btn in selectedButtons)
        {
            btn.SetCompleted();
        }

        selectedItems.Clear();
        selectedButtons.Clear();
        currentLetter = null;

        UpdateUI();
        UpdateLetterDisplay();

        // Check if game is won
        if (completedLetters.Count == selectedLetters.Count)
        {
            yield return new WaitForSeconds(0.5f);
            WinGame();
        }
    }

    private IEnumerator WrongSelectionSequence(char wrongLetter)
    {
        lives--;
        UpdateMessage($"Oops! That starts with \"{wrongLetter}\", not \"{currentLetter}\". Try again!");

        yield return new WaitForSeconds(1f);

        // Deselect all
        foreach (ItemButton btn in selectedButtons)
        {
            btn.SetSelected(false);
        }

        selectedItems.Clear();
        selectedButtons.Clear();
        currentLetter = null;

        UpdateUI();
        UpdateLetterDisplay();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    private void UpdateUI()
    {
        // Update score
        if (scoreText != null)
            scoreText.text = $"Score: {score}/3";

        // Update lives
        for (int i = 0; i < liveHearts.Length; i++)
        {
            if (liveHearts[i] != null)
                liveHearts[i].SetActive(i < lives);
        }
    }

    private void UpdateLetterDisplay()
    {
        for (int i = 0; i < letterTexts.Length && i < selectedLetters.Count; i++)
        {
            char letter = selectedLetters[i];
            Image bg = letterBackgrounds[i].GetComponent<Image>();
            Debug.Log(bg + "bg");

            if (bg != null)
            {
                if (completedLetters.Contains(letter))
                {
                    bg.color = completedLetterColor;
                    Debug.Log("Complete lettercolor");
                }
                else if (currentLetter.HasValue && currentLetter.Value == letter)
                {
                    bg.color = activeLetterColor; 
                    Debug.Log("Active lettercolor");
                }
                else
                {
                    bg.color = normalLetterColor;
                    Debug.Log("Normal lettercolor");
                }
            }
        }
    }

    private void UpdateMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }

    private void GameOver()
    {
        gameOver = true;
        UpdateMessage("Game Over! Try again! 💪");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    private void WinGame()
    {
        gameOver = true;
        UpdateMessage("🎊 You Won! Great job matching all the letters! 🎊");

        if (winPanel != null)
            winPanel.SetActive(true);
    }
}