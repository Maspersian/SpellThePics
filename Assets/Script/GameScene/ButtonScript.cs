using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameOverBn()
    {
        //----------Load Same scene(*New Game*)---------------
        SceneManager.LoadScene(1);
    }
    public void BackHome()
    {
        SceneManager.LoadScene(0);
    }
}
