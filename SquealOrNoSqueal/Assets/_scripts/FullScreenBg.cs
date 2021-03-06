﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenBg : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (!sr) return;

        float worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector3(worldScreenWidth / sr.sprite.bounds.size.x, worldScreenHeight / sr.sprite.bounds.size.y, 1);
    }
}
