using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPrueba.models
{
    [PrimaryKey(nameof(idCultura), nameof(idLugar))]
    public class culturaLugar
    {
        [Key]
        [ForeignKey("id_cultura")]
        public int? idCultura { get; set; }

        [Key]
        [ForeignKey("id_lugar")]
        public int? idLugar { get; set; }

    }
}

