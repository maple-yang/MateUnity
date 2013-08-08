﻿using UnityEngine;
using System.Collections.Generic;

using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[AddComponentMenu("M8/Core/UserData")]
public class UserData : MonoBehaviour {
    public const string UserDataLoadCallName = "OnUserDataLoad";

    [System.Serializable]
    public struct Data {
        public string name;
        public object obj;
    }

    public bool loadOnStart = true;
    public bool autoSave = true;

    protected string mKey = "ud";

    private Dictionary<string, object> mValues = null;

    private static UserData mInstance = null;

    public static UserData instance { get { return mInstance; } }

    public virtual void Load() {
        Data[] dat;

        dat = LoadData();

        mValues = new Dictionary<string, object>();
        foreach(Data datum in dat) {
            mValues.Add(datum.name, datum.obj);
        }

        SceneManager.RootBroadcastMessage(UserDataLoadCallName, this, SendMessageOptions.DontRequireReceiver);
    }
        
    public virtual void Save() {
        if(mValues != null) {
            List<Data> dat = new List<Data>(mValues.Count);
            foreach(KeyValuePair<string, object> pair in mValues)
                dat.Add(new Data() { name = pair.Key, obj = pair.Value });
            SaveData(dat.ToArray());
        }
    }

    public virtual void Delete() {
        if(mValues != null)
            mValues.Clear();

        PlayerPrefs.DeleteKey(mKey);
    }

    public virtual bool HasKey(string name) {
        return mValues != null && mValues.ContainsKey(name);
    }

    public virtual int GetInt(string name, int defaultValue = 0) {
        object ret;
        if(mValues != null && mValues.TryGetValue(name, out ret)) {
            if(ret is int)
                return System.Convert.ToInt32(ret);
        }

        return defaultValue;
    }

    public virtual void SetInt(string name, int value) {
        if(mValues == null) mValues = new Dictionary<string, object>();

        if(!mValues.ContainsKey(name))
            mValues.Add(name, value);
        else
            mValues[name] = value;
    }

    public virtual float GetFloat(string name, float defaultValue = 0) {
        object ret;
        if(mValues != null && mValues.TryGetValue(name, out ret)) {
            if(ret is float)
                return System.Convert.ToSingle(ret);
        }

        return defaultValue;
    }

    public virtual void SetFloat(string name, float value) {
        if(mValues == null) mValues = new Dictionary<string, object>();

        if(!mValues.ContainsKey(name))
            mValues.Add(name, value);
        else
            mValues[name] = value;
    }

    public virtual string GetString(string name, string defaultValue = "") {
        object ret;
        if(mValues != null && mValues.TryGetValue(name, out ret)) {
            if(ret is string)
                return System.Convert.ToString(ret);
        }

        return defaultValue;
    }

    public virtual void SetString(string name, string value) {
        if(mValues == null) mValues = new Dictionary<string, object>();

        if(!mValues.ContainsKey(name))
            mValues.Add(name, value);
        else
            mValues[name] = value;
    }

    public virtual void Delete(string name) {
        if(mValues != null)
            mValues.Remove(name);
    }

    protected virtual Data[] LoadData() {
        Data[] ret = null;

        string dat = PlayerPrefs.GetString(mKey, "");
        if(!string.IsNullOrEmpty(dat)) {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(dat));
            ret = (Data[])bf.Deserialize(ms);
        }

        return ret == null ? new Data[0] : ret;
    }

    protected virtual void SaveData(Data[] dat) {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, dat);
        PlayerPrefs.SetString(mKey, System.Convert.ToBase64String(ms.GetBuffer()));
    }
            
    void OnDisable() {
        if(autoSave)
            Save();
    }

    void OnDestroy() {
        if(mInstance == this)
            mInstance = null;
    }

    protected virtual void Awake() {
        if(mInstance == null)
            mInstance = this;
    }

    protected virtual void Start() {
        if(loadOnStart)
            Load();
    }
}
