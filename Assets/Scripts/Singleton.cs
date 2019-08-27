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
/// Manager�����ӿ�
/// </summary>
public interface IManagerFactory
{
    IManagerBase create();
}

/// <summary>
/// ����Unity��IManagerBase�������
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
/// ����UnityManager��Ĭ�Ϲ�����
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
/// ����Unity MonoBehaviour�����������Զ��幤����
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
/// Ĭ�ϵĻ���Unity MonoBehaviour�ĵ���
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class UnitySingleton<T> : UnitySingleton<T, UnityManagerFactory<T>>
    where T : UnityManager
{
}

/// <summary>
/// ���ʵĵ���
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