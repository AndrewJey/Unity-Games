

<!-- local-development:start -->
# Desarrollo local

Puedes abrir este repositorio en VS Code, Antigravity o Sublime Text. El compilador y los servicios necesarios dependen del componente indicado abajo.

## Unity

Importa el proyecto desde Unity Hub usando `ProjectSettings/ProjectVersion.txt`. Conserva una copia antes de actualizar el editor; las soluciones generadas no son aplicaciones .NET independientes.

`m_EditorVersion: 5.0.0f4`

## Componentes y validación

Inventario y compilaciones locales del 2026-11-15. Una compilación correcta no acredita todavía interacción, compatibilidad de base de datos, instalador ni despliegue.

| Componente | Tipo / destino | Validación |
| --- | --- | --- |
| [MyfirstUnityProject/Assembly-CSharp-vs.csproj](<MyfirstUnityProject/Assembly-CSharp-vs.csproj>) | dotnet / v3.5 | Bloqueado: requiere el editor del juego |
| [MyfirstUnityProject/Assembly-CSharp.csproj](<MyfirstUnityProject/Assembly-CSharp.csproj>) | dotnet / v3.5 | Bloqueado: requiere el editor del juego |
| [MyfirstUnityProject/MyfirstUnityProject-csharp.sln](<MyfirstUnityProject/MyfirstUnityProject-csharp.sln>) | solution | Sin ejecutar |
| [MyfirstUnityProject/MyfirstUnityProject.sln](<MyfirstUnityProject/MyfirstUnityProject.sln>) | solution | Sin ejecutar |
| [MyfirstUnityProject/ProjectSettings/ProjectVersion.txt](<MyfirstUnityProject/ProjectSettings/ProjectVersion.txt>) | unity | Sin ejecutar |

## Archivos ignorados

`.gitignore` excluye credenciales locales, estado del editor y salidas de las herramientas detectadas. No deja de versionar archivos que ya estaban en Git. Los manifiestos, archivos de bloqueo, recursos fuente y scripts de esquema forman parte del código.
<!-- local-development:end -->
