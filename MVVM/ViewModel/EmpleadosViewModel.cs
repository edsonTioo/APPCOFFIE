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
    public partial class EmpleadosViewModel : ObservableObject
    {
        public ObservableCollection<Empleado> EmpleadosList { get; set; } = new ObservableCollection<Empleado>();
        private readonly HttpClient _httpClient;
        private readonly Page _page;
        private string _empleadosId;
        public string Nombre { get; set; }
        public int Telefono { get; set; }
        public string Correo { get; set; }
        public string Estado { get; set; }
        public int Pago { get; set; }
        public string Cedula { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string DIreccion { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
        [ObservableProperty]
        private Empleado empleados;
        public EmpleadosViewModel(Page page,string? empleadoId = null)
            {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5002") };
            _page = page;
            _empleadosId = empleadoId;

        }
        public IAsyncRelayCommand LoadEmpleadosCommand { get; set; }
        public ICommand CommandUpdate { get; set; }
        public async Task LoadEmpleado(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var reponse = await _httpClient.GetAsync($"/api/ControllerEmpleados/{id}");
                    if (reponse.IsSuccessStatusCode)
                    {
                        var jsonReponse = await reponse.Content.ReadAsStringAsync();
                        empleados = JsonSerializer.Deserialize<Empleado>(jsonReponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    else
                    {
                        await _page.DisplayAlert("Información", "Empleado no encontrado.", "OK");
                    }
                }
                else
                {
                    await _page.DisplayAlert("Error", "El ID del empleado no es válido.", "OK");
                }
            }
            catch (Exception ex)
            {
                await _page.DisplayAlert("Error", $"Ocurrió un error al cargar el empleado: {ex.Message}", "OK");
            }
        }
        public async Task UpdateEmpleado(string id)
        {
            try
            {
                // Crear el objeto Cliente con los valores actuales del ViewModel
                var NuevoEmpleado = new Empleado
                {
                    Nombre = Nombre,
                    Cedula = Cedula,
                    Telefono = Telefono,
                    Estado = Estado,
                    Direccion = DIreccion,
                    Correo = Correo,
                    Pago = Pago,
                    FechaNacimiento = FechaNacimiento,
                    Password = Password,
                    Rol = Rol
                };

                // Envía la solicitud PUT
                var response = await _httpClient.PutAsJsonAsync($"/api/ControllerEmpleados/{id}", NuevoEmpleado);

                // Verifica si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {

                    var empleado = EmpleadosList.FirstOrDefault(c => c.Id == id);
                    if (empleado != null)
                    {
                        int index = EmpleadosList.IndexOf(empleado);
                        if (index >= 0)
                        {
                            EmpleadosList[index] = NuevoEmpleado; // Actualiza el cliente en la colección
                        }
                    }


                    await App.Current.MainPage.DisplayAlert("Éxito", "Empleado actualizado correctamente.", "OK");

                  
                    await _page.Navigation.PopAsync(); 
                }
                else
                {
                   
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
                System.Diagnostics.Debug.WriteLine($"Error al actualizar el empleado: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo actualizar el empleado: {ex.Message}", "OK");
            }
        }
    }
}

