using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View.Inventario;

public partial class HomeInventario : ContentPage
{
	private InventarioViewModel model_;
	public HomeInventario()
	{
		InitializeComponent();
        // Inicializar el ViewModel
        model_ = new InventarioViewModel(this);

        // Asignar el BindingContext
        BindingContext = model_;

        // Ejecutar el método para cargar los inventarios
        model_.GetInventori();
    }
}