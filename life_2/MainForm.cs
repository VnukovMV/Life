/*
 * Создано в SharpDevelop.
 * Пользователь: VnukovMV
 * Дата: 19.08.2020
 * Время: 17:34
 * 
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace life_2
{
	public partial class MainForm : Form
	{
		private Graphics graphicsl;
		private int xres;
		private bool[,] xfield;
		private int xrows;
		private int xcols;
		private int countP = 0;
		private int countOldP = 0;
		private int stagnationP = 0;
		private int countG = 0;
		private int maxN = 3;
		private int minN = 2;
		private int burn = 3;
		
		
		
		
		public MainForm()
		{
			InitializeComponent();
			pictureBox1.Refresh();
		}
		
		void StartGame()
		{
			if(timer1.Enabled)
				return;
			
			splitContainer1.SplitterDistance = 
				(int)numericUpDown1.Width + (int)numericUpDown1.Location.X + 10;
			
			
			countG = 0;
			Text = "Поколение " + countG;
			xres = (int)numericUpDown1.Value;
			xrows = pictureBox1.Height / xres;
			xcols = pictureBox1.Width / xres;
			xfield = new bool[xcols,xrows];
			
			Random random = new Random();
			for(int x = 0; x < xcols; x++)
			{
				for(int y = 0; y < xrows; y++)
				{	
					xfield[x,y] = random.Next(100 - (int)numericUpDown2.Value) == 0;
				}
			}
			countP = 100;
			
			pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			graphicsl = Graphics.FromImage(pictureBox1.Image);
			timer1.Start();
		}
		
		void NextG()
		{
			countP = 0;
			
			graphicsl.Clear(Color.Black);
			
			var newXfield = new bool[xcols,xrows];
			
			for(int x = 0; x < xcols; x++)
			{
				for(int y = 0; y < xrows; y++)
				{
					var nCount = CountN(x,y);
					var hasLife = xfield[x,y];
					
					if(!hasLife && nCount == burn)
						newXfield[x,y] = true;
					else if (hasLife && (nCount <minN || nCount > maxN))
					   	newXfield[x,y] = false;
					else
						newXfield[x,y] = xfield[x,y];
					
					if(hasLife)
					{
						graphicsl.FillRectangle(Brushes.LawnGreen,x * xres,y * xres,xres,xres);
						countP++;
					}
				}
			}
			pictureBox1.Refresh();
			Text = "Поколение " + ++countG + "    Популяция: "+countP + " ПредидущаяП: " + countOldP;
			xfield = newXfield;
			pictureBox1.Refresh();
			if(countOldP == countP)
			{
				stagnationP++;
			}
			else
			{
				countOldP = countP;
				stagnationP = 0;
			}
			
		}
		
		int CountN(int x, int y)
		{
			int count = 0;
			
			for(int i = -1; i < 2; i++)
			{
				for(int j = -1; j < 2; j++)
				{
					var col = (x + i + xcols) % xcols;
					var row = (y + j + xrows) % xrows;
	
					var isSelfChecking = col == x && row == y;
					var haslife = xfield[col,row];
					
					if(haslife && !isSelfChecking)
						count++;
				}
			}	
			return count;
		}
		
		void Timer1Tick(object sender, EventArgs e)
		{
			timer1.Interval = 100 - (int)numericUpDown3.Value;
			minN = (int)numericUpDown4.Value;
			maxN  = (int)numericUpDown5.Value;
			burn  = (int)numericUpDown6.Value;
			
			if (countP == 0) 
			{
				timer1.Stop();
				numericUpDown1.Enabled = true;
				numericUpDown2.Enabled = true;
				bPause.Visible = false;
				
				bPause.Font = new Font(bPause.Font.FontFamily, 9);
				bPause.Text = "ПРОДОЛЖИТЬ";
				bStart.Text = "ЗАНОВО";
			}
			else
			{
				NextG();
			}
		}
		
		void BStartClick(object sender, EventArgs e)
		{
			if(timer1.Enabled)
			{
				timer1.Stop();
				numericUpDown1.Enabled = true;
				numericUpDown2.Enabled = true;
				bStart.Text = "СТАРТ";
				bPause.Visible = false;
			}
			else
			{
			numericUpDown1.Enabled = false;
			numericUpDown2.Enabled = false;
			bStart.Text = "СТОП";
			bPause.Font = new Font(bPause.Font.FontFamily, 13);
			bPause.Text = "ПАУЗА";
			StartGame();
			bPause.Visible = true;
			}
						
		
		}
		
		void BPauseClick(object sender, EventArgs e)
		{
			if(timer1.Enabled)
			{
				timer1.Stop();
				bPause.Font = new Font(bPause.Font.FontFamily, 9);
				bPause.Text = "ПРОДОЛЖИТЬ";
				bStart.Text = "ЗАНОВО";
			}
			else
			{
				timer1.Start();
				bPause.Font = new Font(bPause.Font.FontFamily, 13);
				bPause.Text = "ПАУЗА";
				bStart.Text = "СТОП";
			}
		}
		
		void PictureBox1MouseClick(object sender, MouseEventArgs e)
		{
			if(countG == 0)
				return;
			
			if(e.Button == MouseButtons.Left)
			{
				var x = e.Location.X / xres;
				var y = e.Location.Y / xres;
				xfield[x,y] = true;
				graphicsl.FillRectangle(Brushes.Aqua ,x * xres,y * xres,xres,xres);
				pictureBox1.Refresh();
			}
			
			if(e.Button == MouseButtons.Right)
			{
				var x = e.Location.X / xres;
				var y = e.Location.Y / xres;
				xfield[x,y] = false;
				graphicsl.FillRectangle(Brushes.Black,x * xres,y * xres,xres,xres);
				pictureBox1.Refresh();
			}
		}
		
		
		
		void PictureBox1MouseMove(object sender, MouseEventArgs e)
		{
			if(countG == 0)
				return;
			
			if(e.Button == MouseButtons.Left)
			{
				var x = e.Location.X / xres;
				var y = e.Location.Y / xres;
				xfield[x,y] = true;
				graphicsl.FillRectangle(Brushes.Aqua ,x * xres,y * xres,xres,xres);
				pictureBox1.Refresh();
			}
			
			if(e.Button == MouseButtons.Right)
			{
				var x = e.Location.X / xres;
				var y = e.Location.Y / xres;
				xfield[x,y] = false;
				graphicsl.FillRectangle(Brushes.Black,x * xres,y * xres,xres,xres);
				pictureBox1.Refresh();
			}
		}
		
		void MainFormResize(object sender, EventArgs e)
		{
			splitContainer1.SplitterDistance = 
				(int)numericUpDown1.Width + (int)numericUpDown1.Location.X + 10;
		}
	}
}
