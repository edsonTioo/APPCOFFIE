using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View.Inventario;

public partial class CrearInventario : ContentPage
{
    private InventarioViewModel _lienteViewModel;
    public CrearInventario()
	{
		InitializeComponent();
        _lienteViewModel = new InventarioViewModel(this);
        BindingContext = _lienteViewModel;
    }
}