using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;

#endif

[CreateAssetMenu(menuName = "Atlas")]
[ExecuteInEditMode]
public class Atlas : ScriptableObject, ISerializationCallbackReceiver {
#if UNITY_EDITOR
    public DefaultAsset Folder;
#endif

    public Sprite[] Sprites;
    public string[] SpriteNames;
    public Dictionary<string, Sprite> Dics;
    
    

#if UNITY_EDITOR
    [ContextMenu("Pack")]
    [Button()]
    public void Pack() {
        if (this.Folder == null) {
            return;
        }
        List<Sprite> spts = new List<Sprite>();
        List<string> sptnames = new List<string>();

        var folderPath = AssetDatabase.GetAssetPath(this.Folder);
        var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
        foreach (var f in files) {
            var s = AssetDatabase.LoadAssetAtPath<Sprite>(f);
            if (s != null) {
                spts.Add(s);
                sptnames.Add(s.name);
            }
        }
        this.SpriteNames = sptnames.ToArray();
        this.Sprites = spts.ToArray();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
#endif

    public void OnBeforeSerialize() {
    }

    public void OnAfterDeserialize() {
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
        Sprite ret;
        if (this.Dics.TryGetValue(spriteName, out ret)) {
            return ret;
        }
        return null;
    }
}