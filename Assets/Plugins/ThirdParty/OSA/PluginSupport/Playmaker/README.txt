Playmaker support only works with Unity 2017.1.0f3 and up + Playmaker 1.9.0 an up
Instructions:
1. Back up your project!
2. Make sure the original Playmaker asset is imported/installed (it's not included, obviously. Here's their asset link: https://assetstore.unity.com/packages/tools/visual-scripting/playmaker-368)
It should be AT LEAST version 1.9.0
3. (Optional, if you want to use XML) Import their DataMaker add-on (https://hutonggames.fogbugz.com/default.asp?W1133). It doesn't work on Windows Phone!
4. Extract the PlaymakerSupport.zip somewhere OUTSIDE the project. In some unity versions, the packages inside won't be double-clickable, so you need to import them through drag'n'drop or toolbar
5. Import the PMSupport-Unity2017-1-f3.unitypackage. Wait for scripts to compile. A compile directive will be added as OSA_PLAYMAKER
6. (Optional, if #3 was also done) Import the DataMakerXMLTemplates-Unity2017-1-f3.unitypackage. Wait for scripts to compile.
7. Right-click on an UI element in the scene and choose UI->OSA. Follow the instructions in the wizard to set up some basic examples to get you started.
8. See the youtube tutorial linked in the manual (look for the "Playmaker support video" link at the beginning of the doc): https://1drv.ms/w/s!AkSVvbsn1QdDgwDDexlkzmXVMCIb