using APPCOFFIE.MVVM.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APPCOFFIE.MVVM.ViewModel
{
    public partial  class ProductosViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;
        private readonly Page _page;
        private string _ProductoId;
        private string producto;
        private string descripcion;
        private int precio_venta;
        public string categoria;
        [ObservableProperty]
        private Productos ventarioss;
        public ProductosViewModel(Page page, string? productId = null)
        {
            {
                _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5002") };
                _page = page;
                _ProductoId = productId;
                Task.Run(async () => await LoadProduct(productId)).Wait();

            }
        }
        public IAsyncRelayCommand LoadProductoCommand { get; set; }
        public ICommand CommandUpdate { get; set; }
        public ObservableCollection<Productos> InventarioList { get; set; } = new ObservableCollection<Productos>();
        public async Task LoadProduct(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var reponse = await _httpClient.GetAsync($"/api/ControllerProductos/{id}");
                    if (reponse.IsSuccessStatusCode)
                    {
                        var jsonReponse = await reponse.Content.ReadAsStringAsync();
                        ventarioss = JsonSerializer.Deserialize<Productos>(jsonReponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });// aqui en jsonReponse}
                                                                                                                                                          // Asignar los valores a las propiedades observables
                        //producto = ventarioss.Producto;
                        //descripcion = ventarioss.Descripcion;
                        //precio_venta = ventarioss.Precio_venta;
                        //categoria = ventarioss.Categoria;
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
                var NuevoProducto = new Productos
                {
                    Producto = producto,
                    Descripcion = descripcion,
                    Precio_venta = precio_venta,
                    Categoria = categoria,
                };

                // Verifica si todos los valores están presentes (opcional)
                if (string.IsNullOrEmpty(NuevoProducto.Precio_venta.ToString()) ||
                    string.IsNullOrEmpty(NuevoProducto.Producto) ||
                    string.IsNullOrEmpty(NuevoProducto.Categoria.ToString()))
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
                    var cliente = InventarioList.FirstOrDefault(c => c.Id == id);
                    if (cliente != null)
                    {
                        int index = InventarioList.IndexOf(cliente);
                        if (index >= 0)
                        {
                            InventarioList[index] = NuevoProducto; // Actualiza el cliente en la colección
                        }
                    }


                    await App.Current.MainPage.DisplayAlert("Éxito", "Porducto actualizado correctamente.", "OK");

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
                System.Diagnostics.Debug.WriteLine($"Error al actualizar el cliente: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo actualizar el cliente: {ex.Message}", "OK");
            }
        }
    }
}
