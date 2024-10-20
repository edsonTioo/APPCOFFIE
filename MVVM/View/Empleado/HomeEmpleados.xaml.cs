using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View.Empleado;

public partial class HomeEmpleados : ContentPage
{
	public EmpleadoViewModel model;
	public HomeEmpleados()
	{
		InitializeComponent();
		model = new EmpleadoViewModel(this);
		BindingContext = model;
		model.GetCustomers();
	}
}