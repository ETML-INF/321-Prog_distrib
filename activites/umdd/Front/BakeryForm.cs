namespace Front
{
    /// <summary>Fiche d'une boulangerie-pâtisserie, ouverte par un clic sur son étiquette.</summary>
    public partial class BakeryForm : Form
    {
        public BakeryForm(string title)
        {
            InitializeComponent();
            Text = title;
            lblTitle.Text = title;
        }
    }
}
