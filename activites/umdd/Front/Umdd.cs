using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace Umdd
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

        private enum Ground { Grass, Road, Water, Lot, Garden, Plaza }

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

        private enum LandmarkKind { Bank, Police, Cooperative, Telecom, Registry }

        /// <summary>
        /// Bâtiment public symbolisant une fonctionnalité du système distribué, posé sur un lotissement dallé de Size x Size cases.
        /// LabelHeight : altitude de l'étiquette, en largeurs de tuile, pour qu'elle passe au-dessus du bâtiment.
        /// </summary>
        private record Landmark(LandmarkKind Kind, string Name, int X, int Y, int Size, Color Color, float LabelHeight);

        private readonly List<Landmark> _landmarks =
        [
            new(LandmarkKind.Bank, "Banque", 16, 16, 3, Color.FromArgb(225, 185, 70), 1.9f),   // au centre-ville, à l'angle du carrefour des routes 19
            new(LandmarkKind.Police, "Police", 26, 16, 3, Color.FromArgb(80, 130, 230), 2.3f), // supervision, à l'angle des routes 29 et 19
            new(LandmarkKind.Cooperative, "Coopérative", 26, 6, 3, Color.FromArgb(140, 210, 240), 1.6f), // entrepôt frigorifique, à l'angle des routes 29 et 9
            new(LandmarkKind.Telecom, "Télécoms", 6, 16, 3, Color.FromArgb(190, 140, 250), 3.1f),        // service de chat, à l'angle des routes 9 et 19
            new(LandmarkKind.Registry, "Registre du commerce", 6, 26, 3, Color.FromArgb(90, 170, 90), 2.3f), // annuaire, à l'angle des routes 9 et 29
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

        /// <summary>
        /// Étiquettes des BP et des bâtiments publics (zone écran + contour arrondi), calculées une fois par taille de fenêtre.
        /// Sert aussi à savoir sur quelle BP on clique (Bakery est null pour un bâtiment public, non cliquable).
        /// </summary>
        private readonly List<(RectangleF Rect, GraphicsPath Path, string Text, Color Color, Bakery? Bakery)> _labels = [];

        /// <summary>Objets en relief immobiles (arbres, BP, villas), déjà triés par profondeur.</summary>
        private (float Depth, Action<Graphics> Draw)[] _scenery = [];

        // Paramètres de projection, recalculés à chaque redimensionnement
        private float _tileW, _tileH, _originX, _originY;

        // Ressources de rendu réutilisées d'une image à l'autre
        private Bitmap? _background; // ciel, socle et sol (hors eau), rendus une seule fois par taille de fenêtre
        private Font? _labelFont, _titleFont, _smallFont;
        private SizeF _hintSize;
        private readonly Dictionary<int, SolidBrush> _brushes = [];
        private readonly Dictionary<(int Argb, float Width), Pen> _pens = [];

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
            FormClosed += (_, _) => { _timer.Dispose(); ReleaseLayoutResources(); foreach (var b in _brushes.Values) b.Dispose(); };
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

            // Parvis des bâtiments publics
            foreach (var l in _landmarks)
                for (int dx = 0; dx < l.Size; dx++)
                    for (int dy = 0; dy < l.Size; dy++)
                        _ground[l.X + dx, l.Y + dy] = Ground.Plaza;

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

            // Décor immobile trié une fois pour toutes (tri stable : arbres, puis BP, puis villas à profondeur égale)
            var scenery = new List<(float Depth, Action<Graphics> Draw)>();
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    if (_treeSize[x, y] > 0)
                    {
                        int tx = x, ty = y;
                        scenery.Add((tx + ty + 1, g => DrawTree(g, tx, ty, _treeSize[tx, ty])));
                    }
            foreach (var b in _bakeries)
                scenery.Add((b.X + b.Y + LotSize, g => DrawBakery(g, b)));
            foreach (var v in _villas)
                scenery.Add((v.X + v.Y + v.Size, g => DrawVilla(g, v)));
            foreach (var l in _landmarks)
                scenery.Add((l.X + l.Y + l.Size, g => DrawLandmark(g, l)));
            _scenery = [.. scenery.OrderBy(o => o.Depth)];
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
                    if (!inside && ground is Ground.Lot or Ground.Garden or Ground.Water or Ground.Plaza) return false;
                    if (!inside && !corner && ground == Ground.Road) touchesRoad = true;
                }
            return touchesRoad;
        }

        private Ground? GroundAt(int x, int y) =>
            x >= 0 && y >= 0 && x < GridSize && y < GridSize ? _ground[x, y] : null;

        /// <summary>Vrai si une case voisine est occupée par une BP, une villa ou un bâtiment public (on n'y plante pas d'arbre).</summary>
        private bool IsNearBuilding(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx, ny = y + dy;
                    if (nx >= 0 && ny >= 0 && nx < GridSize && ny < GridSize && _ground[nx, ny] is Ground.Lot or Ground.Garden or Ground.Plaza)
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

            // Tout ce qui dépend de la taille est recréé : polices, stylos (épaisseurs), fond et étiquettes (reconstruits au prochain rendu)
            ReleaseLayoutResources();
            _labelFont = new Font("Segoe UI Semibold", Math.Max(10f, _tileW * 0.3f), GraphicsUnit.Pixel);
            _titleFont = new Font("Segoe UI", Math.Max(24f, size.Height * 0.04f), FontStyle.Bold, GraphicsUnit.Pixel);
            _smallFont = new Font("Segoe UI", Math.Max(12f, size.Height * 0.017f), GraphicsUnit.Pixel);
        }

        private void ReleaseLayoutResources()
        {
            _background?.Dispose();
            _background = null;
            foreach (var label in _labels) label.Path.Dispose();
            _labels.Clear();
            foreach (var p in _pens.Values) p.Dispose();
            _pens.Clear();
            _labelFont?.Dispose();
            _titleFont?.Dispose();
            _smallFont?.Dispose();
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
            EnsureBackground();

            // Copie brute du fond pré-rendu (pas de mélange alpha ni d'interpolation)
            g.CompositingMode = CompositingMode.SourceCopy;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(_background!, 0, 0, _background!.Width, _background.Height);
            g.CompositingMode = CompositingMode.SourceOver;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // Seule l'eau du sol est animée
            for (int y = 0; y < GridSize; y++)
                for (int x = 0; x < GridSize; x++)
                    if (_ground[x, y] == Ground.Water)
                        DrawTile(g, x, y);

            // Objets en relief : algorithme du peintre, du fond (x+y petit) vers l'avant.
            // Le décor est déjà trié ; on y intercale les camionnettes (à profondeur égale, le décor passe d'abord)
            var vans = new (float Depth, Van Van, float Gx, float Gy)[_vans.Count];
            for (int k = 0; k < vans.Length; k++)
            {
                var (gx, gy) = VanPosition(_vans[k]);
                vans[k] = (gx + gy, _vans[k], gx, gy);
            }
            Array.Sort(vans, (a, b) => a.Depth.CompareTo(b.Depth));
            int i = 0;
            foreach (var (depth, van, gx, gy) in vans)
            {
                while (i < _scenery.Length && _scenery[i].Depth <= depth)
                    _scenery[i++].Draw(g);
                DrawVan(g, van, gx, gy);
            }
            while (i < _scenery.Length)
                _scenery[i++].Draw(g);

            foreach (var label in _labels)
                DrawLabel(g, label.Rect, label.Path, label.Text, label.Color);

            double ms = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
            _renderMs = _renderMs * 0.9 + ms * 0.1;
            DrawHud(g);
        }

        /// <summary>
        /// Pré-rend dans une image tout ce qui ne bouge pas sous les objets : ciel, socle, sol (sauf l'eau), marquages et bordures de parcelles.
        /// Profite du contexte graphique pour mesurer les étiquettes, qui ne changent pas non plus.
        /// </summary>
        private void EnsureBackground()
        {
            if (_background != null) return;

            _background = new Bitmap(ClientSize.Width, ClientSize.Height, PixelFormat.Format32bppPArgb);
            using var g = Graphics.FromImage(_background);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            DrawSky(g);
            DrawBase(g);
            for (int y = 0; y < GridSize; y++)
                for (int x = 0; x < GridSize; x++)
                    if (_ground[x, y] != Ground.Water)
                        DrawTile(g, x, y);
            foreach (var b in _bakeries)
                DrawParcelBorder(g, b);

            foreach (var b in _bakeries)
                AddLabel(g, Iso(b.X + 1f, b.Y + 1f, _tileW * 1.5f), LabelText(b), b.Color, b);
            foreach (var l in _landmarks)
                AddLabel(g, Iso(l.X + l.Size / 2f, l.Y + l.Size / 2f, _tileW * l.LabelHeight), l.Name, l.Color, null);
            _hintSize = g.MeasureString(Hint, _smallFont!);
        }

        /// <summary>Mesure une étiquette centrée sur son point d'ancrage et prépare son contour arrondi.</summary>
        private void AddLabel(Graphics g, PointF anchor, string text, Color color, Bakery? bakery)
        {
            var size = g.MeasureString(text, _labelFont!);
            var rect = new RectangleF(anchor.X - size.Width / 2 - 8, anchor.Y - size.Height / 2 - 3, size.Width + 16, size.Height + 6);
            _labels.Add((rect, RoundedRect(rect, rect.Height / 2), text, color, bakery));
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
                Ground.Plaza => Color.FromArgb(196, 192, 186),
                Ground.Garden => Shade(Color.FromArgb(140, 196, 104), _grassShade[x, y]),
                _ => Shade(Color.FromArgb(118, 176, 92), _grassShade[x, y]),
            };
            Fill(g, c, diamond);
            g.DrawPolygon(PenOf(Shade(c, 0.9f), 1), diamond);

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

            g.FillEllipse(BrushOf(Color.FromArgb(60, 0, 0, 0)), c.X - r * 0.6f, c.Y - r * 0.35f, r * 2.1f, r * 0.8f);
            g.FillRectangle(BrushOf(Color.FromArgb(110, 76, 48)), c.X - r * 0.15f, c.Y - trunk, r * 0.3f, trunk);

            float fy = c.Y - trunk - r * 0.8f;
            g.FillEllipse(BrushOf(Color.FromArgb(52, 112, 58)), c.X - r, fy - r, r * 2, r * 2);
            g.FillEllipse(BrushOf(Color.FromArgb(86, 150, 76)), c.X - r * 0.75f, fy - r * 0.8f, r * 1.1f, r * 1.1f);
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
            g.DrawPolygon(PenOf(Color.FromArgb(70, 130, 60), Math.Max(1.5f, _tileW / 18)), [Iso(X + 0.03f, Y + 0.03f), Iso(X + S - 0.03f, Y + 0.03f), Iso(X + S - 0.03f, Y + S - 0.03f), Iso(X + 0.03f, Y + S - 0.03f)]);

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

        private void DrawLandmark(Graphics g, Landmark l)
        {
            switch (l.Kind)
            {
                case LandmarkKind.Bank: DrawBank(g, l.X, l.Y); break;
                case LandmarkKind.Police: DrawPoliceStation(g, l.X, l.Y); break;
                case LandmarkKind.Cooperative: DrawColdStore(g, l.X, l.Y); break;
                case LandmarkKind.Telecom: DrawTelecom(g, l.X, l.Y, l.Color); break;
                case LandmarkKind.Registry: DrawRegistry(g, l.X, l.Y); break;
            }
        }

        /// <summary>
        /// Registre du commerce (lotissement 3x3), pour l'annuaire : bâtiment administratif à toit mansardé et tour d'horloge.
        /// Ses fenêtres s'allument l'une après l'autre, comme une recherche qui parcourt les fiches ; un drapeau suisse flotte devant.
        /// </summary>
        private void DrawRegistry(Graphics g, float X, float Y)
        {
            float h = _tileW * 0.85f, cornice = h * 0.05f;
            var sandstone = Color.FromArgb(225, 205, 165);
            var slate = Color.FromArgb(90, 100, 115);
            float x0 = X + 0.35f, y0 = Y + 0.35f, x1 = X + 2.3f, y1 = Y + 2.3f, yMid = (y0 + y1) / 2;

            // Corps de bâtiment, soubassement plus sombre, perron et porte côté route (+x)
            DrawBox(g, x0, y0, x1 - x0, y1 - y0, 0, h, sandstone);
            Fill(g, Shade(sandstone, 0.7f), FaceY(y1, x0, x1, 0, h * 0.1f));
            Fill(g, Shade(sandstone, 0.55f), FaceX(x1, y0, y1, 0, h * 0.1f));
            Fill(g, Color.FromArgb(95, 60, 40), FaceX(x1, yMid - 0.15f, yMid + 0.15f, h * 0.05f, h * 0.32f));
            DrawBox(g, x1, yMid - 0.25f, 0.15f, 0.5f, 0, h * 0.05f, Color.FromArgb(200, 195, 185));

            // Fenêtres : 3 étages x 5 travées par façade ; la « recherche » allume une fenêtre après l'autre, avec une traîne
            const int windows = 15 + 14;
            int lit = (int)(_time * 5) % windows, n = 0;
            Color WindowColor(int index) =>
                index == lit ? Color.FromArgb(255, 232, 150)
                : index == (lit + windows - 1) % windows ? Color.FromArgb(200, 180, 130)
                : Color.FromArgb(85, 100, 125);
            for (int f = 0; f < 3; f++)
            {
                float za = h * (0.15f + f * 0.27f), zb = za + h * 0.15f;
                for (int i = 0; i < 5; i++)
                {
                    float xa = x0 + 0.15f + i * 0.36f;
                    Fill(g, WindowColor(n++), FaceY(y1, xa, xa + 0.18f, za, zb));
                }
                for (int i = 0; i < 5; i++)
                {
                    if (f == 0 && i == 2) continue; // porte
                    float ya = y0 + 0.15f + i * 0.36f;
                    Fill(g, WindowColor(n++), FaceX(x1, ya, ya + 0.18f, za, zb));
                }
            }

            // Corniche et toit mansardé avec lucarnes
            float z = h + cornice, rh = _tileW * 0.22f, inset = 0.3f;
            DrawBox(g, x0 - 0.04f, y0 - 0.04f, x1 - x0 + 0.08f, y1 - y0 + 0.08f, h, cornice, Shade(sandstone, 1.08f));
            DrawMansardRoof(g, x0, y0, x1, y1, z, rh, inset, slate);
            var dormer = Color.FromArgb(215, 220, 228);
            PointF OnFrontSlope(float x, float t) => Iso(x, y1 - t * inset, z + t * rh);
            PointF OnSideSlope(float y, float t) => Iso(x1 - t * inset, y, z + t * rh);
            for (int i = 0; i < 4; i++)
            {
                float a = x0 + 0.3f + i * 0.4f, b = a + 0.16f;
                Fill(g, dormer, OnFrontSlope(a, 0.25f), OnFrontSlope(b, 0.25f), OnFrontSlope(b, 0.7f), OnFrontSlope(a, 0.7f));
                a = y0 + 0.3f + i * 0.4f; b = a + 0.16f;
                Fill(g, Shade(dormer, 0.8f), OnSideSlope(a, 0.25f), OnSideSlope(b, 0.25f), OnSideSlope(b, 0.7f), OnSideSlope(a, 0.7f));
            }

            // Tour d'horloge au centre du toit (l'horloge donne l'heure réelle)
            float s = 0.4f, tx = (x0 + x1) / 2 - s / 2, ty = (y0 + y1) / 2 - s / 2, tz = z + rh, th = _tileW * 0.35f;
            DrawBox(g, tx, ty, s, s, tz, th, sandstone);
            DrawClock(g, Iso(tx + s / 2, ty + s, tz + th * 0.55f), _tileW * 0.075f);
            DrawHipRoof(g, tx - 0.04f, ty - 0.04f, tx + s + 0.04f, ty + s + 0.04f, tz + th, _tileW * 0.22f, slate);

            // Drapeau suisse au coin du parvis, côté carrefour
            DrawSwissFlag(g, X + 2.75f, Y + 2.75f, _tileW * 1.0f);
        }

        /// <summary>Toit mansardé : brisis pentus sur les faces visibles et terrasson plat au sommet.</summary>
        private void DrawMansardRoof(Graphics g, float x0, float y0, float x1, float y1, float z, float rh, float inset, Color c)
        {
            float ix0 = x0 + inset, iy0 = y0 + inset, ix1 = x1 - inset, iy1 = y1 - inset, zt = z + rh;
            Fill(g, Shade(c, 0.7f), Iso(x1, y0, z), Iso(x1, y1, z), Iso(ix1, iy1, zt), Iso(ix1, iy0, zt));
            Fill(g, Shade(c, 0.88f), Iso(x0, y1, z), Iso(x1, y1, z), Iso(ix1, iy1, zt), Iso(ix0, iy1, zt));
            Fill(g, Shade(c, 1.2f), Iso(ix0, iy0, zt), Iso(ix1, iy0, zt), Iso(ix1, iy1, zt), Iso(ix0, iy1, zt));
        }

        /// <summary>Cadran d'horloge (face à l'écran) qui affiche l'heure locale.</summary>
        private void DrawClock(Graphics g, PointF c, float r)
        {
            g.FillEllipse(BrushOf(Color.FromArgb(250, 248, 240)), c.X - r, c.Y - r, r * 2, r * 2);
            g.DrawEllipse(PenOf(Color.FromArgb(60, 55, 50), Math.Max(1f, _tileW / 70)), c.X - r, c.Y - r, r * 2, r * 2);
            var now = DateTime.Now;
            float minutes = now.Minute + now.Second / 60f, hours = now.Hour % 12 + minutes / 60;
            var hand = PenOf(Color.FromArgb(40, 35, 30), Math.Max(1f, _tileW / 60));
            foreach (var (turns, length) in new[] { (hours / 12, 0.5f), (minutes / 60, 0.8f) })
            {
                float a = turns * MathF.Tau - MathF.PI / 2;
                g.DrawLine(hand, c, new PointF(c.X + MathF.Cos(a) * r * length, c.Y + MathF.Sin(a) * r * length));
            }
        }

        /// <summary>Mât et drapeau suisse (carré rouge, croix blanche) qui ondule au vent.</summary>
        private void DrawSwissFlag(Graphics g, float gx, float gy, float poleH)
        {
            var foot = Iso(gx, gy);
            var top = Iso(gx, gy, poleH);
            g.DrawLine(PenOf(Color.FromArgb(200, 200, 205), Math.Max(1f, _tileW / 45)), foot, top);

            float size = _tileW * 0.24f;
            // (u, v) dans [0,1]² → point écran ; l'ondulation croît en s'éloignant du mât
            PointF At(float u, float v) =>
                new(top.X + u * size, top.Y + _tileW * 0.02f + v * size + MathF.Sin(_time * 4 - u * 3) * u * size * 0.12f);
            void Quad(Color c, float u0, float v0, float u1, float v1) =>
                Fill(g, c, At(u0, v0), At(u1, v0), At(u1, v1), At(u0, v1));

            Quad(Color.FromArgb(215, 30, 40), 0, 0, 1, 1);
            Quad(Color.White, 0.4f, 0.2f, 0.6f, 0.8f);
            Quad(Color.White, 0.2f, 0.4f, 0.8f, 0.6f);
        }

        /// <summary>
        /// Télécoms (lotissement 3x3), pour le service de chat : pylône rouge et blanc qui émet des ondes radio,
        /// bâtiment technique à bandeaux vitrés avec paraboles sur le toit, et bulles de dialogue qui s'en échappent.
        /// </summary>
        private void DrawTelecom(Graphics g, float X, float Y, Color accent)
        {
            // Pylône à l'arrière du lotissement, dessiné avant le bâtiment qui est devant lui
            float H = _tileW * 1.7f;
            DrawLatticeTower(g, X + 0.5f, Y + 0.8f, H, accent);

            // Bâtiment technique : bandeaux vitrés filants, entrée côté route
            float h = _tileW * 0.55f, slab = h * 0.05f;
            float x0 = X + 1.0f, y0 = Y + 0.3f, x1 = X + 2.5f, y1 = Y + 2.4f, yMid = (y0 + y1) / 2;
            var glass = Color.FromArgb(60, 80, 120);
            DrawBox(g, x0, y0, x1 - x0, y1 - y0, 0, h, Color.FromArgb(205, 208, 218));
            Fill(g, glass, FaceY(y1, x0 + 0.08f, x1 - 0.08f, h * 0.2f, h * 0.42f));
            Fill(g, glass, FaceY(y1, x0 + 0.08f, x1 - 0.08f, h * 0.58f, h * 0.8f));
            Fill(g, glass, FaceX(x1, y0 + 0.08f, yMid - 0.3f, h * 0.2f, h * 0.42f));
            Fill(g, glass, FaceX(x1, yMid + 0.3f, y1 - 0.08f, h * 0.2f, h * 0.42f));
            Fill(g, glass, FaceX(x1, y0 + 0.08f, y1 - 0.08f, h * 0.58f, h * 0.8f));
            Fill(g, Color.FromArgb(120, 160, 200), FaceX(x1, yMid - 0.2f, yMid + 0.2f, 0, h * 0.4f));
            DrawBox(g, x0, y0, x1 - x0, y1 - y0, h, slab, Color.FromArgb(160, 163, 172));

            // Paraboles sur le toit
            for (int i = 0; i < 2; i++)
                DrawDish(g, Iso(x0 + 0.45f, y0 + 0.5f + i * 0.8f, h + slab));

            // Bulles de dialogue qui montent du toit en alternance, puis s'effacent
            for (int k = 0; k < 2; k++)
            {
                float p = (_time * 0.3f + k * 0.5f) % 1f;
                var at = Iso(x0 + 0.9f, yMid + (k == 0 ? -0.45f : 0.45f), h + slab + _tileW * (0.15f + p * 0.6f));
                DrawChatBubble(g, at, (int)(255 * (1 - p) * Math.Min(1f, p * 6)), accent);
            }
        }

        /// <summary>Pylône en treillis effilé, rayé rouge et blanc, avec antennes, feu d'obstacle et ondes radio animées.</summary>
        private void DrawLatticeTower(Graphics g, float cx, float cy, float H, Color waves)
        {
            const int levels = 6;
            float bottom = 0.22f, top = 0.05f, w = Math.Max(1f, _tileW / 45);
            float Half(int i) => bottom + (top - bottom) * i / levels;
            float Z(int i) => H * i / levels;

            // Montants (les 4 visibles, la structure est ajourée), puis entretoises en zigzag sur les faces avant
            var leg = PenOf(Color.FromArgb(200, 60, 55), w);
            for (int sx = -1; sx <= 1; sx += 2)
                for (int sy = -1; sy <= 1; sy += 2)
                    g.DrawLine(leg, Iso(cx + sx * bottom, cy + sy * bottom), Iso(cx + sx * top, cy + sy * top, H));
            for (int i = 0; i < levels; i++)
            {
                var brace = PenOf(i % 2 == 0 ? Color.FromArgb(200, 60, 55) : Color.FromArgb(245, 245, 245), w);
                float a = Half(i), b = Half(i + 1), s = i % 2 == 0 ? 1 : -1;
                g.DrawLine(brace, Iso(cx - s * a, cy + a, Z(i)), Iso(cx + s * b, cy + b, Z(i + 1)));
                g.DrawLine(brace, Iso(cx + a, cy - s * a, Z(i)), Iso(cx + b, cy + s * b, Z(i + 1)));
                g.DrawLine(brace, Iso(cx - a, cy + a, Z(i)), Iso(cx + a, cy + a, Z(i)));
                g.DrawLine(brace, Iso(cx + a, cy - a, Z(i)), Iso(cx + a, cy + a, Z(i)));
            }

            // Plateforme technique et antennes-panneaux
            float pz = H * 0.72f, ph = H * 0.05f, pr = Half(4) + 0.04f;
            DrawBox(g, cx - pr, cy - pr, pr * 2, pr * 2, pz, ph, Color.FromArgb(235, 235, 235));
            DrawBox(g, cx + pr - 0.03f, cy - 0.04f, 0.04f, 0.08f, pz + ph, H * 0.12f, Color.FromArgb(225, 225, 230));
            DrawBox(g, cx - 0.04f, cy + pr - 0.03f, 0.08f, 0.04f, pz + ph, H * 0.12f, Color.FromArgb(225, 225, 230));

            // Mât sommital et feu d'obstacle clignotant
            var tip = Iso(cx, cy, H * 1.12f);
            g.DrawLine(PenOf(Color.FromArgb(200, 60, 55), w), Iso(cx, cy, H), tip);
            bool on = (_time + 0.3f) % 1.2f < 0.4f;
            float lr = _tileW * 0.03f;
            if (on)
                g.FillEllipse(BrushOf(Color.FromArgb(90, 255, 60, 60)), tip.X - lr * 3, tip.Y - lr * 3, lr * 6, lr * 6);
            g.FillEllipse(BrushOf(on ? Color.FromArgb(255, 70, 70) : Color.FromArgb(120, 30, 30)), tip.X - lr, tip.Y - lr, lr * 2, lr * 2);

            // Ondes radio : arcs qui s'éloignent de chaque côté du sommet en s'estompant
            var source = Iso(cx, cy, H * 0.95f);
            for (int k = 0; k < 3; k++)
            {
                float p = (_time * 0.6f + k / 3f) % 1f, r = _tileW * (0.15f + p * 0.8f);
                var pen = PenOf(Color.FromArgb((int)(200 * (1 - p)), waves), Math.Max(1.5f, _tileW / 30));
                var box = new RectangleF(source.X - r, source.Y - r, r * 2, r * 2);
                g.DrawArc(pen, box, -30, 60);
                g.DrawArc(pen, box, 150, 60);
            }
        }

        /// <summary>Parabole sur pied, tournée vers le ciel.</summary>
        private void DrawDish(Graphics g, PointF foot)
        {
            float stand = _tileW * 0.1f, w = _tileW * 0.24f, hh = _tileW * 0.15f;
            var c = new PointF(foot.X, foot.Y - stand);
            g.DrawLine(PenOf(Color.FromArgb(110, 115, 125), Math.Max(1f, _tileW / 50)), foot, c);
            g.FillEllipse(BrushOf(Color.FromArgb(240, 242, 245)), c.X - w / 2, c.Y - hh / 2, w, hh);
            g.DrawEllipse(PenOf(Color.FromArgb(170, 175, 185), 1), c.X - w / 2, c.Y - hh / 2, w, hh);
            g.DrawLine(PenOf(Color.FromArgb(110, 115, 125), 1), c.X, c.Y, c.X + w * 0.25f, c.Y - hh * 0.6f); // bras du cornet
        }

        /// <summary>Bulle de dialogue « … » (dessinée face à l'écran), avec transparence.</summary>
        private void DrawChatBubble(Graphics g, PointF tail, int alpha, Color accent)
        {
            if (alpha <= 0) return;
            float w = _tileW * 0.34f, hh = _tileW * 0.21f;
            float left = tail.X - w * 0.3f, topY = tail.Y - hh - _tileW * 0.06f;
            var paper = BrushOf(Color.FromArgb(alpha, 255, 255, 255));
            g.FillEllipse(paper, left, topY, w, hh);
            g.FillPolygon(paper, [new PointF(tail.X - _tileW * 0.02f, topY + hh * 0.8f), new PointF(tail.X + _tileW * 0.06f, topY + hh * 0.8f), tail]);
            var dot = BrushOf(Color.FromArgb(alpha, accent));
            float d = _tileW * 0.045f;
            for (int i = -1; i <= 1; i++)
            {
                float dx = left + w / 2 + i * d * 1.6f;
                g.FillEllipse(dot, dx - d / 2, topY + hh / 2 - d / 2, d, d);
            }
        }

        /// <summary>
        /// Coopérative : entrepôt frigorifique (lotissement 3x3) en bardage nervuré, logo flocon, groupes froids sur le toit,
        /// quais de chargement côté route (+x) dont un ouvert, avec un camion frigorifique à quai et de la buée qui s'échappe.
        /// </summary>
        private void DrawColdStore(Graphics g, float X, float Y)
        {
            float h = _tileW * 0.6f, slab = h * 0.05f, dockH = h * 0.18f;
            var wall = Color.FromArgb(236, 240, 244);
            var ice = Color.FromArgb(70, 140, 200);
            var concrete = Color.FromArgb(190, 188, 182);
            var rib = PenOf(Shade(wall, 0.86f), 1);

            // Halle : bardage nervuré et soubassement bleu glacier
            float x0 = X + 0.25f, y0 = Y + 0.25f, x1 = X + 2.2f, y1 = Y + 2.6f;
            DrawBox(g, x0, y0, x1 - x0, y1 - y0, 0, h, wall);
            for (float x = x0 + 0.15f; x < x1; x += 0.15f)
                g.DrawLine(rib, Iso(x, y1, 0), Iso(x, y1, h));
            for (float y = y0 + 0.15f; y < y1; y += 0.15f)
                g.DrawLine(rib, Iso(x1, y, 0), Iso(x1, y, h));
            Fill(g, ice, FaceY(y1, x0, x1, 0, h * 0.1f));
            Fill(g, Shade(ice, 0.8f), FaceX(x1, y0, y1, 0, h * 0.1f));

            DrawSnowflake(g, Iso((x0 + x1) / 2, y1, h * 0.55f), _tileW * 0.17f, ice);

            // Trois portes sectionnelles ; celle du milieu est ouverte sur la chambre froide
            const int open = 1;
            var shutter = PenOf(Color.FromArgb(160, 165, 172), 1);
            for (int i = 0; i < 3; i++)
            {
                float ya = y0 + 0.2f + i * 0.75f, yb = ya + 0.5f;
                if (i == open)
                {
                    Fill(g, Color.FromArgb(55, 85, 115), FaceX(x1, ya, yb, dockH, h * 0.7f));
                    continue;
                }
                Fill(g, Color.FromArgb(205, 210, 216), FaceX(x1, ya, yb, dockH, h * 0.7f));
                for (int k = 1; k < 5; k++)
                {
                    float z = dockH + k * (h * 0.7f - dockH) / 5;
                    g.DrawLine(shutter, Iso(x1, ya, z), Iso(x1, yb, z));
                }
            }

            // Toit plat et groupes froids, du fond vers l'avant, ventilateurs en rotation
            DrawBox(g, x0, y0, x1 - x0, y1 - y0, h, slab, Color.FromArgb(200, 205, 212));
            for (int i = 0; i < 3; i++)
            {
                float cx = x0 + 0.3f, cy = y0 + 0.3f + i * 0.65f, ch = h * 0.15f;
                DrawBox(g, cx, cy, 0.45f, 0.4f, h + slab, ch, Color.FromArgb(180, 190, 200));
                DrawFan(g, Iso(cx + 0.225f, cy + 0.2f, h + slab + ch), _tileW * 0.09f, _time * 12 + i);
            }

            // Quai de chargement et auvent
            DrawBox(g, x1, y0 + 0.1f, 0.2f, y1 - y0 - 0.2f, 0, dockH, concrete);
            DrawBox(g, x1, y0, 0.35f, y1 - y0, h * 0.78f, h * 0.04f, ice);

            // Camion frigorifique à quai devant la porte ouverte : caisse isotherme puis cabine
            float yc = y0 + 0.2f + open * 0.75f + 0.25f, tx = x1 + 0.22f;
            DrawBox(g, tx, yc - 0.15f, 0.4f, 0.3f, 0, _tileW * 0.2f, Color.FromArgb(248, 250, 252));
            Fill(g, ice, FaceY(yc + 0.15f, tx + 0.03f, tx + 0.37f, _tileW * 0.05f, _tileW * 0.09f));
            DrawBox(g, tx + 0.4f, yc - 0.13f, 0.15f, 0.26f, 0, _tileW * 0.14f, Color.FromArgb(140, 210, 240));

            // Buée froide qui s'échappe par le haut de la porte ouverte et retombe
            for (int k = 0; k < 5; k++)
            {
                float p = (_time * 0.4f + k / 5f) % 1f;
                var at = Iso(x1 + 0.05f + p * 0.5f, yc + MathF.Sin(_time + k) * 0.12f, h * (0.65f - p * 0.4f));
                float rad = _tileW * (0.04f + p * 0.08f);
                g.FillEllipse(BrushOf(Color.FromArgb((int)(130 * (1 - p)), 235, 245, 255)), at.X - rad, at.Y - rad, rad * 2, rad * 2);
            }
        }

        /// <summary>Flocon de neige blanc sur une pastille de couleur (dessiné face à l'écran).</summary>
        private void DrawSnowflake(Graphics g, PointF c, float r, Color disc)
        {
            g.FillEllipse(BrushOf(disc), c.X - r, c.Y - r, r * 2, r * 2);
            var pen = PenOf(Color.White, Math.Max(1f, _tileW / 60));
            float arm = r * 0.75f;
            for (int k = 0; k < 6; k++)
            {
                float a = MathF.PI / 2 + k * MathF.PI / 3;
                var end = new PointF(c.X + MathF.Cos(a) * arm, c.Y + MathF.Sin(a) * arm);
                g.DrawLine(pen, c, end);
                var mid = new PointF(c.X + MathF.Cos(a) * arm * 0.55f, c.Y + MathF.Sin(a) * arm * 0.55f);
                for (int s = -1; s <= 1; s += 2) // deux petites branches de part et d'autre du bras
                {
                    float b = a + s * 0.7f;
                    g.DrawLine(pen, mid, new PointF(mid.X + MathF.Cos(b) * arm * 0.3f, mid.Y + MathF.Sin(b) * arm * 0.3f));
                }
            }
        }

        /// <summary>Ventilateur vu du dessus (cercle aplati 2:1 par la projection), deux pales en rotation.</summary>
        private void DrawFan(Graphics g, PointF c, float r, float angle)
        {
            float ry = r / 2;
            g.FillEllipse(BrushOf(Color.FromArgb(60, 65, 75)), c.X - r, c.Y - ry, r * 2, ry * 2);
            var blade = PenOf(Color.FromArgb(170, 175, 185), Math.Max(1f, _tileW / 50));
            for (int k = 0; k < 2; k++)
            {
                float a = angle + k * MathF.PI / 2, dx = MathF.Cos(a) * r * 0.85f, dy = MathF.Sin(a) * ry * 0.85f;
                g.DrawLine(blade, c.X - dx, c.Y - dy, c.X + dx, c.Y + dy);
            }
        }

        /// <summary>
        /// Poste de police (lotissement 3x3) : bâtiment à deux niveaux et toit plat, entrée côté route (+x),
        /// antenne radio à feu clignotant et voiture de patrouille garée devant, gyrophare allumé.
        /// </summary>
        private void DrawPoliceStation(Graphics g, float X, float Y)
        {
            float h = _tileW * 0.9f, rim = h * 0.06f;
            var wall = Color.FromArgb(220, 226, 234);
            var navy = Color.FromArgb(35, 60, 120);
            var glass = Color.FromArgb(150, 190, 220);
            bool blink = _time % 1f < 0.5f;

            // Parking devant (côté +y), avec ses lignes de marquage
            Fill(g, Color.FromArgb(105, 104, 110), Iso(X + 0.2f, Y + 2.0f), Iso(X + 2.8f, Y + 2.0f), Iso(X + 2.8f, Y + 2.85f), Iso(X + 0.2f, Y + 2.85f));
            var line = PenOf(Color.FromArgb(235, 235, 230), Math.Max(1f, _tileW / 50));
            for (int i = 0; i < 4; i++)
            {
                float lx = X + 0.4f + i * 0.7f;
                g.DrawLine(line, Iso(lx, Y + 2.1f), Iso(lx, Y + 2.75f));
            }

            // Bâtiment principal : deux rangées de fenêtres, bandeau bleu en haut des façades
            float x0 = X + 0.3f, y0 = Y + 0.3f, x1 = X + 2.3f, y1 = Y + 1.8f, yMid = (y0 + y1) / 2;
            DrawBox(g, x0, y0, x1 - x0, y1 - y0, 0, h, wall);
            for (int floor = 0; floor < 2; floor++)
            {
                float za = h * (0.12f + floor * 0.4f), zb = za + h * 0.2f;
                for (int i = 0; i < 5; i++)
                {
                    float xa = x0 + 0.15f + i * 0.38f;
                    Fill(g, glass, FaceY(y1, xa, xa + 0.22f, za, zb));
                }
                for (int i = 0; i < 4; i++)
                {
                    if (floor == 0 && i is 1 or 2) continue; // place pour l'entrée
                    float ya = y0 + 0.12f + i * 0.35f;
                    Fill(g, glass, FaceX(x1, ya, ya + 0.2f, za, zb));
                }
            }
            Fill(g, navy, FaceY(y1, x0, x1, h * 0.86f, h * 0.97f));
            Fill(g, Shade(navy, 0.8f), FaceX(x1, y0, y1, h * 0.86f, h * 0.97f));

            // Entrée vitrée sous un auvent bleu
            Fill(g, Color.FromArgb(120, 160, 200), FaceX(x1, yMid - 0.2f, yMid + 0.2f, 0, h * 0.32f));
            DrawBox(g, x1, yMid - 0.32f, 0.3f, 0.64f, h * 0.36f, h * 0.05f, navy);

            // Toit plat avec acrotère, cage d'escalier et antenne radio de supervision
            DrawBox(g, x0, y0, x1 - x0, y1 - y0, h, rim, Color.FromArgb(150, 155, 165));
            DrawBox(g, x0 + 0.2f, y0 + 0.2f, 0.45f, 0.4f, h + rim, h * 0.2f, wall);
            var foot = Iso(x1 - 0.3f, y0 + 0.25f, h + rim);
            var top = Iso(x1 - 0.3f, y0 + 0.25f, h + rim + _tileW * 0.6f);
            var mast = PenOf(Color.FromArgb(90, 95, 105), Math.Max(1f, _tileW / 40));
            g.DrawLine(mast, foot, top);
            for (int k = 1; k <= 2; k++) // barreaux d'antenne
            {
                float ty = top.Y + (foot.Y - top.Y) * k * 0.25f, half = _tileW * (0.04f + k * 0.03f);
                g.DrawLine(mast, top.X - half, ty, top.X + half, ty);
            }
            float lr = _tileW * 0.035f;
            if (blink)
                g.FillEllipse(BrushOf(Color.FromArgb(90, 255, 60, 60)), top.X - lr * 3, top.Y - lr * 3, lr * 6, lr * 6);
            g.FillEllipse(BrushOf(blink ? Color.FromArgb(255, 70, 70) : Color.FromArgb(120, 30, 30)), top.X - lr, top.Y - lr, lr * 2, lr * 2);

            // Voiture de patrouille, gyrophare bleu / rouge en alternance
            DrawPoliceCar(g, X + 1.0f, Y + 2.42f, (int)(_time * 4) % 2 == 0);
        }

        private void DrawPoliceCar(Graphics g, float cx, float cy, bool phase)
        {
            float bh = _tileW * 0.12f, ch = _tileW * 0.08f, lh = _tileW * 0.03f;
            var navy = Color.FromArgb(35, 60, 120);
            var blue = Color.FromArgb(60, 120, 255);
            var red = Color.FromArgb(255, 60, 60);

            Fill(g, Color.FromArgb(60, 0, 0, 0), Iso(cx - 0.3f, cy - 0.05f), Iso(cx + 0.36f, cy - 0.05f), Iso(cx + 0.36f, cy + 0.21f), Iso(cx - 0.3f, cy + 0.21f));
            DrawBox(g, cx - 0.3f, cy - 0.13f, 0.6f, 0.26f, 0, bh, Color.FromArgb(245, 245, 245));
            Fill(g, navy, FaceY(cy + 0.13f, cx - 0.3f, cx + 0.3f, bh * 0.35f, bh * 0.65f));
            DrawBox(g, cx - 0.12f, cy - 0.11f, 0.26f, 0.22f, bh, ch, navy);

            // Rampe lumineuse : deux feux sur le toit, de l'arrière vers l'avant
            float z = bh + ch;
            var (left, right) = phase ? (blue, red) : (red, blue);
            DrawBox(g, cx - 0.02f, cy - 0.1f, 0.08f, 0.1f, z, lh, left);
            DrawBox(g, cx - 0.02f, cy, 0.08f, 0.1f, z, lh, right);
            var glowAt = Iso(cx + 0.02f, phase ? cy - 0.05f : cy + 0.05f, z + lh);
            float gr = _tileW * 0.1f;
            g.FillEllipse(BrushOf(Color.FromArgb(80, phase ? blue : red)), glowAt.X - gr, glowAt.Y - gr, gr * 2, gr * 2);
        }

        /// <summary>Banque néoclassique (lotissement 3x3) : soubassement, colonnade et fronton tournés vers la route (+x).</summary>
        private void DrawBank(Graphics g, float X, float Y)
        {
            float hp = _tileW * 0.12f, h = _tileW * 0.75f, eh = _tileW * 0.1f, r = _tileW * 0.4f;
            var stone = Color.FromArgb(232, 224, 206);
            var marble = Color.FromArgb(246, 243, 236);
            var plinth = Color.FromArgb(205, 198, 186);

            // Soubassement, et deux marches qui descendent vers la route
            DrawBox(g, X + 0.2f, Y + 0.25f, 2.3f, 2.5f, 0, hp, plinth);
            DrawBox(g, X + 2.5f, Y + 0.8f, 0.15f, 1.4f, 0, hp * 0.66f, plinth);
            DrawBox(g, X + 2.65f, Y + 0.8f, 0.15f, 1.4f, 0, hp * 0.33f, plinth);

            // Salle des guichets : hautes fenêtres sur le côté, porte de bronze sous le portique
            float x0 = X + 0.4f, y0 = Y + 0.45f, x1 = X + 1.85f, y1 = Y + 2.55f, xp = X + 2.4f, yMid = (y0 + y1) / 2;
            DrawBox(g, x0, y0, x1 - x0, y1 - y0, hp, h, stone);
            var glass = Color.FromArgb(120, 150, 175);
            for (int i = 0; i < 3; i++)
            {
                float xa = x0 + 0.2f + i * 0.42f;
                Fill(g, glass, FaceY(y1, xa, xa + 0.2f, hp + h * 0.2f, hp + h * 0.75f));
            }
            Fill(g, Color.FromArgb(150, 110, 60), FaceX(x1, yMid - 0.25f, yMid + 0.25f, hp, hp + h * 0.65f));

            // Colonnade, de la plus lointaine à la plus proche
            const int columns = 6;
            for (int i = 0; i < columns; i++)
            {
                float cy = y0 + 0.05f + i * (y1 - y0 - 0.22f) / (columns - 1);
                DrawBox(g, xp - 0.17f, cy, 0.12f, 0.12f, hp, h, marble);
            }

            // Entablement, toit de cuivre patiné et fronton de pierre
            float z = hp + h + eh;
            DrawBox(g, x0, y0, xp - x0, y1 - y0, hp + h, eh, stone);
            DrawGableRoof(g, x0, y0, xp, y1, z, r, Color.FromArgb(105, 160, 140));
            Fill(g, Shade(stone, 0.8f),
                Iso(xp, y0 + 0.15f, z + _tileW * 0.03f), Iso(xp, y1 - 0.15f, z + _tileW * 0.03f), Iso(xp, yMid, z + r - _tileW * 0.06f));

            // Pièce d'or au centre du fronton
            var coin = Iso(xp, yMid, z + r * 0.4f);
            float cr = _tileW * 0.07f;
            var gold = Color.FromArgb(225, 185, 70);
            g.FillEllipse(BrushOf(gold), coin.X - cr, coin.Y - cr, cr * 2, cr * 2);
            g.DrawEllipse(PenOf(Shade(gold, 0.7f), Math.Max(1f, _tileW / 60)), coin.X - cr, coin.Y - cr, cr * 2, cr * 2);
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
            var tiles = PenOf(Shade(c, 0.75f), Math.Max(1f, _tileW / 60));
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
            g.DrawLine(PenOf(Color.FromArgb(90, 80, 70), Math.Max(1f, _tileW / 50)), Iso(gx, gy, tableH), top);
            float w = _tileW * 0.5f, hh = w * 0.3f;
            g.FillPie(BrushOf(Shade(c, 1.05f)), top.X - w / 2, top.Y - hh * 0.6f, w, hh * 1.4f, 180, 180);
            g.FillEllipse(BrushOf(Shade(c, 0.8f)), top.X - w / 2, top.Y + hh * 0.05f, w, hh * 0.25f);
        }

        /// <summary>Une douceur sur l'étal : croissant, baguette, gâteau ou macaron.</summary>
        private void DrawPastry(Graphics g, PointF p, int kind)
        {
            float s = _tileW * 0.13f;
            switch (kind)
            {
                case 0: // croissant
                {
                    g.FillEllipse(BrushOf(Color.FromArgb(226, 160, 70)), p.X - s / 2, p.Y - s * 0.45f, s, s * 0.45f);
                    var crust = PenOf(Color.FromArgb(170, 104, 40), 1);
                    g.DrawLine(crust, p.X - s * 0.15f, p.Y - s * 0.42f, p.X - s * 0.15f, p.Y - s * 0.05f);
                    g.DrawLine(crust, p.X + s * 0.15f, p.Y - s * 0.42f, p.X + s * 0.15f, p.Y - s * 0.05f);
                    break;
                }
                case 1: // baguette
                {
                    g.FillEllipse(BrushOf(Color.FromArgb(200, 140, 72)), p.X - s * 0.7f, p.Y - s * 0.3f, s * 1.4f, s * 0.3f);
                    var cut = PenOf(Color.FromArgb(240, 214, 160), 1);
                    for (int k = -1; k <= 1; k++)
                        g.DrawLine(cut, p.X + k * s * 0.35f - s * 0.08f, p.Y - s * 0.22f, p.X + k * s * 0.35f + s * 0.08f, p.Y - s * 0.1f);
                    break;
                }
                case 2: // gâteau
                {
                    var sponge = BrushOf(Color.FromArgb(240, 150, 180));
                    g.FillRectangle(sponge, p.X - s * 0.4f, p.Y - s * 0.6f, s * 0.8f, s * 0.5f);
                    g.FillEllipse(sponge, p.X - s * 0.4f, p.Y - s * 0.25f, s * 0.8f, s * 0.3f);
                    g.FillEllipse(BrushOf(Color.FromArgb(255, 246, 236)), p.X - s * 0.4f, p.Y - s * 0.75f, s * 0.8f, s * 0.3f);
                    g.FillEllipse(BrushOf(Color.FromArgb(200, 30, 50)), p.X - s * 0.1f, p.Y - s * 0.9f, s * 0.2f, s * 0.2f);
                    break;
                }
                default: // macaron
                {
                    var shell = BrushOf(Color.FromArgb(160, 210, 150));
                    g.FillEllipse(shell, p.X - s * 0.4f, p.Y - s * 0.3f, s * 0.8f, s * 0.3f);
                    g.FillEllipse(shell, p.X - s * 0.4f, p.Y - s * 0.6f, s * 0.8f, s * 0.3f);
                    g.FillRectangle(BrushOf(Color.FromArgb(255, 246, 236)), p.X - s * 0.35f, p.Y - s * 0.38f, s * 0.7f, s * 0.08f);
                    break;
                }
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
                g.FillEllipse(BrushOf(Color.FromArgb((int)(150 * (1 - p)), 245, 240, 235)), px - rad, py - rad, rad * 2, rad * 2);
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

        private static string LabelText(Bakery b) => $"{b.Name}";

        /// <summary>BP dont l'étiquette est sous le point donné ; la dernière dessinée (au premier plan) gagne.</summary>
        private Bakery? LabelAt(Point p)
        {
            for (int i = _labels.Count - 1; i >= 0; i--)
                if (_labels[i].Rect.Contains(p))
                    return _labels[i].Bakery;
            return null;
        }

        private void DrawLabel(Graphics g, RectangleF rect, GraphicsPath path, string text, Color color)
        {
            g.FillPath(BrushOf(Color.FromArgb(200, 40, 26, 36)), path);
            g.DrawPath(PenOf(color, 2), path);
            g.DrawString(text, _labelFont!, Brushes.White, rect.X + 8, rect.Y + 3);
        }

        private const string Hint = "Échap : quitter";

        private void DrawHud(Graphics g)
        {
            var dim = BrushOf(Color.FromArgb(210, 250, 236, 240));

            g.DrawString("UN MONDE DE DOUCEURS", _titleFont!, Brushes.White, 24, 18);
            g.DrawString($"Tick {(int)(_time * 2)}  ·  {_bakeries.Count} boulangeries-pâtisseries  ·  {_villas.Count} villas  ·  maillage {GridSize}×{GridSize}  ·  rendu GDI+ {_renderMs:0.0} ms",
                _smallFont!, dim, 28, 22 + _titleFont!.Height);

            g.DrawString(Hint, _smallFont!, dim, ClientSize.Width - _hintSize.Width - 24, ClientSize.Height - _hintSize.Height - 18);
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

        private void Fill(Graphics g, Color c, params PointF[] points) => g.FillPolygon(BrushOf(c), points);

        /// <summary>Pinceau uni mis en cache par couleur (les couleurs sont des entiers : le cache reste petit, même pour l'eau ou la fumée).</summary>
        private SolidBrush BrushOf(Color c)
        {
            int key = c.ToArgb();
            if (!_brushes.TryGetValue(key, out var brush))
                _brushes[key] = brush = new SolidBrush(c);
            return brush;
        }

        /// <summary>Stylo plein mis en cache par couleur et épaisseur ; vidé au redimensionnement (les épaisseurs dépendent de la taille des tuiles).</summary>
        private Pen PenOf(Color c, float width)
        {
            var key = (c.ToArgb(), width);
            if (!_pens.TryGetValue(key, out var pen))
                _pens[key] = pen = new Pen(c, width);
            return pen;
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
