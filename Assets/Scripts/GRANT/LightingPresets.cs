using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Scriptables/Lighting Preset", order = 1)]

public class LightingPresets : ScriptableObject
{
    // Start is called before the first frame update
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;


}
