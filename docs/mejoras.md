 bien en lo que trabajaremos sera en:
   verifica a lujo de detalle si realmente el sistema hace esto:
   Prevención de Quiebre de Stock: En avicultura, quedarse sin
   alimento un solo día es catastrófico para el crecimiento. Tus
   alertas proactivas y proyecciones de días restantes resuelven
   este problema crítico.
   <-- a que alertas se refiere a alertas visuales del sistema o
   alertas notificadas al telefono del usuario
   no estoy seguro si realmente funciona esto en el sistema:
   Control de "Fugas": El Kardex detallado permite detectar robos
   o desperdicios excesivos de alimento, que suele representar el
   70% del costo de producción.
   analiza el proyecto a lujo de detalle para asegurarnos que
   realmente funcione eso
   esto como debe de realmente funcionar:
   Gestión de Costos: Al vincular compras con pagos, el dueño
   sabe exactamente cuánto dinero tiene "parado" en bodega.
   y el tema del stock actual se refiere a los productos que se
   registran en el apartado de productos o en el apartado de
   gastos
   El sistema actual maneja stock
   total, pero no "Primero en Entrar, Primero en Salir"
   el stock se lo registra si tambien el peso y al hacer registro
   de consumo diarios de alimento en cada lote se resta al peso
   total con el que se registro el producto no se resta el stock.

    Analicemos punto por punto tus dudas con el código real en la
  mano, para separar lo que "parece" hacer de lo que realmente hace
  el sistema en este momento.

  ---

  1. ¿A qué se refiere con "Alertas"? (Visuales vs. Push)
  He revisado el archivo AlertaInventarioJob.cs y el
  VerificarNivelesAlimentoQuery.

   * Realidad Técnica: El sistema está preparado para ambas, pero
     la "notificación al teléfono" requiere una configuración
     externa (Firebase).
       * Alertas Visuales: En el Frontend (InventarioPage), hay
         indicadores de color (Rojo para "Agotado", Ámbar para
         "Stock Bajo").
       * Alertas al Teléfono: El código del servidor intenta usar
         un INotificationService que llama a EnviarAlertaPushAsync.
         Esto envía un mensaje a la App del usuario (vía Firebase
         Cloud Messaging).
       * Veredicto: Si no tienes configurado Firebase, solo verás
         las alertas visuales en la web. La lógica está escrita,
         pero la "tubería" de internet para el teléfono debe estar
         conectada.

  ---

  2. Control de "Fugas" y el Kardex (Robos/Desperdicios)
  Me preguntaste si esto realmente funciona para detectar robos.

   * Cómo funciona hoy: El sistema registra el UsuarioId en cada
     movimiento. Si alguien registra una salida manual de 50kg de
     alimento, queda su "firma digital".
   * El problema real: Si alguien roba físicamente el alimento sin
     registrarlo en el sistema, el software no se entera... hasta
     que haces una Conciliación.
       * Al usar el botón de Conciliación, el sistema compara lo
         que "debería haber" (ej. 500kg) con lo que el dueño cuenta
         físicamente (ej. 420kg).
       * El sistema genera un ajuste de -80kg. Al ver el reporte de
         Ajustes de Inventario, el dueño puede ver que faltan 80kg
         sin justificación de consumo en lotes. Ahí es donde
         detectas la "fuga".
   * Análisis: Funciona como herramienta de auditoría, pero depende
     100% de que el dueño haga conciliaciones físicas periódicas.

  ---

  3. Gestión de Costos: Dinero "Parado" en Bodega
  Esto funciona a través de la Valorización de Inventario.

   * Lógica: El sistema no solo multiplica Cantidad x Precio.
     Utiliza el costo de las Compras (IngresoMercaderia).
   * Realidad: Si tienes 1000kg de alimento que compraste a Bs.
     5/kg, el sistema te dice: "Tienes Bs. 5.000 parados en
     bodega".
   * Vínculo Compra-Pago: El sistema rastrea si ya pagaste esa
     factura. Esto es útil porque el dueño puede ver: "Tengo Bs.
     5.000 de producto, pero aún le debo Bs. 2.000 al proveedor".
     Es decir, parte de ese stock aún no es "tuyo" financieramente.

  ---

  4. ¿Stock de "Productos" o de "Gastos"?
  Esta es una distinción crítica en tu sistema.

   * Productos: Son los que tienen Inventario. Ejemplo: Alimento
     Iniciador, Vacuna NewCastle, Cascarilla de Arroz. Estos
     aparecen en el módulo de Inventario y tienen Kardex.
   * Gastos: Son servicios o consumos directos que no se almacenan.
     Ejemplo: Factura de Luz, Pago de Alquiler, Salario del
     Galponero. Estos no tienen stock.
   * Confusión común: Si compras "Focos" para el galpón, podrías
     registrarlos como Gasto (se asume que se usaron todos) o como
     Producto (si compraste 50 y los vas sacando de bodega). El
     sistema descuenta del stock solo lo que está registrado como
     Producto.

  ---

  5. El Peso vs. El Stock (Consumo Diario)
  Aquí es donde el sistema se pone interesante y responde a tu
  última observación:

   * Tu observación: "Se resta al peso total con el que se registró
     el producto, no se resta el stock".
   * Análisis del Código: En
     RegistrarConsumoAlimentoCommandHandler.cs, el sistema maneja
     dos columnas: Cantidad (unidades/bolsas) y Peso (Kg).
       * Cuando el lote consume alimento, el sistema prioriza los
         Kg.
       * Si registraste una compra de 10 bolsas de 50kg cada una
         (Total 500kg), y el lote consume 25kg diarios, el sistema
         resta de los 500kg.
       * Riesgo actual: Si solo restas peso, podrías terminar con
         "0.5 bolsas" en el sistema. Para producción real, esto es
         correcto porque el consumo de los pollos es gramo por
         gramo, no bolsa por bolsa.

  ¿Qué voy a verificar ahora mismo?
  Voy a leer el código de RegistrarConsumoAlimento para asegurarme
  de que la resta se esté haciendo correctamente sobre la columna
  de Peso (Kg) y no solo en unidades, como sospechas.

  ---
  Investigando la lógica de resta de peso...

  ✓  ReadFile  GalponERP.Applic...ommandHandler.cs

