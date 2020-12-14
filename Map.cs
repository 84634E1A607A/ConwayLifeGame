using System;

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
        public class Point
        {
            public int x;
            public int y;
            public Point() { x = y = 0; }
            public Point(int x, int y) { this.x = x; this.y = y; }
        }
        public class Builtin
        {
            public Point[] points;
            public byte size;
            public byte height;
            public byte width;
            public Builtin() { size = 0; height = 0; width = 0; }
            public Builtin(Point[] points, byte height, byte width)
            {
                this.points = points; this.size = (byte)points.Length; this.height = height; this.width = width;
            }
        }
        private static Head cur = new Head();
        private static Head nxt = new Head();
        private static Builtin[] builtins;
        public enum AddRegionState
        {
            insert,
            delete,
            random
        };
        public static AddRegionState add_region_state;
        public static int selected_builtin, selected_direction, x_pivot, y_pivot, timer, scale;
        public static bool started;
        private static Head Add(int xpos, int ypos, Head acce)
        {
            Head px = nxt;
            if (acce.x <= xpos) px = acce;
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
        public static Head Change(int xpos, int ypos, int type = 0, Head acce = null)     //type: {0: 1.0, 0.1; 1: 0,1.1; 2: 0,1.0}
        {
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
                    Change(x, y, 1);
                }
                px = px.next;
            }
            Clear(cur);
            px = nxt.next;
            while (px != null)
            {
                Node py = px.node.next;
                for (; py != null; py = py.next) if ((py.count == 3) || py.state && py.count == 4) Change(px.x, py.y);
                px = px.next;
            }
            Clear(nxt);
        }
        //public void DrawBuiltin();
        //public void Load(string f);
        //public void Dump(string f);
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
            selected_builtin = selected_direction = 0;
            x_pivot = y_pivot = 0x08000000;
            timer = 100;
            scale = 10;
            Main.control.MapReset();
            Clear(cur);
            cur.next = null; nxt.next = null;
        }
        private static void Clear(Head h)
        {
            h.next = null;
        }
        public static void AddBuiltin(int xpos, int ypos, byte b = 0xff, byte d = 0xff)
        {
            if (b >= 10 || d >= 8) return;
            byte s = builtins[b].size, l = (byte)(builtins[b].width - 1), h = (byte)(builtins[b].height - 1);
            Point[] cur = builtins[b].points;
            switch (d)
            {
                case 0:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].x, ypos + cur[i].y, 1);
                    break;
                case 1:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].x, ypos + h - cur[i].y, 1);
                    break;
                case 2:
                    for (byte i = 0; i < s; i++) Change(xpos + l - cur[i].x, ypos + cur[i].y, 1);
                    break;
                case 3:
                    for (byte i = 0; i < s; i++) Change(xpos + l - cur[i].x, ypos + h - cur[i].y, 1);
                    break;
                case 4:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].y, ypos + cur[i].x, 1);
                    break;
                case 5:
                    for (byte i = 0; i < s; i++) Change(xpos + cur[i].y, ypos + l - cur[i].x, 1);
                    break;
                case 6:
                    for (byte i = 0; i < s; i++) Change(xpos + h - cur[i].y, ypos + cur[i].x, 1);
                    break;
                case 7:
                    for (byte i = 0; i < s; i++) Change(xpos + h - cur[i].y, ypos + l - cur[i].x, 1);
                    break;
            }
        }
        public static void AddDeleteRegion(int left, int top, int right, int bottom, AddRegionState? state = null)
        {
            if (state == null) state = add_region_state;
            Head acce = null;
            switch (state)
            {
                case AddRegionState.random:
                    {
                        Random r = new Random();
                        for (int x = left; x <= right; x++)
                            for (int y = bottom; y >= top; y--)
                                acce = Change(x, y, r.Next(0, 2), acce);
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
        public static Builtin GetBulitinInfo(int b = -1)
        {
            if (b == -1) b = selected_builtin;
            try { return builtins[b]; }
            catch (Exception) { return null; }
        }
        public static void InitBuiltins()
        {
            builtins = new Builtin[6];

            //{1,1,1},
            //{1,0,0},
            //{0,1,0}

            Point[] builtin0 = new Point[] {
                new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 2), new Point(2, 0)
            };
            builtins[0] = new Builtin(builtin0, 3, 3);

            //{0,1,0,0,1},
            //{1,0,0,0,0},
            //{1,0,0,0,1},
            //{1,1,1,1,0}	

            Point[] builtin1 = new Point[] {
                new Point(0, 1), new Point(0, 2), new Point(0, 3), new Point(1, 0), new Point(1, 3), new Point(2, 3), new Point(3, 3), new Point(4, 0), new Point(4, 2),
            };
            builtins[1] = new Builtin(builtin1, 4, 5);

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

            Point[] builtin2 = new Point[] {
                new Point(0, 4), new Point(0, 5), new Point(1, 4), new Point(1, 5),
                new Point(10, 4), new Point(10, 5), new Point(10, 6), new Point(11, 3), new Point(11, 7), new Point(12, 2), new Point(12, 8), new Point(13, 2), new Point(13, 8),
                new Point(14, 5), new Point(15, 3), new Point(15, 7), new Point(16, 4), new Point(16, 5), new Point(16, 6), new Point(17, 5),
                new Point(20, 2), new Point(20, 3), new Point(20, 4), new Point(21, 2), new Point(21, 3), new Point(21, 4), new Point(22, 1), new Point(22, 5),
                new Point(24, 0), new Point(24, 1), new Point(24, 5), new Point(24, 6),
                new Point(34, 2), new Point(34, 3), new Point(35, 2), new Point(35, 3),
            };
            builtins[2] = new Builtin(builtin2, 9, 36);

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

            Point[] builtin3 = new Point[] {
                new Point(0, 6), new Point(1, 5), new Point(1, 7), new Point(2, 5), new Point(2, 7), new Point(3, 6),
                new Point(5, 1), new Point(5, 2), new Point(6, 0), new Point(6, 3), new Point(7, 1), new Point(7, 2),
                new Point(5, 10), new Point(5, 11), new Point(6, 9), new Point(6, 12), new Point(7, 10), new Point(7, 11),
                new Point(9, 6), new Point(10, 5), new Point(10, 7), new Point(11, 5), new Point(11, 7), new Point(12, 6),
            };
            builtins[3] = new Builtin(builtin3, 13, 13);

            //{1, 1, 0, 0}
            //{1, 1, 0, 0}
            //{0, 0, 1, 1}
            //{0, 0, 1, 1}

            Point[] builtin4 = new Point[] {
                new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1),
                new Point(2, 2), new Point(2, 3), new Point(3, 2), new Point(3, 3),
            };
            builtins[4] = new Builtin(builtin4, 4, 4);

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

            Point[] builtin5 = new Point[] {
                new Point(0, 2), new Point(0, 3), new Point(0, 4), new Point(2, 0), new Point(3, 0), new Point(4, 0), new Point(5, 2), new Point(5, 3), new Point(5, 4), new Point(2, 5), new Point(3, 5), new Point(4, 5),
                new Point(0, 8), new Point(0, 9), new Point(0, 10), new Point(2, 7), new Point(3, 7), new Point(4, 7), new Point(5, 8), new Point(5, 9), new Point(5, 10), new Point(2, 12), new Point(3, 12), new Point(4, 12),
                new Point(7, 2), new Point(7, 3), new Point(7, 4), new Point(8, 0), new Point(9, 0), new Point(10, 0), new Point(12, 2), new Point(12, 3), new Point(12, 4), new Point(8, 5), new Point(9, 5), new Point(10, 5),
                new Point(7, 8), new Point(7, 9), new Point(7, 10), new Point(8, 7), new Point(9, 7), new Point(10, 7), new Point(12, 8), new Point(12, 9), new Point(12, 10), new Point(8, 12), new Point(9, 12), new Point(10, 12),
            };
            builtins[5] = new Builtin(builtin5, 13, 13);
        }
    }
}