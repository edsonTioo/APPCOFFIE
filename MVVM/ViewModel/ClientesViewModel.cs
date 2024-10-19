using APPCOFFIE.MVVM.Model;
using APPCOFFIE.MVVM.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace APPCOFFIE.MVVM.ViewModel
{
    public partial class ClientesViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;
        private readonly Page _page;
        private readonly string? _clienteId;
        public string nombre { get; set; }
        public string cedula { get; set; }
        public int telefono { get; set; }
        public string estado { get; set; }
        public string direccion { get; set; }
        public DateTime fecha { get; set; }
        public TimeSpan HoraReservacion { get; set; }
        public TimeSpan HoraFinanReservacion { get; set; }
        [ObservableProperty]
        private Clientes cliente;

        public ClientesViewModel(Page page, string? clienteId = null)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5002") };
            _page = page;
            _clienteId = clienteId;
            LoadClienteByIdCommand = new AsyncRelayCommand(LoadClienteByIdAsync);
            CommandoUpdate = new Command<string>(async (id) => await UpdateCliente(id));
            Task.Run(async () => await LoadClienteByIdAsync()).Wait();
        }
        public IAsyncRelayCommand LoadClienteByIdCommand { get; set;  }
        public ICommand CommandoUpdate { get;  set; }
        public ObservableCollection<Clientes> clientes { get; } = new ObservableCollection<Clientes>();
        public async Task LoadClienteByIdAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(_clienteId))
                {
                    var response = await _httpClient.GetAsync($"api/ControllerCliente/{_clienteId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        cliente = JsonSerializer.Deserialize<Clientes>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });// aqui en jsonReponse
                        

                    }
                    else
                    {
                        await _page.DisplayAlert("Información", "Cliente no encontrado.", "OK");
                    }
                }
                else
                {
                    await _page.DisplayAlert("Error", "El ID del cliente no es válido.", "OK");
                }
            }
            catch (Exception ex)
            {
                await _page.DisplayAlert("Error", $"Ocurrió un error al cargar el cliente: {ex.Message}", "OK");
            }
        }
        public async Task UpdateCliente(string id)
        {
            try
            {
                // Crear el objeto Cliente con los valores actuales del ViewModel
                var nuevoCliente = new Clientes
                {
                    Nombre = nombre ?? string.Empty,
                    Cedula = cedula ?? string.Empty,
                    Telefono = telefono,
                    Estado = estado,
                    Direccion = direccion ?? string.Empty,
                    Fecha = fecha,
                    HoraReservacion = HoraReservacion.ToString(),  // Convertir a string
                    HoraFinanReservacion = HoraFinanReservacion.ToString()  // Convertir a string
                };

                // Verifica si todos los valores están presentes (opcional)
                if (string.IsNullOrEmpty(nuevoCliente.Nombre) ||
                    string.IsNullOrEmpty(nuevoCliente.Cedula) ||
                    string.IsNullOrEmpty(nuevoCliente.Telefono.ToString()))
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                    return;
                }

                // Envía la solicitud PUT
                var response = await _httpClient.PutAsJsonAsync($"/api/ControllerCliente/{id}", nuevoCliente);

                // Verifica si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Actualiza el cliente en la lista si es necesario
                    var cliente = clientes.FirstOrDefault(c => c.Cedula == id);
                    if (cliente != null)
                    {
                        int index = clientes.IndexOf(cliente);
                        if (index >= 0)
                        {
                            clientes[index] = nuevoCliente; // Actualiza el cliente en la colección
                        }
                    }


                    await App.Current.MainPage.DisplayAlert("Éxito", "Cliente actualizado correctamente.", "OK");

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
