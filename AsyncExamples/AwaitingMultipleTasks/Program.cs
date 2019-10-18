using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AwaitingMultipleTasks
{
	class Program
	{
		static void Main(string[] args)
		{
			MainAsync().Wait();
		}

		private async static Task MainAsync()
		{
			var cronometro = new Stopwatch();

			cronometro.Start();
			var t = Task.WhenAll(EnviarEmailAsync(), GravarDBAsync(), GravarArquivoAsync());
			Console.WriteLine();
			await t;
			cronometro.Stop();

			Console.WriteLine($"\nTime Elapsed: { cronometro.Elapsed.TotalSeconds.ToString("0.00") }s");

			Console.WriteLine("\nUMA DE CADA VEZ AGORA\n");

			cronometro.Start();
			await GravarArquivoAsync();
			await GravarDBAsync();
			await EnviarEmailAsync();
			cronometro.Stop();

			Console.WriteLine($"\nTime Elapsed: { cronometro.Elapsed.TotalSeconds.ToString("0.00") }s");

			Console.WriteLine("\nCOMBINANDO AS TASKS\n");

			var cts = new CancellationTokenSource();
			var cancellationToken = cts.Token;
			var t1 = Task.Run(async () => {
				if (cancellationToken.IsCancellationRequested) return;
				await GravarArquivoAsync();
				if (cancellationToken.IsCancellationRequested) return;
				await GravarDBAsync();
				if (cancellationToken.IsCancellationRequested) return;
				await EnviarEmailAsync();
				if (cancellationToken.IsCancellationRequested) return;
				await MandarMensagemAsync();
			});
			var timeout = Task.Delay(TimeSpan.FromSeconds(4));
			var completedTask = await Task.WhenAny(t1, timeout);
			if (completedTask != t1)
			{
				cts.Cancel();
				await t1;
				Console.WriteLine("\nExecução cancelada! Timeout de 4s");
			}
		}

		public async static Task EnviarEmailAsync()
		{
			Console.WriteLine("Enviando email...");
			await Task.Delay(TimeSpan.FromSeconds(2));
			Console.WriteLine("Email enviado com sucesso!");
		}

		public async static Task GravarDBAsync()
		{
			Console.WriteLine("Gravando no DB...");
			await Task.Delay(TimeSpan.FromSeconds(1));
			Console.WriteLine("Gravado no DB com sucesso!");
		}

		public async static Task GravarArquivoAsync()
		{
			Console.WriteLine("Gravando arquivo...");
			await Task.Delay(TimeSpan.FromSeconds(3));
			Console.WriteLine("Gravado no arquivo com sucesso!");
		}

		public async static Task MandarMensagemAsync()
		{
			Console.WriteLine("Mandando mensagem...");
			await Task.Delay(TimeSpan.FromSeconds(5));
			Console.WriteLine("Mensagem mandada!");
		}
	}
}
