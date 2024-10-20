using APPCOFFIE.MVVM.Model;
using APPCOFFIE.MVVM.View.Empleado;
using CommunityToolkit.Mvvm.ComponentModel;
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
     public class EmpleadoViewModel : ObservableObject
    {
        public ObservableCollection<Empleado> EmpleadoList {  get; set; } = new ObservableCollection<Empleado>();
        private readonly HttpClient _httpClient;
        private readonly Page _page;
        private string _empleadoId;
        public string Nombre { get; set; }
        public int Telefono { get; set; }
        public string Correo { get; set; }
        public string Estado { get; set; }
        public int Pago { get; set; }
        public string Cedula { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string DIreccion { get; set; }
        public string Password { get; set; }
        public string Rol {  get; set; }
        public EmpleadoViewModel(Page page)
        {
            _page = page ?? throw new ArgumentNullException(nameof(Page));
            _httpClient = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5002") };
            GetCommand = new Command(async () => GetCustomers());
            EliminarCommand = new Command<string>(async (id) => await EliminarEmpleadoAsync(id));
            EditCommand = new Command<string>(async (id) => await EditEmpleadoAsync(id));
            CrearCommand = new Command(async () => await CrearEmpleados());
            SaveCommand = new Command(async () => StoreEmpleados());
        }
        public ICommand GetCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand CrearCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public async Task GetCustomers()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/ControllerEmpleados");
                response.EnsureSuccessStatusCode();
                var empleado = await response.Content.ReadFromJsonAsync<List<Empleado>>();
                if (empleado != null)
                {
                    EmpleadoList.Clear();
                    foreach (var empl in empleado) { EmpleadoList.Add(empl); }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Información", "No hay empleados para mostrar.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar los empleados: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar los empleados: {ex.Message}", "OK");
            }
        }
        public async Task LoadClienteById(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/ControllerEmpleados/{id}");
                response.EnsureSuccessStatusCode();

                var empleado = await response.Content.ReadFromJsonAsync<Empleado>();
                if (empleado != null)
                {

                    // Asigna los valores a las propiedades del ViewModel
                    Nombre = empleado.Nombre;
                    Cedula = empleado.Cedula;
                    Telefono = empleado.Telefono;
                    Estado = empleado.Estado;
                    DIreccion = empleado.Direccion;
                    Correo = empleado.Correo;
                    Pago = empleado.Pago;
                    FechaNacimiento = empleado.FechaNacimiento;
                    Password = empleado.Password;
                    Rol = empleado.Rol;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Información", "Empleado no encontrado.", "OK");
                }
            }
            catch (HttpRequestException httpRequestEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error en la solicitud HTTP: {httpRequestEx.Message}");
                await App.Current.MainPage.DisplayAlert("Error", $"Error en la solicitud: {httpRequestEx.Message}", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar el empleado: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo cargar el empleado: {ex.Message}", "OK");
            }
        }
        public async Task EliminarEmpleadoAsync(string id)
        {
            System.Diagnostics.Debug.WriteLine($"ID del empleado a eliminar: {id}"); // Verifica el ID
            bool confirm = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea eliminar este empleado?", "Sí", "No");
            if (confirm)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"/api/ControllerEmpleados/{id}");
                    response.EnsureSuccessStatusCode();

                    await App.Current.MainPage.DisplayAlert("Éxito", "Empleados eliminado correctamente", "OK");
                    await GetCustomers();
                }
                catch (HttpRequestException httpEx)
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar el empleado: {httpEx.Message}", "OK");
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"No se pudo eliminar el empleado: {ex.Message}", "OK");
                }
            }
        }
        public async Task EditEmpleadoAsync(string id)
        {
            // Navegar a la vista de editar estudiante
            await _page.Navigation.PushAsync(new EditEmpleado(id));
        }
        public async Task CrearEmpleados()
        {
            await _page.Navigation.PushAsync(new CrearEmpleado());
        }
        public async Task StoreEmpleados()
        {
            try
            {
                var nuevoempleado = new Empleado
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

                var response = await _httpClient.PostAsJsonAsync("/api/ControllerEmpleados", nuevoempleado);
                response.EnsureSuccessStatusCode();

                EmpleadoList.Add(nuevoempleado);
                await App.Current.MainPage.DisplayAlert("Éxito", "Empleado guardado correctamente", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
                GetCustomers();
            }
            catch (Exception ex)
            {
                // Aquí puedes usar un sistema de logging, por ejemplo, Serilog o NLog
                System.Diagnostics.Debug.WriteLine($"Error al guardar el empleado: {ex}");

                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar el empleado: {ex.Message}", "OK");

            }
        }
    }
}
