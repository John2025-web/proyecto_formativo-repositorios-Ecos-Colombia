import { useState } from "react";
import { login } from "../services/api";

function Login() {

    const [usuario, setUsuario] = useState("");
    const [contraseña, setContraseña] = useState("");

    const handleLogin = async (e) => {
        e.preventDefault();

        try {
            const data = await login(usuario, contraseña);

            console.log("Login exitoso:", data);

            // El JWT que devuelve tu backend
            console.log("Token:", data.Token);

        } catch (error) {
            console.error(error);
            alert(error.message);
        }
    };

    return (
        <div>
            <h1>Iniciar sesión</h1>

            <form onSubmit={handleLogin}>

                <input
                    type="text"
                    placeholder="Usuario"
                    value={usuario}
                    onChange={(e) => setUsuario(e.target.value)}
                />

                <input
                    type="password"
                    placeholder="Contraseña"
                    value={contraseña}
                    onChange={(e) => setContraseña(e.target.value)}
                />

                <button type="submit">
                    Iniciar sesión
                </button>

            </form>
        </div>
    );
}

export default Login;