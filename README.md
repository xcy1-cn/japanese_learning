# Japanese Learning Platform

Japanese Learning Platform 是一个基于 ASP.NET Core Web API、EF Core、MySQL、Vue3、Element Plus 和 Three.js 开发的日语学习平台。

系统分为 Admin 后台管理端和 Public 用户学习端。Admin 后台用于管理文章、句子、词汇、语法点和题目数据，并支持 Excel 批量导入；Public 用户端用于文章阅读、题目练习、学习记录保存和 Three.js 学习进度星图展示。

项目重点覆盖了后台内容管理、前后台分端设计、RESTful API、JWT 鉴权、EF Core 多表关系、分页查询、Public 接口缓存优化、Excel 批量导入、错误行反馈、学习记录本地持久化和 Three.js 可视化展示。

## 技术栈

### 后端

* ASP.NET Core Web API
* C#
* EF Core
* MySQL
* JWT Authentication
* IMemoryCache
* ClosedXML
* Swagger

### 前端

* Vue3
* TypeScript
* Vue Router
* Pinia
* Element Plus
* Axios
* ECharts
* Three.js
* Vite

### 数据库

* MySQL
* EF Core Code First / Migration

## 项目架构

项目整体分为三层：

1. Admin 后台管理端
   用于维护学习内容数据，包括文章、句子、词汇、语法点、题目和 Excel 批量导入。

2. Backend API 服务
   基于 ASP.NET Core Web API 提供 RESTful 接口，负责鉴权、业务处理、数据校验、缓存、分页查询和数据库访问。

3. Public 用户学习端
   面向学习用户，提供文章阅读、题目练习、学习记录和 Three.js 学习星图展示。

整体流程：

```text
Admin 后台
  ↓
内容 CRUD / Excel 导入
  ↓
ASP.NET Core Web API
  ↓
EF Core + MySQL
  ↓
Public 用户端读取学习内容
  ↓
阅读 / 答题 / 学习记录 / Three.js 星图
```

## 核心功能

### Admin 后台管理端

* 管理员登录
* JWT 鉴权
* 路由守卫
* Dashboard 数据统计
* 文章管理
* 句子管理
* 词汇管理
* 语法点管理
* 题目管理
* 句子-词汇关联管理
* 句子-语法点关联管理
* Excel 批量导入
* Excel 模板下载
* 导入错误行展示

### Public 用户学习端

* Public 首页
* 文章列表
* 文章详情
* 文章句子展示
* 题目列表
* 题目详情
* 答题提交
* 正确 / 错误反馈
* 正确答案与解析展示
* 阅读记录
* 答题记录
* 错题记录
* 学习进度页面
* Three.js 学习星图
* 星图专注模式

### 后端 API

* 用户认证接口
* Article CRUD
* Sentence CRUD
* Vocabulary CRUD
* GrammarPoint CRUD
* Question CRUD
* Public 公开文章接口
* Public 公开题目接口
* 答题提交接口
* Excel Import 接口
* 统一响应结构
* 分页响应结构
* Public GET 缓存

## 数据模型

核心实体包括：

* AdminUser
* Article
* Sentence
* Vocabulary
* GrammarPoint
* Question
* SentenceVocabulary
* SentenceGrammarPoint

主要关系：

```text
Article 1 —— N Sentence

Sentence N —— N Vocabulary
Sentence N —— N GrammarPoint

Question 可关联 Article / Sentence
```

其中 SentenceVocabulary 和 SentenceGrammarPoint 用于维护句子与词汇、句子与语法点之间的多对多关系。

## 核心亮点

### 1. Admin / Public 分端设计

项目不是单一后台 CRUD，而是拆分为后台内容管理端和前台学习展示端。Admin 负责内容维护，Public 负责学习展示和答题练习，结构更接近真实业务系统。

### 2. EF Core 多表关系设计

项目中实现了 Article 与 Sentence 的一对多关系，以及 Sentence 与 Vocabulary、Sentence 与 GrammarPoint 的多对多关系，并通过中间表维护关联数据。

### 3. JWT 登录鉴权

后台管理端使用 JWT 进行登录认证。前端通过 Axios 请求拦截器自动携带 token，后端通过 `[Authorize]` 限制后台接口访问。

### 4. 统一响应与分页结构

后端使用 `ApiResponse<T>` 统一接口响应格式，使用 `PagedResult<T>` 统一分页返回结构，方便前端统一处理接口数据。

### 5. Public GET 接口缓存优化

