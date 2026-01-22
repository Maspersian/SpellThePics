using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// Main Game Manager - Level Based
public class LetterMatchGame : MonoBehaviour
{
    [System.Serializable]
    public class LetterData
    {
        public char letter;
        public Sprite[] images; // 3 images per letter
        public string[] imageNames;
    }

    [System.Serializable]
    public class LevelConfig
    {
        public int levelNumber;
        public char[] letters; // Letters for this level
        public int gridSize; // 3 for 3x3, 6 for 6x6
    }

    [Header("UI References")]
    public Transform topLettersContainer; // Parent for letter texts
    public GameObject letterTextPrefab; // Prefab for letter display
    public Transform gridContainer; // Parent for tile buttons
    public GameObject tilePrefab; // Prefab for tile button
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI chancesText;
    public TextMeshProUGUI levelText;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject levelCompletePanel;
    public TextMeshProUGUI levelCompleteText;
    public Button nextLevelButton;
    public Button restartButton;

    [Header("Game Data")]
    public LetterData[] allLetters; // All 26 letters with images

    [Header("Level Configurations")]
    public LevelConfig[] levels; // 14 levels configured

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;
    public Color wrongColor = Color.red;

    private int currentLevel = 0;
    private char currentTargetLetter;
    private int consecutiveMatches = 0;
    private int chancesLeft = 3;
    private int totalScore = 0;
    private int levelScore = 0;
    private List<GameObject> currentTiles = new List<GameObject>();
    private List<GameObject> currentLetterTexts = new List<GameObject>();
    private List<int> selectedTiles = new List<int>();
    private bool gameOver = false;
    private Dictionary<char, LetterData> letterDict = new Dictionary<char, LetterData>();

    void Start()
    {
        SetupLevelConfigs();
        BuildLetterDictionary();
        SetupLevel(currentLevel);
    }

