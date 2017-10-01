using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace SceneValidation
{
	public class ValidationInfo
	{
		public ValidationResult Result { get; set; }
		public MethodInfo ValidationMethod { get; private set; }
		public string Description { get; private set; }

		public ValidationInfo(MethodInfo validationMethod, ValidationResult result)
		{
			Result = result;
			ValidationMethod = validationMethod;
			var attribute = (ValidationMethodAttribute)Attribute.GetCustomAttribute(ValidationMethod, typeof(ValidationMethodAttribute));
			Description = attribute.Description;
		}
	}

	/// <summary>
	/// Contains all scene validations and their descriptions..
	/// Kinda like test cases.
	/// </summary>
	public class SceneValidatorState
	{
		public Type ValidatorClass { get; private set; }
		public string ScenePath { get; private set; }

		private readonly List<ValidationInfo> validationInfos = new List<ValidationInfo>();

		public SceneValidatorState(Type validatorClass)
		{
			ValidatorClass = validatorClass;
			var sceneValidatorAttribute = validatorClass.GetSceneValidationAttribute();
			ScenePath = sceneValidatorAttribute.ScenePath;

			validationInfos = FindValidationInfos().ToList();
		}

		private IEnumerable<ValidationInfo> FindValidationInfos()
		{
			return SceneValidatorReflectionUtility.GetSceneValidationMethodsForType(ValidatorClass)
				.Select(mi => new ValidationInfo(mi, ValidationResult.Unknown));
		}

		public IEnumerable<ValidationInfo> GetValidationInfosSorted()
		{
			var sortedInfos = new List<ValidationInfo>(validationInfos);
			return sortedInfos.OrderBy(info => info.Result);
		}

		public void ValidateAll()
		{
			var validatorInstance = Activator.CreateInstance(ValidatorClass);
			foreach (var validationInfo in validationInfos)
			{
				var success = (bool)validationInfo.ValidationMethod.Invoke(validatorInstance, null);
				validationInfo.Result = success ? ValidationResult.Success : ValidationResult.Fail;
			}
		}
	}
}