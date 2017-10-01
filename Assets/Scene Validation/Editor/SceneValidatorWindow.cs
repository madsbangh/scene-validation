using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

namespace SceneValidation
{
	public class SceneValidatorWindow : EditorWindow
	{
		private SceneValidatorState sceneValidatorState;
		private WindowState windowState;

		private enum WindowState
		{
			Valid,
			NotExactlyOneSceneOpen,
			NoValidatorForScene
		}

		[MenuItem("Scene Validator/Show Window")]
		public static void ShowWindow()
		{
			var window = GetWindow<SceneValidatorWindow>("Validation");
			window.Show();
			window.SetupSceneValidator();
		}

		void OnEnable()
		{
			Undo.willFlushUndoRecord += Repaint;
			EditorSceneManager.sceneOpened += OnSceneOpened;
			AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReloaded;
		}

		void OnDisable()
		{
			Undo.willFlushUndoRecord -= Repaint;
			EditorSceneManager.sceneOpened -= OnSceneOpened;
			AssemblyReloadEvents.afterAssemblyReload -= OnAssemblyReloaded;
		}

		void OnGUI()
		{
			var style = new GUIStyle(EditorStyles.largeLabel)
			{
				alignment = TextAnchor.MiddleCenter
			};
			EditorGUILayout.LabelField("Scene Validator", style);

			switch (windowState)
			{
				case WindowState.Valid:
					DrawValidatorItemsGUI(sceneValidatorState);
					break;
				case WindowState.NotExactlyOneSceneOpen:
					EditorGUILayout.HelpBox("Only works with exactly one scene open.", MessageType.Error);
					break;
				case WindowState.NoValidatorForScene:
					EditorGUILayout.HelpBox("No validator script for this scene.", MessageType.Info);
					break;
			}
		}

		public static void DrawValidatorItemsGUI(SceneValidatorState sceneValidatorState)
		{
			if (GUILayout.Button("Validate All", GUILayout.Height(40f)))
			{
				sceneValidatorState.ValidateAll();
			}

			var scrollControlID = GUIUtility.GetControlID(FocusType.Passive);
			var scrollState = (ScrollState)GUIUtility.GetStateObject(typeof(ScrollState), scrollControlID);
			using (var scrollView = new GUILayout.ScrollViewScope(scrollState.scrollPosition))
			{
				scrollState.scrollPosition = scrollView.scrollPosition;
				foreach (var validationInfo in sceneValidatorState.GetValidationInfosSorted())
				{
					
					if (SceneValidatorDrawing.ValidationItem(validationInfo.Result, validationInfo.Description))
					{
						var validatorInstance = Activator.CreateInstance(sceneValidatorState.ValidatorClass);
						var success = (bool)validationInfo.ValidationMethod.Invoke(validatorInstance, null);
						validationInfo.Result = success ? ValidationResult.Success : ValidationResult.Fail;
					}
				}
			}
		}

		private void OnSceneOpened(Scene scene, OpenSceneMode mode)
		{
			SetupSceneValidator();
		}

		private void OnAssemblyReloaded()
		{
			SetupSceneValidator();
		}

		private void SetupSceneValidator()
		{
			sceneValidatorState = null;
			if (EditorSceneManager.loadedSceneCount == 1)
			{
				var scenePath = EditorSceneManager.GetActiveScene().path;
				Type validatorClass = SceneValidatorReflectionUtility.GetValidatorFor(scenePath);
				if (validatorClass != null)
				{
					sceneValidatorState = new SceneValidatorState(validatorClass);
					windowState = WindowState.Valid;
				}
				else
				{
					windowState = WindowState.NoValidatorForScene;
				}
			}
			else
			{
				windowState = WindowState.NotExactlyOneSceneOpen;
			}
		}
	}
}