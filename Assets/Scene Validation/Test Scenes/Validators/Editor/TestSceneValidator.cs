using SceneValidation;
using UnityEngine;
using UnityEngine.EventSystems;

[SceneValidator("Assets/Scene Validation/Test Scenes/Test Scene.unity")]
public class TestSceneValidator
{
	[ValidationMethod("Scene must have an EventSystem")]
	public bool ValidateHasEventSystem()
	{
		return Object.FindObjectOfType<EventSystem>() != null;
	}

	[ValidationMethod("Scene must have a Camera")]
	public bool ValidateHasCamera()
	{
		return Object.FindObjectOfType<Camera>() != null;
	}
}
