al momento de registrar una formula receta se pausa el proyecto backend y me salta error:
POST
/api/Formulas

{
  "nombre": "register",
  "etapa": "Engorde",
  "cantidadBase": 100,
  "detalles": [
    {
      "productoId": "79bf94e4-9bbc-49b8-a7e5-36d63fa9c742",
      "cantidadPorBase": 10
    }
  ]
}

Microsoft.EntityFrameworkCore.DbUpdateException
  HResult=0x80131500
  Mensaje = An error occurred while saving the entity changes. See the inner exception for details.
  Origen = Microsoft.EntityFrameworkCore.Relational
  Seguimiento de la pila:
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.<SaveChangesAsync>d__8.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__113.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__117.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.<ExecuteAsync>d__7`2.MoveNext()
   en Microsoft.EntityFrameworkCore.DbContext.<SaveChangesAsync>d__63.MoveNext()
   en Microsoft.EntityFrameworkCore.DbContext.<SaveChangesAsync>d__63.MoveNext()
   en GalponERP.Infrastructure.Persistence.UnitOfWork.<SaveChangesAsync>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Infrastructure\Persistence\UnitOfWork.cs: línea 16

  Esta excepción se generó originalmente en esta pila de llamadas:
    [Código externo]

Excepción interna 1:
PostgresException: 23503: insert or update on table "FormulaDetalles" violates foreign key constraint "FK_FormulaDetalles_Productos_ProductoId"

DETAIL: Key (ProductoId)=(79bf94e4-9bbc-49b8-a7e5-36d63fa9c742) is not present in table "Productos".

Microsoft.EntityFrameworkCore.DbUpdateException
  HResult=0x80131500
  Mensaje = An error occurred while saving the entity changes. See the inner exception for details.
  Origen = Microsoft.EntityFrameworkCore.Relational
  Seguimiento de la pila:
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.<SaveChangesAsync>d__8.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__113.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__117.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.<ExecuteAsync>d__7`2.MoveNext()
   en Microsoft.EntityFrameworkCore.DbContext.<SaveChangesAsync>d__63.MoveNext()
   en Microsoft.EntityFrameworkCore.DbContext.<SaveChangesAsync>d__63.MoveNext()
   en GalponERP.Infrastructure.Persistence.UnitOfWork.<SaveChangesAsync>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Infrastructure\Persistence\UnitOfWork.cs: línea 16
   en GalponERP.Application.Nutricion.Formulas.Commands.CrearFormula.CrearFormulaCommandHandler.<Handle>d__3.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Nutricion\Formulas\Commands\CrearFormula\CrearFormulaCommandHandler.cs: línea 34

  Esta excepción se generó originalmente en esta pila de llamadas:
    [Código externo]

Excepción interna 1:
PostgresException: 23503: insert or update on table "FormulaDetalles" violates foreign key constraint "FK_FormulaDetalles_Productos_ProductoId"

DETAIL: Key (ProductoId)=(79bf94e4-9bbc-49b8-a7e5-36d63fa9c742) is not present in table "Productos".

Code	Details
500
Undocumented
Error: response status is 500

Response body
Download
{
  "title": "Server Error",
  "status": 500,
  "detail": "An unexpected error occurred",
  "instance": "/api/Formulas"
}

al momento de editar una formula "Receta" y al mandar los nuevos datos lo que pasa es que el proyecto del backend se pausa y me salta este error:

si el error en el endpoind:
PUT
/api/Formulas/{id}
Name	Description
id *
string($uuid)
(path)
79bf94e4-9bbc-49b8-a7e5-36d63fa9c742

{
  "id": "79bf94e4-9bbc-49b8-a7e5-36d63fa9c742",
  "nombre": "test update ",
  "etapa": "Engorde",
  "cantidadBase": 10,
  "detalles": [
    {
      "productoId": "de4e2f44-288f-4007-9768-91d97b66b3e0",
      "cantidadPorBase": 10
    }
  ]
}

Code	Details
500
Undocumented
Error: response status is 500

