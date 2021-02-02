using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    /* PUBLIC API. */

    public static IEnumerator shake(
        float duration = 0.3f, float magnitude = 1, float frequency = 25
    )
    /* Make the camera shake, as if an impact or explosion occurred.
    
    :param float duration: Seconds for which to shake.
    :param float magnitude: How far the camera moves in its shaking.
    :param float frequency: How rapidly the camera shakes.
    */
    {
        // Save where the camera started.
        Vector3 startingEuler = Camera.main.transform.localRotation.eulerAngles;

        // Over the duraction specified, randomly modulate the camera's rotation.
        float seed = Random.value;
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            // PerlinNoise gives random values, but if you pass in values near each
            // other (like Time.time each frame) then the random values are near each
            // other, so you can use this to smooth out your random sequence.
            // Since PerlinNoise gives numbers between 0 and 1, `* 2 - 1` shifts them to
            // between -1 and 1.
            // The first argument to PerlinNoise is a seed so each axis doesn't have
            // identical randomness and make us shake along a single vector in 3D space.
            float randX = Mathf.PerlinNoise(seed + 1, Time.time * frequency) * 2 - 1;
            float randY = Mathf.PerlinNoise(seed + 2, Time.time * frequency) * 2 - 1;
            float randZ = Mathf.PerlinNoise(seed + 3, Time.time * frequency) * 2 - 1;
            Vector3 randEuler = new Vector3(randX, randY, randZ) * magnitude;
            Vector3 newEuler = startingEuler + randEuler;
            Camera.main.transform.localRotation = Quaternion.Euler(newEuler);
            yield return null;
        }

        // Return the camera to its original rotation.
        Camera.main.transform.localRotation = Quaternion.Euler(startingEuler);
    }
}
