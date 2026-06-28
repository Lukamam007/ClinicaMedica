using System;
using Clinicamedica;
using Microsoft.EntityFrameworkCore;

namespace ClinicaMedica
{
    class Program
    {
        static void Main(string[] args)
        {
 
            using var db = new DBContext();

            Console.WriteLine("=== SISTEMA DE GESTIÓN DE TURNOS ===");

            Console.Write("1. Ingrese el DNI del paciente: ");
            string dniIngresado = Console.ReadLine();

            var paciente = db.Pacientes.FirstOrDefault(p => p.Dni == dniIngresado);

            if (paciente != null)
            {
                Console.WriteLine($"\n¡Bienvenido/a de nuevo, {paciente.NombreCompleto}!");

                var turnosReservados = db.Turnos
                    .Include(t => t.MedicoEspecialidad).ThenInclude(me => me.Medico)
                    .Include(t => t.MedicoEspecialidad).ThenInclude(me => me.Especialidad)
                    .Where(t => t.IdPaciente == paciente.IdPaciente && t.Estado == "reservado")
                    .ToList();

                if (turnosReservados.Any())
                {
                    Console.WriteLine("Tus turnos reservados actualmente:");
                    foreach (var t in turnosReservados)
                    {
                        Console.WriteLine($"[ID: {t.IdTurno}] - Fecha: {t.FechaHora} - Médico: {t.MedicoEspecialidad.Medico.NombreCompleto} ({t.MedicoEspecialidad.Especialidad.Nombre})");
                    }

                    Console.Write("\n¿Desea cancelar algún turno? (S/N): ");
                    if (Console.ReadLine().Trim().ToUpper() == "S")
                    {
                        Console.Write("Ingrese el ID del turno a cancelar: ");
                        if (int.TryParse(Console.ReadLine(), out int idTurnoCancelar))
                        {
                            var turnoACancelar = db.Turnos.Find(idTurnoCancelar);
                            if (turnoACancelar != null)
                            {
                                turnoACancelar.Estado = "cancelado";
                                db.SaveChanges();
                                Console.WriteLine("Turno cancelado exitosamente.");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No tenés turnos reservados actualmente.");
                }
            }
            else
            {
                Console.WriteLine("\nEl paciente no existe. Vamos a registrarlo.");
                paciente = new Paciente { Dni = dniIngresado, Activo = 1 };

                Console.Write("Nombre y Apellido: ");
                paciente.NombreCompleto = Console.ReadLine();

                Console.Write("Teléfono: ");
                paciente.Telefono = Console.ReadLine();

                Console.Write("Email: ");
                paciente.CorreoElectronico = Console.ReadLine();

                Console.Write("Fecha de Nacimiento (YYYY-MM-DD): ");
                paciente.FechaNacimiento = Console.ReadLine();

                db.Pacientes.Add(paciente);
                db.SaveChanges();
                Console.WriteLine("¡Paciente registrado con éxito!");
            }

            Console.WriteLine("\n--- 2. Seleccionar Especialidad ---");
            var especialidades = db.Especialidades.ToList();
            foreach (var esp in especialidades)
            {
                Console.WriteLine($"[{esp.IdEspecialidad}] {esp.Nombre} (Duración: {esp.DuracionMinutos} min)");
            }

            Console.Write("Ingrese el ID de la especialidad deseada: ");
            int idEspecialidadElegida = int.Parse(Console.ReadLine());
            var especialidadSeleccionada = especialidades.First(e => e.IdEspecialidad == idEspecialidadElegida);

            Console.WriteLine($"\n--- 3. Seleccionar Médico para {especialidadSeleccionada.Nombre} ---");

            var medicosDisponibles = db.Medico_Especialidad
                .Include(me => me.Medico)
                .Where(me => me.IdEspecialidad == idEspecialidadElegida && me.Medico.Activo == 1)
                .ToList();

            if (!medicosDisponibles.Any())
            {
                Console.WriteLine("No hay médicos disponibles para esta especialidad.");
                return;
            }

            foreach (var me in medicosDisponibles)
            {
                Console.WriteLine($"[{me.IdMedicoEspecialidad}] {me.Medico.NombreCompleto}");
            }

            Console.Write("Ingrese el ID del médico deseado: ");
            int idMedicoEspElegido = int.Parse(Console.ReadLine());
            var medicoElegido = medicosDisponibles.First(m => m.IdMedicoEspecialidad == idMedicoEspElegido);

            Console.WriteLine($"\n--- 4. Disponibilidad del {medicoElegido.Medico.NombreCompleto} ---");
            var disponibilidades = db.Disponibilidades
                .Where(d => d.IdMedicoEspecialidad == idMedicoEspElegido)
                .ToList();

            Console.WriteLine("Días y horarios de atención:");
            foreach (var d in disponibilidades)
            {
                Console.WriteLine($"- Día {d.DiaSemana}: De {d.HoraInicio} a {d.HoraFin}");
            }

            Console.Write("\nIngrese la Fecha y Hora para su turno (Formato YYYY-MM-DD HH:MM): ");
            string fechaHoraIngresada = Console.ReadLine();

            Console.WriteLine("\n--- 5. Confirmación del Turno ---");
            Console.WriteLine($"Paciente: {paciente.NombreCompleto}");
            Console.WriteLine($"Especialidad: {especialidadSeleccionada.Nombre}");
            Console.WriteLine($"Médico: {medicoElegido.Medico.NombreCompleto}");
            Console.WriteLine($"Fecha y Hora: {fechaHoraIngresada}");

            Console.Write("\n¿Confirma la reserva del turno? (S/N): ");
            if (Console.ReadLine().Trim().ToUpper() == "S")
            {
                var nuevoTurno = new Turno
                {
                    IdPaciente = paciente.IdPaciente,
                    IdMedicoEspecialidad = idMedicoEspElegido,
                    FechaHora = fechaHoraIngresada,
                    Estado = "reservado"
                };

                db.Turnos.Add(nuevoTurno);
                db.SaveChanges();

                Console.WriteLine("\n¡El turno se guardó con éxito como RESERVADO!");
            }
            else
            {
                Console.WriteLine("\nReserva cancelada.");
            }

            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
