//
// MemberDefinitionCollection.cs
//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2011 Jb Evain
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

using System.Collections.ObjectModel;

namespace Mi.Assemblies {

	class MemberDefinitionCollection<T> : Collection<T> where T : IMemberDefinition {

		TypeDefinition container;

		internal MemberDefinitionCollection (TypeDefinition container)
		{
			this.container = container;
		}

		internal MemberDefinitionCollection (TypeDefinition container, int capacity)
		{
			this.container = container;
		}

        protected override void InsertItem(int index, T item)
        {
            Attach(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            Detach(oldItem);
            Attach(item);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            Detach(item);
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            foreach (var definition in this)
                Detach(definition);
            
            base.ClearItems();
        }

		void Attach (T element)
		{
			if (element.DeclaringType == container)
				return;

			if (element.DeclaringType != null)
				throw new ArgumentException ("Member already attached");

			element.DeclaringType = this.container;
		}

		static void Detach (T element)
		{
			element.DeclaringType = null;
		}
	}
}
