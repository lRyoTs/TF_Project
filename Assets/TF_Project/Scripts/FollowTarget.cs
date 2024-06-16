using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        MatchTargetTransform();
    }

    // Update is called once per frame
    void Update()
    {
        MatchTargetTransform();
    }

    private void MatchTargetTransform()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
