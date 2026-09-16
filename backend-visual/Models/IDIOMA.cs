﻿using System.ComponentModel.DataAnnotations;

namespace ApiPrueba.models
{
    public class IDIOMA
    {
        [Key]
        public int CodigoIso {  get; set; }

        [Required(ErrorMessage = "campo Requerido")]
        public string IdiomaNombre { get; set; }


    }
}
