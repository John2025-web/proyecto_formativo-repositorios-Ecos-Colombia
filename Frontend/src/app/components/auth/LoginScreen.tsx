import { useRef, useState } from "react";
import {
  ArrowLeft,
  CheckCircle2,
  ChevronRight,
  Eye,
  EyeOff,
  KeyRound,
  Lock,
  Mail,
  MapPin,
  RefreshCw,
  ShieldCheck,
  UserPlus,
  UserRound,
} from "lucide-react";
import { ImageWithFallback } from "@/app/components/figma/ImageWithFallback";
import colombiaLogo from "@/imports/image-removebg-preview.png";
import {
  confirmRegistrationCode,
  googleSendVerificationCode,
  isValidRealEmail,
  login,
  registerUser,
  requestPasswordRecovery,
  resendVerificationCode,
  resetPasswordWithCode,
  validateSixDigitCode,
  verifyLoginCode,
  type UserRole,
} from "@/services/api";
import { C } from "@/app/theme";
import RecaptchaControl, { type RecaptchaControlHandle } from "./RecaptchaControl";
import SupportQuickAccess from "./SupportQuickAccess";
import type { UserType } from "./types";
import "./auth.css";

type LoginScreenProps = {
  onLogin: (user: UserType) => void;
  t: (key: string) => string;
};

type AuthViewMode =
  | "login"
  | "verify-login"
  | "register"
  | "verify-register"
  | "forgot-password"
  | "reset-password"
  | "google-auth";

