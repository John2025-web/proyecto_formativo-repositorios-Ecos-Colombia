import { useState } from "react";
import { login } from "../services/api";

function Login() {

    const [usuario, setUsuario] = useState("");
    const [password, setPassword] = useState("");

    const handleLogin = async (e) => {
    e.preventDefault();

    try {
        await login(usuario, password, "Usuario");

    } catch (error) {
        alert(error.message);
    }
};
    return (
        <div>
            <h1>Iniciar sesión</h1>

            <form onSubmit={handleLogin}>

                <input
                    type="text"
                    placeholder="Usuario o correo electrónico"
                    value={usuario}
                    onChange={(e) => setUsuario(e.target.value)}
                />

                <input
                    type="password"
                    placeholder="Contraseña"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                />

                <button type="submit">
                    Iniciar sesión
                </button>

            </form>
        </div>
    );
}

export default Login;