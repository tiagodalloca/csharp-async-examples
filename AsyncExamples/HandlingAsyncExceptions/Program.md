# Handling Async Exceptions

## Problema

Queremos podemos tratar exceções de funções async.

## Como fazer
```csharp
try
{
	await Operacao1Async();
}			
catch (Exception e)
{
	HandleException(e);
}
```
## Como não fazer
```csharp
try
{
	Operacao2Async();
}
catch (Exception e)
{
	HandleException(e);
}

```
---

Dá pra perceber devemos fazer métodos `async` que retornem um `Task`, porque caso contrário não é possível lidar com a exção
```csharp
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
```
