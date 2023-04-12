/// <summary>
/// Aliasing Types and Namespaces
/// 导⼊命名空间可能会导致类型名称冲突。你可以只导⼊你需要的特定类型⽽不是导⼊整个命名空间，只需要给
/// 每个类型⼀个别名 
/// </summary>
using PropertyInfo2 = System.Reflection.PropertyInfo;

/// <summary>
/// 整个命名空间可以是别名
/// </summary>
using R = System.Reflection;

class MyProperInfo { PropertyInfo2? p; }

class MyProperInfo2 { R.PropertyInfo? p; }

/// <summary>
/// 外部命名空间和命名空间限定符
/// </summary>
/// 请参考书本章节 Chapter 2: Namespaces -> Advanced Namcespace Features
