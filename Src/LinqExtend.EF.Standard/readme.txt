1. 代码是基于EF core 6.0 开始编写的
2. 实测发现 EF core 2.2 并不是很推荐使用,
	- 因为翻译的sql为: select *, 
	- 内部的实现因为在是通过内存数据进行映射处理(猜测
	- 应该和手动的 select * 然后 toList 最后用 Automapper进行实体转行的效果一样
3. (其他的EF core 版本未知)