    void SetupLevelConfigs()
    {
        levels = new LevelConfig[14];

        // Levels 1-7: 3x3 grid with 3 letters each
        levels[0] = new LevelConfig { levelNumber = 1, gridSize = 3, letters = new char[] { 'A', 'E', 'S' } };
        levels[1] = new LevelConfig { levelNumber = 2, gridSize = 3, letters = new char[] { 'B', 'C', 'D' } };
        levels[2] = new LevelConfig { levelNumber = 3, gridSize = 3, letters = new char[] { 'F', 'G', 'H' } };
        levels[3] = new LevelConfig { levelNumber = 4, gridSize = 3, letters = new char[] { 'I', 'J', 'K' } };
        levels[4] = new LevelConfig { levelNumber = 5, gridSize = 3, letters = new char[] { 'L', 'M', 'N' } };
        levels[5] = new LevelConfig { levelNumber = 6, gridSize = 3, letters = new char[] { 'O', 'P', 'Q' } };
        levels[6] = new LevelConfig { levelNumber = 7, gridSize = 3, letters = new char[] { 'R', 'T', 'U' } };

        // Levels 8-14: 6x6 grid with 12 letters each
        levels[7] = new LevelConfig { levelNumber = 8, gridSize = 6, letters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L' } };
        levels[8] = new LevelConfig { levelNumber = 9, gridSize = 6, letters = new char[] { 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X' } };
        levels[9] = new LevelConfig { levelNumber = 10, gridSize = 6, letters = new char[] { 'A', 'C', 'E', 'G', 'I', 'K', 'M', 'O', 'Q', 'S', 'U', 'W' } };
        levels[10] = new LevelConfig { levelNumber = 11, gridSize = 6, letters = new char[] { 'B', 'D', 'F', 'H', 'J', 'L', 'N', 'P', 'R', 'T', 'V', 'X' } };
        levels[11] = new LevelConfig { levelNumber = 12, gridSize = 6, letters = new char[] { 'A', 'E', 'I', 'O', 'U', 'B', 'C', 'D', 'F', 'G', 'H', 'J' } };
        levels[12] = new LevelConfig { levelNumber = 13, gridSize = 6, letters = new char[] { 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'Y' } };
        levels[13] = new LevelConfig { levelNumber = 14, gridSize = 6, letters = new char[] { 'A', 'B', 'E', 'F', 'I', 'J', 'M', 'N', 'Q', 'R', 'U', 'V' } };
    }

    void BuildLetterDictionary()
    {
        foreach (LetterData ld in allLetters)
        {
            if (!letterDict.ContainsKey(ld.letter))
            {
                letterDict.Add(ld.letter, ld);
            }
        }
    }

    void SetupLevel(int levelIndex)
    {
        if (levelIndex >= levels.Length)
        {
            GameComplete();
            return;
        }

        ClearCurrentLevel();

        gameOver = false;
        chancesLeft = 3;
        levelScore = 0;
        consecutiveMatches = 0;
        selectedTiles.Clear();

        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
        if (levelCompletePanel) levelCompletePanel.SetActive(false);

        LevelConfig config = levels[levelIndex];

        // Display level number
        if (levelText) levelText.text = $"Level {config.levelNumber}";

        // Create letter displays at top
        CreateLetterDisplays(config.letters);

        // Create grid
        CreateGrid(config.gridSize, config.letters);

        UpdateUI();
        feedbackText.text = "Match 3 images with the same starting letter!";
        feedbackText.color = Color.black;
    }

    void CreateLetterDisplays(char[] letters)
    {
        currentLetterTexts.Clear();

        foreach (char letter in letters)
        {
            GameObject letterObj;

            if (letterTextPrefab != null)
            {
                letterObj = Instantiate(letterTextPrefab, topLettersContainer);
            }
            else
            {
                letterObj = new GameObject($"Letter_{letter}");
                letterObj.transform.SetParent(topLettersContainer);
                letterObj.AddComponent<Text>();
            }

            Text txt = letterObj.GetComponent<Text>();
            txt.text = letter.ToString();
            txt.fontSize = letters.Length <= 3 ? 60 : 40;
            txt.color = Color.black;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            RectTransform rt = letterObj.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;

            currentLetterTexts.Add(letterObj);
        }
    }

    void CreateGrid(int gridSize, char[] letters)
    {
        currentTiles.Clear();
        int totalTiles = gridSize * gridSize;

        // Configure grid layout
        GridLayoutGroup gridLayout = gridContainer.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = gridSize;

            // Adjust cell size based on grid
            float cellSize = gridSize == 3 ? 150f : 80f;
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
            gridLayout.spacing = new Vector2(10, 10);
        }

        // Create tile list (3 images per letter)
        List<TileInfo> allTiles = new List<TileInfo>();

        foreach (char letter in letters)
        {
            if (letterDict.ContainsKey(letter))
            {
                LetterData data = letterDict[letter];
                for (int i = 0; i < 3; i++)
                {
                    if (i < data.images.Length)
                    {
                        allTiles.Add(new TileInfo
                        {
                            letter = letter,
                            sprite = data.images[i],
                            name = data.imageNames[i]
                        });
                    }
                }
            }
        }

        // Shuffle tiles
        for (int i = 0; i < allTiles.Count; i++)
        {
            TileInfo temp = allTiles[i];
            int randomIndex = Random.Range(i, allTiles.Count);
            allTiles[i] = allTiles[randomIndex];
            allTiles[randomIndex] = temp;
        }

        // Create tile buttons
        for (int i = 0; i < totalTiles && i < allTiles.Count; i++)
        {
            GameObject tileObj;

            if (tilePrefab != null)
            {
                tileObj = Instantiate(tilePrefab, gridContainer);
            }
            else
            {
                tileObj = new GameObject($"Tile_{i}");
                tileObj.transform.SetParent(gridContainer);
                tileObj.AddComponent<Button>();
                GameObject imgObj = new GameObject("Image");
                imgObj.transform.SetParent(tileObj.transform);
                imgObj.AddComponent<Image>();
            }

            Button btn = tileObj.GetComponent<Button>();
            Image img = tileObj.GetComponentInChildren<Image>();

            img.sprite = allTiles[i].sprite;
            img.color = normalColor;

            RectTransform rt = tileObj.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;

            RectTransform imgRt = img.GetComponent<RectTransform>();
            imgRt.anchorMin = Vector2.zero;
            imgRt.anchorMax = Vector2.one;
            imgRt.sizeDelta = Vector2.zero;
            imgRt.localScale = Vector3.one;

            TileController tc = tileObj.GetComponent<TileController>();
            if (tc == null) tc = tileObj.AddComponent<TileController>();
            tc.letter = allTiles[i].letter;
            tc.tileName = allTiles[i].name;
            tc.isActive = true;
            tc.tileImage = img;

            int index = i;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnTileClicked(index));

            currentTiles.Add(tileObj);
        }
    }

    void ClearCurrentLevel()
    {
        foreach (GameObject tile in currentTiles)
        {
            Destroy(tile);
        }
        currentTiles.Clear();

        foreach (GameObject letter in currentLetterTexts)
        {
            Destroy(letter);
        }
        currentLetterTexts.Clear();
    }

    void OnTileClicked(int index)
    {
        if (gameOver || index >= currentTiles.Count) return;

        GameObject tileObj = currentTiles[index];
        TileController tc = tileObj.GetComponent<TileController>();

        if (!tc.isActive) return;

        char clickedLetter = tc.letter;

        if (consecutiveMatches == 0)
        {
            currentTargetLetter = clickedLetter;
            consecutiveMatches = 1;
            selectedTiles.Add(index);

            tc.tileImage.color = selectedColor;
            feedbackText.text = $"Good! Keep selecting '{currentTargetLetter}' images!";
            feedbackText.color = Color.blue;
        }
        else if (clickedLetter == currentTargetLetter)
        {
            consecutiveMatches++;
            selectedTiles.Add(index);
            tc.tileImage.color = selectedColor;

            if (consecutiveMatches == 3)
            {
                StartCoroutine(CompleteMatch());
            }
            else
            {
                feedbackText.text = $"Great! {consecutiveMatches}/3 - One more!";
                feedbackText.color = Color.blue;
            }
        }
        else
        {
            chancesLeft--;
            feedbackText.text = $"Oops! That's '{clickedLetter}', not '{currentTargetLetter}'!";
            feedbackText.color = Color.red;

            StartCoroutine(ShowWrongSelection(index));
            ResetSelection();

            if (chancesLeft <= 0)
            {
                GameOver(false);
            }
            else
            {
                UpdateUI();
            }
        }
    }

    IEnumerator CompleteMatch()
    {
        int points = levels[currentLevel].gridSize == 3 ? 10 : 5;
        levelScore += points;
        totalScore += points;
        feedbackText.text = $"Perfect! +{points} points!";
        feedbackText.color = Color.green;

        foreach (int index in selectedTiles)
        {
            StartCoroutine(AnimateTile(index));
        }

        yield return new WaitForSeconds(1f);

        foreach (int index in selectedTiles)
        {
            TileController tc = currentTiles[index].GetComponent<TileController>();
            tc.isActive = false;
            tc.tileImage.color = new Color(1, 1, 1, 0.3f);
            currentTiles[index].GetComponent<Button>().interactable = false;
        }

        ResetSelection();
        UpdateUI();

        bool allCleared = true;
        foreach (GameObject tileObj in currentTiles)
        {
            if (tileObj.GetComponent<TileController>().isActive)
            {
                allCleared = false;
                break;
            }
        }

        if (allCleared)
        {
            LevelComplete();
        }
        else
        {
            feedbackText.text = "Choose another letter!";
            feedbackText.color = Color.black;
        }
    }

    IEnumerator ShowWrongSelection(int index)
    {
        currentTiles[index].GetComponent<TileController>().tileImage.color = wrongColor;
        yield return new WaitForSeconds(0.5f);
        currentTiles[index].GetComponent<TileController>().tileImage.color = normalColor;
    }

    IEnumerator AnimateTile(int index)
    {
        Transform t = currentTiles[index].transform;
        Vector3 originalScale = t.localScale;

        float time = 0;
        while (time < 0.3f)
        {
            t.localScale = originalScale * (1 + time);
            time += Time.deltaTime * 2;
            yield return null;
        }

        time = 0;
        while (time < 0.3f)
        {
            t.localScale = originalScale * (1.6f - time * 2);
            time += Time.deltaTime * 2;
            yield return null;
        }

        t.localScale = originalScale;
    }

    void ResetSelection()
    {
        foreach (int index in selectedTiles)
        {
            if (currentTiles[index].GetComponent<TileController>().isActive)
            {
                currentTiles[index].GetComponent<TileController>().tileImage.color = normalColor;
            }
        }
        selectedTiles.Clear();
        consecutiveMatches = 0;
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = $"Score: {totalScore}";
        if (chancesText) chancesText.text = $"Chances: {chancesLeft}";
    }

    void LevelComplete()
    {
        gameOver = true;

        if (levelCompletePanel)
        {
            levelCompletePanel.SetActive(true);
            if (levelCompleteText)
            {
                levelCompleteText.text = $"Level {currentLevel + 1} Complete!\nScore: {levelScore}";
            }
        }

        feedbackText.text = "Level Complete! 🎉";
        feedbackText.color = Color.green;
    }

    void GameOver(bool won)
    {
        gameOver = true;
        if (won)
        {
            if (winPanel) winPanel.SetActive(true);
            feedbackText.text = "YOU WIN! 🎉";
            feedbackText.color = Color.green;
        }
        else
        {
            if (losePanel) losePanel.SetActive(true);
            feedbackText.text = "Game Over! Try Again!";
            feedbackText.color = Color.red;
        }
    }

    void GameComplete()
    {
        gameOver = true;
        if (winPanel)
        {
            winPanel.SetActive(true);
            Text winText = winPanel.GetComponentInChildren<Text>();
            if (winText) winText.text = $"ALL 14 LEVELS COMPLETE!\nTotal Score: {totalScore}";
        }
        feedbackText.text = "🎉 GAME COMPLETE! 🎉";
        feedbackText.color = Color.white;
    }

    public void NextLevel()
    {
        currentLevel++;
        SetupLevel(currentLevel);
    }

    public void RestartLevel()
    {
        SetupLevel(currentLevel);
    }

    public void RestartGame()
    {
        currentLevel = 0;
        totalScore = 0;
        SetupLevel(currentLevel);
    }

    private class TileInfo
    {
        public char letter;
        public Sprite sprite;
        public string name;
    }
}

public class TileController : MonoBehaviour
{
    public char letter;
    public string tileName;
    public bool isActive = true;
    public Image tileImage;
}