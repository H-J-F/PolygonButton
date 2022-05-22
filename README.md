# Polygon Button
不规则的图形按钮，不规则的射线检测，支持3D UI，支持Mask、RectMask2D裁剪，使用类似引射线法的算法


##### 此工具是站在巨人的肩膀上完成，参考了宣雨松大佬的文章，但不同的是，已经彻底摆脱了2D Collider做射线检测，并支持了3D UI，且实现了相应的Editor组件

https://www.xuanyusong.com/archives/3492

##### 也从另一个开源库Unity-UI-Polygon中摘出多边形UI的脚本，并优化结合（减少GC，支持不规则的碰撞检测，可一键适配碰撞体到多边形的顶点）

https://github.com/CiaccoDavide/Unity-UI-Polygon



![image-20220514004101651](https://github.com/H-J-F/PolygonButton/blob/master/Docs/Images/1652552733196.gif)



### 原理：

在组件中保存一组点集数组，使用类似引射线法的算法判断射线的击中点是否在点集组成的区域内。相关资料可自行搜索。



### 使用方法：

只需在Canvas下创建对应的UI控件即可，支持3D UI，支持Mask、RectMask2D裁剪，只要勾选了Use Irregular Raycast，就会使用不规则的多边形区域作为射线检测区域（PolygonImage和PolygonRawImage组件没有这个复选框，原因是初衷就是认为这两个组件必须开启用不规则图形的射线检测，如果不想开启，应该去使用Image和RawImage）。


![image-20220514003622269](https://github.com/H-J-F/PolygonButton/blob/master/Docs/Images/image-20220514003622269.png)

![image-20220514004101651](https://github.com/H-J-F/PolygonButton/blob/master/Docs/Images/image-20220515020940.png)



### 注意事项：

1.本插件不使用 Collider2D 组件。
2.自定义和编辑 PolygonButton 的光线投射区域。 Ctrl + 鼠标左键向下删除一个点。
3.支持3D UI、UI旋转、Mask和RectMask2D。
4.使用“Screen-Space - Overlay”渲染模式时，UI位置的Z坐标不小于0。
5.当UI相机的Projection为“Perspective”且Canvas的渲染模式为“World Space”。或“Camera”时，建议您使用Mask而不是RectMask2D。 在这种情况下，RectMask2D 是不准确的。
6.使用 3D UI 时，您的相机可能会看到 UI 的反面，您应该打开附加到 Canvas 的“Graphic Raycaster”组件的“Ignore Reversed Graphics”切换开关。
7.同时支持Github上UI-Extensions的UI Polygon组件，我优化了UI Polygon，同时使PolygonButton支持UI Polygon。
8.如果UI仅仅做水平或垂直翻转，不要改变欧拉角为180°，而是修改X轴或Y轴的缩放为-1，否则会射线检测失败（不检测UI背面）。
