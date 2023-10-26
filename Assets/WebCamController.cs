namespace OpenCvSharp
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class WebCamController : MonoBehaviour
    {
        int width = 1920;
        int height = 1080;
        int fps = 30;
        Texture2D cap_tex;
        Texture2D out_tex;
        public WebCamTexture webCamTexture;
        Color32[] colors = null;
        [SerializeField]
        int cam = 0;

        IEnumerator Init()
        {
            while (true)
            {
                if(webCamTexture.width > 16 && webCamTexture.height > 16)
                {
                    colors = new Color32[webCamTexture.width * webCamTexture.height];
                    cap_tex = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
                    //GetComponent<Renderer>().material,mainTexture = texture;
                    break;
                }
                yield return null;
            }
        }

        private void Awake()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            webCamTexture = new WebCamTexture(devices[cam].name, this.width, this.height, this.fps);
            webCamTexture.Play();
            GetComponent<RectTransform>().sizeDelta = new Vector2(this.width, this.height);
            StartCoroutine(Init());
        }

        private void Update()
        {
            if (colors != null)
            {
                webCamTexture.GetPixels32(colors);

                //int width = webCamTexture.width;
                //int height = webCamTexture.height;
                //Color32 rc = new Color32(0, 0, 0, byte.MaxValue);

                //for (int x = 0; x < width; x++)
                //{
                //    for (int y = 0; y < height; y++)
                //    {
                //        Color32 c = colors[x + y * width];
                //        byte gray = (byte)(0.1f * c.r + 0.7f * c.g + 0.2f * c.b);
                //        rc.r = rc.g = rc.b = gray;
                //        colors[x + y * width] = rc;
                //    }
                //}
                cap_tex.SetPixels32(this.colors);
                cap_tex.Apply();
            }
            //GetComponent<RawImage>().texture = cap_tex;
        }
    }
}