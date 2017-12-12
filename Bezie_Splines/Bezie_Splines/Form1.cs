using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bezie_Splines
{
    public partial class Form1 : Form
    {
        public List<PointF> points = new List<PointF>();
        private Bitmap curBmp;
        public Color pointColor = Color.Gray;
        public Color linesColor = Color.LightGray;
        public Color bezierColor = Color.Blue;
        bool move = false;
        PointF ChangedPoint = new PointF();
        public int changepointindex;
        Graphics g;
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            curBmp = pictureBox1.Image as Bitmap;
            g = Graphics.FromImage(pictureBox1.Image);
        }

        

        private void CurveDrawing(List<PointF> curList)
        {

            var tempList = new List<PointF>(curList);

            if (curList.Count > 4)
            {
                int count_to_add = 0;
                if (curList.Count < 8)
                    count_to_add = 8 - curList.Count;
                else if (curList.Count % 2 == 1)
                    count_to_add = 1;
                else
                    count_to_add = 0;
                for (int i = 1; i <= count_to_add; ++i)
                    tempList.Add(curList[curList.Count - 1]);

                for (int cur = 2; cur < tempList.Count - 3; cur += 2)
                {
                    var p = new PointF((tempList[cur].X + tempList[cur + 1].X) / 2, (tempList[cur].Y + tempList[cur + 1].Y) / 2);
                    tempList.Insert(cur + 1, p);
                    ++cur;
                }
            }

            int curBezierPoint = 0;
            while (curBezierPoint + 3 < tempList.Count)
            {
                PointF point1 = tempList[curBezierPoint];
                PointF point2 = tempList[curBezierPoint + 1];
                PointF point3 = tempList[curBezierPoint + 2];
                PointF point4 = tempList[curBezierPoint + 3];
                curBezierPoint += 3;
                for (double t = 0.0; t <= 1.0; t += 0.0001)
                {
                    double x = Math.Pow(1 - t, 3) * point1.X +
                                3 * t * Math.Pow(1 - t, 2) * point2.X +
                                3 * Math.Pow(t, 2) * (1 - t) * point3.X +
                                Math.Pow(t, 3) * point4.X;
                    double y = Math.Pow(1 - t, 3) * point1.Y +
                                3 * t * Math.Pow(1 - t, 2) * point2.Y +
                                3 * Math.Pow(t, 2) * (1 - t) * point3.Y +
                                Math.Pow(t, 3) * point4.Y;
                    curBmp.SetPixel((int)x, (int)y, bezierColor);
                }
            }
            for (int i = 0; i < tempList.Count; i++)
            {
                g.FillEllipse(new SolidBrush(Color.Green), tempList[i].X - 5, tempList[i].Y - 5, 10, 10);
            }
            pictureBox1.Refresh();
        }

        private void DrawPoint()
        {
            g.Clear(Color.White);
            for (int i = 0; i < points.Count - 1; ++i)
            {
                g.FillEllipse(new SolidBrush(pointColor), points[i].X - 5, points[i].Y - 5, 10, 10);
                g.DrawLine(new Pen(linesColor), points[i], points[i + 1]);
            }
            g.FillEllipse(new SolidBrush(pointColor), points[points.Count - 1].X - 5, points[points.Count - 1].Y - 5, 10, 10);
            if (points.Count >= 4)
            {
                CurveDrawing(points);
            }
            pictureBox1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, pictureBox1.Width, pictureBox1.Height);
            points.Clear();
            listBox1.Items.Clear();
            pictureBox1.Invalidate();
        }
        
        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (listBox1.SelectedItems.Count > 0 && points[i].ToString() == listBox1.SelectedItem.ToString())
                {
                    g.FillEllipse(new SolidBrush(Color.Red), points[i].X - 5, points[i].Y - 5, 10, 10);
                }
                else g.FillEllipse(new SolidBrush(pointColor), points[i].X - 5, points[i].Y - 5, 10, 10);
            }

            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (move)
                {
                    points[changepointindex] = new PointF(e.X, e.Y);
                    listBox1.Items.Remove(listBox1.SelectedItem);
                    listBox1.Items.Add(new PointF(e.X, e.Y));
                    move = false;
                }
                else
                {
                    points.Add(new PointF(e.X, e.Y));
                    listBox1.Items.Add(new PointF(e.X, e.Y));

                }
                DrawPoint();
            }
            else
            {
                if (curBmp.GetPixel(e.X, e.Y).ToArgb() == pointColor.ToArgb())
                {
                    int i, j = 0;
                    double min = double.MaxValue;
                    for (i = 0; i < points.Count; ++i)
                    {
                        double m;
                        if ((m = Math.Sqrt(Math.Pow(e.X - points.ElementAt(i).X, 2) + Math.Pow(e.Y - points.ElementAt(i).Y, 2))) < min)
                        {
                            min = m;
                            j = i;
                        }
                    }
                    points.RemoveAt(j);
                    DrawPoint();
                    pictureBox1.Refresh();
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].ToString() == listBox1.SelectedItem.ToString())
                    {
                        listBox1.Items.Remove(points[i]);
                        points.Remove(points[i]);
                        break;
                    }
                }
                DrawPoint();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                move = true;
                for (int i = 0; i < points.Count; i++)
                {
                    if (points[i].ToString() == listBox1.SelectedItem.ToString())
                    {
                        ChangedPoint = new PointF(points[i].X, points[i].Y);
                        changepointindex = i;

                        break;
                    }
                }
            }
        }
    }
}
