const API_URL = import.meta.env.DEV
    ? ""
    : (import.meta.env.VITE_API_URL && !import.meta.env.VITE_API_URL.includes("-7119.app.github.dev")
        ? import.meta.env.VITE_API_URL.replace(/\/$/, "")
        : "");

export type LoginApiResponse = {
    mensaje?: string;
    token?: string;
    nombre?: string;
    correo?: string;
    rol?: UserRole;
    expiraEn?: string;
    requiereVerificacion?: boolean;
    enviadoPorSmtp?: boolean;
    expiraCodigoEn?: string;
    mantenerSesionUnaHora?: boolean;
    dispositivoConfiableToken?: string | null;
    sesionConfiableHasta?: string | null;
};

export type TrustedDeviceRecord = {
    token: string;
    correo: string;
    usuario?: string;
    rol: UserRole;
    trustedUntil: number;
};

const TRUSTED_DEVICE_STORAGE_KEY = "ecos_trusted_device_sessions_v1";

function readTrustedDevicesStore(): Record<string, TrustedDeviceRecord> {
    if (typeof window === "undefined") return {};
    try {
        const raw = window.localStorage.getItem(TRUSTED_DEVICE_STORAGE_KEY);
        if (!raw) return {};
        const parsed = JSON.parse(raw) as Record<string, TrustedDeviceRecord>;
        return parsed && typeof parsed === "object" ? parsed : {};
    } catch {
        return {};
    }
}

function writeTrustedDevicesStore(store: Record<string, TrustedDeviceRecord>) {
    if (typeof window === "undefined") return;
    try {
        window.localStorage.setItem(TRUSTED_DEVICE_STORAGE_KEY, JSON.stringify(store));
    } catch {
        // Ignorar errores de almacenamiento local
    }
}

export function getActiveTrustedDeviceRecord(identifier: string, rol: UserRole): TrustedDeviceRecord | null {
    const clean = identifier.trim().toLowerCase();
    const store = readTrustedDevicesStore();
    const now = Date.now();
    let changed = false;

    for (const [key, entry] of Object.entries(store)) {
        if (!entry || !entry.trustedUntil || entry.trustedUntil <= now) {
            delete store[key];
            changed = true;
        }
    }
    if (changed) {
        writeTrustedDevicesStore(store);
    }

    if (clean) {
        const directKey = `${rol}:${clean}`;
        if (store[directKey] && store[directKey].trustedUntil > now) {
            return store[directKey];
        }
        for (const entry of Object.values(store)) {
            if (
                entry.rol === rol &&
                entry.trustedUntil > now &&
                (entry.correo.toLowerCase() === clean ||
                    (entry.usuario && entry.usuario.toLowerCase() === clean) ||
                    entry.correo.split("@")[0].toLowerCase() === clean)
            ) {
                return entry;
            }
        }
    } else {
        for (const entry of Object.values(store)) {
            if (entry.rol === rol && entry.trustedUntil > now) {
                return entry;
            }
        }
    }

    return null;
}

export function saveTrustedDeviceRecord(
    correo: string,
    rol: UserRole,
    token: string,
    trustedUntilIsoOrMs: string | number,
    usuario?: string
) {
    const cleanEmail = correo.trim().toLowerCase();
    if (!cleanEmail || !token) return;
    const trustedUntil =
        typeof trustedUntilIsoOrMs === "number"
            ? trustedUntilIsoOrMs
            : Date.parse(trustedUntilIsoOrMs);
    const resolvedUntil = Number.isFinite(trustedUntil) ? trustedUntil : Date.now() + 60 * 60 * 1000;
    const store = readTrustedDevicesStore();
    store[`${rol}:${cleanEmail}`] = {
        token,
        correo: cleanEmail,
        usuario: usuario?.trim() || cleanEmail,
        rol,
        trustedUntil: resolvedUntil,
    };
    writeTrustedDevicesStore(store);
}

export function clearTrustedDeviceRecord(identifier: string, rol?: UserRole) {
    const clean = identifier.trim().toLowerCase();
    const store = readTrustedDevicesStore();
    for (const [key, entry] of Object.entries(store)) {
        const matchesRole = !rol || entry.rol === rol;
        const matchesId =
            !clean ||
            entry.correo.toLowerCase() === clean ||
            (entry.usuario && entry.usuario.toLowerCase() === clean) ||
            key.toLowerCase().endsWith(`:${clean}`);
        if (matchesRole && matchesId) {
            delete store[key];
        }
    }
    writeTrustedDevicesStore(store);
}

