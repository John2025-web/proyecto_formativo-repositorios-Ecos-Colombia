namespace ApiPrueba.Models
{
    public class Login
    {
        public string Usuario { get; set; } = string.Empty;
        public string contraseña { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string CaptchaToken { get; set; } = string.Empty;
        public string? CodigoVerificacion { get; set; }
    }

    public class VerifyLoginCodeRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string CodigoVerificacion { get; set; } = string.Empty;
    }
}
