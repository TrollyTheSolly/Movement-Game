using UnityEditor;

namespace AllIn13DShader
{
	[CustomEditor(typeof(DepthColoringCamera))]
	public class DepthColoringCameraCustomEditor : Editor
	{
		private SerializedProperty spCam;
		private SerializedProperty spDepthColoringProperties;

		private AllIn1DepthColoringProperties depthColoringProperties;
		private SerializedObject soDepthColoringProperties;
		private SerializedProperty spFallOff;
		private SerializedProperty spGradientTex;
		private SerializedProperty spDepthZoneLength;

		private bool depthColoringFoldout;

		private void RefreshReferences()
		{
			if(spCam == null)
			{
				spCam = serializedObject.FindProperty("cam");
			}

			if(spDepthColoringProperties == null)
			{
				spDepthColoringProperties = serializedObject.FindProperty("allIn1DepthColoringProperties");
			}

			RefreshDepthColoringReferences();
		}

		private void RefreshDepthColoringReferences()
		{
			if (spDepthColoringProperties != null)
			{
				depthColoringProperties = spDepthColoringProperties.objectReferenceValue as AllIn1DepthColoringProperties;

				if (depthColoringProperties != null)
				{
					soDepthColoringProperties = new SerializedObject(spDepthColoringProperties.objectReferenceValue);

					spFallOff = soDepthColoringProperties.FindProperty("fallOff");
					spGradientTex = soDepthColoringProperties.FindProperty("depthColoringGradientTex");
					spDepthZoneLength = soDepthColoringProperties.FindProperty("depthZoneLength");
				}
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			RefreshReferences();

			EditorGUILayout.PropertyField(spCam);

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(spDepthColoringProperties);

			if (spDepthColoringProperties != null && soDepthColoringProperties != null && depthColoringProperties != null)
			{
				soDepthColoringProperties.Update();
				depthColoringFoldout = EditorGUILayout.Foldout(depthColoringFoldout, "Depth Coloring properties");
				if (depthColoringFoldout)
				{
					DrawDepthColoringProperties();
				}
				soDepthColoringProperties.ApplyModifiedProperties();
			}
			if (EditorGUI.EndChangeCheck())
			{
				if(depthColoringProperties != null)
				{
					depthColoringProperties.ApplyValues();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawDepthColoringProperties()
		{
			EditorGUILayout.PropertyField(spFallOff);
			EditorGUILayout.PropertyField(spGradientTex);
			EditorGUILayout.PropertyField(spDepthZoneLength);
		}
	}
}