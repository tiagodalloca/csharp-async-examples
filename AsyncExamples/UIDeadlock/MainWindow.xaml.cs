using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UIDeadlock
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			var vm = new UIDeadlockViewModel(3);
			InitializeComponent();

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

			buttonSemDeadlock.Click += async (s, e) =>
			{
				await vm.EsperarSegundos();
				MessageBox.Show($"SIM SIM SIM, se passaram { vm.Segundos } segundos!");
			};
			buttonDeadlock.Click += (s, e) =>
			{
				vm.EsperarSegundos().Wait();
				MessageBox.Show("Nem ferrando que vai dar certo");
			};

			textBoxSegundos.PreviewTextInput += (s, e) =>
			{
				e.Handled = !uint.TryParse(e.Text, out _);
			};
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
