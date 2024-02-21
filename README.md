# unity-marsmon

## [Unity3d之 SkinnedMeshRenderer 渲染UI上](https://www.jianshu.com/p/2a5cb31e5945)

那这个时候怎么办？因为mesh的的渲染永远都是最早的，UI没法遮挡，最后想了一个折中方案，那就是把Mesh渲染到UI上，大家应该都知道UGUI的渲染依靠网格，那么我们把模型的mesh填充进去就可以了

那么问题又来了！正常的MeshRenderer是可以正常填充，SkinnedMeshRenderer也可以填充，但是！！如果SkinnedMeshRenderer用到了骨骼动画，怎么办？？因为它的动画是基于骨骼，不是基于网格点，所以在我们的网格点不变化的情况下 SkinnedMeshRenderer 渲染出来的永远是静态的。

## Reference

* [UnityAvater](https://github.com/zouchunyi/UnityAvater)
* 