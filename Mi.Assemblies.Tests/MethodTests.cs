using System;
using System.Linq;

using Mi.Assemblies;
using Mi.Assemblies.Metadata;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mi.Decompiler.Tests;

namespace Mi.Assemblies.Tests {

	[TestClass]
	public class ZAsm_MethodTests 
    {
		[TestMethod]
		public void AbstractMethod ()
		{
            ModuleDefinition module = SampleInputLoader.LoadAssembly("Methods").MainModule;

			var type = module.Types [1];
			Assert.AreEqual ("Foo", type.Name);
			Assert.AreEqual (2, type.Methods.Count);

			var method = type.GetMethod ("Bar");
			Assert.AreEqual ("Bar", method.Name);
			Assert.IsTrue (method.IsAbstract);
			Assert.IsNotNull (method.ReturnType);

			Assert.AreEqual (1, method.Parameters.Count);

			var parameter = method.Parameters [0];

			Assert.AreEqual ("a", parameter.Name);
			Assert.AreEqual ("System.Int32", parameter.ParameterType.FullName);
		}

		[TestMethod]
		public void SimplePInvoke ()
		{
            ModuleDefinition module = SampleInputLoader.LoadAssembly("Methods").MainModule;
			var bar = module.GetType ("Bar");
			var pan = bar.GetMethod ("Pan");

			Assert.IsTrue (pan.IsPInvokeImpl);
			Assert.IsNotNull (pan.PInvokeInfo);

			Assert.AreEqual ("Pan", pan.PInvokeInfo.EntryPoint);
			Assert.IsNotNull (pan.PInvokeInfo.Module);
			Assert.AreEqual ("foo.dll", pan.PInvokeInfo.Module.Name);
		}

		[TestMethod]
        public void GenericMethodDefinition()
        {
            ModuleDefinition module = SampleInputLoader.LoadAssembly("GenericsAsm").MainModule;
			var baz = module.GetType ("Baz");

			var gazonk = baz.GetMethod ("Gazonk");

			Assert.IsNotNull (gazonk);

			Assert.IsTrue (gazonk.HasGenericParameters);
			Assert.AreEqual (1, gazonk.GenericParameters.Count);
			Assert.AreEqual ("TBang", gazonk.GenericParameters [0].Name);
		}

		[TestMethod]
        public void ReturnGenericInstance()
        {
            ModuleDefinition module = SampleInputLoader.LoadAssembly("GenericsAsm").MainModule;
			var bar = module.GetType ("Bar`1");

			var self = bar.GetMethod ("Self");
			Assert.IsNotNull (self);

			var bar_t = self.ReturnType;

			Assert.IsTrue (bar_t.IsGenericInstance);

			var bar_t_instance = (GenericInstanceType) bar_t;

			Assert.AreEqual (bar.GenericParameters [0], bar_t_instance.GenericArguments [0]);

			var self_str = bar.GetMethod ("SelfString");
			Assert.IsNotNull (self_str);

			var bar_str = self_str.ReturnType;
			Assert.IsTrue (bar_str.IsGenericInstance);

			var bar_str_instance = (GenericInstanceType) bar_str;

			Assert.AreEqual ("System.String", bar_str_instance.GenericArguments [0].FullName);
		}

		[TestMethod]
        public void ReturnGenericInstanceWithMethodParameter()
        {
            ModuleDefinition module = SampleInputLoader.LoadAssembly("GenericsAsm").MainModule;
			var baz = module.GetType ("Baz");

			var gazoo = baz.GetMethod ("Gazoo");
			Assert.IsNotNull (gazoo);

			var bar_bingo = gazoo.ReturnType;

			Assert.IsTrue (bar_bingo.IsGenericInstance);

			var bar_bingo_instance = (GenericInstanceType) bar_bingo;

			Assert.AreEqual (gazoo.GenericParameters [0], bar_bingo_instance.GenericArguments [0]);
		}

