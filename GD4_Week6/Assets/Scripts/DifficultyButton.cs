using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    private Button button;
    [SerializeField] int difficulty;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetDifficulty);
    }

    void SetDifficulty()
    {
        //Save the difficulty and load the game
        PlayerPrefs.SetInt("Difficulty", difficulty);
        SceneManager.LoadScene("GameScene");
    }
}
