StandardGizmo
====

![image](https://user-images.githubusercontent.com/12431632/33120625-027c2984-cfb6-11e7-8845-2dc0ceba467c.png)
![image](https://user-images.githubusercontent.com/12431632/33120648-1846cd8c-cfb6-11e7-9108-8cd18f38ec0a.png)

## Description
HoloLens向けのUnity Likeなオブジェクト操作系です

## Requirement
MixedRealityToolkit 2017.1.2

## Usage
ExternalからPackageをインストールしてください。

1. StandardGizmo/Prefabs/StandardGizomoをシーンに配置する。  

2. StandardGizmo/Scripts/StandardGizmoTargetを操作したいオブジェクトにアタッチする  

3. パラメータの見方  
![パラメータ](https://user-images.githubusercontent.com/12431632/33119067-2e6d4f1e-cfb1-11e7-8b37-60fdb2aa548c.png)  

Is Static Scale: ギズモを固定サイズにするかどうか  
Mover: MovementAxis Prefabを指定  
Movement Lerp Speed: 入力に対する移動量  
Arrow Scale: 矢印の大きさ  
Mov Hover Scale: Focus時の拡大率  
Use Negative Axis: 負方向にも矢印を伸ばすかどうか  
Mov Dragging Color: ドラッグ中の矢印の色  
x,y,z Color: 各軸の色  
Rotator: RotationRingを指定  
Ring Width: Ringの幅  
Ring Diameter: Ringの直径  
Rot Hover Scale: Focus時のリング幅拡大率  
Rot Dragging Color: ドラッグ中のリングの色  
roll,yaw,pitch Color: 各回転リングの色  

## Install
https://github.com/tattichan/StandardGizmo/blob/master/External

## Licence
[MIT](https://github.com/tcnksm/tool/blob/master/LICENCE)

## Author
Shinya Tachihara
[tcnksm](https://github.com/tcnksm)
