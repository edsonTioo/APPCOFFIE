using APPCOFFIE.MVVM.View;
using APPCOFFIE.MVVM.View.Aut;
using APPCOFFIE.MVVM.View.Empleado;
using APPCOFFIE.MVVM.View.Inventario;
using APPCOFFIE.MVVM.View.Producto;

namespace APPCOFFIE
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new HomeEmpleados());
        }
    }
}