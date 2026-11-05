import { useRef, useState } from "react";
import { ArrowLeft, UserPlus, X } from "lucide-react";
import ReCAPTCHA from "react-google-recaptcha";
import { registerUser } from "@/services/api";
import { C } from "@/app/theme";
import RecaptchaControl from "./RecaptchaControl";

type RegisterModalProps = {
  onClose: () => void;
  onRegistered: (email: string) => void;
};

export default function RegisterModal({ onClose, onRegistered }: RegisterModalProps) {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [captchaToken, setCaptchaToken] = useState<string | null>(null);
  const recaptchaRef = useRef<ReCAPTCHA | null>(null);

  const handleRegister = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    if (password !== confirmPassword) {
      setError("Las contraseñas no coinciden.");
      return;
    }
    if (!captchaToken) {
      setError("Completa la verificación reCAPTCHA para crear tu cuenta.");
      return;
    }

    setLoading(true);
    try {
      await registerUser({ nombre: name, correo: email, password, captchaToken });
      onRegistered(email);
    } catch (requestError) {
      setError(requestError instanceof Error ? requestError.message : "No fue posible crear la cuenta.");
    } finally {
      setLoading(false);
      recaptchaRef.current?.reset();
      setCaptchaToken(null);
    }
  };

  return (
    <div className="fixed inset-0 z-[70] flex items-center justify-center px-5" style={{ background: "rgba(20,28,70,0.45)" }}>
      <div className="w-full max-w-md bg-white rounded-2xl px-8 py-8" style={{ boxShadow: "0 20px 60px rgba(36,60,143,0.25)" }}>
        <div className="flex items-center justify-between mb-6">
          <div>
            <h2 className="text-2xl font-bold" style={{ color: C.navy }}>Crear cuenta</h2>
            <p className="text-sm mt-1" style={{ color: "#9b9baf" }}>Tu cuenta se registrará como Usuario.</p>
          </div>
          <button type="button" onClick={onClose} aria-label="Cerrar registro" style={{ color: "#9b9baf" }}><X size={20} /></button>
        </div>
        {error && <div className="mb-4 px-4 py-3 rounded-xl text-sm" style={{ background: "rgba(238,66,102,0.08)", color: C.red }}>{error}</div>}
        <form onSubmit={handleRegister} className="flex flex-col gap-4">
          <input required minLength={2} value={name} onChange={(event) => setName(event.target.value)} placeholder="Nombre completo" className="px-4 py-3 rounded-xl outline-none text-sm" style={{ border: "1px solid rgba(36,60,143,0.18)" }} />
          <input required type="email" value={email} onChange={(event) => setEmail(event.target.value)} placeholder="Correo electrónico" className="px-4 py-3 rounded-xl outline-none text-sm" style={{ border: "1px solid rgba(36,60,143,0.18)" }} />
          <input required minLength={6} type="password" value={password} onChange={(event) => setPassword(event.target.value)} placeholder="Contraseña (mínimo 6 caracteres)" className="px-4 py-3 rounded-xl outline-none text-sm" style={{ border: "1px solid rgba(36,60,143,0.18)" }} />
          <input required minLength={6} type="password" value={confirmPassword} onChange={(event) => setConfirmPassword(event.target.value)} placeholder="Confirmar contraseña" className="px-4 py-3 rounded-xl outline-none text-sm" style={{ border: "1px solid rgba(36,60,143,0.18)" }} />
          <RecaptchaControl recaptchaRef={recaptchaRef} onChange={setCaptchaToken} />
          <button type="submit" disabled={loading || !captchaToken} className="w-full flex items-center justify-center gap-2 py-3.5 rounded-xl text-white text-sm" style={{ background: C.red, fontWeight: 600 }}>
            {loading ? "Creando cuenta..." : <><UserPlus size={16} /> Crear cuenta</>}
          </button>
        </form>
        <button type="button" onClick={onClose} className="w-full flex items-center justify-center gap-2 mt-4 text-sm" style={{ color: C.navy }}><ArrowLeft size={15} /> Volver al inicio de sesión</button>
      </div>
    </div>
  );
}