using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace JA_PROJ
{
    unsafe class Program
    {
        public delegate void Proc(byte[] byteArray, byte[] byteArrayOriginal, int width, int height, int start, int end);

        [DllImport(@"C:\Users\kamso\source\repos\JA_PROJ\x64\Release\JAAsm.dll")]
        /*        static extern void MyProc1(byte* pixelData, byte* outputData, int width, int stride, int height);*/
        public static extern void FiltrAsm(byte[] byteArray, byte[] byteArrayOriginal, int width, int height, int start, int end);

        [DllImport(@"C:\Users\kamso\source\repos\JA_PROJ\x64\Release\C_Library.dll")]
        public static extern void FiltrCpp(byte[] byteArray, byte[] byteArrayOriginal, int width, int height, int start, int end);

        /*        unsafe static private void runInThreads(int threadsNum, int totalRows, int width, ref BitmapData bitmapData, ref BitmapData outputBitmapData)
                {
                    //konwertuj do tablicy typu byte
                    //przekazuj parametry inputImage, outputImage, width, height, start, end - potem height bedzie do usuniecia
                    int stride = bitmapData.Stride;
                    byte* pixelData = (byte*)bitmapData.Scan0;
                    byte* outputData = (byte*)outputBitmapData.Scan0;

                    int rowsPerTask = totalRows / threadsNum;
                    int remainingRows = totalRows % threadsNum;

                    List<Task> tasks = new List<Task>();

                    for (int i = 0; i < threadsNum; i++)
                    {
                        int start = i * rowsPerTask;
                        int end = start + rowsPerTask;

                        // Każdy wątek otrzymuje zakres start do end z dodatkowym wierszem sąsiednim na początku i końcu
                        int startWithNeighbor = start > 0 ? start - 1 : start;
                        int endWithNeighbor = end < totalRows - 1 ? end + 1 : end += remainingRows;

                        tasks.Add(Task.Run(() =>
                        {
                            // Przetwarzanie z dodatkowym zakresem na potrzeby sąsiednich pikseli
                            byte* threadInputData = pixelData + startWithNeighbor * stride;
                            byte* threadOutputData = outputData + start * stride;
                            MyProc2(threadInputData, threadOutputData, width, stride, endWithNeighbor - startWithNeighbor);
                            *//*                    MyProc1(threadInputData, threadOutputData, width, stride, endWithNeighbor - startWithNeighbor)*//*
                        }));
                    }

                    Task.WaitAll(tasks.ToArray());
                }*/
        static private byte[] ImageToByteArray(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        static private Bitmap ConstructBitmap(byte[] pixelData, int width, int height)
        {
            Bitmap modifiedBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Rectangle rect = new Rectangle(0, 0, modifiedBitmap.Width, modifiedBitmap.Height);
            BitmapData bmpData = modifiedBitmap.LockBits(rect, ImageLockMode.ReadWrite, modifiedBitmap.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(modifiedBitmap.PixelFormat) / 8;
            int stride = bmpData.Stride;
            byte[] flippedPixelData = new byte[pixelData.Length];

            for (int y = 0; y < height; y++)
            {
                Array.Copy(
                    pixelData,
                    y * stride,
                    flippedPixelData,
                    (height - 1 - y) * stride,
                    stride
                );
            }

            Marshal.Copy(flippedPixelData, 0, bmpData.Scan0, flippedPixelData.Length);
            modifiedBitmap.UnlockBits(bmpData);

            return modifiedBitmap;
        }

        unsafe static private Bitmap runInThreads(int threadsNum, int totalRows, int width, ref Bitmap bitmap, ref BitmapData outputBitmapData, Proc proc)
        {
            //konwertuj do tablicy typu byte
            //przekazuj parametry inputImage, outputImage, width, height, start, end - potem height bedzie do usuniecia
            byte[] image = ImageToByteArray(bitmap);
            byte[] pixelData = new byte[image.Length - 54]; //input array
            byte[] pixelDataOriginal = new byte[image.Length - 54]; //output array
            Array.Copy(image, 54, pixelDataOriginal, 0, pixelData.Length);

            int rowsPerThread = (totalRows - 2) / threadsNum;
            int remainingRows = (totalRows - 2) % threadsNum;

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < threadsNum; i++)
            {


                Parallel.For(0, threadsNum, threadIndex =>
                {
                    int startRow = threadIndex * rowsPerThread + 1 + (threadIndex == 0 ? 0 : (threadIndex - 1 < remainingRows ? 1 : 0));
                    int endRow = (threadIndex == threadsNum - 1) ? totalRows - 1 : (threadIndex + 1) * rowsPerThread + 1 + (threadIndex < remainingRows ? 1 : 0);
                    proc(pixelData, pixelDataOriginal, width, totalRows, startRow, endRow);
                });

            }

            Task.WaitAll(tasks.ToArray());
            return ConstructBitmap(pixelData, width, totalRows);
        }

        static unsafe void Main(string[] args)
        {
            int doContinue = 1;
            while (doContinue>0)
            {
                Bitmap bmp = new Bitmap("C:\\Users\\kamso\\source\\repos\\JA_PROJ\\sample1.bmp");

                Console.WriteLine(bmp.Height.ToString()+ " "+ bmp.Width.ToString());

                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData bitmapData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

                Bitmap bufforBmp = new Bitmap(bmp.Width, bmp.Height);
                BitmapData buffor = bufforBmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);

                Console.WriteLine("Podaj ilość wątków w których ma być przetwarzany obraz: ");
                int threadsNum = int.Parse(Console.ReadLine());

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                Bitmap resultBitmap_ASM = runInThreads(threadsNum, bmp.Height, bmp.Width, ref bmp, ref buffor, FiltrAsm);

                stopwatch.Stop();

                Console.WriteLine($"Czas wykonania w Asemblerze: {stopwatch.ElapsedMilliseconds} ms");

                stopwatch.Restart();

                Bitmap resultBitmap_C = runInThreads(threadsNum, bmp.Height, bmp.Width, ref bmp, ref buffor, FiltrAsm);

                stopwatch.Stop();

                Console.WriteLine($"Czas wykonania w C: {stopwatch.ElapsedMilliseconds} ms");

                bmp.UnlockBits(bitmapData);

                bufforBmp.UnlockBits(buffor);

                resultBitmap_ASM.Save("C:\\Users\\kamso\\source\\repos\\JA_PROJ\\result_ASM.bmp");

                resultBitmap_C.Save("C:\\Users\\kamso\\source\\repos\\JA_PROJ\\result_C.bmp");

                try
                {
                    Console.WriteLine("Czy chcesz powtorzyc?");
                    Console.WriteLine("1 - nie");
                    Console.WriteLine("2 - tak");
                    doContinue = int.Parse(Console.ReadLine())-1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
        }
    }
}
