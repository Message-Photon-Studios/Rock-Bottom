using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(AutoRenderLayer))]
public class AutoRenderLayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var genManager = (AutoRenderLayer)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("FixLayers"))
        {
            genManager.FixRenderLayers();
        }
    }
}
#endif
