using System.ComponentModel.DataAnnotations;

namespace pruebaApi.models
{
    public class LugarTuristico
    {
        [Key]
        public int IdLugar {  get; set; }

        [Required(ErrorMessage = "campo Requerido")]
        public string Nombre { get; set; }

        
        [Required(ErrorMessage = "campo Requerido")]
        public string Ciudad { get; set; }
        
        
        [Required(ErrorMessage = "campo Requerido")]
        public string Descripcion { get; set; }

       
        
        [Required(ErrorMessage = "campo Requerido")]
        public string Imagen {  get; set; }


        [Required(ErrorMessage = "campo Requerido")]
        public string ubicacion { get; set; }


    }
}
