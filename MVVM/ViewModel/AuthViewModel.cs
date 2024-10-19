using APPCOFFIE.MVVM.Model;
using APPCOFFIE.MVVM.View;
using APPCOFFIE.MVVM.View.Aut;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace APPCOFFIE.MVVM.ViewModel
{
    public partial class AuthViewModel :ObservableObject
    {
        private readonly HttpClient _httpClient;
        private readonly Page _Page;
        public ICommand LoginCommand { get; set; }
        public AuthViewModel(Page page)
        {
            _httpClient = new HttpClient();
            _Page = page;
            LoginCommand = new Command(async ()=> await LoginAsync());
        }
        [ObservableProperty]
        private string correo;
        [ObservableProperty]
        private string password;
        public async Task LoginAsync()
        {
            var user = new User
            {
                Correo = correo,
                Password = password
            };
            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://10.0.2.2:5002/api/controller/login",user);
                if (response.IsSuccessStatusCode)
                { 
                    var JsonReponse = await response.Content.ReadAsStringAsync();
                    var tokenObje = JsonSerializer.Deserialize<TokenResponse>(JsonReponse);
                    //if (tokenObje != null && !string.IsNullOrEmpty(tokenObje.Token))
                    //{
                    //    var token = tokenObje.Token;
                    //    await _Page.Navigation.PushAsync(new PageHome(token));
                    //    await _Page.DisplayAlert("Info", "Navegación exitosa", "OK");
                    //}
                    if (tokenObje != null && !string.IsNullOrEmpty(tokenObje.Token))
                    {
                        var token = tokenObje.Token;
                        //await _Page.DisplayAlert("Token", token, "OK");
                        //await _Page.Navigation.PushAsync(new ClienteHome());
                        await _Page.Navigation.PushAsync(new PageHome(token));
                    }
                    else
                    {
                        await _Page.DisplayAlert("Error", "No se pudo obtener el token de la respuesta.", "OK");
                    }
                }
                else
                {
                    await _Page.DisplayAlert("Error", "Error en el login", "OK");
                }

            }
            catch (Exception ex)
            {
                await _Page.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }
    }
}
