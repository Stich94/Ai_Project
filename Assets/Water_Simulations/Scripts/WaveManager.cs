using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a Singleton
public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float length = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float offset = 2.2f;
    
    public static WaveManager Instance
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

    private void Update()
    {
        offset += Time.deltaTime * speed;
    }


    public float GetWaveHeight(float x)
    {
        return amplitude * Mathf.Sin(x / length + offset);
    }
}
