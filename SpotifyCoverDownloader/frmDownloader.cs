using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace SpotifyCoverDownloader
{
    public partial class frmDownloader : Form
    {
        private HttpClient httpClient;

        public frmDownloader()
        {
            InitializeComponent();

            // Create the HttpClient instance with User-Agent header
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36");

        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            string spotifyTrackUrl = lblAlbumUrl.Text;
            string oembedEndpoint = "https://open.spotify.com/oembed?url=" + spotifyTrackUrl;

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(oembedEndpoint);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                JObject json = JObject.Parse(jsonResponse);
                string picUrl = json["thumbnail_url"].ToString();

                pbCover.Load(picUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }
        }

        private void lblAlbumUrl_Click(object sender, EventArgs e)
        {
            lblAlbumUrl.Text = Clipboard.GetText();
        }

        private void frmDownloader_Load(object sender, EventArgs e)
        {

        }

        private void pbCover_Click(object sender, EventArgs e)
        {
            string download = Environment.GetEnvironmentVariable("USERPROFILE") + @"\" + "Downloads" + @"\" + "cover.jpg";

            try
            {
                // Check if file exists with its full path
                if (System.IO.File.Exists(download))
                {
                    // If file found, delete it
                    System.IO.File.Delete(download);
                }
            }
            catch (System.IO.IOException ioExp)
            {
                Console.WriteLine(ioExp.Message);
            }

            pbCover.Image.Save(download, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    public static class Clipboard
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetClipboardData(uint uFormat);
        [DllImport("user32.dll")]
        static extern bool IsClipboardFormatAvailable(uint format);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool CloseClipboard();
        [DllImport("kernel32.dll")]
        static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        static extern bool GlobalUnlock(IntPtr hMem);

        const uint CF_UNICODETEXT = 13;

        public static string GetText()
        {
            if (!IsClipboardFormatAvailable(CF_UNICODETEXT))
                return null;
            if (!OpenClipboard(IntPtr.Zero))
                return null;

            string data = null;
            var hGlobal = GetClipboardData(CF_UNICODETEXT);
            if (hGlobal != IntPtr.Zero)
            {
                var lpwcstr = GlobalLock(hGlobal);
                if (lpwcstr != IntPtr.Zero)
                {
                    data = Marshal.PtrToStringUni(lpwcstr);
                    GlobalUnlock(lpwcstr);
                }
            }
            CloseClipboard();

            return data;
        }
    }
}
