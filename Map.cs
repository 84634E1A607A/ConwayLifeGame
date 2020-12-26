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
        
        private static Head cur = new Head();
        private static Head nxt = new Head();
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
        public static AddRegionInfo add_region_info;

        public enum KeyboardInputState
        {
            normal,
            bulitin,
            direction
        }
        public static KeyboardInputState keyboard_input_state;

        public enum MouseState
        {
            click,
            pen,
            eraser,
            drag,
            select
        }
        public struct MouseInfo
        {
            public MouseState state;
            public Point previous;          // Rleative to Map
            public Point select_first;      // Relative to Window
            public Point select_second;     // Relative to Window
        }
        public static MouseInfo mouse_info;

        public enum CopyState
        {
            replace,
            merge
        }
        public struct CopyInfo
        {
            public bool state;
            public Point first;
            public Point second;
        }
        public static CopyInfo copy_info;

        public static int selected_preset, selected_direction, x_pivot = 0x08000000, y_pivot = 0x08000000, timer = 100, scale = 10;
        public static bool started;
        
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
            Head px = cur.next, pacce = null, ptmp = null;
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
                x_pivot = reader.ReadInt32();
                y_pivot = reader.ReadInt32();
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
            public Point[] p { get; set; }
            public int xp { get; set; }
            public int yp { get; set; }
            public int s { get; set; }
        }

        public static void LoadLFS(string f) 
        {
            Program.control.Reset_Click(null, null);
            
            string str = File.ReadAllText(f);
            DumpStruct s = JsonSerializer.Deserialize<DumpStruct>(str);

            x_pivot = s.xp; y_pivot = s.yp; scale = s.s;
            foreach (DumpStruct.Point p in s.p) { Change(p.X + x_pivot, p.Y + y_pivot); }
        }

        public static void DumpLFS(string f)
        {
            if (started) Program.control.StartStop_Click(null, null);

            DumpStruct s = new DumpStruct();

            // Stat
            s.xp = x_pivot; s.yp = y_pivot; s.s = scale;

            // Points
            int c = 0;
            for (Head h = cur; h != null; h = h.next) for (Node n = h.node.next; n != null; n = n.next) c++;
            DumpStruct.Point[] p = new DumpStruct.Point[c];
            c = 0;
            for (Head h = cur; h != null; h = h.next) for (Node n = h.node.next; n != null; n = n.next) { p[c] = new DumpStruct.Point(h.x - x_pivot, n.y - y_pivot); c++; }
            s.p = p;

            // Dump
            FileStream fs = new FileStream(f, FileMode.Create);
            fs.Write(JsonSerializer.SerializeToUtf8Bytes(s));
            fs.Close();
        }

        public static void Draw(Graphics graphics, Size size)
        {
            int mid_x = size.Width / 2, mid_y = size.Height / 2;
            graphics.TranslateTransform(mid_x, mid_y);
            int left = (-mid_x) / scale + x_pivot - 1, right = mid_x / scale + x_pivot + 1;
            int top = (-mid_y) / scale + y_pivot - 1, bottom = mid_y / scale + y_pivot + 1;
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
                    graphics.FillRectangle(brush, (px.next.x - x_pivot) * scale + 1, (py.next.y - y_pivot) * scale + 1, scale - 1, scale - 1);
                    py = py.next;
                }
                px = px.next;
            }
            graphics.ResetTransform();
            brush.Dispose();
        }

        private static Head Insert(Head p)
        {
            Head pn = new Head();
            pn.next = p.next;
            p.next = pn;
            Node node = new Node();
            pn.node = node;
            return pn;
        }

        private static Node Insert(Node p)
        {
            Node pn = new Node();
            pn.next = p.next;
            p.next = pn;
            return pn;
        }

        private static void Del(Node p)
        {
            Node pd = p.next;
            p.next = pd.next;
        }

        private static void Del(Head h)
        {
            Head pd = h.next;
            pd.node = null;
            h.next = pd.next;
        }

        public static void Reset()
        {
            started = false;
            selected_preset = selected_direction = 0;
            x_pivot = y_pivot = 0x08000000;
            timer = 100;
            scale = 10;
            Clear(cur);
        }

        private static void Clear(Head h)
        {
            h.next = null;
        }

        public static void AddPreset(int xpos, int ypos)
        {
            byte s = (byte)presets[selected_preset].points.Length, l = (byte)(presets[selected_preset].width - 1), h = (byte)(presets[selected_preset].height - 1);
            Point[] cur = presets[selected_preset].points;
            switch (selected_direction)
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
            int left = p1.X, top = p1.Y, right = p2.X, bottom = p2.Y;
            Head acce = null;
            switch (add_region_info.state)
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
            if (b == -1) b = selected_preset;
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
            presets = new Preset[0];
            string[] files;
            try
            {
                files = Directory.GetFiles("presets");
            }
            catch
            { files = new string[] { "" }; }

            foreach (string fname in files)
                if (fname.EndsWith(".lfs"))
                    LoadPreset(fname);

            if (presets.Length == 0)
            {
                presets = new Preset[1];
                presets[0] = new Preset(new Point[] { new Point() }, 1, 1);
                System.Windows.Forms.MessageBox.Show("No preset found!", "Error");
            }
        }

        private static void LoadPreset(string f)
        {
            string str = File.ReadAllText(f);
            DumpStruct s = JsonSerializer.Deserialize<DumpStruct>(str);
            Point[] points = new Point[s.p.Length];
            int w = 0, h = 0;
            for (int i = 0; i < s.p.Length; i++)
            {
                points[i] = new Point(s.p[i].X, s.p[i].Y);
                if (s.p[i].X > w) w = s.p[i].X;
                if (s.p[i].Y > h) h = s.p[i].Y;
            }
            Preset preset = new Preset(points, (byte)(h + 1), (byte)(w + 1));
            Preset[] arr = new Preset[presets.Length + 1];
            presets.CopyTo(arr, 0);
            presets = arr;
            presets[presets.Length - 1] = preset;
        }

        public static int GetPresetNum() { return presets.Length; }
    }
}