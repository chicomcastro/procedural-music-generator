using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public MusicParameters musicParameters;
    public static AudioManager instance;

    public float currentTempo;
    public int currentCompass;

    private float beatPartition = 2f;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        // musicParameters will be our singleton
        if (MusicParameters.instance == null) {
            MusicParameters.instance = musicParameters;
        }
    }

    private void Start()
    {
        // Sound feedback
        StartCoroutine(CountTempo());
    }

    IEnumerator CountTempo()
    {
        currentTempo = 0f;
        currentCompass = 1;
        yield return new WaitForSeconds(60f / musicParameters.bpm * musicParameters.signature);

        while (true)
        {
            currentTempo = 1f;
            for (int j = 0; j < musicParameters.signature * beatPartition; j++)
            {
                yield return new WaitForSeconds(60f / musicParameters.bpm / beatPartition);
                currentTempo+=1f/beatPartition;
            }
            currentCompass++;
        }
    }
}
