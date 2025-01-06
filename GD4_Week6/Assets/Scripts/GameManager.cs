using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    private int eyehit = 0;
    private int winehit = 0;
    private bool lensing = false;
    private float minDistortion = -0.8f;
    private float maxDistortion = 0.8f;
    private float speed = 1f;
    private Color usedColor;
    public GameObject[] splosionPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        //lensing = true;
        usedColor = color0;
        //Start spawning objects
        StartCoroutine(SpawnObjects());
    }
   
    // Update is called once per frame
    void Update()
    {
        //Get mouse button 0 input and set 'mousing' accordingly
        if (Input.GetMouseButtonDown(0))
        {
            mousing = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            mousing = false;
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

    IEnumerator SpawnObjects()
    {
        //While alive keep spawning objects
        while (lives > 0)
        {
            yield return new WaitForSeconds(1);
            Instantiate(spawnObjects[Random.Range(0, spawnObjects.Count)], new Vector3(Random.Range(-xRange, xRange), -1, 0), Quaternion.identity);
        }
    }

    public void HitFood(Vector3 objectVelocity, GameObject food, int splosionnum)
    {
        float inputx = Input.GetAxis("Mouse X");
        float inputy = Input.GetAxis("Mouse Y");
        Vector2 mousevect = new Vector2(inputx, inputy);
        
        Quaternion splosionRotation = Quaternion.LookRotation(new Vector3(inputx, inputy,0).normalized);
        //Quaternion splosionRotation = Quaternion.LookRotation(objectVelocity);
        GameObject splosion = Instantiate(splosionPrefabs[splosionnum], food.transform.position, splosionRotation);
        //splosion.transform.
        //ParticleSystem parts = splosion.GetComponent<ParticleSystem>();
        //parts
    }

    public void LoseLife()
    {
        //Lose a life
        lives--;
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
}