Public 用户端文章和题目相关 GET 接口使用 `IMemoryCache` 进行缓存优化，缓存时间为 5 分钟。查询使用 `AsNoTracking` 降低 EF Core 跟踪开销，并通过缓存 key 和版本号策略实现缓存失效。

### 6. Excel 批量导入

后台支持 Vocabulary、GrammarPoint 和 Question 三类数据的 Excel 批量导入。后端使用 ClosedXML 解析 `.xlsx` 文件，支持表头校验、逐行字段校验、错误行收集、批量写入和事务处理。前端提供导入页面、模板下载、上传控制和错误行表格展示。

### 7. 学习记录本地持久化

Public 用户端使用 Pinia + localStorage 保存阅读记录、答题记录和错题记录，使用户刷新页面后仍然可以保留学习进度。

### 8. Three.js 学习星图

项目使用 Three.js 将学习文章和答题记录映射为星图节点，通过节点和连线展示学习进度，增强 Public 端的视觉展示效果。

## Excel 批量导入说明

当前支持三类 Excel 导入：

```http
POST /api/Import/vocabularies
POST /api/Import/grammar-points
POST /api/Import/questions
```

上传方式：

```text
Content-Type: multipart/form-data
form-data key: file
file type: .xlsx
```

Vocabulary 模板：

```text
Word | Reading | Meaning | PartOfSpeech | Level
```

GrammarPoint 模板：

```text
Title | Explanation | Structure | Example | Level
```

Question 模板：

```text
ArticleId | SentenceId | Type | Stem | OptionA | OptionB | OptionC | OptionD | Answer | Explanation | Level
```

导入结果返回：

```json
{
  "code": 200,
  "message": "Vocabulary import completed.",
  "data": {
    "successCount": 2,
    "failCount": 1,
    "errors": [
      {
        "rowNumber": 4,
        "field": "Level",
        "message": "等级只能是 N5、N4、N3、N2、N1。"
      }
    ]
  }
}
```

## 缓存优化说明

Public GET 接口使用 `IMemoryCache` 缓存热点查询结果，减少重复访问数据库。缓存 key 根据接口路径和查询参数生成，并通过版本号策略处理缓存失效。

缓存处理流程：

```text
Public GET 请求
  ↓
生成缓存 key
  ↓
检查 IMemoryCache
  ↓
命中：直接返回缓存结果
  ↓
未命中：查询数据库
  ↓
写入缓存，设置 TTL
  ↓
返回结果
```

Admin 修改内容或导入新数据后，会触发缓存失效，保证 Public 用户端可以读取到最新内容。

## 本地运行方式

### 后端运行

```bash
dotnet restore
dotnet ef database update
dotnet run
```

后端默认地址：

```text
http://localhost:5251
```

### 前端运行

```bash
pnpm install
pnpm dev
```

或：

```bash
npm install
npm run dev
```

前端默认地址：

```text
http://localhost:5173
```

### 环境变量

前端 `.env` 示例：

```text
VITE_API_BASE_URL=http://localhost:5251/api
```

## 项目难点与解决方案

### 1. 多表关联管理

难点：句子需要关联多个词汇和多个语法点，如果直接写在一个表中会导致数据冗余。

解决方案：使用中间表 `SentenceVocabulary` 和 `SentenceGrammarPoint` 建立多对多关系，并提供关联和取消关联接口。

### 2. Public 接口性能优化

难点：Public 用户端文章列表、题目列表可能被频繁访问，重复查询会增加数据库压力。

解决方案：使用 `IMemoryCache` 缓存 Public GET 接口结果，并结合 `AsNoTracking` 优化 EF Core 查询性能。

### 3. Excel 导入错误反馈

难点：Excel 批量导入时不能只返回“导入失败”，否则用户不知道具体哪里出错。

解决方案：设计 `ImportResultDto` 和 `ImportErrorDto`，返回成功数量、失败行数、错误行号、错误字段和错误原因。

### 4. 前端文件上传控制

难点：Excel 上传需要在选择导入类型后再决定调用哪个接口。

解决方案：Element Plus `el-upload` 设置 `auto-upload=false`，用户点击“开始导入”后，前端根据导入类型创建 `FormData` 并调用对应接口。

## 后续优化计划

* 增加导入日志表，记录导入人、导入时间、成功数和失败数
* 增加 Excel 文件大小限制
* 增加重复数据校验
* 增加 Question 外键存在性校验
* 大文件分批导入
* ImportService 使用策略模式重构
* Redis 分布式缓存升级
* Controller-Service-Repository 分层优化
* 增加单元测试和集成测试
* 部署线上演示环境
