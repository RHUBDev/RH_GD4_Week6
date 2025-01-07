using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpawnedObject : MonoBehaviour
{
    private Vector2 forceRange = new Vector2(10f, 15f);
    private Vector2 torqueRange = new Vector2(-1f, 1f);
    private Rigidbody rig;
    private GameManager gamemanager;
    public GameObject splosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        //Get gamemanager
        gamemanager = FindObjectOfType<GameManager>();
        //Set object forces
        rig = GetComponent<Rigidbody>();
        rig.AddForce(Vector3.up * Random.Range(forceRange.x, forceRange.y), ForceMode.Impulse);
        rig.AddTorque(new Vector3(Random.Range(torqueRange.x, torqueRange.y), Random.Range(torqueRange.x, torqueRange.y),Random.Range(torqueRange.x, torqueRange.y)),ForceMode.Impulse);
    }

    private void Update()
    {
        if(transform.position.y < -3f)
        {
            //Destroy object if fallen below screen limit
            if (transform.CompareTag("Food"))
            {
                //And lose life if you missed a 'good' item
                gamemanager.LoseLife();
            }
            Destroy(gameObject);
        }
    }

    private void OnMouseEnter()
    {
        //If we are clicking or click-dragging the mouse
        if (gamemanager.mousing)
        {
            if (transform.CompareTag("Food"))
            {
                Debug.Log("Score++");
                gamemanager.HitFood(gameObject, 0);

                //Add score if food
                int scoreToAdd = 1;
                /*if(transform.name == "AppleGreen")
                {
                    scoreToAdd = 2;
                }
                else if (transform.name == "AppleRed")
                {
                    scoreToAdd = 3;
                }*/
                gamemanager.AddScore(scoreToAdd);
            }
            else if (transform.CompareTag("Eye"))
            {
                Debug.Log("Lost Life - Eye");
                //If hit an eye, lose a life, and make screen redder
                gamemanager.HitFood(gameObject, 1); 
                gamemanager.EyeHit();
                gamemanager.LoseLife();
            }
            else if (transform.CompareTag("Wine"))
            {
                Debug.Log("Lost Life - Wine");
                //If hit wine, lose a life, and make screen drunker hehe
                gamemanager.HitFood(gameObject, 1);
                gamemanager.WineHit();
                gamemanager.LoseLife();
            }
            else if (transform.CompareTag("Bomb"))
            {
                Debug.Log("Lost Life - Bomb");
                //If hit a bomb, lose a life, and call BombHit which does both effects
                gamemanager.HitFood(gameObject, 2); 
                gamemanager.BombHit();
                gamemanager.LoseLife();
            }
            //Destroy the object
            Destroy(gameObject);
        }
    }
}
