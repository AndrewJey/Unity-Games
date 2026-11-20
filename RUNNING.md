# Unity-Games: compilación recuperada y ejecución pendiente

Los dos proyectos C# son vistas generadas del **mismo proyecto Unity**, no dos juegos diferentes. Ambos compilan con MSBuild de Visual Studio 2022. Las pruebas puras pasan, pero el editor no pudo abrir el juego por falta de licencia activada.

## Compilar y comprobar

```powershell
Set-Location -LiteralPath 'D:\GH\AJ\Unity-Games'
.\build-local.ps1 -Test
```

Requisitos: Windows, Visual Studio 2022/MSBuild, NuGet y las referencias oficiales Unity 5. El paquete oficial Microsoft.NETFramework.ReferenceAssemblies.net35 1.0.3 aporta los ensamblados de referencia; no hace falta instalar la característica .NET 3.5 en Windows para esta compilación.

En una copia nueva sin la referencia del motor:

```powershell
.\build-local.ps1 -RestoreUnityReferences -Test
```

La restauración descarga aproximadamente 1.2 GB del archivo oficial de Unity y extrae únicamente UnityEngine.dll con 7-Zip. No ejecuta el instalador, no instala el editor ni activa licencias. Si 7-Zip no se detecta:

```powershell
.\Restore-UnityReferences.ps1 -SevenZipPath 'C:\Program Files\7-Zip\7z.exe'
.\build-local.ps1 -Test
```

Las huellas comprobadas están en `tools/unity5-reference-provenance.json`. Las DLL y el instalador se guardan fuera del repositorio en `D:\GH\_tools\unity-5.0.0f4`. También puedes usar una instalación auténtica de Unity **5.0.0f4**:

```powershell
.\build-local.ps1 -UnityManagedDirectory 'C:\Program Files\Unity\Editor\Data\Managed' -Test
```

Para abrir la solución en VS2022, `MyfirstUnityProject/MyfirstUnityProject.sln` y `MyfirstUnityProject/MyfirstUnityProject-csharp.sln` usan las referencias configuradas en `MyfirstUnityProject/Directory.Build.props`. Si las DLL están en otra ruta, usa `/p:UnityManagedDirectory=...` al compilar. Unity puede regenerar sus `.csproj`; las reparaciones de estos archivos corresponden a la copia registrada, no a futuras regeneraciones del editor.

Las pruebas `tests/ViewportChecks` se ejecutan por terminal con SDK .NET 10. El script detecta el portátil de `D:\GH\_tools\dotnet10`; en otro equipo indica `-DotNetPath` o instala el SDK. No abras ese proyecto net10 como si fuera un proyecto compatible con el SDK de VS2022.

Salidas independientes:

- `MyfirstUnityProject/Temp/bin/Assembly-CSharp/Debug/Assembly-CSharp.dll`
- `MyfirstUnityProject/Temp/bin/Assembly-CSharp-vs/Debug/Assembly-CSharp.dll`

Son ensamblados de scripts que necesitan el motor Unity; no son ejecutables independientes.

## Cambios realizados

- Se reparan los dos archivos generados Assembly-CSharp.csproj y Assembly-CSharp-vs.csproj del mismo proyecto Unity. Se restauran referencias .NET 3.5 desde Microsoft.NETFramework.ReferenceAssemblies.net35 1.0.3, sin instalar ni activar .NET 3.5 en Windows.
- Las rutas fijas C:/Program Files/Unity se sustituyen por UnityManagedDirectory configurable. Se usa UnityEngine.dll auténtico extraído del instalador oficial Unity 5.0.0f4, cuya firma Authenticode es válida. No se añaden DLL de Unity ni instaladores al repositorio.
- Se retiran las referencias UnityEditor/UnityEngine.UI que ningún script usa. Cada proyecto tiene sus propias carpetas de restauración/intermedios y salida para evitar colisiones entre los dos ensamblados del mismo nombre.
- AspectUtility recalcula al cambiar la resolución/proporción, conserva el campo serializado y API estática existentes, separa el estado de cámara por instancia, limpia/desactiva su cámara de fondo y restaura el rectángulo anterior al deshabilitarse.
- Las consultas de tamaño/ratón ya no requieren una cámara estática inicializada; guiMousePosition contempla Event.current nulo fuera de OnGUI. El cálculo puro AspectViewport evita divisiones inválidas y el truncamiento a dos decimales que estiraba imágenes de proporción parecida.
- La copia exportable de Librerias se actualiza junto con Assets/Scripts. Se conserva el GUID original de AspectUtility y se añade un GUID independiente al helper nuevo; no se reserializan escenas, prefabs ni ajustes binarios.
- Se añaden restauración reproducible de referencias con SHA-256, NuGet.Config público, build-local.ps1, 14 pruebas aisladas, README y guía actualizados; .gitignore amplía exclusiones de salidas/estado generado.

