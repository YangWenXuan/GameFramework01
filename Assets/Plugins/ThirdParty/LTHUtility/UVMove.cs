using UnityEngine;

public class UVMove : MonoBehaviour {
	public Vector2 Speed;
	public string PropertyName = "_MainTex";
	public Renderer Renderer;
	public bool SharedMaterial = true;

	private Vector2 _offset;

	void Awake() {
		if (this.Renderer == null) {
			this.Renderer = GetComponent<Renderer>();
		}
	}

	// Update is called once per frame
	void Update() {
		Material mat;
#if UNITY_EDITOR
		mat = this.Renderer.material;
#else
		mat= this.SharedMaterial ? this.Renderer.sharedMaterial : this.Renderer.material;
#endif
		this._offset += this.Speed * Time.deltaTime;
		mat.SetTextureOffset(this.PropertyName, this._offset);
	}
}
