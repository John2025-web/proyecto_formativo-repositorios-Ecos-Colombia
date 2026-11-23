import { useCallback, useEffect, useImperativeHandle, useRef, useState } from "react";
import { CheckCircle2, RefreshCw, ShieldCheck } from "lucide-react";
import { C } from "@/app/theme";

const ALPHABET = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";

function generateRandomCode(length = 6): string {
  const array = new Uint32Array(length);
  if (typeof window !== "undefined" && window.crypto?.getRandomValues) {
    window.crypto.getRandomValues(array);
  } else {
    for (let i = 0; i < length; i++) {
      array[i] = Math.floor(Math.random() * 100000);
    }
  }
  return Array.from(array, (val) => ALPHABET[val % ALPHABET.length]).join("");
}

export type RecaptchaControlHandle = {
  reset: () => void;
};

type RecaptchaControlProps = {
  onChange: (token: string | null) => void;
  recaptchaRef?: React.RefObject<RecaptchaControlHandle | null>;
};

export default function RecaptchaControl({ onChange, recaptchaRef }: RecaptchaControlProps) {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [challengeCode, setChallengeCode] = useState<string>(() => generateRandomCode(6));
  const [userInput, setUserInput] = useState("");
  const [confirmed, setConfirmed] = useState(false);
  const [attemptError, setAttemptError] = useState("");

  const drawCaptcha = useCallback((code: string) => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext("2d");
    if (!ctx) return;

    const width = canvas.width;
    const height = canvas.height;

    ctx.clearRect(0, 0, width, height);

    // Fondo degradado elegante
    const gradient = ctx.createLinearGradient(0, 0, width, height);
    gradient.addColorStop(0, "#f4f6fc");
    gradient.addColorStop(0.5, "#eef2ff");
    gradient.addColorStop(1, "#fef9e7");
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, width, height);

    // Cuadrícula de seguridad sutil
    ctx.strokeStyle = "rgba(36, 60, 143, 0.08)";
    ctx.lineWidth = 1;
    for (let x = 0; x < width; x += 14) {
      ctx.beginPath();
      ctx.moveTo(x, 0);
      ctx.lineTo(x, height);
      ctx.stroke();
    }
    for (let y = 0; y < height; y += 14) {
      ctx.beginPath();
      ctx.moveTo(0, y);
      ctx.lineTo(width, y);
      ctx.stroke();
    }

    // Líneas de interferencia curvas
    const lineColors = ["rgba(36,60,143,0.28)", "rgba(238,66,102,0.25)", "rgba(244,211,94,0.45)"];
    for (let i = 0; i < 4; i++) {
      ctx.strokeStyle = lineColors[i % lineColors.length];
      ctx.lineWidth = 1.6;
      ctx.beginPath();
      ctx.moveTo(8, 10 + ((i * 13) % (height - 16)));
      ctx.bezierCurveTo(
        width * 0.33,
        (i % 2 === 0 ? 6 : height - 6),
        width * 0.66,
        (i % 2 === 0 ? height - 8 : 8),
        width - 8,
        12 + ((i * 17) % (height - 20))
      );
      ctx.stroke();
    }

    // Puntos de ruido
    for (let i = 0; i < 28; i++) {
      ctx.fillStyle = i % 2 === 0 ? "rgba(36,60,143,0.22)" : "rgba(238,66,102,0.20)";
      const px = ((i * 37 + code.charCodeAt(i % code.length) * 13) % (width - 8)) + 4;
      const py = ((i * 23 + code.charCodeAt(i % code.length) * 7) % (height - 8)) + 4;
      ctx.beginPath();
      ctx.arc(px, py, 1.3, 0, Math.PI * 2);
      ctx.fill();
    }

    // Dibujar caracteres alfanuméricos
    const charColors = ["#243c8f", "#1e293b", "#be123c", "#1d4ed8", "#0f172a", "#243c8f"];
    const slotWidth = (width - 28) / code.length;

    for (let i = 0; i < code.length; i++) {
      const ch = code[i];
      ctx.save();
      const x = 18 + i * slotWidth + slotWidth * 0.35;
      const y = height / 2 + 7;
      const angle = ((i % 2 === 0 ? 1 : -1) * ((code.charCodeAt(i) % 14) - 4) * Math.PI) / 180;

      ctx.translate(x, y);
      ctx.rotate(angle);
      ctx.font = "700 24px 'Courier New', Consolas, monospace";
      ctx.fillStyle = charColors[i % charColors.length];
      ctx.shadowColor = "rgba(36, 60, 143, 0.18)";
      ctx.shadowBlur = 2;
      ctx.fillText(ch, -7, 0);
      ctx.restore();
    }
  }, []);

  useEffect(() => {
    drawCaptcha(challengeCode);
  }, [challengeCode, drawCaptcha]);

  const handleRefresh = useCallback(() => {
    const next = generateRandomCode(6);
    setChallengeCode(next);
    setUserInput("");
    setConfirmed(false);
    setAttemptError("");
    onChange(null);
  }, [onChange]);

  useImperativeHandle(
    recaptchaRef,
    () => ({
      reset: handleRefresh,
    }),
    [handleRefresh]
  );

  const handleInputChange = (value: string) => {
    const cleaned = value.replace(/\s+/g, "").slice(0, 6);
    setUserInput(cleaned);
    setAttemptError("");

    if (cleaned.toLowerCase() === challengeCode.toLowerCase()) {
      setConfirmed(true);
      onChange(`ALPHANUMERIC_CONFIRMED:${challengeCode}`);
    } else if (confirmed) {
      setConfirmed(false);
      onChange(null);
    }
  };

  const handleManualConfirm = () => {
    if (userInput.trim().toLowerCase() === challengeCode.toLowerCase()) {
      setConfirmed(true);
      setAttemptError("");
      onChange(`ALPHANUMERIC_CONFIRMED:${challengeCode}`);
    } else {
      setConfirmed(false);
      onChange(null);
      setAttemptError("El código no coincide. Revisa las letras y números e inténtalo de nuevo.");
    }
  };

  return (
    <div
      className="rounded-xl p-3.5 transition-all"
      style={{
        background: confirmed ? "rgba(16, 185, 129, 0.05)" : "#f8fafc",
        border: `1.5px solid ${
          confirmed
            ? "rgba(16, 185, 129, 0.45)"
            : attemptError
            ? "rgba(238, 66, 102, 0.45)"
            : "rgba(36, 60, 143, 0.16)"
        }`,
      }}
    >
      <div className="flex items-center justify-between mb-2.5">
        <div className="flex items-center gap-2">
          <ShieldCheck
            size={16}
            style={{ color: confirmed ? "#059669" : C.navy, flexShrink: 0 }}
          />
          <span className="text-xs font-semibold" style={{ color: C.navy }}>
            Verificación de seguridad reCAPTCHA
          </span>
        </div>
        {confirmed && (
          <span
            className="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-[11px] font-semibold"
            style={{
              background: "rgba(16, 185, 129, 0.14)",
              color: "#047857",
            }}
          >
            <CheckCircle2 size={13} /> Confirmado
          </span>
        )}
      </div>

      <div className="flex items-center gap-2.5 mb-2.5">
        <div
          className="relative flex-1 overflow-hidden rounded-lg select-none flex items-center justify-center"
          style={{
            border: "1px solid rgba(36, 60, 143, 0.18)",
            background: "#eef2ff",
            height: 54,
          }}
        >
          <canvas
            ref={canvasRef}
            width={220}
            height={54}
            aria-label="Código reCAPTCHA alfanumérico"
            className="w-full h-full block"
          />
        </div>
        <button
          type="button"
          onClick={handleRefresh}
          title="Generar nuevo código al azar"
          aria-label="Generar nuevo código al azar"
          className="p-2.5 rounded-lg transition-all hover:bg-white active:scale-95 flex items-center justify-center"
          style={{
            border: "1px solid rgba(36, 60, 143, 0.18)",
            color: C.navy,
            background: "#ffffff",
            height: 54,
            width: 46,
          }}
        >
          <RefreshCw size={17} />
        </button>
      </div>

      <div className="flex items-center gap-2">
        <input
          type="text"
          value={userInput}
          onChange={(e) => handleInputChange(e.target.value)}
          placeholder="Escribe los 6 caracteres..."
          maxLength={6}
          autoComplete="off"
          spellCheck={false}
          className="flex-1 px-3.5 py-2.5 rounded-lg text-sm outline-none tracking-widest font-mono"
          style={{
            background: "#ffffff",
            border: `1.5px solid ${
              confirmed ? "#10b981" : "rgba(36, 60, 143, 0.2)"
            }`,
            color: "#1e293b",
          }}
        />
        <button
          type="button"
          onClick={handleManualConfirm}
          disabled={confirmed || userInput.trim().length < 6}
          className="px-3.5 py-2.5 rounded-lg text-xs font-semibold transition-all active:scale-95"
          style={{
            background: confirmed ? "#10b981" : C.navy,
            color: "#ffffff",
            opacity: confirmed || userInput.trim().length < 6 ? 0.8 : 1,
            cursor: confirmed || userInput.trim().length < 6 ? "default" : "pointer",
          }}
        >
          {confirmed ? "Verificado" : "Confirmar"}
        </button>
      </div>

      {attemptError && (
        <p className="text-xs mt-2 font-medium" style={{ color: C.red }}>
          {attemptError}
        </p>
      )}
    </div>
  );
}
