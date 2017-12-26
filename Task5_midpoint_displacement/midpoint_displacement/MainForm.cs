using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace midpoint_displacement
{
	public partial class MainForm : Form
	{
		Bitmap bmp;
		private System.Collections.Generic.List<System.Drawing.Point> pts = new List<Point>();
		int clicks = 0;
		public MainForm()
		{
			InitializeComponent();
			bmp=new Bitmap(pictureBox1.Width,pictureBox1.Height);
			this.pictureBox1.Image=bmp;
		}
		
		void clear(){
			bmp.Dispose();
			bmp=new Bitmap(pictureBox1.Width,pictureBox1.Height);
			this.pictureBox1.Image=bmp;
		}
		
		void Click_clear(object sender, System.EventArgs e){
			//this.pictureBox1.Image.Dispose();
			clear();
			clicks=0;
			pts = new List<Point>();
		}
		
		void draw_points(Graphics g){
			for(int i = 0; i < pts.Count; i++)
				g.DrawLine(new Pen(Color.Black, 3),pts[i].X-1,pts[i].Y-1,pts[i].X+1,pts[i].Y-1);
		}
		
		void pct_click(object sender, System.Windows.Forms.MouseEventArgs e){
			if (clicks > 1)
			 return;
			clicks++;
			Graphics g = Graphics.FromImage(bmp);
			g.DrawLine(new Pen(Color.Black, 3),e.X-1,e.Y-1,e.X+1,e.Y-1);
			pts.Add(new Point(e.X,e.Y));
			g.Dispose();
			pictureBox1.Invalidate();
		}
		
		int length(Point p1, Point p2){
			return (int)Math.Sqrt((p2.X-p1.X)*(p2.X-p1.X)+(p2.Y-p1.Y)*(p2.Y-p1.Y))/10;
		}
		
		void midpoint_displacement(Point pt1, Point pt2, int r, int ind_r){
			if (pt2.X-pt1.X < 2) return;
			
			int l = length(pt1,pt2);
			Random rnd = new Random();
			int h = (pt1.Y + pt2.Y) / 2 + rnd.Next(- r * l, r * l + 1);
			int w = (pt1.X+pt2.X)/2;
			Point mid = new Point(w,h);
			pts.Insert(ind_r, mid);
			clear();
			
			{
				Point[] ppts = pts.ToArray();
				Graphics g = Graphics.FromImage(bmp);
				g.DrawLines(new Pen(Color.Black, 1), ppts);
				draw_points(g);
				g.Dispose();
				System.Threading.Thread.Sleep(100);
			}
			pictureBox1.Update();
			
			midpoint_displacement(mid,pt2,r,ind_r+1);
			midpoint_displacement(pt1,mid,r,ind_r);
		}
		
		void Click_make(object sender, System.EventArgs e){
			midpoint_displacement(pts[0],pts[1],Int32.Parse(textBox1.Text),1);
		}
	}
}
