using System;

namespace SceneValidation
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public sealed class SceneValidatorAttribute : Attribute
	{
		private readonly string scenePath;

		public string ScenePath
		{
			get { return scenePath; }
		}

		/// <summary>
		/// The class is a collection of validation methods tied to a specific scene
		/// </summary>
		/// <param name="scenePath">The path to the scene this validator's methods are run on</param>
		public SceneValidatorAttribute(string scenePath)
		{
			this.scenePath = scenePath;
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class ValidationMethodAttribute : Attribute
	{
		private readonly string description;

		public string Description
		{
			get { return description; }
		}

		/// <summary>
		/// The method validates a single aspect of the scene
		/// </summary>
		/// <param name="description">The description for this test (e.g. "The scene must have an EventSystem")</param>
		public ValidationMethodAttribute(string description)
		{
			this.description = description;
		}
	}
}