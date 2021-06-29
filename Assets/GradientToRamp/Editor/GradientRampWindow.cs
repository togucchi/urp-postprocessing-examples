using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Toguchi.Rendering
{
    public class GradientRampWindow : EditorWindow
    {
        private Gradient _gradient = new Gradient();

        private int _size = 128;
        
        [MenuItem("Window/GradientRamp")]
        private static void Open()
        {
            EditorWindow.GetWindow<GradientRampWindow>();
        }

        private void OnGUI()
        {
            _gradient = EditorGUILayout.GradientField("Gradient", _gradient);
            _size = EditorGUILayout.IntField("Size", _size);

            if (GUILayout.Button("Convert"))
            {
                Convert();
            }
        }

        private void Convert()
        {
            var path = EditorUtility.SaveFilePanelInProject("保存", "gradient", "png", "Save Texture");

            Texture2D texture2D = new Texture2D(_size, 1);

            var colors = new Color[_size];
            for (int i = 0; i < _size; i++)
            {
                var t = (float) i /  (_size - 1);
                colors[i] = _gradient.Evaluate(t);
            }
            
            texture2D.SetPixels(colors);
            var bytes = texture2D.EncodeToPNG();
            
            // 保存
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
        }
    }
}