		[TestMethod]
        public void SimpleOverrides()
        {
            ModuleDefinition module = SampleInputLoader.LoadAssembly("Interfaces").MainModule;
			var ibingo = module.GetType ("IBingo");
			var ibingo_foo = ibingo.GetMethod ("Foo");
			Assert.IsNotNull (ibingo_foo);

			var ibingo_bar = ibingo.GetMethod ("Bar");
			Assert.IsNotNull (ibingo_bar);

			var bingo = module.GetType ("Bingo");

			var foo = bingo.GetMethod ("IBingo.Foo");
			Assert.IsNotNull (foo);

			Assert.IsTrue (foo.HasOverrides);
			Assert.AreEqual (ibingo_foo, foo.Overrides [0]);

			var bar = bingo.GetMethod ("IBingo.Bar");
			Assert.IsNotNull (bar);

			Assert.IsTrue (bar.HasOverrides);
			Assert.AreEqual (ibingo_bar, bar.Overrides [0]);
		}

		[TestMethod]
        public void VarArgs ()
		{
            ModuleDefinition module = SampleInputLoader.LoadAssembly("varargs").MainModule;
			var module_type = module.Types [0];

			Assert.AreEqual (3, module_type.Methods.Count);

			var bar = module_type.GetMethod ("Bar");
			var baz = module_type.GetMethod ("Baz");
			var foo = module_type.GetMethod ("Foo");

            Assert.IsTrue((bar.CallingConvention & MethodCallingConvention.VarArg) != 0);
            Assert.IsFalse((baz.CallingConvention & MethodCallingConvention.VarArg) != 0);

            Assert.IsTrue((foo.CallingConvention & MethodCallingConvention.VarArg) != 0);

			var bar_reference = (MethodReference) baz.Body.Instructions.Where (i => i.Offset == 0x000a).First ().Operand;

            Assert.IsTrue((bar_reference.CallingConvention & MethodCallingConvention.VarArg) != 0);
			Assert.IsTrue (bar_reference.Parameters[0].ParameterType.IsSentinel);

			var foo_reference = (MethodReference) baz.Body.Instructions.Where (i => i.Offset == 0x0023).First ().Operand;

            Assert.IsTrue((foo_reference.CallingConvention & MethodCallingConvention.VarArg) != 0);

			Assert.IsFalse (foo_reference.Parameters[0].ParameterType.IsSentinel);
            Assert.IsTrue(foo_reference.Parameters[1].ParameterType.IsSentinel);
		}

		[TestMethod]
        public void GenericInstanceMethod()
        {
            ModuleDefinition module = SampleInputLoader.LoadAssembly("GenericsAsm").MainModule;
			var type = module.GetType ("It");
			var method = type.GetMethod ("ReadPwow");

			GenericInstanceMethod instance = null;

			foreach (var instruction in method.Body.Instructions) {
				instance = instruction.Operand as GenericInstanceMethod;
				if (instance != null)
					break;
			}

			Assert.IsNotNull (instance);

			Assert.AreEqual (TokenType.MethodSpec, instance.MetadataToken.TokenType);
			Assert.AreNotEqual (0, instance.MetadataToken.RID);
		}

		[TestMethod]
		public void MethodRefDeclaredOnGenerics ()
		{
            ModuleDefinition module = SampleInputLoader.LoadAssembly("GenericsAsm").MainModule;
			var type = module.GetType ("Tamtam");
			var beta = type.GetMethod ("Beta");
			var charlie = type.GetMethod ("Charlie");

			var new_list_beta = (MethodReference) beta.Body.Instructions.First(i=>i.OpCode != Mi.Assemblies.Cil.OpCodes.Nop).Operand;
            var new_list_charlie = (MethodReference)charlie.Body.Instructions.First(i => i.OpCode != Mi.Assemblies.Cil.OpCodes.Nop).Operand;

			Assert.AreEqual ("System.Collections.Generic.List`1<TBeta>", new_list_beta.DeclaringType.FullName);
			Assert.AreEqual ("System.Collections.Generic.List`1<TCharlie>", new_list_charlie.DeclaringType.FullName);
		}
	}
}
