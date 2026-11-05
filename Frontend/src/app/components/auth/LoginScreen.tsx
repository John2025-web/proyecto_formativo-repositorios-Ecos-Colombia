import { useRef, useState } from "react";
import { ChevronRight, Eye, EyeOff, Lock, Mail, MapPin, ShieldCheck, UserRound } from "lucide-react";
import ReCAPTCHA from "react-google-recaptcha";
import { ImageWithFallback } from "@/app/components/figma/ImageWithFallback";
import colombiaLogo from "@/imports/image-removebg-preview.png";
import { login, type UserRole } from "@/services/api";
import { C } from "@/app/theme";
import RegisterModal from "./RegisterModal";
import RecaptchaControl from "./RecaptchaControl";
import type { UserType } from "./types";
import "./auth.css";

type LoginScreenProps = {
  onLogin: (user: UserType) => void;
  t: (key: string) => string;
};

export default function LoginScreen({ onLogin, t }: LoginScreenProps) {
  const [usuario, setUsuario] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState<UserRole>("Usuario");
  const [registerOpen, setRegisterOpen] = useState(false);
  const [showPass, setShowPass] = useState(false);
  const [emailF, setEmailF] = useState(false);
  const [passF, setPassF] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [captchaToken, setCaptchaToken] = useState<string | null>(null);
  const recaptchaRef = useRef<ReCAPTCHA | null>(null);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    if (!usuario || !password) {
      setError("Por favor completa todos los campos.");
      return;
    }
    if (!captchaToken) {
      setError("Completa la verificación reCAPTCHA para continuar.");
      return;
    }

    setLoading(true);
    try {
      const { token, expiresAt } = await login(usuario, password, role, captchaToken);
      onLogin({ name: usuario.charAt(0).toUpperCase() + usuario.slice(1), email: usuario, token, role, expiresAt });
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "No fue posible iniciar sesión.");
    } finally {
      setLoading(false);
      recaptchaRef.current?.reset();
      setCaptchaToken(null);
    }
  };

  const handleGoogle = () => setError("El inicio de sesión con Google aún no está conectado al backend.");

  return (
    <div className="auth-screen min-h-screen flex" style={{ fontFamily: "'Poppins', sans-serif", background: C.bg }}>
      <div className="hidden lg:flex lg:w-1/2 relative flex-col overflow-hidden">
        <img src="https://images.unsplash.com/photo-1768407683731-db676a1ddcd9?w=1200&h=1600&fit=crop&auto=format" alt="Colombia" className="absolute inset-0 w-full h-full object-cover" />
        <div className="absolute inset-0" style={{ background: "rgba(36,60,143,0.58)" }} />
        <div className="absolute top-0 left-0 right-0 h-1" style={{ background: C.yellow }} />
        <div className="relative z-10 flex flex-col justify-between h-full p-12">
          <div className="flex items-center gap-3">
            <ImageWithFallback src={colombiaLogo} alt="Logo" style={{ width: 44, height: 44, objectFit: "contain" }} />
            <span className="text-white text-2xl font-bold" style={{ fontWeight: 700 }}>Ecos <span style={{ color: C.yellow }}>Colombia</span></span>
          </div>
          <div className="flex flex-col gap-6">
            <div className="inline-flex items-center gap-2 self-start px-4 py-1.5 rounded-full text-xs font-semibold tracking-wider uppercase" style={{ background: "rgba(244,211,94,0.18)", color: C.yellow, border: "1px solid rgba(244,211,94,0.4)" }}>🇨🇴 Turismo colombiano</div>
            <h1 className="text-white leading-tight" style={{ fontSize: "clamp(2rem,3.5vw,3rem)", fontWeight: 700, letterSpacing: "-0.02em" }}>Explora cultura,<br />historia y destinos<br /><span style={{ color: C.yellow }}>increíbles</span> del país.</h1>
            <p className="text-white max-w-sm" style={{ fontWeight: 300, opacity: 0.85, fontSize: "0.95rem", lineHeight: 1.7 }}>Más de 1.141 municipios, 5 regiones únicas y paisajes que te dejarán sin palabras.</p>
          </div>
          <div className="flex gap-3 flex-wrap">
            {["Cartagena", "Medellín", "San Andrés", "Caño Cristales", "Salento"].map((city) => (
              <div key={city} className="flex items-center gap-1.5 px-3 py-1.5 rounded-full text-xs font-medium" style={{ background: "rgba(255,255,255,0.14)", color: "rgba(255,255,255,0.9)", border: "1px solid rgba(255,255,255,0.2)" }}><MapPin size={10} /> {city}</div>
            ))}
          </div>
        </div>
      </div>

      <div className="w-full lg:w-1/2 flex items-center justify-center px-6 py-12 relative">
        <div className="w-full max-w-md relative z-10">
          <div className="flex items-center gap-2 mb-10 lg:hidden">
            <ImageWithFallback src={colombiaLogo} alt="Logo" style={{ width: 32, height: 32, objectFit: "contain" }} />
            <span className="font-bold text-xl" style={{ color: C.navy }}>Ecos <span style={{ color: C.red }}>Colombia</span></span>
          </div>
          <div className="auth-screen__card bg-white rounded-2xl px-8 py-10" style={{ boxShadow: "0 4px 6px rgba(36,60,143,0.04), 0 20px 60px rgba(36,60,143,0.10)" }}>
            <div className="mb-8">
              <h2 className="font-bold mb-2" style={{ fontSize: "2rem", color: C.navy, fontWeight: 700 }}>{t("welcome")}</h2>
              <p className="text-sm" style={{ color: "#9b9baf" }}>{t("signInContinue")}</p>
            </div>
            {error && <div className="mb-4 px-4 py-3 rounded-xl text-sm" style={{ background: "rgba(238,66,102,0.08)", color: C.red, border: "1px solid rgba(238,66,102,0.2)" }}>{error}</div>}
            <div className="mb-5">
              <label className="text-xs font-semibold" style={{ color: C.navy }}>Ingresar como</label>
              <div className="grid grid-cols-2 gap-2 mt-2">
                {(["Usuario", "Administrador"] as UserRole[]).map((option) => {
                  const selected = role === option;
                  return <button key={option} type="button" onClick={() => setRole(option)} className="flex items-center justify-center gap-2 py-2.5 rounded-xl text-xs transition-all" style={{ color: selected ? C.white : C.navy, background: selected ? C.navy : "#f7f7fb", border: `1px solid ${selected ? C.navy : "rgba(36,60,143,0.15)"}`, fontWeight: 600 }}>
                    {option === "Administrador" ? <ShieldCheck size={15} /> : <UserRound size={15} />}{option}
                  </button>;
                })}
              </div>
            </div>
            <form onSubmit={handleSubmit} className="flex flex-col gap-5">
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold" style={{ color: C.navy }}>Usuario o correo</label>
                <div className="flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all duration-200" style={{ border: `1.5px solid ${emailF ? C.navy : "rgba(36,60,143,0.18)"}`, background: emailF ? "rgba(36,60,143,0.025)" : "#fafafa", boxShadow: emailF ? "0 0 0 3px rgba(36,60,143,0.08)" : "none" }}>
                  <Mail size={16} style={{ color: emailF ? C.navy : "#b0b0c4", flexShrink: 0 }} />
                  <input type="text" placeholder="Usuario o correo electrónico" value={usuario} onChange={(event) => setUsuario(event.target.value)} onFocus={() => setEmailF(true)} onBlur={() => setEmailF(false)} className="flex-1 bg-transparent outline-none text-sm" style={{ color: "#2a2a3e", fontFamily: "'Poppins', sans-serif" }} />
                </div>
              </div>
              <div className="flex flex-col gap-1.5">
                <div className="flex items-center justify-between">
                  <label className="text-xs font-semibold" style={{ color: C.navy }}>{t("password")}</label>
                  <a href="#" className="text-xs font-semibold hover:opacity-70 transition-opacity" style={{ color: C.red }}>{t("forgotPassword")}</a>
                </div>
                <div className="flex items-center gap-3 px-4 py-3.5 rounded-xl transition-all duration-200" style={{ border: `1.5px solid ${passF ? C.navy : "rgba(36,60,143,0.18)"}`, background: passF ? "rgba(36,60,143,0.025)" : "#fafafa", boxShadow: passF ? "0 0 0 3px rgba(36,60,143,0.08)" : "none" }}>
                  <Lock size={16} style={{ color: passF ? C.navy : "#b0b0c4", flexShrink: 0 }} />
                  <input type={showPass ? "text" : "password"} placeholder="••••••••" value={password} onChange={(event) => setPassword(event.target.value)} onFocus={() => setPassF(true)} onBlur={() => setPassF(false)} className="flex-1 bg-transparent outline-none text-sm" style={{ color: "#2a2a3e", fontFamily: "'Poppins', sans-serif" }} />
                  <button type="button" onClick={() => setShowPass(!showPass)} className="hover:opacity-60 transition-opacity flex-shrink-0" style={{ color: "#b0b0c4" }}>{showPass ? <EyeOff size={16} /> : <Eye size={16} />}</button>
                </div>
              </div>
              <RecaptchaControl recaptchaRef={recaptchaRef} onChange={setCaptchaToken} />
              <button type="submit" disabled={loading || !captchaToken} className="w-full flex items-center justify-center gap-2 py-3.5 rounded-xl text-white text-sm transition-all duration-200 active:scale-95 mt-1" style={{ background: loading ? "rgba(238,66,102,0.7)" : C.red, fontWeight: 600, boxShadow: "0 4px 20px rgba(238,66,102,0.30)", cursor: loading || !captchaToken ? "not-allowed" : "pointer" }}>
                {loading ? (<><span className="w-4 h-4 border-2 border-white/40 border-t-white rounded-full animate-spin" />{t("signingIn")}</>) : (<>{t("signIn")} <ChevronRight size={16} /></>)}
              </button>
              <div className="flex items-center gap-3 my-1">
                <div className="flex-1 h-px" style={{ background: "rgba(36,60,143,0.10)" }} />
                <span className="text-xs" style={{ color: "#b0b0c4" }}>{t("orContinue")}</span>
                <div className="flex-1 h-px" style={{ background: "rgba(36,60,143,0.10)" }} />
              </div>
              <button type="button" onClick={handleGoogle} disabled={loading} className="w-full flex items-center justify-center gap-3 py-3.5 rounded-xl text-sm transition-all duration-200 hover:bg-gray-50 active:scale-95" style={{ border: "1.5px solid rgba(36,60,143,0.18)", color: "#3a3a52", fontWeight: 600, background: C.white }}>
                <svg width="18" height="18" viewBox="0 0 24 24"><path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4" /><path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853" /><path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05" /><path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335" /></svg>
                {t("continueGoogle")}
              </button>
            </form>
            <p className="text-center text-sm mt-8" style={{ color: "#9b9baf" }}>
              {t("noAccount")} {" "}<button type="button" onClick={() => setRegisterOpen(true)} className="font-semibold hover:opacity-70 transition-opacity" style={{ color: C.navy, fontWeight: 600 }}>{t("register")}</button>
            </p>
          </div>
          <p className="text-center text-xs mt-6" style={{ color: "#c0c0d0", fontWeight: 300 }}>
            Al iniciar sesión, aceptas nuestros <a href="#" style={{ color: "#9b9baf" }}>{t("terms")}</a> y <a href="#" style={{ color: "#9b9baf" }}>{t("privacy")}</a>.
          </p>
        </div>
      </div>
      {registerOpen && <RegisterModal onClose={() => setRegisterOpen(false)} onRegistered={(registeredEmail) => { setUsuario(registeredEmail); setRole("Usuario"); setRegisterOpen(false); setError("Registro exitoso. Ahora inicia sesión como Usuario."); }} />}
    </div>
  );
}