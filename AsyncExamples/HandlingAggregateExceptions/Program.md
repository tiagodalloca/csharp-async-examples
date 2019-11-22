Nome de operações da PF
```csharp

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
```
Handling exception
```csharp
	operacoesWhenAll.Exception.Handle((e) =>
	{
		HandleException(e);
		return true;
	});
}
```
```
