using APPCOFFIE.MVVM.ViewModel;

namespace APPCOFFIE.MVVM.View.Aut;

public partial class PageHome : ContentPage
{
	public PageHome(string token)
	{
		InitializeComponent();
		BindingContext = new HomeViewModel(this,token);
	}
}