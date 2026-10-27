# Digital Banking API (.NET 10)

Motor de servicios API RESTful desarrollado para una plataforma de banca digital, siguiendo los principios de **Clean Architecture**, **SOLID** y resguardando las restricciones de diseño exigidas.

---

## 🏛️ Arquitectura del Proyecto

El proyecto está estructurado en 4 capas desacopladas + 1 proyecto de pruebas:

* **`DigitalBanking.Domain`**: Entidades centrales (`Customer`, `Account`, `Transaction`), excepciones de negocio personalizadas y enumeraciones.
* **`DigitalBanking.Application`**: Interfaces de servicio, DTOs y el algoritmo de generación de número de cuenta (`AccountNumberGenerator`).
* **`DigitalBanking.Infrastructure`**: Implementación de `DbContext` (EF Core con SQLite) e implementación de la lógica de negocio (`BankingService`).
* **`DigitalBanking.Api`**: Controladores desacoplados de la persistencia y manejador global de excepciones (`GlobalExceptionHandler`).
* **`DigitalBanking.Tests`**: Pruebas unitarias en xUnit con base de datos en memoria para la lógica bancaria y el generador de cuentas.

---


* **.NET 10 SDK** (o versión superior instalada)
* **Visual Studio 2026** (o VS 2022 / VS Code)

---


### 1. Ejecutar la API
Desde la terminal en la raíz del proyecto o desde **Visual Studio**:

```bash
dotnet run --project src/DigitalBanking.Api
