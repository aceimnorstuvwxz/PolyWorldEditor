using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MaterialsController : MonoBehaviour {

	private int _materialIndex = 1; //start from 1
	public const int palette_unit = 4;
	public const int palette_width = 16;

	public GameObject material_fab;
	private GameObject _content;

	private Dictionary<int, GameObject> _materialsDict;

	private List<int> _selectedMaterials;

	private Dictionary<int, Color> _materialColors;
	private ColorPicker _colorPicker;
	private PolyWorldController _polyWorldController;

	private Texture2D _textureColorPalette;

	// Use this for initialization
	void Start ()
	{
		_content = GameObject.Find ("MaterialsContent");
		_materialsDict = new Dictionary<int, GameObject> ();
		_selectedMaterials = new List<int> ();
		_materialColors = new Dictionary<int, Color> ();
		_colorPicker = GameObject.Find ("ColorPicker").GetComponent<ColorPicker> ();
		_polyWorldController = GameObject.Find ("PolyWorldSpace").GetComponent<PolyWorldController> ();


		_textureColorPalette = new Texture2D(palette_unit * palette_width,
		                                     palette_unit * palette_width);
		
		for (int y = 0; y < _textureColorPalette.height; y++) {
			for (int x = 0; x < _textureColorPalette.width; x++) {
				_textureColorPalette.SetPixel(x, y, Color.magenta);
			}
		}
		_textureColorPalette.Apply();


		Invoke("OnNewMaterial", 0.5f);
	}

	public Texture2D GetPaletteTexture()
	{
		return _textureColorPalette;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void OnNewMaterial()
	{
		Debug.Log ("new material");

		int materialId = _materialIndex++;
		
		var material = Instantiate (material_fab) as GameObject;
		material.transform.SetParent (_content.transform);
		//		var rect = layer.GetComponent<RectTransform> ();
		//		_lineHeight = rect.sizeDelta.y * 1.2f;
		//		rect.localPosition = new Vector3 (0, -_lineHeight * _layerDict.Count, 0);
		
		_materialsDict.Add (materialId, material);
		var col = new Color (Random.value, Random.value, Random.value, 1f);
		_materialColors.Add (materialId, col);
		var materialController = material.GetComponent<MaterialLineController> ();
		materialController.material_id = materialId;
		materialController.SetColor (_materialColors [materialId]);
		
		RefreshContentLayout ();
		
		SelectMaterial (materialId);

		SetPaleeteColor (materialId, col);

	}

	void SetPaleeteColor(int id, Color cc)
	{
		int sy = id / palette_width;
		int sx = id % palette_width;

		for (int iy = sy * palette_unit; iy < sy* palette_unit + palette_unit; iy++) {
			for (int ix = sx * palette_unit; ix < sx * palette_unit + palette_unit; ix++) {
				_textureColorPalette.SetPixel(ix, iy, cc);
			}
		}

		_textureColorPalette.Apply ();
	}


	void RefreshContentLayout()
	{
		int heightCount = 0;
		float lineHeight = 0;
		foreach (GameObject obj in _materialsDict.Values) {
			var rect = obj.GetComponent<RectTransform> ();
			lineHeight = rect.sizeDelta.y * 1.2f;
			rect.localPosition = new Vector3 (0, -lineHeight * heightCount, 0);
			heightCount++;
		}
		
		var contentRect = _content.GetComponent<RectTransform> ();
		contentRect.sizeDelta = new Vector2 (contentRect.sizeDelta.x, _materialsDict.Count * lineHeight);
	}

	public void SelectMaterial(int materialId)
	{
		
		if (Input.GetKey ("right shift") || Input.GetKey ("left shift")) {
			//multiple selection
		} else {
			// de-select old ones
			foreach (int id in _selectedMaterials) {
				_materialsDict [id].GetComponent<MaterialLineController> ().SetSelection (false);
			}
			_selectedMaterials.Clear();
		}
		
		// select new
		if (!_selectedMaterials.Contains (materialId)) {
			_selectedMaterials.Add(materialId);
			_materialsDict [materialId].GetComponent<MaterialLineController> ().SetSelection (true);
			_colorPicker.SetColor(_materialColors[materialId]);

		}

	}

	void DeleteMaterial(int id)
	{
		var obj = _materialsDict [id];
		_materialsDict.Remove (id);
		Destroy (obj);
	}

	public void OnCombineMaterial()
	{
		Debug.Log ("conbine mat");
		if (_selectedMaterials.Count < 2) 
			return;

		int desId = _selectedMaterials[0];
		for (int i = 1; i < _selectedMaterials.Count; i++) {
			int srcId = _selectedMaterials[i];
			//TODO change all with srcId to desId in the scene

			DeleteMaterial(srcId);
		}
		_selectedMaterials.Clear ();
		RefreshContentLayout ();

		SelectMaterial (desId);
	}

	
	public void OnSetColor(Color color)
	{
		Debug.Log ("on set color, " + color.ToString ());

		foreach (int id in _selectedMaterials) {
			_materialColors[id] = color;
			_materialsDict[id].GetComponent<MaterialLineController>().SetColor (color);
			SetPaleeteColor(id, color);
		}
		
		_polyWorldController.RefreshMaterial (_selectedMaterials);
	}

	public Color OnGetColor()
	{
		Debug.Log ("OnGetColor, ");
		return Color.red;
		
	}

	public int GetBrushMaterial()
	{
		//TODO multiple selection with message
		if (_selectedMaterials.Count > 0) {
			int mat = _selectedMaterials [0];
			return mat;
		}

		return 0;
	}

	public Color GetMaterialColor(int id)
	{
		return _materialColors [id];
	}

	public Vector2 GetMaterialPaletteUV(int id)
	{
		float step = 1f / palette_width;
		float y = (id / palette_width + 0.5f) * step;
		float x = (id % palette_width + 0.5f) * step;

		return new Vector2(x, y);
	}

}
