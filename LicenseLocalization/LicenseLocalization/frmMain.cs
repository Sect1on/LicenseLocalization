using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;                        
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using Emgu.Util;
using Emgu.CV.UI;

namespace LicenseLocalization
{
    public partial class frmMain : Form
    {
        Image<Bgr, byte> OriginPic = new Image<Bgr, byte>(@"..\..\..\Pictures\1.jpg");//从文件加载图片

        //图片大小320*240像素，格式jpg
        Image<Bgr, byte> Transferpic;
        Image<Bgr, byte> Licensepic;
        Image<Bgr, byte> chapic1;
        Image<Bgr, byte> chapic2;
        Image<Bgr, byte> chapic3;
        Image<Bgr, byte> chapic4;
        Image<Bgr, byte> chapic5;
        Image<Bgr, byte> chapic6;
        Image<Bgr, byte> chapic7;
        Image<Gray, byte> houghpic;


        public frmMain()
        {
            InitializeComponent();
        }
        public void SetRoiRed(Image<Bgr, byte> image, Rectangle roi)//ROI选取函数
        {
            image.ROI = roi;
            //image.ROI = Rectangle.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i, j;
            int[,] picmat = new int[330, 250];
            int sum;
            int Ws=0, We=0, Hs=0, He=0;

            picOrigin.Image = OriginPic.Bitmap;
            Transferpic = OriginPic.Copy();
            Licensepic = OriginPic.Copy();
            

            for (i = 0; i < 320; i++)
            {
                for (j = 0; j < 240; j++)
                {
                    Color pix=Transferpic.Bitmap.GetPixel(i,j);
                    if (pix.R > 50 || pix.B < 60)
                    {
                        Transferpic.Bitmap.SetPixel(i, j, Color.White);
                        picmat[i, j] = 0;
                    }
                    else
                    {
                        Transferpic.Bitmap.SetPixel(i, j, Color.Black);
                        picmat[i, j] = 1;
                    }
                    
                }
            }
            picRec.Image = Transferpic.Bitmap;

            for (i = 50; i < 320; i++)
            {
                sum = 0;
                for (j=80;j<240;j++)
                {
                    sum = sum + picmat[i, j];
                }

                if(sum>20)
                {
                    Ws = i;
                    break;
                }
                else
                {
                    continue;
                }
            }

            for(i=Ws;i<320;i++)
            {
                sum = 0;
                for (j = 80; j < 240; j++)
                {
                    sum = sum + picmat[i, j];
                }

                if (sum < 2)
                {
                    We = i;
                    break;
                }
                else
                {
                    continue;
                }
            }

            for (j = 80; j < 240; j++)
            {
                sum = 0;
                for (i = 50; i < 320; i++)
                {
                    sum = sum + picmat[i, j];
                }

                if (sum > 50)
                {
                    Hs = j;
                    break;
                }
                else
                {
                    continue;
                }
            }

            for (j = Hs; j < 240; j++)
            {
                sum = 0;
                for (i = 50; i < 320; i++)
                {
                    sum = sum + picmat[i, j];
                }

                if (sum < 5)
                {
                    He = j;
                    break;
                }
                else
                {
                    continue;
                }
            }

            if(He-Hs==0)
            {
                He = He + 1;
                MessageBox.Show("System Error");
            }
            
            if(We-Ws==0)
            {
                We = We + 1;
                MessageBox.Show("System Error");
            }

            OriginPic.Draw(new Rectangle(Ws, Hs, We - Ws, He - Hs),new Bgr(0,0,255) , 2);
            SetRoiRed(Licensepic, new Rectangle(Ws, Hs, We - Ws, He - Hs));
            picOrigin.Image = OriginPic.Bitmap;
            picLicense.Image = Licensepic.Bitmap;

            

            chapic1 = Licensepic.Copy();
            chapic2 = Licensepic.Copy();
            chapic3 = Licensepic.Copy();
            chapic4 = Licensepic.Copy();
            chapic5 = Licensepic.Copy();
            chapic6 = Licensepic.Copy();
            chapic7 = Licensepic.Copy();

            SetRoiRed(chapic1, new Rectangle(0,0,(We-Ws)/7,He-Hs));
            piccha1.Image = chapic1.Bitmap;

            SetRoiRed(chapic2, new Rectangle((We - Ws) / 7, 0, (We - Ws) / 7, He - Hs));
            piccha2.Image = chapic2.Bitmap;

            SetRoiRed(chapic3, new Rectangle((We - Ws)*2 / 7+ (We - Ws)/20, 0, (We - Ws) / 7, He - Hs));
            piccha3.Image = chapic3.Bitmap;

            SetRoiRed(chapic4, new Rectangle((We - Ws) * 3 / 7 + (We - Ws) / 30, 0, (We - Ws) / 7, He - Hs));
            piccha4.Image = chapic4.Bitmap;

            SetRoiRed(chapic5, new Rectangle((We - Ws) * 4 / 7 + (We - Ws) / 30, 0, (We - Ws) / 7, He - Hs));
            piccha5.Image = chapic5.Bitmap;

            SetRoiRed(chapic6, new Rectangle((We - Ws) * 5 / 7, 0, (We - Ws) / 7, He - Hs));
            piccha6.Image = chapic6.Bitmap;

            SetRoiRed(chapic7, new Rectangle((We - Ws) * 6 / 7, 0, (We - Ws) / 7, He - Hs));
            piccha7.Image = chapic7.Bitmap;

            //切割
            

            var grayImage = Transferpic.Convert<Gray, Byte>();
            Image<Gray, byte> Cannypic = new Image<Gray, byte>(Transferpic.Width,Transferpic.Height);
            Image<Gray, byte> Thresholdpic = new Image<Gray, byte>(Transferpic.Width, Transferpic.Height);
            Image<Gray, byte> Contourspic = new Image<Gray, byte>(Transferpic.Width, Transferpic.Height);
            Image<Bgr, byte> Drawpic = new Image<Bgr, byte>(Transferpic.Width, Transferpic.Height);
            VectorOfVectorOfPoint con1 = new VectorOfVectorOfPoint();
            IntPtr Dynstorage = CvInvoke.cvCreateMemStorage(0);

            CvInvoke.cvThreshold(grayImage, Thresholdpic , 0.0001,255.0,THRESH.CV_THRESH_OTSU);
            CvInvoke.cvCanny(grayImage, Cannypic, 60, 100, 3);

            IntPtr Dyncontour = new IntPtr();
            MCvContour con = new MCvContour();
            CvInvoke.cvFindContours(Thresholdpic, Dynstorage, ref Dyncontour, Marshal.SizeOf(con), RETR_TYPE.CV_RETR_TREE, CHAIN_APPROX_METHOD.CV_LINK_RUNS, new Point(0, 0));
            CvInvoke.cvDrawContours(Contourspic, Dyncontour, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 0), 2, 1, LINE_TYPE.CV_AA, new Point(0, 0));

            //待施工
            
            //CvInvoke.cvMinAreaRect2(Dynstorage, Contourspic);//???

            picLaststep.Image = Contourspic.Bitmap;

            houghpic = Contourspic.Copy();
            IntPtr houghline = CvInvoke.cvCreateMemStorage(0);
            ////int[,] linemat = new int[5, 1];
            CvInvoke.cvHoughLines2(houghpic, houghline, HOUGH_TYPE.CV_HOUGH_STANDARD, 1.0, 0.01745, 1, 50, 0);
            
            //picLaststep.Image = houghpic.Bitmap;


            grayImage.Dispose();
            OriginPic.Dispose();
            Transferpic.Dispose();
            Cannypic.Dispose();
            Contourspic.Dispose();
            Drawpic.Dispose();
        }
    }
}