## Comprobaciones realizadas

- 2/2 proyectos C# compilan en Debug mediante MSBuild de Visual Studio 2022, usando las referencias auténticas de Unity 5 y .NET 3.5.
- El script distribuido build-local.ps1 -Test pasó (código 0), sin advertencias de importación duplicada ni errores de compilación.
- 14 pruebas aisladas pasaron: 4:3, 16:9, 16:10, pantalla vertical/cuadrada, proporción solicitada, regresión de truncamiento 1.334 y siete entradas inválidas. Se comprueban proporción conservada y bandas centradas. No se inicializa Unity para estas pruebas.
- Se comprobó la firma válida del instalador oficial de Unity Technologies y se registraron URL, tamaño y SHA-256 de instalador/DLL.
- Se intentó iniciar el editor 5.0.0f4 en batchmode/nographics sobre una copia de Assets/ProjectSettings en _control. Terminó con código 1 por falta de activación de licencia, antes de validar el juego; la escena original quedó intacta.

## Qué falta para ejecutar el juego

- Compilación recuperada no equivale a juego ejecutado: Play Mode, importación de escena/prefabs, renderizado real, audio, animaciones, controles y exportación de Player no quedaron comprobados. La prueba real del editor está bloqueada por licencia de Unity no activada.
- Se mantiene Unity 5.0.0f4 y el destino .NET 3.5 que corresponde a sus scripts. No se declara migración a Unity 6 ni soporte vigente de este motor histórico. Instalar un IDE moderno no reemplaza al editor Unity.
- Para ejecutar o migrar el juego hace falta un editor compatible con licencia válida. Debe probarse la importación/actualización en una copia; no se comprobó la disponibilidad actual del servicio de activación de Unity 5 ni se aceptaron acuerdos o activaron licencias.
- Las bibliotecas del motor se mantienen fuera de Git en D:/GH/_tools/unity-5.0.0f4; en otro equipo hay que restaurarlas desde la URL oficial o indicar una instalación auténtica de Unity 5 mediante UnityManagedDirectory. La primera descarga pesa aproximadamente 1.2 GB y requiere 7-Zip completo para extraer una DLL.
- Las pruebas puras requieren SDK .NET 10 por terminal (se usa el portátil local); VS2022 compila los dos proyectos Unity antiguos, pero no se usa para cargar el proyecto de pruebas net10.
- La carpeta Library histórica ya estaba versionada y se conserva. .gitignore impide añadir nuevas cachés; no borra los archivos ya presentes en el historial. Los plugins Google Play de Librerias son paquetes históricos externos a Assets y no se instalaron ni modernizaron.

## Comprobación manual pendiente dentro de Unity

Con un editor compatible y licencia válida, trabaja primero sobre una copia de Assets y ProjectSettings. Abre `Assets/Escenas/menu.unity`, comprueba que la importación y la consola no tienen errores y entra en Play Mode. Revisa cámaras y personajes/animaciones, ajusta Game View a 4:3, 16:9 y vertical, redimensiona y desactiva/reactiva AspectUtility; confirma bandas negras, relación de aspecto, ausencia de cámaras de fondo sobrantes y comportamiento del ratón. Después exporta un Windows Player de prueba. Esta lista es pendiente, no resultados realizados.

La reparación se publica en `codex/build-repairs-20261115`; `master` queda sin cambios nuevos hasta verificar el juego dentro del editor. El proyecto conserva su versión histórica y no se presenta como migrado a un motor moderno.

Referencias primarias: [Unity 5.0.0f4 y descarga oficial](https://unity.com/releases/editor/whats-new/5.0.0f4), [referencias Microsoft .NET 3.5](https://www.nuget.org/packages/Microsoft.NETFramework.ReferenceAssemblies.net35/1.0.3).

<!-- improvements-20261119:start -->
## Mejoras y verificación — 2026-11-19 (Proyección de puntero acotada e interpolación suave de viewport)

- Añadidos `ScreenToViewportClamped` (mapeo seguro de coordenadas de pantalla al área visible `[0..1]` en pillarbox/letterbox) y `AspectViewport.Lerp` para transiciones suaves en ambas copias de `AspectViewport.cs`.
- Ampliada la suite `tests/ViewportChecks/Program.cs` hasta 18 comprobaciones.
- Validación: `build-local.ps1 -Test` — ambos proyectos C# Unity compilados y 18/18 verificaciones aprobadas.
<!-- improvements-20261119:end -->
