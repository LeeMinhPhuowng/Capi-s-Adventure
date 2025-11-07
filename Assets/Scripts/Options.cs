using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    [SerializeField] private float WaitTime = 2f;
    private float StartTime = 0f;
    public GameObject options;
    // Start is called before the first frame update
    void Start()
    {
        StartTime = Time.time;
        options.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - StartTime >= WaitTime) 
        {
            options.SetActive(true);  
        }
    }
}
