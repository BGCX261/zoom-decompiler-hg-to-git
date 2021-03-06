using System;

using Mi.Assemblies;
using Mi.Assemblies.Metadata;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mi.Decompiler.Tests;

namespace Mi.Assemblies.Tests {

	[TestClass]
	public class ZAsm_EventTests {

		[TestMethod]
		public void TestEventsCS ()
		{
            var module = SampleInputLoader.LoadAssembly("Events").MainModule;

			var type = module.GetType ("Foo");

			Assert.IsTrue (type.HasEvents);

			var events = type.Events;

			Assert.AreEqual (1, events.Count);

			var @event = events [0];

			Assert.IsNotNull (@event);
			Assert.AreEqual ("Bar", @event.Name);
			Assert.IsNotNull (@event.EventType);
			Assert.AreEqual ("Pan", @event.EventType.FullName);

			Assert.IsNotNull (@event.AddMethod);
			Assert.AreEqual (MethodSemanticsAttributes.AddOn, @event.AddMethod.SemanticsAttributes);
			Assert.IsNotNull (@event.RemoveMethod);
			Assert.AreEqual (MethodSemanticsAttributes.RemoveOn, @event.RemoveMethod.SemanticsAttributes);
		}

        [Ignore]
		[TestMethod]
		public void TestOthersIL ()
		{
            ModuleDefinition module = SampleInputLoader.LoadAssembly("Others").MainModule;
			var type = module.GetType ("Others");

			Assert.IsTrue (type.HasEvents);

			var events = type.Events;

			Assert.AreEqual (1, events.Count);

			var @event = events [0];

			Assert.IsNotNull (@event);
			Assert.AreEqual ("Handler", @event.Name);
			Assert.IsNotNull (@event.EventType);
			Assert.AreEqual ("System.EventHandler", @event.EventType.FullName);

			Assert.IsTrue (@event.HasOtherMethods);

			Assert.AreEqual (2, @event.OtherMethods.Count);

			var other = @event.OtherMethods [0];
			Assert.AreEqual ("dang_Handler", other.Name);

			other = @event.OtherMethods [1];
			Assert.AreEqual ("fang_Handler", other.Name);
		}
	}
}
