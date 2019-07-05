using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine.U2D;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Atlas")]
[ExecuteInEditMode]
public class Atlas : ScriptableObject {
    public SpriteAtlas SpriteAtlas;

    public Sprite[] Sprites;
    [HideInInspector]
    public string[] SpriteNames;
    public Dictionary<string, Sprite> Dics;

#if  UNITY_EDITOR
    [ContextMenu("Pack")]
    [Button()]
    public void Pack() {
        if (this.SpriteAtlas == null) {
            return;
        }
        List<Sprite> spts = new List<Sprite>();
        List<string> sptnames = new List<string>();
        
        Sprite[] atlasSpts=new Sprite[this.SpriteAtlas.spriteCount];
        this.SpriteAtlas.GetSprites(atlasSpts);

        foreach (var f in atlasSpts) {
            var s = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(f.texture));
            if (s != null) {
                spts.Add(s);
                sptnames.Add(s.name);
            }
        }
        this.SpriteNames = sptnames.ToArray();
        this.Sprites = spts.ToArray();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif

    public void OnBeforeSerialize() {
    }

    public void Awake() {
        if (this.Sprites == null) {
            return;
        }
        this.Dics = new Dictionary<string, Sprite>();
        for (int i = 0; i < this.Sprites.Length; i++) {
            var sprite = this.Sprites[i];
            var name = this.SpriteNames[i];

            if (sprite == null) {
                Debug.LogError("null sprite for "+name);
                continue;
            }
            if (!Dics.ContainsKey(name)) {
                this.Dics.Add(name, sprite);
            }
        }
    }

    public Sprite GetSprite(string spriteName) {
        if (this.Dics == null) {
            Awake();
        }
        Sprite ret;
        if (this.Dics.TryGetValue(spriteName, out ret)) {
            return ret;
        }
        return null;
    }
}