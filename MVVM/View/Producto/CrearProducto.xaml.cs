using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View.Producto;

public partial class CrearProducto : ContentPage
{
	private ProductoViewModel viewModel;
	public CrearProducto()
	{
		InitializeComponent();
        viewModel = new ProductoViewModel(this);
        BindingContext = viewModel;
    }
}