// ============================================
// GameItem.cs - Data class for game items
// ============================================
using System;
using UnityEngine;

[Serializable]
public class GameItem
{
    public char letter;
    public string itemName;
    public Sprite itemSprite; // Assign in inspector or load from Resources
    public string uniqueId;

    public GameItem(char letter, string itemName, Sprite sprite)
    {
        this.letter = letter;
        this.itemName = itemName;
        this.itemSprite = sprite;
        this.uniqueId = Guid.NewGuid().ToString();
    }
}

