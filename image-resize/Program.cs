using System;
using System.Diagnostics;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace image_resize
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Uso: MiComando <carpetaOrigen> <carpetaDestino> <porcentaje de reduccion final>");
                return;
            }

            string carpetaOrigen = args[0];
            string carpetaDestino = args[1];
            string porcentaje = args[2];

            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            string carpetaDuplicados = Path.Combine(carpetaDestino, "Duplicados");

            if (!Directory.Exists(carpetaDuplicados))
            {
                Directory.CreateDirectory(carpetaDuplicados);
            }

            string[] archivos = Directory.GetFiles(carpetaOrigen, "*.jpg");
            int totalArchivos = archivos.Length;
            int procesados = 0;
            
            // Inicializar un cronómetro para medir el tiempo transcurrido
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (string archivo in archivos)
            {
                string nombreArchivo = Path.GetFileName(archivo);
                string nombreDestino = EliminarPrefijo(nombreArchivo, "Copia de ");
                string rutaDestino = Path.Combine(carpetaDestino, nombreDestino);
                string rutaDuplicados = Path.Combine(carpetaDuplicados, nombreDestino);



                if (File.Exists(rutaDestino))
                {
                    File.Move(archivo, rutaDuplicados);
                }
                else
                {
                    double porc = Double.Parse(porcentaje) / 100;
                    RedimensionarImagen(archivo, rutaDestino, porc);
                }

                procesados++;
                double porcentajeCompletado = (double)procesados / totalArchivos * 100;

                // Calcular el tiempo estimado de finalización
                double tiempoTranscurrido = stopwatch.Elapsed.TotalSeconds;
                double tiempoEstimadoTotal = tiempoTranscurrido / (porcentajeCompletado / 100);
                TimeSpan tiempoEstimado = TimeSpan.FromSeconds(tiempoEstimadoTotal);

                Console.WriteLine($"Progreso: {procesados} de {totalArchivos} archivos procesados.");
                Console.WriteLine($"Tiempo estimado de finalización: {tiempoEstimado.ToString(@"hh\:mm\:ss")}");

            }

            stopwatch.Stop();
        }

        static string EliminarPrefijo(string nombre, string prefijo)
        {
            if (nombre.StartsWith(prefijo))
            {
                return nombre.Substring(prefijo.Length);
            }
            return nombre;
        }

        static void RedimensionarImagen(string rutaOrigen, string rutaDestino, double porcentajeReduccion)
        {
            using (var imagen = Image.Load(rutaOrigen))
            {
                int nuevaAnchura = (int)(imagen.Width * porcentajeReduccion);
                int nuevaAltura = (int)(imagen.Height * porcentajeReduccion);

                imagen.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(nuevaAnchura, nuevaAltura),
                    Mode = ResizeMode.Max
                }));

                imagen.Save(rutaDestino, new JpegEncoder());
            }
        }

    }
}