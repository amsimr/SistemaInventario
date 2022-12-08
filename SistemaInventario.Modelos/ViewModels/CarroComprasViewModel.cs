using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos.ViewModels
{
    public class CarroComprasViewModel
    {
        public Compania Compania { get; set; }  
        public BodegaProducto BodegaProducto { get; set; }
        public CarroCompras CarroCompras { get; set; }
    }
}
