namespace RegistroAlumnosABB;

// Árbol Binario de Búsqueda ordenado por la matrícula del alumno.
public class ArbolBinarioBusqueda
{
    public Nodo? Raiz { get; private set; }

    // Inserta un alumno y rechaza las matrículas repetidas.
    public bool Insertar(int matricula, string nombre, string carrera)
    {
        if (Raiz is null)
        {
            Raiz = new Nodo(matricula, nombre, carrera);
            return true;
        }

        Nodo actual = Raiz;
        while (true)
        {
            if (matricula == actual.Matricula)
            {
                return false;
            }

            if (matricula < actual.Matricula)
            {
                if (actual.Izquierdo is null)
                {
                    actual.Izquierdo = new Nodo(matricula, nombre, carrera);
                    return true;
                }

                actual = actual.Izquierdo;
            }
            else
            {
                if (actual.Derecho is null)
                {
                    actual.Derecho = new Nodo(matricula, nombre, carrera);
                    return true;
                }

                actual = actual.Derecho;
            }
        }
    }

    // Busca una matrícula y devuelve el nodo encontrado o null.
    public Nodo? Buscar(int matricula)
    {
        Nodo? actual = Raiz;
        while (actual is not null)
        {
            if (matricula == actual.Matricula)
            {
                return actual;
            }

            actual = matricula < actual.Matricula
                ? actual.Izquierdo
                : actual.Derecho;
        }

        return null;
    }

    // Elimina un nodo hoja, con un hijo o con dos hijos.
    // En el último caso reemplaza sus datos por el sucesor inorden.
    public bool Eliminar(int matricula)
    {
        Nodo? padre = null;
        Nodo? actual = Raiz;

        while (actual is not null && actual.Matricula != matricula)
        {
            padre = actual;
            actual = matricula < actual.Matricula
                ? actual.Izquierdo
                : actual.Derecho;
        }

        if (actual is null)
        {
            return false;
        }

        if (actual.Izquierdo is not null && actual.Derecho is not null)
        {
            Nodo sucesorPadre = actual;
            Nodo sucesor = actual.Derecho;

            while (sucesor.Izquierdo is not null)
            {
                sucesorPadre = sucesor;
                sucesor = sucesor.Izquierdo;
            }

            actual.Matricula = sucesor.Matricula;
            actual.Nombre = sucesor.Nombre;
            actual.Carrera = sucesor.Carrera;
            padre = sucesorPadre;
            actual = sucesor;
        }

        Nodo? hijo = actual.Izquierdo ?? actual.Derecho;

        if (padre is null)
        {
            Raiz = hijo;
        }
        else if (padre.Izquierdo == actual)
        {
            padre.Izquierdo = hijo;
        }
        else
        {
            padre.Derecho = hijo;
        }

        return true;
    }

    // Imprime el árbol por niveles usando una Queue.
    public void RecorridoAnchura()
    {
        if (Raiz is null)
        {
            Console.WriteLine("El árbol está vacío.");
            return;
        }

        Queue<Nodo> pendientes = new();
        pendientes.Enqueue(Raiz);

        while (pendientes.Count > 0)
        {
            int nodosDelNivel = pendientes.Count;
            for (int i = 0; i < nodosDelNivel; i++)
            {
                Nodo actual = pendientes.Dequeue();
                Console.Write($"{actual.Matricula} ");

                if (actual.Izquierdo is not null)
                {
                    pendientes.Enqueue(actual.Izquierdo);
                }

                if (actual.Derecho is not null)
                {
                    pendientes.Enqueue(actual.Derecho);
                }
            }
            Console.WriteLine();
        }
    }

    // Devuelve los nodos por niveles para que otras interfaces puedan reutilizar el recorrido.
    public List<Nodo> ObtenerRecorridoAnchura()
    {
        List<Nodo> recorrido = new();
        if (Raiz is null)
        {
            return recorrido;
        }

        Queue<Nodo> pendientes = new();
        pendientes.Enqueue(Raiz);
        while (pendientes.Count > 0)
        {
            Nodo actual = pendientes.Dequeue();
            recorrido.Add(actual);

            if (actual.Izquierdo is not null)
            {
                pendientes.Enqueue(actual.Izquierdo);
            }

            if (actual.Derecho is not null)
            {
                pendientes.Enqueue(actual.Derecho);
            }
        }

        return recorrido;
    }

