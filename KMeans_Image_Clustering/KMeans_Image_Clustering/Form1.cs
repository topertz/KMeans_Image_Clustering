using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace KMeans_Image_Clustering
{
    public partial class Form1 : Form
    {
        private Bitmap? originalImage;
        private Bitmap? clusteredImage;
        private int K = 3; // Number of clusters
        private Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
            buttonLoad.Click += btnLoadImage_Click;
            buttonStart.Click += btnStart_Click_Click;
            numericUpDownK.Value = K; // Set initial K value
        }

        private void btnLoadImage_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
                openFileDialog.Title = "Choose image file";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        originalImage = new Bitmap(openFileDialog.FileName);
                        MessageBox.Show("Image loaded!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error while loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnStart_Click_Click(object? sender, EventArgs e)
        {
            // Set K dynamically from NumericUpDown control
            K = (int)numericUpDownK.Value;

            // Check if the image is loaded
            if (originalImage == null)
            {
                MessageBox.Show("Please load an image first.");
                return;
            }

            // Path for saving the clustered image
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string savePath = Path.Combine(basePath, "clustered_image.png");
            KMeansClustering(K);
            SaveClusteredImage(savePath);
            DisplayImages();
        }

        private List<int> InitializeCentroidsFromHistogram(int k, byte[,] grayImage)
        {
            List<int> centroids = new List<int>();

            // Create the histogram of pixel intensities
            int[] histogram = CreateHistogram(grayImage, grayImage.GetLength(0), grayImage.GetLength(1));

            // Get the intensity values with the highest frequency
            List<int> intensityValues = new List<int>();
            for (int i = 0; i < histogram.Length; i++)
            {
                for (int j = 0; j < histogram[i]; j++)
                {
                    intensityValues.Add(i);
                }
            }

            // Randomly pick k values from the intensity values to initialize the centroids
            while (centroids.Count < k)
            {
                int index = rand.Next(0, intensityValues.Count);
                int selectedIntensity = intensityValues[index];

                // Ensure centroids are unique
                if (!centroids.Contains(selectedIntensity))
                {
                    centroids.Add(selectedIntensity);
                }
            }

            return centroids;
        }

        private void KMeansClustering(int k)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Error: No image loaded for clustering.");
                return;
            }

            int width = originalImage.Width;
            int height = originalImage.Height;

            // Convert image to grayscale
            byte[,] grayImage = ConvertToGrayscale(originalImage);

            // Initialize centroids from the histogram (using intensity distribution)
            List<int> centroids = InitializeCentroidsFromHistogram(k, grayImage);

            // Assign pixels to clusters based on the centroids
            int[,] clusterAssignments = new int[width, height];

            bool changed;
            do
            {
                changed = AssignPixelsToClusters(grayImage, width, height, centroids, clusterAssignments);
                UpdateCentroids(centroids, clusterAssignments, grayImage, width, height);
            } while (changed);

            // Color the image based on clusters
            clusteredImage = ColorClusters(clusterAssignments, width, height);
        }

        private byte[,] ConvertToGrayscale(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;
            byte[,] grayImage = new byte[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    byte gray = (byte)((pixelColor.R + pixelColor.G + pixelColor.B) / 3);
                    grayImage[x, y] = gray;
                }
            }

            return grayImage;
        }

        private int[] CreateHistogram(byte[,] grayImage, int width, int height)
        {
            int[] histogram = new int[256];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    byte intensity = grayImage[x, y];
                    histogram[intensity]++;
                }
            }

            return histogram;
        }

        private List<int> InitializeRandomCentroids(int k, byte[,] grayImage)
        {
            List<int> centroids = new List<int>();
            HashSet<int> selectedPixels = new HashSet<int>();

            int width = grayImage.GetLength(0);
            int height = grayImage.GetLength(1);

            while (centroids.Count < k)
            {
                int x = rand.Next(0, width);
                int y = rand.Next(0, height);
                int pixelIntensity = grayImage[x, y];

                // Ensure unique centroids
                if (!selectedPixels.Contains(pixelIntensity))
                {
                    centroids.Add(pixelIntensity);
                    selectedPixels.Add(pixelIntensity);
                }
            }

            return centroids;
        }

        private bool AssignPixelsToClusters(byte[,] grayImage, int width, int height, List<int> centroids, int[,] clusterAssignments)
        {
            bool changed = false;

            // Iterate through each pixel
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    byte intensity = grayImage[x, y];
                    int closestCentroidIndex = FindClosestCentroidIndex(intensity, centroids);

                    // If the pixel was assigned to a different centroid, update the assignment
                    if (clusterAssignments[x, y] != closestCentroidIndex)
                    {
                        clusterAssignments[x, y] = closestCentroidIndex;
                        changed = true;
                    }
                }
            }

            return changed;
        }

        private int FindClosestCentroidIndex(byte intensity, List<int> centroids)
        {
            int closestCentroidIndex = 0;
            int minDistance = Math.Abs(intensity - centroids[0]);

            for (int i = 1; i < centroids.Count; i++)
            {
                int distance = Math.Abs(intensity - centroids[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCentroidIndex = i;
                }
            }

            return closestCentroidIndex;
        }

        private void UpdateCentroids(List<int> centroids, int[,] clusterAssignments, byte[,] grayImage, int width, int height)
        {
            int[] newCentroids = new int[centroids.Count];
            int[] pixelCounts = new int[centroids.Count];

            // Sum intensities for each cluster
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int clusterIndex = clusterAssignments[x, y];
                    byte intensity = grayImage[x, y];

                    newCentroids[clusterIndex] += intensity;
                    pixelCounts[clusterIndex]++;
                }
            }

            // Recalculate centroids based on the average intensity
            for (int i = 0; i < centroids.Count; i++)
            {
                if (pixelCounts[i] > 0)
                {
                    centroids[i] = newCentroids[i] / pixelCounts[i];
                }
            }
        }

        private Bitmap ColorClusters(int[,] clusterAssignments, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);

            // Generate colors from a basic color palette
            Color[] colors = GenerateColorPalette(K);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int clusterIndex = clusterAssignments[x, y];
                    result.SetPixel(x, y, colors[clusterIndex]);
                }
            }

            return result;
        }

        private Color[] GenerateColorPalette(int k)
        {
            Color[] colors = new Color[k];
            for (int i = 0; i < k; i++)
            {
                int hue = (i * 360) / k; // Evenly distribute hues
                colors[i] = ColorFromHSV(hue, 1, 1); // Full saturation and brightness
            }
            return colors;
        }

        private Color ColorFromHSV(int hue, double saturation, double value)
        {
            int h = hue % 360;
            int hi = (h / 60) % 6;
            double f = (h / 60.0) - hi;
            double p = value * (1 - saturation);
            double q = value * (1 - f * saturation);
            double t = value * (1 - (1 - f) * saturation);
            double r = 0, g = 0, b = 0;

            switch (hi)
            {
                case 0: r = value; g = t; b = p; break;
                case 1: r = q; g = value; b = p; break;
                case 2: r = p; g = value; b = t; break;
                case 3: r = p; g = q; b = value; break;
                case 4: r = t; g = p; b = value; break;
                case 5: r = value; g = p; b = q; break;
            }

            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        private void SaveClusteredImage(string filePath)
        {
            if (clusteredImage == null)
            {
                MessageBox.Show("Error: Clustered image is not available to save.");
                return;
            }
            try
            {
                clusteredImage.Save(filePath);
                MessageBox.Show("Clustered image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving clustered image: " + ex.Message);
            }
        }

        private void DisplayImages()
        {
            if (originalImage == null || clusteredImage == null)
            {
                MessageBox.Show("Error: Images are not available for display.");
                return;
            }

            // Create PictureBox for the original image
            PictureBox pbOriginal = new PictureBox
            {
                Image = originalImage,
                Width = originalImage.Width,
                Height = originalImage.Height,
                Location = new Point(10, 10)
            };

            // Create PictureBox for the clustered image
            PictureBox pbClustered = new PictureBox
            {
                Image = clusteredImage,
                Width = clusteredImage.Width,
                Height = clusteredImage.Height,
                Location = new Point(originalImage.Width + 20, 10)
            };

            // Set PictureBox size mode for proper scaling
            pbOriginal.SizeMode = PictureBoxSizeMode.StretchImage;
            pbClustered.SizeMode = PictureBoxSizeMode.StretchImage;

            // Add PictureBox controls to the form
            this.Controls.Add(pbOriginal);
            this.Controls.Add(pbClustered);
        }
    }
}