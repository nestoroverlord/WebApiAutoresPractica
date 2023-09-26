using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor : IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength: 120, ErrorMessage = "El máximo de caracteres permitidos para el campo {0} es {1}")]
        //[PrimeraLetraMayusc]
        public string Nombre { get; set; }
        //[NotMapped]
        //[Range(18, 120)]
        //public int Edad { get; set; }
        //[NotMapped]
        //[CreditCard]
        //public string TarjetaDeCredito { get; set; }
        //[Url]
        //public string URL { get; set; }

        //public int Desde { get; set; }
        //public int Hasta { get; set; }
        public List<Libro> Libros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre))
            {
                var primeraLetra = Nombre.ToString()[0].ToString();
                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra del campo debe ser mayúscula", new string[] { nameof(Nombre) });
                }
            }

            //if (Desde > Hasta)
            //{
            //    yield return new ValidationResult("El valor desde no puede ser mayor al valor hasta", new string[] { nameof(Desde) });
            //}
        }
    }
}
