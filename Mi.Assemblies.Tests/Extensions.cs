using System;
using System.Linq;
using SR = System.Reflection;

using Mi.Assemblies;

namespace Mi.Assemblies.Tests {

	public static class Extensions {

		public static MethodDefinition GetMethod (this TypeDefinition self, string name)
		{
			return self.Methods.Where (m => m.Name == name).First ();
		}

		public static FieldDefinition GetField (this TypeDefinition self, string name)
		{
			return self.Fields.Where (f => f.Name == name).First ();
		}

		public static TypeReference MakeGenericType (this TypeReference self, params TypeReference [] arguments)
		{
			if (self.GenericParameters.Count != arguments.Length)
				throw new ArgumentException ();

			var instance = new GenericInstanceType (self);
			foreach (var argument in arguments)
				instance.GenericArguments.Add (argument);

			return instance;
		}

		public static MethodReference MakeGenericMethod (this MethodReference self, params TypeReference [] arguments)
		{
			if (self.GenericParameters.Count != arguments.Length)
				throw new ArgumentException ();

			var instance = new GenericInstanceMethod (self);
			foreach (var argument in arguments)
				instance.GenericArguments.Add (argument);

			return instance;
		}

		public static MethodReference MakeGeneric (this MethodReference self, params TypeReference [] arguments)
		{
			var reference = new MethodReference(self.Name, self.ReturnType) {
				DeclaringType = self.DeclaringType.MakeGenericType (arguments),
				HasThis = self.HasThis,
				ExplicitThis = self.ExplicitThis,
				CallingConvention = self.CallingConvention,
			};

			foreach (var parameter in self.Parameters)
				reference.Parameters.Add (new ParameterDefinition (parameter.ParameterType));

			foreach (var generic_parameter in self.GenericParameters)
				reference.GenericParameters.Add (new GenericParameter (generic_parameter.Name, reference));

			return reference;
		}

		public static FieldReference MakeGeneric (this FieldReference self, params TypeReference [] arguments)
		{
			return new FieldReference(self.Name, self.FieldType) {
				DeclaringType = self.DeclaringType.MakeGenericType (arguments)
			};
		}
	}
}
