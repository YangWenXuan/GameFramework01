##Json解析库
####类结构
```
JToken： Json结构的基类，表示一个Json节点
┣ JCollection： 表示集合
┃┣ JObject： 表示一个Json对象结构
┃┗ JArray： 表示一个Json数组
┗ JValue： 表示值
  ┣ JNone： 表示不存在的值
  ┣ JNull： 表示存在，但是值为Null
  ┣ JBool： 布尔
  ┣ JInt： 整数
  ┣ JLong： 长整数
  ┣ JFloat： 单精度浮点数
  ┣ JDouble： 双精度浮点数
  ┗ JString： 字符串
```
---
####构造
```cs
JObject root = new JObject();
root["I"] = 1;
root["F"] = 2.5f;
root["S"] = "Hello";

JArray arr = new JArray();
for (int i = 0; i < 5; i++) {
	arr.Add(i);
}
root["Arr"] = arr;

JObject sub = new JObject();
sub["Name"] = "This is me!";
root["Child"] = sub;
```
---
####序列化
```cs
JToken target;//待序列化的对象，一般为JObject或JArray
string json = target.ToString();//转化成Json字符串
string formatJson = target.ToFormatString();//转化成可读的Json字符串

string jsonStr = JsonMapper.ToJson(item);//反射序列化对象，出于效率、兼容、稳定等考虑，不建议使用
```
---
####反序列化
```cs
JObject jobj1 = JObject.Parse(jsonStr);//反序列化Json对象，格式有误会抛异常
JObject jobj2 = JObject.OptParse(jsonStr);//反序列化Json对象，格式有误会返回一个新创建的空JObject

JArray jarr1 = JArray.Parse(jsonStr);//反序列化Json数组，格式有误会抛异常
JArray jarr2 = JArray.OptParse(jsonStr);//反序列化Json数组，格式有误会返回一个新创建的空JArray

TargetClass target = JsonMapper.ToObject<TargetClass>(jsonStr);//反射方式反序列化对象，出于效率、兼容、稳定等考虑，不建议使用
```
---
####使用

* **对子元素的访问**
	1. 可使用下标运算符访问子元素，`JObject`使用字符串访问，`JArray`使用整数访问，其他类型会抛异常。
	2. 使用`Get(path)`方法，可以根据路径来访问子元素。

* **类型转换**
下标访问返回的是`JToken`，可以使用类型转换方法返回所需的类型。
类型转换提供3组方法：
	1. `AsXXXX()`
	严格的类型转换，类型错误将会抛出异常。例如，对一个实际类型是`JInt`的对象使用`AsString()`将会抛出异常。
	2. `GetXXXX(), GetXXXX(def)`
	普通的类型转换，类型错误将会返回`null`。可以传默认值参数。
	3. `OptXXXX(), OptXXXX(def)`
	最宽松的类型转换（会尝试类型转换），类型错误将返回此类型的默认值。例如对一个`JString`对象使用`OptInt()`，会尝试将字符串转换成数字返回。如果转换失败，才返回默认值0。

* **枚举**
可以使用`AsEnum<T>`、`GetEnum<T>`、`OptEnum<T>`方便的将JToken转换成枚举。

* **遍历**
  对于`JObject`和`JArray`，都可以使用`foreach`遍历，对于`JArray`，还可以使用`Count`和下标进行遍历。

---
####使用实例

例如有如下Json：
```
{
	"A": 1,
	"B": "asd",
	"C": {
		"X": null
	},
	"D": [
		3.12,
		null
	],
	"E": true
}
```

```cs
JObject root = JObject.Parse(jsonStr);

int a1 = root["A"].AsInt();//正常返回1
string a2 = root["A"].AsString();//抛出异常
string a3 = root["A"].GetString();//返回null
string a4 = root["A"].OptString();//返回"1"
bool a5 = root["A"].OptBool();//返回false
bool a6 = root["A"].OptBool(true);//返回true

JObject d1 = root["D"].AsObject();//抛出异常

JObject d2 = root["D"].GetObject();//返回null
if (d2 != null) {
	//...
}

foreach (JToken t in root["D"].OptObject()) {//返回一个没有内容的JObject
	//此时循环不会进来
	//...
}

foreach (JToken t in root["D"].OptCollection()) {//返回类型实际是JArray，但是也可以遍历
	//...
}
```