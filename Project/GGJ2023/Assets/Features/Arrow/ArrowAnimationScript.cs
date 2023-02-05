using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnimationScript : MonoBehaviour
{
    [SerializeField]
    AnimationCurve rotationOverValue, verticalScaleOverValue, positionOverValue;

    [SerializeField,Range(0,1)]
    float value;

    [SerializeField]
    Transform bone;

    // Update is called once per frame
    void Update()
    {
        bone.localPosition = Vector3.up * positionOverValue.Evaluate(value);
        
        bone.localRotation = ;
    }
}
