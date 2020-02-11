using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Q_Learning
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }
        double[,,] Qtable = new double[5,5,4];
        Button[,] bu = new Button[5,5];
        void addbutton()
        {
            bu = new Button[5, 5];
            for(int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    bu[x, y] = new Button();
                    bu[x, y].Text = "";
                    bu[x, y].Location = new Point(20 + x * 66, 20 + y * 66);
                    bu[x, y].Size = new Size(65,65);
                    bu[x, y].BackColor = System.Drawing.Color.White;
                    bu[x, y].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    groupBox1.Controls.Add(bu[x, y]);
                }
            }
        }
        List<Point> treasure = new List<Point>()
        {
            new Point(2,0)
        };
        List<Point> trap = new List<Point>()
        {
            new Point(1,0),
            new Point(1,1),
            new Point(1,2),
            new Point(2,3),
            new Point(4,3),
            // new Point(2,3),
            //  new Point(3,3),
            //   new Point(4,1),
            //    new Point(3,1)

        };
        void Drawbu(int x ,int y)
        {
            int count=0;
            foreach(Button b in bu)
            {
                b.BackColor = Color.White;
                //b.Text = Qtable[count].ToString();
                count++;
            }
            foreach (var a in trap)
                bu[a.X, a.Y].BackColor = Color.Black;
            foreach (var a in treasure)
                bu[a.X, a.Y].BackColor = Color.Green;
            bu[x, y].BackColor = Color.Red;
        }
        bool stop = false;
        private void button3_Click(object sender, EventArgs e)
        {
            stop = false;
            double alpha = 0.5;
            double beta = 0.5;
            DataTable dt = new DataTable();
            dt.Columns.Add("Time");
            dataGridView1.DataSource = dt;
            Task.Run(() =>
            {
                int x = 0;
                int y = 0;
                int r=0;
                Random rd = new Random();
                int count = 0;
                while (!stop)
                {
                    List<search_value> candown = new List<search_value>();
                    candown = candown_point(x,y);
                    foreach (var a in candown)
                    {
                        List<search_value> candownM = new List<search_value>();
                        candownM = candown_point(a.point.X, a.point.Y);
                        double max_Q = candownM.Max(z => z.value);
                        if (trap.FindAll(z => z.X == a.point.X && z.Y == a.point.Y).Count != 0)
                        {
                            r = -1;
                            beta = 0;
                        }
                        else if (treasure.FindAll(z => z.X == a.point.X && z.Y == a.point.Y).Count != 0)
                        {
                            r = 1;
                            beta = 0;
                        }
                        else
                        {
                            beta = 0.8;
                            r = 0;
                        }
                        Qtable[x,y,a.s] = a.value + alpha * (r + (beta * max_Q) - a.value);
                    }
                    var candow_g = candown.GroupBy(z=>z.value).Select(z=>z.ToList()).ToList().OrderBy(z=>z.Max(zz=>-zz.value)).ToList();
                    int rdn = rd.Next(0,candow_g[0].Count);
                    x = candow_g[0][rdn].point.X;
                    y = candow_g[0][rdn].point.Y;
                    Drawbu(x,y);
                    count++;
                    if(treasure.FindAll(z => z.X == x && z.Y == y).Count != 0 || trap.FindAll(z => z.X == x && z.Y == y).Count != 0)
                    {
                        Thread.Sleep(20);
                        x = 0;
                        y = 0;
                        dt.Rows.Add(count);
                        if (dt.Rows.Count > 5) dt.Rows.RemoveAt(0);
                        count = 0;
                        Drawbu(x, y);
                       
                    }
                    Thread.Sleep(strp_speed);
                }
            });
        }
        public List<search_value> candown_point(int x , int y)
        {
            List<search_value> candown = new List<search_value>();
            if (chech_wall(x - 1, y)) candown.Add(new search_value() { point = new Point(x - 1, y), value = Qtable[x,y,0] ,s=0});
            if (chech_wall(x + 1, y)) candown.Add(new search_value() { point = new Point(x+1, y), value = Qtable[x,y,1] ,s=1});
            if (chech_wall(x, y - 1)) candown.Add(new search_value() { point = new Point(x, y-1), value = Qtable[x,y,2],s=2 });
            if (chech_wall(x, y + 1)) candown.Add(new search_value() { point = new Point(x, y+1), value = Qtable[x, y, 3],s=3 });
            return candown;
        }
        public bool chech_wall(int x ,int y)
        {
            if (x < 0 || y < 0 || x == 5 || y == 5) return false;
            else return true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            addbutton();
            Drawbu(0,0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            stop = stop ==  true?false:true;
        }
        int strp_speed = 10;
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
            strp_speed = trackBar1.Value;
        }
    }
}
public class search_value
{
    public Point point { get; set; }
    public int s { get; set; }
    public double value { get; set; }
}
