using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        // show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region Button Methods
    public void RestartGame()
    {
        SceneManager.LoadScene("BattleScene");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void InstructionButton()
    {
 
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void BackButton()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
    #endregion
}
