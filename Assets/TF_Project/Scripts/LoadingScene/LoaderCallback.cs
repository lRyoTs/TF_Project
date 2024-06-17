using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool firstUpdate = true;

    private void Start()
    {
        StartCoroutine("TemporalWait");
    }

    private IEnumerator TemporalWait()
    {
        yield return new WaitForSeconds(5f);
        Loader.LoaderCallback();
    }
}
