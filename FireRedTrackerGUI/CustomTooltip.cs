using System;
using System.Drawing;
using System.Windows.Forms;

namespace FireRedTrackerGUI
{
    public class CustomTooltip : Form
    {
        private RichTextBox rtb;

        public CustomTooltip(string text, int x, int y)
        {
            // Fundo igual ao Form principal
            this.BackColor = Color.FromArgb(48, 48, 48);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
          

            // Borda leve
            this.Paint += (s, e) =>
            {
                int borderWidth = 2;
                Color borderColor = Color.Gray;
                e.Graphics.DrawRectangle(new Pen(borderColor, borderWidth),
                    0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            };

            // RichTextBox
            rtb = new RichTextBox();
            rtb.ReadOnly = true;
            rtb.BackColor = Color.FromArgb(48, 48, 48);
            rtb.ForeColor = Color.White;
            rtb.Font = new Font("Consolas", 10, FontStyle.Bold);
            rtb.Location = new Point(0, 0);
            rtb.Width = 300;
            rtb.ScrollBars = RichTextBoxScrollBars.None;
            rtb.Margin = new Padding(0);
            rtb.Padding = new Padding(5, 6, 5, 5); // padding para topo e laterais

            this.Controls.Add(rtb);

            // Adiciona texto e cores nos IVs
            SetText(text);

            // Ajusta altura automaticamente
            rtb.Height = rtb.GetPositionFromCharIndex(rtb.TextLength - 0).Y + rtb.Font.Height + 6;
            this.ClientSize = new Size(rtb.Width, rtb.Height);

            // Posiciona o tooltip
            this.Location = new Point(x, y);
            this.Click += (s, e) => this.Close();
            rtb.Click += (s, e) => this.Close();
        }

        public void SetText(string text)
        {
            rtb.Clear();
            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                if (line.StartsWith("IVs:"))
                {
                    rtb.AppendText("IVs: ");
                   var parts = line.Substring(5).Split('/');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (int.TryParse(parts[i], out int val))
                        {
                            if (val == 28 || val == 29)
                                rtb.SelectionColor = Color.CornflowerBlue; // azul
                            else if (val == 30 || val == 31)
                                rtb.SelectionColor = Color.Lime; // verde
                            else
                                rtb.SelectionColor = Color.White; // outros
                        }
                        else
                        {
                            rtb.SelectionColor = Color.White;
                        }

                        rtb.AppendText(parts[i]); // só o número recebe a cor

                        rtb.SelectionColor = Color.White; // separador volta para branco
                        if (i < parts.Length - 1)
                            rtb.AppendText("/"); // separador sempre branco
                    }
                    rtb.AppendText("\n");
                }
                else
                {
                    rtb.SelectionColor = Color.White;
                    rtb.AppendText(line + "\n");
                }
            }
        }
    }
}