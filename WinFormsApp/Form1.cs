using System.Diagnostics;
using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection.Emit;

namespace WinFormsApp3
{
    public partial class Form1 : Form
    {
        public delegate void Proc(IntPtr pixelData, IntPtr resultData, int width, int height, int stride);

        [DllImport("JAAsm.dll")]
        public static extern void FilterAsm(IntPtr pixelData, IntPtr resultData, int width, int height, int stride);

        [DllImport("C_Library.dll")]
        public static extern void FilterCpp(IntPtr pixelData, IntPtr resultData, int width, int height, int stride);

        Bitmap originalBitmap;
        int threadsNumber;
        Proc currentProc;
        long processingTime;
        public Form1()
        {
            InitializeComponent();
        }
        private void selectImageButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "BMP files (*.bmp)|*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string imagePath = openFileDialog.FileName;
                        originalBitmap = new Bitmap(imagePath);
                        
                        //new scale for the image
                        int newWidth = originalImagePictureBox.Width;
                        int newHeight = originalImagePictureBox.Height;
                        originalImagePictureBox.Image = resizeBitmap(originalBitmap, newWidth, newHeight);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Image loading error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

            }
        }

        private void radioButtonC_CheckedChanged(object sender, EventArgs e)
        {
            currentProc = FilterCpp;
        }

        private void radioButtonAsm_CheckedChanged(object sender, EventArgs e)
        {
            currentProc = FilterAsm;
        }

        private void threadsNumberTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                threadsNumber = int.Parse(threadsNumberTextBox.Text);

                if (threadsNumber < 1)
                    throw new Exception("number must be larger than 0");
            }
            catch (Exception exception)
            {
                threadsNumber = 0;
                throw exception;
            }
        }
        private void processImageButton_Click(object sender, EventArgs e)
        {
            int width = originalBitmap.Width;
            int height = originalBitmap.Height;

            Stopwatch stopwatch = new Stopwatch();

            Bitmap resultBitmap = runInThreads(threadsNumber, ref originalBitmap, ref stopwatch, currentProc);

            processingTimeLabel.Text = stopwatch.ElapsedMilliseconds.ToString() + " ms";

            int newWidth = processedImagePictureBox.Width;
            int newHeight = processedImagePictureBox.Height;
            processedImagePictureBox.Image = resizeBitmap(resultBitmap, newWidth, newHeight);

            String filename = currentProc == FilterAsm ? "result_ASM.bmp" : "result_C.bmp";
            string projectDir = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\");
            resultBitmap.Save(projectDir + filename);
        }

        unsafe static private Bitmap runInThreads(int threadsNum, ref Bitmap bitmap, ref Stopwatch stopwatch, Proc proc)
        {
            Bitmap inputBmp = convertTo8BitGrayscale(bitmap);
            Bitmap outputBmp = convertTo8BitGrayscale(bitmap);

            Rectangle rect = new Rectangle(0, 0, inputBmp.Width, inputBmp.Height);
            BitmapData inputBitmapData = inputBmp.LockBits(rect, ImageLockMode.ReadWrite, inputBmp.PixelFormat);
            BitmapData outputBitmapData = outputBmp.LockBits(rect, ImageLockMode.ReadWrite, outputBmp.PixelFormat);

            int rowsPerThread = inputBmp.Height / threadsNum;
            int remainingRows = inputBmp.Height % threadsNum;

            stopwatch.Start();

            Parallel.For(0, threadsNum, i =>
            {
                int startRow = i * rowsPerThread + Math.Min(i, remainingRows) - ((i!=0) ? 2 : 0);
                int rowCount = rowsPerThread + (i < remainingRows ? 1 : 0) + ((i != 0) ? 2 : 0);

                IntPtr inputPtr = inputBitmapData.Scan0 + startRow * inputBitmapData.Stride;
                IntPtr outputPtr = outputBitmapData.Scan0 + startRow * outputBitmapData.Stride;

                proc(inputPtr, outputPtr, inputBmp.Width, rowCount, inputBitmapData.Stride);
            });

            stopwatch.Stop();

            inputBmp.UnlockBits(inputBitmapData);
            outputBmp.UnlockBits(outputBitmapData);

            return outputBmp;
        }
        private static Bitmap resizeBitmap(Bitmap bitmap, int newWidth, int newHeight)
        {
            Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);

            using (Graphics graphics = Graphics.FromImage(resizedBitmap))
            {
                // drawing quality
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                graphics.DrawImage(bitmap, 0, 0, newWidth, newHeight);
            }

            return resizedBitmap;
        }

        private static Bitmap convertTo8BitGrayscale(Bitmap inputBmp)
        {
            if (inputBmp.PixelFormat != PixelFormat.Format24bppRgb)
            {
                throw new ArgumentException("Bitmap must be in 24-bit format (RGB/BGR)");
            }

            // new 8bpp indexed grayscale Bitmap
            Bitmap grayBmp = new Bitmap(inputBmp.Width, inputBmp.Height, PixelFormat.Format8bppIndexed);

            ColorPalette palette = grayBmp.Palette;
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i); // Grayscale from 0 (black) to 255 (white)
            }
            grayBmp.Palette = palette;

            BitmapData inputData = inputBmp.LockBits(new Rectangle(0, 0, inputBmp.Width, inputBmp.Height), ImageLockMode.ReadOnly, inputBmp.PixelFormat);
            BitmapData outputData = grayBmp.LockBits(new Rectangle(0, 0, inputBmp.Width, inputBmp.Height), ImageLockMode.WriteOnly, grayBmp.PixelFormat);

            // Pointers to the input and output bitmaps
            IntPtr inputPtr = inputData.Scan0;
            IntPtr outputPtr = outputData.Scan0;

            int inputStride = inputData.Stride;
            int outputStride = outputData.Stride;

            unsafe
            {
                byte* inputRow = (byte*)inputPtr;
                byte* outputRow = (byte*)outputPtr;

                for (int y = 0; y < inputBmp.Height; y++)
                {
                    for (int x = 0; x < inputBmp.Width; x++)
                    {
                        int blue = inputRow[x * 3 + 0];
                        int green = inputRow[x * 3 + 1];
                        int red = inputRow[x * 3 + 2];

                        byte gray = (byte)(0.3 * red + 0.59 * green + 0.11 * blue);

                        // Set the grayscale value in the new bitmap
                        outputRow[x] = gray;
                    }
                    
                    //next row
                    inputRow += inputStride;
                    outputRow += outputStride;
                }
            }

            inputBmp.UnlockBits(inputData);
            grayBmp.UnlockBits(outputData);

            return grayBmp;
        }
    }
}
