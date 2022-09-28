using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// https://blog.csdn.net/yq398934906/article/details/104881406
    /// </summary>
    [CustomEditor(typeof(MeshRenderer))]
    public class MeshRendererEditor : UnityEditor.Editor
    {
        private MeshRenderer _meshRenderer;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _meshRenderer = target as MeshRenderer;

            var layerNames = new string[SortingLayer.layers.Length];
            for (var i = 0; i < SortingLayer.layers.Length; i++)
            {
                layerNames[i] = SortingLayer.layers[i].name;
            }

            if (_meshRenderer != null)
            {
                var layerValue = SortingLayer.GetLayerValueFromID(_meshRenderer.sortingLayerID);
                layerValue = EditorGUILayout.Popup("Sorting Layer", layerValue, layerNames);

                var layer = SortingLayer.layers[layerValue];
                _meshRenderer.sortingLayerName = layer.name;
                _meshRenderer.sortingLayerID = layer.id;
            }

            if (_meshRenderer != null)
            {
                _meshRenderer.sortingOrder = EditorGUILayout.IntField("Order in Layer", _meshRenderer.sortingOrder);
            }
        }
    }
}