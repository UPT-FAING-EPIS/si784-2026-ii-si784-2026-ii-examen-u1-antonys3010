# Pasos donde debe intervenir el estudiante

El código ya queda preparado. Las siguientes acciones requieren tus cuentas personales o autorización y por eso no pueden quedar incorporadas como credenciales dentro del repositorio.

## 1. Crear y subir el repositorio de GitHub

Desde PowerShell, dentro de esta carpeta:

```powershell
git init
git add .
git commit -m "Implementar sistema de reserva de vuelos"
git branch -M main
git remote add origin https://github.com/TU_USUARIO/flight-reservation-system.git
git push -u origin main
```

Antes, crea en GitHub un repositorio público vacío llamado `flight-reservation-system`.

## 2. Preparar Azure

Necesitas una suscripción de Azure activa. Azure for Students sirve si tu cuenta es elegible.

Instala Azure CLI, inicia sesión y selecciona la suscripción:

```powershell
az login
az account show
```

Crea un Service Principal limitado a tu suscripción:

```powershell
$subscriptionId = az account show --query id -o tsv
az ad sp create-for-rbac --name "github-flight-reservation" --role Contributor --scopes "/subscriptions/$subscriptionId" --sdk-auth
```

Copia TODO el JSON que devuelve el comando. En GitHub crea el secret `AZURE_CREDENTIALS` y pega ese JSON.

> No subas ese JSON a archivos del repositorio.

Crea también `DB_ADMIN_PASSWORD` con una contraseña fuerte, por ejemplo una generada por un administrador de contraseñas.

## 3. Ejecutar Terraform

En GitHub:

`Actions > 1 - Provision Infrastructure > Run workflow`

Al finalizar, revisa el paso `Show deployment outputs`. Allí aparecerá `application_url` y los nombres de los recursos.

## 4. Configurar SonarQube Cloud

1. Entra a SonarQube Cloud con GitHub.
2. Importa `flight-reservation-system`.
3. Copia el Project Key y Organization Key.
4. Crea un token de análisis.
5. En GitHub Actions agrega:
   - Secret: `SONAR_TOKEN`
   - Variable: `SONAR_PROJECT_KEY`
   - Variable: `SONAR_ORGANIZATION`
6. Ejecuta `2 - SonarQube Cloud`.
7. Revisa que el Quality Gate quede aprobado y que Bugs, Vulnerabilities y Security Hotspots pendientes estén en 0 según lo pedido por el docente.

URL a entregar:

```text
https://sonarcloud.io/project/overview?id=TU_PROJECT_KEY
```

## 5. Configurar Snyk

1. Crea/inicia sesión en Snyk.
2. Obtén tu API token.
3. En GitHub agrega el secret `SNYK_TOKEN`.
4. Ejecuta `3 - Snyk and Semgrep Security`.
5. Descarga el artifact `security-reports` del workflow para conservar evidencia de:
   - `snyk-code.json`
   - `snyk-container.json`
   - `semgrep.json`

El workflow falla ante hallazgos High de Snyk o Error de Semgrep, de forma que no se pueda presentar como limpio si esos hallazgos siguen pendientes.

## 6. Desplegar la aplicación

Después de que `infra.yml` termine correctamente:

`Actions > 4 - Build Test and Deploy > Run workflow`

El workflow:

1. Ejecuta las pruebas .NET.
2. Se autentica en Azure.
3. Construye la imagen Docker.
4. La publica en Azure Container Registry.
5. Actualiza Azure Container Apps.
6. Imprime el FQDN público.

Abre:

```text
https://FQDN_QUE_MUESTRE_EL_WORKFLOW
```

Prueba búsqueda, reserva, modificación y cancelación.

## 7. Generar la documentación solicitada

Ejecuta:

`Actions > 5 - Generate Documentation > Run workflow`

Descarga el artifact `generated-documentation`.

Incluye:

- `data-dictionary.md`
- `er-diagram.md`
- `class-diagram.md`
- `component-diagram.md`
- `deployment-diagram.md`

GitHub renderiza los bloques Mermaid directamente en Markdown.

## 8. Evidencias finales

Antes de entregar toma capturas de:

- Aplicación pública funcionando.
- Swagger `/swagger`.
- GitHub Actions con los 5 workflows en verde.
- Sonar Quality Gate.
- Snyk/Semgrep y artifact de reportes.
- Terraform outputs.
- Documentación Mermaid renderizada.

Entrega finalmente estas tres URLs:

```text
Aplicación: https://...
Repositorio: https://github.com/TU_USUARIO/flight-reservation-system
Sonar: https://sonarcloud.io/project/overview?id=...
```

## 9. Evitar cargos después de la evaluación

Cuando el docente ya no necesite revisar el despliegue, puedes eliminar el Resource Group desde Azure Portal o con Azure CLI. Hazlo únicamente después de la revisión porque elimina también la aplicación y la base de datos.
