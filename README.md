# [Nombre de tu Juego de RV]

**Asignatura:** Interfaces Inteligentes  
**Grupo:** 7  
**Integrantes:** [Nombre de los integrantes]

## 1. Descripción del Proyecto
Pingoso

## 2. Demo en Funcionamiento

![Gif Animado de Ejecución](AQUI_PEGA_EL_ENLACE_A_TU_GIF.gif)

---

## 3. Cuestiones Importantes para el Uso

Para ejecutar correctamente el prototipo, tener en cuenta:

* **Hardware Requerido:** Visor compatible con SteamVR/Oculus (Meta Quest 2/3, HTC Vive, etc.) y mandos con 6DOF.
* **Configuración Inicial:** Se recomienda iniciar la aplicación de escritorio (Oculus Link / SteamVR) antes de lanzar la build. El juego está diseñado para jugarse [de pie / sentado].
* **Controles:**
    * **Gatillo Derecho/Izquierdo:** Agarrar armas u objetos.
    * **Swing (Gesto físico):** Atacar con la espada (velocidad requerida para daño).
    * **[Botón X/A]:** [Acción extra si la hay, sino borrar].

---

## 4. Aspectos Destacados de la Aplicación

Los puntos fuertes del desarrollo técnico y de diseño son:

1.  **IA Enemiga Polimórfica:** Implementación de una arquitectura sólida mediante la clase abstracta `EnemyBase`, permitiendo comportamientos compartidos (visión, salud) y específicos (Embestida del esqueleto, Proyectiles del mago, Salto parabólico del Slime).
2.  **Gestión de Datos por ScriptableObjects:** Todo el balanceo del juego (vida, velocidad, daño, colores de variantes) se gestiona mediante activos de datos (`EnemyStats`), permitiendo iterar sin tocar código.
3.  **Animaciones Procedurales y Reactivas:** Uso de *Animation Events* y corrutinas para sincronizar lógica y visuales. Destaca el **Slime**, que combina navegación NavMesh con saltos físicos calculados en tiempo real para predecir la posición del jugador.
4.  **Feedback de Combate ("Game Feel"):** Implementación de *i-frames* (tiempo de invulnerabilidad) y feedback visual (parpadeo de color/shaders) al recibir daño, mejorando la legibilidad del combate en RV.

---

## 5. Hitos de Programación Logrados

El proyecto integra los siguientes conceptos técnicos de la asignatura:

* **Programación Orientada a Objetos (POO):** Uso extensivo de Herencia (`SkeletonEnemy : EnemyBase`), Polimorfismo (métodos `virtual` y `override` para `Move()` y `TakeDamage()`) y Encapsulamiento.
* **Interfaces:** Implementación de `IDamageable` para estandarizar el sistema de daño entre el jugador y los distintos enemigos.
* **Inteligencia Artificial (NavMesh + FSM):** Máquinas de estados finitos (Patrulla -> Persecución -> Ataque -> Huida) combinadas con el sistema de navegación de Unity.
* **Matemáticas Vectoriales:** Cálculos de `Vector3.Angle` para conos de visión (FOV), `Vector3.Distance` para rangos y `Lerp` para movimientos suaves y animaciones procedurales.
* **Corrutinas y Asincronía:** Gestión de tiempos de espera, cooldowns de ataque y efectos temporales mediante `IEnumerator`.

---

## 6. Sensores e Interfaces Multimodales

Se han utilizado los siguientes sensores y técnicas de interacción:

* **Tracking Posicional (6DOF):** Uso de la posición y rotación de HMD y mandos para interacción natural (esquivar físicamente, golpear con la mano).
* **Retroalimentación Háptica (Vibración):** Uso de los actuadores de los mandos para dar feedback táctil al golpear enemigos o recibir daño.
* **Audio Espacial (Básico):** Detección de la dirección del enemigo mediante fuentes de audio 3D.

---

## 7. Check-list de Recomendaciones de Diseño de RV

Evaluación de cumplimiento de buenas prácticas en RV:

| Recomendación de Diseño | Estado | Notas / Justificación |
| :--- | :---: | :--- |
| **Evitar aceleraciones bruscas de cámara** | ✅ Contempla | El jugador tiene control total y no hay movimientos de cámara forzados. |
| **Mantener 72/90 FPS estables** | ✅ Contempla | Uso de geometría *Low Poly* y optimización de *Draw Calls* (materiales compartidos). |
| **Distancia de interacción cómoda** | ✅ Contempla | Los enemigos mantienen una `StopDistance` para no atravesar al usuario ni invadir su espacio íntimo. |
| **Interfaces Diegéticas** | ✅ Contempla | La información (vida/daño) se muestra integrada en el mundo o mediante feedback visual, no en HUDs pegados a la cara. |
| **Legibilidad de Textos** | ➖ N/A | El juego se basa en lenguaje visual, minimizando el uso de texto. |
| **Prevención de Cinetosis** | ✅ Contempla | Se evita el movimiento suave lateral (strafe) sin referencias estáticas o viñeta. |

---

## 8. Acta de Acuerdos y Reparto de Tareas

**Integrantes:** [Tu Nombre] y [Nombre Compañero]

| Tarea Desarrollada | Responsable | Tipo |
| :--- | :--- | :---: |
| **Arquitectura de IA (`EnemyBase`, `IDamageable`)** | [Tu Nombre] | Individual |
| **IA Específica (Esqueleto, Mago, Slime)** | [Tu Nombre] | Individual |
| **Sistema de Datos (`ScriptableObjects`)** | [Tu Nombre] | Individual |
| **Controlador del Jugador y VR Rig** | [Nombre Compañero] | Individual |
| **Sistema de Combate Jugador (Espada)** | [Nombre Compañero] | Individual |
| **Diseño de Nivel y Escenario** | [Nombre Compañero] | Individual |
| **Integración de Animaciones y Animator** | [Tu Nombre] | Individual |
| **Gestión del Repositorio y Merges** | Ambos | Grupo |
| **Documentación y README** | [Tu Nombre] | Individual |