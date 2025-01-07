using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Slider slider;
    public TMP_Text sliderText;
    public AudioMixer audioMixer;
    public GameObject musicPrefab;
    public TMP_Text titleText;
    public TMP_Text highScoreText;
    // Start is called before the first frame update
    void Start()
    {
        //If music not present, spawn music
        GameObject music = GameObject.FindWithTag("Music");
        if (!music)
        {
            music = Instantiate(musicPrefab, transform.position, Quaternion.identity);
        }

        //Show high score
        int highscore = 0;
        if (PlayerPrefs.HasKey("HighScore"))
        {
            highscore = PlayerPrefs.GetInt("HighScore");
        }
        highScoreText.text = "High Score: " + highscore;
        //Set coloured titles (Fruit Binger)
        titleText.text = "<color=green>F</color><color=red>r</color><color=yellow>u</color><color=purple>i</color><color=orange>t</color> <color=green>B</color><color=red>i</color><color=yellow>n</color><color=purple>g</color><color=orange>e</color><color=green>r</color>";
        
        //If player has changed volume before, load the saved volume
        if (PlayerPrefs.HasKey("masterVol"))
        {
            float volumeToSet = PlayerPrefs.GetFloat("masterVol");
            slider.value = volumeToSet;
            sliderText.text = "Sound Level: " + volumeToSet;
            float audioVol = ((volumeToSet / 100) * 80) - 80f;
            audioMixer.SetFloat("masterVol", audioVol);
        }
    }

    public void UpdateSlider()
    {
        //Change sound settings when sound slider moved
        sliderText.text = "Sound Level: " + slider.value;
        float audioVol = ((slider.value / 100) * 80) - 80f;
        audioMixer.SetFloat("masterVol", audioVol);
        PlayerPrefs.SetFloat("masterVol", slider.value);
    }

    public void PlayGame()
    {
        //Load the game!
        SceneManager.LoadScene("GameScene");
    }

    public void HowToPlay()
    {
        //Load the game!
        SceneManager.LoadScene("HowToPlay");
    }
}
