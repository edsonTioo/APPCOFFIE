using APPCOFFIE.MVVM.Model;
using APPCOFFIE.MVVM.View.Inventario;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace APPCOFFIE.MVVM.ViewModel
{
    public class InventarioViewModel
    {
        public ObservableCollection<Inventario> inventarios {  get; set; } = new ObservableCollection<Inventario>();
        private readonly HttpClient _httpClient;
        private readonly Page _page;
        public string producto;
        public int cantidad;
        public string descripcion;
        public InventarioViewModel(Page page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(Page));
            _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5002") };
            GetCommand = new Command(async () => GetInventori());
            DeleteCommand = new Command<string>(async (id) => EliminarInventario(id));
            CreateCommand = new Command(async () => CrearInventario());
            StoreCommand = new Command(async()=> StoreInventario());
            EditCommand = new Command<string>(async (id) => EditInventario(id));
        }
        public ICommand GetCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        public ICommand StoreCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public async Task GetInventori()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/ControllerInventario");
                response.EnsureSuccessStatusCode();

                // Obtener los productos desde la respuesta
                var inventori = await response.Content.ReadFromJsonAsync<List<Inventario>>();

                // Limpiar la colección antes de agregar nuevos productos
                if (inventori != null && inventori.Any())
                {
                    //Productos.Clear();  // Limpiar la colección observable
                    foreach (var producto in inventori)
                    {
                        inventarios.Add(producto);  // Añadir cada producto a la colección observable
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Información", "No hay productos para mostrar.", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar el inventario: {ex.Message}", "OK");
            }
        }
        public async Task EliminarInventario(string id)
        {
            System.Diagnostics.Debug.WriteLine($"ID del inventario a eliminar: {id}"); // Verifica el ID
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea eliminar este inventario?", "Sí", "No");
            if (confirm)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"/api/ControllerInventario/{id}");
                    response.EnsureSuccessStatusCode();

                    await App.Current.MainPage.DisplayAlert("Éxito", "Inventario eliminado correctamente", "OK");
                    await GetInventori();
                }
                catch (HttpRequestException httpEx)
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar el Inventario: {httpEx.Message}", "OK");
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar el Inventario: {ex.Message}", "OK");
                }
            }
        }
        public async Task EditInventario(string id)
        {
            await _page.Navigation.PushAsync(new EditInventaria(id));
        }
        public async Task CrearInventario()
        {
            await _page.Navigation.PushAsync(new CrearInventario());    
        }
        public async Task StoreInventario()
        {
            try
            {
                var nuevoInventario = new Inventario
                {
                    Nombre = producto,
                    Descripcion = descripcion,
                    Cantidad = cantidad,
                };

                var response = await _httpClient.PostAsJsonAsync("/api/ControllerInventario", nuevoInventario);
                response.EnsureSuccessStatusCode();

                inventarios.Add(nuevoInventario);
                await App.Current.MainPage.DisplayAlert("Éxito", "Inventario guardado correctamente", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
                GetInventori();
            }
            catch (Exception ex)
            {
                // Aquí puedes usar un sistema de logging, por ejemplo, Serilog o NLog
                System.Diagnostics.Debug.WriteLine($"Error al guardar el inventario: {ex}");

                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar el inventario: {ex.Message}", "OK");

            }
        }
    }
}
