using System;
using System.Linq;
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

			// true dat on
			/*
			# Handling Async Exceptions

			## Problema

			Queremos podemos tratar exceções de funções async.
			
			## Como fazer
			*/
			try
			{
				await Operacao1Async();
			}			
			catch (Exception e)
			{
				HandleException(e);
			}
			// true dat off

			Console.WriteLine();
			Console.WriteLine("A próxima operação vai falhar mas não será possível tratá-la corretamente.");

			// true dat on
			/*
			## Como não fazer
			*/
			try
			{
				Operacao2Async();
			}
			catch (Exception e)
			{
				HandleException(e);
			}

			/*
			---

			Dá pra perceber devemos fazer métodos `async` que retornem um `Task`, porque caso contrário não é possível lidar com a exção
			*/
			// true dat off


			await Task.Delay(2000);
			Console.WriteLine("Ainda mais, não será se quer possível aguardar sua execução: ela ocorrerá de forma concorrente mas não será possível utilizar o await.");
			await Task.Delay(2000);
		}

		// true dat on
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
		// true dat off
	}
}
