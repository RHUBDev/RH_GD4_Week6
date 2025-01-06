using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpawnedObject : MonoBehaviour
{
    public Vector2 forceRange = new Vector2(5f, 10f);
    public Vector2 torqueRange = new Vector2(10f, 30f);
    private Rigidbody rig;
    private GameManager gamemanager;
    public GameObject splosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        gamemanager = FindObjectOfType<GameManager>();
        rig = GetComponent<Rigidbody>();
        rig.AddForce(Vector3.up * Random.Range(forceRange.x, forceRange.y), ForceMode.Impulse);
        rig.AddTorque(new Vector3(Random.Range(torqueRange.x, torqueRange.y), Random.Range(torqueRange.x, torqueRange.y),Random.Range(torqueRange.x, torqueRange.y)),ForceMode.Impulse);
    }

    private void Update()
    {
        if(transform.position.y < -3f)
        {
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
                gamemanager.HitFood(rig.velocity, gameObject, 0);
                //Add score if food
                gamemanager.score++;
            }
            else if (transform.CompareTag("Eye"))
            {
                Debug.Log("Lost Life - Eye");
                //If hit an eye, lose a life, and make screen redder
                gamemanager.HitFood(rig.velocity, gameObject, 1); 
                gamemanager.EyeHit();
                gamemanager.LoseLife();
            }
            else if (transform.CompareTag("Wine"))
            {
                Debug.Log("Lost Life - Wine");
                //If hit wine, lose a life, and make screen drunker hehe
                gamemanager.HitFood(rig.velocity, gameObject, 1);
                gamemanager.WineHit();
                gamemanager.LoseLife();
            }
            else if (transform.CompareTag("Bomb"))
            {
                Debug.Log("Lost Life - Bomb");
                //If hit a bomb, lose a life, and call BombHit which does both effects
                gamemanager.HitFood(rig.velocity, gameObject, 2); 
                gamemanager.BombHit();
                gamemanager.LoseLife();
            }
            //Destroy the object
            Destroy(gameObject);
        }
    }
}
