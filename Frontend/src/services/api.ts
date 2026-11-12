const API_URL = import.meta.env.DEV
    ? ""
    : (import.meta.env.VITE_API_URL || "https://localhost:7119").replace(/\/$/, "");

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
};

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

export async function login(usuario: string, password: string, rol: UserRole, captchaToken: string): Promise<LoginApiResponse> {
    const response = await fetch(`${API_URL}/api/Auth/Login`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Usuario: usuario.trim(),
            contraseña: password,
            Rol: rol,
            CaptchaToken: captchaToken
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

export async function googleSendVerificationCode(data: { correo: string; nombre?: string; rol: UserRole; captchaToken: string }): Promise<LoginApiResponse> {
    const response = await fetch(`${API_URL}/api/Auth/GoogleEnviarCodigo`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Correo: data.correo.trim().toLowerCase(),
            Nombre: data.nombre?.trim(),
            Rol: data.rol,
            CaptchaToken: data.captchaToken
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `Error al iniciar sesión con Google: ${response.status}`);
    }

    return response.json();
}

export async function verifyLoginCode(correo: string, rol: UserRole, codigoVerificacion: string) {
    const response = await fetch(`${API_URL}/api/Auth/VerificarCodigo`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Correo: correo.trim(),
            Rol: rol,
            CodigoVerificacion: codigoVerificacion.trim()
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `Error al verificar el código: ${response.status}`);
    }

    const data: LoginApiResponse = await response.json();
    return {
        token: data.token || "",
        nombre: data.nombre || correo.split("@")[0],
        correo: data.correo || correo,
        rol: data.rol || rol,
        expiresAt: data.expiraEn ? Date.parse(data.expiraEn) : Date.now() + 25 * 60 * 1000
    };
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
    const response = await fetch(`${API_URL}/api/Auth/CambiarPasswordConCodigo`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Correo: data.correo.trim(),
            Rol: data.rol,
            CodigoVerificacion: data.codigoVerificacion.trim(),
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
            IdiomaPreferido: "ES",
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
    const response = await fetch(`${API_URL}/api/Auth/ConfirmarRegistro`, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            Correo: correo.trim().toLowerCase(),
            CodigoVerificacion: codigoVerificacion.trim()
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `No fue posible confirmar el registro: ${response.status}`);
    }

    return response.json() as Promise<{ correo: string; mensaje: string }>;
}
