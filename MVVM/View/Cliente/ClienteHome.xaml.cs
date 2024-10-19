
using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View;

public partial class ClienteHome : ContentPage
{
    private ClienteViewModel viewModel;
	public ClienteHome()
	{
		InitializeComponent();
        viewModel = new ClienteViewModel(this); // Asigna el ViewModel al campo de instancia
        BindingContext = viewModel;
        viewModel.GetCustomers();
        
    }
}