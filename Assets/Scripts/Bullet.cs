using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    void OnCollisionEnter(Collision other)
    {
        if(!isRock && other.gameObject.CompareTag("Floor")){ // Bullet Case를 위한 로직
            Destroy(gameObject,3);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag){ // 탄dkf 제거 로직
            case "Floor":
            case "Wall":
                if(!isMelee)
                    Destroy(gameObject);
                break;
        }
    }
}
