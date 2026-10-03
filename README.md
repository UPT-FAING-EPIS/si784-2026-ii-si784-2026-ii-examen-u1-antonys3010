# Sistema de Reserva de Vuelos

Proyecto académico full-stack preparado para cumplir la rúbrica solicitada.

## Stack

- Backend: ASP.NET Core 8 Web API
- Frontend: React + Vite
- Base de datos: PostgreSQL
- ORM: Entity Framework Core
- Contenedor: Docker multi-stage
- Nube: Microsoft Azure
- Infraestructura: Terraform
- CI/CD y seguridad: GitHub Actions, SonarQube Cloud, Snyk y Semgrep
- Documentación: Mermaid + diccionario de datos generado automáticamente

## Funcionalidades

- Buscar vuelos por origen, destino y fecha.
- Ver aerolínea, número de vuelo, horario y precio.
- Crear una reserva y seleccionar asiento.
- Ver reservas por usuario.
- Modificar asiento y datos del pasajero.
- Cancelar reservas.
- Validación en frontend y backend.
- Datos de ejemplo cargados automáticamente la primera vez.

## API

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/flights` | Lista/busca vuelos. Query: `origin`, `destination`, `date` |
| GET | `/flights/{id}` | Obtiene detalle de vuelo |
| POST | `/reservations` | Crea una reserva |
| GET | `/reservations/{userId}` | Lista reservas de un usuario |
| PUT | `/reservations/{id}` | Modifica una reserva |
| DELETE | `/reservations/{id}` | Cancela una reserva |
| GET | `/swagger` | Swagger de la API |

## Ejecución local recomendada

Con Docker Desktop iniciado:

```bash
docker compose up --build
```

Aplicación: `http://localhost:8080`

Swagger: `http://localhost:8080/swagger`

Para detener:

```bash
docker compose down
```

## Automatizaciones solicitadas

- `.github/workflows/infra.yml`: crea la infraestructura Azure con Terraform.
- `.github/workflows/sonar.yml`: compila, ejecuta pruebas/cobertura y analiza en SonarQube Cloud.
- `.github/workflows/snyk-semgrep.yml`: analiza dependencias/código e imagen Docker; publica los reportes como artifact.
- `.github/workflows/deploy.yml`: ejecuta pruebas, construye la imagen, la sube a ACR y actualiza Azure Container Apps.
- `.github/workflows/generase-documentation.yml`: genera diccionario de datos y diagramas Mermaid.

## Infraestructura creada

Terraform aprovisiona:

- Resource Group
- Azure Container Registry (Basic)
- Log Analytics Workspace
- Azure Container Apps Environment
- Azure Container App
- Azure Database for PostgreSQL Flexible Server
- Base de datos `flightsdb`

> Nota: Azure PostgreSQL puede generar costo. Elige una suscripción educativa/crédito académico si dispones de ella y elimina el Resource Group al terminar la evaluación.

## Secrets de GitHub requeridos

En `Settings > Secrets and variables > Actions > Secrets`:

- `AZURE_CREDENTIALS`: JSON del Service Principal de Azure.
- `DB_ADMIN_PASSWORD`: contraseña fuerte para PostgreSQL.
- `SONAR_TOKEN`: token generado por SonarQube Cloud.
- `SNYK_TOKEN`: token generado por Snyk.

En `Settings > Secrets and variables > Actions > Variables`:

- `SONAR_PROJECT_KEY`: clave exacta del proyecto SonarQube Cloud.
- `SONAR_ORGANIZATION`: clave exacta de la organización SonarQube Cloud.

Consulta `SETUP-ENTREGA.md` para el procedimiento completo.

## URLs de entrega

Completar después de ejecutar las automatizaciones:

- Aplicación publicada: `https://<fqdn-de-azure-container-apps>`
- Repositorio: `https://github.com/<usuario>/flight-reservation-system`
- Sonar: `https://sonarcloud.io/project/overview?id=<SONAR_PROJECT_KEY>`

## Estructura

```text
backend/                  ASP.NET Core API
backend.tests/            pruebas unitarias e integración
frontend/                 React SPA
infra/                    Terraform Azure
.github/workflows/        automatizaciones requeridas
scripts/                  generación de documentación
docs/generated/           Mermaid + diccionario de datos
Dockerfile                imagen única frontend + backend
docker-compose.yml        ejecución local con PostgreSQL
```
