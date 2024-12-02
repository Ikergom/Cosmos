using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System.Graphics.Fonts;
using System.Diagnostics;
using IL2CPU.API.Attribs;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.System.Audio.IO;
using Cosmos.System.Audio;
using Cosmos.HAL.Audio;


namespace CosmosKernel1
{
    public class Kernel : Sys.Kernel
    {
        [ManifestResourceStream(ResourceName = "cosmos1.music.wav")] public static byte[] music;
        Canvas canvas;

        static string ASC16Base64 = "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AAB4zMzM2MzGxsbMAAAAAAAA/sbGwMDAwMDAwAAAAAAAAAAA/mxsbGxsbGwAAAAAAAAA/sZgMBgwYMb+AAAAAAAAAAAAftjY2NjYcAAAAAAAAAAAZmZmZmZ8YGDAAAAAAAAAAHbcGBgYGBgYAAAAAAAAAH4YPGZmZjwYfgAAAAAAAAA4bMbG/sbGbDgAAAAAAAA4bMbGxmxsbGzuAAAAAAAAHjAYDD5mZmZmPAAAAAAAAAAAAH7b29t+AAAAAAAAAAAAAwZ+29vzfmDAAAAAAAAAHDBgYHxgYGAwHAAAAAAAAAB8xsbGxsbGxsYAAAAAAAAAAP4AAP4AAP4AAAAAAAAAAAAYGH4YGAAA/wAAAAAAAAAwGAwGDBgwAH4AAAAAAAAADBgwYDAYDAB+AAAAAAAADhsbGBgYGBgYGBgYGBgYGBgYGBgYGNjY2HAAAAAAAAAAABgYAH4AGBgAAAAAAAAAAAAAdtwAdtwAAAAAAAAAOGxsOAAAAAAAAAAAAAAAAAAAAAAAABgYAAAAAAAAAAAAAAAAAAAAGAAAAAAAAAAADwwMDAwM7GxsPBwAAAAAANhsbGxsbAAAAAAAAAAAAABw2DBgyPgAAAAAAAAAAAAAAAAAfHx8fHx8fAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
        static MemoryStream ASC16FontMS = new MemoryStream(Convert.FromBase64String(ASC16Base64));

        public void DrawACSIIString(Canvas canvas, Color color, string s, int x, int y, int scale)
        {
            string[] lines = s.Split('\n');
            for (int l = 0; l < lines.Length; l++)
            {
                for (int c = 0; c < lines[l].Length; c++)
                {
                    int offset = (Encoding.ASCII.GetBytes(lines[l][c].ToString())[0] & 0xFF) * 16;
                    ASC16FontMS.Seek(offset, SeekOrigin.Begin);
                    byte[] fontbuf = new byte[16];
                    ASC16FontMS.Read(fontbuf, 0, fontbuf.Length);

                    for (int i = 0; i < 16 * scale; i++)
                    {
                        for (int j = 0; j < 8 * scale; j++)
                        {
                            if ((fontbuf[i / scale] & (0x80 >> (j / scale))) != 0)
                            {
                                canvas.DrawPoint(color, (x + j) + (c * 8 * scale), y + i + (l * 16 * scale));
                            }
                        }
                    }
                }
            }
        }

        private readonly Sys.Graphics.Bitmap bitmap = new Sys.Graphics.Bitmap(10, 10,
                new byte[] { 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0,
                    255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 23, 59, 88, 255,
                    23, 59, 88, 255, 0, 255, 243, 255, 0, 255, 243, 255, 23, 59, 88, 255, 23, 59, 88, 255, 0, 255, 243, 255, 0,
                    255, 243, 255, 0, 255, 243, 255, 23, 59, 88, 255, 153, 57, 12, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255,
                    243, 255, 0, 255, 243, 255, 153, 57, 12, 255, 23, 59, 88, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243,
                    255, 0, 255, 243, 255, 0, 255, 243, 255, 72, 72, 72, 255, 72, 72, 72, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0,
                    255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 72, 72,
                    72, 255, 72, 72, 72, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    10, 66, 148, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255,
                    243, 255, 10, 66, 148, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 10, 66, 148, 255, 10, 66, 148, 255,
                    10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255,
                    243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148,
                    255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, }, ColorDepth.ColorDepth32);

