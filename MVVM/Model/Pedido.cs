using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPCOFFIE.MVVM.Model
{
    public class Pedido
    {
        public string Producto { get; set; } = null!;
        public decimal PrecioVenta { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }
}
