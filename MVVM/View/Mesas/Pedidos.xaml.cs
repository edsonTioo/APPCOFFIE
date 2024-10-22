using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View.Mesas;

public partial class Pedidos : ContentPage
{
	public Pedidos()
	{
		InitializeComponent();
		BindingContext = new MesasViewModel();
	}
}