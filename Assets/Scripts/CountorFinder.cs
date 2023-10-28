using OpenCvSharp;
using OpenCvSharp.Demo;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CountorFinder : WebCamera
{
    [SerializeField] private FlipMode ImageFlip;
    [SerializeField] private float Threshold = 96.4f;
    [SerializeField] private float CurveAccuracy = 10f;
    [SerializeField] private float MinArea = 5000f;
    //[SerializeField] private PolygonCollider2D PolygonCollider;
    private enum ShowProcessingImage
    {
        Origin = 0,
        BinaryInv = 1,
        OriginContour = 2
    }
    [SerializeField] private ShowProcessingImage showProcessingImage;
    [SerializeField] private bool ShowKeystoneCorrection = false;
    [SerializeField] private bool ShowConvertImage = false;
    [SerializeField] Camera _cam;
    [SerializeField] private GameObject LeftUp;
    [SerializeField] private GameObject LeftDown;
    [SerializeField] private GameObject RightUp;
    [SerializeField] private GameObject RightDown;

    private float aveWidth = 2560;
    private float aveHeight = 1440;
    private Mat image;
    private Mat processImage = new Mat();
    private Mat processOriginImage = new Mat();
    private Mat convertImage = new Mat();
    private Point[][] contours;
    private HierarchyIndex[] hierarchy;
    private Vector2[] vectorList;
    private Point2f _LeftUp;
    private Point2f _RightDown;
    private Point2f _RightUp;
    private Point2f _LeftDown;


    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        image = OpenCvSharp.Unity.TextureToMat(input);

        Cv2.Flip(image, image, ImageFlip);

        transformKeystone(image);

        if(ShowConvertImage == true)
        {
            
            Cv2.CvtColor(convertImage, processImage, ColorConversionCodes.BGR2GRAY);
            convertImage.CopyTo(image);
        }
        else
        {
            Cv2.CvtColor(image, processImage, ColorConversionCodes.BGR2GRAY);
        }   
        Cv2.Threshold(processImage, processImage, Threshold, 255, ThresholdTypes.BinaryInv);
        Cv2.FindContours(processImage, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);
        image.CopyTo(processOriginImage);

        //PolygonCollider.pathCount = 0;
        foreach (Point[] contour in contours)
        {
            Point[] points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true);
            var area = Cv2.ContourArea(contour);

            if (area > MinArea)
            {
                drawContour(processImage, new Scalar(127, 127, 127), 3, points);
                drawContour(processOriginImage, new Scalar(255, 0, 0), 3, points);

                //PolygonCollider.pathCount++;
                //PolygonCollider.SetPath(PolygonCollider.pathCount - 1, toVector2(points));
            }
        }

        if (output == null)
            output = OpenCvSharp.Unity.MatToTexture(OutputTexture(showProcessingImage));
        else
            OpenCvSharp.Unity.MatToTexture(OutputTexture(showProcessingImage), output);

        return true;
    }

    private Vector2[] toVector2(Point[] points)
    {
        vectorList = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vectorList[i] = new Vector2(points[i].X, points[i].Y);
        }
        return vectorList;
    }

    private void drawContour(Mat Image, Scalar Color, int Thickness, Point[] Points)
    {
        for (int i = 1; i < Points.Length; i++)
        {
            Cv2.Line(Image, Points[i - 1], Points[i], Color, Thickness);
        }
        Cv2.Line(Image, Points[Points.Length - 1], Points[0], Color, Thickness);
    }

    private Mat OutputTexture(ShowProcessingImage number)
    {
        switch(number)
        {
            case ShowProcessingImage.Origin:
                return image;
            case ShowProcessingImage.BinaryInv:
                return processImage;
            case ShowProcessingImage.OriginContour:
                return processOriginImage;
        }
        return null;
    }

    private int VectorToMatposX (float pos)
    {
        return Mathf.RoundToInt((pos  * 125/3) + GetComponent<RectTransform>().sizeDelta.x/2);
    }

    private int VectorToMatposY(float pos)
    {
        return Mathf.RoundToInt(-(pos  * 125 / 3) + GetComponent<RectTransform>().sizeDelta.y/2 + 1.5f);
    }

    private void drawKeystone(Mat input)
    {
        LeftUp.SetActive(false);
        RightUp.SetActive(false);
        LeftDown.SetActive(false);
        RightDown.SetActive(false);

        if (ShowKeystoneCorrection == true)
        {
            LeftUp.SetActive(true);
            RightUp.SetActive(true);
            LeftDown.SetActive(true);
            RightDown.SetActive(true);

            Cv2.Circle(input, VectorToMatposX(LeftUp.transform.position.x), VectorToMatposY(LeftUp.transform.position.y), 20, new Scalar(0, 255, 0), -1);
            Cv2.Circle(input, VectorToMatposX(LeftDown.transform.position.x), VectorToMatposY(LeftDown.transform.position.y), 20, new Scalar(0, 255, 0), -1);
            Cv2.Circle(input, VectorToMatposX(RightUp.transform.position.x), VectorToMatposY(RightUp.transform.position.y), 20, new Scalar(0, 255, 0), -1);
            Cv2.Circle(input, VectorToMatposX(RightDown.transform.position.x), VectorToMatposY(RightDown.transform.position.y), 20, new Scalar(0, 255, 0), -1);

            Cv2.Line(input, VectorToMatposX(LeftUp.transform.position.x), VectorToMatposY(LeftUp.transform.position.y), VectorToMatposX(LeftDown.transform.position.x), VectorToMatposY(LeftDown.transform.position.y), new Scalar(0, 255, 0), 3);
            Cv2.Line(input, VectorToMatposX(LeftDown.transform.position.x), VectorToMatposY(LeftDown.transform.position.y), VectorToMatposX(RightDown.transform.position.x), VectorToMatposY(RightDown.transform.position.y), new Scalar(0, 255, 0), 3);
            Cv2.Line(input, VectorToMatposX(RightDown.transform.position.x), VectorToMatposY(RightDown.transform.position.y), VectorToMatposX(RightUp.transform.position.x), VectorToMatposY(RightUp.transform.position.y), new Scalar(0, 255, 0), 3);
            Cv2.Line(input, VectorToMatposX(RightUp.transform.position.x), VectorToMatposY(RightUp.transform.position.y), VectorToMatposX(LeftUp.transform.position.x), VectorToMatposY(LeftUp.transform.position.y), new Scalar(0, 255, 0), 3);
        }
    }

    private void transformKeystone(Mat input)
    {
        drawKeystone(input);

        if (ShowConvertImage == true)
        {
            _LeftUp = new Point2f((LeftUp.transform.position.x * 125 / 3) + GetComponent<RectTransform>().sizeDelta.x / 2, -(LeftUp.transform.position.y * 125 / 3) + GetComponent<RectTransform>().sizeDelta.y / 2);
            _LeftDown = new Point2f((LeftDown.transform.position.x * 125 / 3) + GetComponent<RectTransform>().sizeDelta.x / 2, -(LeftDown.transform.position.y * 125 / 3) + GetComponent<RectTransform>().sizeDelta.y / 2);
            _RightUp = new Point2f((RightUp.transform.position.x * 125 / 3) + GetComponent<RectTransform>().sizeDelta.x / 2, -(RightUp.transform.position.y * 125 / 3) + GetComponent<RectTransform>().sizeDelta.y / 2);
            _RightDown = new Point2f((RightDown.transform.position.x * 125 / 3) + GetComponent<RectTransform>().sizeDelta.x / 2, -(RightDown.transform.position.y * 125 / 3) + GetComponent<RectTransform>().sizeDelta.y / 2);

            var edjePoints = new Point2f[]
                {
                    _LeftUp,
                    _LeftDown,
                    _RightUp,
                    _RightDown,
                };

            var outPoints = new Point2f[]
            {
                    new Point2f(0,0),
                    new Point2f(0,aveHeight),
                    new Point2f(aveWidth, 0),
                    new Point2f(aveWidth,aveHeight),
            };

            Mat transMat = Cv2.GetPerspectiveTransform(edjePoints, outPoints);
            OpenCvSharp.Size imageSize = new OpenCvSharp.Size(aveWidth, aveHeight);

            Cv2.WarpPerspective(input, convertImage, transMat, imageSize);
        }
    }
}