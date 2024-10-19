using APPCOFFIE.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using System.Collections.Generic;

using System.Security.Cryptography;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using APPCOFFIE.MVVM.View;
using APPCOFFIE.MVVM.View.Cliente;
namespace APPCOFFIE.MVVM.ViewModel
{
    public class ClienteViewModel : ObservableObject
    {
        public ObservableCollection<Clientes> Clientes { get; } = new ObservableCollection<Clientes>();
        private readonly HttpClient _httpClient;
        private readonly Page _page;
        private string _clienteId;
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public int Telefono { get; set; }
        public string Estado { get; set; }
        public string Direccion { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraReservacion { get; set; }
        public TimeSpan HoraFinanReservacion { get; set; }
        public ClienteViewModel(Page page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(Page));
            _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5002") };
            GetCommand = new Command(async () => GetCustomers());
            EliminarCommand = new Command<string>(async (id) => await EliminarClienteAsync(id));
            EditCommand = new Command<string>(async (id) => await EditClientesAsync(id));
            CrearCommand = new Command(async() => await Crearcliente());
            SaveCommand = new Command(async () => StoreCliente());
        }
        public ICommand GetCommand { get; set; }
        public ICommand EliminarCommand { get;set; }
        public ICommand EditCommand { get; set; }
        public ICommand CrearCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public async Task GetCustomers()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/ControllerCliente");
                response.EnsureSuccessStatusCode();
                var clientes = await response.Content.ReadFromJsonAsync<List<Clientes>>();
                if (clientes != null)
                {
                    Clientes.Clear();
                    foreach (var cliente in clientes){ Clientes.Add(cliente);}
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Información", "No hay clientes para mostrar.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar los clientes: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar los clientes: {ex.Message}", "OK");
            }
        }
        public async Task LoadClienteById(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/ControllerCliente/{id}");
                response.EnsureSuccessStatusCode();

                var cliente = await response.Content.ReadFromJsonAsync<Clientes>();
                if (cliente != null)
                {

                    // Asigna los valores a las propiedades
                    Nombre = cliente.Nombre;
                    Cedula = cliente.Cedula;
                    Telefono = cliente.Telefono;
                    Estado = cliente.Estado;
                    Direccion = cliente.Direccion;
                    Fecha = cliente.Fecha ?? DateTime.Now;
                    HoraReservacion = TimeSpan.Parse(cliente.HoraReservacion); // Convierte a TimeSpan
                    HoraFinanReservacion = TimeSpan.Parse(cliente.HoraFinanReservacion); // Convierte a TimeSpan
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Información", "Cliente no encontrado.", "OK");
                }
            }
            catch (HttpRequestException httpRequestEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error en la solicitud HTTP: {httpRequestEx.Message}");
                await App.Current.MainPage.DisplayAlert("Error", $"Error en la solicitud: {httpRequestEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar el cliente: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar el cliente: {ex.Message}", "OK");
            }
        }
        public async Task EliminarClienteAsync(string id)
        {
            System.Diagnostics.Debug.WriteLine($"ID del cliente a eliminar: {id}"); // Verifica el ID
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea eliminar este cliente?", "Sí", "No");
            if (confirm)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"/api/ControllerCliente/{id}");
                    response.EnsureSuccessStatusCode();

                    await App.Current.MainPage.DisplayAlert("Éxito", "Cliente eliminado correctamente", "OK");
                    await GetCustomers();
                }
                catch (HttpRequestException httpEx)
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar el cliente: {httpEx.Message}", "OK");
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar el cliente: {ex.Message}", "OK");
                }
            }
        }
        public async Task EditClientesAsync(string id)
        {
            // Navegar a la vista de editar estudiante
            await _page.Navigation.PushAsync(new EditCliente(id));
        }
        public async Task Crearcliente()
        {
            await _page.Navigation.PushAsync(new CrearCliente());
        }
        public async Task StoreCliente()
        {
            try
            {
                var nuevoCliente = new Clientes
                {
                    Nombre = Nombre ?? string.Empty,
                    Cedula = Cedula ?? string.Empty,
                    Telefono = Telefono,
                    Estado = Estado,
                    Direccion = Direccion ?? string.Empty,
                    Fecha = Fecha,
                    HoraReservacion = HoraReservacion.ToString(),  // Convertir a string
                    HoraFinanReservacion = HoraFinanReservacion.ToString()  // Convertir a string
                };

                var response = await _httpClient.PostAsJsonAsync("/api/ControllerCliente", nuevoCliente);
                response.EnsureSuccessStatusCode();

                Clientes.Add(nuevoCliente);
                await App.Current.MainPage.DisplayAlert("Éxito", "Cliente guardado correctamente", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
                GetCustomers();
            }
            catch (Exception ex)
            {
                // Aquí puedes usar un sistema de logging, por ejemplo, Serilog o NLog
                System.Diagnostics.Debug.WriteLine($"Error al guardar el cliente: {ex}");

                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar el cliente: {ex.Message}", "OK");

            }
        }
    }
}
