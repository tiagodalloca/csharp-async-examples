# Handling Aggregate Exceptions

## Problema

Nesse trexo de código definimos um `IEnumerable operacoes` com diferentes nomes escolhidos aleatoriamente a partir do array `operacoesNomes`, com a finalidade de esperar as operacoes serem realizadas ou falharem. Sabemos que as `operacoes` vao falhar.
```csharp

var operacoesNomes = new string[] {
		"Papel Furado", "Deus Tá Vendo", "Déjà Vu", "Houdini", "Pasárgada", "Mão Invisível", "Cana Brava", "Flash Back"
};

const int qtdeOperacoes = 4;
var i = new Random(DateTime.Now.Second).Next(0, operacoesNomes.Length - qtdeOperacoes);
var operacoes = Enumerable.Range(i, qtdeOperacoes).Shuffle().Select((x) => OperacaoFactory.CreateOperacao(operacoesNomes[x])());

```
Qual a maneira correta de capturar exceções de processos concorrentes?

## Como proceder

Definimos a `Task operacoesWhenAll` que será completada quando todas as operacoes forem concluída. O `await` é feito dentro de um bloco `try`. Para capturar a exceção, fazemos o uso de um bloco `catch`, que exploraremos em mais detalhes adiante.
```csharp

var operacoesWhenAll = Task.WhenAll(operacoes.ToArray());
try
{
	await operacoesWhenAll;
}
catch
{
```
Após a conclusão de todas as operações, poderemos tratar cada uma delas uma por uma. O field `Exception` da `Task` nos retorna um opcional para um `AggregateException`, o qual tem o método `Handle`. Através desse método, passamos um função que recebe cada exceção lançada pelas operações e retornamos um booleano para se a exceção foi tratada. Nesse caso vamos retornar true como default porque vamos tratar todas as operações.
```csharp
	operacoesWhenAll.Exception?.Handle((e) =>
	{
		// poderíamos fazer um pattern matching para diferentes tipos de exceção:
		// if (e is InterrupcaoInesperadaException) { ...  return true; }
		// else if (e is TimeoutException) { ...  retrun true; }
		// return false; 

		HandleException(e);
		return true;
	});
}
```
