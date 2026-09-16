const API_URL = import.meta.env.VITE_API_URL;

export async function login(usuario : string, contraseña : string ) {
    const response = await fetch(`${API_URL}/Auth/Login`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            Usuario: usuario,
            contraseña: contraseña
        })
    });

    if (!response.ok) {
        if (response.status === 401) {
            throw new Error("Usuario o contraseña incorrectos");
        }

        throw new Error("Error al iniciar sesión");
    }

    return await response.json();
}


export async function getGastronomia() {
    const response = await fetch(`${API_URL}/Gastronomia`);

    if (!response.ok) {
        throw new Error("Error al obtener gastronomía");
    }

    return await response.json();
}
