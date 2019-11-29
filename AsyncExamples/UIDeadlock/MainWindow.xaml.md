# UI Deadlock

## Problema

Como lidar com um método `async` dentro de um evento de uma UI?

Nós queremos chamar o método async `EsperarSegundos`, da classe `UIDeadlockViewModel`, que retorna uma `Task` dentro de um evento de click de um botão e esperar que ele seja concluído sem travar o resto dos componentes.

[Detalhes da classe `UIDeadlockViewModel`](UIDeadlockViewModel.md)
```csharp

var vm = new UIDeadlockViewModel(3);
InitializeComponent();
// ...
```
```
Temos aqui a definição dos eventos de cada botão e fica bem evidente a difereça entre eles.
```csharp

buttonSemDeadlock.Click += async (s, e) =>
{
	// a forma correta usando await e um função async
	await vm.EsperarSegundos();
	MessageBox.Show($"SIM SIM SIM, se passaram { vm.Segundos } segundos!");
};

buttonDeadlock.Click += (s, e) =>
{
	// Wait numa thread UI gera deadlock
	vm.EsperarSegundos().Wait();
	MessageBox.Show("Nem ferrando que vai dar certo");
};

```
Outra coisa que funciona é `vm.EsperarSegundos().ConfigureAwait(false).GetAwaiter().GetResult()`. O problema que em algumas situações, quando existe contexto compartilhado entre as `Tasks`, você vai acabar gerando uma exceção.
```csharp

```