export type ValidateCredentialsResponse = {
    credencialesValidas: boolean;
    mostrarRecaptcha: boolean;
    nombre: string;
    correo: string;
    rol: UserRole;
    mensaje?: string;
};

type SessionResponse = {
    nombre: string;
    correo: string;
    rol: UserRole;
    expiraEn: string;
};

export type UserRole = "Administrador" | "Usuario";

const REAL_EMAIL_REGEX = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;

export function isValidRealEmail(email: string): boolean {
    return REAL_EMAIL_REGEX.test(email.trim());
}

async function authorizedRequest(path: string, token: string, options: RequestInit = {}) {
    const response = await fetch(`${API_URL}${path}`, {
        ...options,
        credentials: "include",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
            ...options.headers
        }
    });
    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `Error del servidor: ${response.status}`);
    }
    return response.status === 204 ? null : response.json();
}

export type AdminUser = {
    idUsuario: number;
    nombre: string;
    correo: string;
    tipoUsuario: string;
    activo: boolean;
    motivoDeshabilitacion?: string | null;
};

export type AdminReview = {
    idRecena: number;
    idUsuario: number;
    idLugar: number;
    comentario: string;
    calificacion: number;
    fecha?: string;
    visible: boolean;
};

export type AdminAccount = {
    idAdmin: number;
    nombre: string;
    correo: string;
};

export async function getAdministrators(token: string): Promise<AdminAccount[]> {
    return authorizedRequest("/api/ADMINISTRADORES", token);
}

export async function createAdministrator(token: string, data: { nombre: string; correo: string; password: string }): Promise<AdminAccount> {
    const response = await fetch(`${API_URL}/api/ADMINISTRADORES/crear`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {})
        },
        body: JSON.stringify({ Nombre: data.nombre, Correo: data.correo, Password: data.password })
    });
    const result = await response.json().catch(() => null);
    if (!response.ok) {
        throw new Error(result?.mensaje || `Error del servidor: ${response.status}`);
    }
    return result;
}

export async function getAdminUsers(token: string): Promise<AdminUser[]> {
    return authorizedRequest("/api/User/obtenerUsuario", token);
}

export async function getAdminReviews(token: string): Promise<AdminReview[]> {
    return authorizedRequest("/api/Recena", token);
}

export async function setUserStatus(token: string, id: number, activo: boolean, motivo?: string) {
    return authorizedRequest(`/api/User/${id}/estado`, token, {
        method: "PATCH",
        body: JSON.stringify({ Activo: activo, Motivo: motivo })
    });
}

export async function censorReview(token: string, id: number, palabras: string[]) {
    return authorizedRequest(`/api/Recena/${id}/censurar`, token, {
        method: "POST",
        body: JSON.stringify({ Palabras: palabras })
    });
}

export async function deleteReview(token: string, review: AdminReview) {
    return authorizedRequest("/api/Recena", token, {
        method: "DELETE",
        body: JSON.stringify({
            idRecena: review.idRecena,
            idUsuario: review.idUsuario,
            idLugar: review.idLugar,
            Comentario: review.comentario,
            Calificacion: review.calificacion,
            Fecha: review.fecha
        })
    });
}

export async function validateCredentials(usuario: string, password: string, rol: UserRole): Promise<ValidateCredentialsResponse> {
    const response = await fetch(`${API_URL}/api/Auth/ValidarCredenciales`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Usuario: usuario.trim(),
            contraseña: password,
            Rol: rol
        })
    });

    if (response.status === 401) {
        const err = await response.json().catch(() => null);
        throw new Error(err?.mensaje || "Usuario/correo o contraseña incorrectos");
    }

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `Error del servidor: ${response.status}`);
    }

    return response.json();
}

export async function login(
    usuario: string,
    password: string,
    rol: UserRole,
    captchaToken = "",
    mantenerSesionUnaHora = false
): Promise<LoginApiResponse> {
    const trimmedUser = usuario.trim();
    const activeTrusted = getActiveTrustedDeviceRecord(trimmedUser, rol);
    const response = await fetch(`${API_URL}/api/Auth/Login`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Usuario: trimmedUser,
            contraseña: password,
            Rol: rol,
            CaptchaToken: captchaToken,
            MantenerSesionUnaHora: mantenerSesionUnaHora,
            DispositivoConfiableToken: activeTrusted?.token || undefined
        })
    });

    if (response.status === 401) {
        const err = await response.json().catch(() => null);
        throw new Error(err?.mensaje || "Usuario/correo o contraseña incorrectos");
    }

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `Error del servidor: ${response.status}`);
    }

    const data: LoginApiResponse = await response.json();
    if (data.dispositivoConfiableToken && (data.sesionConfiableHasta || data.expiraEn)) {
        saveTrustedDeviceRecord(
            data.correo || trimmedUser,
            data.rol || rol,
            data.dispositivoConfiableToken,
            data.sesionConfiableHasta || data.expiraEn!,
            trimmedUser
        );
    }
    return data;
}

