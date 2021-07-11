using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yarhl.IO;

namespace UniFont
{
    public partial class Form1 : Form
    {
        private int x = 128;
        private int y = 72;
        private int xSpacing = 72;
        private int ySpacing = 81;
        private int count = 16;
        private int rows = 10;
        private List<TextBox> textBoxes;
        private DataStream stream;

        public Form1()
        {
            InitializeComponent();
            textBoxes = new List<TextBox>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var path = OpenFile();

            if (string.IsNullOrWhiteSpace(path))
                return;

            stream = DataStreamFactory.FromFile(path, FileOpenMode.ReadWrite);
            var reader = new DataReader(stream) {Stream = {Position = 0x5}};

            var yMod = y;

            for (int i = 0; i < rows; i++)
            {
                var xMod = x;
                for (int j = 0; j < count; j++)
                {
                    var textBox = new TextBox()
                    {
                        Location = new Point(xMod, yMod),
                        Size = new Size(49, 10),
                        Font = new Font(FontFamily.GenericSansSerif, 8),
                        Text = reader.ReadByte().ToString(),
                        TextAlign = HorizontalAlignment.Center,

                    };
                    textBox.KeyPress += KeyPressOnlyNumeric;
                    textBoxes.Add(textBox);
                    xMod += xSpacing;
                }
                yMod += ySpacing;
            }

            
            Controls.AddRange(textBoxes.ToArray());
            textBoxes.ForEach(x => x.BringToFront());
            pictureBox1.SendToBack();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBoxes.Count == 0)
            {
                MessageBox.Show("You need first to load the font before saving.", "Font not loaded", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var path = SaveFile();

            if (string.IsNullOrWhiteSpace(path))
                return;

            var writer = new DataWriter(CloneStrean()) {Stream = {Position = 0x5}};
            foreach (var box in textBoxes)
            {
                var val = Convert.ToInt32(box.Text);
                if (val > 256)
                    val = 255;
                writer.Write(Convert.ToByte(val));
            }
            writer.Stream.WriteTo(path);
            MessageBox.Show("The custom font has been saved.", "Font updated!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private DataStream CloneStrean()
        {
            var newStream = DataStreamFactory.FromMemory();
            stream.Position = 0;
            stream.WriteTo(newStream);
            return newStream;
        }

        private string OpenFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "WIDTH.bin|*.bin|All files (*.*)|*.*", RestoreDirectory = true
            };

            return openFileDialog.ShowDialog() == DialogResult.OK ? openFileDialog.FileName : string.Empty;
        }


        private string SaveFile()
        {
            var saveFileDialog1 = new SaveFileDialog
            {
                Filter = "WIDTH.bin|*.bin|All files (*.*)|*.*", RestoreDirectory = true
            };


            return saveFileDialog1.ShowDialog() == DialogResult.OK ? saveFileDialog1.FileName : string.Empty;
        }

        private void KeyPressOnlyNumeric(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
