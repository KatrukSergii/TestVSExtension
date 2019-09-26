using System;
using System.Collections.Generic;

namespace TestExtension
{
	public sealed class InversionContainer
	{
		private readonly Dictionary<Type, object> objects;
		private static InversionContainer instance;

		public static InversionContainer Instance => InversionContainer.instance ?? (InversionContainer.instance = new InversionContainer());

		private InversionContainer()
		{
			this.objects = new Dictionary<Type, object>();
		}

		public void Register<T>(object instance) => this.objects[typeof(T)] = instance;

		public T Reslove<T>() where T: class => this.objects.ContainsKey(typeof(T)) ? this.objects[typeof(T)] as T : null;
	}
}