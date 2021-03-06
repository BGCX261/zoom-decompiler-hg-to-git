using System;
using System.IO;

using Mi.Assemblies;
using Mi.Assemblies.PE;
using Mi.Assemblies.Metadata;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mi.Assemblies.Tests {

	[TestClass]
	public class ImageReadTests
    {
		[TestMethod]
		public void ImageSections ()
		{
			var image = GetResourceImage ("hello.exe");

			Assert.AreEqual (3, image.Sections.Length);
			Assert.AreEqual (".text", image.Sections [0].Name);
			Assert.AreEqual (".rsrc", image.Sections [1].Name);
			Assert.AreEqual (".reloc", image.Sections [2].Name);
		}

		[TestMethod]
		public void ImageMetadataVersion ()
		{
			var image = GetResourceImage ("hello.exe");
			Assert.AreEqual (TargetRuntime.Net_2_0, image.Runtime);

			image = GetResourceImage ("hello1.exe");
			Assert.AreEqual (TargetRuntime.Net_1_1, image.Runtime);
		}

		[TestMethod]
		public void ImageModuleKind ()
		{
			var image = GetResourceImage ("hello.exe");
			Assert.AreEqual (ModuleKind.Console, image.Kind);

			image = GetResourceImage ("libhello.dll");
			Assert.AreEqual (ModuleKind.Dll, image.Kind);

			image = GetResourceImage ("hellow.exe");
			Assert.AreEqual (ModuleKind.Windows, image.Kind);
		}

		[TestMethod]
		public void MetadataHeaps ()
		{
			var image = GetResourceImage ("hello.exe");

			Assert.IsNotNull (image.TableHeap);

			Assert.IsNotNull (image.StringHeap);
			Assert.AreEqual (string.Empty, image.StringHeap.Read (0));
			Assert.AreEqual ("<Module>", image.StringHeap.Read (1));

			Assert.IsNotNull (image.UserStringHeap);
			Assert.AreEqual (string.Empty, image.UserStringHeap.Read (0));
			Assert.AreEqual ("Hello Cecil World !", image.UserStringHeap.Read (1));

			Assert.IsNotNull (image.GuidHeap);
			Assert.AreEqual (new Guid (), image.GuidHeap.Read (0));
			Assert.AreEqual (new Guid ("C3BC2BD3-2576-4D00-A80E-465B5632415F"), image.GuidHeap.Read (1));

			Assert.IsNotNull (image.BlobHeap);
			Assert.AreEqual (new byte [0], image.BlobHeap.Read (0));
		}

		[TestMethod]
		public void TablesHeap ()
		{
			var image = GetResourceImage ("hello.exe");
			var heap = image.TableHeap;

			Assert.IsNotNull (heap);

			Assert.AreEqual (1, heap [Table.Module].Length);
			Assert.AreEqual (4, heap [Table.TypeRef].Length);
			Assert.AreEqual (2, heap [Table.TypeDef].Length);
			Assert.AreEqual (0, heap [Table.Field].Length);
			Assert.AreEqual (2, heap [Table.Method].Length);
			Assert.AreEqual (4, heap [Table.MemberRef].Length);
			Assert.AreEqual (2, heap [Table.CustomAttribute].Length);
			Assert.AreEqual (1, heap [Table.Assembly].Length);
			Assert.AreEqual (1, heap [Table.AssemblyRef].Length);
		}

		[TestMethod]
		public void ReadX64Image ()
		{
			var image = GetResourceImage ("hello.x64.exe");

			Assert.AreEqual (TargetArchitecture.AMD64, image.Architecture);
			Assert.AreEqual (ModuleAttributes.ILOnly, image.Attributes);
		}

		[TestMethod]
		public void ReadIA64Image ()
		{
			var image = GetResourceImage ("hello.ia64.exe");

			Assert.AreEqual (TargetArchitecture.IA64, image.Architecture);
			Assert.AreEqual (ModuleAttributes.ILOnly, image.Attributes);
		}

		[TestMethod]
		public void ReadX86Image ()
		{
			var image = GetResourceImage ("hello.x86.exe");

			Assert.AreEqual (TargetArchitecture.I386, image.Architecture);
			Assert.AreEqual (ModuleAttributes.ILOnly | ModuleAttributes.Required32Bit, image.Attributes);
		}

		[TestMethod]
		public void ReadAnyCpuImage ()
		{
			var image = GetResourceImage ("hello.anycpu.exe");

			Assert.AreEqual (TargetArchitecture.I386, image.Architecture);
			Assert.AreEqual (ModuleAttributes.ILOnly, image.Attributes);
		}
	}
}
