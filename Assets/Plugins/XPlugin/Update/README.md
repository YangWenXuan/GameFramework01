[TOC]
# 资源更新与读取

这是一套基于`AssetBundle`的资源更新系统

## 功能
* 对Resources目录下的资源进行更新
* 对场景进行更新
* 对StreamingAssets目录下的资源进行更新

## 更新原理
对于Resources,Scene,StreamingAssets这种使用字符串加载的资源，可以优先加载外部资源，如果外部没有则加载内部资源，这样只需将要更新的资源下载到外部即可自动加载到更新资源。

## 用法
### 加载可更新的Resources
只需要将所有调用Resouces.Load的地方修改为UResouces.Load即可。

加载时将根据名字加载外部资源，如果外部没有，则调用Resource.Load加载内部资源。

UResource.Load用法和Resources.Load用法一致，并且同样提供了异步方式。

### 加载可更新的场景
在所有的LoadScene调用之前调用UResources.ReqScene先请求场景，如果返回值（或回调值）为true时即可继续加载场景。

请求场景时如果发现外部有同名的场景AssetBundle，则会加载该Bundle，之后在调用LoadScene则加载的是Bundle中的场景。

请求场景同样提供了异步方法。

### 加载可更新的StreamingAssets
需要读取StreamingAssets的地方使用UResouces.LoadStreamingAsset或UResouces.LoadStreamingAssetAsync即可，返回值为WWW对象，可能需要自行判断www.error。

## 限制
* 为了简洁，方便，易用，打包AssetsBundle时不会处理依赖，所有的依赖将会打入一个Bundle
* 资源加载时的标识是资源的名称（不包括扩展名），因此，一定要避免名称重复，如以下情况
> * A/Resources/res 和 B/Resources/res ，资源名称都是res
> * A/Resources/res 和 Scene/res ， 资源名和场景名都是res
> * A/Resources/res 和 StreamingAssets/res ， 资源名和流资源名都是res
> * StreamingAssets/A/res 和 StreamingAssets/B/res ， 流资源路径不一样但最终名称一样



你可能需要在`.gitignore`中添加`/BuildAB`

