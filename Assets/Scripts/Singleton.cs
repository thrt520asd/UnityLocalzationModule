using UnityEngine;
using System;

public interface IManagerBase
{
    void init();
    void unInit();
    void onInit();
    void onUnInit();
    void load();
    void unload();
};

/// <summary>
/// Manager工厂接口
/// </summary>
public interface IManagerFactory
{
    IManagerBase create();
}

/// <summary>
/// 基于Unity的IManagerBase抽象基类
/// </summary>
public abstract class UnityManager : MonoBehaviour, IManagerBase
{
    public virtual void init() { }
    public virtual void unInit() { }
    public virtual void onInit() { }
    public virtual void onUnInit() { }
    public virtual void load() { }
    public virtual void unload() { }
    public virtual bool canDestroy { get { return false; } }
}

/// <summary>
/// 基于UnityManager的默认工厂类
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class UnityManagerFactory<T> : IManagerFactory where T : UnityManager
{
    public IManagerBase create()
    {
        GameObject go = new GameObject();
        go.name = typeof(T).ToString();
        go.hideFlags = HideFlags.None;

        return (T)go.AddComponent(typeof(T));
    }
}

/// <summary>
/// 基于Unity MonoBehaviour单例，允许自定义工厂类
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="F"></typeparam>
public abstract class UnitySingleton<T, F> : UnityManager
    where T : UnityManager where F : IManagerFactory, new()
{
    private bool _Initialized = false;
    public bool Initialized
    {
        get { return _Initialized; }
    }

    public static T create()
    {
        if (_instance == null && null == (_instance = FindObjectOfType<T>()))
        {
            IManagerFactory factory = new F();
            _instance = factory.create() as UnityManager;

            if (Application.isPlaying && !instance.canDestroy)
            {
                GameObject.DontDestroyOnLoad(_instance.gameObject);
            }
        }

        _instance.init();
        return (T)_instance;
    }

    public static void terminate()
    {
        if (_instance != null)
        {
            _instance.unInit();
            if (_instance.canDestroy)
            {
                DestroyImmediate(_instance.gameObject);
            }
            _instance = null;
        }
    }

    private static UnityManager _instance = null;
    public static T instance
    {
        get
        {
            return (null != _instance ? (T)_instance : create());
        }
    }

    public static bool IsInit { 
        get { return _instance != null; }
    }

    public sealed override void init()
    {
        if (!_Initialized)
        {
            onInit();
            _Initialized = true;
        }
    }

    public sealed override void unInit()
    {
        if (_Initialized)
        {
            onUnInit();
        }
        _Initialized = false;
    }
}

/// <summary>
/// 默认的基于Unity MonoBehaviour的单例
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class UnitySingleton<T> : UnitySingleton<T, UnityManagerFactory<T>>
    where T : UnityManager
{
}

/// <summary>
/// 普适的单例
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : IManagerBase
    where T : IManagerBase
{
    private bool _Initialized = false;
    public bool Initialized
    {
        get { return _Initialized; }
    }

    private static T _instance = default(T);
    public static T instance
    {
        get
        {
            return _instance != null ? _instance : create();
        }
    }

    public static T create()
    {
        if (_instance == null)
        {
            _instance = Activator.CreateInstance<T>();
            _instance.init();
        }

        return _instance;
    }

    public static void terminate()
    {
        if (_instance != null)
            _instance.unInit();
    }

    protected Singleton()
    {
    }
    
    public void init()
    {
        if (!_Initialized)
        {
            _instance.onInit();
            _Initialized = true;
        }
    }

    public void unInit()
    {
        if (Initialized && _instance != null)
        {
            _instance.onUnInit();
        }
        _Initialized = false;
        _instance = default(T);
    }

    public virtual void onInit() { }
    public virtual void onUnInit() { }
    public virtual void load() { }
    public virtual void unload() { }
}