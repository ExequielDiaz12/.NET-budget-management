using Budget_management.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Budget_management.Models
{
    public class TipoCuenta// : IValidatableObject// el IvalidatableObject es para la validacion del modelo
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:20, MinimumLength =3,ErrorMessage ="La longitud del campo debe estar entre los 3 y 20 caracteres")]
        [PrimeraLetraMayuscula]
        [Remote(action: "VerificarExisteTipoCuenta",controller:"TipoCuenta")]
        public string? Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) //esto es la validacion del modelo
        //{
        //    if(Nombre != null && Nombre.Length > 0)
        //    {
        //        var primeraLetra = Nombre[0].ToString();
        //        if (primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra debe ser Mayuscula", new[] { nameof(Nombre) });
        //        }
        //    }
        //}
    }
}
