using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SharpGL;

namespace _3DSExplorer
{
    public partial class frmAbout : Form
    {
        private const int TexturesNum = 3;
        private const int TextureCube3 = 0;
        private const int TextureCubeD = 1;
        private const int TextureCubeS = 2;
        private double _rquad;
        private readonly uint[] _textures = new uint[TexturesNum];

        private Bitmap _pokeBmp;
        private readonly ushort[] _pokedex = {
                                               0x06B1, 0x06B8, 0x86B0, 0x16AD, 0x96A5, 0x96AC, 0x16A4, 0x86AD, 0x06A5,
                                               0x06AC, 0x86A4, 0x1EA9, 0x9EA1, 0x9EA8, 0x1EA0, 0x8EA9, 0x0EA1, 0x0EA8, 
                                               0x8EA0, 0x731F, 0xF317, 0xF31E, 0x7316, 0xE31F, 0x6317, 0x631E, 0xE316, 
                                               0x7B1B, 0x6A05, 0xFB13, 0xFB1A, 0x7B12, 0xEB1B, 0x6B13, 0x6B1A, 0xEB12, 
                                               0x7B0F, 0xFB07, 0xFB0E, 0x7B06, 0xEB0F, 0x6B07, 0x6B0E, 0xEB06, 0x730B, 
                                               0xF303, 0xF30A, 0x7302, 0xE30B, 0x6303, 0x630A, 0xE302, 0x5B1F, 0xDB17, 
                                               0xDB1E, 0x5B16, 0xCB1F, 0x4B17, 0x7A19, 0x4B1E, 0xCB16, 0x531B, 0xD313, 
                                               0xD31A, 0xFA11, 0x5312, 0xC31B, 0x4313, 0x431A, 0xC312, 0x530F, 0xD307, 
                                               0xD30E, 0x5306, 0xC30F, 0x4307, 0x430E, 0xC306, 0x5B0B, 0xDB03, 0xDB0A,
                                               0x5B02, 0xCB0B, 0x4B03, 0x4B0A, 0xCB02, 0x739D, 0xF395, 0xF39C, 0x7394,
                                               0xE39D, 0x6395, 0x639C, 0xE394, 0x7B99, 0xFA18, 0x7A10, 0xEA19, 0xFB91,
                                               0x6A11, 0x6A18, 0xEA10, 0xFB98, 0x7B90, 0xEB99, 0x6B91, 0x6B98, 0xEB90, 
                                               0x6A0C, 0x7B8D, 0xEA04, 0xFB85, 0x5B9D, 0xDB95, 0xDB9C, 0x5B94, 0xCB9D,
                                               0x4B95, 0x4B9C, 0xCB94, 0x5399, 0xD391, 0xD398, 0x5390, 0xC399, 0x4391, 
                                               0x4398, 0xC390, 0x538D, 0xD385, 0xD38C, 0x5384, 0xC38D, 0x4385, 0x438C, 
                                               0xC384, 0x5B89, 0xDB81, 0xDB88, 0x5B80, 0xCB89, 0x4B81, 0x4B88, 0xCB80, 
                                               0x729F, 0xF297, 0xF29E, 0x7296, 0xE29F, 0x6297
                                           };

        private readonly int[][][] _cube = {  new[]{new[]{-1,1,-1}, new[]{-1,1,1}, new[]{1,1,1},new[]{1,1,-1}},
                                    new[]{new[]{-1,-1,1},new[]{-1,-1,-1}, new[]{1,-1,-1}, new[]{1,-1,1}},
                                    new[]{new[]{-1,-1,1}, new[]{1,-1,1}, new[]{1,1,1},new[]{-1,1,1}},
                                    new[]{new[]{1,-1,-1},new[]{-1,-1,-1}, new[]{-1,1,-1}, new[]{1,1,-1}},
                                    new[]{new[]{-1,-1,-1}, new[]{-1,-1,1}, new[]{-1,1,1},new[]{-1,1,-1}},
                                    new[]{new[]{1,-1,1},new[]{1,-1,-1}, new[]{1,1,-1}, new[]{1,1,1}}
                                };
        public frmAbout()
        {
            InitializeComponent();
            lblTitle.Text = 'v' + Application.ProductVersion;
            for (var i = 0; i < _pokedex.Length; i++)
                cmbDex.Items.Add(i+1);
            _pokeBmp = new Bitmap(54,54);
            Graphics.FromImage(_pokeBmp).FillRectangle(Brushes.Black, 0, 0, 53, 53);
            picDex.Image = _pokeBmp;
        }

        private void DrawQuadsWithTexture(OpenGL gl, int texture, params int[][][] quads)
        {
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _textures[texture]);
            gl.Begin(OpenGL.GL_QUADS);
            foreach (var quad in quads)
            {
                gl.TexCoord(0.0f, 0.0f); gl.Vertex(quad[0]);
                gl.TexCoord(1.0f, 0.0f); gl.Vertex(quad[1]);
                gl.TexCoord(1.0f, 1.0f); gl.Vertex(quad[2]);
                gl.TexCoord(0.0f, 1.0f); gl.Vertex(quad[3]);
            }
            gl.End();
        }

        private void openGLControl1_OpenGLDraw(object sender, PaintEventArgs e)
        {
            var gl = openGLControl1.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            gl.Translate(0.0f, 0.0f, -5.0f); //Camera
            gl.Rotate(50, 1.0f, 0.5f, 0.5f);  //Rotation
            gl.Rotate(_rquad += 3.0f, 0.0f, 1.0f, 0.0f);

            DrawQuadsWithTexture(gl, TextureCube3, _cube[0]); // _cube[1] isn't visible
            DrawQuadsWithTexture(gl, TextureCubeD, _cube[2], _cube[3]);
            DrawQuadsWithTexture(gl, TextureCubeS, _cube[4], _cube[5]);
            gl.Flush();
        }

        private void BindBitmapToTexture(OpenGL gl, Bitmap bmp, int textureNumber)
        {
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, _textures[textureNumber]);
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, 3, bmp.Width, bmp.Height, 0, 
                OpenGL.GL_BGR, 
                OpenGL.GL_UNSIGNED_BYTE, 
                bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height), 
                    ImageLockMode.ReadOnly, 
                    PixelFormat.Format24bppRgb
                    ).Scan0
                );
            //  Specify linear filtering.
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            // the texture wraps over at the edges (repeat)
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
        }

        private void openGLControl1_OpenGLInitialized(object sender, System.EventArgs e)
        {
            //  Get the OpenGL object, for quick access.
            var gl = openGLControl1.OpenGL;

            //  A bit of extra initialisation here, we have to enable textures.
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            //  Get one texture id, and stick it into the textures array.
            gl.GenTextures(TexturesNum, _textures);
            //  Make the textures
            BindBitmapToTexture(gl, Properties.Resources.cube_3, TextureCube3);
            BindBitmapToTexture(gl, Properties.Resources.cube_d, TextureCubeD);
            BindBitmapToTexture(gl, Properties.Resources.cube_s, TextureCubeS);
        }

        private void cmbDex_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var g = Graphics.FromImage(picDex.Image);
            var val = _pokedex[cmbDex.SelectedIndex];
            for (var y = 0; y < 4; y++)
                for (var x = 0; x < 4; x++)
                    g.FillRectangle((((val >> (15 - x - y * 4))) & 1) > 0 ? Brushes.Black : Brushes.White, 9 + x * 9, 9 + y * 9, 9, 9);
            picDex.Invalidate();

        }
    }
}
