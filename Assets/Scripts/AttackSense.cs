using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSense : MonoBehaviour
{
    private static AttackSense instance;

    public static AttackSense Instance
    {
        get
        {
            if (instance == null)
                instance = Transform.FindObjectOfType<AttackSense>();
            return instance;
        }
    }
    private bool isShake;

    public void HitPause(int duration)
    {
        // Debug.Log("停顿");
        StartCoroutine(Pause(duration));
    }

    IEnumerator Pause(int duration)
    {
        float pauseTime = duration / 60f;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(pauseTime);
        Time.timeScale = 1;
    }

    public void CameraShake(float duration, float strength)
    {
        // Debug.Log("晃动");
        if (!isShake)
            StartCoroutine(Shake(duration, strength));
    }

    IEnumerator Shake(float duration, float strength)
    {
        isShake = true;
        Transform cameraTrans = Camera.main.transform;
        Vector3 startPosition = cameraTrans.position;

        while (duration > 0)
        {
            cameraTrans.position = Random.insideUnitSphere * strength + startPosition;
            duration -= Time.deltaTime;
            yield return null;
        }
        cameraTrans.position = startPosition;
        isShake = false;
    }
}
