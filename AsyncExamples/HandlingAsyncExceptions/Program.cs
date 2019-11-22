using System;
using System.Threading.Tasks;

namespace HandlingAsyncExceptions
{
	class Program
	{
		public static void Main(string[] args)
		{
			MainAsync().Wait();
		}

		public static async Task MainAsync()
		{
			Console.WriteLine("Hello World! A próxima operação vai falhar mas será possível tratá-la corretamente.");

			try
			{
				await Operacao1Async();
			}
			catch (Exception e)
			{
				HandleException(e);
			}

			Console.WriteLine("\nA próxima operação vai falhar mas não será possível tratá-la corretamente.");

			try
			{
				Operacao2Async();
			}
			catch (Exception e)
			{
				HandleException(e);
			}


			await Task.Delay(2000);
			Console.WriteLine("Ainda mais, não será se quer possível aguardar sua execução: ela ocorrerá de forma concorrente mas não será possível utilizar o await.");

			Console.WriteLine();
		}

		private static async Task Operacao1Async()
		{
			await Task.Delay(2000);
			throw new Exception("Oopsie. Operacao1 falhou");
		}

		private static async void Operacao2Async()
		{
			await Task.Delay(3000);
			throw new Exception("Oopsie. Operacao2 falhou");
		}

		private static void HandleException(Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}
}
