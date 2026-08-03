using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tablas_ecoscolombia
{
    public class Region
    {
        [Key]
        public int idRegion {  get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        public List<Tadicion> Tradiciones { get; set; }
    }
}
