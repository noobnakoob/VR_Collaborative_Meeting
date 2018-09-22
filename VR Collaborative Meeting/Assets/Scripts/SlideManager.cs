using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class SlideManager : MonoBehaviour {

    public Image slide_Image;


    private Texture2D current_Texture;

	// Use this for initialization
	void Start () {

        current_Texture = new Texture2D(0, 0);
        CreateTextureFromFile();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CreateTextureFromFile()
    {
        string path = "C:\\Users\\aleks\\Desktop\\2.pptx";

        if (File.Exists(path))
        {
            WWW www = new WWW(path);

            www.LoadImageIntoTexture(current_Texture);
            Sprite newSprite = Sprite.Create(current_Texture, new Rect(0,0,current_Texture.width, current_Texture.height), new Vector2(0.5f, 0.5f));
            slide_Image.sprite = newSprite;
        }
        else
            Debug.Log("Not");
    }
}
