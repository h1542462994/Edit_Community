Edit_Community
============================================================================================
# 声明
+ 此软件是开源软件,禁止用于商业目的.
# 软件介绍
这是一款**作业展示器**软件,能够实现在班级教室电脑展示当天的功能.
+ 主要功能
	+ 将当天的作业显示在6个**RichTextBox**中.![screenshot](https://github.com/h1542462944/Edit_Community/blob/master/Edit_Community/Picture/main.jpg)
	+ 支持编辑文本的字号,字体,加粗,倾斜,颜色.**只需要选中文字并松开左键即可** ![screenshot](https://github.com/h1542462944/Edit_Community/blob/master/Edit_Community/Picture/editbox.jpg)
		+ 如需修改预设字体颜色,只要右键颜色即可,将会跳出`ColorDialog`
	+ 通过左键单击左下角区域来切换当前显示的文件夹.
		+ 如果要编辑模板,可以按如图所示操作![screenshot](https://github.com/h1542462944/Edit_Community/blob/master/Edit_Community/Picture/settings1.jpg)
	+ 背景:支持修改`背景颜色`和`显示的图片`.只需在`设置`>`个性化`的相关栏目进行设置.
	+ 天气:支持天气的显示,
+ 扩展功能
	+ 在根文件下修改`StartUp.xml`可以更改文件的缓存路径.</br>
	```xml
	<?xml version="1.0" encoding="utf-8"?>
	<!--这是设置1.0.3.0版本的本地文件,可以实现改变字段值自动保存的功能,支持保存数组.-->
	<StartUp filetype="settings" version="1.0.3.0">
		<!--LocalCache文件夹所在路径-->
		<RootFolder type="System.String">C:\User\App\Edit Community\LocalCache\</RootFolder>
		<!--是否为当前路径,若为True则指定%Root%LocalCache/为缓存文件夹-->
		<IsCurrentDomain type="System.Boolean">False</IsCurrentDomain>
	</StartUp>
	```		
# 注意事项
+ 代码下载:该软件引用HTLibrary库,点击跳转
[htlibrary https://github.com/h1542462944/HTLibrary](https://github.com/h1542462944/HTLibrary)
+ 软件更新需要HTStudioService服务.
[HTStudioService](https://github.com/h1542462944/HTStudioService)
+ 自动更新部署.
	1. 将Edit_CommunityUpdater放在Edit_Community\SoftWareCache\Updater文件夹内.
	2. 启动HTStudioService服务(需要部署到iis,或者本机).
	3. 重新生成HTLibrary,Edit_Community,Edit_CommunityUpdater(如果必要的话).
# 历史
+ *2017年8年12日 - 2017年9月29日* **117503445**发布[HomeWorker](https://github.com/117503445/HomeWorker)版本
+ *2017年10月1日 - 2017年12月6日* **h1542462994**发布[Edit](https://github.com/h1542462994/Edit)版本
+ *2017年12月7日 - 2018年5月15日* **h1542462994**发布[Edit_Community](https://github.com/h1542462944/Edit_Community)版本 </br>
# 更新
+ StartUpdate **2017年12月27日 - 2018年1月1日**
	+ *2017年12月27日* **1.0.0.0**
		+ `important` 发布第一个Edit版本.
	+ *2017年12月28日* **1.0.0.1**
		+ `debug` `window` 修复窗体初始化的问题,并且能够实现*普通,最大化和全屏*三种态的切换.
		+ `debug` `main` 解决输入法的问题(*需要安装英文语言*).
	+ *2017年12月30日* **1.0.0.2**
		+ `debug` `userlibrary` 解决UProperty首次创建xml文件时无法保存设置的问题.
		+ `improve` `main` 可以兼容老的文件版本[只以标准时间字符串命名].例如`20170621`
		+ `improve` `window` colorDialog以及对话框均实现点击其他区域自动退出.
		+ `improve` `window` colorPicker加入是否可以编辑不透明度的功能.
	+ *2017年12月31日]* **1.0.0.3**
		+ `new` `window` 加入了背景图片的功能,根据本地文件来读取.
		+ `improve` `winodw` 加入了调整背景颜色以及预设和历史颜色的功能(支持不透明度).
		+ `improve` `window` 加入两个调整页数的按钮是否显示.
		+ `improve` `edit` 加入editcolorHistory的功能,如果选择的颜色是新的颜色,那么将会加入到GridEditBox.
		+ `debug` `main` 解决日期至日期字符串没有加0的问题.
		+ `improve` `window` 所有颜色有关的控件将显示ARGB(RGB)的详细信息.
		+ `improve` `window` ColorPicker支持输入ARGB颜色来选取颜色.
	+ *2018年1月1日* **1.0.0.4**
		+ `improve` `extension` 加入编辑模板的功能.
+ Brush Update **2018年1月2日 - 2018年1月8日**
	+ *2018年1月2日-2018年1月7日* **1.1.0.0**
		+ `new` `brush` 加入笔刷图层新功能.
		+ `improve` `window` 加入鼠标无动作6s隐藏鼠标图标,自动隐藏菜单栏和两个按钮.的功能.
		+ `improve` `brush` 加入调整ink笔粗细,支持全屏清除笔迹的功能.
		+ `improve` `brush` 给MultiInkCanvasWithTool加上*依赖属性*IsTransparentStyle,以适应*PPT_Helper*的功能.
		+ `debug` `brush` 在荧光笔模式下调整粗细导致的问题.
		+ `debug` `main` 修复创建时间篡改的bug.
	+ *2018年1月8日* **1.1.0.1**
		+ `debug` `brush` 修复MultiInkCanvas关于橡皮大小产生的问题.
		+ `debug` `brush` 修复MultiInkCanvasWithTool调整粗细导致的问题.
+ Preview Update  **2018年1月9日 - 2018年1月13日**
	+ *2018年1月9日-2018年1月13日* **1.2.0.0**
		+ `debug` `brush` 支持笔刷有关的控件.
	+ *2018年1月26日* **1.2.0.1**
		+ `new` `extension` 加入天气预报功能.
		+ `new` `window` 加入**F11**全屏快捷键.
+ Unversil Update **2018年3月11日 - 2018年5月15日**
	+ *2018年3月11日* **1.3.0.1**
		+ `new` `edit` 加入调整字体的功能.
	+ *2018年3月15日* **1.3.2.0**
		+ `change` `htlibrary` 弃置Library库,使用HTLibrary库.
		+ `improve` `window` 控件使用平滑控件.
	+ *2018年3月16日* **1.3.2.1**
		+ `improve` `window` 背景采用无,图片和幻灯片三种模式,可以自己选择.
	+ *2018年3月18日* **1.3.4.0**
		+ `improve` `window` 设置下册新添4个`QuickButton`按钮.
	+ *2018年3月19日* **1.3.4.3**
		+ `new` `main` 加入文件路径自定义的功能 *仅限Release模式*
	+ *2018年3月21日* **1.3.4.3**
		+ `new` `extension` 天气预报回归.
	+ *2018年3月31日* **1.3.5.0**
		+ `new` `extension` 加入自动更新功能初版本(HTStudioService).
	+ *2018年4月1日* **1.3.5.1**
		+ `new` `extension` 加入通知系统(当前仅限于更新服务).
	+ *2018年4月3日* **1.3.5.3**
		+ `new` `extension` 加入通知推送系统.
	+ *2018年4月3日* **1.3.5.4**
		+ `new` `window`加入透明特性.
	+ *2018年4月26日* **1.3.5.6**
		+ `new` `brush`增加橡皮.
	+ *2018年4月27日* **1.3.5.8**
		+ `new` `extension`天气更新策略调整.
		+ `improve` `brush`橡皮功能改善，画笔时触屏下移除鼠标.
	+ *2018年5月13日* **1.3.6.0**
		+ `improve` `window`修复调整窗格大小导致的问题.
		+ `new` `extension`加入文档内容查看功能.
	+ *2018年5月14日* **1.3.6.1**
		+ `improve` 删除影响性能的所有功能
	+ *2018年5月15日* **null**
		+ `important` 正式结束Edit_Community的主流支持.(到高考为止)
# 开发人员
+ [117503445](https://github.com/117503445)提供天气服务的支持.
+ [h1542462994](https://github.com/h1542462944)主要的开发人员.
# 备注
+ 第一个使用本软件的用户:zyl
