using System;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace ConwayLifeGame
{
    public static class Map
    {
        public class Node
        {
            public int y;
            public bool state;
            public byte count;
            public Node next;
            public Node() { y = 0; state = false; count = 0; next = null; }
        };
        
        public class Head
        {
            public int x;
            public Node node;
            public Head next;
            public Head() { x = 0; node = new Node(); next = null; }
        };
        
        public class Preset
        {
            public Point[] points;
            public byte height;
            public byte width;
            public string name;
            public Preset() { height = 0; width = 0; name = ""; }
            public Preset(Point[] points, byte height, byte width, string name = "")
            {
                this.points = points; this.height = height; this.width = width; this.name = name;
            }
        }
        
        private readonly static Head cur = new Head();
        private readonly static Head nxt = new Head();
        private static Preset[] presets;
        
        public enum AddRegionState
        {
            normal,
            insert,
            delete,
            random
        };
        public struct AddRegionInfo
        {
            public AddRegionState state;
            public bool count;
            public MouseState lastMouseState;
        };
        public static AddRegionInfo AddRgnInfo;

        public enum KeyboardInputState
        {
            normal,
            bulitin,
            direction
        }
        public static KeyboardInputState KeybdInputState { get; set; }

        public enum MouseState
        {
            click,
            pen,
            eraser,
            drag,
            select
        }
        public struct MouseInformation
        {
            public MouseState state;
            public Point previous;          // Rleative to Map
            public Point select_first;      // Relative to Window
            public Point select_second;     // Relative to Window
        }
        public static MouseInformation MouseInfo;

        public enum CopyState
        {
            replace,
            merge,
            cancel
        }
        public struct CopyInformation
        {
            public bool state;
            public Point first;
            public Point second;
            public CopyState copyState;
        }
        public static CopyInformation CopyInfo;

        private static int _selectedPreset, _selectedDirection, _xPivot = 0x08000000, _yPivot = 0x08000000, _timer = 100, _scale = 10;
        public static int SelectedPreset { get => _selectedPreset; set => _selectedPreset = value; }
        public static int SelectedDirection { get => _selectedDirection; set => _selectedDirection = value; }
        public static int XPivot { get => _xPivot; set => _xPivot = value; }
        public static int YPivot { get => _yPivot; set => _yPivot = value; }
        public static int Timer { get => _timer; set => _timer = value; }
        public static int Scale { get => _scale; set => _scale = value; }

        private static bool _started;
        public static bool Started { get => _started; set => _started = value; }

        private static Head Add(int xpos, int ypos, Head acce)
        {
            Head px = nxt;
            if (acce != null && acce.x <= xpos) px = acce;
            while (px.next != null && px.next.x <= xpos) px = px.next;
            if (px.x == xpos && px.node != null)
            {
                Node py = px.node;
                while (py.next != null && py.next.y <= ypos) py = py.next;
                if (py.y == ypos) py.count++;               //If the Node already exists: add 1 to count
                else
                {                                      //If the Node doesn't exist: insert a Node
                    Node pn = Insert(py);
                    pn.y = ypos;
                    pn.count = 1;
                }
            }
            else
            {                                           //If the row doesn't exist: insert Head and Node
                Head pn = Insert(px);
                Node node = Insert(pn.node);

                pn.x = xpos;
                node.y = ypos;
                node.count = 1;
            }
            return px;
        }

        /*type: {
            0: 1->0, 0->1;
            1: 0,1->1;
            2: 0,1->0}
        */
        public static Head Change(int xpos, int ypos, int type = 0, Head acce = null)
        {
            if (xpos <= 0 || ypos <= 0) return null;
            Head px = cur;
            if (acce != null && acce.x <= xpos) px = acce;
            while (px.next != null && px.next.x <= xpos) px = px.next;
            if (px.x == xpos && px.node != null)
            {
                Node py = px.node;
                while (py.next != null && py.next.y < ypos) py = py.next;
                if (py.next != null && py.next.y == ypos) { if (type != 1) Del(py); }  //If the Node already exists: destroy the Node
                else if (type != 2)
                {                                       //If the Node doesn't exist: insert a Node
                    Node pn = Insert(py);
                    pn.y = ypos;
                    pn.state = true;
                }
            }
            else if (type != 2)
            {                                           //If the row doesn't exist: insert Head and Node
                Head pn = Insert(px);
                Node pnode = Insert(pn.node);
                pn.x = xpos;
                pnode.y = ypos;
                pnode.state = true;
            }
            return px;
        }

        public static void ChangeNxt(int xpos, int ypos) 
        {
            Head px = nxt;
            while (px.x < xpos) px = px.next;
            Node py = px.node.next;
            while (py.y < ypos) py = py.next;
            py.state = true;
        }

        public static void Calc()
        {
            Head px = cur.next, pacce = null, ptmp;
            while (px != null)
            {
                Node py = px.node.next;
                for (; py != null; py = py.next)
                {
                    if (!py.state) continue;
                    int x = px.x;
                    int y = py.y;
                    ptmp = pacce;
                    ptmp = Add(x - 1, y - 1, ptmp);
                    pacce = ptmp;
                    Add(x - 1, y, ptmp);
                    Add(x - 1, y + 1, ptmp);
                    ptmp = Add(x, y - 1, ptmp);
                    Add(x, y, ptmp);
                    Add(x, y + 1, ptmp);
                    ptmp = Add(x + 1, y - 1, ptmp);
                    Add(x + 1, y, ptmp);
                    Add(x + 1, y + 1, ptmp);
                    ChangeNxt(x, y);
                }
                px = px.next;
            }
            Clear(cur);
            px = nxt.next;
            while (px != null)
            {
                Node py = px.node.next;
                for (; py != null; py = py.next) if ((py.count == 3) || (py.state && py.count == 4)) Change(px.x, py.y);
                px = px.next;
            }
            Clear(nxt);
            Draw();
        }

        public static void LoadLF(string f)
        {
            // Stop
            Program.control.Reset_Click(null, null);
            FileStream fs = null;
            try
            {
                fs = new FileStream(f, FileMode.Open);
                BinaryReader reader = new BinaryReader(fs);
                _xPivot = reader.ReadInt32();
                _yPivot = reader.ReadInt32();
                if (reader.ReadUInt32() != 0xffffffff)
                {
                    Program.control.Reset_Click(null, null);
                    throw new Exception("Bad file! Map reset");
                }
                while (true)
                {
                    uint x, y;
                    x = reader.ReadUInt32();
                    y = reader.ReadUInt32();
                    if (x == 0xffffffff && y == 0xfffffffd) break;
                    Change((int)x, (int)y);
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message, "Error", System.Windows.Forms.MessageBoxButtons.OK);
                if (fs != null && fs.CanRead) fs.Close();
            }
        }

        private class DumpStruct
        {
            public struct Point
            {
                public int X { get; set; }
                public int Y { get; set; }
                public Point(int x, int y) { X = x; Y = y; }
            }
            public Point[] P { get; set; }
            public int Xp { get; set; }
            public int Yp { get; set; }
            public int S { get; set; }
        }

        public static void LoadLFS(string f) 
        {
            Program.control.Reset_Click(null, null);
            
            string str = File.ReadAllText(f);
            DumpStruct s = JsonSerializer.Deserialize<DumpStruct>(str);

            _xPivot = s.Xp; _yPivot = s.Yp; _scale = s.S;
            foreach (DumpStruct.Point p in s.P) { Change(p.X + _xPivot, p.Y + _yPivot); }
        }

        public static void DumpLFS(string f)
        {
            if (_started) Program.control.StartStop_Click(null, null);

            DumpStruct s = new DumpStruct
            {
                // Stat
                Xp = _xPivot,
                Yp = _yPivot,
                S = _scale
            };

            // Points
            int c = 0;
            for (Head h = cur; h != null; h = h.next) for (Node n = h.node.next; n != null; n = n.next) c++;
            DumpStruct.Point[] p = new DumpStruct.Point[c];
            c = 0;
            for (Head h = cur; h != null; h = h.next) for (Node n = h.node.next; n != null; n = n.next) { p[c] = new DumpStruct.Point(h.x - _xPivot, n.y - _yPivot); c++; }
            s.P = p;

            // Dump
            FileStream fs = new FileStream(f, FileMode.Create);
            fs.Write(JsonSerializer.SerializeToUtf8Bytes(s));
            fs.Close();
        }

        public static void Draw()
        {
            Bitmap bitmap = new Bitmap(Program.main.MainPictureBox.Width, Program.main.MainPictureBox.Height);
            int mid_x = Program.main.MainPictureBox.Width / 2, mid_y = Program.main.MainPictureBox.Height / 2;
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);
            graphics.TranslateTransform(mid_x, mid_y);
            int left = (-mid_x) / _scale + _xPivot - 1, right = mid_x / _scale + _xPivot + 1;
            int top = (-mid_y) / _scale + _yPivot - 1, bottom = mid_y / _scale + _yPivot + 1;
            Head pl = cur;
            while (pl.next != null && pl.next.x < left) pl = pl.next;
            Head px = pl;
            SolidBrush brush = new SolidBrush(Color.Black);
            while (px.next != null && px.next.x <= right)
            {
                Node py = px.next.node;
                while (py.next != null && py.next.y < top) py = py.next;
                while (py.next != null && py.next.y <= bottom)
                {
                    graphics.FillRectangle(brush, (px.next.x - _xPivot) * _scale + 1, (py.next.y - _yPivot) * _scale + 1, _scale - 1, _scale - 1);
                    py = py.next;
                }
                px = px.next;
            }
            graphics.Dispose();
            brush.Dispose();
            Bitmap t = Main.MapBitmap;
            Main.MapBitmap = bitmap;
            t.Dispose();
        }

        private static Head Insert(Head p)
        {
            Node node = new Node();
            Head pn = new Head { next = p.next, node = node };
            p.next = pn;
            return pn;
        }

        private static Node Insert(Node p)
        {
            Node pn = new Node { next = p.next };
            p.next = pn;
            return pn;
        }

        private static void Del(Node p)
        {
            p.next = p.next?.next;
        }

        /*private static void Del(Head h)
        {
            Head pd = h.next;
            pd.node = null;
            h.next = pd.next;
        }*/

        public static void Reset()
        {
            _started = false;
            _selectedPreset = _selectedDirection = 0;
            _xPivot = _yPivot = 0x08000000;
            _timer = 100;
            _scale = 10;
            Clear(cur);
        }

        private static void Clear(Head h)
        {
            h.next = null;
        }

        public static void AddPreset(int xpos, int ypos)
        {
            byte s = (byte)presets[_selectedPreset].points.Length, l = (byte)(presets[_selectedPreset].width - 1), h = (byte)(presets[_selectedPreset].height - 1);
            Point[] cur = presets[_selectedPreset].points;
            switch (_selectedDirection)
            {
                case 0:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].X, ypos + cur[i].Y, 1);
                    break;
                case 1:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].X, ypos + h - cur[i].Y, 1);
                    break;
                case 2:
                    for (byte i = 0; i < s; i++) Change(xpos + l - cur[i].X, ypos + cur[i].Y, 1);
                    break;
                case 3:
                    for (byte i = 0; i < s; i++) Change(xpos + l - cur[i].X, ypos + h - cur[i].Y, 1);
                    break;
                case 4:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].Y, ypos + cur[i].X, 1);
                    break;
                case 5:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].Y, ypos + l - cur[i].X, 1);
                    break;
                case 6:
                    for (byte i = 0; i < s; i++) Change(xpos + h - cur[i].Y, ypos + cur[i].X, 1);
                    break;
                case 7:
                    for (byte i = 0; i < s; i++) Change(xpos + h - cur[i].Y, ypos + l - cur[i].X, 1);
                    break;
            }
        }

        public static void AddDeleteRegion(Point p1, Point p2)
        {
            int left = Math.Min(p1.X, p2.X), top = Math.Min(p1.Y, p2.Y), right = Math.Max(p1.X, p2.X), bottom = Math.Max(p1.Y, p2.Y);
            Head acce = null;
            switch (AddRgnInfo.state)
            {
                case AddRegionState.random:
                    {
                        Random r = new Random();
                        for (int x = left; x <= right; x++)
                            for (int y = bottom; y >= top; y--)
                                if (r.Next(0, 2) != 0)
                                    acce = Change(x, y, 1, acce);
                        break;
                    }
                case AddRegionState.insert:
                    {
                        for (int x = left; x <= right; x++)
                            for (int y = bottom; y >= top; y--)
                                acce = Change(x, y, 1, acce);
                        break;
                    }

                case AddRegionState.delete:
                    {
                        for (int x = left; x <= right; x++)
                            for (int y = bottom; y >= top; y--)
                                acce = Change(x, y, 2, acce);
                        break;
                    }
                default: break;
            }
        }

        public static Preset GetBulitinInfo(int b = -1)
        {
            if (b == -1) b = _selectedPreset;
            try { return presets[b]; }
            catch (Exception) { return null; }
        }

        public static void Initialize()
        {
            InitPresets();
        }

        /*private static void InitPresets()
        {
            presets = new Preset[6];

            //{1,1,1},
            //{1,0,0},
            //{0,1,0}

            Point[] preset0 = new Point[] {
                new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 2), new Point(2, 0)
            };
            presets[0] = new Preset(preset0, 3, 3);

            //{0,1,0,0,1},
            //{1,0,0,0,0},
            //{1,0,0,0,1},
            //{1,1,1,1,0}	

            Point[] preset1 = new Point[] {
                new Point(0, 1), new Point(0, 2), new Point(0, 3), new Point(1, 0), new Point(1, 3), new Point(2, 3), new Point(3, 3), new Point(4, 0), new Point(4, 2),
            };
            presets[1] = new Preset(preset1, 4, 5);

            //
            //{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
            //{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0},
            //{0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
            //{0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
            //{1,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            //{1,1,0,0,0,0,0,0,0,0,1,0,0,0,1,0,1,1,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0},
            //{0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
            //{0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            //{0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
            //

            Point[] preset2 = new Point[] {
                new Point(0, 4), new Point(0, 5), new Point(1, 4), new Point(1, 5),
                new Point(10, 4), new Point(10, 5), new Point(10, 6), new Point(11, 3), new Point(11, 7), new Point(12, 2), new Point(12, 8), new Point(13, 2), new Point(13, 8),
                new Point(14, 5), new Point(15, 3), new Point(15, 7), new Point(16, 4), new Point(16, 5), new Point(16, 6), new Point(17, 5),
                new Point(20, 2), new Point(20, 3), new Point(20, 4), new Point(21, 2), new Point(21, 3), new Point(21, 4), new Point(22, 1), new Point(22, 5),
                new Point(24, 0), new Point(24, 1), new Point(24, 5), new Point(24, 6),
                new Point(34, 2), new Point(34, 3), new Point(35, 2), new Point(35, 3),
            };
            presets[2] = new Preset(preset2, 9, 36);

            //
            //{0,0,0,0,0,0,1,0,0,0,0,0,0},
            //{0,0,0,0,0,1,0,1,0,0,0,0,0},
            //{0,0,0,0,0,1,0,1,0,0,0,0,0},
            //{0,0,0,0,0,0,1,0,0,0,0,0,0},
            //{0,0,0,0,0,0,0,0,0,0,0,0,0},
            //{0,1,1,0,0,0,0,0,0,0,1,1,0},
            //{1,0,0,1,0,0,0,0,0,1,0,0,1},
            //{0,1,1,0,0,0,0,0,0,0,1,1,0},
            //{0,0,0,0,0,0,0,0,0,0,0,0,0},
            //{0,0,0,0,0,0,1,0,0,0,0,0,0},
            //{0,0,0,0,0,1,0,1,0,0,0,0,0},
            //{0,0,0,0,0,1,0,1,0,0,0,0,0},
            //{0,0,0,0,0,0,1,0,0,0,0,0,0}
            //

            Point[] preset3 = new Point[] {
                new Point(0, 6), new Point(1, 5), new Point(1, 7), new Point(2, 5), new Point(2, 7), new Point(3, 6),
                new Point(5, 1), new Point(5, 2), new Point(6, 0), new Point(6, 3), new Point(7, 1), new Point(7, 2),
                new Point(5, 10), new Point(5, 11), new Point(6, 9), new Point(6, 12), new Point(7, 10), new Point(7, 11),
                new Point(9, 6), new Point(10, 5), new Point(10, 7), new Point(11, 5), new Point(11, 7), new Point(12, 6),
            };
            presets[3] = new Preset(preset3, 13, 13);

            //{1, 1, 0, 0}
            //{1, 1, 0, 0}
            //{0, 0, 1, 1}
            //{0, 0, 1, 1}

            Point[] preset4 = new Point[] {
                new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1),
                new Point(2, 2), new Point(2, 3), new Point(3, 2), new Point(3, 3),
            };
            presets[4] = new Preset(preset4, 4, 4);

            //
            // {0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0}
            // {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            // {1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1}
            // {1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1}
            // {1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1}
            // {0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0}
            // {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            // {0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0}
            // {1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1}
            // {1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1}
            // {1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1}
            // {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
            // {0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0}
            //

            Point[] preset5 = new Point[] {
                new Point(0, 2), new Point(0, 3), new Point(0, 4), new Point(2, 0), new Point(3, 0), new Point(4, 0), new Point(5, 2), new Point(5, 3), new Point(5, 4), new Point(2, 5), new Point(3, 5), new Point(4, 5),
                new Point(0, 8), new Point(0, 9), new Point(0, 10), new Point(2, 7), new Point(3, 7), new Point(4, 7), new Point(5, 8), new Point(5, 9), new Point(5, 10), new Point(2, 12), new Point(3, 12), new Point(4, 12),
                new Point(7, 2), new Point(7, 3), new Point(7, 4), new Point(8, 0), new Point(9, 0), new Point(10, 0), new Point(12, 2), new Point(12, 3), new Point(12, 4), new Point(8, 5), new Point(9, 5), new Point(10, 5),
                new Point(7, 8), new Point(7, 9), new Point(7, 10), new Point(8, 7), new Point(9, 7), new Point(10, 7), new Point(12, 8), new Point(12, 9), new Point(12, 10), new Point(8, 12), new Point(9, 12), new Point(10, 12),
            };
            presets[5] = new Preset(preset5, 13, 13);
        }*/

        private static void InitPresets()
        {
            presets = Array.Empty<Preset>();
            string[] files;
            try { files = Directory.GetFiles("presets"); }
            catch { files = Array.Empty<string>(); }

            foreach (string fname in files)
                if (fname.EndsWith(".lfs"))
                    try { LoadPreset(fname); }
                    catch { System.Windows.Forms.MessageBox.Show("Bad preset file: " + fname, "Error"); }

            if (presets.Length == 0)
            {
                presets = Array.Empty<Preset>();
                //presets = new Preset[] { new Preset(new Point[] { new Point() }, 1, 1) };
                System.Windows.Forms.MessageBox.Show("No preset found!", "Error");
            }
        }

        private static void LoadPreset(string f)
        {
            string str = File.ReadAllText(f);
            DumpStruct s = JsonSerializer.Deserialize<DumpStruct>(str);
            Point[] points = new Point[s.P.Length];
            int w = 0, h = 0;
            for (int i = 0; i < s.P.Length; i++)
            {
                points[i] = new Point(s.P[i].X, s.P[i].Y);
                if (s.P[i].X > w) w = s.P[i].X;
                if (s.P[i].Y > h) h = s.P[i].Y;
            }
            Preset preset = new Preset(points, (byte)(h + 1), (byte)(w + 1));
            Preset[] arr = new Preset[presets.Length + 1];
            presets.CopyTo(arr, 0);
            presets = arr;
            presets[^1] = preset;
        }

        public static int PresetNum { get => presets.Length; }

        public static void Paste(int x, int y)
        {
            int left = Math.Min(CopyInfo.first.X, CopyInfo.second.X), top = Math.Min(CopyInfo.first.Y, CopyInfo.second.Y),
                right = Math.Max(CopyInfo.first.X, CopyInfo.second.X), bottom = Math.Max(CopyInfo.first.Y, CopyInfo.second.Y),
                width = right - left + 1, height = bottom - top + 1;
            Point[] points = new Point[width * height];
            Head px = cur.next; int pos = 0;
            while (px != null && px.x < left) px = px.next;
            while (px != null && px.x <= right)
            {
                Node py = px.node;
                while (py != null && py.y < top) py = py.next;
                while (py != null && py.y <= bottom)
                {
                    points[pos++] = new Point(px.x - left, py.y - top);
                    py = py.next;
                }
                px = px.next;
            }

            bool conflict = false;
            px = cur.next;
            while (px != null && px.x < x) px = px.next;
            while (px != null && px.x <= x + width)
            {
                Node py = px.node;
                while (py != null && py.y < y) py = py.next;
                if (py != null && py.y <= y + height) { conflict = true; break; }
                px = px.next;
            }

            if (conflict)
            {
                new CopyConflict().ShowDialog();
                switch (CopyInfo.copyState)
                {
                    case CopyState.cancel:
                        {
                            return;
                        }
                    case CopyState.merge:
                        {
                            break;
                        }
                    case CopyState.replace:
                        {
                            px = cur.next;
                            while (px != null && px.x < x) px = px.next;
                            while (px != null && px.x <= x + width)
                            {
                                Node py = px.node;
                                while (py.next != null && py.next.y < y) py = py.next;
                                while (py.next != null && py.next.y <= y + height) Del(py);
                                px = px.next;
                            }
                            break;
                        }
                }
            }
            for (int i = 0; i < pos; i++) Change(x + points[i].X, y + points[i].Y, 1);
        }
    }
}