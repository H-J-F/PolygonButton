# Polygon Button



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

1. 在Canvas的Overlay模式下，UI的Z坐标最好设置为大于等于0，否则会出现UI的部分区域射线检测不成功的情况，根据测试结果来看，应该是被背离鼠标的射线起点了。
2. 如果UI仅仅做水平或垂直翻转，不要改变欧拉角为180°，而是修改X轴或Y轴的缩放为-1，否则会射线检测失败（不检测UI背面）。
3. 在3D UI模式下，也就是Canvas是World Space模式时，UI可能有旋转，此时可能摄像机看到的是UI的背面，尽管没有旋转至180°，也会导致射线检测失败，此时要把Canvas的Graphic Raycaster组件取消勾选Ignore Reversed Graphics，这样无论UI如何旋转，都能正确检测到。
4. 如果UI有旋转，且摄像机为透视摄像机，则3D UI下的RectMask2D裁剪可能不是矩形的，并且碰撞检测区域也会不适配，需要改用Mask才会得到正确的裁剪和射线检测区域。
