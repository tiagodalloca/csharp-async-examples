using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;

namespace UIDeadlock
{
	class UIDeadlockViewModel : INotifyPropertyChanged
	{
		public uint Segundos { get; set; }
		public bool Loading { get; set; }

		public UIDeadlockViewModel(uint segundos)
		{
			Segundos = segundos;
		}

		public async Task EsperarSegundos()
		{
			var ticktack = new Timer(Segundos * 1000);
			var t = new TaskCompletionSource<object>();
			ticktack.Elapsed += (s, e) =>
			{
				
				t.SetResult(null);
				ticktack.Stop();
				Loading = false;
			};

			Loading = true;
			ticktack.Start();
			await t.Task;
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
