import { useState } from "react";
import { CheckCircle2, Mail, Send, X } from "lucide-react";
import { isValidRealEmail, sendSupportEmail } from "@/services/api";
import { C } from "@/app/theme";

type SupportQuickAccessProps = {
  defaultEmail?: string;
  defaultName?: string;
  roleLabel?: string;
  label?: string;
  className?: string;
  textColor?: string;
  lang?: string;
};

export default function SupportQuickAccess({
  defaultEmail = "",
  defaultName = "",
  roleLabel = "Visitante (Zona de Login)",
  label = "Soporte o ayuda",
  className = "",
  textColor,
  lang = "ES",
}: SupportQuickAccessProps) {
  const isEn = lang !== "ES";
  const [open, setOpen] = useState(false);
  const [senderEmail, setSenderEmail] = useState(defaultEmail);
  const [senderName, setSenderName] = useState(defaultName);
  const [subject, setSubject] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [sentConfirmation, setSentConfirmation] = useState<{
    remitente: string;
    destinatario: string;
    asunto: string;
  } | null>(null);

  const handleOpen = () => {
    setSenderEmail(defaultEmail || senderEmail);
    setSenderName(defaultName || senderName);
    setError("");
    setSentConfirmation(null);
    setOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    const cleanEmail = senderEmail.trim().toLowerCase();
    if (!isValidRealEmail(cleanEmail)) {
      setError(
        isEn
          ? "Please enter a valid email address so we can see who is writing and reply to you."
          : "Ingresa un correo electrónico válido para que podamos ver quién escribe y responderte."
      );
      return;
    }
    if (!subject.trim() || !message.trim()) {
      setError(
        isEn
          ? "Please complete both the subject and the message."
          : "Por favor completa el asunto y el mensaje."
      );
      return;
    }

    setLoading(true);
    try {
      const response = await sendSupportEmail({
        correoRemitente: cleanEmail,
        nombreRemitente: senderName.trim() || cleanEmail.split("@")[0],
        rol: roleLabel,
        asunto: subject.trim(),
        mensaje: message.trim(),
      });
      setSentConfirmation({
        remitente: response.remitente || cleanEmail,
        destinatario: response.destinatario || "ecos.colombia1@gmail.com",
        asunto: response.asunto || subject.trim(),
      });
      setSubject("");
      setMessage("");
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : isEn
            ? "Could not send the support email."
            : "No fue posible enviar el correo de soporte."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      {/* Botón de texto limpio y no invasivo visualmente */}
      <button
        type="button"
        onClick={handleOpen}
        className={`bg-transparent border-0 p-0 m-0 text-xs font-medium hover:underline cursor-pointer transition-opacity hover:opacity-80 ${className}`}
        style={{ color: textColor || C.navy }}
      >
        {label}
      </button>

      {open && (
        <div
          className="fixed inset-0 z-[90] flex items-center justify-center px-4"
          style={{ background: "rgba(15, 23, 42, 0.45)", backdropFilter: "blur(2px)" }}
          onClick={(e) => {
            if (e.target === e.currentTarget) setOpen(false);
          }}
        >
          <div
            className="w-full max-w-md bg-white rounded-2xl p-6 relative text-left"
            style={{
              boxShadow: "0 20px 60px rgba(36, 60, 143, 0.22)",
              fontFamily: "'Poppins', sans-serif",
            }}
          >
            <div className="flex items-start justify-between gap-3 mb-4">
              <div>
                <span
                  className="inline-flex items-center gap-1.5 text-[11px] font-semibold uppercase tracking-wider px-2.5 py-0.5 rounded-full mb-1.5"
                  style={{
                    background: "rgba(36, 60, 143, 0.08)",
                    color: C.navy,
                  }}
                >
                  <Mail size={12} /> {isEn ? "Quick Access · Support & Help" : "Acceso rápido · Soporte o ayuda"}
                </span>
                <h3
                  className="text-lg font-bold"
                  style={{ color: C.navy, fontWeight: 700 }}
                >
                  {isEn ? "Contact Ecos Colombia" : "Contactar a Ecos Colombia"}
                </h3>
              </div>
              <button
                type="button"
                onClick={() => setOpen(false)}
                aria-label={isEn ? "Close support" : "Cerrar soporte"}
                className="p-1.5 rounded-lg hover:bg-slate-100 transition-colors"
                style={{ color: "#64748b" }}
              >
                <X size={18} />
              </button>
            </div>

            {/* Recuadro visible con destinatario oficial y correo de quien escribe */}
            <div
              className="rounded-xl p-3.5 mb-4 text-xs space-y-1.5"
              style={{
                background: "#f8fafc",
                border: "1px solid rgba(36, 60, 143, 0.16)",
              }}
            >
              <div className="flex items-center justify-between gap-2">
                <span style={{ color: "#64748b" }}>{isEn ? "Send directly to:" : "Enviar directamente a:"}</span>
                <span className="font-semibold" style={{ color: C.navy }}>
                  ecos.colombia1@gmail.com
                </span>
              </div>
              <div className="flex items-center justify-between gap-2">
                <span style={{ color: "#64748b" }}>{isEn ? "Sender's email:" : "Correo de quien escribe:"}</span>
                <span
                  className="font-semibold truncate max-w-[210px]"
                  style={{ color: C.red }}
                >
                  {senderEmail.trim() || (isEn ? "Enter your email below" : "Ingresa tu correo abajo")}
                </span>
              </div>
            </div>

            {error && (
              <div
                className="mb-3 px-3.5 py-2.5 rounded-xl text-xs"
                style={{
                  background: "rgba(238, 66, 102, 0.08)",
                  color: C.red,
                  border: "1px solid rgba(238, 66, 102, 0.2)",
                }}
              >
                {error}
              </div>
            )}

            {sentConfirmation ? (
              <div className="py-3 space-y-4">
                <div
                  className="p-4 rounded-xl text-xs space-y-2"
                  style={{
                    background: "rgba(16, 185, 129, 0.08)",
                    border: "1px solid rgba(16, 185, 129, 0.3)",
                    color: "#065f46",
                  }}
                >
                  <div className="flex items-center gap-2 font-bold text-sm">
                    <CheckCircle2 size={18} /> {isEn ? "Email sent successfully" : "Correo enviado correctamente"}
                  </div>
                  <p>
                    {isEn ? "Your request was sent to " : "Se envió tu solicitud a "}
                    <strong>{sentConfirmation.destinatario}</strong>.
                  </p>
                  <p>
                    {isEn
                      ? "In the email received by Ecos Colombia it is registered that it was sent by: "
                      : "En el correo recibido por Ecos Colombia aparece registrado que escribió: "}
                    <strong>{sentConfirmation.remitente}</strong> ({roleLabel}).
                  </p>
                </div>

                <div className="flex items-center justify-end gap-2">
                  <button
                    type="button"
                    onClick={() => setSentConfirmation(null)}
                    className="px-4 py-2 rounded-xl text-xs font-semibold"
                    style={{
                      color: C.navy,
                      background: "rgba(36, 60, 143, 0.08)",
                    }}
                  >
                    {isEn ? "Send another message" : "Enviar otro mensaje"}
                  </button>
                  <button
                    type="button"
                    onClick={() => setOpen(false)}
                    className="px-4 py-2 rounded-xl text-xs font-semibold text-white"
                    style={{ background: C.navy }}
                  >
                    {isEn ? "Close" : "Cerrar"}
                  </button>
                </div>
              </div>
            ) : (
              <form onSubmit={handleSubmit} className="space-y-3.5">
                <div>
                  <label
                    className="block text-xs font-semibold mb-1"
                    style={{ color: C.navy }}
                  >
                    {isEn ? "Your email address (sender)" : "Tu correo electrónico (quien escribe)"}
                  </label>
                  <input
                    type="email"
                    required
                    value={senderEmail}
                    onChange={(e) => setSenderEmail(e.target.value)}
                    placeholder={isEn ? "youremail@example.com" : "tucorreo@ejemplo.com"}
                    className="w-full px-3.5 py-2.5 rounded-xl text-xs outline-none"
                    style={{
                      border: "1.5px solid rgba(36, 60, 143, 0.18)",
                      background: "#fafafa",
                      color: "#1e293b",
                    }}
                  />
                </div>

                <div>
                  <label
                    className="block text-xs font-semibold mb-1"
                    style={{ color: C.navy }}
                  >
                    {isEn ? "Your name (optional)" : "Tu nombre (opcional)"}
                  </label>
                  <input
                    type="text"
                    value={senderName}
                    onChange={(e) => setSenderName(e.target.value)}
                    placeholder={isEn ? "Full name" : "Nombre completo"}
                    className="w-full px-3.5 py-2.5 rounded-xl text-xs outline-none"
                    style={{
                      border: "1.5px solid rgba(36, 60, 143, 0.18)",
                      background: "#fafafa",
                      color: "#1e293b",
                    }}
                  />
                </div>

                <div>
                  <label
                    className="block text-xs font-semibold mb-1"
                    style={{ color: C.navy }}
                  >
                    {isEn ? "Support or help subject" : "Asunto de soporte o ayuda"}
                  </label>
                  <input
                    type="text"
                    required
                    value={subject}
                    onChange={(e) => setSubject(e.target.value)}
                    placeholder={isEn ? "E.g. Access issue or inquiry" : "Ej. Problema con mi acceso o consulta"}
                    className="w-full px-3.5 py-2.5 rounded-xl text-xs outline-none"
                    style={{
                      border: "1.5px solid rgba(36, 60, 143, 0.18)",
                      background: "#fafafa",
                      color: "#1e293b",
                    }}
                  />
                </div>

                <div>
                  <label
                    className="block text-xs font-semibold mb-1"
                    style={{ color: C.navy }}
                  >
                    {isEn ? "Message for ecos.colombia1@gmail.com" : "Mensaje para ecos.colombia1@gmail.com"}
                  </label>
                  <textarea
                    required
                    rows={4}
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                    placeholder={isEn ? "Describe how we can help you..." : "Describe en qué podemos ayudarte..."}
                    className="w-full px-3.5 py-2.5 rounded-xl text-xs outline-none resize-y"
                    style={{
                      border: "1.5px solid rgba(36, 60, 143, 0.18)",
                      background: "#fafafa",
                      color: "#1e293b",
                    }}
                  />
                </div>

                <button
                  type="submit"
                  disabled={loading}
                  className="w-full flex items-center justify-center gap-2 py-3 rounded-xl text-white text-xs font-semibold transition-all active:scale-95"
                  style={{
                    background: loading ? "rgba(238, 66, 102, 0.65)" : C.red,
                    cursor: loading ? "not-allowed" : "pointer",
                  }}
                >
                  <Send size={14} />
                  {loading
                    ? isEn
                      ? "Sending email to ecos.colombia1@gmail.com..."
                      : "Enviando correo a ecos.colombia1@gmail.com..."
                    : isEn
                      ? "Send email to ecos.colombia1@gmail.com"
                      : "Enviar correo a ecos.colombia1@gmail.com"}
                </button>
              </form>
            )}
          </div>
        </div>
      )}
    </>
  );
}
