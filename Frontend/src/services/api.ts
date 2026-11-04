const API_URL = import.meta.env.DEV
    ? ""
    : (import.meta.env.VITE_API_URL || "https://localhost:7119").replace(/\/$/, "");

type LoginResponse = {
    mensaje?: string;
};

export type UserRole = "Administrador" | "Usuario";

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

export async function login(usuario: string, password: string, rol: UserRole) {
    const response = await fetch(`${API_URL}/api/Auth/Login`, {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Usuario: usuario,
            contraseña: password,
            Rol: rol
        })
    });

    if (response.status === 401) {
        throw new Error("Usuario o contraseña incorrectos");
    }

    if (!response.ok) {
        throw new Error(`Error del servidor: ${response.status}`);
    }

    const data: LoginResponse = await response.json();
    return { token: "" };
}

export async function logout() {
    await fetch(`${API_URL}/api/Auth/Logout`, {
        method: "POST",
        credentials: "include"
    });
}

export async function registerUser(data: { nombre: string; correo: string; password: string }) {
    const response = await fetch(`${API_URL}/api/Auth/Register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            Nombre: data.nombre,
            Correo: data.correo,
            Password: data.password,
            IdiomaPreferido: "ES",
            TipoUsuario: "Usuario"
        })
    });

    if (!response.ok) {
        const error = await response.json().catch(() => null);
        throw new Error(error?.mensaje || `Error del servidor: ${response.status}`);
    }
}

