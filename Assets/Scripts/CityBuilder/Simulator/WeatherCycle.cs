using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherCycle : MonoBehaviour
{
    public static WeatherCycle instance;

    [Header("Sun")]
    public Transform sun;
    public Transform Axis;

    public Gradient sunColor = new Gradient()
    {
        colorKeys = new GradientColorKey[3]{new GradientColorKey(new Color(1, 0.75f, 0.3f), 0),new GradientColorKey(new Color(0.95f, 0.95f, 1), 0.5f),new GradientColorKey(new Color(1, 0.75f, 0.3f), 1),},
        alphaKeys = new GradientAlphaKey[2]{new GradientAlphaKey(1, 0),new GradientAlphaKey(1, 1)}
    };

    [Header("Sky")]
    [GradientUsage(true)]
    public Gradient skyColorDay = new Gradient()
    {
        colorKeys = new GradientColorKey[3]{new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 0),new GradientColorKey(new Color(0.7f, 1.4f, 3), 0.5f),new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 1),},
        alphaKeys = new GradientAlphaKey[2]{new GradientAlphaKey(1, 0),new GradientAlphaKey(1, 1)}
    };

    [GradientUsage(true)]
    public Gradient skyColorNight = new Gradient()
    {
        colorKeys = new GradientColorKey[3]{new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 0),new GradientColorKey(new Color(0.44f, 1, 1), 0.5f),new GradientColorKey(new Color(0.75f, 0.3f, 0.17f), 1),},
        alphaKeys = new GradientAlphaKey[2]{new GradientAlphaKey(1, 0),new GradientAlphaKey(1, 1)}
    };
    // enum value type data type
    public enum TimeOfDay { Morning, Noon, Evening, LateEvening, Night, Dawn }
    public float[] Brightness;
    public Vector3[] AxisController;

    public TimeOfDay whatisDay;
    private Light sunLight;
    private float sunAngle;

    void Awake()
    {
        if (instance == null) instance = this;
        else Debug.Log("Warning; Multiples instances found of {0}, only one instance of {0} allowed.", this);
    }

    void Start()
    {
        sunLight = sun.GetComponent<Light>();
        Brightness = new float[] { 0.25f, 1, 0.25f, 0.08f, 0f, 0.08f };
        AxisController = new Vector3[] { new Vector3(0, 0, 300), new Vector3(0, 0, 0), new Vector3(0, 0, 60), new Vector3(0, 0, 0), new Vector3(0, 0, 180), new Vector3(0, 0, 0) };
        Axis.rotation = Quaternion.Euler(AxisController[(int)whatisDay]);
        sun.transform.localRotation = Quaternion.Euler(90, 0, 0);
        sunLight.intensity = Brightness[(int)whatisDay];
        if((int)whatisDay == 1)
        {
            sunLight.color = sunColor.Evaluate(0.5f);
        }
        else if((int)whatisDay == 1 || (int)whatisDay == 3)
        {
            sunLight.color = sunColor.Evaluate(0.25f);
        }
        else
        {
            sunLight.color = sunColor.Evaluate(0.25f);
        }
        
    }
}
