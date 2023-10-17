using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using UnityEngine.UI;


namespace OpenCvSharp
{
    public class shape : MonoBehaviour
    {
        public Texture2D originalimage;

        Mat originmat;
        Mat hsver;
        Mat bindata;
        Texture2D binary_tex;

        //抽出したい色のHSV形式での下限上限のベクトル
        private readonly static Scalar LOWER = new Scalar(50, 150, 10);
        private readonly static Scalar UPPER = new Scalar(100, 255, 255);
        void Start()
        {
            originmat = OpenCvSharp.Unity.TextureToMat(this.originalimage);
            hsver = new Mat();
            Cv2.CvtColor(originmat, hsver, ColorConversionCodes.BGR2HSV);
            bindata = hsver.InRange(LOWER, UPPER);
            binary_tex = OpenCvSharp.Unity.MatToTexture(bindata);
            GetComponent<RawImage>().texture = binary_tex;
        }

        //Updateを残す必要があるみたい、毎フレーム呼び出す必要があるから？
        void Update()
        {

        }
    }

}