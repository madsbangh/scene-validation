using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SceneValidation
{
	public static class SceneValidatorReflectionUtility
	{
		public static Type GetValidatorFor(string scenePath)
		{
			return GetTypesWith<SceneValidatorAttribute>(false)
				.Where(v => scenePath.Equals(v.GetSceneValidationAttribute().ScenePath))
				.FirstOrDefault();
		}

		public static SceneValidatorAttribute GetSceneValidationAttribute(this Type sceneValidatorClass)
		{
			return (SceneValidatorAttribute)sceneValidatorClass.GetCustomAttributes(typeof(SceneValidatorAttribute), false).FirstOrDefault();
		}

		public static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit) where TAttribute : Attribute
		{
			return from a in AppDomain.CurrentDomain.GetAssemblies()
				   from t in a.GetTypes()
				   where t.IsDefined(typeof(TAttribute), inherit)
				   select t;
		}

		public static IEnumerable<MethodInfo> GetSceneValidationMethodsForType(Type type)
		{
			foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
			{
				if (method.GetCustomAttributes(typeof(ValidationMethodAttribute), false).Length > 0 &&
					method.ReturnType == typeof(bool) &&
					method.GetParameters().Length == 0)
				{
					yield return method;
				}
			}
		}
	}
}