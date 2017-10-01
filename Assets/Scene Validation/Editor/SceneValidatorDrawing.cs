using UnityEditor;
using UnityEngine;

namespace SceneValidation
{
	public class ScrollState
	{
		public Vector2 scrollPosition;
	}

	/// <summary>
	/// Utility functions for drawing the scene validator window
	/// </summary>
	public static class SceneValidatorDrawing
	{
		private const string CrossIconPath = "Assets/Scene Validation/Editor/Graphics/cross.png";
		private const string CheckIconPath = "Assets/Scene Validation/Editor/Graphics/check.png";
		private const string QuestionIconPath = "Assets/Scene Validation/Editor/Graphics/question.png";

		public static bool ValidationItem(ValidationResult validationState, string text)
		{
			using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
			{
				var rect = GUILayoutUtility.GetRect(30f, 30f, GUILayout.ExpandWidth(true));
				switch (validationState)
				{
					case ValidationResult.Success:
						DrawIcon(rect, CheckIconPath);
						break;
					case ValidationResult.Fail:
						DrawIcon(rect, CrossIconPath);
						break;
					case ValidationResult.Unknown:
						DrawIcon(rect, QuestionIconPath);
						break;
				}
				var labelStyle = new GUIStyle(EditorStyles.label)
				{
					wordWrap = true
				};
				EditorGUILayout.LabelField(text, labelStyle);
				GUILayout.FlexibleSpace();
				return GUILayout.Button("Validate", GUILayout.MinHeight(30f));
			}
		}

		private static void DrawIcon(Rect position, string iconPath)
		{
			EditorGUI.LabelField(position, new GUIContent(AssetDatabase.LoadAssetAtPath<Texture>(iconPath)));
		}
	}
}