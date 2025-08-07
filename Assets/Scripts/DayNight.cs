using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    [Range(0.0f, 1.0f)] // 하루의 범위 0 ~ 1 -> 0 : 자정, 0.25 : 아침, 0.5 정오, 0.75 : 저녁
    public float time;
    public float fullDayLength; // 하루 길이
    public float startTime = 0.4f;  // 시작시간 0.4 니까 오전
    private float timeRate;
    public Vector3 noon;    // Vector 90 0 0 (정오)

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;               // 그라데이션
    public AnimationCurve sunIntensity;     // 밝기 곡선

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;      // 밝기
    public AnimationCurve reflectionIntensityMultiplier;    // 반사조명

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

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);           // 씬 전체의 전반 조명 밝기 ( 시간에 따라 조절 )
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);      // 반사 밝기 ( 시간에 따라 조절 )
    }

    void UpdateLighting(Light lightSource, Gradient gradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time);    // 밝기 = 시간에 따라

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4f;   // 12시를 90도로 맞춰야한다 = 0.5 - ( 0.25 아침, 0.75 저녁 ) * 4f
        lightSource.color = gradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if(lightSource.intensity == 0 && go.activeInHierarchy)      // 광원의 밝기 == 0 인데 하이어라키활성화면
        {
            go.SetActive(false);                                    // 꺼준다.
        }
        else if(lightSource.intensity > 0 && !go.activeInHierarchy)
        {
            go.SetActive(true);
        }
    }
}
