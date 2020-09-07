using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicComposer : MonoBehaviour
{
    private void Start() {
        StartCoroutine(PlayMusic());
    }

    IEnumerator PlayMusic() {
        // Wrote here your music
        // TODO turn this visual
        yield return null;
    }
}
