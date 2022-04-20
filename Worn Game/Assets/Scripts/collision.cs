using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class collision : MonoBehaviour
{
    public GameObject button;
    public GameObject toggleText;
    public GameObject toggleCounter;
    public Text scoreText;
    public Text counterText;
    int score;
    void Start()
    {
        score = 0; 
        counterText.text = "Score: "+score.ToString();
        scoreText.text = "You scored: "+score.ToString();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "spider(Clone)")
        {

            button.SetActive(true);
            toggleText.SetActive(true);
            toggleCounter.SetActive(false);
            
            Time.timeScale = 0;
        }
        if (collision.gameObject.name == "Apple(Clone)")
        {
            Destroy(collision.gameObject);
            score+=10;
            counterText.text = "Score: "+score.ToString();
            scoreText.text = "You scored: "+score.ToString();
        }
    }
}
