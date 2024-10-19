using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View.Producto;

public partial class HomeProducto : ContentPage
{
	private ProductoViewModel viewModel;
    public HomeProducto()
	{
		InitializeComponent();
        viewModel = new ProductoViewModel(this); // Asigna el ViewModel al campo de instancia
        BindingContext = viewModel;
        viewModel.GetInventori();
    }
}