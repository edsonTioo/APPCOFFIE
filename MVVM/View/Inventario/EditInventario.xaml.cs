using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View.Inventario;

public partial class EditInventaria : ContentPage
{
    private InventariosViewModel_ model;
    private string idinventario;
    public EditInventaria(string id)
	{
		InitializeComponent();
        idinventario = id;
        model = new InventariosViewModel_(this, id);
        this.BindingContext = model;

        Task.Run(async () =>
        {
            await model.LoadInventario(id);
            // Notificar que los datos han cambiado
            OnPropertyChanged(nameof(model.Inttt));
        });
    }
}