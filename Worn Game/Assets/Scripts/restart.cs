using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class restart : MonoBehaviour
{
    public GameObject button;
    public GameObject toggleText;
    public GameObject toggleCounter;

    // Start is called before the first frame update
    void Start()
    {
        button.SetActive(false);
        toggleText.SetActive(false);
        toggleCounter.SetActive(true);
        Time.timeScale = 1;
    }

    public void resetScene()
    {
        Debug.Log("load scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
