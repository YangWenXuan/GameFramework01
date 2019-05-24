using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using DG.Tweening.Plugins.Core.PathCore;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Path = System.IO.Path;

public class CreateFontFromSpriteSheet {
    [MenuItem("Assets/Create/CreateFontFromSpriteSheet")]
    public static void Go() {
        Texture fntTex = (Texture) Selection.activeObject;

        var fontName = fntTex.name;
        var path = AssetDatabase.GetAssetPath(fntTex);
        Sprite[] spts = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();

        var dirPath = Path.GetDirectoryName(path);

        var matPath = Path.Combine(dirPath, fontName + ".mat");
        var fntMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (fntMat == null) {
            fntMat = new Material(Shader.Find("UI/Default"));
            fntMat.mainTexture = fntTex;
            AssetDatabase.CreateAsset(fntMat, matPath);
        }


        var fontPath = Path.Combine(dirPath, fntTex.name + ".fontsettings");
        var font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
        if (font == null) {
            font = new Font(fntTex.name);
            AssetDatabase.CreateAsset(font, fontPath);
        }
        CharacterInfo[] infos=new CharacterInfo[spts.Length];
        for (var i = 0; i < spts.Length; i++) {
            var spt = spts[i];
            if (i == 0) {
                Debug.Log("rect:" + spt.rect);
                Debug.Log("textureRect:" + spt.textureRect);
                Debug.Log("textureRectOffset:" + spt.textureRectOffset);
            }
            var uvRect = spt.rect;
            uvRect.x = (float)uvRect.x / fntTex.width;
            uvRect.y = (float)uvRect.y / fntTex.height;
            uvRect.width = (float)uvRect.width / fntTex.width;
            uvRect.height =((float)uvRect.height / fntTex.height);
//            uvRect.y -= uvRect.height;
            
            var info = new CharacterInfo();
            info.index = spt.name[0];
            
            Debug.Log(spt.name[0]);
            info.glyphWidth = (int) spt.rect.width;
            info.glyphHeight = (int) spt.rect.height;
            info.advance = (int) (spt.textureRect.width + 1);
            Debug.Log(info.glyphWidth);
            Debug.Log(info.glyphHeight);
            info.uvBottomLeft = uvRect.min;
            info.uvTopRight = uvRect.max;
//            info.uvTopLeft = new Vector2(uvRect.xMin, uvRect.yMin);
//            info.uvBottomRight = new Vector2(uvRect.xMax, uvRect.yMin);
//            info.uvTopLeft = new Vector2(uvRect.xMin, uvRect.yMax);
//            info.uvBottomRight = uvRect.max; // new Vector2(uvRect.xMax, uvRect.yMax);
            infos[i] = info;
        }
        font.characterInfo=infos;
        
        EditorUtility.SetDirty(font);
        AssetDatabase.SaveAssets();

        //EditorUtility.DisplayDialog("提示", string.Format("功能未实现\nNot Implemented\n実装されていない\nKeine umsetzung"), "确定");
        EditorUtility.DisplayDialog("提示", string.Format("字体创建完成：{0}。\n注意：需要手动修改一下字体配置，否则退出后不会保存！", fontPath), "确定");
    }
}