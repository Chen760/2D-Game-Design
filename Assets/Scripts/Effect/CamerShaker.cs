using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class CamerShaker : MonoBehaviour
{
    private List<ShakeRequest> _requests = new List<ShakeRequest>();
    private CinemachineBasicMultiChannelPerlin _noise;

    [SerializeField]
    private float _shakeDecreaseAmount = 10f;

    private void Awake()
    {
        CinemachineVirtualCamera cm = GetComponent<CinemachineVirtualCamera>();
        _noise = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (_requests.Count == 0)
        {
            _noise.m_AmplitudeGain = 0;
            return;
        }

        float strongestShake = _requests.Max(shake => shake.ShakeAmount);
        _noise.m_AmplitudeGain = strongestShake;

        for (int i = _requests.Count - 1; i >= 0; i--)
        {
            ShakeRequest request = _requests[i];
            request.ShakeTime -= Time.deltaTime;
            if (request.ShakeTime <= 0)
            {
                request.ShakeAmount = Mathf.Max(0, request.ShakeAmount - Time.deltaTime * _shakeDecreaseAmount);
            }

            if (request.ShakeAmount == 0) _requests.Remove(request);

        }
    }

    public void RequestShake(float amount)
    {
        RequestShake(amount, 0);
    }

    public void RequestShake(float amount, float time)
    {
        _requests.Add(new ShakeRequest
        {
            ShakeAmount = amount,
            ShakeTime = time

        });
    }
    private class ShakeRequest
    {
        public float ShakeAmount;
        public float ShakeTime;
    }

}
