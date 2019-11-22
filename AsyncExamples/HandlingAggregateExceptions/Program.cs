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

			var operacoesNomes = new string[] {
					"Papel Furado", "Deus Tá Vendo", "Déjà Vu", "Houdini", "Pasárgada", "Mão Invisível", "Cana Brava", "Flash Back"
			};

			const int qtdeOperacoes = 4;
			var i = new Random(DateTime.Now.Second).Next(0, operacoesNomes.Length - qtdeOperacoes);
			var operacoes = Enumerable.Range(i, qtdeOperacoes).Shuffle().Select((x) => OperacaoFactory.CreateOperacao(operacoesNomes[x])());
			var operacoesWhenAll = Task.WhenAll(operacoes.ToArray());

			try
			{
				await operacoesWhenAll;
			}
			catch
			{
				operacoesWhenAll.Exception.Handle((e) =>
				{
					HandleException(e);
					return true;
				});
			}
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
