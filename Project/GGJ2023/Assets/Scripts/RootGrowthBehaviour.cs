using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootGrowthBehaviour : MonoBehaviour
{
    [SerializeField]
    float growthTime, minDelay, maxDelay, finalScale;

    float startTime, actualDelay;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = Vector3.zero;
        actualDelay = Mathf.Max(minDelay + Random.Range(0f, maxDelay - minDelay),0);
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float scale = Mathf.Clamp01((Time.time - startTime - actualDelay) / growthTime);
        transform.localScale = Vector3.one * scale * finalScale;
        if (scale == 1)
            this.enabled = false;
    }
}
