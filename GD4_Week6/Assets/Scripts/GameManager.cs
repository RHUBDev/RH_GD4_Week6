using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<GameObject> spawnObjects;
    private int lives = 3;
    private float xRange = 4.5f;
    public bool mousing = false;
    public int score = 0;
    public Volume volume;
    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;
    private Color color0 = new Color(1, 1, 1, 1);
    private Color color1 = new Color(1f, 0.5f, 0.5f, 1f);
    private Color color2 = new Color(1f, 0f, 0f, 1f);
    private Color color3 = new Color(0f, 0f, 0f, 1f);
    private int eyehit = 0;
    private int winehit = 0;
    private bool lensing = false;
    private float minDistortion = -0.8f;
    private float maxDistortion = 0.8f;
    private float speed = 1f;
    private Color usedColor;
    public GameObject[] splosionPrefabs;
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public GameObject gameOverText;
    public TMP_Text highScoreText;
    public Transform foodParent;
    public GameObject menuButton;
    public GameObject retryButton;
    public GameObject[] hearts;
    public AudioClip punchSound;
    public AudioClip popSound;
    public AudioSource audioSource;
    public ParticleSystem trail;
    private Camera cam;
    public GameObject slicesound;
    private float heartGrowRate = 1.7f;
    public GameObject pausePanel;
    private Vector2 objectSpawnRate = new Vector2(1f, 1f);
    private int difficulty = 1;
    // Start is called before the first frame update
    void Start()
    {
        difficulty = PlayerPrefs.GetInt("Difficulty");
        if(difficulty == 1)
        {
            objectSpawnRate = new Vector2(1f, 1f);
        }
        else if (difficulty == 2)
        {
            objectSpawnRate = new Vector2(0.5f, 0.7f);
        }
        else if (difficulty == 3)
        {
            objectSpawnRate = new Vector2(0.3f, 0.4f);
        }
        cam = Camera.main;
        usedColor = color0;
        //Start spawning objects
        StartCoroutine(SpawnObjects());
    }
   
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {
            //Pause if 'P' pressed
            if (Input.GetKeyDown(KeyCode.P))
            {
                Time.timeScale = 0.0f;
                pausePanel.SetActive(true);
            }

            //Get mouse button 0 input and set 'mousing' accordingly
            if (Input.GetMouseButtonDown(0))
            {
                mousing = true;
                //also start mouse trail and slice sounds
                trail.gameObject.SetActive(false);
                trail.gameObject.SetActive(true);
                var emiss = trail.emission;
                emiss.enabled = true;
                slicesound.SetActive(true);
            }
            if (Input.GetMouseButtonUp(0))
            {
                //stop mouse trail and sounds
                mousing = false;
                var emiss = trail.emission;
                emiss.enabled = false;
                slicesound.SetActive(false);
            }

            if (mousing)
            {
                //Get mouse position and set mouse trail position
                Vector3 mousepos = Input.mousePosition;
                trail.transform.parent.position = cam.ScreenToWorldPoint(new Vector3(mousepos.x, mousepos.y, 10f));
            }

            if (volume != null)
            {
                //I googled how to do these Global Volume changes
                volume.profile.TryGet(out ColorAdjustments colorAdjustments);
                if (colorAdjustments != null)
                {
                    colorAdjustments.colorFilter.value = usedColor;
                }
            }

            if (lensing)
            {
                volume.profile.TryGet(out LensDistortion lensDistortion);
                if (lensDistortion != null)
                {
                    //I googled how to loop the lens distortion
                    float t = (Mathf.Sin(Time.time * speed) + 1) * 0.5f;
                    lensDistortion.intensity.value = Mathf.Lerp(minDistortion, maxDistortion, t);
                }
            }
        }
    }

    IEnumerator SpawnObjects()
    {
        //While alive keep spawning objects
        while (lives > 0)
        {
            Instantiate(spawnObjects[Random.Range(0, spawnObjects.Count)], new Vector3(Random.Range(-xRange, xRange), -1, 0), Quaternion.identity, foodParent);
            float theWait = Random.Range(objectSpawnRate.x, objectSpawnRate.y);
            yield return new WaitForSeconds(theWait);
        }
    }

    #region HitObjectEffects
    public void HitFood(GameObject food, int splosionnum)
    {
        //spawn explosions
        float inputx = Input.GetAxis("Mouse X");
        float inputy = Input.GetAxis("Mouse Y");
        
        Quaternion splosionRotation = Quaternion.LookRotation(new Vector3(inputx, inputy,0).normalized);
        Instantiate(splosionPrefabs[splosionnum], food.transform.position, splosionRotation);
        audioSource.PlayOneShot(punchSound);
    }

    public void EyeHit()
    {
        //If hit eye, make vision redder
        if (eyehit == 0)
        {
            usedColor = color1;
        }//If hit eye a second time, make vision very red
        else if (eyehit == 1)
        {
            usedColor = color2;
        }
        eyehit++;
    }

    public void WineHit()
    {
        //I googled how to do these Global Volume changes, but typed them myself
        
        //If hit wine, increase chromatic aberration
        if (winehit == 0)
        {
            volume.profile.TryGet(out ChromaticAberration chromaticAberration);
            if (chromaticAberration != null)
            {
                chromaticAberration.active = true;
            }
        }//If hit wine a second time, use lens distotion
        else if (winehit == 1)
        {
            volume.profile.TryGet(out LensDistortion lensDistortion);
            if (lensDistortion != null)
            {
                lensDistortion.active = true;
                lensing = true;
            }
        }
        winehit++;
    }

    public void BombHit()
    {
        //If hit a bomb, increase both effects
        EyeHit();
        WineHit();
    }
    #endregion

    #region ScoresAndLives
    public void AddScore(int scoreToAdd)
    {
        //Add to score
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void LoseLife()
    {
        //Lose a life
        lives--;
        audioSource.PlayOneShot(popSound, 0.4f);
        StartCoroutine(LoseHeart(hearts[lives].GetComponent<RectTransform>()));
        
        //livesText.text = "Lives: " + lives;
        if (lives <= 0)
        {
            //Show end game UI
            gameOverText.SetActive(true);
            menuButton.SetActive(true);
            retryButton.SetActive(true);
            DoHighScores();

            //Set screen to black
            usedColor = color3;

            //Destroy all current objects
            foreach (Transform tran in foodParent)
            {
                Destroy(tran.gameObject);
            }
        }
    }

    IEnumerator LoseHeart(RectTransform heart)
    {
        while (heart.localScale.x < 3)
        {
            float growAmount = Time.deltaTime * heartGrowRate;
            heart.localScale += new Vector3(growAmount, growAmount, growAmount);
            yield return null;
        }
        heart.gameObject.SetActive(false);
    }

    void DoHighScores()
    {
        //Compare high scores, show UI, and save new high score if you beat it
        int highscore = 0;

        if (PlayerPrefs.HasKey("HighScore" + difficulty))
        {
            highscore = PlayerPrefs.GetInt("HighScore" + difficulty);
        }
        if (score > highscore)
        {
            //beat high score
            highScoreText.text = "New high score!";
            PlayerPrefs.SetInt("HighScore" + difficulty, score);
        }
        else
        {
            //failed to beat high score
            highScoreText.text = "Score to beat: " + highscore;
        }
    }
    #endregion

    public void Continue()
    {
        //Unpause
        pausePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }
    
    public void Retry()
    {
        //Reload level
        SceneManager.LoadScene("GameScene");
    }

    public void Menu()
    {
        //Go back to menu scene
        SceneManager.LoadScene("MenuScene");
    }
}
