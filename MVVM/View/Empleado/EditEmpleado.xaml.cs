using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View.Empleado;

public partial class EditEmpleado : ContentPage
{
    private EmpleadosViewModel viewModel;
    private string idempleados;
    public EditEmpleado(string id)
	{
		InitializeComponent();
        idempleados = id;
        viewModel = new EmpleadosViewModel(this, id);
        this.BindingContext = viewModel;

        Task.Run(async () =>
        {
            await viewModel.LoadEmpleado(id);
            // Notificar que los datos han cambiado
            OnPropertyChanged(nameof(viewModel.EmpleadosList));
        });
    }
}