using APPCOFFIE.MVVM.ViewModel;
using System.Collections.ObjectModel;
using APPCOFFIE.MVVM.Model;
namespace APPCOFFIE.MVVM.View.Mesas;

public partial class Ventas : ContentPage
{
	public Ventas(ObservableCollection<APPCOFFIE.MVVM.Model.Productos> productos)
    {
		InitializeComponent();
		BindingContext = productos;
		BindingContext = new MesasViewModel();
	}
    private async void AggPro(object sender, EventArgs e)
    {
        // Navegar a la p�gina PagePedido de manera as�ncrona
        await Navigation.PushAsync(new Pedidos());
    }
}