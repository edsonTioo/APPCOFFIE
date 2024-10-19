using APPCOFFIE.MVVM.View.Aut;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace APPCOFFIE.MVVM.ViewModel
{
    public class HomeViewModel: ObservableObject
    {
        private readonly HttpClient _httpClient;
        private readonly Page _page;
        private readonly string _token;
        public HomeViewModel(Page page, string token)
        {
           _httpClient = new HttpClient();
            _page = page;
            _token = token;
            _httpClient.BaseAddress = new Uri("http://10.0.2.2:5002");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            InitializePage();
        }
        private async void InitializePage()
        {
            // Reemplaza "YourTargetPage" con la página que deseas abrir
            await _page.Navigation.PushAsync(new PageHome(_token));
        }
    }
}
