using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace WinFormsApp1
{
    /// <summary>
    /// Maquette visuelle : ville factice 20x20 en vue isométrique ("2D et demi"), rendue en GDI+.
    /// Aucune donnée réelle : tout est généré localement pour évaluer le rendu.
    /// </summary>
    public partial class Pedaloland : Form
    {
        private const int GridSize = 20;
        private const int LotSize = 3;

        private enum Ground { Grass, Road, Water, Lot }

        private record Factory(string Name, int Parcel, int X, int Y, Color Color);

        private static readonly (int Dx, int Dy)[] Neighbours = [(1, 0), (-1, 0), (0, 1), (0, -1)];

        private readonly Ground[,] _ground = new Ground[GridSize, GridSize];
        private readonly float[,] _treeSize = new float[GridSize, GridSize];
        private readonly float[,] _grassShade = new float[GridSize, GridSize];
        private readonly List<Factory> _factories =
        [
            new("Usine A", 1, 3, 3, Color.FromArgb(214, 96, 77)),
            new("Usine B", 2, 14, 3, Color.FromArgb(77, 140, 214)),
            new("Usine C", 3, 3, 14, Color.FromArgb(232, 178, 62)),
            new("Usine D", 4, 14, 14, Color.FromArgb(156, 104, 204)),
        ];

        private readonly System.Windows.Forms.Timer _timer = new() { Interval = 33 };
        private readonly Stopwatch _clock = Stopwatch.StartNew();
        private float _time;
        private double _renderMs;

        // Paramètres de projection, recalculés à chaque redimensionnement
        private float _tileW, _tileH, _originX, _originY;

        public Pedaloland()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            KeyPreview = true;

            KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Close(); };
            Resize += (_, _) => { ComputeLayout(); Invalidate(); };
            FormClosed += (_, _) => _timer.Dispose();
            _timer.Tick += (_, _) => { _time = (float)_clock.Elapsed.TotalSeconds; Invalidate(); };

            BuildCity();
            ComputeLayout();
            _timer.Start();
        }

        #region Génération de la ville factice

        private void BuildCity()
        {
            // Axes principaux : une croix qui traverse la ville
            for (int i = 0; i < GridSize; i++)
            {
                _ground[i, 9] = Ground.Road;
                _ground[9, i] = Ground.Road;
            }

            // Bretelles reliant chaque usine à la croix
            for (int y = 6; y <= 8; y++) { _ground[4, y] = Ground.Road; _ground[15, y] = Ground.Road; }
            for (int y = 10; y <= 13; y++) { _ground[4, y] = Ground.Road; _ground[15, y] = Ground.Road; }

            // Un lac, indispensable pour tester des pédalos
            for (int x = 10; x <= 12; x++)
                for (int y = 13; y <= 17; y++)
                    _ground[x, y] = Ground.Water;
            _ground[11, 12] = _ground[12, 12] = _ground[13, 15] = _ground[13, 16] = Ground.Water;

            // Parcelles des usines
            foreach (var f in _factories)
                for (int dx = 0; dx < LotSize; dx++)
                    for (int dy = 0; dy < LotSize; dy++)
                        _ground[f.X + dx, f.Y + dy] = Ground.Lot;

            // Végétation aléatoire (graine fixe pour un rendu reproductible)
            var rng = new Random(7);
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                {
                    _grassShade[x, y] = 0.92f + (float)rng.NextDouble() * 0.12f;
                    bool tree = rng.NextDouble() < 0.22;
                    if (tree && _ground[x, y] == Ground.Grass && !IsNearLot(x, y))
                        _treeSize[x, y] = 0.6f + (float)rng.NextDouble() * 0.4f;
                }
        }

        private bool IsNearLot(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx, ny = y + dy;
                    if (nx >= 0 && ny >= 0 && nx < GridSize && ny < GridSize && _ground[nx, ny] == Ground.Lot)
                        return true;
                }
            return false;
        }

        #endregion

        #region Projection isométrique

        private void ComputeLayout()
        {
            var size = ClientSize;
            if (size.Width == 0 || size.Height == 0) return;

            // La carte fait 20 tuiles de large et 10 de haut (tuiles 2:1) : on prend le facteur limitant
            _tileW = Math.Min(size.Width * 0.9f / GridSize, size.Height * 0.8f * 2 / GridSize);
            _tileH = _tileW / 2;
            _originX = size.Width / 2f;
            _originY = (size.Height - GridSize * _tileH) / 2f + size.Height * 0.03f;
        }

        /// <summary>Coordonnées de grille (gx, gy) + altitude z en pixels → coordonnées écran.</summary>
        private PointF Iso(float gx, float gy, float z = 0) =>
            new(_originX + (gx - gy) * _tileW / 2, _originY + (gx + gy) * _tileH / 2 - z);

        #endregion

        #region Rendu

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_tileW <= 0 || ClientSize.Width == 0 || ClientSize.Height == 0) return;
            long start = Stopwatch.GetTimestamp();

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            DrawSky(g);
            DrawBase(g);
            for (int y = 0; y < GridSize; y++)
                for (int x = 0; x < GridSize; x++)
                    DrawTile(g, x, y);
            foreach (var f in _factories)
                DrawParcelBorder(g, f);
            DrawLakePedalos(g);

            // Objets en relief : algorithme du peintre, du fond (x+y petit) vers l'avant
            var objects = new List<(float Depth, Action Draw)>();
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    if (_treeSize[x, y] > 0)
                    {
                        int tx = x, ty = y;
                        objects.Add((tx + ty + 1, () => DrawTree(g, tx, ty, _treeSize[tx, ty])));
                    }
            foreach (var f in _factories)
                objects.Add((f.X + f.Y + LotSize, () => DrawFactory(g, f)));
            foreach (var o in objects.OrderBy(o => o.Depth))
                o.Draw();

            using (var labelFont = new Font("Segoe UI Semibold", _tileW * 0.17f, GraphicsUnit.Pixel))
                foreach (var f in _factories)
                    DrawLabel(g, f, labelFont);

            double ms = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
            _renderMs = _renderMs * 0.9 + ms * 0.1;
            DrawHud(g);
        }

        private void DrawSky(Graphics g)
        {
            using var sky = new LinearGradientBrush(ClientRectangle,
                Color.FromArgb(28, 38, 64), Color.FromArgb(88, 118, 160), LinearGradientMode.Vertical);
            g.FillRectangle(sky, ClientRectangle);
        }

        /// <summary>Socle de terre sous la carte, pour l'effet "maquette".</summary>
        private void DrawBase(Graphics g)
        {
            float d = _tileH * 0.8f;
            Fill(g, Color.FromArgb(120, 84, 52),
                Iso(0, GridSize), Iso(GridSize, GridSize), Iso(GridSize, GridSize, -d), Iso(0, GridSize, -d));
            Fill(g, Color.FromArgb(92, 62, 38),
                Iso(GridSize, 0), Iso(GridSize, GridSize), Iso(GridSize, GridSize, -d), Iso(GridSize, 0, -d));
        }

        private void DrawTile(Graphics g, int x, int y)
        {
            PointF[] diamond = [Iso(x, y), Iso(x + 1, y), Iso(x + 1, y + 1), Iso(x, y + 1)];
            Color c = _ground[x, y] switch
            {
                Ground.Road => Color.FromArgb(78, 80, 86),
                Ground.Water => WaterColor(x, y),
                Ground.Lot => Color.FromArgb(178, 174, 164),
                _ => Shade(Color.FromArgb(108, 168, 84), _grassShade[x, y]),
            };
            Fill(g, c, diamond);
            using (var edge = new Pen(Shade(c, 0.9f), 1))
                g.DrawPolygon(edge, diamond);

            if (_ground[x, y] == Ground.Road)
                DrawRoadMarkings(g, x, y);
        }

        private Color WaterColor(int x, int y)
        {
            float w = MathF.Sin(_time * 1.5f + x * 0.8f + y * 0.5f);
            return Color.FromArgb(56 + (int)(w * 8), 128 + (int)(w * 10), 196 + (int)(w * 12));
        }

        /// <summary>Marquage central : un segment du centre vers chaque voisin connecté.</summary>
        private void DrawRoadMarkings(Graphics g, int x, int y)
        {
            using var pen = new Pen(Color.FromArgb(220, 235, 225, 170), Math.Max(1f, _tileW / 45)) { DashPattern = [2f, 2f] };
            var center = Iso(x + 0.5f, y + 0.5f);
            foreach (var (dx, dy) in Neighbours)
                if (IsConnected(x + dx, y + dy))
                    g.DrawLine(pen, center, Iso(x + 0.5f + dx * 0.5f, y + 0.5f + dy * 0.5f));
        }

        private bool IsConnected(int x, int y) =>
            x < 0 || y < 0 || x >= GridSize || y >= GridSize || _ground[x, y] is Ground.Road or Ground.Lot;

        private void DrawParcelBorder(Graphics g, Factory f)
        {
            using var pen = new Pen(Color.FromArgb(200, f.Color), Math.Max(1.5f, _tileW / 35)) { DashPattern = [3f, 2f] };
            g.DrawPolygon(pen, [Iso(f.X, f.Y), Iso(f.X + LotSize, f.Y), Iso(f.X + LotSize, f.Y + LotSize), Iso(f.X, f.Y + LotSize)]);
        }

        private void DrawTree(Graphics g, int x, int y, float size)
        {
            var c = Iso(x + 0.5f, y + 0.5f);
            float r = _tileW * 0.17f * size;
            float trunk = _tileW * 0.12f * size;

            using (var shadow = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
                g.FillEllipse(shadow, c.X - r * 0.6f, c.Y - r * 0.35f, r * 2.1f, r * 0.8f);
            using (var bark = new SolidBrush(Color.FromArgb(110, 76, 48)))
                g.FillRectangle(bark, c.X - r * 0.15f, c.Y - trunk, r * 0.3f, trunk);

            float fy = c.Y - trunk - r * 0.8f;
            using (var leaves = new SolidBrush(Color.FromArgb(52, 112, 58)))
                g.FillEllipse(leaves, c.X - r, fy - r, r * 2, r * 2);
            using (var light = new SolidBrush(Color.FromArgb(86, 150, 76)))
                g.FillEllipse(light, c.X - r * 0.75f, fy - r * 0.8f, r * 1.1f, r * 1.1f);
        }

        private void DrawFactory(Graphics g, Factory f)
        {
            float h = _tileW * 0.45f;

            // Hall de production
            float hx0 = f.X + 0.2f, hy0 = f.Y + 0.2f, hx1 = f.X + 2.4f, hy1 = f.Y + 1.8f;
            Fill(g, Color.FromArgb(70, 0, 0, 0),
                Iso(hx1, hy0), Iso(hx1 + 0.5f, hy0 - 0.15f), Iso(hx1 + 0.5f, hy1 - 0.15f), Iso(hx1, hy1));
            DrawBox(g, hx0, hy0, hx1 - hx0, hy1 - hy0, 0, h, f.Color);
            DrawSawtoothRoof(g, hx0, hy0, hx1 - hx0, hy1 - hy0, h, f.Color);

            var glow = Color.FromArgb(255, 236, 160);
            for (int i = 0; i < 5; i++)
            {
                float xa = hx0 + 0.15f + i * 0.42f;
                Fill(g, glow, FaceY(hy1, xa, xa + 0.26f, h * 0.45f, h * 0.75f));
            }
            Fill(g, Color.FromArgb(60, 62, 70), FaceX(hx1, hy0 + 0.5f, hy0 + 1.1f, 0, h * 0.6f));
            Fill(g, glow, FaceX(hx1, hy0 + 1.25f, hy0 + 1.45f, h * 0.45f, h * 0.75f));

            // Cheminée et fumée
            float chimneyH = h * 2;
            DrawBox(g, f.X + 2.55f, f.Y + 0.3f, 0.3f, 0.3f, 0, chimneyH, Color.FromArgb(150, 90, 70));
            DrawSmoke(g, Iso(f.X + 2.7f, f.Y + 0.45f, chimneyH), f.Parcel);

            // Bâtiment administratif
            float ox0 = f.X + 0.2f, oy0 = f.Y + 1.95f, ox1 = f.X + 1.5f, oy1 = f.Y + 2.7f, oh = h * 0.6f;
            DrawBox(g, ox0, oy0, ox1 - ox0, oy1 - oy0, 0, oh, Color.FromArgb(225, 222, 212));
            for (int i = 0; i < 3; i++)
            {
                float xa = ox0 + 0.12f + i * 0.4f;
                Fill(g, Color.FromArgb(120, 170, 210), FaceY(oy1, xa, xa + 0.25f, oh * 0.25f, oh * 0.45f));
                Fill(g, Color.FromArgb(120, 170, 210), FaceY(oy1, xa, xa + 0.25f, oh * 0.6f, oh * 0.8f));
            }
            Fill(g, Color.FromArgb(90, 70, 55), FaceX(ox1, oy0 + 0.25f, oy0 + 0.5f, 0, oh * 0.5f));

            // Stock de pédalos produits (varie dans le temps pour simuler la production)
            int stock = 1 + (int)((_time / 2.5f + f.Parcel) % 4);
            (float X, float Y)[] slots = [(2.0f, 2.25f), (2.5f, 2.25f), (2.0f, 2.65f), (2.5f, 2.65f)];
            for (int i = 0; i < stock; i++)
                DrawPedalo(g, Iso(f.X + slots[i].X, f.Y + slots[i].Y), f.Color, wake: false);
        }

        /// <summary>Toiture en sheds, typique des bâtiments industriels.</summary>
        private void DrawSawtoothRoof(Graphics g, float x0, float y0, float sx, float sy, float z, Color c)
        {
            const int teeth = 3;
            float s = sx / teeth, r = _tileW * 0.18f, y1 = y0 + sy;
            for (int i = 0; i < teeth; i++)
            {
                float a = x0 + i * s, b = a + s;
                Fill(g, Shade(c, 1.1f), Iso(a, y0, z), Iso(b, y0, z + r), Iso(b, y1, z + r), Iso(a, y1, z));
                Fill(g, Shade(c, 0.82f), Iso(a, y1, z), Iso(b, y1, z + r), Iso(b, y1, z));
                Fill(g, Color.FromArgb(150, 200, 230), Iso(b, y0, z), Iso(b, y0, z + r), Iso(b, y1, z + r), Iso(b, y1, z));
            }
        }

        private void DrawSmoke(Graphics g, PointF top, int seed)
        {
            for (int k = 0; k < 6; k++)
            {
                float p = (_time * 0.35f + k / 6f + seed * 0.13f) % 1f;
                float rad = _tileW * (0.06f + p * 0.16f);
                float px = top.X + p * _tileW * 0.35f + MathF.Sin(_time + k) * _tileW * 0.03f;
                float py = top.Y - p * _tileW * 0.9f;
                using var b = new SolidBrush(Color.FromArgb((int)(150 * (1 - p)), 225, 225, 230));
                g.FillEllipse(b, px - rad, py - rad, rad * 2, rad * 2);
            }
        }

        private void DrawLakePedalos(Graphics g)
        {
            for (int i = 0; i < 3; i++)
            {
                float a = _time * 0.25f + i * MathF.Tau / 3;
                float gx = 11.5f + MathF.Cos(a) * 0.9f, gy = 15.5f + MathF.Sin(a) * 1.7f;
                float bob = MathF.Sin(_time * 3 + i) * _tileW * 0.01f;
                DrawPedalo(g, Iso(gx, gy, bob), _factories[i].Color, wake: true);
            }
        }

        private void DrawPedalo(Graphics g, PointF p, Color c, bool wake)
        {
            float w = _tileW * 0.22f, hgt = w * 0.45f;
            if (wake)
                using (var foam = new Pen(Color.FromArgb(120, 255, 255, 255), 1.5f))
                    g.DrawEllipse(foam, p.X - w * 0.7f, p.Y - hgt * 0.3f, w * 1.4f, hgt);

            using (var hull = new SolidBrush(Color.FromArgb(240, 240, 235)))
                g.FillEllipse(hull, p.X - w / 2, p.Y - hgt / 2, w, hgt);
            using (var body = new SolidBrush(c))
                g.FillEllipse(body, p.X - w * 0.35f, p.Y - hgt * 0.9f, w * 0.7f, hgt * 0.9f);
            using (var seat = new SolidBrush(Shade(c, 0.5f)))
                g.FillRectangle(seat, p.X - w * 0.12f, p.Y - hgt * 1.3f, w * 0.24f, hgt * 0.5f);
        }

        private void DrawLabel(Graphics g, Factory f, Font font)
        {
            var anchor = Iso(f.X + 1.5f, f.Y + 1.5f, _tileW * 1.15f);
            string text = $"{f.Name}  ·  parcelle {f.Parcel}";
            var size = g.MeasureString(text, font);
            var rect = new RectangleF(anchor.X - size.Width / 2 - 8, anchor.Y - size.Height / 2 - 3, size.Width + 16, size.Height + 6);

            using var path = RoundedRect(rect, rect.Height / 2);
            using (var bg = new SolidBrush(Color.FromArgb(190, 20, 24, 32)))
                g.FillPath(bg, path);
            using (var border = new Pen(f.Color, 2))
                g.DrawPath(border, path);
            g.DrawString(text, font, Brushes.White, rect.X + 8, rect.Y + 3);
        }

        private void DrawHud(Graphics g)
        {
            using var title = new Font("Segoe UI", _tileW * 0.38f, FontStyle.Bold, GraphicsUnit.Pixel);
            using var small = new Font("Segoe UI", _tileW * 0.18f, GraphicsUnit.Pixel);
            using var dim = new SolidBrush(Color.FromArgb(200, 220, 228, 240));

            g.DrawString("PEDALOLAND", title, Brushes.White, 24, 18);
            g.DrawString($"Tick {(int)(_time * 2)}  ·  {_factories.Count} usines  ·  maillage {GridSize}×{GridSize}  ·  rendu GDI+ {_renderMs:0.0} ms",
                small, dim, 28, 22 + title.Height);

            const string hint = "Échap : quitter";
            var hs = g.MeasureString(hint, small);
            g.DrawString(hint, small, dim, ClientSize.Width - hs.Width - 24, ClientSize.Height - hs.Height - 18);
        }

        #endregion

        #region Primitives

        /// <summary>Parallélépipède : seules les 3 faces visibles (dessus, avant-gauche, avant-droite) sont dessinées.</summary>
        private void DrawBox(Graphics g, float x0, float y0, float sx, float sy, float z, float h, Color c)
        {
            float x1 = x0 + sx, y1 = y0 + sy;
            Fill(g, Shade(c, 0.82f), Iso(x0, y1, z), Iso(x1, y1, z), Iso(x1, y1, z + h), Iso(x0, y1, z + h));
            Fill(g, Shade(c, 0.62f), Iso(x1, y0, z), Iso(x1, y1, z), Iso(x1, y1, z + h), Iso(x1, y0, z + h));
            Fill(g, Shade(c, 1.08f), Iso(x0, y0, z + h), Iso(x1, y0, z + h), Iso(x1, y1, z + h), Iso(x0, y1, z + h));
        }

        /// <summary>Rectangle sur une façade orientée vers +y (avant-gauche).</summary>
        private PointF[] FaceY(float y, float xa, float xb, float za, float zb) =>
            [Iso(xa, y, za), Iso(xb, y, za), Iso(xb, y, zb), Iso(xa, y, zb)];

        /// <summary>Rectangle sur une façade orientée vers +x (avant-droite).</summary>
        private PointF[] FaceX(float x, float ya, float yb, float za, float zb) =>
            [Iso(x, ya, za), Iso(x, yb, za), Iso(x, yb, zb), Iso(x, ya, zb)];

        private static void Fill(Graphics g, Color c, params PointF[] points)
        {
            using var brush = new SolidBrush(c);
            g.FillPolygon(brush, points);
        }

        private static Color Shade(Color c, float factor) =>
            Color.FromArgb(c.A, Clamp(c.R * factor), Clamp(c.G * factor), Clamp(c.B * factor));

        private static int Clamp(float v) => (int)Math.Clamp(v, 0, 255);

        private static GraphicsPath RoundedRect(RectangleF r, float radius)
        {
            float d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        #endregion
    }
}
