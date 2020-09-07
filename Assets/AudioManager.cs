using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public MusicParameters musicParameters;

    public int currentTempo;
    public int currentCompass;

    private void Start()
    {
        currentTempo = 1;
        currentCompass = 1;

        // Sound feedback
        StartCoroutine(CountTempo());
    }

    IEnumerator CountTempo()
    {
        yield return new WaitUntil(() => currentTempo == 1);

        while (true)
        {
            currentTempo = 1;
            for (int j = 0; j < musicParameters.signature; j++)
            {
                yield return new WaitForSeconds(60f / musicParameters.bpm);
                currentTempo++;
            }
            currentCompass++;
        }
    }
}
