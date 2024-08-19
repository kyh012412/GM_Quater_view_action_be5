using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Floor")){
            Destroy(gameObject,3);
        }else if(other.gameObject.CompareTag("Wall")){
            Destroy(gameObject);
        }
    }
}
