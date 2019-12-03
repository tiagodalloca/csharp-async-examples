using System;
using System.Threading.Tasks;

namespace AsyncHelloWorld
{
	class Program
	{
		// true dat on
		/*
		A fazer ..
		*/
		// true dat off

		private const uint SEGUNDOS = 3;

		static void Main(string[] args)
		{
			MainAsync().Wait();

			Console.WriteLine();

			Task.Delay(TimeSpan.FromSeconds(SEGUNDOS)).Wait();
			Console.WriteLine($"Sync Hello World depois de { SEGUNDOS } segundos!");
		}

		private async static Task MainAsync()
		{
			await Task.Delay(TimeSpan.FromSeconds(SEGUNDOS));
			Console.WriteLine($"Async Hello World depois de { SEGUNDOS } segundos!");
		}
	}
}
