
using UnityEngine;

[CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/ Gun Trail Config" , order = 4)]
public class TrailConfigScriptableObject : ScriptableObject
{
    public Material Material;
    public AnimationCurve WidthCurve;

    public float Duration;
    public float MinVertexDistance = 0.1f;
    public float MissDistance = 100f;
    public float SimulationSpeed = 100f;


    public Gradient Color;



}
