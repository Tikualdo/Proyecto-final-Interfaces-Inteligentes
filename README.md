# Armando Adventure

**Asignatura:** Interfaces Inteligentes  
**Grupo:** 7  
## Integrantes
| Nombre Completo | Alu | Rol Principal |
| :--- | :--- | :--- |
| **Paulo Padilla Domingues** | 0101571836 | Enemigos |
| **Salvador González Cueto** | 0101424750 | Jugador |
| **Manuel José Sebastián Noda** | 0101499918 | Escenario |

## 1. Descripción del Proyecto
Prototipo de experiencia inmersiva en Realidad Virtual donde el jugador encarna a un hechicero. La mecánica principal se basa en una interfaz multimodal por voz: el usuario debe pronunciar comandos verbales específicos para conjurar hechizos y combatir enemigos. El objetivo es limpiar diferentes zonas de criaturas hostiles (Slimes, Esqueletos, Magos) para progresar y desbloquear un grimorio de habilidades más complejo.

## 2. Demo en Funcionamiento

![Gif Animado de Ejecución](AQUI_PEGA_EL_ENLACE_A_TU_GIF.gif)

---

## 3. Cuestiones Importantes para el Uso

* **Hardware Requerido:** Visor VR compatible, mandos 6DOF y **Micrófono funcional** (integrado en el visor o externo).
* **Configuración Inicial:** Asegurarse de que el micrófono predeterminado en Windows/SteamVR sea el del casco. Se recomienda jugar en un entorno con ruido moderado/bajo para mejorar la detección.
* **Controles:**
    * **Comandos de Voz:** Mecánica principal de ataque (Pronunciar hechizos como "Fireball", "Black hole", etc.).
    * **Botón Y / B**: Abrir inventario.
    * **Botón A**: Saltar.
    * **Gatillo Trigger (índice):** 
    * **Grip (botón del mango):** Agarrar objetos. 
    * **Tracking de Manos:** Apuntar la dirección del hechizo.

---

## 4. Aspectos Destacados de la Aplicación

Los puntos fuertes del desarrollo técnico y de diseño son:

1.  **IA Enemiga Polimórfica:** Implementación de una arquitectura sólida mediante la clase abstracta `EnemyBase`, permitiendo comportamientos compartidos (visión, salud) y específicos (Embestida del esqueleto, Proyectiles del mago, Salto parabólico del Slime).
2.  **Gestión de Datos por ScriptableObjects:** Todo el balanceo del juego (vida, velocidad, daño, colores de variantes) se gestiona mediante activos de datos, permitiendo iterar sin tocar código.
3.  **Animaciones Procedurales y Reactivas:** Uso de *Animation Events* y corrutinas para sincronizar lógica y visuales. Destaca el **Slime**, que combina navegación NavMesh con saltos físicos calculados en tiempo real para predecir la posición del jugador.
4.  **Feedback de Combate:** Implementación de *i-frames* (tiempo de invulnerabilidad) y feedback visual (parpadeo de color/shaders) al recibir daño, mejorando la legibilidad del combate en RV.

---

## 5. Hitos de Programación Logrados

El proyecto integra los siguientes conceptos técnicos de la asignatura:

* **Programación Orientada a Objetos (POO):** Uso extensivo de Herencia, Polimorfismo (métodos `virtual` y `override` para `Move()` y `TakeDamage()`) y Encapsulamiento.
* **Interfaces:** Implementación de `IDamageable` para estandarizar el sistema de daño entre el jugador y los distintos enemigos.
* **Inteligencia Artificial (NavMesh + FSM):** Máquinas de estados finitos (Patrulla -> Persecución -> Ataque -> Huida) combinadas con el sistema de navegación de Unity.
* **Matemáticas Vectoriales:** Cálculos de `Vector3.Angle` para conos de visión (FOV), `Vector3.Distance` para rangos y `Lerp` para movimientos suaves y animaciones procedurales.
* **Corrutinas y Asincronía:** Gestión de tiempos de espera, cooldowns de ataque y efectos temporales mediante `IEnumerator`.

