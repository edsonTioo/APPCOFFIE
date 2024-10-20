using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View.Empleado;

public partial class CrearEmpleado : ContentPage
{
	public EmpleadoViewModel model;
	public CrearEmpleado()
	{
		InitializeComponent();
		model = new EmpleadoViewModel(this);
		BindingContext = model;
	}
}