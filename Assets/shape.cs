using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using UnityEngine.UI;
using TMPro.EditorUtilities;

namespace OpenCvSharp
{
    public class Shape : MonoBehaviour
    {

        // public Texture2D originalimage;
        public WebCamTexture originalimage;

        Mat originmat;
        Mat hsver;
        Mat gray = new Mat();
        Mat bindata = new Mat();
        Texture2D binary_tex;

        [SerializeField]
        [Range(0.0f, 255f)]
        double lower = 110;
        [SerializeField]
        [Range(0f,255f)]
        double upper = 255;

        //抽出したい色のHSV形式での下限上限のベクトル
        [SerializeField]
        private Scalar LOWER = new Scalar(0,0,150);
        [SerializeField]
        private Scalar UPPER = new Scalar(179,100,255);
        void Start()
        {
            originalimage = GetComponent<WebCamController>().webCamTexture;
        }

        //Updateを残す必要があるみたい、毎フレーム呼び出す必要があるから？
        //void Update()
        //{
        //    originmat = OpenCvSharp.Unity.TextureToMat(this.originalimage);
        //    hsver = new Mat();
        //    Cv2.CvtColor(originmat, hsver, ColorConversionCodes.BGR2HSV);
        //    bindata = hsver.InRange(LOWER, UPPER);
        //    binary_tex = OpenCvSharp.Unity.MatToTexture(bindata);
        //    GetComponent<RawImage>().texture = binary_tex;
        //}

        void Update()
        {
            originmat = OpenCvSharp.Unity.TextureToMat(this.originalimage);
            Cv2.CvtColor(originmat, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, bindata, lower, upper, ThresholdTypes.BinaryInv);

            GetComponent<RawImage>().texture = OpenCvSharp.Unity.MatToTexture(this.bindata);
        }

    }

}