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

const SUPPORT_TR: Record<string, Record<string, string>> = {
  ES: {
    invalidEmail: "Ingresa un correo electrónico válido para que podamos ver quién escribe y responderte.",
    missingFields: "Por favor completa el asunto y el mensaje.",
    sendError: "No fue posible enviar el correo de soporte.",
    badge: "Acceso rápido · Soporte o ayuda",
    title: "Contactar a Ecos Colombia",
    closeAria: "Cerrar soporte",
    sendTo: "Enviar directamente a:",
    senderEmailLabel: "Correo de quien escribe:",
    enterBelow: "Ingresa tu correo abajo",
    sentTitle: "Correo enviado correctamente",
    sentToPrefix: "Se envió tu solicitud a ",
    sentFromPrefix: "En el correo recibido por Ecos Colombia aparece registrado que escribió: ",
    sendAnother: "Enviar otro mensaje",
    close: "Cerrar",
    fieldEmail: "Tu correo electrónico (quien escribe)",
    placeholderEmail: "tucorreo@ejemplo.com",
    fieldName: "Tu nombre (opcional)",
    placeholderName: "Nombre completo",
    fieldSubject: "Asunto de soporte o ayuda",
    placeholderSubject: "Ej. Problema con mi acceso o consulta",
    fieldMessage: "Mensaje para ecos.colombia1@gmail.com",
    placeholderMessage: "Describe en qué podemos ayudarte...",
    sendingBtn: "Enviando correo a ecos.colombia1@gmail.com...",
    sendBtn: "Enviar correo a ecos.colombia1@gmail.com",
  },
  EN: {
    invalidEmail: "Please enter a valid email address so we can see who is writing and reply to you.",
    missingFields: "Please complete both the subject and the message.",
    sendError: "Could not send the support email.",
    badge: "Quick Access · Support & Help",
    title: "Contact Ecos Colombia",
    closeAria: "Close support",
    sendTo: "Send directly to:",
    senderEmailLabel: "Sender's email:",
    enterBelow: "Enter your email below",
    sentTitle: "Email sent successfully",
    sentToPrefix: "Your request was sent to ",
    sentFromPrefix: "In the email received by Ecos Colombia it is registered that it was sent by: ",
    sendAnother: "Send another message",
    close: "Close",
    fieldEmail: "Your email address (sender)",
    placeholderEmail: "youremail@example.com",
    fieldName: "Your name (optional)",
    placeholderName: "Full name",
    fieldSubject: "Support or help subject",
    placeholderSubject: "E.g. Access issue or inquiry",
    fieldMessage: "Message for ecos.colombia1@gmail.com",
    placeholderMessage: "Describe how we can help you...",
    sendingBtn: "Sending email to ecos.colombia1@gmail.com...",
    sendBtn: "Send email to ecos.colombia1@gmail.com",
  },
  PT: {
    invalidEmail: "Insira um e-mail válido para que possamos identificar quem escreve e responder.",
    missingFields: "Por favor, preencha o assunto e a mensagem.",
    sendError: "Não foi possível enviar o e-mail de suporte.",
    badge: "Acesso rápido · Suporte ou ajuda",
    title: "Contatar Ecos Colombia",
    closeAria: "Fechar suporte",
    sendTo: "Enviar diretamente para:",
    senderEmailLabel: "E-mail do remetente:",
    enterBelow: "Insira seu e-mail abaixo",
    sentTitle: "E-mail enviado com sucesso",
    sentToPrefix: "Sua solicitação foi enviada para ",
    sentFromPrefix: "No e-mail recebido pela Ecos Colombia consta que foi enviado por: ",
    sendAnother: "Enviar outra mensagem",
    close: "Fechar",
    fieldEmail: "Seu endereço de e-mail (remetente)",
    placeholderEmail: "seuemail@exemplo.com",
    fieldName: "Seu nome (opcional)",
    placeholderName: "Nome completo",
    fieldSubject: "Assunto de suporte ou ajuda",
    placeholderSubject: "Ex. Problema de acesso ou dúvida",
    fieldMessage: "Mensagem para ecos.colombia1@gmail.com",
    placeholderMessage: "Descreva como podemos ajudar...",
    sendingBtn: "Enviando e-mail para ecos.colombia1@gmail.com...",
    sendBtn: "Enviar e-mail para ecos.colombia1@gmail.com",
  },
  FR: {
    invalidEmail: "Veuillez saisir une adresse e-mail valide afin que nous puissions vous répondre.",
    missingFields: "Veuillez remplir l'objet et le message.",
    sendError: "Impossible d'envoyer l'e-mail d'assistance.",
    badge: "Accès rapide · Support & Aide",
    title: "Contacter Ecos Colombia",
    closeAria: "Fermer le support",
    sendTo: "Envoyer directement à :",
    senderEmailLabel: "E-mail de l'expéditeur :",
    enterBelow: "Saisissez votre e-mail ci-dessous",
    sentTitle: "E-mail envoyé avec succès",
    sentToPrefix: "Votre demande a été envoyée à ",
    sentFromPrefix: "Dans l'e-mail reçu par Ecos Colombia, l'expéditeur enregistré est : ",
    sendAnother: "Envoyer un autre message",
    close: "Fermer",
    fieldEmail: "Votre adresse e-mail (expéditeur)",
    placeholderEmail: "votreemail@exemple.com",
    fieldName: "Votre nom (facultatif)",
    placeholderName: "Nom complet",
    fieldSubject: "Objet de votre demande",
    placeholderSubject: "Ex. Problème d'accès ou question",
    fieldMessage: "Message pour ecos.colombia1@gmail.com",
    placeholderMessage: "Décrivez comment nous pouvons vous aider...",
    sendingBtn: "Envoi de l'e-mail à ecos.colombia1@gmail.com...",
    sendBtn: "Envoyer l'e-mail à ecos.colombia1@gmail.com",
  },
  DE: {
    invalidEmail: "Bitte geben Sie eine gültige E-Mail-Adresse ein, damit wir Ihnen antworten können.",
    missingFields: "Bitte füllen Sie Betreff und Nachricht aus.",
    sendError: "Support-E-Mail konnte nicht gesendet werden.",
    badge: "Schnellzugriff · Support & Hilfe",
    title: "Ecos Colombia kontaktieren",
    closeAria: "Support schließen",
    sendTo: "Direkt senden an:",
    senderEmailLabel: "E-Mail des Absenders:",
    enterBelow: "Geben Sie unten Ihre E-Mail ein",
    sentTitle: "E-Mail erfolgreich gesendet",
    sentToPrefix: "Ihre Anfrage wurde gesendet an ",
    sentFromPrefix: "In der empfangenen E-Mail wurde folgender Absender registriert: ",
    sendAnother: "Weitere Nachricht senden",
    close: "Schließen",
    fieldEmail: "Ihre E-Mail-Adresse (Absender)",
    placeholderEmail: "ihreemail@beispiel.de",
    fieldName: "Ihr Name (optional)",
    placeholderName: "Vollständiger Name",
    fieldSubject: "Betreff für Support oder Hilfe",
    placeholderSubject: "z. B. Zugangsproblem oder Anfrage",
    fieldMessage: "Nachricht an ecos.colombia1@gmail.com",
    placeholderMessage: "Beschreiben Sie, wie wir Ihnen helfen können...",
    sendingBtn: "E-Mail wird an ecos.colombia1@gmail.com gesendet...",
    sendBtn: "E-Mail an ecos.colombia1@gmail.com senden",
  },
  ZH: {
    invalidEmail: "请输入有效的电子邮箱地址，以便我们查看发件人并回复您。",
    missingFields: "请完整填写主题和留言内容。",
    sendError: "无法发送客服支持邮件。",
    badge: "快速通道 · 客服与帮助",
    title: "联系 Ecos Colombia",
    closeAria: "关闭客服窗口",
    sendTo: "直接发送至：",
    senderEmailLabel: "发件人邮箱：",
    enterBelow: "请在下方输入您的邮箱",
    sentTitle: "邮件发送成功",
    sentToPrefix: "您的请求已发送至 ",
    sentFromPrefix: "Ecos Colombia 收到的邮件中已登记发件人为：",
    sendAnother: "发送另一条消息",
    close: "关闭",
    fieldEmail: "您的电子邮箱（发件人）",
    placeholderEmail: "youremail@example.com",
    fieldName: "您的姓名（选填）",
    placeholderName: "全名",
    fieldSubject: "客服或帮助主题",
    placeholderSubject: "例如：登录问题或旅行咨询",
    fieldMessage: "发送给 ecos.colombia1@gmail.com 的留言",
    placeholderMessage: "请描述我们需要为您提供什么帮助...",
    sendingBtn: "正在发送邮件至 ecos.colombia1@gmail.com...",
    sendBtn: "发送邮件至 ecos.colombia1@gmail.com",
  },
  JA: {
    invalidEmail: "返信できるよう、有効なメールアドレスを入力してください。",
    missingFields: "件名とメッセージの両方を入力してください。",
    sendError: "サポートメールを送信できませんでした。",
    badge: "クイックアクセス・サポートとヘルプ",
    title: "Ecos Colombia へのお問い合わせ",
    closeAria: "サポートを閉じる",
    sendTo: "送信先メールアドレス：",
    senderEmailLabel: "送信者のメールアドレス：",
    enterBelow: "下にメールアドレスを入力してください",
    sentTitle: "メールが正常に送信されました",
    sentToPrefix: "お問い合わせは以下の宛先に送信されました：",
    sentFromPrefix: "Ecos Colombia が受信したメールに記録された送信者：",
    sendAnother: "別のメッセージを送る",
    close: "閉じる",
    fieldEmail: "あなたのメールアドレス（送信者）",
    placeholderEmail: "youremail@example.com",
    fieldName: "お名前（任意）",
    placeholderName: "氏名",
    fieldSubject: "お問い合わせ件名",
    placeholderSubject: "例：ログインに関する問題やご質問",
    fieldMessage: "ecos.colombia1@gmail.com へのメッセージ",
    placeholderMessage: "ご用件を詳しくご記入ください...",
    sendingBtn: "ecos.colombia1@gmail.com へ送信中...",
    sendBtn: "ecos.colombia1@gmail.com へメールを送信",
  },
  KO: {
    invalidEmail: "회신받을 수 있도록 유효한 이메일 주소를 입력해 주세요.",
    missingFields: "제목과 메시지 내용을 모두 입력해 주세요.",
    sendError: "고객지원 이메일을 전송하지 못했습니다.",
    badge: "빠른 문의 · 고객지원 및 도움말",
    title: "Ecos Colombia 문의하기",
    closeAria: "고객지원 닫기",
    sendTo: "공식 수신 이메일:",
    senderEmailLabel: "보내는 분 이메일:",
    enterBelow: "아래에 이메일을 입력하세요",
    sentTitle: "이메일이 성공적으로 전송되었습니다",
    sentToPrefix: "문의하신 내용이 다음 주소로 전송되었습니다: ",
    sentFromPrefix: "Ecos Colombia 수신 메일에 기록된 발신자: ",
    sendAnother: "다른 메시지 보내기",
    close: "닫기",
    fieldEmail: "보내는 분 이메일 주소",
    placeholderEmail: "youremail@example.com",
    fieldName: "이름 (선택사항)",
    placeholderName: "성함",
    fieldSubject: "문의 또는 도움말 제목",
    placeholderSubject: "예: 로그인 문제 또는 여행 문의",
    fieldMessage: "ecos.colombia1@gmail.com 으로 보낼 메시지",
    placeholderMessage: "도움이 필요한 내용을 작성해 주세요...",
    sendingBtn: "ecos.colombia1@gmail.com 으로 전송 중...",
    sendBtn: "ecos.colombia1@gmail.com 으로 이메일 보내기",
  },
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
  const s = SUPPORT_TR[lang] ?? SUPPORT_TR.EN;
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
      setError(s.invalidEmail);
      return;
    }
    if (!subject.trim() || !message.trim()) {
      setError(s.missingFields);
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
      setError(err instanceof Error ? err.message : s.sendError);
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
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
                  <Mail size={12} /> {s.badge}
                </span>
                <h3
                  className="text-lg font-bold"
                  style={{ color: C.navy, fontWeight: 700 }}
                >
                  {s.title}
                </h3>
              </div>
              <button
                type="button"
                onClick={() => setOpen(false)}
                aria-label={s.closeAria}
                className="p-1.5 rounded-lg hover:bg-slate-100 transition-colors"
                style={{ color: "#64748b" }}
              >
                <X size={18} />
              </button>
            </div>

            <div
              className="rounded-xl p-3.5 mb-4 text-xs space-y-1.5"
              style={{
                background: "#f8fafc",
                border: "1px solid rgba(36, 60, 143, 0.16)",
              }}
            >
              <div className="flex items-center justify-between gap-2">
                <span style={{ color: "#64748b" }}>{s.sendTo}</span>
                <span className="font-semibold" style={{ color: C.navy }}>
                  ecos.colombia1@gmail.com
                </span>
              </div>
              <div className="flex items-center justify-between gap-2">
                <span style={{ color: "#64748b" }}>{s.senderEmailLabel}</span>
                <span
                  className="font-semibold truncate max-w-[210px]"
                  style={{ color: C.red }}
                >
                  {senderEmail.trim() || s.enterBelow}
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
                    <CheckCircle2 size={18} /> {s.sentTitle}
                  </div>
                  <p>
                    {s.sentToPrefix}
                    <strong>{sentConfirmation.destinatario}</strong>.
                  </p>
                  <p>
                    {s.sentFromPrefix}
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
                    {s.sendAnother}
                  </button>
                  <button
                    type="button"
                    onClick={() => setOpen(false)}
                    className="px-4 py-2 rounded-xl text-xs font-semibold text-white"
                    style={{ background: C.navy }}
                  >
                    {s.close}
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
                    {s.fieldEmail}
                  </label>
                  <input
                    type="email"
                    required
                    value={senderEmail}
                    onChange={(e) => setSenderEmail(e.target.value)}
                    placeholder={s.placeholderEmail}
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
                    {s.fieldName}
                  </label>
                  <input
                    type="text"
                    value={senderName}
                    onChange={(e) => setSenderName(e.target.value)}
                    placeholder={s.placeholderName}
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
                    {s.fieldSubject}
                  </label>
                  <input
                    type="text"
                    required
                    value={subject}
                    onChange={(e) => setSubject(e.target.value)}
                    placeholder={s.placeholderSubject}
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
                    {s.fieldMessage}
                  </label>
                  <textarea
                    required
                    rows={4}
                    value={message}
                    onChange={(e) => setMessage(e.target.value)}
                    placeholder={s.placeholderMessage}
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
                  {loading ? s.sendingBtn : s.sendBtn}
                </button>
              </form>
            )}
          </div>
        </div>
      )}
    </>
  );
}
