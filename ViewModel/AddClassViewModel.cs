using System;
using System.Collections.Generic;

namespace ViewModel
{
	public class AddClassViewModel
	{
		private readonly IViewService viewService;
		public AddClassViewModel(IViewService viewService)
		{
			this.viewService = viewService;
		}

		public List<string> AvailableClasses => new List<string>
		{
			"Class1",
			"Class2",
			"Class3"
		};

		public string SelectedClass
		{
			get;
			set;
		}

		public Command CloseCommand => new Command(() => this.viewService.CloseWindow());
	}
}
