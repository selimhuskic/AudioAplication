using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;
using NAudio.Wave;
using AudioApp.Helper;
using WMPLib;

namespace AudioApp
{
    public partial class Form1 : Form
    {
        public WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }
        private void button3_Click(object sender, EventArgs e)
        {

            Stream myStream = null;

            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "audio Files(*.wav;*.mp3;*)|*.wav;*.mp3;|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

        
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {

                   /*if (!openFileDialog1.FileName.EndsWith("wav") ||
                            !openFileDialog1.FileName.EndsWith("mp3"))
                            throw new Exception(message: "You have to choose a wav or mp3 file!");  */
                            
                        using (myStream)
                        {
                            
                        }
                    }

                    textBox1.Text = openFileDialog1.FileName;
                    wplayer.URL = textBox1.Text;
                    wplayer.controls.stop();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
             
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            
                if (textBox1.Text != "")
                {
                    wplayer.controls.stop();
                }
                
        }
        private void button4_Click(object sender, EventArgs e)
        {
            
            try {
                if(textBox1.Text != "")
                {                   
                    wplayer.controls.play();
                }
                else
                    throw new Exception(message: "You have to choose a wav or mp3 file!");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";



            var outputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "NAudio");
            Directory.CreateDirectory(outputFolder);
            string FileName = KeyGeneretor.GetUniqueKey(8);
            var outputFilePath = Path.Combine(outputFolder, FileName + ".wav");


            var waveIn = new WaveInEvent();
         
            WaveFileWriter writer = null;

            bool closing = false;

            var format = new WaveFormat(16000, 16, 1);

            writer = new WaveFileWriter(outputFilePath, format);
            waveIn.StartRecording();
            button1.Enabled = false;
            button2.Enabled = true;

            /*WaveFileReader reader = new NAudio.Wave.WaveFileReader(outputFilePath);
            WaveFormat newFormat = new WaveFormat(8000, 16, 1);
            WaveFormatConversionStream str = new WaveFormatConversionStream(newFormat, reader);*/


            waveIn.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);
                if (writer.Position > waveIn.WaveFormat.AverageBytesPerSecond * 30)
                {
                    waveIn.StopRecording();
                }
            };


            button2.Click += (s, a) => 
                waveIn.StopRecording();

            FormClosing += (s, a) => { closing = true; waveIn.StopRecording(); };

           
            waveIn.RecordingStopped += (s, a) =>
            {
                writer?.Dispose();
                writer = null;
                button1.Enabled = true;
                button2.Enabled = true;

                if (closing)
                {
                    waveIn.Dispose();
                }
            };
            
        }     
    }
}
