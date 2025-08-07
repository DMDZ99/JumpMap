using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    [Range(0.0f, 1.0f)] // �Ϸ��� ���� 0 ~ 1 -> 0 : ����, 0.25 : ��ħ, 0.5 ����, 0.75 : ����
    public float time;
    public float fullDayLength; // �Ϸ� ����
    public float startTime = 0.4f;  // ���۽ð� 0.4 �ϱ� ����
    private float timeRate;
    public Vector3 noon;    // Vector 90 0 0 (����)

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;               // �׶��̼�
    public AnimationCurve sunIntensity;     // ��� �

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;      // ���
    public AnimationCurve reflectionIntensityMultiplier;    // �ݻ�����

    void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }


    void Update()
    {
        time = (time + timeRate *  Time.deltaTime) % 1.0f;

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);           // �� ��ü�� ���� ���� ��� ( �ð��� ���� ���� )
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);      // �ݻ� ��� ( �ð��� ���� ���� )
    }

    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);    // ��� = �ð��� ����

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4f;   // 12�ø� 90���� ������Ѵ� = 0.5 - ( 0.25 ��ħ, 0.75 ���� ) * 4f
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if(lightSource.intensity == 0 && go.activeInHierarchy)      // ������ ��� == 0 �ε� ���̾��ŰȰ��ȭ��
        {
            go.SetActive(false);                                    // ���ش�.
        }
        else if(lightSource.intensity > 0 && !go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }
}
