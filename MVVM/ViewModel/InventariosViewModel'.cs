using APPCOFFIE.MVVM.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;


namespace APPCOFFIE.MVVM.ViewModel
{
    public partial class InventariosViewModel_: ObservableObject
    {
        private readonly HttpClient _httpClient;
        private readonly Page _page;
        private string? idinventario;
        private string nombre;
        private int cantidad;
        private string descri;
        [ObservableProperty]
        private Inventario inttt;
        public InventariosViewModel_(Page page,string? InventarioId = null)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5002") };
            _page = page;
            idinventario = InventarioId;
            //LoadIventario = new AsyncRelayCommand(LoadInventario());
            Task.Run(async () => await LoadInventario(InventarioId)).Wait();
        }
        public IAsyncRelayCommand LoadIventario { get; set; }
        public ICommand IcommandUpdate { get; set; }
        public ObservableCollection<Inventario> inventarioList { get; set; } = new ObservableCollection<Inventario>();
        public async Task LoadInventario(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var response = await _httpClient.GetAsync($"/api/ControllerInventario/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonReponse = await response.Content.ReadAsStringAsync();
                        inttt = JsonSerializer.Deserialize<Inventario>(jsonReponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });// aqui en jsonReponse}
                    }
                    else
                    {
                        await _page.DisplayAlert("Información", "Producto no encontrado.", "OK");
                    }
                }
                else
                {
                    await _page.DisplayAlert("Error", "El ID del producto no es válido.", "OK");
                }
            }
            catch (Exception ex)
            {
                await _page.DisplayAlert("Error", $"Ocurrió un error al cargar el producto: {ex.Message}", "OK");
            }
        }
        public async Task UpdateCliente(string id)
        {
            try
            {
                // Crear el objeto Cliente con los valores actuales del ViewModel
                var NuevoProducto = new Inventario
                {
                    Nombre = nombre,
                    Descripcion = descri,
                    Cantidad = cantidad,
                   
                };

                // Verifica si todos los valores están presentes (opcional)
                if (string.IsNullOrEmpty(NuevoProducto.Cantidad.ToString()) ||
                    string.IsNullOrEmpty(NuevoProducto.Nombre) ||
                    string.IsNullOrEmpty(NuevoProducto.Descripcion.ToString()))
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                    return;
                }

                // Envía la solicitud PUT
                var response = await _httpClient.PutAsJsonAsync($"/api/ControllerProductos/{id}", NuevoProducto);

                // Verifica si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Actualiza el cliente en la lista si es necesario
                    var cliente = inventarioList.FirstOrDefault(c => c.Id == id);
                    if (cliente != null)
                    {
                        int index = inventarioList.IndexOf(cliente);
                        if (index >= 0)
                        {
                            inventarioList[index] = NuevoProducto; // Actualiza el cliente en la colección
                        }
                    }


                    await App.Current.MainPage.DisplayAlert("Éxito", "Inventario actualizado correctamente.", "OK");

                    // Navega de regreso a la página ClienteHome
                    await _page.Navigation.PopAsync(); // Vuelve a la página anterior
                }
                else
                {
                    // Muestra el mensaje de error de la respuesta
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await App.Current.MainPage.DisplayAlert("Error", $"Error al actualizar: {errorContent}", "OK");
                }
            }
            catch (HttpRequestException httpEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error en la solicitud: {httpEx.Message}");
                await App.Current.MainPage.DisplayAlert("Error", $"Error en la solicitud: {httpEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar el inventario: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo actualizar el inventario: {ex.Message}", "OK");
            }
        }

    }
}
