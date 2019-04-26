using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace XUI {
	public class UIStack : UIPool {

		[Tooltip("是否为每个组创建一个物体")]
		public bool CreateGroupGameObject = true;

		private int CanvasSortOffset = 20;
		private Canvas ThisCanvas;

		[System.NonSerialized]
		public Stack<UIPage> Pages = new Stack<UIPage>();

		protected override void Awake() {
			base.Awake();
			this.ThisCanvas = GetComponent<Canvas>();
		}

		/// <summary>
		/// 叠加一个组
		/// </summary>
		/// <param name="prefabs"></param>
		public UIGroup Overlay(IEnumerable<GameObject> prefabs,params object[] args) {
			if (this.Pages.Count == 0) {
				this.Pages.Push(new UIPage());
			}
			var page = this.Pages.Peek();//返回栈顶元素(不删除).
			var lastGroup = page.Last;
			if (lastGroup != null) {
				lastGroup.Value.SendBeenOverlay();//不覆盖.
			}
			var group = new UIGroup(this,prefabs);//实例一个新组.
			
			page.AddLast(group);//添加到栈顶元素链表的末尾.
			UpdateCanvasOrder();
			group.SendEnterStack(args);//执行--OnUIShow()
			return group;
		}

		/// <summary>
		/// 覆盖一个组
		/// </summary>
		/// <param name="prefabs"></param>
		/// <param name="destroyBefore"></param>
		public UIGroup Cover(IEnumerable<GameObject> prefabs, bool destroyBefore,params object[] args) {
			//关闭底下的界面
			if (this.Pages.Count > 0) {//栈里面有页面.
				var lastPage = this.Pages.Peek();//返回栈顶元素但是不弹出该元素.(最后一个显示出来的页面)
				lastPage.SendBeenCover();//OnUIPause().
				foreach (var @group in lastPage) {
					foreach (var uiPageItem in @group) {
						uiPageItem.Instance.SetActive(false);//禁用上一个Group.
//						Despawn(uiPageItem.Instance, destroyBefore);//这里不使用回收，为了避免复用之后，退回该界面状态丢失
					}
				}
			}
			//生成新层
			UIPage newPage = new UIPage();
			this.Pages.Push(newPage);//入栈.

			var newGroup = new UIGroup(this, prefabs);
			newPage.AddLast(newGroup);
			UpdateCanvasOrder();
			newGroup.SendEnterStack(args);
			return newGroup;
		}

		/// <summary>
		/// 退回到上一界面
		/// </summary>
		public void Back(bool destroy = true) {
			if (this.Pages.Count == 0) {
				UnityEngine.Debug.LogError("没有任何界面可以回退");
				return;
			}
			//回收最前面的界面
			var lastPage = this.Pages.Peek();
			var lastGroup = lastPage.Last.Value;
			lastGroup.SendLeaveStack();
			lastGroup.Despawn(destroy);
			lastPage.RemoveLast();
			//显示上一个界面，如果当前页面还有组，则只需通知，如果当前页面没有组了,则需要把下一页全部显示出来
			if (lastPage.Count == 0) {
				this.Pages.Pop();
				if (this.Pages.Count == 0) {
					return;
				}
				lastPage = this.Pages.Peek();
				foreach (var g in lastPage) {
					foreach (var i in g) {
						i.Instance.SetActive(true);
					}
//					g.Spawn();
				}
				lastPage.SendDeCover();//注意：这里要先spawn在发消息
			} else {
				lastPage.Last.Value.SendDeOverlay();
			}
			UpdateCanvasOrder();
		}


		/// <summary>
		/// 退回到指定界面Group
		/// </summary>
		public void BackTo(UIGroup group, bool destroy=true) {
			if (this.Pages.Count == 0) {
				UnityEngine.Debug.LogError("fail");
				return;
			}
			var lastPage = this.Pages.Peek();
			var lastGroup = lastPage.Last.Value;
			bool decover = false;
			bool deoverlay = false;
			while (lastGroup != group) {//一直回收，直到指定界面
				lastGroup.SendLeaveStack();
				lastGroup.Despawn(destroy);
				lastPage.RemoveLast();
				deoverlay = true;
				if (lastPage.Count == 0) {
					this.Pages.Pop();
					if (this.Pages.Count == 0) {
						return;
					}
					lastPage = this.Pages.Peek();
					decover = true;
					deoverlay = false;
				}
				lastGroup = lastPage.Last.Value;
			}
			//达到指定页面
			if (decover) {//如果经历过翻页，需要生成它
//				lastGroup.Spawn();
				foreach (var i in lastGroup) {
					i.Instance.SetActive(true);
				}
				lastPage.SendDeCover();//注意：这里要先spawn在发消息
			}
			if (deoverlay) {//如果被叠加过，需要发送反叠加消息
				lastGroup.SendDeOverlay();
			}
			UpdateCanvasOrder();
		}

		public void UpdateCanvasOrder() {
//			var page = this.Pages.Peek();
//			if (page != null) {
//				int i = 0;
//				foreach (var group in page) {
//					foreach (var item in group) {
//						if (item.Instance.activeSelf) {
//							item.InstanceCanvas.overrideSorting = true;
//							item.InstanceCanvas.sortingLayerID = this.ThisCanvas.sortingLayerID;
//							item.InstanceCanvas.sortingOrder = i * this.CanvasSortOffset + this.ThisCanvas.sortingOrder;
//							i++;
//						}
//					}
//				}
//			}
		}

		internal UIItem PrefabToUIItem(GameObject prefab, GameObject parent) {
			var ins = Spawn(prefab);
			if (parent != null) {
				ins.transform.SetParent(parent.transform);
			}
//			Canvas canvas = ins.GetOrAddComponent<Canvas>();
//			ins.GetOrAddComponent<GraphicRaycaster>();
			return new UIItem(prefab, ins, null);
		}

		public virtual void OnUIGroupCreated(UIGroup uiGroup) {
		}



	}
}