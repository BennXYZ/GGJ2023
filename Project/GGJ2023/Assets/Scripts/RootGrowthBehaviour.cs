using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootGrowthBehaviour : MonoBehaviour
{
    [SerializeField]
    float growthTime, growthDelay, finalScale;

    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * Mathf.Clamp01((Time.time - startTime - startTime - growthDelay) / growthTime) * finalScale;
    }
}
