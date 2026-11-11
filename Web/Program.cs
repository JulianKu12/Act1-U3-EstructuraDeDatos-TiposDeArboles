using RegistroAlumnosABB;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ArbolBinarioBusqueda>();

WebApplication app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/arbol", (ArbolBinarioBusqueda arbol) =>
    Results.Ok(new
    {
        raiz = ConvertirNodo(arbol.Raiz),
        altura = arbol.Altura(),
        nodos = arbol.ContarNodos(),
        minimo = ConvertirNodo(arbol.Minimo()),
        maximo = ConvertirNodo(arbol.Maximo())
    }));

app.MapPost("/api/alumnos", (AlumnoEntrada entrada, ArbolBinarioBusqueda arbol) =>
{
    if (!EntradaValida(entrada, out string error))
    {
        return Results.BadRequest(new { mensaje = error });
    }

    if (!arbol.Insertar(entrada.Matricula, entrada.Nombre.Trim(), entrada.Carrera.Trim()))
    {
        return Results.Conflict(new { mensaje = "La matrícula ya existe." });
    }

    return Results.Created($"/api/alumnos/{entrada.Matricula}", new
    {
        mensaje = "Alumno insertado correctamente.",
        alumno = ConvertirNodo(arbol.Buscar(entrada.Matricula))
    });
});

app.MapGet("/api/alumnos/{matricula:int}", (int matricula, ArbolBinarioBusqueda arbol) =>
{
    Nodo? alumno = arbol.Buscar(matricula);
    return alumno is null
        ? Results.NotFound(new { mensaje = "No existe un alumno con esa matrícula." })
        : Results.Ok(ConvertirNodo(alumno));
});

app.MapDelete("/api/alumnos/{matricula:int}", (int matricula, ArbolBinarioBusqueda arbol) =>
    arbol.Eliminar(matricula)
        ? Results.Ok(new { mensaje = "Alumno eliminado correctamente." })
        : Results.NotFound(new { mensaje = "No existe un alumno con esa matrícula." }));

app.MapGet("/api/recorridos/{tipo}", (string tipo, ArbolBinarioBusqueda arbol) =>
{
    List<Nodo>? recorrido = tipo.ToLowerInvariant() switch
    {
        "anchura" => arbol.ObtenerRecorridoAnchura(),
        "preorden" => arbol.ObtenerPreorden(),
        "inorden" => arbol.ObtenerInorden(),
        "postorden" => arbol.ObtenerPostorden(),
        _ => null
    };

    return recorrido is null
        ? Results.BadRequest(new { mensaje = "Tipo inválido. Usa anchura, preorden, inorden o postorden." })
        : Results.Ok(new { tipo, nodos = recorrido.Select(ConvertirNodo).ToList() });
});

app.MapPost("/api/datos-prueba", (ArbolBinarioBusqueda arbol) =>
{
    (int matricula, string nombre, string carrera)[] alumnos =
    {
        (2050, "Ana López", "Ingeniería de Software"),
        (2030, "Bruno García", "Redes"),
        (2070, "Carla Méndez", "Diseño Digital"),
        (2020, "Diego Torres", "Contaduría"),
        (2040, "Elena Ruiz", "Administración"),
        (2060, "Fabián Cruz", "Mecatrónica"),
        (2080, "Gabriela Soto", "Arquitectura")
    };

    int insertados = alumnos.Count(alumno =>
        arbol.Insertar(alumno.matricula, alumno.nombre, alumno.carrera));

    return Results.Ok(new
    {
        mensaje = $"{insertados} alumnos de prueba insertados. Los duplicados fueron omitidos.",
        insertados
    });
});

app.MapDelete("/api/arbol", (ArbolBinarioBusqueda arbol) =>
{
    while (arbol.Raiz is not null)
    {
        arbol.Eliminar(arbol.Raiz.Matricula);
    }

    return Results.Ok(new { mensaje = "El árbol se limpió correctamente." });
});

app.Run();

static bool EntradaValida(AlumnoEntrada entrada, out string error)
{
    if (entrada.Matricula <= 0)
    {
        error = "La matrícula debe ser un número entero positivo.";
        return false;
    }

    if (string.IsNullOrWhiteSpace(entrada.Nombre))
    {
        error = "El nombre no puede estar vacío.";
        return false;
    }

    if (string.IsNullOrWhiteSpace(entrada.Carrera))
    {
        error = "La carrera no puede estar vacía.";
        return false;
    }

    error = string.Empty;
    return true;
}

static object? ConvertirNodo(Nodo? nodo)
{
    return nodo is null
        ? null
        : new
        {
            matricula = nodo.Matricula,
            nombre = nodo.Nombre,
            carrera = nodo.Carrera,
            izquierdo = ConvertirNodo(nodo.Izquierdo),
            derecho = ConvertirNodo(nodo.Derecho)
        };
}

public sealed record AlumnoEntrada(int Matricula, string Nombre, string Carrera);