export async function googleSendVerificationCode(data: {
    correo: string;
    nombre?: string;
    rol: UserRole;
    captchaToken: string;
    mantenerSesionUnaHora?: boolean;
}): Promise<LoginApiResponse> {
    const cleanEmail = data.correo.trim().toLowerCase();
    const activeTrusted = getActiveTrustedDeviceRecord(cleanEmail, data.rol);
    const response = await fetch(`${API_URL}/api/Auth/GoogleEnviarCodigo`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Correo: cleanEmail,
            Nombre: data.nombre?.trim(),
            Rol: data.rol,
            CaptchaToken: data.captchaToken,
            MantenerSesionUnaHora: Boolean(data.mantenerSesionUnaHora),
            DispositivoConfiableToken: activeTrusted?.token || undefined
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `Error al iniciar sesión con Google: ${response.status}`);
    }

    const resData: LoginApiResponse = await response.json();
    if (resData.dispositivoConfiableToken && (resData.sesionConfiableHasta || resData.expiraEn)) {
        saveTrustedDeviceRecord(
            resData.correo || cleanEmail,
            resData.rol || data.rol,
            resData.dispositivoConfiableToken,
            resData.sesionConfiableHasta || resData.expiraEn!,
            cleanEmail
        );
    }
    return resData;
}

export async function resendVerificationCode(correo: string, rol: UserRole): Promise<LoginApiResponse> {
    const response = await fetch(`${API_URL}/api/Auth/ReenviarCodigo`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Correo: correo.trim(),
            Rol: rol
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `No se pudo reenviar el código: ${response.status}`);
    }

    return response.json();
}

export async function validateSixDigitCode(correo: string, rol: UserRole, codigoVerificacion: string) {
    const cleanCode = codigoVerificacion.replace(/\D/g, "");
    const response = await fetch(`${API_URL}/api/Auth/ValidarCodigo6Digitos`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Correo: correo.trim(),
            Rol: rol,
            CodigoVerificacion: cleanCode
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `El código de 6 dígitos no es válido: ${response.status}`);
    }

    return response.json() as Promise<{
        codigoValido: boolean;
        mostrarRecaptcha: boolean;
        correo: string;
        nombre: string;
        rol: UserRole;
        mensaje: string;
    }>;
}

export async function verifyLoginCode(
    correo: string,
    rol: UserRole,
    codigoVerificacion: string,
    captchaToken = "",
    mantenerSesionUnaHora = false,
    usuarioAlias?: string
) {
    const cleanCode = codigoVerificacion.replace(/\D/g, "");
    const response = await fetch(`${API_URL}/api/Auth/VerificarCodigo`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Correo: correo.trim(),
            Rol: rol,
            CodigoVerificacion: cleanCode,
            CaptchaToken: captchaToken,
            MantenerSesionUnaHora: mantenerSesionUnaHora
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `Error al verificar el código: ${response.status}`);
    }

    const data: LoginApiResponse = await response.json();
    const resolvedEmail = data.correo || correo;
    const resolvedRole = data.rol || rol;

    if (data.dispositivoConfiableToken && (data.sesionConfiableHasta || data.expiraEn)) {
        saveTrustedDeviceRecord(
            resolvedEmail,
            resolvedRole,
            data.dispositivoConfiableToken,
            data.sesionConfiableHasta || data.expiraEn!,
            usuarioAlias || correo
        );
    } else if (!mantenerSesionUnaHora) {
        clearTrustedDeviceRecord(resolvedEmail, resolvedRole);
    }

    return {
        token: data.token || "",
        nombre: data.nombre || correo.split("@")[0],
        correo: resolvedEmail,
        rol: resolvedRole,
        expiresAt: data.expiraEn
            ? Date.parse(data.expiraEn)
            : Date.now() + (mantenerSesionUnaHora ? 60 : 25) * 60 * 1000,
        mantenerSesionUnaHora: Boolean(data.mantenerSesionUnaHora || mantenerSesionUnaHora)
    };
}

