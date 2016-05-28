using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AutomatKomorkowy
{
    public partial class Form1 : Form
    {
        private int[][] _pixelData = new int[20][];

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                LoadPixelData();
                DrawAllPixels(bitmap, _pixelData);
                pictureBox1.Image = bitmap;
            }
            catch (Exception exception)
            {
                label3.Text = exception.Message;
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var deadRules = ParseRules(textBox1.Text);
                var aliveRules = ParseRules(textBox2.Text);
                CalculateNewPixelData(deadRules, aliveRules);
                Bitmap bitmap = new Bitmap(pictureBox1.Image);
                DrawAllPixels(bitmap, _pixelData);
                pictureBox1.Image = bitmap;
            }
            catch (Exception exception)
            {
                label3.Text = exception.Message;
            }
            
        }

        private static void SetPixel(Bitmap bitmap, Point point, Color color)
        {
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    bitmap.SetPixel(point.X * 20 + i, point.Y * 20 + j, color);
                }
            }
        }

        private static void DrawAllPixels(Bitmap bitmap, int[][] pixelData)
        {
            for (int i = 0; i < pixelData.Length; i++)
            {
                for (int j = 0; j < pixelData[i].Length; j++)
                {
                    Color c = Color.Silver;
                    if (pixelData[i][j] == 1)
                    {
                        c = Color.Black;
                    }
                    SetPixel(bitmap, new Point(j, i), c);
                }
            }
        }

        private void LoadPixelData()
        {
            using (StreamReader streamReader = new StreamReader(@"PixelData.txt"))
            {
                string pixelData = streamReader.ReadToEnd();
                string[] pixelRows = pixelData.Split('\n');
                if (pixelRows.Length != 20)
                    return;
                for (int i = 0; i < 20; i++)
                {
                    string[] pixels = pixelRows[i].Trim().Split(' ');
                    if (pixels.Length != 20) return;
                    _pixelData[i] = new int[20];
                    for (int j = 0; j < 20; j++)
                    {
                        int.TryParse(pixels[j], out _pixelData[i][j]);
                    }
                }
            }
        }

        private void CalculateNewPixelData(int[] rulesDead, int[] rulesAlive)
        {
            int[][] tempPixelData = DeepPixelCopy();
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    if (tempPixelData[i][j] == 0)
                    {
                        int aliveN = CountAliveNeighbours(new Point(i, j), tempPixelData);
                        for (int k = 0; k < rulesDead.Length; k++)
                        {
                            if (rulesDead[k] == aliveN)
                            {
                                _pixelData[i][j] = 1;
                            }
                        }
                    }
                    if (tempPixelData[i][j] == 1)
                    {
                        int aliveN = CountAliveNeighbours(new Point(i, j), tempPixelData);
                        _pixelData[i][j] = 0;
                        for (int k = 0; k < rulesAlive.Length; k++)
                        {
                            if (rulesAlive[k] == aliveN)
                            {
                                _pixelData[i][j] = 1;
                            }
                        }
                    }
                }
            }
        }

        private int CountAliveNeighbours(Point point, int[][] pixelData)
        {
            int aliveNeighbours = 0;
            if (point.X - 1 > 0 && point.Y - 1 > 0 && pixelData[point.X - 1][point.Y - 1] == 1)
                aliveNeighbours++;
            if (point.Y - 1 > 0 && pixelData[point.X][point.Y - 1] == 1)
                aliveNeighbours++;
            if (point.X + 1 < 20 && point.Y - 1 > 0 && pixelData[point.X + 1][point.Y - 1] == 1)
                aliveNeighbours++;
            if (point.X - 1 > 0 && pixelData[point.X - 1][point.Y] == 1)
                aliveNeighbours++;
            if (point.X + 1 < 20 && pixelData[point.X + 1][point.Y] == 1)
                aliveNeighbours++;
            if (point.X - 1 > 0 && point.Y + 1 < 20 && pixelData[point.X - 1][point.Y + 1] == 1)
                aliveNeighbours++;
            if (point.Y + 1 < 20 && pixelData[point.X][point.Y + 1] == 1)
                aliveNeighbours++;
            if (point.X + 1 < 20 && point.Y + 1 < 20 && pixelData[point.X + 1][point.Y + 1] == 1)
                aliveNeighbours++;
            return aliveNeighbours;
        }

        private int[] ParseRules(string rulesString)
        {
            var splitedRules = rulesString.Split(',');
            var rules = new int[splitedRules.Length];
            for (int i = 0; i < splitedRules.Length; i++)
            {
                int.TryParse(splitedRules[i], out rules[i]);
            }
            return rules;
        }

        private int[][] DeepPixelCopy()
        {
            var pixelCopy = new int[20][];
            for (int i = 0; i < _pixelData.Length; i++)
            {
                pixelCopy[i] = new int[_pixelData[i].Length];

                for (int j = 0; j < _pixelData[i].Length; j++)
                {
                    pixelCopy[i][j] = _pixelData[i][j];
                }
            }
            return pixelCopy;
        }
    }
}
