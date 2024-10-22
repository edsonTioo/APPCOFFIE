namespace APPCOFFIE.MVVM.Model
{
    public class Productos
    {
        public string? Id { get; set; }
        public string Producto { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public int Precio_venta { get; set; }
        public string Categoria { get; set; } = null!;
    }
}
