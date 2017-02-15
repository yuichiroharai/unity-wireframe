# unity-wireframe

Unityのメッシュをライン描画に変更するためのユーティリティ的なの。  
※Unity/C#を始めて3日目に作ったので、お作法とかよく分かってないかもしれません・・・

### Usage

#### `static int[] MakeIndices(int[] triangles)`

三角形の描画順にそのまま沿ったインデックス配列を生成します。

```c#
Mesh mesh = ...

int[] indices = Wireframe.MakeIndices(mesh.triangles);

mesh.SetIndices(indices, MeshTopology.Lines, 0);
```

#### `public static int[] MakeIndicesNoDuplicates(int[] triangles, Vector3[] vertices)`

頂点番号の組が重複しているものを除外してインデックス配列を生成します。  
※ただし、座標は同じでも頂点番号が異なる場合の重複は取り除けません。
```c#
Mesh mesh = ...

int[] indices = Wireframe.MakeIndicesNoDuplicates(mesh.triangles, mesh.vertices);

mesh.SetIndices(indices, MeshTopology.Lines, 0);
```

#### `public static int[] MakeIndicesOnlyEdges(int[] triangles, Vector3[] vertices, float thresholdAngle)`
メッシュのエッジ部分だけを描画するインデックス配列を生成します。  
いわゆる斜め線を取り除けます。

同じ辺を共有する2つの面の法線のなす角が`thresholdAngle`(単位：角度)以下だった場合、その辺が取り除かれます。

参考：  
https://github.com/mrdoob/three.js/blob/master/src/geometries/EdgesGeometry.js

```c#
Mesh mesh = ...

int[] indices = Wireframe.MakeIndicesOnlyEdges(mesh.triangles, mesh.vertices, 5);

mesh.SetIndices(indices, MeshTopology.Lines, 0);
```
