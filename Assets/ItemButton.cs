// ============================================
// ItemButton.cs - Attach to each item button
// ============================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemButton : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemText;
    public Button button;
    public Image highlightBorder;

    private GameItem gameItem;
    private AlphabetMatchGame gameManager;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        if (highlightBorder != null)
            highlightBorder.gameObject.SetActive(false);
    }

    public void Initialize(GameItem item, AlphabetMatchGame manager)
    {
        Debug.Log("Initialize");
        gameItem = item;
        gameManager = manager;

        if (itemImage != null && item.itemSprite != null)
            itemImage.sprite = item.itemSprite;

        if (itemText != null)
            itemText.text = item.itemName;

        SetInteractable(true);
        SetSelected(false);
    }

    private void OnButtonClick()
    {
        if (gameManager != null)
            gameManager.OnItemClicked(gameItem, this);
    }

    public void SetSelected(bool selected)
    {
        if (highlightBorder != null)
            highlightBorder.gameObject.SetActive(selected);
    }

    public void SetCompleted()
    {
        SetInteractable(false);
        if (itemImage != null)
        {
            Color c = itemImage.color;
            c.a = 0.3f;
            itemImage.color = c;
        }
    }

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
    }

    public GameItem GetGameItem()
    {
        return gameItem;
    }
}