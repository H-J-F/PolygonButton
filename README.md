# Polygon Button



##### 此工具是站在巨人的肩膀上完成，参考了宣雨松大佬的文章，并支持了3D UI

https://www.xuanyusong.com/archives/3492

##### 也从另一个开源库Unity-UI-Polygon中摘出多边形UI的脚本，并优化结合（减少GC，支持不规则的碰撞检测，可一键适配碰撞体到多边形的顶点）

https://github.com/CiaccoDavide/Unity-UI-Polygon



![image-20220514004101651](https://github.com/H-J-F/PolygonButton/blob/master/Docs/Images/1652462093238.gif)



### 使用方法：

只需在Canvas下创建对应的UI控件即可，支持3D UI，支持Mask、RectMask2D裁剪。

![image-20220514003622269](https://github.com/H-J-F/PolygonButton/blob/master/Docs/Images/image-20220514003622269.png)

创建的UI Polygon，如果要加上不规则的射线检测，需要自己手动添加PolygonCollider2D组件，点击UI Polygon组件Inspector面板上的Adapt Polygon按钮，可以让PolygonCollider2D自动适应到正多边形的顶点上

![image-20220514004101651](https://github.com/H-J-F/PolygonButton/blob/master/Docs/Images/image-20220514004101651.png)

*可以看到PolygonCollider2D的顶点已经贴合到UI Polygon的顶点*



### 注意事项：

1. 在Canvas的Overlay模式下，UI的Z坐标最好设置为大于等于0，否则会出现UI的部分区域射线检测不成功的情况，根据测试结果来看，应该是被背离鼠标的射线起点了。
2. 如果UI仅仅做水平或垂直翻转，不要改变欧拉角为180°，而是修改X轴或Y轴的缩放为-1，否则会射线检测失败（不检测UI背面）。
3. 在3D UI模式下，也就是Canvas是World Space模式时，UI可能有旋转，此时可能摄像机看到的是UI的背面，尽管没有旋转至180°，也会导致射线检测失败，此时要把Canvas的Graphic Raycaster组件取消勾选Ignore Reversed Graphics，这样无论UI如何旋转，都能正确检测到。
4. 如果UI有旋转，且摄像机为透视摄像机，则3D UI下的Mask或RectMask2D裁剪可能不是矩形的。
