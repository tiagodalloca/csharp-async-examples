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
```csharp
var t = Task.WhenAll(EnviarEmailAsync(), GravarDBAsync(), GravarArquivoAsync());
await t;
```
## Ordem importa

Suponhamos que agora tenhamos as tarefas de
* gravar um arquivo
* gravar dados no banco de dados
* enviar um email

Suponhamos ainda que queiramos realizar essas tarefas de forma async mas exatamente na ordem disposta. 

Simplesmente aguardamos (`await`) a realização de cada uma.
```csharp
await GravarArquivoAsync();
await GravarDBAsync();
await EnviarEmailAsync();
```
# Timeout e cancelamento

E se quisermos executar nossas tarefas em uma dada ordem mas interromper sua execução se passar de um tempo? Mais ainda, e se quisermos permitir o usuário cancelar um conjunto de tarefa? Podemos fazer exatamente com um `CancellationToken`.

Definimos o token (posso fornecer mais detalhes sobre o token se necessário):
```csharp
var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;

```
Combinamos nossas tarefas através do `Task.Run`.

Observe que através da propriedade `IsCancellationRequested` do token nós checamos se o _pedido_ de cancelamento foi feito.
```csharp
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
```
`completedTask` terá agora a referência para a tarefa que for completada primeiro: `t1` ou o `timeout` de 4 segundos. Caso o `timeout` tenha sido completado primeiro, requisitamos o cancelamento da tarefa, a qual ainda temos que aguardar (`await t1`) porque estamos interessados na execução de possíveis ações para revertes alterações indesejadas que foram feitas antes que o cancelamento fosse requisitado.
```csharp
var timeout = Task.Delay(TimeSpan.FromSeconds(4));
var completedTask = await Task.WhenAny(t1, timeout);
if (completedTask != t1)
{
	cts.Cancel();
	await t1;
	Console.WriteLine("\nExecução cancelada! Timeout de 4s");
}
```
