# SimpleECS - 轻量级ECS框架

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/shangdibaozi/SimpleEcs/blob/main/LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](https://github.com/shangdibaozi/SimpleEcs/pulls)

> 一个简单高效的Entity-Component-System(ECS)框架，借鉴了[ecslite](https://github.com/Leopotam/ecslite)和[DragonEcs](https://github.com/DCFApixels/DragonECS)的设计思想

## 目录
- [核心概念](#核心概念)
- [快速开始](#快速开始)
- [Aspect使用指南](#aspect使用指南)
- [System开发](#system开发)
- [实体操作](#实体操作)
- [最佳实践](#最佳实践)
- [代码生成器](#代码生成器)

## 核心概念

### 🧩 Aspect
- 将World划分为多个独立的Aspect
- 每个Aspect管理自己的实体和组件
- 不同Aspect之间完全隔离，提高模块化

### 🧠 Component
- **数据组件**：存储实体状态（如位置、生命值）
- **标签组件**：标记实体特性（如Player、Enemy）
- 所有组件都是struct结构体

### 🚀 System
- 处理游戏逻辑的核心单元
- 通过Query筛选需要处理的实体
- 在Update中执行业务逻辑

### 🌐 World
- 管理所有Aspect的容器
- 全局访问点，通常整个应用只有一个World实例

## 快速开始

### 基本使用

### 创建Aspect
```csharp
// 定义AvatarAspect
public partial class AvatarAspect : Aspect<AvatarAspect>
{
    public AvatarAspect(Config cfg) : base(cfg) { }
}
```

### 创建World
```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class World
{
    public static AvatarAspect avatarAspect;

    public static void InitWorld()
    {
        // EntityCapacity | 初始实体容量
        // RecycledEntityCapacity | 实体回收池容量
        // CPoolCapacity | 组件池容量
        avatarAspect = new AvatarAspect(new SimpleEcs.Config
        {
            EntityCapacity = 128,
            RecycledEntityCapacity = 64,
            CPoolCapacity = 128,
        });
    }

    public static void ClearAspects()
    {
        avatarAspect.Clear();
        skillAspect.Clear();
    }
}

```

### 创建System
```csharp
public class LifeTimeSystem : IEcsSystem
{
    private Query query;
    
    public void OnInit() 
    {
        // 查询包含LifeTimeComponent的所有实体
        query = World.avatarAspect.Inc<LifeTimeComponent>().End();
    }

    public void OnUpdate() 
    {
        float dt = Time.deltaTime;
        foreach(var entity in query) 
        {
            ref var life = ref World.avatarAspect.lifeTimePool[entity];
            life.leftTime -= dt;
            if(life.leftTime <= 0) {
                World.avatarAspect.DestroyEntity(entity);
            }
        }
    }
}
```

### 系统组合
```csharp
// 系统执行顺序管理
public static void OnUpdate() 
{
    // 按指定顺序执行系统
    sys.Update<LifeTimeSystem, MoveSystem>();
    sys.Update<RenderSystem>();
}
```

## 实体操作

### 创建实体
```csharp
// 创建实体并添加初始组件
var entity = World.avatarAspect
    .NewEntityWith(new LifeTimeComponent { leftTime = 60 })
    .WithTag<TagPlayer>()
    .EndWith();
```

### 组件操作
```csharp
// 获取组件引用
ref var lifeComp = ref World.avatarAspect.lifeTimePool[entity];

// 删除组件
World.avatarAspect.lifeTimePool.Del(entity);

// 添加标签组件
World.avatarAspect.tagDestroy.Add(entity);

// 条件操作组件
World.avatarAspect.renderPool.TryAdd(entity); // 不存在时添加
World.avatarAspect.renderPool.TryDel(entity); // 存在时删除
```

### 查询实体
```csharp
// 包含LifeTimeComponent的所有实体
var q1 = World.avatarAspect.Inc<LifeTimeComponent>().End();

// 包含LifeTimeComponent和TagPlayer，且不包含TagDestroy的实体
var q2 = World.avatarAspect
    .Inc<LifeTimeComponent, TagPlayer>()
    .Exc<TagDestroy>()
    .End();
```

## 代码生成器

使用SimpleEcsSG代码生成器自动生成：
- Aspect的组件池字段
- 系统组合代码

```csharp
// 自动生成的SystemHelper
public static class SystemHelper 
{
    public static EcsSystemGroup CreateRootSystem() 
    {
        return new EcsSystemGroup()
            .Add<LifeTimeSystem>()
            .Add<MoveSystem>()
            .Add<RenderSystem>();
    }
}
```

### 创建Startup
```csharp
void Awake()
{
    // 初始化World
    World.InitWorld();
    // 创建系统
    RootSystem.Init();
}

// 游戏循环中更新系统
void Update() 
{
    RootSystem.OnUpdate();
}
```