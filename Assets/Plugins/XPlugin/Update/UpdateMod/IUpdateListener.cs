using System;
using UnityEngine;
using System.Collections;

namespace XPlugin.Update {
	public interface IUpdateListener {

		/// <summary>
		/// 没有发现更新
		/// </summary>
		void OnPassUpdate();
		/// <summary>
		/// 发现完整更新时的回调
		/// </summary>
		/// <param name="info">更新信息</param>
		void OnFindMainUpdate(string info);
		/// <summary>
		/// 发现补丁更新的回调
		/// </summary>
		/// <param name="info">更新信息</param>
		/// <param name="size">更新大小(kb)</param>
		void OnFindPatchUpdate(string info, float size);
		/// <summary>
		/// 补丁下载进度
		/// </summary>
		/// <param name="patchName">当前补丁名称</param>
		/// <param name="currentProgress">当前补丁下载进度(0-1)</param>
		/// <param name="currentPatch">当前下载补丁序号</param>
		/// <param name="totalPatch">总补丁数</param>
		void OnPatchDownloadProgress(string patchName, float currentProgress, int currentPatch, int totalPatch);


		/// <summary>
		/// 补丁下载失败
		/// </summary>
		/// <param name="error"></param>
		void OnPatchDownloadFail(string error);


		/// <summary>
		/// 补丁下载完成
		/// </summary>
		void OnPatchDownloadFinish();

		/// <summary>
		/// 文件检查开始
		/// </summary>
		void OnVerifyStart();
		/// <summary>
		/// 文件检查失败
		/// </summary>
		void OnVerifyFail();
		/// <summary>
		/// 文件检查成功
		/// </summary>
		void OnVerifySuccess();

		/// <summary>
		/// 出错
		/// </summary>
		/// <param name="error"></param>
		void OnError(string error);


	}

}