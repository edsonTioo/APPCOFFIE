namespace APPCOFFIE.MVVM.View.Mesas;

public partial class HomeMesas : ContentPage
{
	public HomeMesas()
	{
		InitializeComponent();
	}
	public async void AggProducto(Object sender,EventArgs e)
	{
		if(App.Current.MainPage is NavigationPage)
		{
            await Navigation.PushAsync(new Pedidos());
        }
		else
		{
			App.Current.MainPage = new NavigationPage(new Pedidos());
        }
	}
}