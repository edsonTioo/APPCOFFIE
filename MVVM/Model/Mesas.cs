using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPCOFFIE.MVVM.Model
{
    public class Mesas
    {
        public string? Id { get; set; }
        public int NumeroMesa { get; set; }
        public int Estado { get; set; }
        public string Cliente { get; set; } = null!;
        public int Reservacion { get; set; }
        public List<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public decimal Total { get; set; }
    }
}
