# 一些C#项目的工具类库

## 依赖问题
### Flurl.Http 
使用的是Flurl.Http 3.2.0,它只依赖newtonsoft.json 12及以上版本.不使用Flurl.Http 4.0.0及以上版本,因为4.0.0版本开始依赖System.Text.Json和一堆系统库依赖,而不是newtonsoft.json.考虑到Unity3D项目的兼容性问题,所以使用3.2.0版本.
  