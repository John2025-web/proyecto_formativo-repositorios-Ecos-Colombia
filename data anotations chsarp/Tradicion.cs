using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tablas_ecoscolombia
{
    public class Tradicion
    {
        public int idTradicion { get; set; }
        public string Nombre { get; set; }
        public int idRegion { get; set; }
        public DateTime? FechaCelebracion { get; set; }
        
        public Region Region { get; set; }
    }
}