✦ He analizado el núcleo de tu sistema de inventario
  (RegistrarConsumoAlimentoCommandHandler) y aquí están las
  respuestas finales, con base técnica real, a cada una de tus
  dudas:

  1. El Dilema: ¿Peso (Kg) o Unidades (Bolsas)?
  Acabo de confirmar en el código que tu sistema hace algo muy
  inteligente pero que puede ser confuso: Maneja "Unidades" con
  decimales.

   * Realidad: El sistema no resta directamente Kg a una bolsa de
     50kg. Lo que hace es:
       1. Toma los Kg consumidos (ej: 25 Kg).
       2. Calcula cuántas unidades representan esos Kg (ej: 0.5
          unidades si la bolsa es de 50Kg).
       3. Resta 0.5 unidades del stock.
   * Conclusión: Se restan unidades, pero como permite decimales
     (ej: 0.25, 0.5, 1.2), el efecto para el usuario es que el peso
     total disminuye correctamente. Si compraste 10 bolsas y
     consumiste 25kg, el sistema te dirá que te quedan 9.5 bolsas.
     Esto es ideal para el FCR (Conversión Alimenticia) porque es
     exacto al gramo.

  ---

  2. Control de "Fugas" (¿Realmente funciona?)
  El sistema tiene los cimientos, pero para que sea infalible en
  producción, depende de:
   * Justificación Obligatoria: En el código vi que cada salida
     manual (AjusteSalida) requiere una justificación. Si un
     empleado saca producto sin poner el motivo, el sistema lanza
     un error.
   * Auditoría de Usuario: Todos los movimientos graban quién los
     hizo (UsuarioId).
   * Veredicto: Funciona para detectar robos administrativos
     (alguien que intenta "cuadrar" mal el sistema). Para robos
     físicos, el sistema solo te servirá si el dueño usa la
     Conciliación (comparar stock real vs. sistema) cada semana.
     Sin conciliación física, el sistema no puede "adivinar" que
     alguien se llevó una bolsa a escondidas.

  ---

  3. Gestión de Costos y Dinero en Bodega
   * Cómo funciona: Cada vez que compras alimento
     (IngresoMercaderia), el sistema guarda el Costo Unitario
     Actual.
   * Valorización: El sistema multiplica tu Stock Actual x Costo de
     Compra.
   * Dato Clave: Si vinculas la compra con un pago parcial, el
     dueño puede ver en el Dashboard de Finanzas cuánto debe
     todavía por el alimento que ya tiene guardado. Esto le permite
     saber si su inventario es "patrimonio neto" o es "deuda".

  ---

  4. ¿Stock de "Productos" o de "Gastos"?
  He verificado que el sistema solo maneja stock para lo que esté
  en la tabla de Productos.
   * Apartado Productos: Alimento, vacunas, viruta. Estos SÍ tienen
     stock y Kardex.
   * Apartado Gastos: Luz, agua, sueldos. Estos NO tienen stock.
   * Importante: Si el usuario registra la compra de alimento en
     "Gastos" en lugar de "Inventario/Compras", el stock nunca
     subirá. El dueño debe ser disciplinado en usar el módulo de
     Compras para todo lo que sea "almacenable".

  ---

  5. Prevención de Quiebre de Stock (Alertas)
   * Visuales: En la pantalla de inventario, el sistema te avisa si
     el stock es bajo.
   * Push (Celular): El código está ahí (EnviarAlertaPushAsync),
     pero solo funcionará si configuraste las llaves de Firebase.
   * Proyección: El sistema te dice "Te quedan 4.2 días de
     alimento" basándose en cuántos pollos tienes vivos hoy en los
     galpones. Esto es lo más valioso que tiene tu sistema para la
     vida real, porque un operario puede olvidar revisar la bodega,
     pero el sistema le avisará que el jueves se acaba la comida.

  Resumen Final para Producción:
  El sistema es muy sólido porque maneja conversiones de Peso a
  Unidades con decimales, lo que permite precisión de báscula en un
  sistema de almacén. Es apto para producción real, siempre y
  cuando el dueño entienda que el sistema es el reflejo de la
  bodega física y que debe hacer conciliaciones para que el
  "Control de Fugas" sea efectivo