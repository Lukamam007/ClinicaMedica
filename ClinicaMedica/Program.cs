using Clinicamedica;
using Microsoft.EntityFrameworkCore;

namespace ClinicaMedica
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = new ClinicaContext();

            //seleccionar y cargar todos los turnos y sus relaciones
            var turnos = context.Turnos
                .Include(t => t.Paciente)
                .Include(t => t.Medico)
                .Include(t => t.Especialidad)
                .Include(t => t.Estado)
                .OrderBy(t => t.Fecha)
                .ThenBy(t => t.Hora)
                .ToList();

            foreach (var t in turnos)
            {
                Console.WriteLine($"{t.Fecha} {t.Hora} | {t.Paciente.Nombre} {t.Paciente.Apellido} | {t.Medico.Nombre} {t.Medico.Apellido} | {t.Especialidad.Nombre} | {t.Estado.Descripcion}");
            }

            var turnos2 = context.Turnos.Where(t => t.Estado.Descripcion == "cancelado");
            foreach (var turno in turnos2)
            {
                
                Console.WriteLine($"{turno.Estado.Descripcion} | {turno.Especialidad.Nombre} | {turno.Paciente.Nombre}");
            }

            Paciente paciente = context.Pacientes.FirstOrDefault(p => p.Dni == 27999000);
            Console.WriteLine($"{paciente.Nombre} {paciente.Apellido}");
        }
    }
}
