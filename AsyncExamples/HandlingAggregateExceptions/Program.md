```csharp
var operacoesNomes = new string[] {
		"Papel Furado", "Deus Tá Vendo", "Déjà Vu", "Houdini", "Pasárgada", "Mão Invisível", "Cana Brava", "Flash Back"
};

```
Nesse trexo de código definimos um `IEnumerable operacoes`, com a finalidade de esperar as operacoes serem realizadas ou falharem. Sabemos que as `operacoes` vao falhar.

Qual a maneira correta de capturar exceções de processos concorrentes?
```csharp
const int qtdeOperacoes = 4;
var i = new Random(DateTime.Now.Second).Next(0, operacoesNomes.Length - qtdeOperacoes);
var operacoes = Enumerable.Range(i, qtdeOperacoes).Shuffle().Select((x) => OperacaoFactory.CreateOperacao(operacoesNomes[x])());

```
Definimos a `Task operacoesWhenAll` que será completada quando todas as operacoes forem concluída.
```csharp

var operacoesWhenAll = Task.WhenAll(operacoes.ToArray());
try
{
	await operacoesWhenAll;
}
catch
{
```
Tratando a exceção
```csharp
	operacoesWhenAll.Exception.Handle((e) =>
	{
		HandleException(e);
		return true;
	});
}
```