export async function configureOneHourDeviceSession(data: {
    correo: string;
    nombre?: string;
    rol: UserRole;
    activar: boolean;
}) {
    const response = await fetch(`${API_URL}/api/Auth/SesionUnaHora`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Correo: data.correo.trim(),
            Nombre: data.nombre?.trim(),
            Rol: data.rol,
            Activar: data.activar
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `No se pudo configurar la sesión de 1 hora: ${response.status}`);
    }

    const resData: LoginApiResponse = await response.json();
    if (data.activar && resData.dispositivoConfiableToken && (resData.sesionConfiableHasta || resData.expiraEn)) {
        saveTrustedDeviceRecord(
            resData.correo || data.correo,
            resData.rol || data.rol,
            resData.dispositivoConfiableToken,
            resData.sesionConfiableHasta || resData.expiraEn!,
            data.correo
        );
    } else if (!data.activar) {
        clearTrustedDeviceRecord(data.correo, data.rol);
    }

    return resData;
}

export async function requestPasswordRecovery(correo: string, rol: UserRole, captchaToken: string) {
    const response = await fetch(`${API_URL}/api/Auth/SolicitarRecuperacion`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Correo: correo.trim(),
            Rol: rol,
            CaptchaToken: captchaToken
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `No fue posible solicitar la recuperación: ${response.status}`);
    }

    return response.json() as Promise<{ correo: string; rol: UserRole; expiraEn: string; mensaje: string }>;
}

export async function resetPasswordWithCode(data: { correo: string; rol: UserRole; codigoVerificacion: string; nuevaPassword: string }) {
    const cleanCode = data.codigoVerificacion.replace(/\D/g, "");
    const response = await fetch(`${API_URL}/api/Auth/CambiarPasswordConCodigo`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Correo: data.correo.trim(),
            Rol: data.rol,
            CodigoVerificacion: cleanCode,
            NuevaPassword: data.nuevaPassword
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `No fue posible cambiar la contraseña: ${response.status}`);
    }

    return response.json() as Promise<{ mensaje: string }>;
}

export async function getSession() {
    const response = await fetch(`${API_URL}/api/Auth/Session`, {
        credentials: "include"
    });

    if (!response.ok) {
        throw new Error("No hay una sesión activa.");
    }

    const data: SessionResponse = await response.json();
    const expiresAt = Date.parse(data.expiraEn);
    if (!Number.isFinite(expiresAt) || expiresAt <= Date.now()) {
        throw new Error("La sesión expiró.");
    }

    return {
        name: data.nombre,
        email: data.correo,
        role: data.rol,
        token: "",
        expiresAt
    };
}

export async function logout() {
    await fetch(`${API_URL}/api/Auth/Logout`, {
        method: "POST",
        credentials: "include"
    });
}

export async function registerUser(data: { nombre: string; correo: string; password: string; captchaToken: string; requiereVerificacionCorreo?: boolean }) {
    const response = await fetch(`${API_URL}/api/Auth/Register`, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            Nombre: data.nombre.trim(),
            Correo: data.correo.trim().toLowerCase(),
            Password: data.password,
            CaptchaToken: data.captchaToken,
            TipoUsuario: "Usuario",
            RequiereVerificacionCorreo: data.requiereVerificacionCorreo ?? true
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `Error del servidor: ${response.status}`);
    }

    return response.json().catch(() => ({}));
}

export async function confirmRegistrationCode(correo: string, codigoVerificacion: string) {
    const cleanCode = codigoVerificacion.replace(/\D/g, "");
    const response = await fetch(`${API_URL}/api/Auth/ConfirmarRegistro`, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            Correo: correo.trim().toLowerCase(),
            CodigoVerificacion: cleanCode
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `No fue posible confirmar el registro: ${response.status}`);
    }

    return response.json() as Promise<{ correo: string; mensaje: string }>;
}

export async function sendSupportEmail(data: {
    correoRemitente: string;
    nombreRemitente?: string;
    rol?: string;
    asunto: string;
    mensaje: string;
}) {
    const response = await fetch(`${API_URL}/api/Auth/Soporte`, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            CorreoRemitente: data.correoRemitente.trim(),
            NombreRemitente: data.nombreRemitente?.trim(),
            Rol: data.rol,
            Asunto: data.asunto.trim(),
            Mensaje: data.mensaje.trim()
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `No fue posible enviar el mensaje de soporte: ${response.status}`);
    }

    return response.json() as Promise<{
        enviado: boolean;
        destinatario: string;
        remitente: string;
        asunto: string;
        mensaje: string;
    }>;
}