    // Recorre raíz, izquierda y derecha.
    public void Preorden()
    {
        Preorden(Raiz);
        Console.WriteLine();
    }

    // Devuelve los nodos en preorden: raíz, izquierda y derecha.
    public List<Nodo> ObtenerPreorden()
    {
        List<Nodo> recorrido = new();
        ObtenerPreorden(Raiz, recorrido);
        return recorrido;
    }

    private static void ObtenerPreorden(Nodo? nodo, List<Nodo> recorrido)
    {
        if (nodo is null)
        {
            return;
        }

        recorrido.Add(nodo);
        ObtenerPreorden(nodo.Izquierdo, recorrido);
        ObtenerPreorden(nodo.Derecho, recorrido);
    }

    private static void Preorden(Nodo? nodo)
    {
        if (nodo is null)
        {
            return;
        }

        Console.Write($"{nodo.Matricula} ");
        Preorden(nodo.Izquierdo);
        Preorden(nodo.Derecho);
    }

    // Recorre izquierda, raíz y derecha.
    public void Inorden()
    {
        Inorden(Raiz);
        Console.WriteLine();
    }

    // Devuelve los nodos en inorden: izquierda, raíz y derecha.
    public List<Nodo> ObtenerInorden()
    {
        List<Nodo> recorrido = new();
        ObtenerInorden(Raiz, recorrido);
        return recorrido;
    }

    private static void ObtenerInorden(Nodo? nodo, List<Nodo> recorrido)
    {
        if (nodo is null)
        {
            return;
        }

        ObtenerInorden(nodo.Izquierdo, recorrido);
        recorrido.Add(nodo);
        ObtenerInorden(nodo.Derecho, recorrido);
    }

    private static void Inorden(Nodo? nodo)
    {
        if (nodo is null)
        {
            return;
        }

        Inorden(nodo.Izquierdo);
        Console.Write($"{nodo.Matricula} ");
        Inorden(nodo.Derecho);
    }

    // Recorre izquierda, derecha y raíz.
    public void Postorden()
    {
        Postorden(Raiz);
        Console.WriteLine();
    }

    // Devuelve los nodos en postorden: izquierda, derecha y raíz.
    public List<Nodo> ObtenerPostorden()
    {
        List<Nodo> recorrido = new();
        ObtenerPostorden(Raiz, recorrido);
        return recorrido;
    }

    private static void ObtenerPostorden(Nodo? nodo, List<Nodo> recorrido)
    {
        if (nodo is null)
        {
            return;
        }

        ObtenerPostorden(nodo.Izquierdo, recorrido);
        ObtenerPostorden(nodo.Derecho, recorrido);
        recorrido.Add(nodo);
    }

    private static void Postorden(Nodo? nodo)
    {
        if (nodo is null)
        {
            return;
        }

        Postorden(nodo.Izquierdo);
        Postorden(nodo.Derecho);
        Console.Write($"{nodo.Matricula} ");
    }

    // Devuelve la cantidad de niveles del árbol.
    public int Altura()
    {
        return Altura(Raiz);
    }

    private static int Altura(Nodo? nodo)
    {
        if (nodo is null)
        {
            return 0;
        }

        return 1 + Math.Max(Altura(nodo.Izquierdo), Altura(nodo.Derecho));
    }

    // Cuenta todos los nodos del árbol.
    public int ContarNodos()
    {
        return ContarNodos(Raiz);
    }

    private static int ContarNodos(Nodo? nodo)
    {
        return nodo is null
            ? 0
            : 1 + ContarNodos(nodo.Izquierdo) + ContarNodos(nodo.Derecho);
    }

    // Devuelve el nodo con la matrícula más pequeña.
    public Nodo? Minimo()
    {
        if (Raiz is null)
        {
            return null;
        }

        Nodo minimo = Raiz;
        while (minimo.Izquierdo is not null)
        {
            minimo = minimo.Izquierdo;
        }

        return minimo;
    }

    // Devuelve el nodo con la matrícula más grande.
    public Nodo? Maximo()
    {
        if (Raiz is null)
        {
            return null;
        }

        Nodo maximo = Raiz;
        while (maximo.Derecho is not null)
        {
            maximo = maximo.Derecho;
        }

        return maximo;
    }
}
