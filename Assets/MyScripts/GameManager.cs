using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform player;
    public static GameManager instance;
    
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Debug.LogError("Game Manager is Null!");
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
        
    }

    public Vector3 GetPlayerPos()
    {
        return Instance.player.transform.position;
    }

    public Transform GetPlayerTransform()
    {
        return Instance.player.transform;
    }
}
