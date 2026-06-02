# japanese_learning

Day 1 完成后端基础结构搭建：
1. 创建 ASP.NET Core Web API 项目结构。
2. 配置 MySQL 连接字符串。
3. 安装并配置 EF Core + Pomelo MySQL Provider。
4. 创建 AppDbContext。
5. 创建 AdminUser、Article、Sentence、Vocabulary、GrammarPoint、Question 等核心实体。
6. 创建 SentenceVocabulary、SentenceGrammarPoint 中间表。
7. 配置 Article 与 Sentence 的一对多关系。
8. 配置 Sentence 与 Vocabulary、GrammarPoint 的多对多关系。
9. 执行 EF Core Migration 并成功生成数据库表。
10. 通过 MySQL 和 DBeaver 验证数据库结构和表关系。

Day 2 的目标是完成核心业务资源的基础 CRUD Controller，包括：
1. ArticleController
2. SentenceController
3. VocabularyController
4. GrammarPointController
5. QuestionController

Controller → AppDbContext → EF Core → MySQL → Swagger 测试

Day 3｜JWT 登录鉴权总结:
1. JWT 的基本作用:前后端分离登录鉴权方案
2. Token 登录机制
3. PasswordHash 的意义
4. LoginRequest 的作用
5. AuthController 的写法
6. JWT Token 的生成方式
7. Program.cs 中 Authentication / Authorization 的配置
8. [Authorize] 如何保护接口