---

## 6. Sensores e Interfaces Multimodales

Este proyecto pone especial énfasis en la interacción multimodal:

* **Reconocimiento de Voz (Speech-to-Text):** Implementación de una interfaz natural (NUI) que procesa la entrada de audio del micrófono para detectar palabras clave y ejecutar acciones (Spellcasting), eliminando la abstracción de botones para el combate.
* **Tracking Posicional (6DOF):** Uso de la posición de la mano y la orientación de la cabeza para apuntar los proyectiles invocados por la voz.
* **Retroalimentación Háptica:** Vibración en los mandos al lanzar un hechizo con éxito o recibir daño.

---

## 7. Check-list de Recomendaciones de Diseño de RV

Evaluación de cumplimiento de buenas prácticas en RV:

| Recomendación de Diseño | Estado | Notas / Justificación |
| :--- | :---: | :--- |
| **Evitar aceleraciones bruscas de cámara** | ✅ Contempla | El jugador tiene control total y no hay movimientos de cámara forzados. |
| **Mantener 72/90 FPS estables** | ✅ Contempla | Uso de geometría *Low Poly* y optimización de *Draw Calls* (materiales compartidos). |
| **Distancia de interacción cómoda** | ✅ Contempla | Los enemigos mantienen una `StopDistance` para no atravesar al usuario ni invadir su espacio íntimo. |
| **Interfaces Diegéticas** | ✅ Contempla | La información (vida/daño) se muestra integrada en el mundo o mediante feedback visual, no en HUDs pegados a la cara. |
| **Legibilidad de Textos** |  ✅ Contempla | El juego se basa en lenguaje visual, minimizando el uso de texto, pero puede leer claramente el nombre de las habilidades de als que dispone y encuentra. |
| **Prevención de Cinetosis** | ✅ Contempla | Se evita el movimiento suave lateral (strafe) sin referencias estáticas o viñeta. |

---

## 8. Diseño De Mapa

El mapa se creo con unas dimensiones de 500x500, pensado como un valle rodea do momtañas( estas actuan como limites del mapa para el jugador).
Se usaron una serie de asset de la Asset Store de Unity para darle vida al mundo:
- Fantasy landscape (creacion de los pueblos, coliseo y decoracion).
- Handpainted Forest Pack v1.0 ( para ambientación forestal).
- Stylized Tree & Grass Samples ( para el bosque principal del mapa).
- Mid poly Axes Collection, 3D Low-Poly Shields, Long Sword ( para decorar y ambientar el coliseo).
  
Así como el uso de las herramientas unity (por ejemplo el pincel del terreno) para poder modelar el mapeado.
Recalcar que se nesecitaron ajustar varios materiales de los asset para que fueran compatible con URP, ademas de quitar, ajustar y añadir algunas mallas y colliders para el correcto fucnionamiento de las fisicas del entorno con el PJ y los NPC enemigos.


## 9. Acta de Acuerdos y Reparto de Tareas

**Integrantes:** Paulo Padilla Domingues, Salvador González Cueto y Manuel José Sebastián Noda

| Tarea Desarrollada | Responsable | Tipo |
| :--- | :--- | :---: |
| **Arquitectura de IA (`EnemyBase`, `IDamageable`)** | Paulo | Individual |
| **IA Específica (Esqueleto, Mago, Slime)** | Paulo | Individual |
| **Sistema de Datos (`ScriptableObjects`)** | Paulo y Salvador | Grupal |
| **Controlador del Jugador y VR Rig** | Salvador | Individual |
| **Sistema de Combate Jugador (Espada)** | Salvador | Individual |
| **Diseño de Nivel y Escenario** | Manuel | Individual |
| **Integración de Animaciones y Animator** | Todos | Grupal |
| **Gestión del Repositorio y Merges** | Todos | Grupal |
| **Documentación y README** | Todos | Grupal |
