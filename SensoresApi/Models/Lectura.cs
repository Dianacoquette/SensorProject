using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SensoresApi.Models;

[Table("lecturas")]
public class Lectura
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("temperatura")]
    public float Temperatura { get; set; }

    [Column("humedad")]
    public float Humedad { get; set; }

    [Column("fecha")]
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}