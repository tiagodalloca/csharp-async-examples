using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandlingAggregateExceptions
{
	class Program
	{

		static void Main(string[] args)
		{
			MainAsync().Wait();
		}

		private static async Task MainAsync()
		{
			Console.WriteLine();
			Console.WriteLine("Trataremos agora um agregado de exceções:\n");
			await Task.Delay(2000);

			// true dat on
			var operacoesNomes = new string[] {
					"Papel Furado", "Deus Tá Vendo", "Déjà Vu", "Houdini", "Pasárgada", "Mão Invisível", "Cana Brava", "Flash Back"
			};

			/*
			# Handling Aggregate Exceptions

			## Problema

			Nesse trexo de código definimos um `IEnumerable operacoes`, com a finalidade de esperar as operacoes serem realizadas ou falharem. Sabemos que as `operacoes` vao falhar.

			Qual a maneira correta de capturar exceções de processos concorrentes?
			*/
			const int qtdeOperacoes = 4;
			var i = new Random(DateTime.Now.Second).Next(0, operacoesNomes.Length - qtdeOperacoes);
			var operacoes = Enumerable.Range(i, qtdeOperacoes).Shuffle().Select((x) => OperacaoFactory.CreateOperacao(operacoesNomes[x])());

			/*
			
			## Como proceder
			
			Definimos a `Task operacoesWhenAll` que será completada quando todas as operacoes forem concluída.
			*/

			var operacoesWhenAll = Task.WhenAll(operacoes.ToArray());
			try
			{
				await operacoesWhenAll;
			}
			catch
			{
				/*
				Tratando a exceção
				 */
				operacoesWhenAll.Exception.Handle((e) =>
				{
					HandleException(e);
					return true;
				});
			}
			// true dat off
		}

		private static void HandleException(Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}

	class OperacaoFactory
	{
		public static Func<Task> CreateOperacao(string nomeOperacao) => async () =>
		{
			var delayMs = (new Random(DateTime.Now.Second).Next(1, 3)) * 1000;
			await Task.Delay(delayMs);
			throw new Exception($"Operação \"{nomeOperacao}\" falhou.");
		};
	}

	static class Helpers
	{
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
		{
			var newList = list.ToList();
			var rnd = new Random(DateTime.Now.Second);
			for (int i = 0; i < 2*newList.Count; i++)
			{
				var j = rnd.Next(0, newList.Count);
				var k =  rnd.Next(0, newList.Count);
				var aux = newList[j];
				newList[j] = newList[k];
				newList[k] = aux;
			}
			return newList;
		}
	}
}
