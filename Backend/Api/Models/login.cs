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

    public class ValidateCredentialsRequest
    {
        public string Usuario { get; set; } = string.Empty;
        public string contraseña { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }

    public class VerifyLoginCodeRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string CodigoVerificacion { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = "Usuario";
        public string CaptchaToken { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = "Usuario";
        public string CodigoVerificacion { get; set; } = string.Empty;
        public string NuevaPassword { get; set; } = string.Empty;
    }

    public class GoogleSendCodeRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string Rol { get; set; } = "Usuario";
        public string CaptchaToken { get; set; } = string.Empty;
    }

    public class VerifyRegisterCodeRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string CodigoVerificacion { get; set; } = string.Empty;
    }
}
