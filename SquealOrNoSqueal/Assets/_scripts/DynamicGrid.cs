using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class DynamicGrid : MonoBehaviour {

    public int col, row;
    RectTransform parent;
    GridLayoutGroup grid;

    // Use this for initialization
    void Start () {
        parent = gameObject.GetComponent<RectTransform>();
        grid = gameObject.GetComponent<GridLayoutGroup>();

        
	}
	
	// Update is called once per frame
	void Update () {
        grid.cellSize = new Vector2(parent.rect.width / col, parent.rect.height / row);
    }
}
