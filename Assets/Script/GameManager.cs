using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour 
{
    public int puzzile1Selected = 0;
    public int puzzile2Selected = 0;
    public int puzzile3Selected = 0;
    public bool isPuzzile1Selected = false;
    public bool isPuzzile2Selected = false;
    public bool isPuzzile3Selected = false;
    public bool anyButtonPressed = false;
    public int anyButtonpress = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayerImagePress(int imageNum)
    {
        Debug.Log("Player Image Pressed: " + imageNum);
        // Disable the clicked button
        EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>().interactable = false;

        if (imageNum == 0 && isPuzzile1Selected == false)
        {
            Debug.Log("Image 1 Selected");
            puzzile1Selected += 1;
            Debug.Log(puzzile1Selected);
        }
        else if (imageNum == 1 && isPuzzile2Selected == false)
        {
            Debug.Log("Image 2 Selected");
            puzzile2Selected += 1;
            Debug.Log(puzzile2Selected);
        }
        else if (imageNum == 2 && isPuzzile3Selected == false)
        {
            Debug.Log("Image 3 Selected");
            puzzile3Selected += 1;
            Debug.Log(puzzile3Selected);
        }
        anyButtonpress += 1;
        int select1 = puzzile1Selected;
        int select2 = puzzile2Selected;
        int select3 = puzzile3Selected;
        if (anyButtonpress == 3 && puzzile1Selected==3 || anyButtonpress == 3 && puzzile2Selected == 3|| anyButtonpress == 3 && puzzile3Selected == 3)
        {
            anyButtonpress = 0;
            anyButtonPressed = true;
            Debug.Log("3 Images Selected");
            if(puzzile1Selected == 3)
            {
                isPuzzile1Selected = true;
                Debug.Log("Puzzile 1 Completed");
            }
            if(puzzile2Selected == 3)
            {
                isPuzzile2Selected = true;
                Debug.Log("Puzzile 2 Completed");
            }
            if(puzzile3Selected == 3)
            {
                isPuzzile3Selected = true;
                Debug.Log("Puzzile 3 Completed");
            }
        }
        else if(anyButtonpress == 3)
        {
            anyButtonpress = 0;
            Debug.Log("Resetting Selection");
            puzzile1Selected = 0;
            puzzile2Selected = 0;
            puzzile3Selected = 0;
            isPuzzile1Selected = false;
            isPuzzile2Selected = false;
            isPuzzile3Selected = false;
        }
    }
}
