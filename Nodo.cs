namespace RegistroAlumnosABB;

// Representa un alumno y sus dos posibles hijos dentro del árbol.
public class Nodo
{
    public int Matricula { get; set; }
    public string Nombre { get; set; }
    public string Carrera { get; set; }
    public Nodo? Izquierdo { get; set; }
    public Nodo? Derecho { get; set; }

    public Nodo(int matricula, string nombre, string carrera)
    {
        Matricula = matricula;
        Nombre = nombre;
        Carrera = carrera;
    }

    public override string ToString()
    {
        return $"{Matricula} - {Nombre} - {Carrera}";
    }
}
