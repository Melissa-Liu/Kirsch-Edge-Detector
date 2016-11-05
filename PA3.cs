using System;
// using static System.Console;
using System.IO;
using System.Linq;

namespace Bme121.Pa3
{
    static partial class Program
    {
        static void Main()
        {

            string inputFile = @"21_training.csv";
            string outputFile = @"21_training_edges.csv";
            int height;
            int width;
            FileStream inFile = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(inFile);

            // Read the input image from its csv file.

            //set height and width to the values given in the CSV file, parse to turn the string input into an int, and trim the comma
            width = int.Parse(sr.ReadLine().Trim(','));
            height = int.Parse(sr.ReadLine().Trim(','));
            Color[,] inImage = new Color[height, width];
            Color[,] outImage = new Color[height, width];
            //created a while loop to read until the end of the file 
            //created nested for loops to fill a string array with the values in CSV file, then parse the string values into ints while filling in a color array
            while (!sr.EndOfStream)
            {
                for (int i = 0; i < height; i++)
                {
                    string lineInput = sr.ReadLine();
                    string[] pixelvaluesinput = lineInput.Split(",".ToCharArray());
                        for (int j = 0; j < (4 * width); j = j + 4)
                        {
                            inImage[i, j / 4] = Color.FromArgb(int.Parse(pixelvaluesinput[j]), int.Parse(pixelvaluesinput[j + 1]), int.Parse(pixelvaluesinput[j + 2]), int.Parse(pixelvaluesinput[j + 3]));
                        }
                }
            }

            sr.Dispose();
            inFile.Dispose();

            // Generate the output image using Kirsch edge detection.
            //replacing the main body of the image by using the GetKirschEdgeValue method, then filling an output color array with the new values
            for (int r = 1; r < height - 1; r++)
            {
                for (int c = 1; c < width - 1; c++)
                {
                    outImage[r, c] = GetKirschEdgeValue(inImage[r - 1, c - 1], inImage[r - 1, c], inImage[r - 1, c + 1],
                                                      inImage[r, c - 1], inImage[r, c], inImage[r, c + 1],
                                                      inImage[r + 1, c - 1], inImage[r + 1, c], inImage[r + 1, c + 1]);
                }
            }

            //keeping the edges of the image the same
            for (int rowEdge = 0; rowEdge < height; rowEdge++)
            {
                outImage[rowEdge, 0] = inImage[rowEdge, 0];
                outImage[rowEdge, width - 1] = inImage[rowEdge, width - 1];
            }
            for (int colEdge = 0; colEdge < width; colEdge++)
            {
                outImage[0, colEdge] = inImage[0, colEdge];
                outImage[height - 1, colEdge] = inImage[height - 1, colEdge];
            }
            // Write the output image to its csv file.
            // TO DO:

            // created a file
            FileStream outCSVFile = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(outCSVFile);



            //adding the rows and columns as the first 2 rows
            sw.WriteLine(height);
            sw.WriteLine(width);

            //outputing the values from the output array

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j == width - 1)
                    {
                        sw.WriteLine(outImage[i, j].A + "," + outImage[i, j].R + "," + outImage[i, j].G + "," + outImage[i, j].B);
                    }
                    else
                    {
                        sw.Write(outImage[i, j].A + "," + outImage[i, j].R + "," + outImage[i, j].G + "," + outImage[i, j].B + ",");
                    }
                }
            }
            sw.Dispose(); //end stream
            outCSVFile.Dispose(); //end file  

        }

        // This method computes the Kirsch edge-detection value for pixel color
        // at the centre location given the centre-location pixel color and the
        // colors of its eight neighbours.  These are numbered as follows.
        // The resulting color has the same alpha as the centre pixel, 
        // and Kirsch edge-detection intensities which are computed separately
        // for each of the red, green, and blue components using its eight neighbours.
        // c1 c2 c3
        // c4    c5
        // c6 c7 c8
        static Color GetKirschEdgeValue(
            Color c1, Color c2, Color c3,
            Color c4, Color centre, Color c5,
            Color c6, Color c7, Color c8)
        {
            int newRed = GetKirschEdgeValue(c1.R, c2.R, c3.R, c4.R, c5.R, c6.R, c7.R, c8.R);
            int newGreen = GetKirschEdgeValue(c1.G, c2.G, c3.G, c4.G, c5.G, c6.G, c7.G, c8.G);
            int newBlue = GetKirschEdgeValue(c1.B, c2.B, c3.B, c4.B, c5.B, c6.B, c7.B, c8.B);
            
            Color newColor = Color.FromArgb(255, newRed, newGreen, newBlue);
            
            return newColor;

        }

        // This method computes the Kirsch edge-detection value for pixel intensity
        // at the centre location given the pixel intensities of the eight neighbours.
        // These are numbered as follows.
        // i1 i2 i3
        // i4    i5
        // i6 i7 i8
        static int GetKirschEdgeValue(
            int i1, int i2, int i3,
            int i4, int i5,
            int i6, int i7, int i8)
        {
            // TO DO: (Replace the following line.)
            
            int kirschSum1 = 5 * (i1 + i2 + i3) + (-3) * (i4 + i5 + i6 + i7 + i8);
            int kirschSum2 = 5 * (i5 + i2 + i3) + (-3) * (i1 + i4 + i6 + i7 + i8);
            int kirschSum3 = 5 * (i8 + i5 + i3) + (-3) * (i1 + i2 + i6 + i7 + i4);
            int kirschSum4 = 5 * (i7 + i5 + i8) + (-3) * (i1 + i2 + i3 + i4 + i6);
            int kirschSum5 = 5 * (i6 + i7 + i8) + (-3) * (i1 + i2 + i3 + i4 + i5);
            int kirschSum6 = 5 * (i4 + i6 + i7) + (-3) * (i1 + i2 + i3 + i5 + i8);
            int kirschSum7 = 5 * (i1 + i4 + i6) + (-3) * (i2 + i3 + i5 + i7 + i8);
            int kirschSum8 = 5 * (i1 + i2 + i4) + (-3) * (i3 + i5 + i6 + i7 + i8);
            
            int[] kirschSumAll = {kirschSum1, kirschSum2, kirschSum3, kirschSum4, kirschSum5, kirschSum6, kirschSum7, kirschSum8};
            
            int KirschMaxSum = kirschSumAll.Max();


            //restrict the sum to values between and including 0 and 255
            if (KirschMaxSum < 0)
            {
                KirschMaxSum = 0;
            }
            else if (KirschMaxSum > 255)
            {
                KirschMaxSum = 255;
            }

            return KirschMaxSum;
        }

        // Implementation of part of System.Drawing.Color.
        // This is needed because .Net Core doesn't seem to include the assembly 
        // containing System.Drawing.Color even though docs.microsoft.com claims 
        // it is part of the .Net Core API.
        struct Color
        {
            int alpha;
            int red;
            int green;
            int blue;

            public int A { get { return alpha; } }
            public int R { get { return red; } }
            public int G { get { return green; } }
            public int B { get { return blue; } }

            public static Color FromArgb(int alpha, int red, int green, int blue)
            {
                Color result = new Color();
                result.alpha = alpha;
                result.red = red;
                result.green = green;
                result.blue = blue;
                return result;
            }
        }
    }
}
