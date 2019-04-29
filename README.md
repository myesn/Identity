# Identity
Includes Organization、User management， relationalship is many-to-many

# 简介
自己写的身份库，有 4 个表分别是： Organization、User、OrganizationUser、UserToken

Organization 和 User 之间是 many-to-many 的关系

# 架构
这个库从架构上分为 Abstractions、EFCore、Provider(read)、MutableService(write)
可以看出是 读写分离 架构模式

# 有什么
1. 目前有一套相对完整的管理接口
2. 完整的测试用例
