namespace OpenCvSharp
{
    using UnityEngine;
    using System.Collections;
    using OpenCvSharp;
    using UnityEngine.UI;

    public class CircleDetected : MonoBehaviour
    {
        [SerializeField]
        [Range(0.0f, 255f)]
        double lower = 110;
        [SerializeField]
        [Range(0f, 255f)]
        double upper = 255;

        [SerializeField]
        [Range(0, 100)]
        int minNum = 15;

        [SerializeField]
        [Range(0, 100)]
        int maxNum = 20;


        //public Texture2D texture;
        public WebCamTexture webCamTexture = null;

        // Use this for initialization
        void Start()
        {
            //webCamTexture = GetComponent<WebCamController>().webCamTexture;
        }

        // Update is called once per frame
        void Update()
        {

            //Load texture
            Mat image = Unity.TextureToMat(this.webCamTexture);

            //Gray scale image
            Mat grayMat = new Mat();
            Cv2.CvtColor(image, grayMat, ColorConversionCodes.BGR2GRAY);


            Mat thresh = new Mat();
            Cv2.Threshold(grayMat, thresh, lower, upper, ThresholdTypes.BinaryInv);


            //Extract Contours
            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(thresh, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, null);

            foreach (Point[] contour in contours)
            {
                double length = Cv2.ArcLength(contour, true);
                Point[] approx = Cv2.ApproxPolyDP(contour, length * 0.01, true);
                string shapeName = null;
                Scalar color = new Scalar();


                //if (approx.Length == 3)
                //{
                //    shapeName = "Triangle";
                //    color = new Scalar(0, 255, 0);
                //}
                //else if (approx.Length == 4)
                //{
                //    OpenCvSharp.Rect rect = Cv2.BoundingRect(contour);
                //    if (rect.Width / rect.Height <= 0.1)
                //    {
                //        shapeName = "Square";
                //        color = new Scalar(0, 125, 255);
                //    }
                //    else
                //    {
                //        shapeName = "Rectangle";
                //        color = new Scalar(0, 0, 255);
                //    }
                //}
                //else if (approx.Length == 10)
                //{
                //    shapeName = "Star";
                //    color = new Scalar(255, 255, 0);
                //}
                //else if (approx.Length >= 15)
                //{
                //    shapeName = "Circle";
                //    color = new Scalar(0, 255, 255);
                //}

                if(maxNum > approx.Length  && approx.Length >= minNum)
                {
                    shapeName = "Name";
                    color = new Scalar(0, 255, 255);
                }

                if (shapeName != null)
                {
                    Moments m = Cv2.Moments(contour);
                    int cx = (int)(m.M10 / m.M00);
                    int cy = (int)(m.M01 / m.M00);

                    Cv2.DrawContours(image, new Point[][] { contour }, 0, color, -1);
                    Cv2.PutText(image, shapeName, new Point(cx - 50, cy), HersheyFonts.HersheySimplex, 1.0, new Scalar(0, 0, 0));
                }
            }
            // Render texture
            Texture2D texture = Unity.MatToTexture(image);
            RawImage rawImage = gameObject.GetComponent<RawImage>();
            rawImage.texture = texture;
        }


    }
}