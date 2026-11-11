namespace RegistroAlumnosABB;

public static class Program
{
    public static void Main()
    {
        ArbolBinarioBusqueda arbol = new();
        bool salir = false;

        while (!salir)
        {
            MostrarMenu();
            int opcion = LeerEntero("Selecciona una opción: ");
            Console.WriteLine();

            switch (opcion)
            {
                case 1:
                    InsertarAlumno(arbol);
                    break;
                case 2:
                    BuscarAlumno(arbol);
                    break;
                case 3:
                    EliminarAlumno(arbol);
                    break;
                case 4:
                    Console.WriteLine("Recorrido en anchura:");
                    arbol.RecorridoAnchura();
                    break;
                case 5:
                    Console.Write("Preorden: ");
                    arbol.Preorden();
                    break;
                case 6:
                    Console.Write("Inorden: ");
                    arbol.Inorden();
                    break;
                case 7:
                    Console.Write("Postorden: ");
                    arbol.Postorden();
                    break;
                case 8:
                    Console.WriteLine($"Altura: {arbol.Altura()} niveles.");
                    break;
                case 9:
                    Console.WriteLine($"Nodos: {arbol.ContarNodos()}.");
                    break;
                case 10:
                    MostrarExtremo("Mínimo", arbol.Minimo());
                    break;
                case 11:
                    MostrarExtremo("Máximo", arbol.Maximo());
                    break;
                case 12:
                    CargarDatosPrueba(arbol);
                    break;
                case 0:
                    salir = true;
                    Console.WriteLine("Programa finalizado.");
                    break;
                default:
                    Console.WriteLine("Opción inválida. Elige un número del menú.");
                    break;
            }

            if (!salir)
            {
                Console.WriteLine("\nPresiona Enter para continuar...");
                Console.ReadLine();
                LimpiarPantalla();
            }
        }
    }

    private static void MostrarMenu()
    {
        Console.WriteLine("==============================================");
        Console.WriteLine("   REGISTRO DE ALUMNOS - ÁRBOL BINARIO");
        Console.WriteLine("==============================================");
        Console.WriteLine("1. Insertar alumno");
        Console.WriteLine("2. Buscar alumno");
        Console.WriteLine("3. Eliminar alumno");
        Console.WriteLine("4. Recorrido en anchura");
        Console.WriteLine("5. Recorrido preorden");
        Console.WriteLine("6. Recorrido inorden");
        Console.WriteLine("7. Recorrido postorden");
        Console.WriteLine("8. Mostrar altura");
        Console.WriteLine("9. Contar nodos");
        Console.WriteLine("10. Mostrar matrícula mínima");
        Console.WriteLine("11. Mostrar matrícula máxima");
        Console.WriteLine("12. Cargar datos de prueba");
        Console.WriteLine("0. Salir");
        Console.WriteLine("==============================================");
    }

    private static void InsertarAlumno(ArbolBinarioBusqueda arbol)
    {
        int matricula = LeerEntero("Matrícula: ");
        string nombre = LeerTextoNoVacio("Nombre: ");
        string carrera = LeerTextoNoVacio("Carrera: ");

        if (arbol.Insertar(matricula, nombre, carrera))
        {
            Console.WriteLine("Alumno insertado correctamente.");
        }
        else
        {
            Console.WriteLine("No se insertó: la matrícula ya existe.");
        }
    }

    private static void BuscarAlumno(ArbolBinarioBusqueda arbol)
    {
        int matricula = LeerEntero("Matrícula a buscar: ");
        Nodo? alumno = arbol.Buscar(matricula);

        Console.WriteLine(alumno is null
            ? "No existe un alumno con esa matrícula."
            : $"Alumno encontrado: {alumno}");
    }

    private static void EliminarAlumno(ArbolBinarioBusqueda arbol)
    {
        int matricula = LeerEntero("Matrícula a eliminar: ");
        Console.WriteLine(arbol.Eliminar(matricula)
            ? "Alumno eliminado correctamente."
            : "No existe un alumno con esa matrícula.");
    }

    private static void MostrarExtremo(string nombre, Nodo? alumno)
    {
        Console.WriteLine(alumno is null
            ? $"No se puede obtener el {nombre.ToLower()} porque el árbol está vacío."
            : $"{nombre}: {alumno}");
    }

    private static void CargarDatosPrueba(ArbolBinarioBusqueda arbol)
    {
        (int matricula, string nombre, string carrera)[] alumnos =
        {
            (2050, "Ana López", "Ingeniería de Software"),
            (2030, "Bruno García", "Redes"),
            (2070, "Carla Méndez", "Diseño Digital"),
            (2020, "Diego Torres", "Contaduría"),
            (2040, "Elena Ruiz", "Administración"),
            (2060, "Fabian Cruz", "Mecatrónica"),
            (2080, "Gabriela Soto", "Arquitectura")
        };

        int insertados = 0;
        foreach (var alumno in alumnos)
        {
            if (arbol.Insertar(alumno.matricula, alumno.nombre, alumno.carrera))
            {
                insertados++;
            }
        }

        Console.WriteLine(insertados == alumnos.Length
            ? "Datos de prueba cargados correctamente."
            : $"Se cargaron {insertados} alumnos nuevos; los duplicados fueron omitidos.");
    }

    private static int LeerEntero(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string entrada = Console.ReadLine() ?? string.Empty;

            if (int.TryParse(entrada, out int valor))
            {
                return valor;
            }

            Console.WriteLine("Entrada inválida: escribe un número entero.");
        }
    }

    private static string LeerTextoNoVacio(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            string entrada = (Console.ReadLine() ?? string.Empty).Trim();

            if (entrada.Length > 0)
            {
                return entrada;
            }

            Console.WriteLine("El texto no puede estar vacío.");
        }
    }

    private static void LimpiarPantalla()
    {
        if (!Console.IsInputRedirected && !Console.IsOutputRedirected)
        {
            Console.Clear();
        }
    }
}
