# Unity-Games

Proyecto histórico `MyfirstUnityProject`, creado con Unity 5.0.0f4. Las dos soluciones/proyectos generados representan el mismo juego.

**Estado:** ambos componentes C# compilan en Visual Studio 2022/MSBuild y pasan 14 comprobaciones aisladas de relación de aspecto. La prueba del editor se detuvo por falta de licencia activada; el juego completo y su exportación todavía no están comprobados.

```powershell
.\build-local.ps1 -Test
# Si aún faltan las referencias oficiales del motor:
.\build-local.ps1 -RestoreUnityReferences -Test
```

Se repararon las referencias .NET/Unity, el ajuste de cámara al redimensionar, consultas sin cámara/OnGUI, limpieza de la cámara de fondo y la precisión del viewport. Se conservan escenas, prefabs, GUID de AspectUtility y su campo serializado `_wantedAspectRatio`.

Consulta [RUNNING.md](RUNNING.md) para requisitos, rutas, comandos, comprobaciones y límites. La recuperación de compilación mantiene Unity 5/.NET 3.5; no certifica migración a Unity 6 ni preparación para producción.
