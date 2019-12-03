using System;
using System.Diagnostics;
using System.Runtime.Serialization;
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
			// true dat on
			/*
			# Awaiting Multiple Exceptions

			## Problema

			Você tem um conjunto de tarefas (`Tasks`) que você gostaria de executar.

			Como executá-las? Vamos ver cada caso:

			* Ordem não importa
			* Ordem importa
			* Timeout e cancelamento
			
			Comecemos pelo mais simples.

			## Ordem não importa

			O objetivo aqui é combinar todas as tarefas em uma só e aguardar a execução (`await`) de todas elas, sem um ordem específica para qual deve ser a ordem de execução.

			Bem direto e intuitivo.
			*/
			// true dat off

			var cronometro = new Stopwatch();

			cronometro.Start();
			// true dat on
			var t = Task.WhenAll(EnviarEmailAsync(), GravarDBAsync(), GravarArquivoAsync());
			await t;
			// true dat off
			cronometro.Stop();
			Console.WriteLine();

			Console.WriteLine($"\nTime Elapsed: { cronometro.Elapsed.TotalSeconds.ToString("0.00") }s");

			Console.WriteLine("\nUMA DE CADA VEZ AGORA\n");

			cronometro.Start();
			// true dat on
			/*
			## Ordem importa

			Suponhamos que agora tenhamos as tarefas de
			* gravar um arquivo
			* gravar dados no banco de dados
			* enviar um email
			
			Suponhamos ainda que queiramos realizar essas tarefas de forma async mas exatamente na ordem disposta. 
			
			Simplesmente aguardamos (`await`) a realização de cada uma.
			*/
			await GravarArquivoAsync();
			await GravarDBAsync();
			await EnviarEmailAsync();
			// true dat off
			cronometro.Stop();

			Console.WriteLine($"\nTime Elapsed: { cronometro.Elapsed.TotalSeconds.ToString("0.00") }s");

			Console.WriteLine("\nCOMBINANDO AS TASKS\n");

			// true dat on
			/*
			# Timeout e cancelamento

			E se quisermos executar nossas tarefas em uma dada ordem mas interromper sua execução se passar de um tempo? Mais ainda, e se quisermos permitir o usuário cancelar um conjunto de tarefa? Podemos fazer exatamente com um `CancellationToken`.

			Definimos o token (posso fornecer mais detalhes sobre o token se necessário):
			*/
			var cts = new CancellationTokenSource();
			var cancellationToken = cts.Token;

			/*
			Combinamos nossas tarefas através do `Task.Run`.

			Observe que através da propriedade `IsCancellationRequested` do token nós checamos se o _pedido_ de cancelamento foi feito.
			*/
			var t1 = Task.Run(async () => {
				try
				{
					if (cancellationToken.IsCancellationRequested) throw new CancelamentoRequisitadoException();
					await GravarArquivoAsync();
					if (cancellationToken.IsCancellationRequested) throw new CancelamentoRequisitadoException(); ;
					await GravarDBAsync();
					if (cancellationToken.IsCancellationRequested) throw new CancelamentoRequisitadoException(); ;
					await EnviarEmailAsync();
					if (cancellationToken.IsCancellationRequested) throw new CancelamentoRequisitadoException(); ;
					await MandarMensagemAsync();
				}
				catch (CancelamentoRequisitadoException e)
				{
					// desfazer alterações que tenham sido feitas
				}
			});
			/*
			`completedTask` terá agora a referência para a tarefa que for completada primeiro: `t1` ou o `timeout` de 4 segundos. Caso o `timeout` tenha sido completado primeiro, requisitamos o cancelamento da tarefa, a qual ainda temos que aguardar (`await t1`) porque estamos interessados na execução de possíveis ações para revertes alterações indesejadas que foram feitas antes que o cancelamento fosse requisitado.
			*/
			var timeout = Task.Delay(TimeSpan.FromSeconds(4));
			var completedTask = await Task.WhenAny(t1, timeout);
			if (completedTask != t1)
			{
				cts.Cancel();
				await t1;
				Console.WriteLine("\nExecução cancelada! Timeout de 4s");
			}
			// true dat off
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

		[Serializable]
		private class CancelamentoRequisitadoException : Exception
		{
			public CancelamentoRequisitadoException()
			{
			}

			public CancelamentoRequisitadoException(string message) : base(message)
			{
			}

			public CancelamentoRequisitadoException(string message, Exception innerException) : base(message, innerException)
			{
			}

			protected CancelamentoRequisitadoException(SerializationInfo info, StreamingContext context) : base(info, context)
			{
			}
		}
	}
}
