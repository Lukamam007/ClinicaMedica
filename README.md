# ClinicaMedica

La consiga a desarrollar es el flujo basico para registrar un nuevo turno:
- 1 — Ingresar DNI del paciente
El sistema solicita el DNI. Si el paciente existe, muestra sus datos incluidos los turnos reservados del paciente. Si el paciente desea cancelar un turno se selecciona el turno y se lo marca como cancelado. Si no existe, solicita nombre, apellido, teléfono, email y fecha de nacimiento, y lo registra.
- 2 — Seleccionar especialidad
El sistema lista todas las especialidades disponibles y solicita al usuario que elija una.
- 3 — Seleccionar médico
El sistema lista los médicos que atienden la especialidad seleccionada y solicita que el usuario elija uno.
- 4 — Mostrar disponibilidad
El sistema muestra los días y horarios en que el médico elegido atiende esa especialidad. El usuario ingresa la fecha y hora deseada.
- 5 — Confirmar y registrar el turno
El sistema muestra un resumen del turno y solicita confirmación. Si el usuario confirma, el turno se guarda con estado "reservado".

 Para instalar Entity Framework
- dotnet add package Microsoft.EntityFrameworkCore
- dotnet add package Microsoft.EntityFrameworkCore.Sqlite
