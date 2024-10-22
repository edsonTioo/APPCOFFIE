using APPCOFFIE.MVVM.Model;
using APPCOFFIE.MVVM.View.Mesas;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace APPCOFFIE.MVVM.ViewModel
{
    public class MesasViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;
        public ObservableCollection<Productos> Productos { get; } = new ObservableCollection<Productos>();

        // Lista para almacenar los productos seleccionados
        public ObservableCollection<Productos> productosSeleccionados = new ObservableCollection<Productos>();
        public MesasViewModel()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5002") };
            LoadAsyncProducto();
        }
        
        public async Task LoadAsyncProducto()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/ControllerProductos");
                response.EnsureSuccessStatusCode();
                var productos = await response.Content.ReadFromJsonAsync<List<Productos>>();
                if (productos != null)
                {
                    Productos.Clear();
                    foreach (var producto in productos)
                    {
                        Productos.Add(producto);
                        Debug.WriteLine($"Producto agregado: {producto.Producto} - Precio: {producto.Precio_venta}");
                    }
                }
                else
                {
                    Debug.WriteLine("No se encontraron productos.");
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"");
            }

        }
        // Comando para agregar un producto a la lista
        public ICommand AgregarProductoCommand => new RelayCommand<Productos>((producto) => AgregarProducto(producto));

        // Método para agregar un producto a la lista
        private async void AgregarProducto(Productos producto)
        {
            if (producto == null)

            {
                await Application.Current.MainPage.DisplayAlert("Error", "Producto es nulo. No se puede agregar.", "OK");
                return;
            }

            if (!productosSeleccionados.Any(p => p.Producto == producto.Producto))
            {
                productosSeleccionados.Add(producto);
                await Application.Current.MainPage.DisplayAlert("Producto agregado", $"Producto: {producto.Producto} - Precio: {producto.Precio_venta}", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Producto duplicado", $"El producto {producto.Producto} ya está en la lista.", "OK");
            }
        }





        // Comando para navegar y pasar todos los productos seleccionados
        public ICommand ConfirmarCompraCommand => new RelayCommand(async () => await ConfirmarCompra());

        private async Task ConfirmarCompra()
        {
            if (productosSeleccionados.Count == 0)
            {
                // Muestra un mensaje al usuario si no hay productos seleccionados
                await Application.Current.MainPage.DisplayAlert("Advertencia", "No se ha seleccionado ningún producto.", "OK");
                return;
            }

            // Muestra un mensaje de confirmación antes de navegar a la página de ventas
            bool confirmar = await Application.Current.MainPage.DisplayAlert("Confirmar Compra",
                $"Está a punto de confirmar la compra de {productosSeleccionados.Count} productos. ¿Desea continuar?",
                "Sí", "No");

            if (confirmar)
            {
                Debug.WriteLine($"Navegando a la página con {productosSeleccionados.Count} productos.");
                // Navega a la página de ventas y pasa la lista de productos como parámetro
                await Application.Current.MainPage.Navigation.PushAsync(new Ventas(productosSeleccionados));
            }
            else
            {
                Debug.WriteLine("El usuario canceló la compra.");
            }
        }

        public ICommand EliminarProductoCommand => new RelayCommand<Productos>((producto) => EliminarProducto(producto));
        private async void EliminarProducto(Productos producto)
        {
            if (producto != null)
            {
                // Si el producto se encuentra, lo eliminamos
                productosSeleccionados.Remove(producto);

                // Mostrar mensaje de éxito
                await Application.Current.MainPage.DisplayAlert("Producto eliminado", $"Producto: {producto.Producto} ha sido eliminado de la lista.", "OK");

            }
            else
            {
                // Si el producto no se encuentra en la lista, mostrar mensaje de error
                await Application.Current.MainPage.DisplayAlert("Error", "Producto no encontrado.", "OK");
            }
        }
    }

}

