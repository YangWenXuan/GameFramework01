using System.IO;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using XPlugin.Localization;

namespace XPlugin.UI
{
    [DisallowMultipleComponent]
    public class ImageSpriteReassign : MonoBehaviour
    {
        public string atlasPath;
        public string spriteName;

        public bool autoResize = false;

        private Image image;

        private void Awake()
        {
            ReAssign();
        }


        [ContextMenu("ReAssign")]
        public void ReAssign()
        {
            if (image == null)
            {
                image = GetComponent<Image>();
            }
            if (image != null)
            {
                if (!string.IsNullOrEmpty(this.atlasPath))
                {//如果该路径不为空.
                    if (Application.isPlaying)
                    {
                        image.sprite = LResources.Load<Atlas>(this.atlasPath)?.GetSprite(this.spriteName);
                    }
                    else
                    {
                        image.sprite = Resources.Load<Atlas>(this.atlasPath)?.GetSprite(this.spriteName);
                    }
                }
                else
                {//如果该路径为空.
                    if (Application.isPlaying)
                    {
                        image.sprite = LResources.Load<Sprite>(this.spriteName);//加载根目录下的图片.
                    }
                    else
                    {
                        image.sprite = Resources.Load<Sprite>(this.spriteName);
                    }
                }
                if (this.autoResize)
                {
                    this.image.SetNativeSize();
                }
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Record")]
        //记录和移除引用.
        public void RecordAndRemoveReference()
        {
            RecordAndRemoveReference(this.gameObject, false);
            EditorUtility.SetDirty(gameObject);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        public static void RecordAndRemoveReference(GameObject g, bool removeReference)
        {
            Image image = g.GetComponent<Image>();
            if (image == null)
            {
                return;
            }
            Sprite spt = image.sprite;
            if (spt == null)
            {
                return;
            }
            var path = AssetDatabase.GetAssetPath(spt);
            TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                return;
            }
            var atlasName = importer.spritePackingTag;
            var sptName = spt.name;
            if (string.IsNullOrEmpty(atlasName))
            { //不在图集中
                sptName = UIUpdateUtil.GetResourcePath(path);
                if (sptName == null)
                {
                    return;
                }
            }
            else
            {
                if (atlasName.Contains("/"))
                { //可能使用/来处理本地化图集等变种
                    atlasName = atlasName.Substring(atlasName.LastIndexOf('/') + 1);
                }
            }
            var reassign = g.GetOrAddComponent<ImageSpriteReassign>();
            reassign.atlasPath = atlasName;
            reassign.spriteName = sptName;
            if (removeReference)
            {
                image.sprite = null;
            }
        }
#endif
    }
}