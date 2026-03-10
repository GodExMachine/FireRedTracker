using System;
using System.IO;
using System.Windows.Forms;
using PKHeX.Core;
using System.Drawing;

namespace FireRedTrackerGUI
{
    public partial class Form1 : Form
    {
        string savePath = @"..\Pokemon - Fire Red.sav";
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        FlowLayoutPanel partyPanel = new FlowLayoutPanel();
        ToolTip tip = new ToolTip();
        PictureBox activeSprite = null;
        DateTime lastRead = DateTime.MinValue;

        

        public Form1()
        {
            InitializeComponent();

            // Janela vertical fina
            this.Text = "FireRed Tracker";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Width = 180;
            this.Height = 880;
            this.StartPosition = FormStartPosition.Manual;
            this.Top = 0;
            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.BackColor = Color.FromArgb(48, 48, 48);
            this.Font = new Font("Consolas", 10, FontStyle.Bold);

            // Painel vertical para sprites
            partyPanel.Dock = DockStyle.Fill;
            partyPanel.WrapContents = false;
            partyPanel.FlowDirection = FlowDirection.TopDown;
            partyPanel.AutoScroll = true;
            this.Controls.Add(partyPanel);

            // Tooltip instantâneo
            tip.InitialDelay = 0;
            tip.ReshowDelay = 0;
            tip.AutoPopDelay = 60000; // 1 min
            tip.ShowAlways = true;

            // Timer para atualizar a party
            timer.Interval = 1000;
            timer.Tick += UpdateParty;
            timer.Start();
        }

        string GetSpriteName(string species)
        {
            species = species.ToLower();
            species = species.Replace(" ", "-").Replace(".", "").Replace("'", "");
            species = species.Replace("♀", "-f").Replace("♂", "-m");
            return species;
        }

       CustomTooltip activeTooltip = null;

private void UpdateParty(object sender, EventArgs e)
{
    if (!File.Exists(savePath)) return;

    var lastWrite = File.GetLastWriteTime(savePath);
    if (lastWrite == lastRead) return;
    lastRead = lastWrite;

    var data = File.ReadAllBytes(savePath);
    var sav = new SAV3FRLG(data);

    partyPanel.Controls.Clear();

    foreach (var p in sav.PartyData)
    {
        if (p.Species == 0) continue;

        string species = GameInfo.Strings.Species[p.Species];
        string ability = GameInfo.Strings.Ability[p.Ability];
        string item = GameInfo.Strings.Item[p.HeldItem];
        string m1 = GameInfo.Strings.Move[p.Move1];
        string m2 = GameInfo.Strings.Move[p.Move2];
        string m3 = GameInfo.Strings.Move[p.Move3];
        string m4 = GameInfo.Strings.Move[p.Move4];
        string hpType = GameInfo.Strings.Types[p.HPType];
        string nature = GameInfo.Strings.Natures[p.Nature];

        string commentText =
            $"Level: {p.CurrentLevel}\n" +
            $"Nature: {nature}\n" +
            $"Ability: {ability}\n" +
            $"Item: {item}\n" +
            $"Hidden Power: {hpType} ({p.HPPower})\n" +
            "HP/ATK/DEF/SPA/SPD/SPE \n" +
            $"IVs: {p.IV_HP}/{p.IV_ATK}/{p.IV_DEF}/{p.IV_SPA}/{p.IV_SPD}/{p.IV_SPE}\n" +
            $"EVs: {p.EV_HP}/{p.EV_ATK}/{p.EV_DEF}/{p.EV_SPA}/{p.EV_SPD}/{p.EV_SPE}\n" +
            $"Moves:\n{m1}\n{m2}\n{m3}\n{m4}";

        string spriteName = GetSpriteName(species);
        string url = $"https://img.pokemondb.net/sprites/ruby-sapphire/normal/{spriteName}.png";

        PictureBox pb = new PictureBox();
        pb.Width = 128;
        pb.Height = 128;
        pb.SizeMode = PictureBoxSizeMode.Zoom;
        pb.ImageLocation = url;
        pb.Margin = new Padding(5);

        pb.MouseDown += (s, ev) =>
        {
            if (ev.Button == MouseButtons.Left)
            {
                if (activeTooltip != null)
                {
                    activeTooltip.Close();
                    activeTooltip = null;
                }
                else
                {
                    var screenPos = pb.PointToScreen(Point.Empty);

                    int tooltipX = screenPos.X + pb.Width + 5;
                    int tooltipY = screenPos.Y;

                    int screenRight = Screen.PrimaryScreen.WorkingArea.Right;
                    int tooltipWidth = 300;
                    if (tooltipX + tooltipWidth > screenRight)
                        tooltipX = screenPos.X - tooltipWidth - 5;

                    int screenBottom = Screen.PrimaryScreen.WorkingArea.Bottom;
                    int tooltipHeight = 200;
                    if (tooltipY + tooltipHeight > screenBottom)
                        tooltipY = screenBottom - tooltipHeight - 5;

                    activeTooltip = new CustomTooltip(commentText, tooltipX, tooltipY);
                    activeTooltip.Show();
                }
            }
            else if (ev.Button == MouseButtons.Right)
            {
                string movesUrl = species.ToLower()
                    .Replace(" ", "-")
                    .Replace(".", "")
                    .Replace("'", "")
                    .Replace("♀", "-f")
                    .Replace("♂", "-m");

                string link = $"https://pokemondb.net/pokedex/{movesUrl}/moves/3";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = link,
                    UseShellExecute = true
                });
            }
        };

        partyPanel.Controls.Add(pb);
    }
}
       


    }
}