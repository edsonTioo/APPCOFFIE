using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View.Aut;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
		BindingContext = new AuthViewModel(this);

    }
}