// ============================================
// LetterData.cs - ScriptableObject for letter data
// ============================================
//using UnityEngine;

using UnityEngine;

[CreateAssetMenu(fileName = "LetterData", menuName = "Game/Letter Data")]
public class LetterData : ScriptableObject
{
    public char letter;
    public Sprite[] itemSprites; // 3 sprites for each letter
    public string[] itemNames;   // 3 names for each letter
}