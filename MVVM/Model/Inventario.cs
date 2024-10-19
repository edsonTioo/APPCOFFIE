using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPCOFFIE.MVVM.Model
{
    public class Inventario
    {
        public string? Id {  get; set; }   
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public string Descripcion { get; set; }

    }
}