export default function LoginScreen({ onLogin, t }: LoginScreenProps) {
  const [viewMode, setViewMode] = useState<AuthViewMode>("login");

  // Estados de Login
  const [usuario, setUsuario] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState<UserRole>("Usuario");
  const [credentialsVerified, setCredentialsVerified] = useState(false);
  const [showPass, setShowPass] = useState(false);
  const [emailF, setEmailF] = useState(false);
  const [passF, setPassF] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [infoMessage, setInfoMessage] = useState("");
  const [captchaToken, setCaptchaToken] = useState<string | null>(null);
  const recaptchaRef = useRef<RecaptchaControlHandle | null>(null);

  // Estados de Verificación de Código (Login / Google / Registro)
  const [verificationCode, setVerificationCode] = useState("");
  const [codeValidated, setCodeValidated] = useState(false);
  const [pendingEmail, setPendingEmail] = useState("");
  const [pendingName, setPendingName] = useState("");

  // Estados de Registro integrado en el mismo apartado
  const [regName, setRegName] = useState("");
  const [regEmail, setRegEmail] = useState("");
  const [regPassword, setRegPassword] = useState("");
  const [regConfirmPassword, setRegConfirmPassword] = useState("");

  // Estados de Recuperación de Contraseña
  const [forgotIdentifier, setForgotIdentifier] = useState("");
  const [resetCode, setResetCode] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmNewPassword, setConfirmNewPassword] = useState("");

  // Estados de Inicio de Sesión con Google
  const [googleEmail, setGoogleEmail] = useState("");
  const [googleName, setGoogleName] = useState("");

  const resetCaptchaAndMessages = () => {
    setError("");
    setInfoMessage("");
    setCaptchaToken(null);
    recaptchaRef.current?.reset();
  };

  const switchView = (nextView: AuthViewMode) => {
    resetCaptchaAndMessages();
    setCredentialsVerified(false);
    setCodeValidated(false);
    setVerificationCode("");
    setResetCode("");
    setViewMode(nextView);
  };

  // Paso 1 del Login: Valida usuario/correo y contraseña y envía el código de 6 dígitos al correo desde ecos.colombia1@gmail.com
  const handleLoginSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    setInfoMessage("");

    const trimmedUser = usuario.trim();
    if (!trimmedUser || !password) {
      setError("Por favor ingresa tu usuario o correo y tu contraseña.");
      return;
    }

    setLoading(true);
    try {
      const response = await login(trimmedUser, password, role);

      if (response.requiereVerificacion) {
        setCredentialsVerified(true);
        setCodeValidated(false);
        setCaptchaToken(null);
        setPendingEmail(response.correo || pendingEmail || trimmedUser);
        setPendingName(response.nombre || pendingName || trimmedUser.split("@")[0]);
        setVerificationCode("");
        setViewMode("verify-login");
        setInfoMessage(
          response.mensaje ||
            `Hemos enviado un código de verificación de 6 dígitos desde ecos.colombia1@gmail.com a tu correo ${
              response.correo || pendingEmail
            }.`
        );
      } else {
        onLogin({
          name: response.nombre || trimmedUser.split("@")[0],
          email: response.correo || trimmedUser,
          token: response.token || "",
          role: response.rol || role,
          expiresAt: response.expiraEn
            ? Date.parse(response.expiraEn)
            : Date.now() + 25 * 60 * 1000,
        });
      }
    } catch (requestError) {
      setError(
        requestError instanceof Error
          ? requestError.message
          : "No fue posible iniciar sesión."
      );
    } finally {
      setLoading(false);
    }
  };

  // Paso 2 del Login: Primero valida el código de 6 dígitos correctamente; luego muestra y envía el reCAPTCHA para finalizar el acceso
  const handleVerifyLoginCode = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    const cleaned = verificationCode.replace(/\D/g, "");
    if (cleaned.length !== 6) {
      setError("Ingresa el código de verificación de 6 dígitos enviado a tu correo.");
      return;
    }

    if (!codeValidated) {
      setInfoMessage("");
      setLoading(true);
      try {
        const res = await validateSixDigitCode(pendingEmail || usuario.trim(), role, cleaned);
        if (res.codigoValido) {
          setCodeValidated(true);
          setCaptchaToken(null);
          setInfoMessage(
            res.mensaje ||
              "Código de 6 dígitos validado correctamente. Ahora confirma el reCAPTCHA para ingresar."
          );
        }
      } catch (valError) {
        setError(
          valError instanceof Error
            ? valError.message
            : "El código de 6 dígitos ingresado no es válido."
        );
      } finally {
        setLoading(false);
      }
      return;
    }

    if (!captchaToken) {
      setError("Completa y confirma la verificación reCAPTCHA para finalizar el inicio de sesión.");
      return;
    }

    setLoading(true);
    try {
      const verified = await verifyLoginCode(pendingEmail || usuario.trim(), role, cleaned, captchaToken);
      onLogin({
        name: verified.nombre || pendingName || pendingEmail.split("@")[0],
        email: verified.correo || pendingEmail,
        token: verified.token,
        role: verified.rol || role,
        expiresAt: verified.expiresAt,
      });
    } catch (verifyError) {
      setError(
        verifyError instanceof Error
          ? verifyError.message
          : "El código de verificación o el reCAPTCHA no son válidos."
      );
      recaptchaRef.current?.reset();
      setCaptchaToken(null);
    } finally {
      setLoading(false);
    }
  };

  // Registro integrado en el mismo panel (sin abrir otra pestaña ni modal externo)
  const handleRegisterSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    setInfoMessage("");

    if (!isValidRealEmail(regEmail)) {
      setError("Ingresa un correo electrónico real y válido (ejemplo: usuario@gmail.com).");
      return;
    }
    if (regPassword.length < 6) {
      setError("La contraseña debe tener al menos 6 caracteres.");
      return;
    }
    if (regPassword !== regConfirmPassword) {
      setError("Las contraseñas no coinciden.");
      return;
    }
    if (!captchaToken) {
      setError("Completa y confirma el código reCAPTCHA alfanumérico para crear tu cuenta.");
      return;
    }

    setLoading(true);
    try {
      const result = await registerUser({
        nombre: regName,
        correo: regEmail,
        password: regPassword,
        captchaToken,
        requiereVerificacionCorreo: true,
      });

      if (result?.requiereVerificacion) {
        setPendingEmail(regEmail.trim().toLowerCase());
        setPendingName(regName.trim());
        setVerificationCode("");
        setViewMode("verify-register");
        setInfoMessage(
          result.mensaje ||
            `Enviamos un código de verificación de 6 dígitos a ${regEmail.trim().toLowerCase()} para confirmar que tu correo existe.`
        );
      } else {
        setUsuario(regEmail.trim().toLowerCase());
        setRole("Usuario");
        switchView("login");
        setInfoMessage("Cuenta creada exitosamente. Ahora inicia sesión.");
      }
    } catch (regError) {
      setError(
        regError instanceof Error
          ? regError.message
          : "No fue posible crear la cuenta."
      );
      recaptchaRef.current?.reset();
      setCaptchaToken(null);
    } finally {
      setLoading(false);
    }
  };

  const handleConfirmRegisterCode = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    if (verificationCode.trim().length !== 6) {
      setError("Ingresa el código de 6 dígitos enviado a tu correo.");
      return;
    }

    setLoading(true);
    try {
      const res = await confirmRegistrationCode(pendingEmail, verificationCode);
      setUsuario(res.correo || pendingEmail);
      setPassword(regPassword);
      setRole("Usuario");
      switchView("login");
      setInfoMessage(
        res.mensaje ||
          "Correo confirmado y cuenta creada exitosamente. Ya puedes iniciar sesión."
      );
    } catch (confirmError) {
      setError(
        confirmError instanceof Error
          ? confirmError.message
          : "Código de verificación incorrecto."
      );
    } finally {
      setLoading(false);
    }
  };

  // Recuperación y cambio de contraseña ("¿Olvidaste tu contraseña?")
  const handleForgotPasswordSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    setInfoMessage("");

    if (!forgotIdentifier.trim()) {
      setError("Ingresa tu usuario o correo electrónico.");
      return;
    }
    if (!captchaToken) {
      setError("Completa y confirma el código reCAPTCHA alfanumérico para enviar el código de recuperación.");
      return;
    }

    setLoading(true);
    try {
      const response = await requestPasswordRecovery(
        forgotIdentifier.trim(),
        role,
        captchaToken
      );
      setPendingEmail(response.correo);
      setResetCode("");
      setNewPassword("");
      setConfirmNewPassword("");
      setViewMode("reset-password");
      setInfoMessage(
        response.mensaje ||
          `Se envió un código de recuperación al correo ${response.correo}.`
      );
    } catch (forgotError) {
      setError(
        forgotError instanceof Error
          ? forgotError.message
          : "No fue posible procesar la solicitud."
      );
      recaptchaRef.current?.reset();
      setCaptchaToken(null);
    } finally {
      setLoading(false);
    }
  };

  const handleResetPasswordSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    if (resetCode.trim().length !== 6) {
      setError("Ingresa el código de 6 dígitos que enviamos a tu correo.");
      return;
    }
    if (newPassword.length < 6) {
      setError("La nueva contraseña debe tener al menos 6 caracteres.");
      return;
    }
    if (newPassword !== confirmNewPassword) {
      setError("La confirmación de la nueva contraseña no coincide.");
      return;
    }

    setLoading(true);
    try {
      const response = await resetPasswordWithCode({
        correo: pendingEmail,
        rol: role,
        codigoVerificacion: resetCode.trim(),
        nuevaPassword: newPassword,
      });
      setUsuario(pendingEmail);
      setPassword("");
      switchView("login");
      setInfoMessage(
        response.mensaje ||
          "Contraseña actualizada correctamente. Inicia sesión con tu nueva contraseña."
      );
    } catch (resetError) {
      setError(
        resetError instanceof Error
          ? resetError.message
          : "No se pudo cambiar la contraseña."
      );
    } finally {
      setLoading(false);
    }
  };

  // Inicio de sesión con Google con verificación anti-bots y envío de código al correo seleccionado
  const handleGoogleSendCode = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    setInfoMessage("");

    const cleanEmail = googleEmail.trim().toLowerCase();
    if (!isValidRealEmail(cleanEmail)) {
      setError("Selecciona o escribe un correo de Google válido y real (no se permiten correos de bots).");
      return;
    }
    if (!captchaToken) {
      setError("Confirma el reCAPTCHA alfanumérico para verificar que no eres un bot.");
      return;
    }

    setLoading(true);
    try {
      const response = await googleSendVerificationCode({
        correo: cleanEmail,
        nombre: googleName.trim() || cleanEmail.split("@")[0],
        rol: role,
        captchaToken,
      });
      setPendingEmail(response.correo || cleanEmail);
      setPendingName(response.nombre || googleName.trim() || cleanEmail.split("@")[0]);
      setVerificationCode("");
      setViewMode("verify-login");
      setInfoMessage(
        response.mensaje ||
          `Se envió un código de verificación a ${cleanEmail} para confirmar que el correo existe.`
      );
    } catch (googleError) {
      setError(
        googleError instanceof Error
          ? googleError.message
          : "No se pudo iniciar la verificación con Google."
      );
      recaptchaRef.current?.reset();
      setCaptchaToken(null);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      className="auth-screen min-h-screen flex"
      style={{ fontFamily: "'Poppins', sans-serif", background: C.bg }}
    >
      <div className="hidden lg:flex lg:w-1/2 relative flex-col overflow-hidden">
        <img
          src="https://images.unsplash.com/photo-1768407683731-db676a1ddcd9?w=1200&h=1600&fit=crop&auto=format"
          alt="Colombia"
          className="absolute inset-0 w-full h-full object-cover"
        />
        <div
          className="absolute inset-0"
          style={{ background: "rgba(36,60,143,0.58)" }}
        />
        <div
          className="absolute top-0 left-0 right-0 h-1"
          style={{ background: C.yellow }}
        />
        <div className="relative z-10 flex flex-col justify-between h-full p-12">
          <div className="flex items-center gap-3">
            <ImageWithFallback
              src={colombiaLogo}
              alt="Logo"
              style={{ width: 44, height: 44, objectFit: "contain" }}
            />
            <span
              className="text-white text-2xl font-bold"
              style={{ fontWeight: 700 }}
            >
              Ecos <span style={{ color: C.yellow }}>Colombia</span>
            </span>
          </div>
          <div className="flex flex-col gap-6">
            <div
              className="inline-flex items-center gap-2 self-start px-4 py-1.5 rounded-full text-xs font-semibold tracking-wider uppercase"
              style={{
                background: "rgba(244,211,94,0.18)",
                color: C.yellow,
                border: "1px solid rgba(244,211,94,0.4)",
              }}
            >
              🇨🇴 Turismo colombiano
            </div>
            <h1
              className="text-white leading-tight"
              style={{
                fontSize: "clamp(2rem,3.5vw,3rem)",
                fontWeight: 700,
                letterSpacing: "-0.02em",
              }}
            >
              Explora cultura,
              <br />
              historia y destinos
              <br />
              <span style={{ color: C.yellow }}>increíbles</span> del país.
            </h1>
            <p
              className="text-white max-w-sm"
              style={{
                fontWeight: 300,
                opacity: 0.85,
                fontSize: "0.95rem",
                lineHeight: 1.7,
              }}
            >
              Más de 1.141 municipios, 5 regiones únicas y paisajes que te
              dejarán sin palabras.
            </p>
          </div>
          <div className="flex gap-3 flex-wrap">
            {[
              "Cartagena",
              "Medellín",
              "San Andrés",
              "Caño Cristales",
              "Salento",
            ].map((city) => (
              <div
                key={city}
                className="flex items-center gap-1.5 px-3 py-1.5 rounded-full text-xs font-medium"
                style={{
                  background: "rgba(255,255,255,0.14)",
                  color: "rgba(255,255,255,0.9)",
                  border: "1px solid rgba(255,255,255,0.2)",
                }}
              >
                <MapPin size={10} /> {city}
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className="w-full lg:w-1/2 flex items-center justify-center px-6 py-12 relative">
        <div className="w-full max-w-md relative z-10">
          <div className="flex items-center gap-2 mb-10 lg:hidden">
            <ImageWithFallback
              src={colombiaLogo}
              alt="Logo"
              style={{ width: 32, height: 32, objectFit: "contain" }}
            />
            <span className="font-bold text-xl" style={{ color: C.navy }}>
              Ecos <span style={{ color: C.red }}>Colombia</span>
            </span>
          </div>

          <div
            className="auth-screen__card bg-white rounded-2xl px-8 py-10"
            style={{
              boxShadow:
                "0 4px 6px rgba(36,60,143,0.04), 0 20px 60px rgba(36,60,143,0.10)",
            }}
          >
            {error && (
              <div
                className="mb-4 px-4 py-3 rounded-xl text-sm"
                style={{
                  background: "rgba(238,66,102,0.08)",
                  color: C.red,
                  border: "1px solid rgba(238,66,102,0.2)",
                }}
              >
                {error}
              </div>
            )}

            {infoMessage && (
              <div
                className="mb-4 px-4 py-3 rounded-xl text-xs"
                style={{
                  background: "rgba(16, 185, 129, 0.08)",
                  color: "#047857",
                  border: "1px solid rgba(16, 185, 129, 0.25)",
                }}
              >
                {infoMessage}
              </div>
            )}

            {/* ─── VISTA 1: INICIO DE SESIÓN ─── */}
            {viewMode === "login" && (
              <>
                <div className="mb-7">
                  <h2
                    className="font-bold mb-2"
                    style={{ fontSize: "2rem", color: C.navy, fontWeight: 700 }}
                  >
                    {t("welcome")}
                  </h2>
                  <p className="text-sm" style={{ color: "#9b9baf" }}>
                    {t("signInContinue")}
                  </p>
                </div>

                <div className="mb-5">
                  <label
                    className="text-xs font-semibold"
                    style={{ color: C.navy }}
                  >
                    Ingresar como
                  </label>
                  <div className="grid grid-cols-2 gap-2 mt-2">
                    {(["Usuario", "Administrador"] as UserRole[]).map(
                      (option) => {
                        const selected = role === option;
                        return (
                          <button
                            key={option}
                            type="button"
                            onClick={() => {
                              setRole(option);
                              setCredentialsVerified(false);
                              setCaptchaToken(null);
                            }}
                            className="flex items-center justify-center gap-2 py-2.5 rounded-xl text-xs transition-all"
                            style={{
                              color: selected ? C.white : C.navy,
                              background: selected ? C.navy : "#f7f7fb",
                              border: `1px solid ${
                                selected ? C.navy : "rgba(36,60,143,0.15)"
                              }`,
                              fontWeight: 600,
                            }}
                          >
                            {option === "Administrador" ? (
                              <ShieldCheck size={15} />
                            ) : (
                              <UserRound size={15} />
                            )}
                            {option}
                          </button>
                        );
                      }
                    )}
                  </div>
                </div>

                <form onSubmit={handleLoginSubmit} className="flex flex-col gap-5">
                  <div className="flex flex-col gap-1.5">
                    <label
                      className="text-xs font-semibold"
                      style={{ color: C.navy }}
                    >
                      Usuario o correo
                    </label>
                    <div
                      className="flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all duration-200"
                      style={{
                        border: `1.5px solid ${
                          emailF ? C.navy : "rgba(36,60,143,0.18)"
                        }`,
                        background: emailF
                          ? "rgba(36,60,143,0.025)"
                          : "#fafafa",
                        boxShadow: emailF
                          ? "0 0 0 3px rgba(36,60,143,0.08)"
                          : "none",
                      }}
                    >
                      <Mail
                        size={16}
                        style={{
                          color: emailF ? C.navy : "#b0b0c4",
                          flexShrink: 0,
                        }}
                      />
                      <input
                        type="text"
                        required
                        placeholder="Usuario o correo"
                        value={usuario}
                        onChange={(event) => {
                          setUsuario(event.target.value);
                        }}
                        onFocus={() => setEmailF(true)}
                        onBlur={() => setEmailF(false)}
                        className="flex-1 bg-transparent outline-none text-sm"
                        style={{
                          color: "#2a2a3e",
                          fontFamily: "'Poppins', sans-serif",
                        }}
                      />
                    </div>
                  </div>

                  <div className="flex flex-col gap-1.5">
                    <div className="flex items-center justify-between">
                      <label
                        className="text-xs font-semibold"
                        style={{ color: C.navy }}
                      >
                        {t("password")}
                      </label>
                      <button
                        type="button"
                        onClick={() => {
                          setForgotIdentifier(usuario);
                          switchView("forgot-password");
                        }}
                        className="text-xs font-semibold hover:opacity-70 transition-opacity"
                        style={{ color: C.red }}
                      >
                        {t("forgotPassword")}
                      </button>
                    </div>
                    <div
                      className="flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all duration-200"
                      style={{
                        border: `1.5px solid ${
                          passF ? C.navy : "rgba(36,60,143,0.18)"
                        }`,
                        background: passF
                          ? "rgba(36,60,143,0.025)"
                          : "#fafafa",
                        boxShadow: passF
                          ? "0 0 0 3px rgba(36,60,143,0.08)"
                          : "none",
                      }}
                    >
                      <Lock
                        size={16}
                        style={{
                          color: passF ? C.navy : "#b0b0c4",
                          flexShrink: 0,
                        }}
                      />
                      <input
                        type={showPass ? "text" : "password"}
                        required
                        placeholder="••••••••"
                        value={password}
                        onChange={(event) => {
                          setPassword(event.target.value);
                        }}
                        onFocus={() => setPassF(true)}
                        onBlur={() => setPassF(false)}
                        className="flex-1 bg-transparent outline-none text-sm"
                        style={{
                          color: "#2a2a3e",
                          fontFamily: "'Poppins', sans-serif",
                        }}
                      />
                      <button
                        type="button"
                        onClick={() => setShowPass(!showPass)}
                        className="hover:opacity-60 transition-opacity flex-shrink-0"
                        style={{ color: "#b0b0c4" }}
                      >
                        {showPass ? <EyeOff size={16} /> : <Eye size={16} />}
                      </button>
                    </div>
                  </div>

                  <button
                    type="submit"
                    disabled={loading}
                    className="w-full flex items-center justify-center gap-2 py-3.5 rounded-xl text-white text-sm transition-all duration-200 active:scale-95 mt-1"
                    style={{
                      background: loading ? "rgba(238,66,102,0.65)" : C.red,
                      fontWeight: 600,
                      boxShadow: "0 4px 20px rgba(238,66,102,0.30)",
                      cursor: loading ? "not-allowed" : "pointer",
                    }}
                  >
                    {loading ? (
                      <>
                        <span className="w-4 h-4 border-2 border-white/40 border-t-white rounded-full animate-spin" />
                        Enviando código de 6 dígitos al correo...
                      </>
                    ) : (
                      <>
                        {t("signIn")} <ChevronRight size={16} />
                      </>
                    )}
                  </button>

                  <div className="flex items-center gap-3 my-1">
                    <div
                      className="flex-1 h-px"
                      style={{ background: "rgba(36,60,143,0.10)" }}
                    />
                    <span className="text-xs" style={{ color: "#b0b0c4" }}>
                      {t("orContinue")}
                    </span>
                    <div
                      className="flex-1 h-px"
                      style={{ background: "rgba(36,60,143,0.10)" }}
                    />
                  </div>

                  <button
                    type="button"
                    onClick={() => switchView("google-auth")}
                    disabled={loading}
                    className="w-full flex items-center justify-center gap-3 py-3.5 rounded-xl text-sm transition-all duration-200 hover:bg-gray-50 active:scale-95"
                    style={{
                      border: "1.5px solid rgba(36,60,143,0.18)",
                      color: "#3a3a52",
                      fontWeight: 600,
                      background: C.white,
                    }}
                  >
                    <svg width="18" height="18" viewBox="0 0 24 24">
                      <path
                        d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                        fill="#4285F4"
                      />
                      <path
                        d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                        fill="#34A853"
                      />
                      <path
                        d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                        fill="#FBBC05"
                      />
                      <path
                        d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                        fill="#EA4335"
                      />
                    </svg>
                    {t("continueGoogle")}
                  </button>
                </form>

                <p
                  className="text-center text-sm mt-8"
                  style={{ color: "#9b9baf" }}
                >
                  {t("noAccount")}{" "}
                  <button
                    type="button"
                    onClick={() => switchView("register")}
                    className="font-semibold hover:opacity-70 transition-opacity"
                    style={{ color: C.navy, fontWeight: 600 }}
                  >
                    {t("register")}
                  </button>
                </p>
                <div className="text-center mt-3">
                  <SupportQuickAccess
                    defaultEmail={usuario.includes("@") ? usuario : pendingEmail}
                    roleLabel={`Zona de Login (${role})`}
                    label="Soporte o ayuda"
                    textColor="#9b9baf"
                  />
                </div>
              </>
            )}

            {/* ─── VISTA 2: REGISTRO EN EL MISMO APARTADO ─── */}
            {viewMode === "register" && (
              <div>
                <div className="mb-6">
                  <h2
                    className="font-bold mb-1"
                    style={{ fontSize: "1.85rem", color: C.navy, fontWeight: 700 }}
                  >
                    Crear cuenta
                  </h2>
                  <p className="text-sm" style={{ color: "#9b9baf" }}>
                    Regístrate con tu correo electrónico real para recibir tu código de activación.
                  </p>
                </div>

                <form onSubmit={handleRegisterSubmit} className="flex flex-col gap-4">
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold" style={{ color: C.navy }}>
                      Nombre completo
                    </label>
                    <input
                      required
                      minLength={2}
                      value={regName}
                      onChange={(event) => setRegName(event.target.value)}
                      placeholder="Nombre completo"
                      className="px-4 py-3 rounded-xl outline-none text-sm"
                      style={{
                        border: "1.5px solid rgba(36,60,143,0.18)",
                        background: "#fafafa",
                      }}
                    />
                  </div>

                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold" style={{ color: C.navy }}>
                      Usuario o correo
                    </label>
                    <input
                      required
                      type="email"
                      value={regEmail}
                      onChange={(event) => setRegEmail(event.target.value)}
                      placeholder="Correo electrónico real"
                      className="px-4 py-3 rounded-xl outline-none text-sm"
                      style={{
                        border: "1.5px solid rgba(36,60,143,0.18)",
                        background: "#fafafa",
                      }}
                    />
                  </div>

                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold" style={{ color: C.navy }}>
                      Contraseña
                    </label>
                    <input
                      required
                      minLength={6}
                      type="password"
                      value={regPassword}
                      onChange={(event) => setRegPassword(event.target.value)}
                      placeholder="Contraseña (mínimo 6 caracteres)"
                      className="px-4 py-3 rounded-xl outline-none text-sm"
                      style={{
                        border: "1.5px solid rgba(36,60,143,0.18)",
                        background: "#fafafa",
                      }}
                    />
                  </div>

                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold" style={{ color: C.navy }}>
                      Confirmar contraseña
                    </label>
                    <input
                      required
                      minLength={6}
                      type="password"
                      value={regConfirmPassword}
                      onChange={(event) => setRegConfirmPassword(event.target.value)}
                      placeholder="Confirmar contraseña"
                      className="px-4 py-3 rounded-xl outline-none text-sm"
                      style={{
                        border: "1.5px solid rgba(36,60,143,0.18)",
                        background: "#fafafa",
                      }}
                    />
                  </div>

                  <RecaptchaControl
                    recaptchaRef={recaptchaRef}
                    onChange={setCaptchaToken}
                  />

                  <button
                    type="submit"
                    disabled={loading || !captchaToken}
                    className="w-full flex items-center justify-center gap-2 py-3.5 rounded-xl text-white text-sm transition-all active:scale-95"
                    style={{
                      background:
                        loading || !captchaToken
                          ? "rgba(238,66,102,0.6)"
                          : C.red,
                      fontWeight: 600,
                      cursor:
                        loading || !captchaToken ? "not-allowed" : "pointer",
                    }}
                  >
                    {loading ? (
                      "Enviando código de verificación..."
                    ) : (
                      <>
                        <UserPlus size={16} /> Crear cuenta y verificar correo
                      </>
                    )}
                  </button>
                </form>

                <button
                  type="button"
                  onClick={() => switchView("login")}
                  className="w-full flex items-center justify-center gap-2 mt-5 text-sm font-semibold hover:opacity-75"
                  style={{ color: C.navy }}
                >
                  <ArrowLeft size={15} /> Volver al inicio de sesión
                </button>
              </div>
            )}

            {/* ─── VISTA 3: CONFIRMAR REGISTRO POR CORREO ─── */}
            {viewMode === "verify-register" && (
              <div>
                <div className="mb-6">
                  <div
                    className="inline-flex items-center gap-2 px-3 py-1 rounded-full text-xs font-semibold mb-3"
                    style={{
                      background: "rgba(36,60,143,0.08)",
                      color: C.navy,
                    }}
                  >
                    <Mail size={14} /> Confirmación de correo real
                  </div>
                  <h2
                    className="font-bold mb-2"
                    style={{ fontSize: "1.65rem", color: C.navy, fontWeight: 700 }}
                  >
                    Confirma tu correo
                  </h2>
                  <p className="text-sm" style={{ color: "#64748b" }}>
                    Ingresa el código de 6 dígitos enviado exclusivamente a la bandeja de entrada de{" "}
                    <strong style={{ color: C.navy }}>{pendingEmail}</strong>.
                  </p>
                </div>

                <form onSubmit={handleConfirmRegisterCode} className="flex flex-col gap-4">
                  <input
                    type="text"
                    inputMode="numeric"
                    maxLength={6}
                    required
                    value={verificationCode}
                    onChange={(e) =>
                      setVerificationCode(e.target.value.replace(/\D/g, "").slice(0, 6))
                    }
                    placeholder="000000"
                    className="w-full px-4 py-3.5 rounded-xl text-center text-xl font-bold tracking-[0.4em] font-mono outline-none"
                    style={{
                      border: "1.5px solid rgba(36,60,143,0.25)",
                      background: "#fafafa",
                      color: C.navy,
                    }}
                  />

                  <button
                    type="submit"
                    disabled={loading || verificationCode.trim().length !== 6}
                    className="w-full flex items-center justify-center gap-2 py-3.5 rounded-xl text-white text-sm"
                    style={{
                      background:
                        loading || verificationCode.trim().length !== 6
                          ? "rgba(238,66,102,0.6)"
                          : C.red,
                      fontWeight: 600,
                    }}
                  >
                    {loading ? "Confirmando..." : "Confirmar y activar cuenta"}
                  </button>
                </form>

                <button
                  type="button"
                  onClick={() => switchView("register")}
                  className="w-full flex items-center justify-center gap-2 mt-5 text-xs font-semibold"
                  style={{ color: C.navy }}
                >
                  <ArrowLeft size={14} /> Volver al formulario de registro
                </button>
              </div>
            )}

            {/* ─── VISTA 4: INICIAR SESIÓN CON GOOGLE (CON ANTI-BOT Y VERIFICACIÓN DE CORREO) ─── */}
            {viewMode === "google-auth" && (
              <div>
                <div className="mb-6">
                  <div className="flex items-center gap-2 mb-2">
                    <svg width="22" height="22" viewBox="0 0 24 24">
                      <path
                        d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                        fill="#4285F4"
                      />
                      <path
                        d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                        fill="#34A853"
                      />
                      <path
                        d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                        fill="#FBBC05"
                      />
                      <path
                        d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                        fill="#EA4335"
                      />
                    </svg>
                    <h2
                      className="font-bold"
                      style={{ fontSize: "1.5rem", color: C.navy, fontWeight: 700 }}
                    >
                      Continuar con Google
                    </h2>
                  </div>
                  <p className="text-xs" style={{ color: "#64748b", lineHeight: 1.6 }}>
                    Selecciona o ingresa tu cuenta de Google. Para evitar cuentas de bots, enviaremos un código de verificación a tu bandeja de entrada para confirmar que el correo existe.
                  </p>
                </div>

                <form onSubmit={handleGoogleSendCode} className="flex flex-col gap-4">
                  <div className="flex flex-col gap-1.5">
                    <label className="text-xs font-semibold" style={{ color: C.navy }}>
                      Correo de Google
                    </label>
                    <input
                      type="email"
                      required
                      value={googleEmail}
                      onChange={(e) => setGoogleEmail(e.target.value)}
                      placeholder="tucuenta@gmail.com"
                      className="px-4 py-3 rounded-xl outline-none text-sm"
                      style={{
                        border: "1.5px solid rgba(36,60,143,0.18)",
                        background: "#fafafa",
                      }}
                    />
                  </div>

                  <div className="flex flex-col gap-1.5">
                    <label className="text-xs font-semibold" style={{ color: C.navy }}>
                      Nombre de la cuenta (opcional)
                    </label>
                    <input
                      type="text"
                      value={googleName}
                      onChange={(e) => setGoogleName(e.target.value)}
                      placeholder="Tu nombre"
                      className="px-4 py-3 rounded-xl outline-none text-sm"
                      style={{
                        border: "1.5px solid rgba(36,60,143,0.18)",
                        background: "#fafafa",
                      }}
                    />
                  </div>

                  <RecaptchaControl
                    recaptchaRef={recaptchaRef}
                    onChange={setCaptchaToken}
                  />

                  <button
                    type="submit"
                    disabled={loading || !captchaToken}
                    className="w-full flex items-center justify-center gap-2 py-3.5 rounded-xl text-white text-sm transition-all active:scale-95"
                    style={{
                      background:
                        loading || !captchaToken
                          ? "rgba(36,60,143,0.55)"
                          : C.navy,
                      fontWeight: 600,
                      cursor:
                        loading || !captchaToken ? "not-allowed" : "pointer",
                    }}
                  >
                    {loading
                      ? "Enviando correo de verificación..."
                      : "Enviar código de confirmación al correo"}
                  </button>
                </form>

                <button
                  type="button"
                  onClick={() => switchView("login")}
                  className="w-full flex items-center justify-center gap-2 mt-5 text-xs font-semibold"
                  style={{ color: C.navy }}
                >
                  <ArrowLeft size={14} /> Volver al inicio de sesión
                </button>
              </div>
            )}

            {/* ─── VISTA 5: OLVIDASTE TU CONTRASEÑA (SOLICITAR CÓDIGO) ─── */}
            {viewMode === "forgot-password" && (
              <div>
                <div className="mb-6">
                  <h2
                    className="font-bold mb-1.5"
                    style={{ fontSize: "1.65rem", color: C.navy, fontWeight: 700 }}
                  >
                    Recuperar contraseña
                  </h2>
                  <p className="text-xs" style={{ color: "#64748b", lineHeight: 1.6 }}>
                    Ingresa tu usuario o correo electrónico registrado. Te enviaremos un código de seguridad de 6 dígitos a tu correo para que puedas cambiar tu contraseña.
                  </p>
                </div>

                <form onSubmit={handleForgotPasswordSubmit} className="flex flex-col gap-4">
                  <div className="flex flex-col gap-1.5">
                    <label className="text-xs font-semibold" style={{ color: C.navy }}>
                      Usuario o correo
                    </label>
                    <input
                      type="text"
                      required
                      value={forgotIdentifier}
                      onChange={(e) => setForgotIdentifier(e.target.value)}
                      placeholder="Usuario o correo"
                      className="px-4 py-3.5 rounded-xl outline-none text-sm"
                      style={{
                        border: "1.5px solid rgba(36,60,143,0.18)",
                        background: "#fafafa",
                      }}
                    />
                  </div>

                  <RecaptchaControl
                    recaptchaRef={recaptchaRef}
                    onChange={setCaptchaToken}
                  />

                  <button
                    type="submit"
                    disabled={loading || !captchaToken}
                    className="w-full flex items-center justify-center gap-2 py-3.5 rounded-xl text-white text-sm"
                    style={{
                      background:
                        loading || !captchaToken
                          ? "rgba(238,66,102,0.6)"
                          : C.red,
                      fontWeight: 600,
                      cursor:
                        loading || !captchaToken ? "not-allowed" : "pointer",
                    }}
                  >
                    {loading
                      ? "Enviando código al correo..."
                      : "Enviar código de recuperación"}
                  </button>
                </form>

                <button
                  type="button"
                  onClick={() => switchView("login")}
                  className="w-full flex items-center justify-center gap-2 mt-5 text-xs font-semibold"
                  style={{ color: C.navy }}
                >
                  <ArrowLeft size={14} /> Volver al inicio de sesión
                </button>
              </div>
            )}

            {/* ─── VISTA 6: CAMBIAR CONTRASEÑA CON CÓDIGO ─── */}
            {viewMode === "reset-password" && (
              <div>
                <div className="mb-6">
                  <h2
                    className="font-bold mb-1.5"
                    style={{ fontSize: "1.65rem", color: C.navy, fontWeight: 700 }}
                  >
                    Cambiar contraseña
                  </h2>
                  <p className="text-xs" style={{ color: "#64748b", lineHeight: 1.6 }}>
                    Ingresa el código enviado al correo{" "}
                    <strong style={{ color: C.navy }}>{pendingEmail}</strong> y escribe tu nueva contraseña.
                  </p>
                </div>

                <form onSubmit={handleResetPasswordSubmit} className="flex flex-col gap-4">
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold" style={{ color: C.navy }}>
                      Código de verificación (6 dígitos)
                    </label>
                    <input
                      type="text"
                      inputMode="numeric"
                      maxLength={6}
                      required
                      value={resetCode}
                      onChange={(e) =>
                        setResetCode(e.target.value.replace(/\D/g, "").slice(0, 6))
                      }
                      placeholder="000000"
                      className="w-full px-4 py-3 rounded-xl text-center text-lg font-bold tracking-[0.4em] font-mono outline-none"
                      style={{
                        border: "1.5px solid rgba(36,60,143,0.25)",
                        background: "#fafafa",
                        color: C.navy,
                      }}
                    />
                  </div>

                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold" style={{ color: C.navy }}>
                      Nueva contraseña
                    </label>
                    <input
                      type="password"
                      required
                      minLength={6}
                      value={newPassword}
                      onChange={(e) => setNewPassword(e.target.value)}
                      placeholder="Mínimo 6 caracteres"
                      className="px-4 py-3 rounded-xl outline-none text-sm"
                      style={{
                        border: "1.5px solid rgba(36,60,143,0.18)",
                        background: "#fafafa",
                      }}
                    />
                  </div>

                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold" style={{ color: C.navy }}>
                      Confirmar nueva contraseña
                    </label>
                    <input
                      type="password"
                      required
                      minLength={6}
                      value={confirmNewPassword}
                      onChange={(e) => setConfirmNewPassword(e.target.value)}
                      placeholder="Repite tu nueva contraseña"
                      className="px-4 py-3 rounded-xl outline-none text-sm"
                      style={{
                        border: "1.5px solid rgba(36,60,143,0.18)",
                        background: "#fafafa",
                      }}
                    />
                  </div>

                  <button
                    type="submit"
                    disabled={loading || resetCode.trim().length !== 6}
                    className="w-full flex items-center justify-center gap-2 py-3.5 rounded-xl text-white text-sm"
                    style={{
                      background:
                        loading || resetCode.trim().length !== 6
                          ? "rgba(238,66,102,0.6)"
                          : C.red,
                      fontWeight: 600,
                    }}
                  >
                    {loading ? "Guardando nueva contraseña..." : "Guardar y actualizar contraseña"}
                  </button>
                </form>

                <button
                  type="button"
                  onClick={() => switchView("login")}
                  className="w-full flex items-center justify-center gap-2 mt-5 text-xs font-semibold"
                  style={{ color: C.navy }}
                >
                  <ArrowLeft size={14} /> Cancelar y volver al login
                </button>
              </div>
            )}

            {/* ─── VISTA 7: VERIFICACIÓN DE CÓDIGO DE LOGIN (SOLO POR CORREO, SIN MOSTRAR CÓDIGO EN FRONTEND) ─── */}
            {viewMode === "verify-login" && (
              <div>
                <div className="mb-6">
                  <div
                    className="inline-flex items-center gap-2 px-3 py-1 rounded-full text-xs font-semibold mb-3"
                    style={{
                      background: "rgba(36,60,143,0.08)",
                      color: C.navy,
                    }}
                  >
                    <KeyRound size={14} /> Verificación por correo electrónico
                  </div>
                  <h2
                    className="font-bold mb-2"
                    style={{ fontSize: "1.65rem", color: C.navy, fontWeight: 700 }}
                  >
                    Revisa tu correo
                  </h2>
                  <p className="text-sm" style={{ color: "#64748b" }}>
                    Enviamos un código de 6 dígitos directamente al correo{" "}
                    <strong style={{ color: C.navy }}>{pendingEmail}</strong>.
                    Revisa tu bandeja de entrada o carpeta de spam.
                  </p>
                </div>

                <form onSubmit={handleVerifyLoginCode} className="flex flex-col gap-4">
                  <div className="flex flex-col gap-1.5">
                    <label
                      className="text-xs font-semibold"
                      style={{ color: C.navy }}
                    >
                      Código de verificación (6 dígitos)
                    </label>
                    <input
                      type="text"
                      inputMode="numeric"
                      maxLength={6}
                      required
                      value={verificationCode}
                      onChange={(e) => {
                        setVerificationCode(e.target.value.replace(/\D/g, "").slice(0, 6));
                        if (codeValidated) {
                          setCodeValidated(false);
                          setCaptchaToken(null);
                        }
                      }}
                      placeholder="000000"
                      className="w-full px-4 py-3.5 rounded-xl text-center text-xl font-bold tracking-[0.4em] font-mono outline-none"
                      style={{
                        border: `1.5px solid ${codeValidated ? "#10b981" : "rgba(36,60,143,0.25)"}`,
                        background: codeValidated ? "rgba(16, 185, 129, 0.05)" : "#fafafa",
                        color: C.navy,
                      }}
                    />
                  </div>

                  {/* El reCAPTCHA aparece y se envía únicamente una vez se valide el código de 6 dígitos correctamente */}
                  {codeValidated && (
                    <RecaptchaControl
                      recaptchaRef={recaptchaRef}
                      onChange={setCaptchaToken}
                    />
                  )}

                  <button
                    type="submit"
                    disabled={
                      loading ||
                      verificationCode.trim().length !== 6 ||
                      (codeValidated && !captchaToken)
                    }
                    className="w-full flex items-center justify-center gap-2 py-3.5 rounded-xl text-white text-sm transition-all active:scale-95"
                    style={{
                      background:
                        loading ||
                        verificationCode.trim().length !== 6 ||
                        (codeValidated && !captchaToken)
                          ? "rgba(238,66,102,0.6)"
                          : C.red,
                      fontWeight: 600,
                      boxShadow: "0 4px 20px rgba(238,66,102,0.30)",
                      cursor:
                        loading ||
                        verificationCode.trim().length !== 6 ||
                        (codeValidated && !captchaToken)
                          ? "not-allowed"
                          : "pointer",
                    }}
                  >
                    {loading ? (
                      <>
                        <span className="w-4 h-4 border-2 border-white/40 border-t-white rounded-full animate-spin" />
                        {codeValidated
                          ? "Iniciando sesión..."
                          : "Validando código de 6 dígitos..."}
                      </>
                    ) : codeValidated ? (
                      <>
                        <CheckCircle2 size={16} /> Confirmar reCAPTCHA e ingresar
                      </>
                    ) : (
                      <>
                        <CheckCircle2 size={16} /> Validar código de 6 dígitos
                      </>
                    )}
                  </button>
                </form>

                <div className="flex items-center justify-between mt-5 pt-4 border-t border-slate-100">
                  <button
                    type="button"
                    onClick={() => switchView("login")}
                    className="inline-flex items-center gap-1.5 text-xs font-semibold hover:opacity-75"
                    style={{ color: C.navy }}
                  >
                    <ArrowLeft size={14} /> Volver al inicio
                  </button>

                  <button
                    type="button"
                    onClick={async () => {
                      setError("");
                      setInfoMessage("");
                      setCodeValidated(false);
                      setCaptchaToken(null);
                      setLoading(true);
                      try {
                        const target = pendingEmail || usuario.trim();
                        const res = await resendVerificationCode(target, role);
                        if (res.correo) {
                          setPendingEmail(res.correo);
                        }
                        setVerificationCode("");
                        setInfoMessage(
                          res.mensaje ||
                            `Se envió un nuevo código de 6 dígitos a ${res.correo || target}. Tanto el nuevo código como el anterior son válidos por 30 minutos.`
                        );
                      } catch (err) {
                        if (password) {
                          try {
                            await login(pendingEmail || usuario.trim(), password, role);
                            setVerificationCode("");
                            setInfoMessage(
                              `Se envió un nuevo código de 6 dígitos a ${pendingEmail}. Tanto el nuevo código como el anterior son válidos por 30 minutos.`
                            );
                            return;
                          } catch {
                            // fallthrough
                          }
                        }
                        setError(err instanceof Error ? err.message : "No se pudo reenviar el código.");
                      } finally {
                        setLoading(false);
                      }
                    }}
                    disabled={loading}
                    className="inline-flex items-center gap-1.5 text-xs font-semibold hover:opacity-75"
                    style={{ color: C.red }}
                  >
                    <RefreshCw size={13} /> Solicitar otro código
                  </button>
                </div>
              </div>
            )}
          </div>

          <p
            className="text-center text-xs mt-6"
            style={{ color: "#c0c0d0", fontWeight: 300 }}
          >
            Al iniciar sesión, aceptas nuestros{" "}
            <a href="#" style={{ color: "#9b9baf" }}>
              {t("terms")}
            </a>{" "}
            y{" "}
            <a href="#" style={{ color: "#9b9baf" }}>
              {t("privacy")}
            </a>
            {" · "}
            <SupportQuickAccess
              defaultEmail={usuario.includes("@") ? usuario : pendingEmail}
              roleLabel={`Zona de Login (${role})`}
              label="Soporte o ayuda"
              textColor="#9b9baf"
            />
          </p>
        </div>
      </div>
    </div>
  );
}
