using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonsFun : MonoBehaviour
{
    [SerializeField] private GameObject levelPanel;
    public Button[] levelButtons;
    public Button[] puzzilImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LevelPanel()
    {
               Debug.Log("Level Panel Button Clicked");

        //------------------------ Secen Changing-----------------------
        Debug.Log("Scene Changing");
        SceneManager.LoadScene(1);
        // levelPanel.SetActive(true);
    }
    public void CloseLevelPanel()
    {
        levelPanel.SetActive(false);
    }
    public void GameStart(int levelNum)
    {
        if (levelNum == 0)
        {
            Debug.Log("Level 1 Start");
            SceneManager.LoadScene(1);
        }
        else if (levelNum == 1)
        {
            Debug.Log("Level 2 Start");
            SceneManager.LoadScene(1);
        }
        else if (levelNum == 2)
        {
            Debug.Log("Level 3 Start");
            SceneManager.LoadScene(1);
        }
    }
    
}
