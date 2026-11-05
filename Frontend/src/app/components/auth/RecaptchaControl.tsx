import ReCAPTCHA from "react-google-recaptcha";

const GOOGLE_TEST_SITE_KEY = "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI";
const isDev = import.meta.env.DEV;
const siteKey = import.meta.env.VITE_RECAPTCHA_SITE_KEY || (isDev ? GOOGLE_TEST_SITE_KEY : "");

type RecaptchaControlProps = {
  onChange: (token: string | null) => void;
  recaptchaRef: React.RefObject<ReCAPTCHA | null>;
};

export default function RecaptchaControl({ onChange, recaptchaRef }: RecaptchaControlProps) {
  if (!siteKey) {
    return (
      <p role="alert" className="text-sm text-red-600">
        reCAPTCHA no está configurado. Contacta al administrador.
      </p>
    );
  }

  return (
    <div className="flex justify-center overflow-x-auto">
      <ReCAPTCHA
        ref={recaptchaRef}
        sitekey={siteKey}
        onChange={onChange}
        onExpired={() => onChange(null)}
        onErrored={() => onChange(null)}
      />
    </div>
  );
}