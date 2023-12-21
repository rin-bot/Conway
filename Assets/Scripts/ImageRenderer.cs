using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageRenderer : MonoBehaviour
{
    public Texture2D image;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    public List<Vector3> GetPositions()
    {
        image = MakeGrey(image);
        Color[] pix = image.GetPixels();
        print(image.width);
        print(image.height);
        print(image.width * image.height);
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < image.width; i++)
        {
            for (int j = 0; j < image.height; j++)
            {
                Color col = image.GetPixel(i, j);
                if (col == Color.black)
                {
                    positions.Add(new Vector3(i, j, 0));
                }
            }
        }
        return positions;
    }
    private Texture2D MakeGrey(Texture2D tex)
    {
        var texColours = tex.GetPixels();
        print(texColours == null);
        for (int i = 0; i < texColours.Length; i++)
        {
            var greyValue = texColours[i].grayscale;
            greyValue = greyValue >= 0.5f ? 1 : 0;
            texColours[i] = new Color(greyValue, greyValue, greyValue, texColours[i].a);
        }
        tex.SetPixels(texColours);
        return tex;
    }
    void Update()
    {
        
    }
}
