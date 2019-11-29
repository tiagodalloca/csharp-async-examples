using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UIDeadlock
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			// true dat on
			/*
			# UI Deadlock

			## Problema

			Como lidar com um método `async` dentro de um evento de uma UI?

			Nós queremos chamar o método async `EsperarSegundos`, da classe `UIDeadlockViewModel`, que retorna uma `Task` dentro de um evento de click de um botão e esperar que ele seja concluído sem travar o resto dos componentes.

			[Detalhes da classe `UIDeadlockViewModel`](UIDeadlockViewModel.md)
			*/

			var vm = new UIDeadlockViewModel(3);
			InitializeComponent();
			// ...
			// true dat off

			var labelBinding = new Binding("Segundos");
			labelBinding.Source = vm;
			labelBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			labelBinding.Converter = new LabelSegundosConverter();
			BindingOperations.SetBinding(label, ContentProperty, labelBinding);

			var textBoxSegundosBinding = new Binding("Segundos");
			textBoxSegundosBinding.Source = vm;
			textBoxSegundosBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			textBoxSegundosBinding.Converter = new TextBoxSegundosConverter();
			BindingOperations.SetBinding(textBoxSegundos, TextBox.TextProperty, textBoxSegundosBinding);

			var isEnabledBinding = new Binding("Loading");
			isEnabledBinding.Source = vm;
			isEnabledBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			isEnabledBinding.Converter = new IsEnabledConverter();
			buttonDeadlock.SetBinding(IsEnabledProperty, isEnabledBinding);
			buttonSemDeadlock.SetBinding(IsEnabledProperty, isEnabledBinding);
			textBoxSegundos.SetBinding(IsEnabledProperty, isEnabledBinding);

			// true dat on

			/*
			Temos aqui a definição dos eventos de cada botão e fica bem evidente a difereça entre eles.
			*/ 

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

			/*
			Outra coisa que funciona é `vm.EsperarSegundos().ConfigureAwait(false).GetAwaiter().GetResult()`. O problema que em algumas situações, quando existe contexto compartilhado entre as `Tasks`, você vai acabar gerando uma exceção.
			*/

			// true dat off

			textBoxSegundos.PreviewTextInput += (s, e) =>
			{
				e.Handled = !uint.TryParse(e.Text, out _);
			};
			// true da off
		}
	}

	internal class LabelSegundosConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => string.Format("Esperar {0} segundos", value);
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => uint.Parse(value.ToString().Split(' ')[1]);	
	}

	internal class TextBoxSegundosConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value.ToString();

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var val = string.IsNullOrEmpty(value.ToString()) ? "0" : value.ToString();
			return uint.Parse(val);
		}
	}

	internal class IsEnabledConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
	}

}
