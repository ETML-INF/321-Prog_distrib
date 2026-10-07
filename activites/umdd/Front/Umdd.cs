using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Front
{
    /// <summary>
    /// Maquette visuelle : "Un Monde De Douceurs", ville factice 40x40 en vue isométrique ("2D et demi"), rendue en GDI+.
    /// Aucune donnée réelle : tout est généré localement pour évaluer le rendu.
    /// </summary>
    public partial class Umdd : Form
    {
        private const int GridSize = 40;
        private const int LotSize = 2;

        /// <summary>Indices des routes (lignes et colonnes) qui quadrillent la ville.</summary>
        private static readonly int[] Roads = [9, 19, 29];

        private enum Ground { Grass, Road, Water, Lot, Garden }

        /// <summary>Boulangerie-pâtisserie (BP) posée sur un lotissement de 2x2 cases.</summary>
        private record Bakery(string Name, int Parcel, int X, int Y, Color Color);

        /// <summary>Camionnette de livraison qui circule en boucle sur une route.</summary>
        private record Van(bool AlongX, int Road, int Direction, float Speed, float Offset, int Bakery);

        private static readonly (int Dx, int Dy)[] Neighbours = [(1, 0), (-1, 0), (0, 1), (0, -1)];

        private readonly Ground[,] _ground = new Ground[GridSize, GridSize];
        private readonly float[,] _treeSize = new float[GridSize, GridSize];
        private readonly float[,] _grassShade = new float[GridSize, GridSize];
        private readonly List<Bakery> _bakeries =
        [
            new("BP A", 1, 5, 7, Color.FromArgb(240, 150, 170)),   // rose bonbon
            new("BP B", 2, 15, 7, Color.FromArgb(140, 205, 170)),  // pistache
            new("BP C", 3, 25, 10, Color.FromArgb(245, 214, 110)), // citron
            new("BP D", 4, 35, 7, Color.FromArgb(180, 150, 215)),  // violette
            new("BP E", 5, 7, 22, Color.FromArgb(205, 150, 100)),  // caramel
            new("BP F", 6, 16, 27, Color.FromArgb(130, 185, 230)), // myrtille
            new("BP G", 7, 30, 24, Color.FromArgb(235, 125, 100)), // framboise
            new("BP H", 8, 10, 33, Color.FromArgb(240, 180, 120)), // abricot
            new("BP I", 9, 3, 17, Color.FromArgb(120, 210, 200)),  // menthe
            new("BP J", 10, 20, 14, Color.FromArgb(165, 115, 85)), // chocolat
            new("BP K", 11, 35, 30, Color.FromArgb(250, 175, 150)),// pêche
            new("BP L", 12, 30, 36, Color.FromArgb(190, 220, 110)),// citron vert
        ];

        private const int VillaCount = 50;

        /// <summary>
        /// Villa sur un lotissement de Size x Size cases (2 ou 3).
        /// Model 0 = chalet, 1 = moderne à toit plat, 2 = villa à toit en croupe avec piscine.
        /// </summary>
        private record Villa(int X, int Y, int Size, int Model, Color Wall, Color Roof);

        private readonly List<Villa> _villas = [];

        // Palettes par modèle : chaque villa tire ses couleurs au hasard dans celles de son modèle
        private static readonly Color[][] VillaWalls =
        [
            [Color.FromArgb(245, 235, 205), Color.FromArgb(240, 215, 150), Color.FromArgb(185, 210, 170), Color.FromArgb(170, 205, 230), Color.FromArgb(240, 180, 160), Color.FromArgb(215, 195, 230)],
            [Color.FromArgb(245, 245, 242), Color.FromArgb(200, 200, 205), Color.FromArgb(95, 98, 108), Color.FromArgb(220, 200, 165), Color.FromArgb(200, 120, 90), Color.FromArgb(120, 160, 175)],
            [Color.FromArgb(225, 185, 110), Color.FromArgb(240, 170, 180), Color.FromArgb(160, 200, 225), Color.FromArgb(170, 220, 190), Color.FromArgb(200, 175, 225), Color.FromArgb(250, 225, 140)],
        ];
        private static readonly Color[][] VillaRoofs =
        [
            [Color.FromArgb(120, 80, 55), Color.FromArgb(150, 55, 50), Color.FromArgb(85, 95, 110), Color.FromArgb(90, 120, 80)],
            [Color.FromArgb(60, 62, 70), Color.FromArgb(235, 235, 230), Color.FromArgb(150, 150, 155)],
            [Color.FromArgb(190, 95, 60), Color.FromArgb(165, 70, 55), Color.FromArgb(110, 115, 130), Color.FromArgb(205, 130, 80)],
        ];
        private readonly List<Van> _vans =
        [
            new(true, 9, +1, 1.6f, 0, 0),
            new(true, 19, -1, 1.3f, 12, 1),
            new(true, 29, +1, 1.1f, 25, 2),
            new(false, 9, +1, 1.4f, 5, 3),
            new(false, 19, -1, 1.7f, 18, 4),
            new(false, 29, +1, 1.2f, 31, 5),
            new(true, 19, +1, 0.9f, 3, 6),
            new(false, 29, -1, 1.5f, 8, 7),
            new(true, 9, -1, 1.25f, 20, 8),
            new(true, 29, -1, 1.45f, 6, 9),
            new(false, 9, -1, 1.05f, 27, 10),
            new(false, 19, +1, 1.35f, 14, 11),
        ];

        private readonly System.Windows.Forms.Timer _timer = new() { Interval = 33 };
        private readonly Stopwatch _clock = Stopwatch.StartNew();
        private float _time;
        private double _renderMs;

        /// <summary>Zones écran des étiquettes au dernier rendu, pour savoir sur quelle BP on clique.</summary>
        private readonly List<(RectangleF Rect, Bakery Bakery)> _labelHits = [];

        // Paramètres de projection, recalculés à chaque redimensionnement
        private float _tileW, _tileH, _originX, _originY;

        public Umdd()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            KeyPreview = true;

            KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) Close(); };
            MouseMove += (_, e) => Cursor = LabelAt(e.Location) is null ? Cursors.Default : Cursors.Hand;
            MouseClick += (_, e) =>
            {
                if (e.Button != MouseButtons.Left || LabelAt(e.Location) is not { } bakery) return;
                using var form = new BakeryForm(LabelText(bakery));
                form.ShowDialog(this);
            };
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
            // Quadrillage de routes
            foreach (int r in Roads)
                for (int i = 0; i < GridSize; i++)
                {
                    _ground[i, r] = Ground.Road;
                    _ground[r, i] = Ground.Road;
                }

            // Un étang dans le parc central
            for (int x = 21; x <= 26; x++)
                for (int y = 21; y <= 26; y++)
                    _ground[x, y] = Ground.Water;
            _ground[21, 21] = _ground[26, 26] = Ground.Grass;
            _ground[27, 23] = _ground[27, 24] = _ground[23, 27] = _ground[24, 20] = Ground.Water;

            // Lotissements des boulangeries-pâtisseries (chacun touche une route)
            foreach (var b in _bakeries)
                for (int dx = 0; dx < LotSize; dx++)
                    for (int dy = 0; dy < LotSize; dy++)
                        _ground[b.X + dx, b.Y + dy] = Ground.Lot;

            var rng = new Random(7); // graine fixe pour un rendu reproductible
            PlaceVillas(rng);

            // Végétation aléatoire
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                {
                    _grassShade[x, y] = 0.92f + (float)rng.NextDouble() * 0.12f;
                    bool tree = rng.NextDouble() < 0.2;
                    if (tree && _ground[x, y] == Ground.Grass && !IsNearBuilding(x, y))
                        _treeSize[x, y] = 0.6f + (float)rng.NextDouble() * 0.4f;
                }
        }

        /// <summary>
        /// Les villas bordent les routes : un lotissement de 2x2 ou 3x3 cases d'herbe qui touche une route,
        /// entouré d'une bande d'herbe libre (pas collé à une BP, une autre villa ou l'étang).
        /// </summary>
        private void PlaceVillas(Random rng)
        {
            var origins = new List<(int X, int Y)>();
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    origins.Add((x, y));

            foreach (var (x, y) in origins.OrderBy(_ => rng.Next()))
            {
                if (_villas.Count == VillaCount) break;

                // Environ 4 villas sur 10 ont un grand terrain ; on se rabat sur 2x2 si le 3x3 ne passe pas
                int size = rng.NextDouble() < 0.4 && CanPlaceVilla(x, y, 3) ? 3 : 2;
                if (!CanPlaceVilla(x, y, size)) continue;

                int model = _villas.Count % 3;
                _villas.Add(new Villa(x, y, size, model,
                    VillaWalls[model][rng.Next(VillaWalls[model].Length)],
                    VillaRoofs[model][rng.Next(VillaRoofs[model].Length)]));
                for (int dx = 0; dx < size; dx++)
                    for (int dy = 0; dy < size; dy++)
                        _ground[x + dx, y + dy] = Ground.Garden;
            }
        }

        private bool CanPlaceVilla(int x, int y, int size)
        {
            if (x + size > GridSize || y + size > GridSize) return false;

            bool touchesRoad = false;
            for (int cx = x - 1; cx <= x + size; cx++)
                for (int cy = y - 1; cy <= y + size; cy++)
                {
                    bool inside = cx >= x && cx < x + size && cy >= y && cy < y + size;
                    bool corner = (cx == x - 1 || cx == x + size) && (cy == y - 1 || cy == y + size);
                    var ground = GroundAt(cx, cy);
                    if (inside && ground != Ground.Grass) return false;
                    if (!inside && ground is Ground.Lot or Ground.Garden or Ground.Water) return false;
                    if (!inside && !corner && ground == Ground.Road) touchesRoad = true;
                }
            return touchesRoad;
        }

        private Ground? GroundAt(int x, int y) =>
            x >= 0 && y >= 0 && x < GridSize && y < GridSize ? _ground[x, y] : null;

        /// <summary>Vrai si une case voisine est occupée par une BP ou une villa (on n'y plante pas d'arbre).</summary>
        private bool IsNearBuilding(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx, ny = y + dy;
                    if (nx >= 0 && ny >= 0 && nx < GridSize && ny < GridSize && _ground[nx, ny] is Ground.Lot or Ground.Garden)
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

            // La carte fait GridSize tuiles de large et GridSize/2 de haut (tuiles 2:1) : on prend le facteur limitant
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
            foreach (var b in _bakeries)
                DrawParcelBorder(g, b);

            // Objets en relief : algorithme du peintre, du fond (x+y petit) vers l'avant
            var objects = new List<(float Depth, Action Draw)>();
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    if (_treeSize[x, y] > 0)
                    {
                        int tx = x, ty = y;
                        objects.Add((tx + ty + 1, () => DrawTree(g, tx, ty, _treeSize[tx, ty])));
                    }
            foreach (var b in _bakeries)
                objects.Add((b.X + b.Y + LotSize, () => DrawBakery(g, b)));
            foreach (var v in _villas)
                objects.Add((v.X + v.Y + v.Size, () => DrawVilla(g, v)));
            foreach (var v in _vans)
            {
                var (gx, gy) = VanPosition(v);
                objects.Add((gx + gy, () => DrawVan(g, v, gx, gy)));
            }
            foreach (var o in objects.OrderBy(o => o.Depth))
                o.Draw();

            _labelHits.Clear();
            using (var labelFont = new Font("Segoe UI Semibold", Math.Max(10f, _tileW * 0.3f), GraphicsUnit.Pixel))
                foreach (var b in _bakeries)
                    DrawLabel(g, b, labelFont);

            double ms = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
            _renderMs = _renderMs * 0.9 + ms * 0.1;
            DrawHud(g);
        }

        private void DrawSky(Graphics g)
        {
            using var sky = new LinearGradientBrush(ClientRectangle,
                Color.FromArgb(58, 40, 70), Color.FromArgb(214, 160, 170), LinearGradientMode.Vertical);
            g.FillRectangle(sky, ClientRectangle);
        }

        /// <summary>Socle sous la carte, façon gâteau en couches, pour l'effet "maquette".</summary>
        private void DrawBase(Graphics g)
        {
            float d = _tileH * 1.6f;
            (float From, float To, Color Left, Color Right)[] layers =
            [
                (0, 0.35f, Color.FromArgb(250, 240, 225), Color.FromArgb(225, 212, 196)), // crème
                (0.35f, 0.65f, Color.FromArgb(150, 90, 60), Color.FromArgb(120, 70, 46)), // chocolat
                (0.65f, 1f, Color.FromArgb(222, 180, 120), Color.FromArgb(190, 150, 96)), // biscuit
            ];
            foreach (var (from, to, left, right) in layers)
            {
                Fill(g, left, Iso(0, GridSize, -d * from), Iso(GridSize, GridSize, -d * from),
                    Iso(GridSize, GridSize, -d * to), Iso(0, GridSize, -d * to));
                Fill(g, right, Iso(GridSize, 0, -d * from), Iso(GridSize, GridSize, -d * from),
                    Iso(GridSize, GridSize, -d * to), Iso(GridSize, 0, -d * to));
            }
        }

        private void DrawTile(Graphics g, int x, int y)
        {
            PointF[] diamond = [Iso(x, y), Iso(x + 1, y), Iso(x + 1, y + 1), Iso(x, y + 1)];
            Color c = _ground[x, y] switch
            {
                Ground.Road => Color.FromArgb(84, 82, 88),
                Ground.Water => WaterColor(x, y),
                Ground.Lot => Color.FromArgb(206, 192, 170),
                Ground.Garden => Shade(Color.FromArgb(140, 196, 104), _grassShade[x, y]),
                _ => Shade(Color.FromArgb(118, 176, 92), _grassShade[x, y]),
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
            return Color.FromArgb(70 + (int)(w * 8), 150 + (int)(w * 10), 200 + (int)(w * 12));
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

        private void DrawParcelBorder(Graphics g, Bakery b)
        {
            using var pen = new Pen(Color.FromArgb(220, b.Color), Math.Max(1.5f, _tileW / 25)) { DashPattern = [3f, 2f] };
            g.DrawPolygon(pen, [Iso(b.X, b.Y), Iso(b.X + LotSize, b.Y), Iso(b.X + LotSize, b.Y + LotSize), Iso(b.X, b.Y + LotSize)]);
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

        private void DrawBakery(Graphics g, Bakery b)
        {
            float h = _tileW * 0.6f;

            // Boutique-fournil : murs pastel et toit à deux pans
            float x0 = b.X + 0.2f, y0 = b.Y + 0.15f, x1 = b.X + 1.8f, y1 = b.Y + 1.25f;
            Fill(g, Color.FromArgb(70, 0, 0, 0),
                Iso(x1, y0), Iso(x1 + 0.35f, y0 - 0.1f), Iso(x1 + 0.35f, y1 - 0.1f), Iso(x1, y1));
            DrawBox(g, x0, y0, x1 - x0, y1 - y0, 0, h, b.Color);

            var glow = Color.FromArgb(255, 232, 170);
            Fill(g, glow, FaceY(y1, x0 + 0.15f, x0 + 0.85f, h * 0.12f, h * 0.5f));                 // vitrine
            Fill(g, Color.FromArgb(120, 78, 52), FaceY(y1, x0 + 1.0f, x0 + 1.35f, 0, h * 0.55f));   // porte
            Fill(g, glow, FaceX(x1, y0 + 0.3f, y0 + 0.75f, h * 0.3f, h * 0.65f));                   // fenêtre du fournil

            DrawAwning(g, x0 + 0.05f, x1 - 0.05f, y1, h * 0.62f, b.Color);

            float ridge = _tileW * 0.28f, yMid = (y0 + y1) / 2;
            DrawGableRoof(g, x0, y0, x1, y1, h, ridge, Color.FromArgb(176, 92, 70));

            // Cheminée du four à bois, posée sur le pan arrière, et sa fumée
            float cx = x0 + 1.15f, cy = y0 + 0.15f, cs = 0.18f;
            float roofZ = h + ridge * (cy + cs - y0) / (yMid - y0);
            float chimneyTop = h + ridge * 1.6f;
            DrawBox(g, cx, cy, cs, cs, roofZ, chimneyTop - roofZ, Color.FromArgb(200, 190, 180));
            DrawSmoke(g, Iso(cx + cs / 2, cy + cs / 2, chimneyTop), b.Parcel);

            // Petite terrasse : table et parasol
            DrawParasol(g, b.X + 0.45f, b.Y + 1.7f, h, b.Color);

            // Étal de douceurs (le stock varie dans le temps pour simuler la production)
            float tz = h * 0.28f;
            DrawBox(g, b.X + 1.05f, b.Y + 1.5f, 0.75f, 0.3f, 0, tz, Color.FromArgb(170, 120, 80));
            int stock = 1 + (int)((_time / 2.5f + b.Parcel) % 4);
            for (int i = 0; i < stock; i++)
                DrawPastry(g, Iso(b.X + 1.17f + i * 0.17f, b.Y + 1.65f, tz), (i + b.Parcel) % 4);
        }

        private void DrawVilla(Graphics g, Villa v)
        {
            // Toutes les cotes sont exprimées pour un terrain unitaire, puis mises à l'échelle du lotissement
            float S = v.Size, X = v.X, Y = v.Y;
            float h = _tileW * 0.24f * S;
            var glass = Color.FromArgb(150, 200, 230);
            var door = Color.FromArgb(110, 75, 50);

            // Haie autour du jardin
            using (var hedge = new Pen(Color.FromArgb(70, 130, 60), Math.Max(1.5f, _tileW / 18)))
                g.DrawPolygon(hedge, [Iso(X + 0.03f, Y + 0.03f), Iso(X + S - 0.03f, Y + 0.03f), Iso(X + S - 0.03f, Y + S - 0.03f), Iso(X + 0.03f, Y + S - 0.03f)]);

            switch (v.Model)
            {
                case 0: // Chalet : toit à deux pans
                {
                    float x0 = X + 0.2f * S, y0 = Y + 0.25f * S, x1 = X + 0.8f * S, y1 = Y + 0.75f * S;
                    DrawBox(g, x0, y0, x1 - x0, y1 - y0, 0, h, v.Wall);
                    Fill(g, door, FaceY(y1, x0 + 0.08f * S, x0 + 0.2f * S, 0, h * 0.6f));
                    Fill(g, glass, FaceY(y1, x0 + 0.3f * S, x0 + 0.5f * S, h * 0.35f, h * 0.7f));
                    Fill(g, glass, FaceX(x1, y0 + 0.15f * S, y0 + 0.35f * S, h * 0.35f, h * 0.7f));
                    DrawGableRoof(g, x0, y0, x1, y1, h, _tileW * 0.17f * S, v.Roof);
                    break;
                }
                case 1: // Moderne : deux cubes décalés à toit plat, grandes baies vitrées
                {
                    float x0 = X + 0.15f * S, y0 = Y + 0.2f * S, x1 = X + 0.85f * S, y1 = Y + 0.8f * S;
                    float h1 = h * 0.8f, h2 = h * 0.7f;
                    DrawBox(g, x0, y0, x1 - x0, y1 - y0, 0, h1, v.Wall);
                    Fill(g, glass, FaceY(y1, x0 + 0.06f * S, x0 + 0.45f * S, h1 * 0.1f, h1 * 0.8f));
                    Fill(g, Shade(v.Roof, 0.9f), FaceY(y1, x0 + 0.52f * S, x0 + 0.64f * S, 0, h1 * 0.75f));
                    Fill(g, glass, FaceX(x1, y0 + 0.1f * S, y0 + 0.5f * S, h1 * 0.3f, h1 * 0.8f));
                    float ux1 = x0 + 0.42f * S, uy1 = y0 + 0.38f * S, rim = 0.02f * S;
                    DrawBox(g, x0, y0, ux1 - x0, uy1 - y0, h1, h2, v.Wall);
                    Fill(g, glass, FaceY(uy1, x0 + 0.05f * S, ux1 - 0.05f * S, h1 + h2 * 0.25f, h1 + h2 * 0.75f));
                    Fill(g, glass, FaceX(ux1, y0 + 0.08f * S, uy1 - 0.08f * S, h1 + h2 * 0.25f, h1 + h2 * 0.75f));
                    DrawBox(g, x0 - rim, y0 - rim, ux1 - x0 + 2 * rim, uy1 - y0 + 2 * rim, h1 + h2, h * 0.08f, v.Roof); // acrotère
                    break;
                }
                default: // Toit en croupe (pyramidal) et piscine
                {
                    Fill(g, Color.FromArgb(230, 225, 210), Iso(X + 0.58f * S, Y + 0.55f * S), Iso(X + 0.95f * S, Y + 0.55f * S), Iso(X + 0.95f * S, Y + 0.95f * S), Iso(X + 0.58f * S, Y + 0.95f * S));
                    Fill(g, WaterColor(v.X, v.Y), Iso(X + 0.63f * S, Y + 0.6f * S), Iso(X + 0.9f * S, Y + 0.6f * S), Iso(X + 0.9f * S, Y + 0.9f * S), Iso(X + 0.63f * S, Y + 0.9f * S));
                    float x0 = X + 0.1f * S, y0 = Y + 0.1f * S, x1 = X + 0.6f * S, y1 = Y + 0.6f * S, over = 0.05f * S;
                    DrawBox(g, x0, y0, x1 - x0, y1 - y0, 0, h, v.Wall);
                    Fill(g, door, FaceY(y1, x0 + 0.2f * S, x0 + 0.3f * S, 0, h * 0.6f));
                    Fill(g, glass, FaceY(y1, x0 + 0.05f * S, x0 + 0.15f * S, h * 0.35f, h * 0.7f));
                    Fill(g, glass, FaceY(y1, x0 + 0.35f * S, x0 + 0.45f * S, h * 0.35f, h * 0.7f));
                    Fill(g, glass, FaceX(x1, y0 + 0.15f * S, y0 + 0.35f * S, h * 0.35f, h * 0.7f));
                    DrawHipRoof(g, x0 - over, y0 - over, x1 + over, y1 + over, h, _tileW * 0.18f * S, v.Roof);
                    break;
                }
            }
        }

        /// <summary>Toit en croupe : quatre pans qui se rejoignent en un sommet.</summary>
        private void DrawHipRoof(Graphics g, float x0, float y0, float x1, float y1, float z, float r, Color c)
        {
            var apex = Iso((x0 + x1) / 2, (y0 + y1) / 2, z + r);
            Fill(g, Shade(c, 1.15f), Iso(x0, y0, z), Iso(x1, y0, z), apex);
            Fill(g, Shade(c, 1.05f), Iso(x0, y0, z), Iso(x0, y1, z), apex);
            Fill(g, Shade(c, 0.7f), Iso(x1, y0, z), Iso(x1, y1, z), apex);
            Fill(g, Shade(c, 0.92f), Iso(x0, y1, z), Iso(x1, y1, z), apex);
        }

        /// <summary>Toit à deux pans, faîtage parallèle à l'axe x.</summary>
        private void DrawGableRoof(Graphics g, float x0, float y0, float x1, float y1, float z, float r, Color c)
        {
            float yMid = (y0 + y1) / 2, over = 0.06f;
            Fill(g, Shade(c, 1.15f), Iso(x0, y0 - over, z), Iso(x1 + over, y0 - over, z), Iso(x1 + over, yMid, z + r), Iso(x0, yMid, z + r));
            Fill(g, Shade(c, 0.95f), Iso(x0, y1 + over, z), Iso(x1 + over, y1 + over, z), Iso(x1 + over, yMid, z + r), Iso(x0, yMid, z + r));
            Fill(g, Shade(c, 0.6f), Iso(x1, y0, z), Iso(x1, y1, z), Iso(x1, yMid, z + r));

            // Rangées de tuiles sur le pan avant
            using var tiles = new Pen(Shade(c, 0.75f), Math.Max(1f, _tileW / 60));
            for (int i = 1; i < 4; i++)
            {
                float t = i / 4f, ty = y1 + over - t * (y1 + over - yMid);
                g.DrawLine(tiles, Iso(x0, ty, z + t * r), Iso(x1 + over, ty, z + t * r));
            }
        }

        /// <summary>Store rayé au-dessus de la vitrine (façade avant-gauche).</summary>
        private void DrawAwning(Graphics g, float xa, float xb, float y, float z, Color c)
        {
            const int stripes = 7;
            float depth = 0.28f, drop = _tileW * 0.12f, s = (xb - xa) / stripes;
            for (int i = 0; i < stripes; i++)
            {
                float a = xa + i * s, bb = a + s;
                var col = i % 2 == 0 ? Color.FromArgb(250, 248, 240) : Shade(c, 0.9f);
                Fill(g, col, Iso(a, y, z), Iso(bb, y, z), Iso(bb, y + depth, z - drop), Iso(a, y + depth, z - drop));
                Fill(g, Shade(col, 0.85f), Iso(a, y + depth, z - drop), Iso(bb, y + depth, z - drop),
                    Iso(bb, y + depth, z - drop * 1.6f), Iso(a, y + depth, z - drop * 1.6f));
            }
        }

        private void DrawParasol(Graphics g, float gx, float gy, float h, Color c)
        {
            float tableH = h * 0.22f, poleH = h * 0.7f;
            DrawBox(g, gx - 0.12f, gy - 0.12f, 0.24f, 0.24f, 0, tableH, Color.FromArgb(240, 236, 228));
            var top = Iso(gx, gy, poleH);
            using (var pole = new Pen(Color.FromArgb(90, 80, 70), Math.Max(1f, _tileW / 50)))
                g.DrawLine(pole, Iso(gx, gy, tableH), top);
            float w = _tileW * 0.5f, hh = w * 0.3f;
            using (var cloth = new SolidBrush(Shade(c, 1.05f)))
                g.FillPie(cloth, top.X - w / 2, top.Y - hh * 0.6f, w, hh * 1.4f, 180, 180);
            using (var rim = new SolidBrush(Shade(c, 0.8f)))
                g.FillEllipse(rim, top.X - w / 2, top.Y + hh * 0.05f, w, hh * 0.25f);
        }

        /// <summary>Une douceur sur l'étal : croissant, baguette, gâteau ou macaron.</summary>
        private void DrawPastry(Graphics g, PointF p, int kind)
        {
            float s = _tileW * 0.13f;
            switch (kind)
            {
                case 0: // croissant
                    using (var dough = new SolidBrush(Color.FromArgb(226, 160, 70)))
                        g.FillEllipse(dough, p.X - s / 2, p.Y - s * 0.45f, s, s * 0.45f);
                    using (var crust = new Pen(Color.FromArgb(170, 104, 40), 1))
                    {
                        g.DrawLine(crust, p.X - s * 0.15f, p.Y - s * 0.42f, p.X - s * 0.15f, p.Y - s * 0.05f);
                        g.DrawLine(crust, p.X + s * 0.15f, p.Y - s * 0.42f, p.X + s * 0.15f, p.Y - s * 0.05f);
                    }
                    break;
                case 1: // baguette
                    using (var bread = new SolidBrush(Color.FromArgb(200, 140, 72)))
                        g.FillEllipse(bread, p.X - s * 0.7f, p.Y - s * 0.3f, s * 1.4f, s * 0.3f);
                    using (var cut = new Pen(Color.FromArgb(240, 214, 160), 1))
                        for (int k = -1; k <= 1; k++)
                            g.DrawLine(cut, p.X + k * s * 0.35f - s * 0.08f, p.Y - s * 0.22f, p.X + k * s * 0.35f + s * 0.08f, p.Y - s * 0.1f);
                    break;
                case 2: // gâteau
                    using (var sponge = new SolidBrush(Color.FromArgb(240, 150, 180)))
                    {
                        g.FillRectangle(sponge, p.X - s * 0.4f, p.Y - s * 0.6f, s * 0.8f, s * 0.5f);
                        g.FillEllipse(sponge, p.X - s * 0.4f, p.Y - s * 0.25f, s * 0.8f, s * 0.3f);
                    }
                    using (var cream = new SolidBrush(Color.FromArgb(255, 246, 236)))
                        g.FillEllipse(cream, p.X - s * 0.4f, p.Y - s * 0.75f, s * 0.8f, s * 0.3f);
                    using (var cherry = new SolidBrush(Color.FromArgb(200, 30, 50)))
                        g.FillEllipse(cherry, p.X - s * 0.1f, p.Y - s * 0.9f, s * 0.2f, s * 0.2f);
                    break;
                default: // macaron
                    using (var shell = new SolidBrush(Color.FromArgb(160, 210, 150)))
                    {
                        g.FillEllipse(shell, p.X - s * 0.4f, p.Y - s * 0.3f, s * 0.8f, s * 0.3f);
                        g.FillEllipse(shell, p.X - s * 0.4f, p.Y - s * 0.6f, s * 0.8f, s * 0.3f);
                    }
                    using (var filling = new SolidBrush(Color.FromArgb(255, 246, 236)))
                        g.FillRectangle(filling, p.X - s * 0.35f, p.Y - s * 0.38f, s * 0.7f, s * 0.08f);
                    break;
            }
        }

        private void DrawSmoke(Graphics g, PointF top, int seed)
        {
            for (int k = 0; k < 6; k++)
            {
                float p = (_time * 0.35f + k / 6f + seed * 0.13f) % 1f;
                float rad = _tileW * (0.05f + p * 0.12f);
                float px = top.X + p * _tileW * 0.3f + MathF.Sin(_time + k) * _tileW * 0.03f;
                float py = top.Y - p * _tileW * 0.8f;
                using var brush = new SolidBrush(Color.FromArgb((int)(150 * (1 - p)), 245, 240, 235));
                g.FillEllipse(brush, px - rad, py - rad, rad * 2, rad * 2);
            }
        }

        /// <summary>Position courante d'une camionnette : elle boucle sur sa route, sur sa voie.</summary>
        private (float Gx, float Gy) VanPosition(Van v)
        {
            float along = (_time * v.Speed + v.Offset) % GridSize;
            if (v.Direction < 0) along = GridSize - along;
            float lane = v.Road + (v.Direction > 0 ? 0.7f : 0.3f);
            return v.AlongX ? (along, lane) : (lane, along);
        }

        private void DrawVan(Graphics g, Van v, float gx, float gy)
        {
            var color = _bakeries[v.Bakery].Color;
            float h = _tileW * 0.2f, half = 0.13f, d = v.Direction;

            // Ombre portée
            Fill(g, Color.FromArgb(60, 0, 0, 0), v.AlongX
                ? [Iso(gx - 0.3f, gy - half + 0.08f), Iso(gx + 0.33f, gy - half + 0.08f), Iso(gx + 0.33f, gy + half + 0.08f), Iso(gx - 0.3f, gy + half + 0.08f)]
                : [Iso(gx - half + 0.08f, gy - 0.3f), Iso(gx + half + 0.08f, gy - 0.3f), Iso(gx + half + 0.08f, gy + 0.33f), Iso(gx - half + 0.08f, gy + 0.33f)]);

            // Caisse (à l'arrière) et cabine (à l'avant), de la plus lointaine à la plus proche
            (float Along, float Len, float H, Color C) cargo = (-d * 0.07f, 0.42f, h, color);
            (float Along, float Len, float H, Color C) cab = (d * 0.2f, 0.14f, h * 0.7f, Color.FromArgb(245, 245, 240));
            var parts = d > 0 ? new[] { cargo, cab } : new[] { cab, cargo };

            foreach (var (along, len, ph, c) in parts)
            {
                float lo = along - len / 2;
                if (v.AlongX) DrawBox(g, gx + lo, gy - half, len, half * 2, 0, ph, c);
                else DrawBox(g, gx - half, gy + lo, half * 2, len, 0, ph, c);
            }
        }

        private static string LabelText(Bakery b) => $"{b.Name}  ·  parcelle {b.Parcel}";

        /// <summary>BP dont l'étiquette est sous le point donné ; la dernière dessinée (au premier plan) gagne.</summary>
        private Bakery? LabelAt(Point p)
        {
            for (int i = _labelHits.Count - 1; i >= 0; i--)
                if (_labelHits[i].Rect.Contains(p))
                    return _labelHits[i].Bakery;
            return null;
        }

        private void DrawLabel(Graphics g, Bakery b, Font font)
        {
            var anchor = Iso(b.X + 1f, b.Y + 1f, _tileW * 1.5f);
            string text = LabelText(b);
            var size = g.MeasureString(text, font);
            var rect = new RectangleF(anchor.X - size.Width / 2 - 8, anchor.Y - size.Height / 2 - 3, size.Width + 16, size.Height + 6);
            _labelHits.Add((rect, b));

            using var path = RoundedRect(rect, rect.Height / 2);
            using (var bg = new SolidBrush(Color.FromArgb(200, 40, 26, 36)))
                g.FillPath(bg, path);
            using (var border = new Pen(b.Color, 2))
                g.DrawPath(border, path);
            g.DrawString(text, font, Brushes.White, rect.X + 8, rect.Y + 3);
        }

        private void DrawHud(Graphics g)
        {
            using var title = new Font("Segoe UI", Math.Max(24f, ClientSize.Height * 0.04f), FontStyle.Bold, GraphicsUnit.Pixel);
            using var small = new Font("Segoe UI", Math.Max(12f, ClientSize.Height * 0.017f), GraphicsUnit.Pixel);
            using var dim = new SolidBrush(Color.FromArgb(210, 250, 236, 240));

            g.DrawString("UN MONDE DE DOUCEURS", title, Brushes.White, 24, 18);
            g.DrawString($"Tick {(int)(_time * 2)}  ·  {_bakeries.Count} boulangeries-pâtisseries  ·  {_villas.Count} villas  ·  maillage {GridSize}×{GridSize}  ·  rendu GDI+ {_renderMs:0.0} ms",
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
