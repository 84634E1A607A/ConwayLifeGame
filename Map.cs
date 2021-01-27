using System;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

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
            public System.Drawing.Point[] points;
            public byte height;
            public byte width;
            public string name;
            public Preset() { height = 0; width = 0; name = ""; }
            public Preset(in System.Drawing.Point[] points, in byte height, in byte width, string name = "")
            {
                this.points = points; this.height = height; this.width = width; this.name = name;
            }
        }
        
        private readonly static Head cur = new Head();
        private static List<Preset> presets;
        
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
            preset,
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
            public System.Drawing.Point previous;          // Rleative to Map
            public System.Drawing.Point select_first;      // Relative to Window
            public System.Drawing.Point select_second;     // Relative to Window
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
            public System.Drawing.Point first;
            public System.Drawing.Point second;
            public CopyState copyState;
        }
        public static CopyInformation CopyInfo;

        private static int _selectedPreset = 1, _selectedDirection = 1, _xPivot = 0x08000000, _yPivot = 0x08000000, _timer = 100, _scale = 10;
        public static int SelectedPreset { get => _selectedPreset; set => _selectedPreset = value; }
        public static int SelectedDirection { get => _selectedDirection; set => _selectedDirection = value; }
        public static int XPivot { get => _xPivot; set => _xPivot = value; }
        public static int YPivot { get => _yPivot; set => _yPivot = value; }
        public static int Timer { get => _timer; set => _timer = value; }
        public static int Scale { get => _scale; set => _scale = value; }

        private static bool _started, _calculating;
        public static bool Started { get => _started; set => _started = value; }
        public static bool Calculating { get => _calculating; }

        private static Head Add(in Head nxt, in int xpos, in int ypos, in Head acce)
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
        private static Head Change(in int xpos, in int ypos, Head head, int type = 0, Head acce = null)
        {
            if (xpos <= 0 || ypos <= 0) return null;
            Head px = head ?? cur;
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

        public static void Change(in int xpos, in int ypos)
        {
            if (_calculating) { } // Todo
            else
            {
                if (xpos <= 0 || ypos <= 0) return;
                Head px = cur;
                while (px.next != null && px.next.x <= xpos) px = px.next;
                if (px.x == xpos && px.node != null)
                {
                    Node py = px.node;
                    while (py.next != null && py.next.y < ypos) py = py.next;
                    if (py.next != null && py.next.y == ypos)
                        Del(py);                            //If the Node already exists: destroy the Node
                    else
                    {                                       //If the Node doesn't exist: insert a Node
                        Node pn = Insert(py);
                        pn.y = ypos;
                        pn.state = true;
                    }
                }
                else
                {                                           //If the row doesn't exist: insert Head and Node
                    Head pn = Insert(px);
                    Node pnode = Insert(pn.node);
                    pn.x = xpos;
                    pnode.y = ypos;
                    pnode.state = true;
                }
            }
        }

        public static void ChangeNxt(in Head nxt, in int xpos, in int ypos) 
        {
            Head px = nxt;
            while (px.x < xpos) px = px.next;
            Node py = px.node.next;
            while (py.y < ypos) py = py.next;
            py.state = true;
        }

        public static void Calculate()
        {
            if (_calculating) return;
            _calculating = true;
            Head nxt = new Head();
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
                    ptmp = Add(nxt, x - 1, y - 1, ptmp);
                    pacce = ptmp;
                    Add(nxt, x - 1, y, ptmp);
                    Add(nxt, x - 1, y + 1, ptmp);
                    ptmp = Add(nxt, x, y - 1, ptmp);
                    Add(nxt, x, y, ptmp);
                    Add(nxt, x, y + 1, ptmp);
                    ptmp = Add(nxt, x + 1, y - 1, ptmp);
                    Add(nxt, x + 1, y, ptmp);
                    Add(nxt, x + 1, y + 1, ptmp);
                    ChangeNxt(nxt, x, y);
                }
                px = px.next;
            }
            Head calced = new Head();
            px = nxt.next;
            while (px != null)
            {
                Node py = px.node.next;
                for (; py != null; py = py.next) if ((py.count == 3) || (py.state && py.count == 4)) Change(px.x, py.y, head: calced);
                px = px.next;
            }
            cur.next = calced.next;
            _calculating = false;
        }

        public static void LoadLF(in string f)
        {
            // Stop
            Program.control.Reset_Click(null, null);
            FileStream fs = new FileStream(f, FileMode.Open);
            BinaryReader reader = new BinaryReader(fs);
            _xPivot = reader.ReadInt32();
            _yPivot = reader.ReadInt32();
            if (reader.ReadUInt32() != 0xffffffff)
                throw new IOException("Unexpected sign of file");
            while (true)
            {
                uint x, y;
                x = reader.ReadUInt32();
                y = reader.ReadUInt32();
                if (x == 0xffffffff && y == 0xfffffffd) break;
                Change((int)x, (int)y, cur);
            }
        }

        private class DumpStruct
        {
            public struct Point
            {
                public int X { get; set; }
                public int Y { get; set; }
                public Point(in int x, in int y) { X = x; Y = y; }
            }
            public Point[] P { get; set; }
            public int Xp { get; set; }
            public int Yp { get; set; }
            public int S { get; set; }
        }

        public static void LoadLFS(in string f) 
        {
            Program.control.Reset_Click(null, null);
            
            string str = File.ReadAllText(f);
            DumpStruct s = JsonSerializer.Deserialize<DumpStruct>(str);

            _xPivot = s.Xp; _yPivot = s.Yp; _scale = s.S;
            foreach (DumpStruct.Point p in s.P) { Change(p.X + _xPivot, p.Y + _yPivot, cur); }
        }

        public static void DumpLFS(in string f)
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

        private static class PaintTools {
            public static SolidColorBrush brush;
        }

        public static void Draw(in RenderTarget renderTarget)
        {
            int mid_x = Program.main.MainPanel.Width / 2, mid_y = Program.main.MainPanel.Height / 2;
            int left = (-mid_x) / _scale + _xPivot - 1, right = mid_x / _scale + _xPivot + 1;
            int top = (-mid_y) / _scale + _yPivot - 1, bottom = mid_y / _scale + _yPivot + 1;
            Head pl = cur;
            while (pl.next != null && pl.next.x < left) pl = pl.next;
            Head px = pl;
            if (PaintTools.brush == null) { PaintTools.brush = new SolidColorBrush(renderTarget, new RawColor4(0, 0, 0, 1)); }
            while (px.next != null && px.next.x <= right)
            {
                Node py = px.next.node;
                while (py.next != null && py.next.y < top) py = py.next;
                while (py.next != null && py.next.y <= bottom)
                {
                    renderTarget.FillRectangle(new RawRectangleF(
                        (px.next.x - _xPivot) * _scale + 1 + mid_x,
                        (py.next.y - _yPivot) * _scale + 1 + mid_y,
                        (px.next.x - _xPivot + 1) * _scale + mid_x,
                        (py.next.y - _yPivot + 1) * _scale + mid_y
                        ), PaintTools.brush);
                    py = py.next;
                }
                px = px.next;
            }
        }

        private static Head Insert(in Head p)
        {
            Node node = new Node();
            Head pn = new Head { next = p.next, node = node };
            p.next = pn;
            return pn;
        }

        private static Node Insert(in Node p)
        {
            Node pn = new Node { next = p.next };
            p.next = pn;
            return pn;
        }

        private static void Del(in Node p)
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

        private static void Clear(in Head h)
        {
            h.next = null;
        }

        public static void AddPreset(in int xpos, in int ypos)
        {
            byte s = (byte)presets[_selectedPreset - 1].points.Length, l = (byte)(presets[_selectedPreset - 1].width - 1), h = (byte)(presets[_selectedPreset - 1].height - 1);
            System.Drawing.Point[] cur = presets[_selectedPreset - 1].points;
            switch (_selectedDirection)
            {
                case 1:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].X, ypos + cur[i].Y, Map.cur, 1);
                    break;
                case 2:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].X, ypos + h - cur[i].Y, Map.cur, 1);
                    break;
                case 3:
                    for (byte i = 0; i < s; i++) Change(xpos + l - cur[i].X, ypos + cur[i].Y, Map.cur, 1);
                    break;
                case 4:
                    for (byte i = 0; i < s; i++) Change(xpos + l - cur[i].X, ypos + h - cur[i].Y, Map.cur, 1);
                    break;
                case 5:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].Y, ypos + cur[i].X, Map.cur, 1);
                    break;
                case 6:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].Y, ypos + l - cur[i].X, Map.cur, 1);
                    break;
                case 7:
                    for (byte i = 0; i < s; i++) Change(xpos + h - cur[i].Y, ypos + cur[i].X, Map.cur, 1);
                    break;
                case 8:
                    for (byte i = 0; i < s; i++) Change(xpos + h - cur[i].Y, ypos + l - cur[i].X, Map.cur, 1);
                    break;
            }
        }

        public static void AddDeleteRegion(in System.Drawing.Point p1, in System.Drawing.Point p2)
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
                                    acce = Change(x, y, cur, 1, acce);
                        break;
                    }
                case AddRegionState.insert:
                    {
                        for (int x = left; x <= right; x++)
                            for (int y = bottom; y >= top; y--)
                                acce = Change(x, y, cur, 1, acce);
                        break;
                    }

                case AddRegionState.delete:
                    {
                        for (int x = left; x <= right; x++)
                            for (int y = bottom; y >= top; y--)
                                acce = Change(x, y, cur, 2, acce);
                        break;
                    }
                default: break;
            }
        }

        public static Preset GetBulitinInfo()
        {
            try { return presets[_selectedPreset - 1]; }
            catch (Exception) { return presets[0]; }
        }

        public static void Initialize()
        {
            InitPresets();
        }

        private static void InitPresets()
        {
            presets = new List<Preset>();
            string[] files;
            try { files = Directory.GetFiles("presets"); }
            catch { files = Array.Empty<string>(); }

            foreach (string fname in files)
                if (fname.EndsWith(".lfs"))
                    try { LoadPreset(fname); }
                    catch { System.Windows.Forms.MessageBox.Show("Bad preset file: " + fname, "Error"); }

            if (presets.Count == 0)
            {
                presets.Add(new Preset(new System.Drawing.Point[] { new System.Drawing.Point() }, 1, 1));
                System.Windows.Forms.MessageBox.Show("No preset found!", "Error");
            }
        }

        private static void LoadPreset(in string f)
        {
            string str = File.ReadAllText(f);
            DumpStruct s = JsonSerializer.Deserialize<DumpStruct>(str);
            System.Drawing.Point[] points = new System.Drawing.Point[s.P.Length];
            int w = 0, h = 0;
            for (int i = 0; i < s.P.Length; i++)
            {
                points[i] = new System.Drawing.Point(s.P[i].X, s.P[i].Y);
                if (s.P[i].X > w) w = s.P[i].X;
                if (s.P[i].Y > h) h = s.P[i].Y;
            }
            presets.Add(new Preset(points, (byte)(h + 1), (byte)(w + 1)));
        }

        public static int PresetNum { get => presets.Count; }

        public static void Paste(in int x, in int y)
        {
            int left = Math.Min(CopyInfo.first.X, CopyInfo.second.X), top = Math.Min(CopyInfo.first.Y, CopyInfo.second.Y),
                right = Math.Max(CopyInfo.first.X, CopyInfo.second.X), bottom = Math.Max(CopyInfo.first.Y, CopyInfo.second.Y),
                width = right - left + 1, height = bottom - top + 1;
            System.Drawing.Point[] points = new System.Drawing.Point[width * height];
            Head px = cur.next; int pos = 0;
            while (px != null && px.x < left) px = px.next;
            while (px != null && px.x <= right)
            {
                Node py = px.node;
                while (py != null && py.y < top) py = py.next;
                while (py != null && py.y <= bottom)
                {
                    points[pos++] = new System.Drawing.Point(px.x - left, py.y - top);
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
                            for (int i = 0; i < pos; i++) Change(x + points[i].X, y + points[i].Y, cur, 1);
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
        }
    }
}