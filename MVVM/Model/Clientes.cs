using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPCOFFIE.MVVM.Model
{
    public class Clientes
    {
        public string Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Cedula { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public string Estado { get; set; }
        public int Telefono { get; set; }
        public DateTime? Fecha { get; set; }
        public string? HoraReservacion { get; set; }
        public string? HoraFinanReservacion { get; set; }
    }
}