Response body
Download
{
  "title": "Server Error",
  "status": 500,
  "detail": "An unexpected error occurred",
  "instance": "/api/Formulas/79bf94e4-9bbc-49b8-a7e5-36d63fa9c742"
} 

Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException
  HResult=0x80131500
  Mensaje = The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded. See https://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions.
  Origen = Npgsql.EntityFrameworkCore.PostgreSQL
  Seguimiento de la pila:
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<ThrowAggregateUpdateConcurrencyExceptionAsync>d__10.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<Consume>d__7.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.<SaveChangesAsync>d__8.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__113.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__117.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.<ExecuteAsync>d__7`2.MoveNext()
   en Microsoft.EntityFrameworkCore.DbContext.<SaveChangesAsync>d__63.MoveNext()
   en GalponERP.Infrastructure.Persistence.UnitOfWork.<SaveChangesAsync>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Infrastructure\Persistence\UnitOfWork.cs: línea 16

  Esta excepción se generó originalmente en esta pila de llamadas:
    [Código externo]
    GalponERP.Infrastructure.Persistence.UnitOfWork.SaveChangesAsync(System.Threading.CancellationToken) en UnitOfWork.cs

Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException
  HResult=0x80131500
  Mensaje = The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded. See https://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions.
  Origen = Npgsql.EntityFrameworkCore.PostgreSQL
  Seguimiento de la pila:
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<ThrowAggregateUpdateConcurrencyExceptionAsync>d__10.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<Consume>d__7.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.<SaveChangesAsync>d__8.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__113.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__117.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.<ExecuteAsync>d__7`2.MoveNext()
   en Microsoft.EntityFrameworkCore.DbContext.<SaveChangesAsync>d__63.MoveNext()
   en GalponERP.Infrastructure.Persistence.UnitOfWork.<SaveChangesAsync>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Infrastructure\Persistence\UnitOfWork.cs: línea 16
   en GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommandHandler.<Handle>d__3.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Nutricion\Formulas\Commands\ActualizarFormula\ActualizarFormulaCommandHandler.cs: línea 52

  Esta excepción se generó originalmente en esta pila de llamadas:
    [Código externo]
    GalponERP.Infrastructure.Persistence.UnitOfWork.SaveChangesAsync(System.Threading.CancellationToken) en UnitOfWork.cs
    GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommandHandler.Handle(GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommand, System.Threading.CancellationToken) en ActualizarFormulaCommandHandler.cs

Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException
  HResult=0x80131500
  Mensaje = The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded. See https://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions.
  Origen = Npgsql.EntityFrameworkCore.PostgreSQL
  Seguimiento de la pila:
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<ThrowAggregateUpdateConcurrencyExceptionAsync>d__10.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<Consume>d__7.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.<SaveChangesAsync>d__8.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__113.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__117.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.<ExecuteAsync>d__7`2.MoveNext()
   en Microsoft.EntityFrameworkCore.DbContext.<SaveChangesAsync>d__63.MoveNext()
   en GalponERP.Infrastructure.Persistence.UnitOfWork.<SaveChangesAsync>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Infrastructure\Persistence\UnitOfWork.cs: línea 16
   en GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommandHandler.<Handle>d__3.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Nutricion\Formulas\Commands\ActualizarFormula\ActualizarFormulaCommandHandler.cs: línea 52
   en GalponERP.Application.Behaviors.AuditoriaBehavior`2.<Handle>d__4.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Behaviors\AuditoriaBehavior.cs: línea 33

  Esta excepción se generó originalmente en esta pila de llamadas:
    [Código externo]
    GalponERP.Infrastructure.Persistence.UnitOfWork.SaveChangesAsync(System.Threading.CancellationToken) en UnitOfWork.cs
    GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommandHandler.Handle(GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommand, System.Threading.CancellationToken) en ActualizarFormulaCommandHandler.cs
    GalponERP.Application.Behaviors.AuditoriaBehavior<TRequest, TResponse>.Handle(TRequest, MediatR.RequestHandlerDelegate<TResponse>, System.Threading.CancellationToken) en AuditoriaBehavior.cs

Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException
  HResult=0x80131500
  Mensaje = The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded. See https://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions.
  Origen = Npgsql.EntityFrameworkCore.PostgreSQL
  Seguimiento de la pila:
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<ThrowAggregateUpdateConcurrencyExceptionAsync>d__10.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<Consume>d__7.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.<SaveChangesAsync>d__8.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__113.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__117.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.<ExecuteAsync>d__7`2.MoveNext()
   en Microsoft.EntityFrameworkCore.DbContext.<SaveChangesAsync>d__63.MoveNext()
   en GalponERP.Infrastructure.Persistence.UnitOfWork.<SaveChangesAsync>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Infrastructure\Persistence\UnitOfWork.cs: línea 16
   en GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommandHandler.<Handle>d__3.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Nutricion\Formulas\Commands\ActualizarFormula\ActualizarFormulaCommandHandler.cs: línea 52
   en GalponERP.Application.Behaviors.AuditoriaBehavior`2.<Handle>d__4.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Behaviors\AuditoriaBehavior.cs: línea 33
   en GalponERP.Application.Behaviors.ValidationBehavior`2.<Handle>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Behaviors\ValidationBehavior.cs: línea 24

  Esta excepción se generó originalmente en esta pila de llamadas:
    [Código externo]
    GalponERP.Infrastructure.Persistence.UnitOfWork.SaveChangesAsync(System.Threading.CancellationToken) en UnitOfWork.cs
    GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommandHandler.Handle(GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommand, System.Threading.CancellationToken) en ActualizarFormulaCommandHandler.cs
    GalponERP.Application.Behaviors.AuditoriaBehavior<TRequest, TResponse>.Handle(TRequest, MediatR.RequestHandlerDelegate<TResponse>, System.Threading.CancellationToken) en AuditoriaBehavior.cs

Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException
  HResult=0x80131500
  Mensaje = The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded. See https://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions.
  Origen = Npgsql.EntityFrameworkCore.PostgreSQL
  Seguimiento de la pila:
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<ThrowAggregateUpdateConcurrencyExceptionAsync>d__10.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<Consume>d__7.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.<SaveChangesAsync>d__8.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__113.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__117.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.<ExecuteAsync>d__7`2.MoveNext()
   en Microsoft.EntityFrameworkCore.DbContext.<SaveChangesAsync>d__63.MoveNext()
   en GalponERP.Infrastructure.Persistence.UnitOfWork.<SaveChangesAsync>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Infrastructure\Persistence\UnitOfWork.cs: línea 16
   en GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommandHandler.<Handle>d__3.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Nutricion\Formulas\Commands\ActualizarFormula\ActualizarFormulaCommandHandler.cs: línea 52
   en GalponERP.Application.Behaviors.AuditoriaBehavior`2.<Handle>d__4.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Behaviors\AuditoriaBehavior.cs: línea 33
   en GalponERP.Application.Behaviors.ValidationBehavior`2.<Handle>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Behaviors\ValidationBehavior.cs: línea 24

  Esta excepción se generó originalmente en esta pila de llamadas:
    [Código externo]
    GalponERP.Infrastructure.Persistence.UnitOfWork.SaveChangesAsync(System.Threading.CancellationToken) en UnitOfWork.cs
    GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommandHandler.Handle(GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommand, System.Threading.CancellationToken) en ActualizarFormulaCommandHandler.cs
    GalponERP.Application.Behaviors.AuditoriaBehavior<TRequest, TResponse>.Handle(TRequest, MediatR.RequestHandlerDelegate<TResponse>, System.Threading.CancellationToken) en AuditoriaBehavior.cs

Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException
  HResult=0x80131500
  Mensaje = The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded. See https://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions.
  Origen = Npgsql.EntityFrameworkCore.PostgreSQL
  Seguimiento de la pila:
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<ThrowAggregateUpdateConcurrencyExceptionAsync>d__10.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<Consume>d__7.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.<SaveChangesAsync>d__8.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__113.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__117.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.<ExecuteAsync>d__7`2.MoveNext()
   en Microsoft.EntityFrameworkCore.DbContext.<SaveChangesAsync>d__63.MoveNext()
   en GalponERP.Infrastructure.Persistence.UnitOfWork.<SaveChangesAsync>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Infrastructure\Persistence\UnitOfWork.cs: línea 16
   en GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommandHandler.<Handle>d__3.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Nutricion\Formulas\Commands\ActualizarFormula\ActualizarFormulaCommandHandler.cs: línea 52
   en GalponERP.Application.Behaviors.AuditoriaBehavior`2.<Handle>d__4.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Behaviors\AuditoriaBehavior.cs: línea 33
   en GalponERP.Application.Behaviors.ValidationBehavior`2.<Handle>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Behaviors\ValidationBehavior.cs: línea 24
   en GalponERP.Api.Controllers.FormulasController.<Actualizar>d__5.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Api\Controllers\FormulasController.cs: línea 52

  Esta excepción se generó originalmente en esta pila de llamadas:
    [Código externo]
    GalponERP.Infrastructure.Persistence.UnitOfWork.SaveChangesAsync(System.Threading.CancellationToken) en UnitOfWork.cs
    GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommandHandler.Handle(GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommand, System.Threading.CancellationToken) en ActualizarFormulaCommandHandler.cs
    GalponERP.Application.Behaviors.AuditoriaBehavior<TRequest, TResponse>.Handle(TRequest, MediatR.RequestHandlerDelegate<TResponse>, System.Threading.CancellationToken) en AuditoriaBehavior.cs
    GalponERP.Api.Controllers.FormulasController.Actualizar(System.Guid, GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommand) en FormulasController.cs

Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException
  HResult=0x80131500
  Mensaje = The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded. See https://go.microsoft.com/fwlink/?LinkId=527962 for information on understanding and handling optimistic concurrency exceptions.
  Origen = Npgsql.EntityFrameworkCore.PostgreSQL
  Seguimiento de la pila:
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<ThrowAggregateUpdateConcurrencyExceptionAsync>d__10.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Update.Internal.NpgsqlModificationCommandBatch.<Consume>d__7.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.<ExecuteAsync>d__50.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.<ExecuteAsync>d__9.MoveNext()
   en Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.<SaveChangesAsync>d__8.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__113.MoveNext()
   en Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.<SaveChangesAsync>d__117.MoveNext()
   en Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.<ExecuteAsync>d__7`2.MoveNext()
   en Microsoft.EntityFrameworkCore.DbContext.<SaveChangesAsync>d__63.MoveNext()
   en GalponERP.Infrastructure.Persistence.UnitOfWork.<SaveChangesAsync>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Infrastructure\Persistence\UnitOfWork.cs: línea 16
   en GalponERP.Application.Nutricion.Formulas.Commands.ActualizarFormula.ActualizarFormulaCommandHandler.<Handle>d__3.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Nutricion\Formulas\Commands\ActualizarFormula\ActualizarFormulaCommandHandler.cs: línea 52
   en GalponERP.Application.Behaviors.AuditoriaBehavior`2.<Handle>d__4.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Behaviors\AuditoriaBehavior.cs: línea 33
   en GalponERP.Application.Behaviors.ValidationBehavior`2.<Handle>d__2.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Application\Behaviors\ValidationBehavior.cs: línea 24
   en GalponERP.Api.Controllers.FormulasController.<Actualizar>d__5.MoveNext() en D:\scripts-csharp\Pollos_NicoLu\Pollos-NicoLu\GalponERP.Api\Controllers\FormulasController.cs: línea 52
   en Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.TaskOfIActionResultExecutor.<Execute>d__0.MoveNext()
   en System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult()
   en Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<<InvokeActionMethodAsync>g__Awaited|12_0>d.MoveNext()
   en Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<<InvokeNextActionFilterAsync>g__Awaited|10_0>d.MoveNext()

SOLUCCIONEMOS ESTE PROBLEMA AGREGANDO ESPECIFICACIONES AL USUARIO SOBRE QUE ES LO QUE ESTA MAL COLOCADO