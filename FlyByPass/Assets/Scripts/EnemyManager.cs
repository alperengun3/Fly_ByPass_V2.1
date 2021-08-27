using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemy;
    void Start()
    {
        Instantiate(enemy[0], new Vector3(0,8f,-390), Quaternion.identity);
        Instantiate(enemy[1], new Vector3(10, 8, -375), Quaternion.identity);
        Instantiate(enemy[2], new Vector3(-10, 8, -350), Quaternion.identity);
    }

    void Update()
    {
        
    }
}
