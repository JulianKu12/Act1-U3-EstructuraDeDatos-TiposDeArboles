const svg = document.querySelector("#arbol-svg");
const mensaje = document.querySelector("#mensaje");
let estado = { raiz: null, nodos: 0, altura: 0, minimo: null, maximo: null };
let zoom = 1;
let animando = false;

async function solicitar(url, opciones = {}) {
    const respuesta = await fetch(url, { headers: { "Content-Type": "application/json" }, ...opciones });
    const datos = await respuesta.json().catch(() => ({}));
    if (!respuesta.ok) throw new Error(datos.mensaje || "No fue posible completar la operación.");
    return datos;
}

function mostrarMensaje(texto, tipo = "exito") {
    mensaje.textContent = texto;
    mensaje.className = tipo;
}

function datosFormulario() {
    const matricula = Number(document.querySelector("#matricula").value);
    const nombre = document.querySelector("#nombre").value.trim();
    const carrera = document.querySelector("#carrera").value.trim();
    if (!Number.isInteger(matricula) || matricula <= 0) throw new Error("La matrícula debe ser un entero positivo.");
    if (!nombre) throw new Error("El nombre no puede estar vacío.");
    if (!carrera) throw new Error("La carrera no puede estar vacía.");
    return { matricula, nombre, carrera };
}

async function actualizar() {
    estado = await solicitar("/api/arbol");
    document.querySelector("#altura").textContent = estado.altura;
    document.querySelector("#nodos").textContent = estado.nodos;
    document.querySelector("#minimo").textContent = estado.minimo?.matricula ?? "-";
    document.querySelector("#maximo").textContent = estado.maximo?.matricula ?? "-";
    dibujar();
    cargarTabla();
}

function mostrarError(error) {
    mostrarMensaje(error.message, "error");
}

document.querySelector("#alumno-form").addEventListener("submit", async (evento) => {
    evento.preventDefault();
    try {
        await solicitar("/api/alumnos", { method: "POST", body: JSON.stringify(datosFormulario()) });
        mostrarMensaje("Alumno insertado correctamente.");
        await actualizar();
    } catch (error) { mostrarError(error); }
});

document.querySelector("#buscar").addEventListener("click", async () => {
    try {
        const matricula = Number(document.querySelector("#matricula").value);
        if (!Number.isInteger(matricula) || matricula <= 0) throw new Error("Escribe una matrícula entera positiva.");
        const alumno = await solicitar(`/api/alumnos/${matricula}`);
        dibujar(matricula);
        mostrarMensaje(`Alumno encontrado: ${alumno.nombre} - ${alumno.carrera}.`);
    } catch (error) { mostrarError(error); }
});

document.querySelector("#eliminar").addEventListener("click", async () => {
    try {
        const matricula = Number(document.querySelector("#matricula").value);
        if (!Number.isInteger(matricula) || matricula <= 0) throw new Error("Escribe una matrícula entera positiva.");
        const respuesta = await solicitar(`/api/alumnos/${matricula}`, { method: "DELETE" });
        mostrarMensaje(respuesta.mensaje);
        await actualizar();
    } catch (error) { mostrarError(error); }
});

document.querySelector("#prueba").addEventListener("click", async () => {
    try {
        const respuesta = await solicitar("/api/datos-prueba", { method: "POST" });
        mostrarMensaje(respuesta.mensaje);
        await actualizar();
    } catch (error) { mostrarError(error); }
});

document.querySelector("#limpiar").addEventListener("click", async () => {
    try {
        const respuesta = await solicitar("/api/arbol", { method: "DELETE" });
        mostrarMensaje(respuesta.mensaje);
        await actualizar();
    } catch (error) { mostrarError(error); }
});

function recolectar(nodo, nivel, posiciones, resultado) {
    if (!nodo) return;
    recolectar(nodo.izquierdo, nivel + 1, posiciones, resultado);
    resultado.push({ nodo, nivel, indice: resultado.length });
    recolectar(nodo.derecho, nivel + 1, posiciones, resultado);
}

