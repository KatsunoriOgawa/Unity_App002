using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;
using System;

public class ImageLocalize : MonoBehaviour {
	void Awake () {
		Image img = gameObject.GetComponent<Image>();
		if (img) {
			string texturePath = "Images/" + Common.language + "/" + img.sprite.name;
			UnityEngine.Object obj = Resources.Load(texturePath);
			if (obj) {
				Texture2D tex = (Texture2D)Instantiate(obj);
				img.sprite.texture.LoadImage(tex.EncodeToPNG());
			}
		}
	}
}