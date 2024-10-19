using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using APPCOFFIE.MVVM.Model;
using APPCOFFIE.MVVM.View.Inventario;
using APPCOFFIE.MVVM.View.Producto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace APPCOFFIE.MVVM.ViewModel
{
    public class ProductoViewModel : ObservableObject
    {
        public ObservableCollection<Productos> Productos { get; set; } = new ObservableCollection<Productos>(); 
        private readonly HttpClient _httpClient;
        private readonly Page _page;
        // Usamos un backing field para las propiedades
        private string _producto;
        private string _descripcion;
        private int _precio_venta;
        private string _categoria;
        public string Producto
        {
            get => _producto;
            set => SetProperty(ref _producto, value); // Notifica cambios
        }
        public string Descripcion
        {
            get => _descripcion;
            set => SetProperty(ref _descripcion, value); // Notifica cambios
        }
        public int Precio_venta
        {
            get => _precio_venta;
            set => SetProperty(ref _precio_venta, value); // Notifica cambios
        }
        public string Categoria
        {
            get => _categoria;
            set => SetProperty(ref _categoria, value); // Notifica cambios
        }
        public ProductoViewModel(Page page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(Page));
            _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5002") };
            GetCommando = new Command(async () => GetInventori());
            EliminarCommand = new Command<string>(async (id) => await EliminarClienteAsync(id));
            EditCommand = new Command<string>(async (id) => await FuctionEditProducto(id));
            CrearCommand = new Command(async () => await CrearProductos());
            SaveCommand = new Command(async () => StoreProducto());

        }
        public ICommand GetCommando { get; set; }
        public ICommand EliminarCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand CrearCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public async Task GetInventori()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/ControllerProductos");
                response.EnsureSuccessStatusCode();

                // Obtener los productos desde la respuesta
                var inventori = await response.Content.ReadFromJsonAsync<List<Productos>>();

                // Limpiar la colección antes de agregar nuevos productos
                if (inventori != null && inventori.Any())
                {
                    //Productos.Clear();  // Limpiar la colección observable
                    foreach (var producto in inventori)
                    {
                        Productos.Add(producto);  // Añadir cada producto a la colección observable
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
        public async Task EliminarClienteAsync(string id)
        {
            System.Diagnostics.Debug.WriteLine($"ID del cliente a eliminar: {id}"); // Verifica el ID
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea eliminar este Prodducto?", "Sí", "No");
            if (confirm)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"/api/ControllerProductos/{id}");
                    response.EnsureSuccessStatusCode();

                    await App.Current.MainPage.DisplayAlert("Éxito", "Producto eliminado correctamente", "OK");
                    await GetInventori();
                }
                catch (HttpRequestException httpEx)
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar el producto: {httpEx.Message}", "OK");
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar el producto: {ex.Message}", "OK");
                }
            }
        }
        public async Task FuctionEditProducto(string id)
        {
            await _page.Navigation.PushAsync(new EditProducto(id));
        }
        public async Task CrearProductos()
        {
            await _page.Navigation.PushAsync(new CrearProducto());
        }
        public async Task StoreProducto()
        {
            try
            {
                var NuevoProducto = new Productos
                {
                    Producto = Producto,
                    Descripcion = Descripcion,
                    Precio_venta = Precio_venta,
                    Categoria = Categoria,
                };

                var response = await _httpClient.PostAsJsonAsync("/api/ControllerProductos", NuevoProducto);
                response.EnsureSuccessStatusCode();

                Productos.Add(NuevoProducto);
                GetInventori();
                await App.Current.MainPage.DisplayAlert("Éxito", "Producto guardado correctamente", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
                
            }
            catch (Exception ex)
            {
                // Aquí puedes usar un sistema de logging, por ejemplo, Serilog o NLog
                System.Diagnostics.Debug.WriteLine($"Error al guardar el producto: {ex}");

                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar el producto: {ex.Message}", "OK");

            }
        }
    }
}