function dibujar(resaltado = null, visitados = new Set(), ordenes = new Map()) {
    svg.replaceChildren();
    const nodos = [];
    recolectar(estado.raiz, 0, new Map(), nodos);
    const ancho = Math.max(900, nodos.length * 150);
    const alto = Math.max(400, (estado.altura + 1) * 125);
    svg.setAttribute("width", ancho);
    svg.setAttribute("height", alto);
    svg.style.transform = `scale(${zoom})`;
    const posiciones = new Map();
    nodos.forEach((item) => posiciones.set(item.nodo.matricula, {
        x: 75 + item.indice * ((ancho - 150) / Math.max(1, nodos.length - 1)),
        y: 65 + item.nivel * 115
    }));
    nodos.forEach((item) => {
        const punto = posiciones.get(item.nodo.matricula);
        [item.nodo.izquierdo, item.nodo.derecho].forEach((hijo) => {
            if (!hijo) return;
            const destino = posiciones.get(hijo.matricula);
            const linea = document.createElementNS("http://www.w3.org/2000/svg", "line");
            linea.setAttribute("x1", punto.x); linea.setAttribute("y1", punto.y);
            linea.setAttribute("x2", destino.x); linea.setAttribute("y2", destino.y);
            linea.classList.add("linea"); svg.append(linea);
        });
    });
    nodos.forEach((item) => {
        const punto = posiciones.get(item.nodo.matricula);
        const grupo = document.createElementNS("http://www.w3.org/2000/svg", "g");
        const circulo = document.createElementNS("http://www.w3.org/2000/svg", "circle");
        circulo.setAttribute("cx", punto.x); circulo.setAttribute("cy", punto.y); circulo.setAttribute("r", 38);
        circulo.classList.add("nodo-circulo");
        if (item.nodo === estado.raiz) circulo.classList.add("raiz");
        if (item.nodo.matricula === resaltado) circulo.classList.add("encontrado");
        if (visitados.has(item.nodo.matricula)) circulo.classList.add("visitado");
        const texto = document.createElementNS("http://www.w3.org/2000/svg", "text");
        texto.setAttribute("x", punto.x); texto.setAttribute("y", punto.y + 4);
        texto.classList.add("nodo-texto"); texto.textContent = item.nodo.matricula;
        const nombre = document.createElementNS("http://www.w3.org/2000/svg", "text");
        nombre.setAttribute("x", punto.x); nombre.setAttribute("y", punto.y + 58);
        nombre.classList.add("nodo-texto", "nodo-nombre"); nombre.textContent = item.nodo.nombre;
        const tarjeta = document.createElementNS("http://www.w3.org/2000/svg", "title");
        tarjeta.textContent = `${item.nodo.matricula} - ${item.nodo.nombre} - ${item.nodo.carrera}`;
        grupo.append(circulo, texto, nombre);
        if (ordenes.has(item.nodo.matricula)) {
            const orden = document.createElementNS("http://www.w3.org/2000/svg", "text");
            orden.setAttribute("x", punto.x + 31); orden.setAttribute("y", punto.y - 27);
            orden.classList.add("nodo-texto"); orden.textContent = ordenes.get(item.nodo.matricula);
            grupo.append(orden);
        }
        grupo.append(tarjeta); svg.append(grupo);
    });
}

async function cargarTabla() {
    const datos = await solicitar("/api/recorridos/inorden");
    document.querySelector("#tabla-alumnos").innerHTML = datos.nodos.map((alumno) =>
        `<tr><td>${alumno.matricula}</td><td>${alumno.nombre}</td><td>${alumno.carrera}</td></tr>`).join("");
}

document.querySelectorAll("[data-recorrido]").forEach((boton) => {
    boton.addEventListener("click", async () => {
        if (animando) return;
        animando = true;
        document.querySelectorAll("[data-recorrido]").forEach((elemento) => elemento.disabled = true);
        try {
            const tipo = boton.dataset.recorrido;
            const datos = await solicitar(`/api/recorridos/${tipo}`);
            const visitados = new Set();
            const ordenes = new Map();
            for (let i = 0; i < datos.nodos.length; i++) {
                visitados.add(datos.nodos[i].matricula);
                ordenes.set(datos.nodos[i].matricula, i + 1);
                dibujar(null, visitados, ordenes);
                document.querySelector("#resultado-recorrido").textContent =
                    `Visitando ${i + 1}: ${datos.nodos[i].matricula}`;
                const valorVelocidad = Number(document.querySelector("#velocidad").value);
                const retardo = 1600 - valorVelocidad;
                await new Promise((resolver) => setTimeout(resolver, retardo));
            }
            document.querySelector("#resultado-recorrido").textContent =
                `${tipo}: ${datos.nodos.map((nodo) => nodo.matricula).join(", ")}`;
        } catch (error) { mostrarError(error); }
        finally {
            animando = false;
            document.querySelectorAll("[data-recorrido]").forEach((elemento) => elemento.disabled = false);
        }
    });
});

document.querySelector("#zoom-mas").addEventListener("click", () => { zoom = Math.min(2, zoom + .1); dibujar(); document.querySelector("#zoom-texto").textContent = `${Math.round(zoom * 100)}%`; });
document.querySelector("#zoom-menos").addEventListener("click", () => { zoom = Math.max(.5, zoom - .1); dibujar(); document.querySelector("#zoom-texto").textContent = `${Math.round(zoom * 100)}%`; });

actualizar().catch(mostrarError);