        Sys.FileSystem.CosmosVFS fs = new Cosmos.System.FileSystem.CosmosVFS();

        
        protected override void BeforeRun()
        {
            var mixer = new AudioMixer();
            //var audioStream = MemoryAudioStream.FromWave(music);
            var audioStream = new MemoryAudioStream(new SampleFormat(AudioBitDepth.Bits16, 2, true), 48000, music);
            var driver = AC97.Initialize(bufferSize: 4096);
            mixer.Streams.Add(audioStream);

            var audioManager = new AudioManager()
            {
                Stream = mixer,
                Output = driver
            };
            audioManager.Enable();

            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            Sys.KeyboardManager.SetKeyLayout(new Sys.ScanMaps.ESStandardLayout());
            Console.WriteLine("Cosmos booted successfully. Let's go in Graphical Mode");

            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1920, 1080, ColorDepth.ColorDepth32));
            canvas.Clear(Color.Turquoise);
        }
        protected override void Run()
            {
            int conty = 0;
            int contx = 5;
            int xfijo = contx;
            
            DrawACSIIString(canvas, Color.Black, "====================", 540, conty += 300, 4);
            DrawACSIIString(canvas, Color.Black, "Iker Samu OS", 610, conty += 60, 5);
            DrawACSIIString(canvas, Color.Black, "====================", 540, conty += 80, 4);
            DrawACSIIString(canvas, Color.Black, "Presiona cualquier tecla para seguir", 450, conty += 100, 4);
            canvas.Display();
            Console.ReadKey(true);
            canvas.Clear(Color.Turquoise);

            int optMenuPrin = optUsuario();
            canvas.Clear(Color.Turquoise);

            while (true)
            {
                switch (optMenuPrin)
                {
                    case 1:
                        while (true)
                        {
                            string nomcomando = ""; // Inicializamos el comando

                            while (true)
                            {
                                canvas.Clear(Color.Turquoise);

                                Console.Clear();
                                DrawACSIIString(canvas, Color.Black, "Escribe el nombre del comando: " + nomcomando, 10, 10, 2);
                                canvas.Display();
                                canvas.Clear(Color.Turquoise);

                                var keyInfo = Console.ReadKey(intercept: true);

                                if (keyInfo.Key == ConsoleKey.Enter)
                                    break;

                                if (keyInfo.Key == ConsoleKey.Backspace && nomcomando.Length > 0)
                                {
                                    nomcomando = nomcomando.Substring(0, nomcomando.Length - 1);
                                }
                                else if (keyInfo.Key != ConsoleKey.Backspace)
                                {
                                    nomcomando += keyInfo.KeyChar;
                                }
                            }
                            canvas.Clear(Color.Turquoise);
                            switch (nomcomando.ToLower())
                            {
                                case "help":
                                    help(); 
                                    break;

                                case "exit":
                                    DrawACSIIString(canvas, Color.Black, "Saliendo del bucle", 10, conty += 16, 2);

                                    return;

                                case "mkdir":
                                
                                    var NomCarp = "";
                                    bool Carpt = true;

                                    while (Carpt)
                                    {
                                        DrawACSIIString(canvas, Color.Black, "Nombre de la nueva carpeta: " + NomCarp, 10, conty += 16, 2);
                                        canvas.Display();

                                        var carpt = Console.ReadKey(intercept: true);

                                        if (carpt.Key == ConsoleKey.Enter)
                                        {
                                            Carpt = false;
                                        }
                                        else if (carpt.Key == ConsoleKey.Backspace)
                                        {
                                            if (NomCarp.Length > 0)
                                            {
                                                NomCarp = NomCarp.Substring(0, NomCarp.Length - 1);
                                                contx = 10 + (NomCarp.Length * 8);
                                            }
                                        }
                                        else
                                        {
                                            NomCarp += carpt.KeyChar;
                                            contx = 10 + (NomCarp.Length * 8);
                                        }
                                        canvas.Clear(Color.Turquoise);
                                        conty = 10;
                                        canvas.Display();
                                    }

                                    var rutaCarpeta = @"0:\" + NomCarp;

                                    try
                                    {
                                        if (!Directory.Exists(rutaCarpeta))
                                        {
                                            Directory.CreateDirectory(rutaCarpeta);
                                            DrawACSIIString(canvas, Color.Green, "Carpeta: " + NomCarp + " se ha creado correctamente.", xfijo, conty += 16, 2);
                                        }
                                        else
                                        {
                                            DrawACSIIString(canvas, Color.Red, "La carpeta ya existe: " + NomCarp, xfijo, conty += 16, 2);
                                        }
                                        canvas.Display();
                                        Console.ReadKey(true);

                                    }
                                    catch (Exception ex)
                                    {
                                        DrawACSIIString(canvas, Color.Red, "La carpeta no se ha podido crear: " + ex.Message, xfijo, conty += 16, 2);
                                        DrawACSIIString(canvas, Color.Red, "Ruta: " + rutaCarpeta, xfijo, conty += 30, 2);

                                        canvas.Display();
                                        Console.ReadKey(true);

                                    }

                                    break;
                                    

                                default:
                                    DrawACSIIString(canvas, Color.Black, "Comando no reconocido.", 10, conty += 16, 2);

                                    
                                    break;
                            }
                        }
                        break;

                    case 2:
                        while (true)
                        {
                            DrawACSIIString(canvas, Color.Black, "Presiona cualquier tecla para seguir", 10, conty += 30, 2);
                            Console.Clear();
                            conty = 0;
                            canvas.Clear(Color.Turquoise);
                            int opcion = menu();
                            canvas.Clear(Color.Turquoise);


                            switch (opcion)
                            {
                                case 1:
                                    help();
                                    break;

                                case 2:

                                    DrawACSIIString(canvas, Color.Black, "Apagar el sistema", 10, conty += 16, 2);
                                    canvas.Display();
                                    Cosmos.System.Power.Shutdown();
                                    return;

                                case 3:
                                    DrawACSIIString(canvas, Color.Black, "Reiniciar el sistema", 10, conty += 16, 2);
                                    canvas.Display();
                                    Cosmos.System.Power.Reboot();
                                    break;

                                case 4:
                                    var available_space = fs.GetAvailableFreeSpace(@"0:\");
                                    DrawACSIIString(canvas, Color.Black, "Available Free Space: " + available_space, 10, conty += 16, 2);
                                    canvas.Display();
                                    Console.ReadKey(true);

                                    break;

                                case 5:
                                    var fs_type = fs.GetFileSystemType(@"0:\");
                                    DrawACSIIString(canvas, Color.Black, "File System Type: " + fs_type, 10, conty += 16, 2);
                                    canvas.Display();
                                    Console.ReadKey(true);
                                    break;

                                case 6:
                                    var cont = 0;
                                    string NomCarpeta = "";
                                    bool Carpeta = true;

                                    while (Carpeta)
                                    {
                                        DrawACSIIString(canvas, Color.Black, "Escribe el nombre de la carpeta: " + NomCarpeta, xfijo, conty += 16, 2);
                                        canvas.Display();

                                        var key = Console.ReadKey(intercept: true);

                                        if (key.Key == ConsoleKey.Enter)
                                        {
                                            Carpeta = false;
                                        }
                                        else if (key.Key == ConsoleKey.Backspace)
                                        {
                                            if (NomCarpeta.Length > 0)
                                            {
                                                NomCarpeta = NomCarpeta.Substring(0, NomCarpeta.Length - 1);
                                                contx = 10 + (NomCarpeta.Length * 8);
                                            }
                                        }
                                        else
                                        {
                                            NomCarpeta += key.KeyChar;
                                            contx = 10 + (NomCarpeta.Length * 8);
                                        }
                                        canvas.Clear(Color.Turquoise);
                                        conty = 10;
                                        canvas.Display();
                                    }



                                    var ruta = @"0:\" + NomCarpeta;
                                    string[] ficheros;

                                    try
                                    {
                                        ficheros = Directory.GetFiles(ruta);
                                        DrawACSIIString(canvas, Color.Green, "Has entrado: ", 10, conty += 30, 2);
                                        canvas.Display();
                                        Console.ReadKey(true);
                                    }
                                    catch (Exception ex)
                                    {
                                        DrawACSIIString(canvas, Color.Red, "Error al acceder a la carpeta: " + ex.Message, 10, conty += 30, 2);
                                        canvas.Display();
                                        Console.ReadKey(true);

                                        break;
                                    }

                                    foreach (var fichero in ficheros)
                                    {
                                        cont++;
                                        DrawACSIIString(canvas, Color.Black, cont + ") " + fichero, 10, conty += 30, 2);
                                        canvas.Display();

                                    }
                                    Console.ReadKey(true);
                                    break;

                                case 7:
                                    string nomFich = "";
                                    bool fitchero = true;

                                    while (fitchero)
                                    {
                                        DrawACSIIString(canvas, Color.Black, "Escribe el nombre del fichero: " + nomFich, xfijo, conty += 16, 2);
                                        canvas.Display();

                                        var fich = Console.ReadKey(intercept: true);

                                        if (fich.Key == ConsoleKey.Enter)
                                        {
                                            fitchero = false;
                                        }
                                        else if (fich.Key == ConsoleKey.Backspace)
                                        {
                                            if (nomFich.Length > 0)
                                            {
                                                nomFich = nomFich.Substring(0, nomFich.Length - 1);
                                                contx = 10 + (nomFich.Length * 8);
                                            }
                                        }
                                        else
                                        {
                                            nomFich += fich.KeyChar;
                                            contx = 10 + (nomFich.Length * 8);
                                        }
                                        canvas.Clear(Color.Turquoise);
                                        conty = 10;
                                        canvas.Display();
                                        var rutaFichero = @"0:\" + fich;

                                        try
                                        {
                                            var file_stream = File.Create(rutaFichero);

                                            DrawACSIIString(canvas, Color.Green, "El fichero: " + fich + " se ha creado correctamente", 10, conty += 16, 2);
                                            canvas.Display();
                                            Console.ReadKey(true);
                                        }
                                        catch (Exception e)
                                        {
                                            DrawACSIIString(canvas, Color.Black, "No se ha podido crear el fichero: " + fich, 10, conty += 16, 2);
                                            DrawACSIIString(canvas, Color.Black, "El error es: " + e.Message, 10, conty += 30, 2);
                                            canvas.Display();
                                            Console.ReadKey(true);

                                        }
                                    }
                                    break;

                                case 8:
                                    var NomCarp = "";
                                    bool Carpt = true;

                                    while (Carpt)
                                    {
                                        DrawACSIIString(canvas, Color.Black, "Nombre de la nueva carpeta: " + NomCarp, xfijo, conty += 16, 2);
                                        canvas.Display();

                                        var carpt = Console.ReadKey(intercept: true);

                                        if (carpt.Key == ConsoleKey.Enter)
                                        {
                                            Carpt = false;
                                        }
                                        else if (carpt.Key == ConsoleKey.Backspace)
                                        {
                                            if (NomCarp.Length > 0)
                                            {
                                                NomCarp = NomCarp.Substring(0, NomCarp.Length - 1);
                                                contx = 10 + (NomCarp.Length * 8);
                                            }
                                        }
                                        else
                                        {
                                            NomCarp += carpt.KeyChar;
                                            contx = 10 + (NomCarp.Length * 8);
                                        }
                                        canvas.Clear(Color.Turquoise);
                                        conty = 10;
                                        canvas.Display();
                                    }

                                    var rutaCarpeta = @"0:\" + NomCarp;

                                    try
                                    {
                                        if (!Directory.Exists(rutaCarpeta))
                                        {
                                            Directory.CreateDirectory(rutaCarpeta);
                                            DrawACSIIString(canvas, Color.Green, "Carpeta: " + NomCarp + " se ha creado correctamente.", xfijo, conty += 16, 2);
                                        }
                                        else
                                        {
                                            DrawACSIIString(canvas, Color.Red, "La carpeta ya existe: " + NomCarp, xfijo, conty += 16, 2);
                                        }
                                        canvas.Display();
                                        Console.ReadKey(true);

                                    }
                                    catch (Exception ex)
                                    {
                                        DrawACSIIString(canvas, Color.Red, "La carpeta no se ha podido crear: " + ex.Message, xfijo, conty += 16, 2);
                                        DrawACSIIString(canvas, Color.Red, "Ruta: " + rutaCarpeta, xfijo, conty += 30, 2);

                                        canvas.Display();
                                        Console.ReadKey(true);

                                    }

                                    break;

                                case 9:
                                    string rutaDirectorio = @"0:\";


                                    try
                                    {
                                        var directorios = Directory.GetDirectories(rutaDirectorio);
                                        var ficheross = Directory.GetFiles(rutaDirectorio);

                                        DrawACSIIString(canvas, Color.Pink, "Carpetas en: " + rutaDirectorio, 10, conty += 16, 2);
                                        canvas.Display();
                                        foreach (var directorio in directorios)
                                        {
                                            DrawACSIIString(canvas, Color.Black, "- " + Path.GetFileName(directorio), 10, conty += 30, 2);
                                            canvas.Display();


                                        }

                                        DrawACSIIString(canvas, Color.Pink, "Ficheros en: " + rutaDirectorio, 10, conty += 60, 2);
                                        canvas.Display();

                                        foreach (var fichero in ficheross)
                                        {
                                            DrawACSIIString(canvas, Color.Black, "- " + Path.GetFileName(fichero), 10, conty += 30, 2);
                                            canvas.Display();


                                        }
                                        Console.ReadKey(true);
                                    }
                                    catch (Exception ex)
                                    {
                                        DrawACSIIString(canvas, Color.Red, "Error al acceder a la ruta: " + ex.Message, 10, conty += 16, 2);
                                        canvas.Display();
                                        Console.ReadKey(true);

                                    }
                                    break;
                                default:
                                    DrawACSIIString(canvas, Color.Red, "Opción no válida. Inténtalo de nuevo.", 10, conty += 16, 2);
                                    canvas.Display();
                                    Console.ReadKey(true);

                                    break;
                            }
                            ;
                            }
                        break;
                    case 3:
                        int opcionCalc = calculo();

                        DrawACSIIString(canvas, Color.Black, "Escribe el primer numero : " , xfijo, conty += 16, 2);
                        canvas.Display();
                        var input1 = Console.ReadLine();
                        int num1 = int.Parse(input1);
                        canvas.Clear(Color.Turquoise);

                        DrawACSIIString(canvas, Color.Black, "Escribe el primer numero : ", xfijo, conty += 16, 2);
                        canvas.Display();
                        var input2 = Console.ReadLine();
                        int num2 = int.Parse(input2);
                        canvas.Clear(Color.Turquoise);

                        switch (opcionCalc)
                        {
                            case 1:
                                
                                int numSum = num1 + num2;
                                DrawACSIIString(canvas, Color.Black, "El resultado de la suma es:" + numSum, 10, conty = 16, 2);
                                canvas.Display();
                                Console.ReadKey(true);

                                return;

                            case 2:
                                int numRest = num1 - num2;
                                DrawACSIIString(canvas, Color.Black, "El resultado de la resta es:" + numRest, 10, conty = 16, 2);
                                canvas.Display();
                                Console.ReadKey(true);

                                return;

                            case 3:
                                int numMult = num1 * num2;
                                DrawACSIIString(canvas, Color.Black, "El resultado de la multiplicacion es:" + numMult, 10, conty = 16, 2);
                                canvas.Display();
                                Console.ReadKey(true);

                                return;
                            case 4:
                                int numDivi = num1 / num2;
                                DrawACSIIString(canvas, Color.Black, "El resultado de la division es:" + numDivi, 10, conty = 16, 2);
                                canvas.Display();
                                Console.ReadKey(true);

                                return;
                            default :
                                break;
                        }

                        break;
                    default: break;
                }

            }

                
          }
        private int menu()
            {
            int conty = 0;
            DrawACSIIString(canvas, Color.Black, "======================================", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "        Menu Principal !!", 50, conty += 30, 3);
            DrawACSIIString(canvas, Color.Black, "======================================", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "1) Help ", 10, conty += 50, 2);
            DrawACSIIString(canvas, Color.Black, "2) Apagar", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "3) Reiniciar", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "4) Espacio libre", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "5) Tipo de sitem archivo", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "6) Mirar si existe carpeta", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "7) Lista de ficheros", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "8) Crear Carpeta", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "9) Mirar ficheros y carpetas creados", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "Por favor, selecciona una opción entre 1 y 6", 10, conty += 50, 2);
            canvas.Display();

            string input  = Console.ReadLine();
                int num;

            if (int.TryParse(input, out num))
            {
                if (num >= 1 && num <= 10)
                {
                    return num; // Devuelve la opción válida
                }
                else
                {
                    DrawACSIIString(canvas, Color.Black, "Por favor, selecciona una opción entre 1 y 9", 10, conty += 16, 2);
                    canvas.Display();

                    return 0; // Devuelve 0 si la opción no está en el rango
                }
            }
            else {
                DrawACSIIString(canvas, Color.Black, "Entrada no valida", 10, conty += 16, 2);
                canvas.Display();

                return 0;
            }
            }   

        private int calculo()
        {
            int conty = 0;
            DrawACSIIString(canvas, Color.Black, "======================================", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "        Menu Principal !!", 30, conty += 30, 3);
            DrawACSIIString(canvas, Color.Black, "======================================", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "1) Suma ", 10, conty += 50, 2);
            DrawACSIIString(canvas, Color.Black, "2) Resta", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "3) Multiplicacion", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "4) Division", 10, conty += 30, 2);
            canvas.Display();

            string input = Console.ReadLine();
            int num;


            if (int.TryParse(input, out num))
            {
                if (num >= 1 && num <= 4)
                {
                    return num; // Devuelve la opción válida
                }
                else
                {
                    DrawACSIIString(canvas, Color.Black, "Por favor, selecciona una opción entre 1 y 4", 10, conty += 16, 2);
                    canvas.Display();

                    return 0; // Devuelve 0 si la opción no está en el rango
                }
            }
            else
            {
                DrawACSIIString(canvas, Color.Black, "Entrada no valida", 10, conty += 16, 2);
                canvas.Display();

                return 0;
            }


        }
    
        private int optUsuario()
        {
            int conty = 0;
            DrawACSIIString(canvas, Color.Black, "======================================", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "        Menu Principal !!", 50, conty += 30, 3);
            DrawACSIIString(canvas, Color.Black, "======================================", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "1) Escribir comando ", 10, conty += 50, 2);
            DrawACSIIString(canvas, Color.Black, "2) Opciones predeterminadas", 10, conty += 30, 2);
            DrawACSIIString(canvas, Color.Black, "3) Calculos", 10, conty += 30, 2);
            canvas.Display();

            string input = Console.ReadLine();
            int num;


            if (int.TryParse(input, out num))
            {
                if (num >= 1 && num <= 3)
                {
                    return num; // Devuelve la opción válida
                }
                else
                {
                    DrawACSIIString(canvas, Color.Black, "Por favor, selecciona una opción entre 1 y 6", 10, conty += 16, 2);
                    canvas.Display();

                    return 0; // Devuelve 0 si la opción no está en el rango
                }
            }
            else
            {
                DrawACSIIString(canvas, Color.Black, "Entrada no valida", 10, conty += 16, 2);
                canvas.Display();

                return 0;
            }
            

        }
        private void help()
            {
            Console.Clear();
            int conty = 0;
                
                DrawACSIIString(canvas, Color.Black, "\ncp <ruta-nombre-fichero-copiar> <ruta-nombre-destino>: Copia el archivo a otra ruta", 10, conty , 2);
                DrawACSIIString(canvas, Color.Black, "\ncat  <ruta-nombre-archivo>:  Muestra el contenido del archivo", 10, conty += 30, 2);
                DrawACSIIString(canvas, Color.Black, "\ntouch  <nombre-del-archivo>:  Crea un nfichero nuevo", 10, conty += 30, 2);
                DrawACSIIString(canvas, Color.Black, "\ntouch  <nombre-fichero> : Actualiza la fecha de edicion", 10, conty += 30, 2);
                DrawACSIIString(canvas, Color.Black, "\nrm  <nombre-del-archivo>:  Elimina el fichero", 10, conty += 30, 2);
                DrawACSIIString(canvas, Color.Black, "\ncd  <ruta-nombre-carpeta-> : Vas a la carpeta", 10, conty += 30, 2);
                DrawACSIIString(canvas, Color.Black, "\nnano <ruta-nombre-fichero> : Para entrar y modificar el fichero", 10, conty += 30, 2);
                DrawACSIIString(canvas, Color.Black, "\nls <ruta-nombre-carpeta> : Te muestra el contenido de esa carpeta", 10, conty += 30, 2);
                DrawACSIIString(canvas, Color.Black, "\nls -l <ruta-nombre-carpeta> : Lo mismo que ls pero con detalles (Permisos,Espacio)", 10, conty += 30, 2);
                DrawACSIIString(canvas, Color.Black, "\nls -a <ruta-nombre-carpeta> : Lo mismo que ls pero tambien te mustra los que estan ocultos", 10, conty += 30, 2);
                DrawACSIIString(canvas, Color.Black, "\nmkdir <nombre-carpeta> : Crea una carpeta", 10, conty += 30, 2);
                DrawACSIIString(canvas, Color.Black, "\nrmdir <nombre-carpeta> : Elimina la carpeta", 10, conty += 30, 2);
                DrawACSIIString(canvas, Color.Black, "Presiona cualquier tecla para seguir", 10, conty += 100, 2);
                canvas.Display();
                Console.ReadKey(true);


        }
    }

    }
